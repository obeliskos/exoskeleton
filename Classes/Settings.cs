using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Exoskeleton.Classes
{
    public class Settings
    {
        public List<string> StartupCommands { get; set; }  = new List<string>();

        /// <summary>
        /// Whether we should set current directory to settings path (true) 
        /// or use existing current directory (false)
        /// </summary>
        public bool CurrentDirectoryUseSettingsPath { get; set; } = true;
        /// <summary>
        /// If true, we will either 
        /// 'tweak' current directory with relative offset defined in CurrentDirectoryProvidedPath, or
        /// 'set' current directory with absolute path defined in CurrentDirectoryProvidedPath
        /// </summary>
        public bool CurrentDirectoryUseProvidedPath { get; set; } = false;
        /// <summary>
        /// Either an absolute path or relative modifer to effective 'current directory'
        /// </summary>
        public string CurrentDirectoryProvidedPath { get; set; } = "";

        public string ApplicationShortName { get; set; } = "DefaultExoApp";
        public string ApplicationDescription { get; set; } = "Default Exoskeleton App Description.";

        public string WindowTitle { get; set; } = "My Exoskeleton App";
        public int WindowWidth { get; set; } = 820;
        public int WindowHeight { get; set; } = 512;
        public bool WindowAllowFullscreenF11 { get; set; } = true;
        public bool WindowAllowResize { get; set; } = true;
        public string WindowIconPath { get; set; } = "";

        // When logger is enabled and the logger is minimized, we will 
        // notify of activity if these settings are enabled.
        public bool NotifyOnLoggedErrors { get; set; } = true;
        public bool NotifyOnLoggedWarnings { get; set; } = true;
        public bool NotifyOnLoggedInfo { get; set; } = true;

        public bool WebServerSelfHost { get; set; } = false;
        public int WebServerListenPort { get; set; } = 8080;
        public string WebServerHostDirectory { get; set; } = "";
        // Web services are enabled with these settings.
        // If a request is made for a url with this extension, we will
        // invoke javascript on the main window to process it.
        public bool WebServerServicesEnabled { get; set; } = false;
        public string WebServerServicesExtension { get; set; } = ".svc";

        /// <summary>
        /// If user sets this to true, we will hide the webbrowser and they
        /// will have to layout native controls on host panel for ui.
        /// </summary>
        public bool DefaultToNativeUi { get; set; } = false;

        public bool WebBrowserRefreshOnFirstLoad { get; set; } = true;
        public bool WebBrowserContextMenu { get; set; } = true;
        public string WebBrowserDefaultUrl { get; set; } = "";
        public string WebBrowserBaseUrl { get; set; } = "";
        public int WebBrowserAutoRefreshSecs { get; set; } = 0;
        public bool WebBrowserAllowChildWindows { get; set; } = true;
        public bool WebBrowserScriptErrorsSuppressed { get; set; } = true;
        public bool WebBrowserShortcutsEnabled { get; set; } = false;

        // will probably want all of these on but when we generate
        // a default one (if none exists) we will use these defaults 
        // which are safe enough to run as a sitewrap app
        public bool ScriptingEnabled { get; set; } = true;
        public bool ScriptingMediaEnabled { get; set; } = true;
        public bool ScriptingFilesEnabled { get; set; } = false;
        public bool ScriptingProcessEnabled { get; set; } = false;
        public bool ScriptingSystemEnabled { get; set; } = false;
        public bool ScriptingLoggerEnabled { get; set; } = true;
        public bool ScriptingComObjectsEnabled { get; set; } = false;
        public bool ScriptingNetEnabled { get; set; } = false;
        public bool ScriptingEncryptionEnabled { get; set; } = true;
        public bool ScriptingMenuEnabled { get; set; } = false;
        public bool ScriptingToolStripEnabled { get; set; } = false;
        public bool ScriptingStatusStripEnabled { get; set; } = false;
        public bool ScriptingDialogEnabled { get; set; } = true;
        public bool ScriptingFormEnabled { get; set; } = true;

        public void Save(string filename)
        {
            XmlSerializer s = new XmlSerializer(this.GetType());

            using (TextWriter w = new StreamWriter(filename))
            {
                s.Serialize(w, this);
                w.Close();
            }
        }

        public static Settings Load(string filename, bool createIfNotExists)
        {
            Settings settings = new Settings();

            if (File.Exists(filename))
            {
                XmlSerializer s = new XmlSerializer(typeof(Settings));
                TextReader r = new StreamReader(filename);
                try
                {
                    settings = (Settings)s.Deserialize(r);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error Loading settings", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                }
                finally
                {
                    r.Close();
                }
            }
            else
            {
                if (createIfNotExists)
                {
                    settings.Save(filename);
                }
            }

            return settings;
        }

    }
}
