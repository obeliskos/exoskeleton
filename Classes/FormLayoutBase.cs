using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exoskeleton.Classes
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class FormLayoutBase : IDisposable
    {
        protected IHostWindow host;

        protected Dictionary<string, Form> formDictionary;
        protected Dictionary<string, Dictionary<string, Control>> controlDictionary;

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
        /// <returns></returns>
        protected T AddControl<T>(string formName, string json, string parentName = null) where T : Control
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

            return ctl;
        }

        /// <summary>
        /// Applies property values to controls which have already been added.
        /// Can be used for separating control layout and data initialization.  
        /// </summary>
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

                var json = JsonConvert.SerializeObject(valuesDictionary[key]); // must be a better way
                JsonConvert.PopulateObject(json, control);
            }
        }

        /// <summary>
        /// Applies a dialog/form defintion to the current dialog.
        /// Definitions allow representation of a series of controls, nesting, and property attributes
        /// within a single json object definition.
        /// </summary>
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

                string propJson = JsonConvert.SerializeObject(props);

                switch (type)
                {
                    case "Panel": AddPanel(formName, propJson, parent); break;
                    case "Label": AddLabel(formName, propJson, parent); break;
                    case "CheckBox": AddCheckBox(formName, propJson, parent); break;
                    case "RadioButton": AddRadioButton(formName, propJson, parent); break;
                    case "TextBox": AddTextBox(formName, propJson, parent); break;
                    case "MaskedTextBox": AddMaskedTextBox(formName, propJson, parent); break;
                    case "ListBox": AddListBox(formName, propJson, parent); break;
                    case "ComboBox": AddComboBox(formName, propJson, parent); break;
                    case "NumericUpDown": AddNumericUpDown(formName, propJson, parent); break;
                    case "DateTimePicker": AddDateTimePicker(formName, propJson, parent); break;
                    case "MonthCalendar": AddMonthCalendar(formName, propJson, parent); break;
                    case "DataGridView": AddDataGridView(formName, propJson, null, parent); break;
                    case "DialogButton":
                        string dialogResult = "";
                        if (!String.IsNullOrEmpty((string)cdef["DialogResult"]))
                        {
                            dialogResult = (string)cdef["DialogResult"];
                        }
                        AddDialogButton(formName, propJson, dialogResult, parent);
                        break;
                    case "EventButton":
                        string eventName = "";
                        if (!String.IsNullOrEmpty((string) cdef["EventName"]))
                        {
                            eventName = (string)cdef["EventName"];
                        }
                        AddEventButton(formName, propJson, eventName, parent);
                        break;
                    case "CheckedListBox":
                        string idxJson = null;
                        if (cdef["CheckedIndices"] != null)
                        {
                            idxJson = JsonConvert.SerializeObject(cdef["CheckedIndices"]);
                        }
                        AddCheckedListBox(formName, propJson, idxJson, parent);
                        break;
                }
            }
        }

        /// <summary>
        /// Used internally to construct response json to pass called when dismissing dialog.
        /// </summary>
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
        /// Used internally for event handlers which don't need response wrapped in object containing DialogResult.
        /// </summary>
        /// <param name="formName"></param>
        /// <returns></returns>
        protected string GenerateDynamicResponse(string formName)
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

                if (control.GetType() == typeof(TextBox))
                {
                    TextBox t = (TextBox)control;

                    dynamic tval = new { Name = t.Name, Text = t.Text };
                    dyn.Add(t.Name, JObject.FromObject(tval));
                }

                if (control.GetType() == typeof(MaskedTextBox))
                {
                    MaskedTextBox t = (MaskedTextBox)control;

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

                    dynamic clbval = new
                    {
                        Name = clb.Name,
                        Selected = clb.CheckedItems,
                        CheckedIndices = clb.CheckedIndices
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
                    dyn.Add(dtp.Name, JObject.FromObject(dtpval));
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
                    dyn.Add(mc.Name, JObject.FromObject(dtpval));
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

            return dyn;
        }

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

        public void Close(string formName)
        {
            if (formDictionary[formName] != null)
            {
                formDictionary[formName].Close();
            }
        }

        #region "Add" Dialog composition methods

        /// <summary>
        /// Adds a Panel to the Dialog singleton Form.
        /// </summary>
        /// <param name="panelJson"></param>
        /// <param name="parentName"></param>
        public void AddPanel(string formName, string panelJson, string parentName = null)
        {
            Panel p = JsonConvert.DeserializeObject<Panel>(panelJson);
            p.SuspendLayout();

            controlDictionary[formName][p.Name] = p;

            if (parentName == null)
            {
                formDictionary[formName].Controls.Add(p);
            }
            else
            {
                controlDictionary[formName][parentName].Controls.Add(p);
            }
        }

        /// <summary>
        /// Adds a Label to the Dialog singleton Form.
        /// </summary>
        /// <param name="labelJson"></param>
        /// <param name="parentName"></param>
        public void AddLabel(string formName, string labelJson, string parentName = null)
        {
            Label l = JsonConvert.DeserializeObject<Label>(labelJson);
            l.SuspendLayout();

            controlDictionary[formName][l.Name] = l;

            if (parentName == null)
            {
                formDictionary[formName].Controls.Add(l);
            }
            else
            {
                controlDictionary[formName][parentName].Controls.Add(l);
            }
        }

        /// <summary>
        /// Adds a CheckBox to the Dialog singleton Form.
        /// </summary>
        /// <param name="checkboxJson"></param>
        /// <param name="parentName"></param>
        public void AddCheckBox(string formName, string checkboxJson, string parentName = null)
        {
            AddControl<CheckBox>(formName, checkboxJson, parentName);
        }

        /// <summary>
        /// Adds a RadioButton to the Dialog singleton Form.
        /// </summary>
        /// <param name="radiobuttonJson"></param>
        /// <param name="parentName"></param>
        public void AddRadioButton(string formName, string radiobuttonJson, string parentName = null)
        {
            AddControl<RadioButton>(formName, radiobuttonJson, parentName);
        }

        /// <summary>
        /// Adds a TextBox to the Dialog singleton Form.
        /// </summary>
        /// <param name="textboxJson"></param>
        /// <param name="parentName"></param>
        public void AddTextBox(string formName, string textboxJson, string parentName = null)
        {
            AddControl<TextBox>(formName, textboxJson, parentName);
        }

        /// <summary>
        /// Adds a MaskedTextBox to the dialog singleton form.
        /// </summary>
        /// <param name="maskedtextboxJson"></param>
        /// <param name="parentName"></param>
        public void AddMaskedTextBox(string formName, string maskedtextboxJson, string parentName = null)
        {
            AddControl<MaskedTextBox>(formName, maskedtextboxJson, parentName);
        }

        /// <summary>
        /// Adds a ListBox to the Dialog singleton Form.
        /// </summary>
        /// <param name="listboxJson"></param>
        /// <param name="parentName"></param>
        public void AddListBox(string formName, string listboxJson, string parentName = null)
        {
            AddControl<ListBox>(formName, listboxJson, parentName);
        }

        /// <summary>
        /// Adds a ComboBox to the Dialog singleton Form.
        /// </summary>
        /// <param name="comboboxJson"></param>
        /// <param name="parentName"></param>
        public void AddComboBox(string formName, string comboboxJson, string parentName = null)
        {

            AddControl<ComboBox>(formName, comboboxJson, parentName);
        }

        /// <summary>
        /// Adds a NumericUpDown control to the global dialog singleton.
        /// </summary>
        /// <param name="numericJson"></param>
        /// <param name="parentName"></param>
        public void AddNumericUpDown(string formName, string numericJson, string parentName)
        {
            AddControl<NumericUpDown>(formName, numericJson, parentName);
        }

        /// <summary>
        /// Adds a DateTimePicker control to the global dialog singleton
        /// </summary>
        /// <param name="dateTimePicker"></param>
        /// <param name="parentName"></param>
        public void AddDateTimePicker(string formName, string dateTimePicker, string parentName)
        {
            AddControl<DateTimePicker>(formName, dateTimePicker, parentName);
        }

        /// <summary>
        /// Adds a MonthCalendar control to the global dialog singleton.
        /// </summary>
        /// <param name="monthcalendar"></param>
        /// <param name="parentName"></param>
        public void AddMonthCalendar(string formName, string monthcalendar, string parentName)
        {
            AddControl<MonthCalendar>(formName, monthcalendar, parentName);
        }

        /// <summary>
        /// Adds a Button to the Dialog and wires up an event to dismiss dialog with a result.
        /// </summary>
        /// <param name="buttonJson"></param>
        /// <param name="dialogResult"></param>
        /// <param name="parentName"></param>
        public void AddDialogButton(string formName, string buttonJson, string dialogResult, string parentName = null)
        {
            DialogResult dr = (DialogResult)Enum.Parse(typeof(DialogResult), dialogResult);

            Button btn = AddControl<Button>(formName, buttonJson, parentName);
            btn.Click += (sender, args) => {
                formDictionary[formName].DialogResult = dr;
            };
        }

        public void AddEventButton(string formName, string buttonJson, string eventName, string parentName = null)
        {
            Button btn = AddControl<Button>(formName, buttonJson, parentName);
            btn.Click += (sender, args) =>
            {
                host.PackageAndUnicast(eventName, GenerateDynamicResponseObject(formName));
            };
        }

        /// <summary>
        /// Adds a CheckedListBox to the Dialog singeton Form.
        /// </summary>
        /// <param name="checklistJson"></param>
        /// <param name="parentName"></param>
        public void AddCheckedListBox(string formName, string checklistJson, string checkedIndices, string parentName = null)
        {
            CheckedListBox clb = AddControl<CheckedListBox>(formName, checklistJson, parentName);
            if (!String.IsNullOrEmpty(checkedIndices))
            {
                int[] items = JsonConvert.DeserializeObject<int[]>(checkedIndices);
                foreach (int item in items)
                {
                    clb.SetItemChecked(item, true);
                }
            }
        }

        /// <summary>
        /// Adds a DataGridView to the global dialog singleton
        /// </summary>
        /// <param name="gridViewJson"></param>
        /// <param name="objectArrayJson"></param>
        /// <param name="parentName"></param>
        public void AddDataGridView(string formName, string gridViewJson, string objectArrayJson, string parentName = null)
        {
            DataGridView dgv = AddControl<DataGridView>(formName, gridViewJson, parentName);
            if (!String.IsNullOrEmpty(objectArrayJson))
            {
                dynamic objArray = JsonConvert.DeserializeObject(objectArrayJson);
                dgv.DataSource = objArray;
            }
        }

        #endregion

    }
}
