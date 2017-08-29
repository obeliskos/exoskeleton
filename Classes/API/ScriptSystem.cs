using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exoskeleton.Classes.API
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class ScriptSystem: IDisposable
    {
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public ScriptSystem()
        {
        }

        public void Dispose()
        {
        }

        /// <summary>
        /// Get information about the system which this program is being run on.
        /// </summary>
        /// <returns>Json encoded system information object.</returns>
        public string GetSystemInfo()
        {
            var info = new
            {
                CommandLine = Environment.CommandLine,
                CurrentDirectory = Environment.CurrentDirectory,
                Is64BitOperatingSystem = Environment.Is64BitOperatingSystem,
                Is64BitProcess = Environment.Is64BitProcess,
                MachineName = Environment.MachineName,
                OSVersion = Environment.OSVersion,
                OSVersionString = Environment.OSVersion.ToString(),
                ProcessorCount = Environment.ProcessorCount,
                SystemDirectory = Environment.SystemDirectory,
                UserDomainName = Environment.UserDomainName,
                userName = Environment.UserName,
                DotNetVersion = Environment.Version
            };

            string sysinfo = JsonConvert.SerializeObject(info);

            return sysinfo;
        }

        /// <summary>
        /// Retrieves a single environment variable value.
        /// </summary>
        /// <param name="varName">The name of the environment variable to retrieve value for.</param>
        /// <returns>The string value of the environment variable (if found).</returns>
        public string GetEnvironmentVariable(string varName)
        {
            string result = Environment.GetEnvironmentVariable(varName);

            return result;
        }

        /// <summary>
        /// Returns a list of all environment variables as properties and property values.
        /// on a dynamic object
        /// </summary>
        /// <returns>Json object with properties representing variables</returns>
        public string GetEnvironmentVariables()
        {
            var result = Environment.GetEnvironmentVariables();

            string json = JsonConvert.SerializeObject(result);

            return json;
        }

        /// <summary>
        /// Sets an environment variable only within this process or child processes.
        /// </summary>
        /// <param name="varName">The name of the environment variable.</param>
        /// <param name="varValue">The value to assign to the environment variable.</param>
        public void SetEnvironmentVariable(string varName, string varValue)
        {
            Environment.SetEnvironmentVariable(varName, varValue);
        }

        /// <summary>
        /// Finds an existing application window by either class or name and focuses it.
        /// </summary>
        /// <param name="className">The class name of the window, or null.</param>
        /// <param name="windowName">The window name of the window, or null.</param>
        public void FocusWindow(string className, string windowName)
        {
            IntPtr ip = FindWindow(className, windowName);
            if (ip != null)
            {
                SetForegroundWindow(ip);
            }
        }

        /// <summary>
        /// Finds a window and sends keycodes to it.
        /// </summary>
        /// <param name="className">The class name of the window, or null.</param>
        /// <param name="windowName">The window name of the window, or null.</param>
        /// <param name="keys">String array of keys or keycodes to send.</param>
        /// <returns></returns>
        public bool FocusAndSendKeys(string className, string windowName, string[] keys)
        {
            IntPtr ip = FindWindow(className, windowName);

            if (ip == null) return false;

            SetForegroundWindow(ip);

            foreach(string key in keys)
            {
                SendKeys.SendWait(key);
            }

            SendKeys.Flush();

            return true;
        }

        /// <summary>
        /// Sends keys to another process by process name.
        /// </summary>
        /// <param name="processName">Name of process as appears in Task Manager</param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public bool SendKeysToProcess(string processName, string[] keys)
        {
            Process proc = Process.GetProcessesByName(processName).FirstOrDefault();
            if (proc == null)
            {
                return false;
            }

            SetForegroundWindow(proc.Handle);

            foreach (string key in keys)
            {
                SendKeys.SendWait(key);
            }

            SendKeys.Flush();

            return true;
        }
    }
}
