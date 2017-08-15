using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Exoskeleton.Classes;
using System.Diagnostics;

namespace Exoskeleton
{
    /// <summary>
    /// This windows form will serve as the app host container for our exoskelton app.
    /// Each 'app' can be customized with their own settings file to configure them or we will use default settings.
    /// We will set up a mime mapping xml file to use when self hosting.
    /// The Classes\ScriptInterface.cs class will serve as a scripting object for our exeskeleton app's javascript.
    /// </summary>
    public partial class MainForm : Form
    {
        public bool isAdmin = false;
        public static MainForm formInstance = null;

        private string settingsPath;
        private string mappingsPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\mappings.xml";
        public Settings settings;
        public MimeTypeMappings mappings;

        private SimpleHTTPServer simpleServer = null;
        ScriptInterface scriptInterface;

        private bool fullscreen = false;
        private DateTime lastFsDateTime = DateTime.MinValue;

        private bool cacheIsStale = true;

        private int actualPort = 0;

        #region Form Initialization and Shutdown

        /// <summary>
        /// In order to have the .net webbrowser control render in ie 11 instead of default ie7 mode, we need to configure
        /// feature_browser_emulation registy key for this exe/filepath.
        /// </summary>
        private void SetupRegistryKeys()
        {
            var exeName = System.AppDomain.CurrentDomain.FriendlyName;
            object tk = Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION", exeName, "");

            // Only need to do this the first time they run this app
            if (tk == null || tk.ToString() == "")
            {
                RegistryKey mrk = Registry.CurrentUser.OpenSubKey("Software", false);
                mrk = mrk.OpenSubKey("Microsoft", false);
                mrk = mrk.OpenSubKey("Internet Explorer", false);
                mrk = mrk.OpenSubKey("Main", true);

                RegistryKey fc = mrk.OpenSubKey("FeatureControl", true);
                if (fc == null)
                {
                    fc = mrk.CreateSubKey("FeatureControl");
                }

                mrk = mrk.OpenSubKey("FeatureControl", true);

                RegistryKey fbr = mrk.OpenSubKey("FEATURE_BROWSER_EMULATION", true);
                if (fbr == null)
                {
                    fbr = mrk.CreateSubKey("FEATURE_BROWSER_EMULATION");
                }
                RegistryKey fgr = mrk.OpenSubKey("FEATURE_GPU_RENDERING", true);
                if (fgr == null)
                {
                    fgr = mrk.CreateSubKey("FEATURE_GPU_RENDERING");
                }

                RegistryKey far = mrk.OpenSubKey("FEATURE_ALIGNED_TIMERS", true);
                if (far == null)
                {
                    far = mrk.CreateSubKey("FEATURE_ALIGNED_TIMERS");
                }

                object rk = fbr.GetValue(exeName, "");
                if (rk is string && (string)rk == "")
                {
                    fbr.SetValue(exeName, 11001, RegistryValueKind.DWord);
                }

                object rk2 = fgr.GetValue(exeName, "");
                if (rk2 is string && (string)rk2 == "")
                {
                    fgr.SetValue(exeName, 1, RegistryValueKind.DWord);
                }

                object rk3 = far.GetValue(exeName, "");
                if (rk3 is string && (string)rk3 == "")
                {
                    far.SetValue(exeName, 1, RegistryValueKind.DWord);
                }
            }
        }

        /// <summary>
        /// Determines what filename to use or establish for maintaining individual app settings.
        /// </summary>
        private void InitializeSettings()
        {
            string settingsFile = "settings.xml";

            // For starters, we will only allow 1 command line argument which is a filename/filepath to a xml settings file
            if (Environment.GetCommandLineArgs().Count() > 1)
            {

                settingsFile = Environment.GetCommandLineArgs()[1];
                settingsFile.Replace("/", "\\");

                // if it is a path
                if (settingsFile.Contains("\\"))
                {
                    settingsPath = settingsFile;
                }
                else
                {
                    if (!settingsFile.ToLower().Contains(".xml"))
                    {
                        settingsFile = settingsFile + ".xml";
                    }

                }
            }

            settingsPath = Directory.GetCurrentDirectory() + "\\" + settingsFile;

            settings = Settings.Load(settingsPath);
        }

        /// <summary>
        /// Form constructor logic where we will establish settings, registry keys, 
        /// (optionally) start self-hosting web server and invoke any configured startup processes 
        /// (in case you want to load a nodejs web server, for example).
        /// </summary>
        public MainForm()
        {
            InitializeSettings();
            SetupRegistryKeys();

            formInstance = this;
            isAdmin = IsAdministrator();

            mappings = Classes.MimeTypeMappings.Load(mappingsPath);
            scriptInterface = new ScriptInterface(settings);

            if (settings.WebServerSelfHost)
            {
                actualPort = startServer();
            }

            // if configured with startup commands, run them now...
            // we might use this to start a node process like a webserver
            foreach(string cmd in settings.StartupCommands)
            {
                Process.Start(cmd);
            }

            InitializeComponent();

            this.Width = settings.WindowWidth;
            this.Height = settings.WindowHeight;
            this.Text = settings.WindowTitle;
        }

        /// <summary>
        /// Window load event where we will set up WebBrowser control to load our app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            if (settings.WebBrowserDefaultUrl == "")
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("<h3>Exoskeleton default page</h3>");
                sb.Append("Please create a settings.xml file in the same folder as the executable");

                HostWebBrowser.DocumentText = sb.ToString();
            }
            else
            {
                HostWebBrowser.Url = new Uri(settings.WebBrowserDefaultUrl.Replace("{port}", actualPort.ToString()));
            }

            HostWebBrowser.ObjectForScripting = scriptInterface;
            HostWebBrowser.IsWebBrowserContextMenuEnabled = settings.WebBrowserContextMenu;

            HostWebBrowser.Focus();
        }

        /// <summary>
        /// Listens to form closing event so that it can shutdown webserver (if self-hosting),
        /// as well as notify the hosted application via javascript in case it needs to shutdown/flush any changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (settings.ScriptingEnabled)
            {
                var result = scriptInterface.shutdown(HostWebBrowser);
            }


            if (settings.WebServerSelfHost && simpleServer != null)
            {
                try
                {
                    simpleServer.Stop();
                }
                catch {}
            }
        }

        #endregion

        #region WebServer logic

        // Leaving in case we add optional http server features which require admin
        public static bool IsAdministrator()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
                    .IsInRole(WindowsBuiltInRole.Administrator);
        }

        private int startServer()
        {
            int port = SimpleHTTPServer.GetOpenPort(settings.WebServerListenPort);
            simpleServer = new SimpleHTTPServer(settings.WebServerHostDirectory, port, mappings);

            return port;
        }

        #endregion

        #region WebBrowser Event Handlers

        private void HostWebBrowser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.F11)
            {
                ToggleFullscreen();
            }
        }

        /// <summary>
        /// The .net webbrowser control aggressively caches content, so we can force refresh on first load if 'RefreshOnFirstLoad' option is set.
        /// May need to turn this bool flag into hashtable based on url so that nested web pages are refreshed on first load as well.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HostWebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (settings.WebBrowserRefreshOnFirstLoad && cacheIsStale)
            {
                cacheIsStale = false;
                HostWebBrowser.Refresh(WebBrowserRefreshOption.Completely);
            }
        }

        #endregion

        #region Form Level ScriptingHandlers

        public void ShowNotification(int timeout, string tipTitle, string tipText, ToolTipIcon toolTipIcon)
        {
            ExoskeletonNotification.ShowBalloonTip(timeout, tipTitle, tipText, toolTipIcon);
        }

        public void ToggleFullscreen()
        {
            if (fullscreen)
            {
                fullscreen = false;
                ExitFullscreen();
            }
            else
            {
                fullscreen = true;
                EnterFullscreen();
            }
        }

        public void EnterFullscreen()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            HostWebBrowser.Focus();
        }

        public void ExitFullscreen()
        {
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.WindowState = FormWindowState.Normal;

            HostWebBrowser.Focus();
        }

        #endregion
    }
}
