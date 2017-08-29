using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Exoskeleton.Classes.API
{
    /// <summary>
    /// API class for doing simple COM scripting.
    /// </summary>
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class ScriptComObjects
    {
        private Type t = null;
        private object instance = null;

        /// <summary>
        /// Allows creation of global singletons for further operations.
        /// </summary>
        /// <param name="comObjectName">Com class type name to instance.</param>
        public void CreateInstance(string comObjectName)
        {
            t = Type.GetTypeFromProgID(comObjectName);
            instance = Activator.CreateInstance(t);
        }

        /// <summary>
        /// Allows invocation of a method on the global singleton com object instance.
        /// </summary>
        /// <param name="methodName">Com interface method to invoke.</param>
        /// <param name="methodParams">Parameters to pass to com interface method.</param>
        public void InvokeMethod(string methodName, string methodParams)
        {
            List<object> lo = JsonConvert.DeserializeObject<List<Object>>(methodParams);
            t.InvokeMember(methodName, BindingFlags.InvokeMethod, null, instance, lo.ToArray());
        }

        /// <summary>
        /// Activates instance to Com type, calls a single method (with params) and then disposes instance.
        /// </summary>
        /// <param name="comObjectName">Com class type name to instance.</param>
        /// <param name="methodName">Com interface method to invoke.</param>
        /// <param name="methodParams">Parameters to pass to com interface method.</param>
        public void CreateAndInvokeMethod(string comObjectName, string methodName, string methodParams)
        {
            List<object> lo = JsonConvert.DeserializeObject<List<Object>>(methodParams);
            Type t = Type.GetTypeFromProgID(comObjectName);
            object obj = Activator.CreateInstance(t);
            t.InvokeMember(methodName, BindingFlags.InvokeMethod, null, obj, lo.ToArray());
        }
    }
}
