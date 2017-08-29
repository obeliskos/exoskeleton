using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exoskeleton.Classes.API
{
    /// <summary>
    /// Implements and exposes a Key/Value Session storage 
    /// </summary>
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class ScriptSession: IDisposable
    {
        public Dictionary<string, string> SessionHashtable = new Dictionary<string, string>();

        public void Dispose()
        {
        }

        /// <summary>
        /// Lookups the Value for the Session key provided.
        /// </summary>
        /// <param name="key">The key name to lookup a value for in the session store.</param>
        /// <returns></returns>
        public string Get(string key)
        {
            if (!SessionHashtable.ContainsKey(key))
            {
                return null;
            }

            return SessionHashtable[key];
        }
        
        /// <summary>
        /// Assigns a key/value setting within the session store.
        /// </summary>
        /// <param name="key">The name of the session variable.</param>
        /// <param name="value">The string value of the session variable.</param>
        public void Set(string key, string value)
        {
            SessionHashtable[key] = value;
        }

        /// <summary>
        /// Obtains a string list of all keys currently in the session store.
        /// </summary>
        /// <returns>JSON encoded string array of key names.</returns>
        public string List()
        {
            List<string> result = new List<string>(SessionHashtable.Keys);

            return JsonConvert.SerializeObject(result);
        }
    }
}
