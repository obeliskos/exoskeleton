using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exoskeleton.Classes.API
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class ScriptInterface
    {
        public ScriptMain main = null;
        public ScriptMedia media = null;
        public ScriptFile file = null;
        public ScriptProcess proc = null;
        public ScriptSession session = null;
        public ScriptSystem system = null;
        public ScriptLogger logger = null;

        public ScriptInterface(IHostWindow host, Settings exosettings, LoggerForm loggerForm)
        {
            main = exosettings.ScriptingEnabled ? new ScriptMain(host) : null;
            media = exosettings.ScriptingMediaEnabled ? new ScriptMedia() : null;
            file = exosettings.ScriptingFilesEnabled ? new ScriptFile() : null;
            proc = exosettings.ScriptingProcessEnabled ? new ScriptProcess() : null;
            system = exosettings.ScriptingSystemEnabled ? new ScriptSystem() : null;
            logger = exosettings.ScriptingLoggerEnabled ? new ScriptLogger(loggerForm) : null;
        }

        public void multicast(string eventName, string eventData)
        {
            MainForm.FormInstance.MulticastEvent(eventName, eventData);
        }

        public void shutdown()
        {
            MainForm.FormInstance.Close();
        }
    }
}
