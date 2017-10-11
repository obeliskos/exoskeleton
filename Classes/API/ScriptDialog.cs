using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exoskeleton.Classes.API
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class ScriptDialog : IDisposable
    {
        IHostWindow host;

        private Form dialog;
        private Dictionary<string, Control> controlDictionary;

        public ScriptDialog(IHostWindow host)
        {
            this.host = host;
        }
        
        /// <summary>
        /// Displays a predefined dialog to allow user to input a string.
        /// </summary>
        /// <param name="title">Dialog window title text</param>
        /// <param name="prompt">Textbox caption label text</param>
        /// <param name="defaultText">Texbox default value text</param>
        /// <returns></returns>
        public string ShowInputDialog(string title, string prompt, string defaultText)
        {
            Initialize(title, 360, 200);

            Label l = new Label();
            l.AutoSize = true;
            l.Text = prompt;
            l.Left = 10;
            l.Top = 20;

            dialog.Controls.Add(l);

            TextBox tb = new TextBox();
            tb.Text = defaultText;
            tb.Width = 300;
            tb.Left = 15;
            tb.Top = 48;

            dialog.Controls.Add(tb);

            Button ok = new Button();
            ok.Text = "OK";
            ok.Top = 100;
            ok.Left = 98;
            ok.Width = 100;
            ok.Height = 30;
            ok.Click += (sender, args) => { dialog.DialogResult = DialogResult.OK; };

            dialog.Controls.Add(ok);

            Button cancel = new Button();
            cancel.Text = "Cancel";
            cancel.Top = 100;
            cancel.Left = 210;
            cancel.Width = 100;
            cancel.Height = 30;
            cancel.Click += (sender, args) => { dialog.DialogResult = DialogResult.Cancel; };

            dialog.Controls.Add(cancel);

            DialogResult dr = dialog.ShowDialog();

            return (dr == DialogResult.OK) ? tb.Text : null;
        }

        public string ShowPickList(string title, string prompt, string values, string selectedItem, bool multiselect)
        {
            Initialize(title, 360, 300);

            Label l = new Label();
            l.AutoSize = true;
            l.Text = prompt;
            l.Left = 10;
            l.Top = 20;

            dialog.Controls.Add(l);

            string[] vals = JsonConvert.DeserializeObject<string[]>(values);

            ListBox lb = new ListBox();
            lb.Width = 300;
            lb.Height = 160;
            lb.Left = 15;
            lb.Top = 48;
            lb.Items.AddRange(vals);
            if (selectedItem != null && selectedItem != "")
            {
                lb.SelectedItem = selectedItem;
            }
            if (multiselect == true)
            {
                lb.SelectionMode = SelectionMode.MultiSimple;
            }

            dialog.Controls.Add(lb);

            Button ok = new Button();
            ok.Text = "OK";
            ok.Top = 220;
            ok.Left = 98;
            ok.Width = 100;
            ok.Height = 30;
            ok.Click += (sender, args) => { dialog.DialogResult = DialogResult.OK; };

            dialog.Controls.Add(ok);

            Button cancel = new Button();
            cancel.Text = "Cancel";
            cancel.Top = 220;
            cancel.Left = 210;
            cancel.Width = 100;
            cancel.Height = 30;
            cancel.Click += (sender, args) => { dialog.DialogResult = DialogResult.Cancel; };

            dialog.Controls.Add(cancel);

            DialogResult dr = dialog.ShowDialog();

            if (dr != DialogResult.OK)
            {
                return null;
            }

            return (multiselect)?JsonConvert.SerializeObject(lb.SelectedItems):lb.SelectedItem.ToString();
        }

        /// <summary>
        /// Initializes the Dialog singleton Form.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Initialize(string title, int width, int height)
        {
            dialog = new Form();
            dialog.Text = title;
            dialog.Width = width;
            dialog.Height = height;
            dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
            dialog.ControlBox = false;
            dialog.MinimizeBox = false;
            dialog.MaximizeBox = false;
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.SuspendLayout();

            controlDictionary = new Dictionary<string, Control>();
        }
        
        /// <summary>
        /// Adds a Panel to the Dialog singleton Form.
        /// </summary>
        /// <param name="panelJson"></param>
        /// <param name="parentName"></param>
        public void AddPanel(string panelJson, string parentName = null)
        {
            Panel p = JsonConvert.DeserializeObject<Panel>(panelJson);
            p.SuspendLayout();

            controlDictionary[p.Name] = p;

            if (parentName == null)
            {
                dialog.Controls.Add(p);
            }
            else
            {
                controlDictionary[parentName].Controls.Add(p);
            }
        }

        /// <summary>
        /// Adds a Label to the Dialog singleton Form.
        /// </summary>
        /// <param name="labelJson"></param>
        /// <param name="parentName"></param>
        public void AddLabel(string labelJson, string parentName = null)
        {
            Label l = JsonConvert.DeserializeObject<Label>(labelJson);
            l.SuspendLayout();

            controlDictionary[l.Name] = l;

            if (parentName == null)
            {
                dialog.Controls.Add(l);
            }
            else { 
                controlDictionary[parentName].Controls.Add(l);
            }
        }

        /// <summary>
        /// Adds a CheckBox to the Dialog singleton Form.
        /// </summary>
        /// <param name="checkboxJson"></param>
        /// <param name="parentName"></param>
        public void AddCheckBox(string checkboxJson, string parentName = null)
        {
            CheckBox cb = JsonConvert.DeserializeObject<CheckBox>(checkboxJson);
            cb.SuspendLayout();

            controlDictionary[cb.Name] = cb;

            if (parentName == null)
            {
                dialog.Controls.Add(cb);
            }
            else
            {
                controlDictionary[parentName].Controls.Add(cb);
            }
        }

        public void AddRadioButton(string radiobuttonJson, string parentName = null)
        {
            RadioButton rb = JsonConvert.DeserializeObject<RadioButton>(radiobuttonJson);
            rb.SuspendLayout();

            controlDictionary[rb.Name] = rb;

            if (parentName == null)
            {
                dialog.Controls.Add(rb);
            }
            else
            {
                controlDictionary[parentName].Controls.Add(rb);
            }
        }

        /// <summary>
        /// Adds a TextBox to the Dialog singleton Form.
        /// </summary>
        /// <param name="textboxJson"></param>
        /// <param name="parentName"></param>
        public void AddTextBox(string textboxJson, string parentName = null)
        {
            TextBox tb = JsonConvert.DeserializeObject<TextBox>(textboxJson);
            tb.SuspendLayout();

            controlDictionary[tb.Name] = tb;

            if (parentName == null)
            {
                dialog.Controls.Add(tb);
            }
            else
            {
                controlDictionary[parentName].Controls.Add(tb);
            }
        }

        /// <summary>
        /// Adds a ListBox to the Dialog singleton Form.
        /// </summary>
        /// <param name="listboxJson"></param>
        /// <param name="parentName"></param>
        public void AddListBox(string listboxJson, string parentName = null)
        {
            ListBox lb = JsonConvert.DeserializeObject<ListBox>(listboxJson);
            lb.SuspendLayout();

            controlDictionary[lb.Name] = lb;

            if (parentName == null)
            {
                dialog.Controls.Add(lb);
            }
            else
            {
                controlDictionary[parentName].Controls.Add(lb);
            }
        }

        /// <summary>
        /// Adds a ComboBox to the Dialog singleton Form.
        /// </summary>
        /// <param name="comboboxJson"></param>
        /// <param name="parentName"></param>
        public void AddComboBox(string comboboxJson, string parentName = null)
        {
            ComboBox cb = JsonConvert.DeserializeObject<ComboBox>(comboboxJson);
            cb.SuspendLayout();

            controlDictionary[cb.Name] = cb;

            if (parentName == null)
            {
                dialog.Controls.Add(cb);
            }
            else
            {
                controlDictionary[parentName].Controls.Add(cb);
            }
        }

        /// <summary>
        /// Adds a NumericUpDown control to the global dialog singleton.
        /// </summary>
        /// <param name="numericJson"></param>
        /// <param name="parentName"></param>
        public void AddNumericUpDown(string numericJson, string parentName)
        {
            NumericUpDown nud = JsonConvert.DeserializeObject<NumericUpDown>(numericJson);
            nud.SuspendLayout();

            controlDictionary[nud.Name] = nud;

            if (parentName == null)
            {
                dialog.Controls.Add(nud);
            }
            else
            {
                controlDictionary[parentName].Controls.Add(nud);
            }
        }

        /// <summary>
        /// Adds a Button to the Dialog and wires up an event to dismiss dialog with a result.
        /// </summary>
        /// <param name="buttonJson"></param>
        /// <param name="dialogResult"></param>
        /// <param name="parentName"></param>
        public void AddDialogButton(string buttonJson, string dialogResult, string parentName = null)
        {
            Button b = JsonConvert.DeserializeObject<Button>(buttonJson);
            b.SuspendLayout();

            controlDictionary[b.Name] = b;

            DialogResult dr = (DialogResult) Enum.Parse(typeof(DialogResult), dialogResult);

            b.Click += (sender, args) => {
                dialog.DialogResult = dr;
            };

            if (parentName == null)
            {
                dialog.Controls.Add(b);
            }
            else
            {
                controlDictionary[parentName].Controls.Add(b);
            }
        }

        /// <summary>
        /// Adds a PropertyGrid to the global dialog singleton.
        /// </summary>
        /// <param name="pgJson"></param>
        /// <param name="objectJson"></param>
        /// <param name="parentName"></param>
        public void AddPropertyGrid(string pgJson, string objectJson, string parentName = null)
        {
            PropertyGrid pg = new PropertyGrid();
            pg.SuspendLayout();

            controlDictionary[pg.Name] = pg;

            if (parentName == null)
            {
                dialog.Controls.Add(pg);
            }
            else
            {
                controlDictionary[parentName].Controls.Add(pg);
            }
        }

        protected void FinalizeLayout()
        {
            foreach (KeyValuePair<string, Control> controlKV in controlDictionary)
            {
                Control control = controlKV.Value;

                control.ResumeLayout(false);
                control.PerformLayout();
            }

            dialog.ResumeLayout(false);
            dialog.PerformLayout();
        }

        /// <summary>
        /// Shows the singleton Dialog and returns the result and control values to caller.
        /// </summary>
        /// <returns></returns>
        public string ShowDialog()
        {
            FinalizeLayout();

            DialogResult dr = dialog.ShowDialog(host.GetForm());

            return GenerateDynamicResponse(dr);
        }

        /// <summary>
        /// Used internally to construct response json to pass called when dismissing dialog.
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        protected string GenerateDynamicResponse(DialogResult dr)
        {
            var dyn = new JObject();

            foreach (KeyValuePair<string, Control> controlKV in controlDictionary)
            {
                Control control = controlKV.Value;

                if (control.GetType() == typeof(TextBox))
                {
                    TextBox t = (TextBox)control;

                    dynamic tval = new { Name = t.Name, Text = t.Text };
                    dyn.Add(t.Name, JObject.FromObject(tval));
                }

                if (control.GetType() == typeof(CheckBox))
                {
                    CheckBox cb = (CheckBox)control;

                    dynamic cbval = new { Name = cb.Name, Checked = cb.Checked };
                    dyn.Add(cb.Name, JObject.FromObject(cbval));
                }

                if (control.GetType() == typeof(RadioButton))
                {
                    RadioButton rb = (RadioButton)control;

                    dynamic rbval = new { Name = rb.Name, Checked = rb.Checked };
                    dyn.Add(rb.Name, JObject.FromObject(rbval));
                }

                if (control.GetType() == typeof(ListBox))
                {
                    ListBox lb = (ListBox)control;

                    dynamic lbval = new { Name = lb.Name, Selected = lb.SelectedItem };
                    dyn.Add(lb.Name, JObject.FromObject(lbval));
                }

                if (control.GetType() == typeof(ComboBox))
                {
                    ComboBox cb = (ComboBox)control;

                    dynamic cbval = new { Name = cb.Name, Selected = cb.SelectedItem };
                    dyn.Add(cb.Name, JObject.FromObject(cbval));
                }

                if (control.GetType() == typeof(NumericUpDown))
                {
                    NumericUpDown nud = (NumericUpDown)control;

                    dynamic nudval = new { Name = nud.Name, Value = nud.Value };
                    dyn.Add(nud.Name, JObject.FromObject(nudval));
                }

            }

            dynamic result = new
            {
                Result = dr.ToString(),
                Controls = dyn
            };

            string json = JsonConvert.SerializeObject(result);

            return json;
        }

        public void Dispose()
        {
            dialog = null;
            controlDictionary = null;
            host = null;
        }
    }
}
