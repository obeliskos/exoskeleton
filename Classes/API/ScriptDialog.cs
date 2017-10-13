using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
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
        /// Initializes the Dialog singleton Form.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Initialize(string formJson)
        {
            dialog = new Form();
            JsonConvert.PopulateObject(formJson, dialog);

            dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
            dialog.ControlBox = false;
            dialog.MinimizeBox = false;
            dialog.MaximizeBox = false;
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.SuspendLayout();

            controlDictionary = new Dictionary<string, Control>();
        }

        /// <summary>
        /// Private overload to initialize predefined dialogs
        /// </summary>
        /// <param name="title"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void Initialize(string title, int width, int height)
        {
            dialog = new Form();
            dialog.Text = title;
            dialog.Width = width;
            dialog.Height = height;
            dialog.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
            dialog.ControlBox = false;
            dialog.MinimizeBox = false;
            dialog.MaximizeBox = false;
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.SuspendLayout();

            controlDictionary = new Dictionary<string, Control>();
        }

        #region Predefined Dialogs

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

        /// <summary>
        /// Displays a predefined dialog to allow user to pick a date.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="prompt"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string ShowDatePicker(string title, string prompt, string defaultValue)
        {
            Initialize(title, 360, 200);

            Label l = new Label();
            l.AutoSize = true;
            l.Text = prompt;
            l.Left = 10;
            l.Top = 20;

            dialog.Controls.Add(l);

            DateTimePicker dtp = new DateTimePicker();
            dtp.Width = 300;
            dtp.Left = 15;
            dtp.Top = 48;
            if (!String.IsNullOrEmpty(defaultValue))
            {
                dtp.Value = DateTime.Parse(defaultValue);
            }

            dialog.Controls.Add(dtp);

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

            TimeSpan ts = dtp.Value - new DateTime(1970, 1, 1);
            TimeSpan tsu = dtp.Value.ToUniversalTime() - new DateTime(1970, 1, 1);
            dynamic dtpval = new
            {
                Value = dtp.Value,
                Date = dtp.Value.Date,
                Short = dtp.Value.ToShortDateString(),
                TimeOfDay = dtp.Value.TimeOfDay,
                Epoch = (long)ts.TotalMilliseconds,
                UniversalTime = dtp.Value.ToUniversalTime(),
                UniversalEpoch = (long)tsu.TotalMilliseconds
            };

            return (dr == DialogResult.OK) ? JsonConvert.SerializeObject(dtpval) : null;
        }

        /// <summary>
        /// Displays a predefined dialog to allow user to select item(s) from a listbox.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="prompt"></param>
        /// <param name="values"></param>
        /// <param name="selectedItem"></param>
        /// <param name="multiselect"></param>
        /// <returns></returns>
        public string ShowList(string title, string prompt, string values, string selectedItem, bool multiselect)
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
        /// Displays a predefined dialog to allow user to select item(s) from a checkedlistbox.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="prompt"></param>
        /// <param name="values"></param>
        /// <param name="checkedIndices"></param>
        /// <returns></returns>
        public string ShowCheckedList(string title, string prompt, string values, string checkedIndices)
        {
            Initialize(title, 360, 300);

            Label l = new Label();
            l.AutoSize = true;
            l.Text = prompt;
            l.Left = 10;
            l.Top = 20;

            dialog.Controls.Add(l);

            string[] vals = JsonConvert.DeserializeObject<string[]>(values);

            CheckedListBox clb = new CheckedListBox();
            clb.Width = 300;
            clb.Height = 160;
            clb.Left = 15;
            clb.Top = 48;
            clb.CheckOnClick = true;
            clb.Items.AddRange(vals);
            if (!String.IsNullOrEmpty(checkedIndices))
            {
                int[] items = JsonConvert.DeserializeObject<int[]>(checkedIndices);
                foreach(int item in items)
                {
                    clb.SetItemChecked(item, true);
                }
            }

            dialog.Controls.Add(clb);

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

            return JsonConvert.SerializeObject(clb.CheckedItems);
        }

        // experimental
        public void ShowPropertyGrid(string title, string caption, string objectJson)
        {
            Initialize(title, 460, 400);

            Label l = new Label();
            l.AutoSize = true;
            l.Text = caption;
            l.Left = 10;
            l.Top = 20;

            dialog.Controls.Add(l);

            dynamic obj = JsonConvert.DeserializeObject(objectJson);

            PropertyGrid pg = new PropertyGrid();
            pg.Width = 400;
            pg.Height = 260;
            pg.Left = 15;
            pg.Top = 48;
            pg.SelectedObject = obj;
 
            dialog.Controls.Add(pg);

            Button ok = new Button();
            ok.Text = "OK";
            ok.Top = 320;
            ok.Left = 210;
            ok.Width = 100;
            ok.Height = 30;
            ok.Click += (sender, args) => { dialog.DialogResult = DialogResult.OK; };

            dialog.Controls.Add(ok);
            DialogResult dr = dialog.ShowDialog();

            return;
        }

        /// <summary>
        /// Displays a predefined dialog to allow user to select item(s) from a DataGridView.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="caption"></param>
        /// <param name="objectJson"></param>
        /// <returns></returns>
        public string ShowDataGridView(string title, string caption, string objectJson)
        {
            Initialize(title, 720, 480);

            dynamic obj = JsonConvert.DeserializeObject(objectJson);

            DataGridView dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.DataSource = obj;
            //dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dialog.Controls.Add(dgv);

            Label l = new Label();
            l.Dock = DockStyle.Top;
            l.Padding = new Padding(10);
            l.Height = 40;
            l.Text = caption;
            dialog.Controls.Add(l);

            Button ok = new Button();
            ok.Dock = DockStyle.Bottom;
            ok.Text = "OK";
            ok.Click += (sender, args) => { dialog.DialogResult = DialogResult.OK; };
            dialog.Controls.Add(ok);

            dialog.ResumeLayout(false);
            dialog.PerformLayout();

            DialogResult dr = dialog.ShowDialog();

            var result = JsonConvert.SerializeObject(dgv.SelectedRows);

            Int32 selectedRowCount = dgv.Rows.GetRowCount(DataGridViewElementStates.Selected);
            if (selectedRowCount == 0)
            {
                return null;
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            List<int> selectedIndices = new List<int>();

            for (int i = 0; i < selectedRowCount; i++)
            {
                selectedIndices.Add(dgv.SelectedRows[i].Index);
            }

            return JsonConvert.SerializeObject(selectedIndices);
        }

        #endregion

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

        /// <summary>
        /// Adds a RadioButton to the Dialog singleton Form.
        /// </summary>
        /// <param name="radiobuttonJson"></param>
        /// <param name="parentName"></param>
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
        /// Adds a CheckedListBox to the Dialog singeton Form.
        /// </summary>
        /// <param name="checklistJson"></param>
        /// <param name="parentName"></param>
        public void AddCheckedListBox(string checklistJson, string checkedIndices, string parentName = null)
        {
            CheckedListBox clb = JsonConvert.DeserializeObject<CheckedListBox>(checklistJson);
            clb.SuspendLayout();
            if (!String.IsNullOrEmpty(checkedIndices))
            {
                int[] items = JsonConvert.DeserializeObject<int[]>(checkedIndices);
                foreach (int item in items)
                {
                    clb.SetItemChecked(item, true);
                }
            }

            controlDictionary[clb.Name] = clb;

            if (parentName == null)
            {
                dialog.Controls.Add(clb);
            }
            else
            {
                controlDictionary[parentName].Controls.Add(clb);
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
        /// Adds a DateTimePicker control to the global dialog singleton
        /// </summary>
        /// <param name="dateTimePicker"></param>
        /// <param name="parentName"></param>
        public void AddDateTimePicker(string dateTimePicker, string parentName)
        {
            DateTimePicker dtp = JsonConvert.DeserializeObject<DateTimePicker>(dateTimePicker);
            dtp.SuspendLayout();

            controlDictionary[dtp.Name] = dtp;

            if (parentName == null)
            {
                dialog.Controls.Add(dtp);
            }
            else
            {
                controlDictionary[parentName].Controls.Add(dtp);
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

        public void AddDataGridView(string gridViewJson, string objectArrayJson, string parentName = null)
        {
            DataGridView dgv = new DataGridView();
            dgv.SuspendLayout();

            controlDictionary[dgv.Name] = dgv;

            if (parentName == null)
            {
                dialog.Controls.Add(dgv);
            }
            else
            {
                controlDictionary[parentName].Controls.Add(dgv);
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

                if (control.GetType() == typeof(CheckedListBox))
                {
                    CheckedListBox clb = (CheckedListBox)control;

                    dynamic clbval = new {
                        Name = clb.Name,
                        Selected = clb.CheckedItems,
                        SelectedIndices = clb.CheckedIndices
                    };
                    dyn.Add(clb.Name, JObject.FromObject(clbval));
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

                if (control.GetType() == typeof(DateTimePicker))
                {
                    DateTimePicker dtp = (DateTimePicker)control;
                    TimeSpan ts = dtp.Value - new DateTime(1970, 1, 1);
                    TimeSpan tsu = dtp.Value.ToUniversalTime() - new DateTime(1970, 1, 1);
                    dynamic dtpval = new {
                        Name = dtp.Name,
                        Value = dtp.Value,
                        Date = dtp.Value.Date,
                        Short = dtp.Value.ToShortDateString(),
                        TimeOfDay = dtp.Value.TimeOfDay,
                        Epoch = (long)ts.TotalMilliseconds,
                        UniversalTime = dtp.Value.ToUniversalTime(),
                        UniversalEpoch = (long)tsu.TotalMilliseconds
                    };
                    dyn.Add(dtp.Name, JObject.FromObject(dtpval));
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
                    dyn.Add(dgv.Name, JObject.FromObject(dgvval));
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
