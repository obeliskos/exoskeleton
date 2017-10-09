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
        /// Initializes the Dialog singleton Form.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Initialize(string title, int width, int height)
        {
            dialog = new Form();
            dialog.Text = title;
            dialog.Width = 400;
            dialog.Height = 240;
            dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
            dialog.ControlBox = false;
            dialog.MinimizeBox = false;
            dialog.MaximizeBox = false;
            dialog.StartPosition = FormStartPosition.CenterParent;

            controlDictionary = new Dictionary<string, Control>();
        }
        
        /// <summary>
        /// Adds a Panel to the Dialog singleton Form.
        /// </summary>
        /// <param name="panelJson"></param>
        /// <param name="parentId"></param>
        public void AddPanel(string panelJson, string parentId = null)
        {
            Panel p = JsonConvert.DeserializeObject<Panel>(panelJson);
            controlDictionary[p.Name] = p;

            if (parentId == null)
            {
                dialog.Controls.Add(p);
            }
            else
            {
                // lookup parent control and add this control to it
                controlDictionary[parentId].Controls.Add(p);
            }
        }

        /// <summary>
        /// Adds a Label to the Dialog singleton Form.
        /// </summary>
        /// <param name="labelJson"></param>
        /// <param name="parentId"></param>
        public void AddLabel(string labelJson, string parentId = null)
        {
            Label l = JsonConvert.DeserializeObject<Label>(labelJson);
            controlDictionary[l.Name] = l;

            if (parentId == null)
            {
                dialog.Controls.Add(l);
            }
            else { 
                // lookup parent control and add this control to it
                controlDictionary[parentId].Controls.Add(l);
            }
        }

        /// <summary>
        /// Adds a TextBox to the Dialog singleton Form.
        /// </summary>
        /// <param name="textboxJson"></param>
        /// <param name="parentId"></param>
        public void AddTextBox(string textboxJson, string parentId = null)
        {
            TextBox tb = JsonConvert.DeserializeObject<TextBox>(textboxJson);
            controlDictionary[tb.Name] = tb;

            if (parentId == null)
            {
                dialog.Controls.Add(tb);
            }
            else
            {
                // lookup parent control and add this control to it
                controlDictionary[parentId].Controls.Add(tb);
            }
        }

        /// <summary>
        /// Adds a ListBox to the Dialog singleton Form.
        /// </summary>
        /// <param name="listboxJson"></param>
        /// <param name="parentId"></param>
        public void AddListBox(string listboxJson, string parentId = null)
        {
            ListBox lb = JsonConvert.DeserializeObject<ListBox>(listboxJson);
            controlDictionary[lb.Name] = lb;

            if (parentId == null)
            {
                dialog.Controls.Add(lb);
            }
            else
            {
                // lookup parent control and add this control to it
                controlDictionary[parentId].Controls.Add(lb);
            }
        }

        /// <summary>
        /// Adds a Button to the Dialog and wires up an event to dismiss dialog with a result.
        /// </summary>
        /// <param name="buttonJson"></param>
        /// <param name="dialogResult"></param>
        /// <param name="parentId"></param>
        public void AddDialogButton(string buttonJson, string dialogResult, string parentId = null)
        {
            Button b = JsonConvert.DeserializeObject<Button>(buttonJson);
            controlDictionary[b.Name] = b;

            DialogResult dr = (DialogResult) Enum.Parse(typeof(DialogResult), dialogResult);

            b.Click += (sender, args) => {
                dialog.DialogResult = dr;
            };

            if (parentId == null)
            {
                dialog.Controls.Add(b);
            }
            else
            {
                // lookup parent control and add this control to it
                controlDictionary[parentId].Controls.Add(b);
            }
        }

        /// <summary>
        /// Shows the singleton Dialog and returns the result and control values to caller.
        /// </summary>
        /// <returns></returns>
        public string ShowDialog()
        {
            DialogResult dr = dialog.ShowDialog();

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

                if (control.GetType() == typeof(Label))
                {
                    Label l = (Label)control;

                    dynamic lval = new { Name = l.Name, Text = l.Text };
                    dyn.Add(l.Name, JObject.FromObject(lval));
                }

                if (control.GetType() == typeof(TextBox))
                {
                    TextBox t = (TextBox)control;

                    dynamic tval = new { Name = t.Name, Text = t.Text };
                    dyn.Add(t.Name, JObject.FromObject(tval));
                }

                if (control.GetType() == typeof(Button))
                {
                    Button b = (Button)control;

                    dynamic bval = new { Name = b.Name, Text = b.Text };
                    dyn.Add(b.Name, JObject.FromObject(bval));
                }

                if (control.GetType() == typeof(ListBox))
                {
                    ListBox lb = (ListBox)control;

                    dynamic lbval = new { Name = lb.Name, Selected = lb.SelectedItem };
                    dyn.Add(lb.Name, JObject.FromObject(lbval));
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

        }
    }
}
