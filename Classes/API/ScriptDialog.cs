using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exoskeleton.Classes.API
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class ScriptDialog : FormLayoutBase, IDisposable
    {
        public ScriptDialog(IHostWindow host) : base(host)
        {
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        #region .NET Provided Dialogs

        /// <summary>
        /// Display an 'OpenFileDialog'
        /// </summary>
        /// <param name="dialogOptions">Optional object containing 'OpenFileDialog' properties to initialize dialog with.</param>
        /// <returns>'OpenFileDialog' properties after dialog was dismissed, or null if cancelled.</returns>
        public string ShowOpenFileDialog(string dialogOptions)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dialogOptions != null && dialogOptions != "")
            {
                dlg = JsonConvert.DeserializeObject<OpenFileDialog>(dialogOptions);
            }
            DialogResult dr = dlg.ShowDialog(Form.ActiveForm);
            if (dr != DialogResult.OK) return null;

            var result = JsonConvert.SerializeObject(dlg);
            return result;
        }

        /// <summary>
        /// Display a 'SaveFileDialog'
        /// <param name="dialogOptions">Optional object containing 'SaveFileDialog' properties to initialize dialog with.</param>
        /// </summary>
        /// <returns>'SaveFileDialog' properties after dialog was dismissed, or null if cancelled.</returns>
        public string ShowSaveFileDialog(string dialogOptions)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            if (dialogOptions != null && dialogOptions != "")
            {
                dlg = JsonConvert.DeserializeObject<SaveFileDialog>(dialogOptions);
            }
            DialogResult dr = dlg.ShowDialog(Form.ActiveForm);
            if (dr != DialogResult.OK) return null;

            var result = JsonConvert.SerializeObject(dlg);
            return result;
        }

        /// <summary>
        /// Display a dialog to allow the user to select a color.
        /// </summary>
        /// <param name="dialogOptions"></param>
        /// <returns></returns>
        public string ShowColorDialog(string dialogOptions)
        {
            ColorDialog dlg = new ColorDialog();
            if (!String.IsNullOrEmpty(dialogOptions))
            {
                dlg = JsonConvert.DeserializeObject<ColorDialog>(dialogOptions);
            }
            dlg.FullOpen = true;

            DialogResult dr = dlg.ShowDialog(Form.ActiveForm);
            if (dr != DialogResult.OK) return null;

            var result = new
            {
                Color = dlg.Color,
                HexColor = String.Format(
                    "#{0}{1}{2}",
                    dlg.Color.R.ToString("X2"),
                    dlg.Color.G.ToString("X2"),
                    dlg.Color.B.ToString("X2")
                )
            };

            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Display a dialog to allow the user to select a font.
        /// </summary>
        /// <param name="dialogOptions"></param>
        /// <returns></returns>
        public string ShowFontDialog(string dialogOptions)
        {
            FontDialog dlg = new FontDialog();
            if (!String.IsNullOrEmpty(dialogOptions))
            {
                dlg = JsonConvert.DeserializeObject<FontDialog>(dialogOptions);
            }
            DialogResult dr = dlg.ShowDialog(Form.ActiveForm);
            if (dr != DialogResult.OK) return null;

            var result = new
            {
                Font = dlg.Font,
                Family = dlg.Font.FontFamily,
                Size = dlg.Font.Size,
                Italic = dlg.Font.Italic,
                Bold = dlg.Font.Bold,
                Underline = dlg.Font.Underline,
                Strikeout = dlg.Font.Strikeout,
                SystemFontName = dlg.Font.SystemFontName
            };

            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Displays a message box to the user and returns the button they clicked.
        /// </summary>
        /// <param name="text">Message to display to user.</param>
        /// <param name="caption">Caption of message box window.</param>
        /// <param name="buttons">String representation of a MessageBoxButtons enum.</param>
        /// <param name="icon">string representation of a MessageBoxIcon enum.</param>
        /// <returns>Text (ToString) representation of button clicked.</returns>
        public string ShowMessageBox(string text, string caption, string buttons, string icon)
        {
            MessageBoxButtons _buttons = MessageBoxButtons.OK;
            if (buttons != null && buttons != "")
            {
                _buttons = (MessageBoxButtons)Enum.Parse(typeof(MessageBoxButtons), buttons);
            }
            MessageBoxIcon _icon = MessageBoxIcon.Information;
            if (icon != null && icon != "")
            {
                _icon = (MessageBoxIcon)Enum.Parse(typeof(MessageBoxIcon), icon);
            }

            //DialogResult dr = MessageBox.Show(host.GetForm(), text, caption, _buttons, _icon);
            DialogResult dr = MessageBox.Show(Form.ActiveForm, text, caption, _buttons, _icon);
            return dr.ToString();
        }

        #endregion

        #region Predefined "Prompt" Dialogs

        /// <summary>
        /// Displays a predefined dialog to allow user to select item(s) from a checkedlistbox.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="prompt"></param>
        /// <param name="values"></param>
        /// <param name="checkedIndices"></param>
        /// <returns></returns>
        public string PromptCheckedList(string title, string prompt, string values, string checkedIndices)
        {
            Initialize("PromptCheckedList", title, 360, 300);

            Label l = new Label();
            l.AutoSize = true;
            l.Text = prompt;
            l.Left = 10;
            l.Top = 20;

            formDictionary["PromptCheckedList"].Controls.Add(l);

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

            formDictionary["PromptCheckedList"].Controls.Add(clb);

            Button ok = new Button();
            ok.Text = "OK";
            ok.Top = 220;
            ok.Left = 98;
            ok.Width = 100;
            ok.Height = 30;
            ok.Click += (sender, args) => { formDictionary["PromptCheckedList"].DialogResult = DialogResult.OK; };

            formDictionary["PromptCheckedList"].Controls.Add(ok);

            Button cancel = new Button();
            cancel.Text = "Cancel";
            cancel.Top = 220;
            cancel.Left = 210;
            cancel.Width = 100;
            cancel.Height = 30;
            cancel.Click += (sender, args) => { formDictionary["PromptCheckedList"].DialogResult = DialogResult.Cancel; };

            formDictionary["PromptCheckedList"].Controls.Add(cancel);

            DialogResult dr = formDictionary["PromptCheckedList"].ShowDialog();

            if (dr != DialogResult.OK)
            {
                return null;
            }

            return JsonConvert.SerializeObject(clb.CheckedItems);
        }

        /// <summary>
        /// Displays a predefined dialog to allow user to select item(s) from a DataGridView.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="caption"></param>
        /// <param name="objectJson"></param>
        /// <returns></returns>
        public string PromptDataGridView(string title, string caption, string objectJson, bool autoSizeColumns)
        {
            Initialize("PredefinedDataGridView", title, 720, 480);

            dynamic obj = JsonConvert.DeserializeObject(objectJson);

            DataGridView dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.DataSource = obj;
            if (autoSizeColumns)
            {
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            formDictionary["PredefinedDataGridView"].Controls.Add(dgv);

            Label l = new Label();
            l.Dock = DockStyle.Top;
            l.Padding = new Padding(10);
            l.Height = 40;
            l.Text = caption;
            formDictionary["PredefinedDataGridView"].Controls.Add(l);

            Panel pb = new Panel();
            pb.Dock = DockStyle.Bottom;
            pb.Height = 48;
            formDictionary["PredefinedDataGridView"].Controls.Add(pb);

            Button ok = new Button();
            //ok.Dock = DockStyle.Bottom;
            ok.Text = "OK";
            ok.Left = 580;
            ok.Width = 110;
            ok.Height = 30;
            ok.Top = 7;
            ok.Click += (sender, args) => { formDictionary["PredefinedDataGridView"].DialogResult = DialogResult.OK; };
            pb.Controls.Add(ok);

            formDictionary["PredefinedDataGridView"].ResumeLayout(false);
            formDictionary["PredefinedDataGridView"].PerformLayout();

            DialogResult dr = formDictionary["PredefinedDataGridView"].ShowDialog();

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

        /// <summary>
        /// Displays a predefined dialog to allow user to pick a date.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="prompt"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string PromptDatePicker(string title, string prompt, string defaultValue)
        {
            Initialize("PredefinedDatePicker", title, 360, 200);

            Label l = new Label();
            l.AutoSize = true;
            l.Text = prompt;
            l.Left = 10;
            l.Top = 20;

            formDictionary["PredefinedDatePicker"].Controls.Add(l);

            DateTimePicker dtp = new DateTimePicker();
            dtp.Width = 300;
            dtp.Left = 15;
            dtp.Top = 48;
            if (!String.IsNullOrEmpty(defaultValue))
            {
                dtp.Value = DateTime.Parse(defaultValue);
            }

            formDictionary["PredefinedDatePicker"].Controls.Add(dtp);

            Button ok = new Button();
            ok.Text = "OK";
            ok.Top = 100;
            ok.Left = 98;
            ok.Width = 100;
            ok.Height = 30;
            ok.Click += (sender, args) => { formDictionary["PredefinedDatePicker"].DialogResult = DialogResult.OK; };

            formDictionary["PredefinedDatePicker"].Controls.Add(ok);

            Button cancel = new Button();
            cancel.Text = "Cancel";
            cancel.Top = 100;
            cancel.Left = 210;
            cancel.Width = 100;
            cancel.Height = 30;
            cancel.Click += (sender, args) => { formDictionary["PredefinedDatePicker"].DialogResult = DialogResult.Cancel; };

            formDictionary["PredefinedDatePicker"].Controls.Add(cancel);

            DialogResult dr = formDictionary["PredefinedDatePicker"].ShowDialog();

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
        /// Displays a predefined dialog to allow user to input a string.
        /// </summary>
        /// <param name="title">Dialog window title text</param>
        /// <param name="prompt">Textbox caption label text</param>
        /// <param name="defaultText">Texbox default value text</param>
        /// <returns></returns>
        public string PromptInput(string title, string prompt, string defaultText)
        {
            Initialize("PredefinedPrompt", title, 360, 200);

            Label l = new Label();
            l.AutoSize = true;
            l.Text = prompt;
            l.Left = 10;
            l.Top = 20;

            formDictionary["PredefinedPrompt"].Controls.Add(l);

            TextBox tb = new TextBox();
            tb.Text = defaultText;
            tb.Width = 300;
            tb.Left = 15;
            tb.Top = 48;

            formDictionary["PredefinedPrompt"].Controls.Add(tb);

            Button ok = new Button();
            ok.Text = "OK";
            ok.Top = 100;
            ok.Left = 98;
            ok.Width = 100;
            ok.Height = 30;
            ok.Click += (sender, args) => { formDictionary["PredefinedPrompt"].DialogResult = DialogResult.OK; };

            formDictionary["PredefinedPrompt"].Controls.Add(ok);

            Button cancel = new Button();
            cancel.Text = "Cancel";
            cancel.Top = 100;
            cancel.Left = 210;
            cancel.Width = 100;
            cancel.Height = 30;
            cancel.Click += (sender, args) => { formDictionary["PredefinedPrompt"].DialogResult = DialogResult.Cancel; };

            formDictionary["PredefinedPrompt"].Controls.Add(cancel);

            DialogResult dr = formDictionary["PredefinedPrompt"].ShowDialog();

            return (dr == DialogResult.OK) ? tb.Text : null;
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
        public string PromptList(string title, string prompt, string values, string selectedItem, bool multiselect)
        {
            Initialize("PredefinedList", title, 360, 300);

            Label l = new Label();
            l.AutoSize = true;
            l.Text = prompt;
            l.Left = 10;
            l.Top = 20;

            formDictionary["PredefinedList"].Controls.Add(l);

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

            formDictionary["PredefinedList"].Controls.Add(lb);

            Button ok = new Button();
            ok.Text = "OK";
            ok.Top = 220;
            ok.Left = 98;
            ok.Width = 100;
            ok.Height = 30;
            ok.Click += (sender, args) => { formDictionary["PredefinedList"].DialogResult = DialogResult.OK; };

            formDictionary["PredefinedList"].Controls.Add(ok);

            Button cancel = new Button();
            cancel.Text = "Cancel";
            cancel.Top = 220;
            cancel.Left = 210;
            cancel.Width = 100;
            cancel.Height = 30;
            cancel.Click += (sender, args) => { formDictionary["PredefinedList"].DialogResult = DialogResult.Cancel; };

            formDictionary["PredefinedList"].Controls.Add(cancel);

            DialogResult dr = formDictionary["PredefinedList"].ShowDialog();

            if (dr != DialogResult.OK)
            {
                return null;
            }

            return (multiselect)?JsonConvert.SerializeObject(lb.SelectedItems):lb.SelectedItem.ToString();
        }

        // experimental (can only be used on objects which can be restored to valid .net type)
        public string PromptPropertyGrid(string title, string caption, string objectJson, string assemblyName, string typeName)
        {
            Initialize("PredefinedPropertyGrid", title, 460, 500);

            Label l = new Label();
            l.AutoSize = true;
            l.Text = caption;
            l.Left = 10;
            l.Top = 20;

            formDictionary["PredefinedPropertyGrid"].Controls.Add(l);

            ListBox lb = new ListBox();

            dynamic obj;
            if (String.IsNullOrEmpty(typeName))
            {
                obj = JsonConvert.DeserializeObject(objectJson);
            }
            else
            {
                obj = Activator.CreateInstance(assemblyName, typeName).Unwrap();
                JsonConvert.PopulateObject(objectJson, obj);
            }

            PropertyGrid pg = new PropertyGrid();
            pg.Width = 400;
            pg.Height = 360;
            pg.Left = 15;
            pg.Top = 48;
            pg.SelectedObject = obj;
 
            formDictionary["PredefinedPropertyGrid"].Controls.Add(pg);

            Button ok = new Button();
            ok.Text = "OK";
            ok.Top = 420;
            ok.Left = 300;
            ok.Width = 110;
            ok.Height = 30;
            ok.Click += (sender, args) => { formDictionary["PredefinedPropertyGrid"].DialogResult = DialogResult.OK; };

            formDictionary["PredefinedPropertyGrid"].Controls.Add(ok);

            formDictionary["PredefinedPropertyGrid"].ResumeLayout(false);
            formDictionary["PredefinedPropertyGrid"].PerformLayout();

            DialogResult dr = formDictionary["PredefinedPropertyGrid"].ShowDialog();

            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.Error += delegate (object sender, ErrorEventArgs args)
            {
                args.ErrorContext.Handled = true;
            };

            return JsonConvert.SerializeObject(obj, jss);

        }

        #endregion

        /// <summary>
        /// Initializes the Dialog singleton Form.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Initialize(string formName, string formJson)
        {
            formDictionary[formName] = new Form();

            JsonConvert.PopulateObject(formJson, formDictionary[formName]);

            formDictionary[formName].FormBorderStyle = FormBorderStyle.FixedDialog;
            formDictionary[formName].ControlBox = false;
            formDictionary[formName].MinimizeBox = false;
            formDictionary[formName].MaximizeBox = false;
            formDictionary[formName].StartPosition = FormStartPosition.CenterParent;
            formDictionary[formName].SuspendLayout();

            controlDictionary[formName] = new Dictionary<string, Control>();
        }

        /// <summary>
        /// Private overload to initialize predefined dialogs
        /// </summary>
        /// <param name="title"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void Initialize(string formName, string title, int width, int height)
        {
            formDictionary[formName] = new Form();
            formDictionary[formName].Text = title;
            formDictionary[formName].Width = width;
            formDictionary[formName].Height = height;
            formDictionary[formName].Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            formDictionary[formName].FormBorderStyle = FormBorderStyle.FixedDialog;
            formDictionary[formName].ControlBox = false;
            formDictionary[formName].MinimizeBox = false;
            formDictionary[formName].MaximizeBox = false;
            formDictionary[formName].StartPosition = FormStartPosition.CenterParent;
            formDictionary[formName].SuspendLayout();

            controlDictionary[formName] = new Dictionary<string, Control>();
        }

        /// <summary>
        /// Shows the singleton Dialog and returns the result and control values to caller.
        /// </summary>
        /// <returns></returns>
        public string ShowDialog(string formName)
        {
            FinalizeLayout(formName);

            DialogResult dr = formDictionary[formName].ShowDialog(host.GetForm());

            return GenerateDynamicResponse(formName, dr);
        }

    }
}
