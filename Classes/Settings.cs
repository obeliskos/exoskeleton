using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Exoskeleton.Classes
{
    public class Settings
    {
        public List<string> StartupCommands = new List<string>();

        public string WindowTitle = "My Exoskeleton App";
        public int WindowWidth = 820;
        public int WindowHeight = 512;
        public bool WindowAllowFullscreenF11 = true;
        public bool WindowAllowResize = true;
        public string WindowIconPath = "";

        public bool WebServerSelfHost = false;
        public int WebServerListenPort = 8080;
        public string WebServerHostDirectory = "";

        public bool WebBrowserRefreshOnFirstLoad = true;
        public bool WebBrowserContextMenu = true;
        public string WebBrowserDefaultUrl = "";
        public string WebBrowserBaseUrl = "";
        public int WebBrowserAutoRefreshSecs = 0;
        public bool WebBrowserAllowChildWindows = true;
        public bool WebBrowserScriptErrorsSuppressed = true;

        // will probably want all of these on but when we generate
        // a default one (if none exists) we will use these defaults 
        // which are safe enough to run as a sitewrap app
        public bool ScriptingEnabled = true;
        public bool ScriptingMediaEnabled = true;
        public bool ScriptingFilesEnabled = false;
        public bool ScriptingProcessEnabled = false;
        public bool ScriptingSystemEnabled = false;
        public bool ScriptingLoggerEnabled = false;
        public bool ScriptingComObjectsEnabled = false;
        public bool ScriptingNetEnabled = false;
        public bool ScriptingEncryptionEnabled = true;

        public void Save(string filename)
        {
            XmlSerializer s = new XmlSerializer(this.GetType());

            using (TextWriter w = new StreamWriter(filename))
            {
                s.Serialize(w, this);
                w.Close();
            }
        }

        public static Settings Load(string filename)
        {
            Settings settings = new Settings();

            if (File.Exists(filename))
            {
                XmlSerializer s = new XmlSerializer(typeof(Settings));
                TextReader r = new StreamReader(filename);
                settings = (Settings)s.Deserialize(r);
                r.Close();
            }
            else
            {
                settings.Save(filename);
            }

            return settings;
        }

    }
}
