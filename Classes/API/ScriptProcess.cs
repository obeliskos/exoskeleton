using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exoskeleton.Classes.API
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class ScriptProcess: IDisposable
    {
        // Need this utility class to effectively deserialize json into a ProcessStartInfo instance
        private class SerializableExpandableContractResolver : DefaultContractResolver
        {
            protected override JsonContract CreateContract(Type objectType)
            {
                if (TypeDescriptor.GetAttributes(objectType).Contains(new TypeConverterAttribute(typeof(ExpandableObjectConverter))))
                {
                    return CreateObjectContract(objectType);
                }
                return base.CreateContract(objectType);
            }
        }

        public void Dispose()
        {
        }

        /// <summary>
        /// Starts a process resource by specifying the name of a document or application file.
        /// </summary>
        /// <param name="procPath">Program to execute.</param>
        /// <returns></returns>
        public string StartPath(string procPath)
        {
            Process p = Process.Start(procPath);

            // Some properties (their accessors) may throw exception if their value is non-applicable in the 
            // process current state so we will ignore errors by providing our own error delegate.
            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.Error += delegate (object sender, ErrorEventArgs args)
            {
                args.ErrorContext.Handled = true;
            };

            return JsonConvert.SerializeObject(p, jss);
        }

        /// <summary>
        /// Starts a process resource by providing information in a ProcessStartInfo format.
        /// </summary>
        /// <param name="startInfo">Serialized javascript object closely resembling a c# ProcessStartInfo object.</param>
        public string Start(string startInfo)
        {
            ProcessStartInfo psi = JsonConvert.DeserializeObject<ProcessStartInfo>(startInfo,
                new JsonSerializerSettings() { ContractResolver = new SerializableExpandableContractResolver() });

            Process p = new Process();
            p.StartInfo = psi;
            p.Start();

            dynamic dyn = new
            {
                Id = p.Id,
                CreationTime = p.ProcessName,
                CreationTimeUtc = p.MainWindowTitle,
            };

            return JsonConvert.SerializeObject(dyn);
        }

        /// <summary>
        /// Gets a list of running processes.
        /// </summary>
        /// <returns>Json encoded array of .net Process entries.</returns>
        public string GetProcesses()
        {
            Process[] result = Process.GetProcesses();

            // Some properties (their accessors) may throw exception if their value is non-applicable in the 
            // process current state so we will ignore errors by providing our own error delegate.
            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.Error += delegate (object sender, ErrorEventArgs args)
            {
                args.ErrorContext.Handled = true;
            };

            return JsonConvert.SerializeObject(result, jss);
        }

        /// <summary>
        /// Because GetProcesses takes so long to serialize, this may be more useful.
        /// </summary>
        /// <returns>Serialized array of objects containing process id, name, and window title (if applicable)</returns>
        public string GetProcessesSimplified()
        {
            Process[] pi = Process.GetProcesses();
            List<dynamic> dpi = new List<dynamic>();

            foreach (Process p in pi)
            {
                dpi.Add(
                    new
                    {
                        Id = p.Id,
                        CreationTime = p.ProcessName,
                        CreationTimeUtc = p.MainWindowTitle,
                    }
                );
            }

            return JsonConvert.SerializeObject(dpi);
        }

        /// <summary>
        /// Gets detailed process information by process id.
        /// </summary>
        /// <param name="id">The windows process id to get detailed information about.</param>
        /// <returns>The .net Process object for that process id, serialized as json</returns>
        public string GetProcessInfoById(int id)
        {
            Process pi = Process.GetProcessById(id);

            // Some properties (their accessors) may throw exception if their value is non-applicable in the 
            // process current state so we will ignore errors by providing our own error delegate.
            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.Error += delegate (object sender, ErrorEventArgs args)
            {
                args.ErrorContext.Handled = true;
            };

            return JsonConvert.SerializeObject(pi, jss);
        }

        /// <summary>
        /// Gets a list of processes of the provided name.
        /// </summary>
        /// <param name="name">name of process to get list of.</param>
        /// <returns>Json encoded array of .net Process entries.</returns>
        public string GetProcessesByName(string name)
        {
            Process[] result = Process.GetProcessesByName(name);

            // Some properties (their accessors) may throw exception if their value is non-applicable in the 
            // process current state so we will ignore errors by providing our own error delegate.
            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.Error += delegate (object sender, ErrorEventArgs args) 
            {
                args.ErrorContext.Handled = true;
            };

            return JsonConvert.SerializeObject(result, jss);
        }

        /// <summary>
        /// Kills a running process.
        /// </summary>
        /// <param name="id">The id of the process to kill.</param>
        /// <returns>true if found, or false if not.</returns>
        public bool KillProcessById(int id)
        {
            Process p = Process.GetProcessById(id);
            if (p == null)
            {
                return false;
            }

            p.Kill();
            return true;
        }
    }
}
