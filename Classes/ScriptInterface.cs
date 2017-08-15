using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exoskeleton.Classes
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class ScriptInterface
    {
        public ScriptMain main;
        public ScriptMedia media = new ScriptMedia();
        public ScriptFile file = new ScriptFile();
        public ScriptProcess proc = new ScriptProcess();

        public ScriptInterface(Settings exosettings)
        {
            main = exosettings.ScriptingEnabled ? new ScriptMain() : null;
            media = exosettings.ScriptingMediaEnabled ? new ScriptMedia() : null;
            file = exosettings.ScriptingFilesEnabled ? new ScriptFile() : null;
            proc = exosettings.ScriptingProcessEnabled ? new ScriptProcess() : null;
        }

        public bool shutdown(WebBrowser browser)
        {
            try
            {
                browser.Document.InvokeScript("exoskeletonShutdown");
            }
            catch { };

            return true;
        }

        public string getSystemInfo()
        {
            var info = new
            {
                CommandLine = Environment.CommandLine,
                CurrentDirectory = Environment.CurrentDirectory,
                Is64BitOperatingSyste = Environment.Is64BitOperatingSystem,
                Is64BitProcess = Environment.Is64BitProcess,
                MachineName = Environment.MachineName,
                OSVersion = Environment.OSVersion,
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
