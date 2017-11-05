using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exoskeleton.Classes
{
    /// <summary>
    /// Base class for both Dialog and Form classes to inherit from.
    /// Manages form references, control references, serialization, etc.
    /// </summary>
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class FormLayoutBase : IDisposable
    {
        protected IHostWindow host;

        protected Dictionary<string, Form> formDictionary;
        protected Dictionary<string, Dictionary<string, Control>> controlDictionary;

        /// <summary>
        /// Button subclass to differentiate via type in apply payload phase
        /// </summary>
        public class DialogButton : Button { }
        
        public FormLayoutBase(IHostWindow host)
        {
            this.host = host;
            formDictionary = new Dictionary<string, Form>();
            controlDictionary = new Dictionary<string, Dictionary<string, Control>>();
        }

        public virtual void Dispose()
        {
            formDictionary = null;
            controlDictionary = null;
            host = null;
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
                formDictionary[formName].Controls.Add(ctl);
            }
            else
            {
                controlDictionary[formName][parentName].Controls.Add(ctl);
            }

            if (emitEvents)
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
            JObject pdef = (JObject) payload;

            if (control.GetType() == typeof(DialogButton))
            {
                if (pdef["DialogResult"] != null)
                {
                    DialogResult dr = 
                        (DialogResult) Enum.Parse(typeof(DialogResult), pdef["DialogResult"].ToString());

                    ((Button)control).Click += (sender, args) => {
                        formDictionary[formName].DialogResult = dr;
                    };
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

            // Probably won't ever need this for dialogs since it should be set at definition, using generic
            if (control.GetType() == typeof(DialogButton))
            {
                ApplyControlPayload<DialogButton>(formName, control, payloadDynamic);
            }

            // Not 'really' needed since we have workaround to detect and set this with properties
            if (control.GetType() == typeof(CheckedListBox))
            {
                ApplyControlPayload<CheckedListBox>(formName, control, payloadDynamic);
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

                switch (type)
                {
                    case "Button": AddControlInstance<Button>(formName, propJson, parent, emitEvents, payload); break;
                    case "Panel": AddControlInstance<Panel>(formName, propJson, parent, emitEvents, payload); break;
                    case "Label": AddControlInstance<Label>(formName, propJson, parent, emitEvents, payload); break;
                    case "CheckBox": AddControlInstance<CheckBox>(formName, propJson, parent, emitEvents, payload); break;
                    case "RadioButton": AddControlInstance<RadioButton>(formName, propJson, parent, emitEvents, payload); break;
                    case "TextBox": AddControlInstance<TextBox>(formName, propJson, parent, emitEvents, payload); break;
                    case "MaskedTextBox": AddControlInstance<MaskedTextBox>(formName, propJson, parent, emitEvents, payload); break;
                    case "ListBox": AddControlInstance<ListBox>(formName, propJson, parent, emitEvents, payload); break;
                    case "ComboBox": AddControlInstance<ComboBox>(formName, propJson, parent, emitEvents, payload); break;
                    case "NumericUpDown": AddControlInstance<NumericUpDown>(formName, propJson, parent, emitEvents, payload); break;
                    case "DateTimePicker": AddControlInstance<DateTimePicker>(formName, propJson, parent, emitEvents, payload); break;
                    case "MonthCalendar": AddControlInstance<MonthCalendar>(formName, propJson, parent, emitEvents, payload); break;
                    case "DataGridView": AddControlInstance<DataGridView>(formName, propJson, parent, emitEvents, payload); break;
                    case "DialogButton": AddControlInstance<DialogButton>(formName, propJson, parent, emitEvents, payload); break;
                    case "CheckedListBox": AddControlInstance<CheckedListBox>(formName, propJson, parent, emitEvents, payload); break;
                    case "PictureBox": AddControlInstance<PictureBox>(formName, propJson, parent, emitEvents, payload); break;
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
                control.PerformLayout();
            }

            formDictionary[formName].ResumeLayout(false);
            formDictionary[formName].PerformLayout();
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
            if (formDictionary[formName] != null)
            {
                formDictionary[formName].Close();
            }
        }

        #region Individual control "Add" methods for dialog/form composition

        /// <summary>
        /// Adds a Panel to a named dialog or form.
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="panelJson"></param>
        /// <param name="parentName"></param>
        /// <param name="emitEvents"></param>
        public void AddPanel(string formName, string panelJson, string parentName=null, bool emitEvents=false)
        {
            AddControlInstance<Panel>(formName, panelJson, parentName, emitEvents);
        }

        public void AddPictureBox(string formName, string picboxJson, string parentName=null, 
            bool emitEvents=false, string payload = null)
        {
            AddControlInstance<PictureBox>(formName, picboxJson, parentName, emitEvents, payload);
        }

        /// <summary>
        /// Adds a Label to a named dialog or form.
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="labelJson"></param>
        /// <param name="parentName"></param>
        /// <param name="emitEvents"></param>
        public void AddLabel(string formName, string labelJson, string parentName=null, bool emitEvents=false)
        {
            AddControlInstance<Label>(formName, labelJson, parentName, emitEvents);
        }

        /// <summary>
        /// Adds a CheckBox to a named dialog or form.
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="checkboxJson"></param>
        /// <param name="parentName"></param>
        /// <param name="emitEvents"></param>
        public void AddCheckBox(string formName, string checkboxJson, string parentName=null, bool emitEvents=false)
        {
            AddControlInstance<CheckBox>(formName, checkboxJson, parentName, emitEvents);
        }

        /// <summary>
        /// Adds a RadioButton to a named dialog or form.
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="radiobuttonJson"></param>
        /// <param name="parentName"></param>
        /// <param name="emitEvents"></param>
        public void AddRadioButton(string formName, string radiobuttonJson, string parentName=null, bool emitEvents=false)
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
        public void AddTextBox(string formName, string textboxJson, string parentName=null, bool emitEvents=false)
        {
            AddControlInstance<TextBox>(formName, textboxJson, parentName, emitEvents);
        }

        /// <summary>
        /// Adds a MaskedTextBox to a named dialog or form.
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="maskedtextboxJson"></param>
        /// <param name="parentName"></param>
        /// <param name="emitEvents"></param>
        public void AddMaskedTextBox(string formName, string maskedtextboxJson, string parentName=null, bool emitEvents=false)
        {
            AddControlInstance<MaskedTextBox>(formName, maskedtextboxJson, parentName, emitEvents);
        }

        /// <summary>
        /// Adds a ListBox to a named dialog or form.
        /// </summary>
        /// <param name="listboxJson"></param>
        /// <param name="parentName"></param>
        public void AddListBox(string formName, string listboxJson, string parentName=null, bool emitEvents=false)
        {
            AddControlInstance<ListBox>(formName, listboxJson, parentName, emitEvents);
        }

        /// <summary>
        /// Adds a ComboBox to a named dialog or form.
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="comboboxJson"></param>
        /// <param name="parentName"></param>
        /// <param name="emitEvents"></param>
        public void AddComboBox(string formName, string comboboxJson, string parentName=null, bool emitEvents=false)
        {
            AddControlInstance<ComboBox>(formName, comboboxJson, parentName, emitEvents);
        }

        /// <summary>
        /// Adds a NumericUpDown control to a named dialog or form.
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="numericJson"></param>
        /// <param name="parentName"></param>
        /// <param name="emitEvents"></param>
        public void AddNumericUpDown(string formName, string numericJson, string parentName=null, bool emitEvents=false)
        {
            AddControlInstance<NumericUpDown>(formName, numericJson, parentName, emitEvents);
        }

        /// <summary>
        /// Adds a DateTimePicker control to a named dialog or form.
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="dateTimePicker"></param>
        /// <param name="parentName"></param>
        /// <param name="emitEvents"></param>
        public void AddDateTimePicker(string formName, string dateTimePicker, string parentName=null, bool emitEvents=false)
        {
            AddControlInstance<DateTimePicker>(formName, dateTimePicker, parentName, emitEvents);
        }

        /// <summary>
        /// Adds a MonthCalendar control to a named dialog or form.
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="monthcalendar"></param>
        /// <param name="parentName"></param>
        /// <param name="emitEvents"></param>
        public void AddMonthCalendar(string formName, string monthcalendar, string parentName=null, bool emitEvents=false)
        {
            AddControlInstance<MonthCalendar>(formName, monthcalendar, parentName, emitEvents);
        }

        /// <summary>
        /// Adds a Button to a named Dialog and wires up an event to dismiss dialog with a result.
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="buttonJson"></param>
        /// <param name="parentName"></param>
        /// <param name="payload"></param>
        public void AddDialogButton(string formName, string buttonJson, string parentName=null, string payload=null)
        {
            if (payload == null)
            {
                DialogButton btn = AddControlInstance<DialogButton>(formName, buttonJson, parentName, false, null);
            }
            else
            {
                dynamic payloadDynamic = JsonConvert.DeserializeObject<dynamic>(payload);

                // DialogResult is not a js event, we will handle its DialogResult in apply payload phase
                DialogButton btn = AddControlInstance<DialogButton>(formName, buttonJson, parentName, false, payloadDynamic);
            }
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
            if (payload == null)
            {
                Button btn = AddControlInstance<Button>(formName, buttonJson, parentName);
            }
            else
            {
                dynamic payloadDynamic = JsonConvert.DeserializeObject<dynamic>(payload);
                Button btn = AddControlInstance<Button>(formName, buttonJson, parentName, true, payloadDynamic);
            }
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
            if (payload == null)
            {
                CheckedListBox clb = AddControlInstance<CheckedListBox>(formName, checklistJson, parentName, emitEvents);
            }
            else
            {
                dynamic payloadDynamic = JsonConvert.DeserializeObject<dynamic>(payload);

                CheckedListBox clb = AddControlInstance<CheckedListBox>(formName, checklistJson, parentName, 
                    emitEvents, payloadDynamic);
            }
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
            if (payload == null)
            {
                AddControlInstance<DataGridView>(formName, gridViewJson, parentName, emitEvents);
            }
            else
            {
                dynamic payloadDynamic = JsonConvert.DeserializeObject<dynamic>(payload);
                AddControlInstance<DataGridView>(formName, gridViewJson, parentName, emitEvents, payloadDynamic);
            }
        }

        #endregion

    }
}
