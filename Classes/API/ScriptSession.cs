using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exoskeleton.Classes.API
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class ScriptSession
    {
        public Dictionary<string, string> SessionHashtable = new Dictionary<string, string>();

        public string get(string key)
        {
            if (SessionHashtable.ContainsKey(key))
            {
                return SessionHashtable[key];
            }
            else
            {
                return null;
            }
        }

        public void set(string key, string value)
        {
            SessionHashtable[key] = value;
        }

        public string list()
        {
            List<string> result = new List<string>(SessionHashtable.Keys);

            return JsonConvert.SerializeObject(result);
        }

        // TODO: Possibly implement an eventing system which will work across multiple
        // pages; before this can be used i would need to implement functionality to 
        // intercept 'open in new window' logic to use a new winform container.  All opened
        // winform containers will reference the same ScriptInterface object instance.
        // This will also require javascript object for actual (local) attachment of listeners

        public void Emit(string Eventname, string eventData)
        {
            // foreach webbrowser control currently loaded, invokescript
            // to call matching exoskeleton emit function
        }
    }
}
