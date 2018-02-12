using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.ListViewItem;

namespace Exoskeleton.Classes
{
    /// <summary>
    /// Base class for both Dialog and Form classes to inherit from.
    /// Manages form references, control references, serialization, etc.
    /// </summary>
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class FormLayoutBase : IDisposable
    {
        protected class ImageListPlaceHolder : Control {}

        protected IHostWindow host;

        protected string nativeContainer = "$host";
        protected Dictionary<string, Form> formDictionary;
        protected Dictionary<string, Control.ControlCollection> containerDictionary;
        protected Dictionary<string, Dictionary<string, Control>> controlDictionary;

        /// <summary>
        /// Button subclass to differentiate via type in apply payload phase
        /// </summary>
        public class DialogButton : Button { }
        
        public FormLayoutBase(IHostWindow host)
        {
            this.host = host;
            formDictionary = new Dictionary<string, Form>();
            containerDictionary = new Dictionary<string, Control.ControlCollection>();
            controlDictionary = new Dictionary<string, Dictionary<string, Control>>();
            containerDictionary[nativeContainer] = host.GetHostPanel().Controls;
        }

        public virtual void Dispose()
        {
            containerDictionary = null;
            controlDictionary = null;
            host = null;
        }

        private void InitializeContainer(string formName)
        {
            if (formName == nativeContainer)
            {
                // clear existing instead of reassign dictionary entry
                containerDictionary[formName].Clear();
            }
            else
            {
                // if form already exists, close it first
                if (formDictionary.ContainsKey(formName))
                {
                    formDictionary[formName].Close();
                }

                // create the new form
                Form f = new Form();
                f.Icon = host.GetForm().Icon;
                formDictionary[formName] = f;

                // reassign dictionary entry with new form controls ref
                containerDictionary[formName] = f.Controls;
            }

            // recreate control dictionary
            controlDictionary[formName] = new Dictionary<string, Control>();
        }

        public void InitializeForm(string formName, string formJson)
        {
            InitializeContainer(formName);

            if (formName == nativeContainer)
            {
                host.GetHostPanel().SuspendLayout();
                host.GetHostPanel().Visible = false;
            }
            else
            {
                formDictionary[formName].SuspendLayout();
            }

            // if any form level properties are supplied, apply them
            if (formJson != null) {
                if (formName == nativeContainer)
                {
                    JsonConvert.PopulateObject(formJson, host.GetForm());
                }
                else
                {
                    JsonConvert.PopulateObject(formJson, formDictionary[formName]);
                }

                JObject dyn = JsonConvert.DeserializeObject<JObject>(formJson);
                if (formName != nativeContainer && dyn["StartPosition"] == null)
                {
                    formDictionary[formName].StartPosition = FormStartPosition.CenterParent;
                }
            }
        }

        public void InitializeDialog(string formName, string formJson)
        {
            InitializeContainer(formName);

            if (formName == nativeContainer)
            {
                throw new Exception("Not allowed to initialize host form as dialog");
            }

            if (formJson != null)
            {
                JsonConvert.PopulateObject(formJson, formDictionary[formName]);
            }

            formDictionary[formName].FormBorderStyle = FormBorderStyle.FixedDialog;
            formDictionary[formName].ControlBox = false;
            formDictionary[formName].MinimizeBox = false;
            formDictionary[formName].MaximizeBox = false;
            formDictionary[formName].StartPosition = FormStartPosition.CenterParent;
            formDictionary[formName].SuspendLayout();
        }

        public void InitializeNative()
        {
            InitializeContainer(nativeContainer);
        }

        /// <summary>
        /// Generic implementation for adding a control to the dialog/form
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formName"></param>
        /// <param name="json"></param>
        /// <param name="parentName"></param>
        /// <param name="emitEvents"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        protected T AddControlInstance<T>(string formName, string json, string parentName = null, 
            bool emitEvents = false, dynamic payload = null) 
            where T : Control
        {
            T ctl = JsonConvert.DeserializeObject<T>(json);
            ctl.SuspendLayout();

            controlDictionary[formName][ctl.Name] = ctl;

            if (parentName == null)
            {
                containerDictionary[formName].Add(ctl);
            }
            else
            {
                controlDictionary[formName][parentName].Controls.Add(ctl);
            }

            // .net hack to avoid flickering listbox when losing focus
            if (ctl is ListBox)
            {
                (ctl as ListBox).Leave += (sender, args) => { ctl.Update(); };
            }

            if (emitEvents || ctl.GetType() == typeof(Button))
            {
                AddControlEvents<T>(formName, ctl);
            }

            if (payload != null)
            {
                ApplyControlPayload<T>(formName, ctl, payload);
            }

            return ctl;
        }

        /// <summary>
        /// Generic implementation for applying default events for certain control types
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formName"></param>
        /// <param name="control"></param>
        protected void AddControlEvents<T>(string formName, Control control) where T : Control
        {
            if (control.GetType() == typeof(Label))
            {
                ((Label)control).Click += (sender, args) =>
                {
                    host.PackageAndUnicast(String.Concat(formName, ".", control.Name, ".Click"),
                        GetControlPropertiesDynamic(formName, control.Name));
                };
            }

            // EventButton (Button)
            if (control.GetType() == typeof(Button) && control.GetType() != typeof(DialogButton))
            {
                ((Button) control).Click += (sender, args) =>
                {
                    Application.DoEvents();

                    host.PackageAndUnicast(String.Concat(formName, ".", control.Name , ".Click"), 
                        GenerateDynamicResponseObject(formName));
                };
            }

            if (control.GetType() == typeof(ListView))
            {
                ((ListView)control).Click += (sender, args) =>
                {
                    host.PackageAndUnicast(String.Concat(formName, ".", control.Name, ".Click"),
                        GetControlPropertiesDynamic(formName, control.Name));
                };
                ((ListView)control).DoubleClick += (sender, args) =>
                {
                    host.PackageAndUnicast(String.Concat(formName, ".", control.Name, ".DoubleClick"),
                        GetControlPropertiesDynamic(formName, control.Name));
                };
            }

            if (control.GetType() == typeof(TreeView))
            {
                ((TreeView)control).AfterSelect += (sender, args) =>
                {
                    host.PackageAndUnicast(String.Concat(formName, ".", control.Name, ".AfterSelect"),
                        GetControlPropertiesDynamic(formName, control.Name));
                };
                ((TreeView)control).NodeMouseClick += (sender, args) =>
                {
                    host.PackageAndUnicast(String.Concat(formName, ".", control.Name, ".NodeMouseClick"),
                        GetControlPropertiesDynamic(formName, control.Name));
                };
                ((TreeView)control).NodeMouseDoubleClick += (sender, args) =>
                {
                    host.PackageAndUnicast(String.Concat(formName, ".", control.Name, ".NodeMouseDoubleClick"),
                        GetControlPropertiesDynamic(formName, control.Name));
                };
            }

            if (control.GetType() == typeof(CheckBox))
            {
                ((CheckBox)control).CheckedChanged += (sender, args) =>
                {
                    host.PackageAndUnicast(String.Concat(formName, ".", control.Name, ".CheckedChanged"),
                        GetControlPropertiesDynamic(formName, control.Name));
                };
            }

            if (control.GetType() == typeof(RadioButton))
            {
                ((RadioButton)control).CheckedChanged += (sender, args) =>
                {
                    host.PackageAndUnicast(String.Concat(formName, ".", control.Name, ".CheckedChanged"),
                        GetControlPropertiesDynamic(formName, control.Name));
                };
            }

            if (control.GetType() == typeof(DateTimePicker))
            {
                ((DateTimePicker)control).ValueChanged += (sender, args) =>
                {
                    host.PackageAndUnicast(String.Concat(formName, ".", control.Name, ".ValueChanged"),
                        GetControlPropertiesDynamic(formName, control.Name));
                };
            }

            if (control.GetType() == typeof(MonthCalendar))
            {
                ((MonthCalendar)control).DateChanged += (sender, args) =>
                {
                    host.PackageAndUnicast(String.Concat(formName, ".", control.Name, ".DateChanged"),
                        GetControlPropertiesDynamic(formName, control.Name));
                };
            }

            if (control.GetType() == typeof(TextBox))
            {
                ((TextBox)control).TextChanged += (sender, args) =>
                {
                    host.PackageAndUnicast(String.Concat(formName, ".", control.Name, ".TextChanged"),
                        GetControlPropertiesDynamic(formName, control.Name));
                };
            }

            if (control.GetType() == typeof(MaskedTextBox))
            {
                ((MaskedTextBox)control).TextChanged += (sender, args) =>
                {
                    host.PackageAndUnicast(String.Concat(formName, ".", control.Name, ".TextChanged"),
                        GetControlPropertiesDynamic(formName, control.Name));
                };
            }

            if (control.GetType() == typeof(NumericUpDown))
            {
                ((NumericUpDown)control).ValueChanged += (sender, args) =>
                {
                    host.PackageAndUnicast(String.Concat(formName, ".", control.Name, ".ValueChanged"),
                        GetControlPropertiesDynamic(formName, control.Name));
                };
            }

            if (control.GetType() == typeof(ComboBox))
            {
                ((ComboBox)control).SelectedIndexChanged += (sender, args) =>
                {
                    host.PackageAndUnicast(String.Concat(formName, ".", control.Name, ".SelectedIndexChanged"),
                        GetControlPropertiesDynamic(formName, control.Name));
                };
            }

            if (control.GetType() == typeof(ListBox)) {
                ((ListBox) control).SelectedIndexChanged += (sender, args) =>
                {
                    host.PackageAndUnicast(String.Concat(formName, ".", control.Name , ".SelectedIndexChanged"), 
                        GetControlPropertiesDynamic(formName, control.Name));
                };
            }

            if (control.GetType() == typeof(PictureBox))
            {
                ((PictureBox)control).Click += (sender, args) =>
               {
                   host.PackageAndUnicast(String.Concat(formName, ".", control.Name, ".Click"),
                       GetControlPropertiesDynamic(formName, control.Name));
               };
            }

            if (control.GetType() == typeof(CheckedListBox))
            {
                // Normally this event fires before item is acutally checked.
                // To ensure our immediately checked/unchecked item is represented, will will
                // arrange our event handler with this hack.
                ItemCheckEventHandler handler = null;
                handler = (sender, args) =>
                {
                    CheckedListBox clb = (CheckedListBox)sender;
                    clb.ItemCheck -= handler;
                    clb.SetItemCheckState(args.Index, args.NewValue);
                    clb.ItemCheck += handler;

                    host.PackageAndUnicast(String.Concat(formName, ".", control.Name, ".ItemCheck"),
                        GetControlPropertiesDynamic(formName, control.Name));
                };

                ((CheckedListBox)control).ItemCheck += handler;
            }

            if (control.GetType() == typeof(DataGridView))
            {
                ((DataGridView)control).SelectionChanged += (sender, args) =>
                {
                    host.PackageAndUnicast(String.Concat(formName, ".", control.Name, ".SelectionChanged"),
                        GetControlPropertiesDynamic(formName, control.Name));
                };
            }

        }

        /// <summary>
        /// Generic implementation for apply payload to certain control types
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formName"></param>
        /// <param name="control"></param>
        /// <param name="payload"></param>
        protected void ApplyControlPayload<T>(string formName, Control control, dynamic payload) where T : Control
        {
            if (payload is string) {
                payload = JsonConvert.DeserializeObject<dynamic>(payload);
            }

            JObject pdef = (JObject) payload;

            if (control.GetType() == typeof(DialogButton))
            {
                if (formName != nativeContainer && pdef["DialogResult"] != null)
                {
                    DialogResult dr = 
                        (DialogResult) Enum.Parse(typeof(DialogResult), pdef["DialogResult"].ToString());

                    ((Button)control).Click += (sender, args) => {
                        formDictionary[formName].DialogResult = dr;
                    };
                }
            }

            // Panel may be associated with background Image from ImageList
            if (control.GetType() == typeof(Panel))
            {
                Panel p = (Panel)control;

                if (pdef["BackgroundImageList"] != null)
                {
                    if (pdef["BackgroundImageIndex"] != null)
                    {
                        p.BackgroundImage = host.ImageListDictionary[(string)pdef["BackgroundImageList"]].Images[(int)pdef["BackgroundImageIndex"]];
                    }

                    if (pdef["BackgroundImageKey"] != null)
                    {
                        p.BackgroundImage = host.ImageListDictionary[(string)pdef["BackgroundImageList"]].Images[(string)pdef["BackgroundImageKey"]];
                    }
                }
            }

            // ListBox payload may contain array of objects to databind.
            // This expects that 'DisplayMember' and 'ValueMember' properties are set.
            if (control.GetType() == typeof(ListBox))
            {
                if (pdef["DataSource"] != null)
                {
                    ListBox lb = (ListBox)control;

                    dynamic ds = pdef["DataSource"];

                    if ((bool?) pdef["DataSourceKeepSelection"] == true)
                    {
                        string oldSelection = lb.Text;
                        lb.DataSource = ds;
                        lb.Text = oldSelection;
                    }
                    else
                    {
                        lb.DataSource = ds;
                    }
                }
            }

            // ListBox payload may contain array of objects to databind.
            // This expects that 'DisplayMember' and 'ValueMember' properties are set.
            if (control.GetType() == typeof(ComboBox))
            {
                if (pdef["DataSource"] != null)
                {
                    ComboBox cb = (ComboBox)control;

                    dynamic ds = pdef["DataSource"];

                    if ((bool?)pdef["DataSourceKeepSelection"] == true)
                    {
                        string oldSelection = cb.Text;
                        cb.DataSource = ds;
                        cb.Text = oldSelection;
                    }
                    else
                    {
                        cb.DataSource = ds;
                    }
                }
            }

            if (control.GetType() == typeof(CheckedListBox))
            {
                if (pdef["CheckedIndices"] != null)
                {
                    CheckedListBox clb = (CheckedListBox)control;

                    dynamic dp = pdef["CheckedIndices"];
                    int[] items = ((JArray)pdef["CheckedIndices"]).Select(ci => (int)ci).ToArray();

                    // clear any already selected items
                    foreach (int i in clb.CheckedIndices)
                    {
                        clb.SetItemCheckState(i, CheckState.Unchecked);
                    }

                    // now select only those indices passed
                    foreach (int item in items)
                    {
                        clb.SetItemChecked(item, true);
                    }
                }

            }

            if (control.GetType() == typeof(PictureBox))
            {
                if (pdef["ImagePath"] != null) {
                    PictureBox clb = (PictureBox)control;

                    string imagePath = pdef["ImagePath"].ToString();

                    Image pic = Image.FromFile(imagePath);

                    clb.Image = pic;
                }
            }

            if (control.GetType() == typeof(DataGridView))
            {
                if (pdef["ObjectArray"] != null)
                {
                    List<dynamic> dynObjArray = ((JArray) pdef["ObjectArray"]).Select(gi => (dynamic) gi).ToList();
                    ((DataGridView) control).DataSource = dynObjArray;
                }
            }

            if (control.GetType() == typeof(ListView))
            {
                ListView ctl = (ListView)control;

                if (pdef["LargeImageList"] != null)
                {
                    ctl.LargeImageList = host.ImageListDictionary[(string)pdef["LargeImageList"]];
                }

                if (pdef["SmallImageList"] != null)
                {
                    ctl.SmallImageList = host.ImageListDictionary[(string)pdef["SmallImageList"]];
                }

                if (pdef["Columns"] != null)
                {
                    ctl.Columns.Clear();

                    JArray cols = (JArray) pdef["Columns"];

                    foreach(JObject col in cols)
                    {
                        ColumnHeader ch = new ColumnHeader();

                        if (col["Text"] != null) { ch.Text = (string)col["Text"]; }
                        if (col["Width"] != null) { ch.Width = (int)col["Width"]; }

                        ctl.Columns.Add(ch);
                    }
                }

                // Items array supports single dimension array where each array element is a different list item.
                // AppendItems is the same as above but the list will not be cleared before adding those (new) items.
                if (pdef["Items"] != null || pdef["AppendItems"] != null)
                {
                    JArray items;

                    if (pdef["AppendItems"] != null)
                    {
                        items = (JArray)pdef["AppendItems"];
                    }
                    else
                    {
                        ctl.Items.Clear();
                        items = (JArray)pdef["Items"];
                    }

                    foreach (dynamic item in items)
                    {
                        ListViewItem lvi = new ListViewItem();
                        if (item["Text"] != null) { lvi.Text = (string)item["Text"]; }
                        if (item["Tag"] != null) { lvi.Tag = item["Tag"]; }
                        if (item["ImageIndex"] != null) { lvi.ImageIndex = (int)item["ImageIndex"]; }

                        ctl.Items.Add(lvi);
                    }
                }

                // ItemArrays support 2-dimension array allowing multiple columns for each 'row'.
                // AppendItemArrays will not clear the listview before appending the new items.
                if (pdef["ItemArrays"] != null || pdef["AppendItemArrays"] != null)
                {
                    JArray items;

                    if (pdef["AppendItemArrays"] != null)
                    {
                        items = (JArray)pdef["AppendItemArrays"];
                    }
                    else
                    {
                        ctl.Items.Clear();
                        items = (JArray)pdef["ItemArrays"];
                    }

                    foreach (dynamic item in items)
                    {
                        ListViewItem lvi = new ListViewItem();

                        JArray cols = (JArray)item;
                        
                        if (item[0]["Text"] != null) { lvi.Text = (string)item[0]["Text"]; }
                        if (item[0]["Tag"] != null) { lvi.Tag = item[0]["Tag"]; }
                        if (item[0]["ImageIndex"] != null) { lvi.ImageIndex = (int)item[0]["ImageIndex"]; }

                        for(int idx=1; idx < cols.Count; idx++)
                        {
                            ListViewSubItem lvsi = new ListViewSubItem();
                            if (item[idx]["Text"] != null) { lvsi.Text = (string)item[idx]["Text"]; }
                            if (item[idx]["Tag"] != null) { lvsi.Tag = item[idx]["Tag"]; }
                            lvi.SubItems.Add(lvsi);
                        }
                        ctl.Items.Add(lvi);
                    }
                }

                if (pdef["SelectedIndices"] != null) {
                    JArray indices;
                    indices = (JArray)pdef["SelectedIndices"];
                    foreach (dynamic index in indices)
                    {
                        ctl.Items[(int) index].Selected = true;
                        ctl.Items[(int) index].Focused = true;
                    }
                    ctl.Focus();
                }
            }

            if (control.GetType() == typeof(TreeView))
            {
                TreeView ctl = (TreeView)control;

                if (pdef["ImageList"] != null)
                {
                    ctl.ImageList = host.ImageListDictionary[(string)pdef["ImageList"]];
                }

                if (pdef["Nodes"] != null)
                {
                    ctl.Nodes.Clear();

                    JArray nodes = (JArray) pdef["Nodes"];
                    foreach(JObject node in nodes)
                    {
                        addTreeNodeBranch(node, ctl.Nodes);
                    }
                }

                // payload for resolving text captions where fullpath is backslash delimited string of node text values
                if (pdef["GraftNodes"] != null)
                {
                    string parent = (string)pdef["GraftNodes"]["Parent"];
                    string[] parentPath = parent.Split('\\');
                    
                    JArray nodes = (JArray)(pdef["GraftNodes"]["Nodes"]);

                    TreeNode search = findTreeNodeByPathArray(ctl.Nodes, parentPath);

                    if (search != null)
                    {
                        foreach (JObject node in nodes)
                        {
                            addTreeNodeBranch(node, search.Nodes);
                        }

                        search.Expand();
                    }
                }

                // payload for resolving text captions where path is array of node text captions
                if (pdef["GraftPath"] != null)
                {
                    JArray parentPath = (JArray)pdef["GraftPath"]["ParentPath"];

                    JArray nodes = (JArray)(pdef["GraftPath"]["Nodes"]);

                    TreeNode search = findTreeNodeByPathArray(ctl.Nodes, parentPath.ToObject<string[]>());

                    if (search != null)
                    {
                        foreach (JObject node in nodes)
                        {
                            addTreeNodeBranch(node, search.Nodes);
                        }

                        search.Expand();
                    }
                }
            }
        }

        /// <summary>
        /// navigates a tree view by text path
        /// </summary>
        /// <param name="nodeCollection">Root TreeNodeCollection to base path resolving to.</param>
        /// <param name="parentPath">string[] of node text captions, indicating path of node to find.</param>
        /// <returns>Leaf TreeNode addressed by parentPath if found, or null if not.</returns>
        private TreeNode findTreeNodeByPathArray(TreeNodeCollection nodeCollection, string[] parentPath)
        {
            int count = parentPath.Length;

            foreach (string path in parentPath)
            {
                foreach (TreeNode node in nodeCollection)
                {
                    if (node.Text == path)
                    {
                        if (--count == 0)
                        {
                            return node;
                        }
                        else
                        {
                            // rereference to sub node collection and break current iteration
                            nodeCollection = node.Nodes;
                            break;
                        }
                    }
                }
            }

            return null;
        }

        // Private helper function for recursively populating hierarchical TreeView node collection
        private void addTreeNodeBranch(JObject rootObject, TreeNodeCollection rootNode)
        {
            TreeNode treeNode = new TreeNode();
            rootNode.Add(treeNode);

            if (rootObject["Text"] != null)
            {
                treeNode.Text = (string)rootObject["Text"];
            }

            if (rootObject["Tag"] != null)
            {
                treeNode.Tag = rootObject["Tag"];
            }

            if ((bool?) rootObject["IsExpanded"] == true)
            {
                treeNode.Expand();
            }

            if (rootObject["ImageIndex"] != null)
            {
                treeNode.ImageIndex = (int)rootObject["ImageIndex"];
            }

            if (rootObject["ImageKey"] != null)
            {
                treeNode.ImageIndex = (int)rootObject["ImageKey"];
            }

            if (rootObject["SelectedImageIndex"] != null)
            {
                treeNode.SelectedImageIndex = (int)rootObject["SelectedImageIndex"];
            }

            if (rootObject["SelectedImageKey"] != null)
            {
                treeNode.SelectedImageKey = (string)rootObject["SelectedImageKey"];
            }

            if (rootObject["Nodes"] != null)
            {
                JArray nodes = (JArray)rootObject["Nodes"];
                foreach(JObject node in nodes)
                {
                    addTreeNodeBranch(node, treeNode.Nodes);
                }
            }
        }

        /// <summary>
        /// Public api method for generic implementation for applying payload to certain controls.
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="controlName"></param>
        /// <param name="payload"></param>
        public void ApplyControlPayload(string formName, string controlName, string payload)
        {
            if (payload == null) return;

            Control control = controlDictionary[formName][controlName];
            dynamic payloadDynamic = JsonConvert.DeserializeObject(payload);

            if (control.GetType() == typeof(Panel))
            {
                ApplyControlPayload<Panel>(formName, control, payloadDynamic);
            }

            // Probably won't ever need this for dialogs since it should be set at definition, using generic
            if (control.GetType() == typeof(DialogButton))
            {
                ApplyControlPayload<DialogButton>(formName, control, payloadDynamic);
            }

            if (control.GetType() == typeof(ListView))
            {
                ApplyControlPayload<DialogButton>(formName, control, payloadDynamic);
            }

            if (control.GetType() == typeof(TreeView))
            {
                ApplyControlPayload<TreeView>(formName, control, payloadDynamic);
            }

            // Not 'really' needed since we have workaround to detect and set this with properties
            if (control.GetType() == typeof(CheckedListBox))
            {
                ApplyControlPayload<CheckedListBox>(formName, control, payloadDynamic);
            }

            if (control.GetType() == typeof(ListBox))
            {
                ApplyControlPayload<ListBox>(formName, control, payloadDynamic);
            }

            if (control.GetType() == typeof(ComboBox))
            {
                ApplyControlPayload<ComboBox>(formName, control, payloadDynamic);
            }

            // The only control type where this is currently needed for applying after definition.
            // More payload types may be needed in future so this api method leaves open this possibility.
            if (control.GetType() == typeof(DataGridView))
            {
                ApplyControlPayload<DataGridView>(formName, control, payloadDynamic);
            }
        }

        /// <summary>
        /// Applies property values to controls which have already been added.
        /// Can be used for separating control layout and data initialization.  
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="controlValues"></param>
        public void ApplyControlProperties(string formName, string controlValues)
        {
            var valuesDictionary = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(controlValues);

            foreach (var key in valuesDictionary.Keys)
            {
                var control = controlDictionary[formName][key];

                // CheckedListBox is exception that we need to programmatically assign
                if (control.GetType() == typeof(CheckedListBox))
                {
                    CheckedListBox clb = (CheckedListBox)control;

                    foreach (int i in clb.CheckedIndices)
                    {
                        clb.SetItemChecked(i, false);
                    }

                    JObject clbValues = valuesDictionary[key];
                    if (clbValues["CheckedIndices"] != null)
                    {
                        int[] items = JsonConvert.DeserializeObject<int[]>(clbValues["CheckedIndices"].ToString());
                        foreach (int item in items)
                        {
                            clb.SetItemChecked(item, true);
                        }
                    }

                    clbValues.Remove("CheckedIndices");
                }

                var json = JsonConvert.SerializeObject(valuesDictionary[key]);

                JsonConvert.PopulateObject(json, control);
            }
        }

        /// <summary>
        /// Applies a dialog/form defintion to the current dialog.
        /// Definitions allow representation of a series of controls, nesting, and property attributes
        /// within a single json object definition.
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="defintion"></param>
        public void ApplyDefinition(string formName, string defintion)
        {
            var definitionDict = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(defintion);

            foreach (var key in definitionDict.Keys)
            {
                JObject cdef = definitionDict[key];
                string type = cdef["Type"].ToString();

                string parent = null;
                if (!String.IsNullOrEmpty((string)cdef["Parent"]))
                {
                    parent = (string)cdef["Parent"];
                }

                JObject props = (JObject)cdef["Properties"];
                if (props["Name"] == null)
                {
                    props["Name"] = key;
                }

                JObject payload = (JObject)cdef["Payload"];

                bool emitEvents = false;
                if (cdef["EmitEvents"] != null)
                {
                    emitEvents = (bool)cdef["EmitEvents"];
                }

                string propJson = JsonConvert.SerializeObject(props);

                if (type == "ImageList")
                {
                    host.GetScriptInterface.Media.CreateImageList(key, propJson);
                    return;
                }

                switch (type)
                {
                    case "Button": AddControlInstance<Button>(formName, propJson, parent, emitEvents, payload); break;
                    case "CheckBox": AddControlInstance<CheckBox>(formName, propJson, parent, emitEvents, payload); break;
                    case "CheckedListBox": AddControlInstance<CheckedListBox>(formName, propJson, parent, emitEvents, payload); break;
                    case "ComboBox": AddControlInstance<ComboBox>(formName, propJson, parent, emitEvents, payload); break;
                    case "DataGridView": AddControlInstance<DataGridView>(formName, propJson, parent, emitEvents, payload); break;
                    case "DateTimePicker": AddControlInstance<DateTimePicker>(formName, propJson, parent, emitEvents, payload); break;
                    case "DialogButton": AddControlInstance<DialogButton>(formName, propJson, parent, emitEvents, payload); break;
                    case "Label": AddControlInstance<Label>(formName, propJson, parent, emitEvents, payload); break;
                    case "ListBox": AddControlInstance<ListBox>(formName, propJson, parent, emitEvents, payload); break;
                    case "MaskedTextBox": AddControlInstance<MaskedTextBox>(formName, propJson, parent, emitEvents, payload); break;
                    case "MonthCalendar": AddControlInstance<MonthCalendar>(formName, propJson, parent, emitEvents, payload); break;
                    case "NumericUpDown": AddControlInstance<NumericUpDown>(formName, propJson, parent, emitEvents, payload); break;
                    case "Panel": AddControlInstance<Panel>(formName, propJson, parent, emitEvents, payload); break;
                    case "PictureBox": AddControlInstance<PictureBox>(formName, propJson, parent, emitEvents, payload); break;
                    case "RadioButton": AddControlInstance<RadioButton>(formName, propJson, parent, emitEvents, payload); break;
                    case "TextBox": AddControlInstance<TextBox>(formName, propJson, parent, emitEvents, payload); break;
                    case "ListView": AddControlInstance<ListView>(formName, propJson, parent, emitEvents, payload); break;
                    case "TreeView": AddControlInstance<TreeView>(formName, propJson, parent, emitEvents, payload); break;
                    default: break;
                }
            }
        }

        /// <summary>
        /// Loads a definition from a text file and applies it to the specified named form.
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="filename"></param>
        public void LoadDefinition(string formName, string filename)
        {
            string definition = File.ReadAllText(filename);

            ApplyDefinition(formName, definition);
        }

        /// <summary>
        /// Used internally to construct response json to pass called when dismissing dialog.
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="dr"></param>
        /// <returns></returns>
        protected string GenerateDynamicResponse(string formName, DialogResult dr)
        {
            dynamic dyn = GenerateDynamicResponseObject(formName);

            dynamic result = new
            {
                Result = dr.ToString(),
                Controls = dyn
            };

            string json = JsonConvert.SerializeObject(result);

            return json;
        }

        /// <summary>
        /// API (and internally used for event handlers) method for obtaining representation of all control values.
        /// </summary>
        /// <param name="formName"></param>
        /// <returns></returns>
        public string GenerateDynamicResponse(string formName)
        {
            dynamic dyn = GenerateDynamicResponseObject(formName);

            string json = JsonConvert.SerializeObject(dyn);

            return json;
        }

        /// <summary>
        /// Used internally to generate dynamic control containing selected property values for all controls
        /// </summary>
        /// <param name="formName"></param>
        /// <returns></returns>
        protected dynamic GenerateDynamicResponseObject(string formName)
        {
            var dyn = new JObject();

            foreach (KeyValuePair<string, Control> controlKV in controlDictionary[formName])
            {
                Control control = controlKV.Value;
                dynamic cdyn = GetControlPropertiesDynamic(formName, control.Name);

                if (cdyn != null)
                {
                    dyn.Add(control.Name, JObject.FromObject(cdyn));
                }
            }

            return dyn;
        }

        /// <summary>
        /// Internal method used after all controls have been added, before 'show'ing the dialog or form.
        /// </summary>
        /// <param name="formName"></param>
        protected void FinalizeLayout(string formName)
        {
            foreach (KeyValuePair<string, Control> controlKV in controlDictionary[formName])
            {
                Control control = controlKV.Value;

                control.ResumeLayout(false);
            }

            if (formName == nativeContainer)
            {
                host.GetHostPanel().ResumeLayout(false);
                host.GetHostPanel().PerformLayout();
            }
            else
            {
                formDictionary[formName].ResumeLayout(false);
                formDictionary[formName].PerformLayout();
            }
        }

        /// <summary>
        /// API method for retrieving simplified representation of properties for a control.
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="controlName"></param>
        /// <returns></returns>
        public string GetControlProperties(string formName, string controlName)
        {
            dynamic cdyn = GetControlPropertiesDynamic(formName, controlName);
            string json = JsonConvert.SerializeObject(cdyn);
            return json;
        }

        /// <summary>
        /// Internal method for creating simplified dynamic object containing properties to return to user.
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="controlName"></param>
        /// <returns></returns>
        protected dynamic GetControlPropertiesDynamic(string formName, string controlName)
        {
            Control control = controlDictionary[formName][controlName];

            if (control.GetType() == typeof(TextBox))
            {
                TextBox t = (TextBox)control;

                dynamic tval = new { Name = t.Name, Text = t.Text };
                return tval;
            }

            if (control.GetType() == typeof(ListView))
            {
                ListView lv = (ListView)control;
                List<dynamic> selectedItemsDynamic = new List<dynamic>();
                foreach(ListViewItem item in lv.SelectedItems)
                {
                    selectedItemsDynamic.Add(new { item.Text, item.Tag, item.ImageIndex, item.ImageKey });
                }

                dynamic lvval = new {
                    SelectedItems = selectedItemsDynamic,
                    SelectedIndices = lv.SelectedIndices,
                    ItemCount = lv.Items.Count
                };
                return lvval;
            }

            if (control.GetType() == typeof(TreeView))
            {
                TreeView tv = (TreeView)control;

                if (tv.SelectedNode == null) return null;

                dynamic tvval = new {
                    tv.SelectedNode.Text,
                    tv.SelectedNode.Tag,
                    tv.SelectedNode.Level,
                    tv.SelectedNode.FullPath,
                    FullPaths = tv.SelectedNode.FullPath.Split('\\')
                };
                return tvval;
            }

            if (control.GetType() == typeof(MaskedTextBox))
            {
                MaskedTextBox t = (MaskedTextBox)control;

                dynamic tval = new { Name = t.Name, Text = t.Text };
                return tval;
            }

            if (control.GetType() == typeof(CheckBox))
            {
                CheckBox cb = (CheckBox)control;

                dynamic cbval = new { Name = cb.Name, Checked = cb.Checked };
                return cbval;
            }

            if (control.GetType() == typeof(RadioButton))
            {
                RadioButton rb = (RadioButton)control;

                dynamic rbval = new { Name = rb.Name, Checked = rb.Checked };
                return rbval;
            }

            if (control.GetType() == typeof(ListBox))
            {
                ListBox lb = (ListBox)control;

                dynamic lbval = new { Name = lb.Name, Selected = lb.SelectedItem };
                return lbval;
            }

            if (control.GetType() == typeof(CheckedListBox))
            {
                CheckedListBox clb = (CheckedListBox)control;

                dynamic clbval = new
                {
                    Name = clb.Name,
                    Selected = clb.CheckedItems,
                    CheckedIndices = clb.CheckedIndices
                };
                return clbval;
            }

            if (control.GetType() == typeof(ComboBox))
            {
                ComboBox cb = (ComboBox)control;

                dynamic cbval = new { Name = cb.Name, Selected = cb.SelectedItem };
                return cbval;
            }

            if (control.GetType() == typeof(NumericUpDown))
            {
                NumericUpDown nud = (NumericUpDown)control;

                dynamic nudval = new { Name = nud.Name, Value = nud.Value };
                return nudval;
            }

            if (control.GetType() == typeof(DateTimePicker))
            {
                DateTimePicker dtp = (DateTimePicker)control;
                TimeSpan ts = dtp.Value - new DateTime(1970, 1, 1);
                TimeSpan tsu = dtp.Value.ToUniversalTime() - new DateTime(1970, 1, 1);
                dynamic dtpval = new
                {
                    Name = dtp.Name,
                    Value = dtp.Value,
                    Date = dtp.Value.Date,
                    Short = dtp.Value.ToShortDateString(),
                    TimeOfDay = dtp.Value.TimeOfDay,
                    Epoch = (long)ts.TotalMilliseconds,
                    UniversalTime = dtp.Value.ToUniversalTime(),
                    UniversalEpoch = (long)tsu.TotalMilliseconds
                };
                return dtpval;
            }

            if (control.GetType() == typeof(MonthCalendar))
            {
                MonthCalendar mc = (MonthCalendar)control;
                TimeSpan ssu = mc.SelectionStart.ToUniversalTime() - new DateTime(1970, 1, 1);
                TimeSpan seu = mc.SelectionEnd.ToUniversalTime() - new DateTime(1970, 1, 1);
                dynamic dtpval = new
                {
                    Name = mc.Name,
                    SelStart = mc.SelectionStart.Date,
                    SelStartShort = mc.SelectionStart.Date.ToShortDateString(),
                    SelStartEpoch = (long)ssu.TotalMilliseconds,
                    SelEnd = mc.SelectionEnd.Date,
                    SelEndShort = mc.SelectionEnd.Date.ToShortDateString(),
                    SelEndEpoch = (long)seu.TotalMilliseconds
                };
                return dtpval;
            }

            if (control.GetType() == typeof(DataGridView))
            {
                DataGridView dgv = (DataGridView)control;

                List<int> selectedIndices = new List<int>();

                Int32 selectedRowCount = dgv.Rows.GetRowCount(DataGridViewElementStates.Selected);
                if (selectedRowCount > 0)
                {
                    for (int i = 0; i < selectedRowCount; i++)
                    {
                        selectedIndices.Add(dgv.SelectedRows[i].Index);
                    }
                }

                dynamic dgvval = new { Name = dgv.Name, SelectedIndices = selectedIndices };
                return dgvval;
            }

            return null;
        }

        /// <summary>
        /// Allows user to close a form
        /// </summary>
        /// <param name="formName"></param>
        public void Close(string formName)
        {
            if (formName == nativeContainer)
            {
                throw new Exception("To close the host form, use exoskeleton.shutdown() instead");
            }

            if (containerDictionary[formName] != null)
            {
                formDictionary[formName].Close();
            }
        }

        #region (API Exposed) Individual control "Add" methods for dialog/form composition

        /// <summary>
        /// Adds a CheckBox to a named dialog or form.
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="checkboxJson"></param>
        /// <param name="parentName"></param>
        /// <param name="emitEvents"></param>
        public void AddCheckBox(string formName, string checkboxJson, string parentName=null, 
            bool emitEvents=false)
        {
            AddControlInstance<CheckBox>(formName, checkboxJson, parentName, emitEvents);
        }

        /// <summary>
        /// Adds a CheckedListBox to a named dialog or form.
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="checklistJson"></param>
        /// <param name="parentName"></param>
        /// <param name="emitEvents"></param>
        /// <param name="payload"></param>
        public void AddCheckedListBox(string formName, string checklistJson, string parentName = null, 
            bool emitEvents=false, string payload=null)
        {
            AddControlInstance<CheckedListBox>(formName, checklistJson, parentName, emitEvents, payload);
        }

        /// <summary>
        /// Adds a ComboBox to a named dialog or form.
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="comboboxJson"></param>
        /// <param name="parentName"></param>
        /// <param name="emitEvents"></param>
        public void AddComboBox(string formName, string comboboxJson, string parentName=null, 
            bool emitEvents=false, string payload = null)
        {
            AddControlInstance<ComboBox>(formName, comboboxJson, parentName, emitEvents, payload);
        }

        /// <summary>
        /// Adds a DataGridView to a named dialog or form.
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="gridViewJson"></param>
        /// <param name="parentName"></param>
        /// <param name="emitEvents"></param>
        /// <param name="payload"></param>
        public void AddDataGridView(string formName, string gridViewJson, string parentName=null, 
            bool emitEvents=false, string payload=null)
        {
            AddControlInstance<DataGridView>(formName, gridViewJson, parentName, emitEvents, payload);
        }

        /// <summary>
        /// Adds a DateTimePicker control to a named dialog or form.
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="dateTimePicker"></param>
        /// <param name="parentName"></param>
        /// <param name="emitEvents"></param>
        public void AddDateTimePicker(string formName, string dateTimePicker, string parentName=null, 
            bool emitEvents=false)
        {
            AddControlInstance<DateTimePicker>(formName, dateTimePicker, parentName, emitEvents);
        }

        /// <summary>
        /// Adds a Button to a named Dialog and wires up an event to dismiss dialog with a result.
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="buttonJson"></param>
        /// <param name="parentName"></param>
        /// <param name="payload"></param>
        public void AddDialogButton(string formName, string buttonJson, string parentName=null, 
            string payload=null)
        {
            AddControlInstance<DialogButton>(formName, buttonJson, parentName, false, payload);
        }

        /// <summary>
        /// Add EventButton, which is just a Button. If emitEvents is true, payload should contain EventName
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="buttonJson"></param>
        /// <param name="parentName"></param>
        /// <param name="emitEvents"></param>
        /// <param name="payload"></param>
        public void AddEventButton(string formName, string buttonJson, string parentName = null, 
            bool emitEvents = false, string payload = null)
        {
            AddControlInstance<Button>(formName, buttonJson, parentName, true, payload);
        }

        /// <summary>
        /// Adds a Label to a named dialog or form.
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="labelJson"></param>
        /// <param name="parentName"></param>
        /// <param name="emitEvents"></param>
        public void AddLabel(string formName, string labelJson, string parentName=null, 
            bool emitEvents=false)
        {
            AddControlInstance<Label>(formName, labelJson, parentName, emitEvents);
        }

        /// <summary>
        /// Adds a ListBox to a named dialog or form.
        /// </summary>
        /// <param name="listboxJson"></param>
        /// <param name="parentName"></param>
        public void AddListBox(string formName, string listboxJson, string parentName=null, 
            bool emitEvents=false, string payload = null)
        {
            AddControlInstance<ListBox>(formName, listboxJson, parentName, emitEvents, payload);
        }

        public void AddListView(string formName, string listviewJson, string parentName=null,
            bool emitEvents=false, string payload = null)
        {
            AddControlInstance<ListView>(formName, listviewJson, parentName, emitEvents, payload);
        }

        /// <summary>
        /// Adds a MaskedTextBox to a named dialog or form.
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="maskedtextboxJson"></param>
        /// <param name="parentName"></param>
        /// <param name="emitEvents"></param>
        public void AddMaskedTextBox(string formName, string maskedtextboxJson, string parentName=null, 
            bool emitEvents=false)
        {
            AddControlInstance<MaskedTextBox>(formName, maskedtextboxJson, parentName, emitEvents);
        }

        /// <summary>
        /// Adds a MonthCalendar control to a named dialog or form.
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="monthcalendar"></param>
        /// <param name="parentName"></param>
        /// <param name="emitEvents"></param>
        public void AddMonthCalendar(string formName, string monthcalendar, string parentName=null, 
            bool emitEvents=false)
        {
            AddControlInstance<MonthCalendar>(formName, monthcalendar, parentName, emitEvents);
        }

        /// <summary>
        /// Adds a NumericUpDown control to a named dialog or form.
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="numericJson"></param>
        /// <param name="parentName"></param>
        /// <param name="emitEvents"></param>
        public void AddNumericUpDown(string formName, string numericJson, string parentName=null, 
            bool emitEvents=false)
        {
            AddControlInstance<NumericUpDown>(formName, numericJson, parentName, emitEvents);
        }

        /// <summary>
        /// Adds a Panel to a named dialog or form.
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="panelJson"></param>
        /// <param name="parentName"></param>
        /// <param name="emitEvents"></param>
        public void AddPanel(string formName, string panelJson, string parentName=null, 
            bool emitEvents=false)
        {
            AddControlInstance<Panel>(formName, panelJson, parentName, emitEvents);
        }

        /// <summary>
        /// Adds a PictureBox to a named dialog or form.
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="picboxJson"></param>
        /// <param name="parentName"></param>
        /// <param name="emitEvents"></param>
        /// <param name="payload"></param>
        public void AddPictureBox(string formName, string picboxJson, string parentName=null, 
            bool emitEvents=false, string payload = null)
        {
            AddControlInstance<PictureBox>(formName, picboxJson, parentName, emitEvents, payload);
        }

        /// <summary>
        /// Adds a RadioButton to a named dialog or form.
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="radiobuttonJson"></param>
        /// <param name="parentName"></param>
        /// <param name="emitEvents"></param>
        public void AddRadioButton(string formName, string radiobuttonJson, string parentName=null, 
            bool emitEvents=false)
        {
            AddControlInstance<RadioButton>(formName, radiobuttonJson, parentName);
        }

        /// <summary>
        /// Adds a TextBox to a named dialog or form.
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="textboxJson"></param>
        /// <param name="parentName"></param>
        /// <param name="emitEvents"></param>
        public void AddTextBox(string formName, string textboxJson, string parentName=null, 
            bool emitEvents=false)
        {
            AddControlInstance<TextBox>(formName, textboxJson, parentName, emitEvents);
        }

        public void AddTreeView(string formName, string treeviewJson, string parentName=null,
            bool emitEvents=false)
        {
            AddControlInstance<TreeView>(formName, treeviewJson, parentName, emitEvents);
        }
        #endregion

    }
}
