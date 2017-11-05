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
    public class ScriptForm : FormLayoutBase, IDisposable
    {
        public ScriptForm(IHostWindow host) : base(host)
        {
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public void Initialize(string formName, string formJson)
        {
            // If any form by that name is already loaded, close it first
            if (formDictionary.ContainsKey(formName))
            {
                formDictionary[formName].Close();
                formDictionary.Remove(formName);
            }

            formDictionary[formName] = new Form();
            formDictionary[formName].Icon = host.GetForm().Icon;

            JsonConvert.PopulateObject(formJson, formDictionary[formName]);

            JObject dyn = JsonConvert.DeserializeObject<JObject>(formJson);
            if (dyn["StartPosition"] == null)
            {
                formDictionary[formName].StartPosition = FormStartPosition.CenterParent;
            }

            formDictionary[formName].SuspendLayout();

            controlDictionary[formName] = new Dictionary<string, Control>();
        }

        /// <summary>
        /// Allows setting properties on the Form object, after it has been initialized.
        /// </summary>
        /// <param name="formName">The name of the form to apply properties to.</param>
        /// <param name="formJson">Json object containing properties to update.</param>
        public void ApplyFormProperties(string formName, string formJson)
        {
            if (formDictionary[formName] == null)
            {
                throw new Exception("A form by the name of " + formJson + " was not found.");
            }

            JsonConvert.PopulateObject(formJson, formDictionary[formName]);
        }

        /// <summary>
        /// Shows the singleton Dialog and returns the result and control values to caller.
        /// </summary>
        /// <returns></returns>
        public void Show(string formName)
        {
            FinalizeLayout(formName);

            if (formDictionary[formName].StartPosition == FormStartPosition.CenterParent)
            {
                Form frm = formDictionary[formName];
                Form parent = host.GetForm();
                frm.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                frm.Location = new System.Drawing.Point(
                    (parent.Location.X + parent.Width / 2) - (frm.Width / 2),
                    (parent.Location.Y + parent.Height / 2) - (frm.Height / 2)
                );
            }

            formDictionary[formName].Show(host.GetForm());
        }

    }
}
