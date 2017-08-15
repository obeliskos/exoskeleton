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

        public bool WebServerSelfHost = false;
        public int WebServerListenPort = 8080;
        public string WebServerHostDirectory = "";

        public bool WebBrowserRefreshOnFirstLoad = true;
        public bool WebBrowserContextMenu = true;
        public string WebBrowserDefaultUrl = "";

        public bool ScriptingEnabled = true;
        public bool ScriptingFilesEnabled = true;
        public bool ScriptingProcessEnabled = true;
        public bool ScriptingMediaEnabled = true;

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
