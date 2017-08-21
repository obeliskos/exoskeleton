using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exoskeleton.Classes.API
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class ScriptSystem
    {
        public string getSystemInfo()
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

    }
}
