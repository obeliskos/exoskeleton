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
using Exoskeleton.Classes.API;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Exoskeleton
{
    /// <summary>
    /// This windows form will serve as the app host container for our exoskelton app.
    /// Each 'app' can be customized with their own settings file to configure them or we will use default settings.
    /// We will set up a mime mapping xml file to use when self hosting.
    /// The Classes\ScriptInterface.cs class will serve as a scripting object for our exeskeleton app's javascript.
    /// </summary>
    public partial class MainForm : Form, IPrimaryHostWindow
    {
        private MimeTypeMappings mappings;
        private Settings settings;
        private LoggerForm loggerForm = null;
        private ScriptInterface scriptInterface;
        private List<IHostWindow> hostWindows = new List<IHostWindow>();
        private Dictionary<string, bool> cacheRefreshed = new Dictionary<string, bool>();

        private string environmentLocationSettings;
        private string environmentLocationCurrent { get { return Environment.CurrentDirectory; } }
        private string environmentLocationExecutable = Path.GetDirectoryName(Application.ExecutablePath);

        private string settingsPath;
        private string mappingsPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "mappings.xml");
        private Icon applicationIcon;
        private SimpleHTTPServer simpleServer = null;
        private bool fullscreen = false;
        // if multiple instances are running with same default port, actual port
        // will be next available
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
            string settingsFile = "settings.xos";
                
            // If there is at least one command line argument and it is an exoskeleton config file, use it.
            // If not, we will expect there to be a settings.xml file in the current directory or create one there ourselves.
            if (Environment.GetCommandLineArgs().Count() > 1)
            {
                string firstArgument = Environment.GetCommandLineArgs()[1];
                string firstArgExt = Path.GetExtension(firstArgument).ToLower();

                if (firstArgExt == ".xos" || firstArgExt == ".xml")
                {
                    settingsFile = firstArgument;
                    settingsFile.Replace("/", "\\");
                }

            }

            FileInfo fi = new FileInfo(settingsFile);
            bool createIfNotExists = false;
            environmentLocationSettings = fi.DirectoryName;
            settingsPath = fi.FullName;

            if (!fi.Exists)
            {
                DialogResult dr = MessageBox.Show(
                    "Could not find : " + settingsFile + Environment.NewLine + "Do you want to create that settings file?", 
                    "Could not find settings file", 
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                createIfNotExists = (dr == DialogResult.OK);
            }

            settings = Settings.Load(settingsPath, createIfNotExists);

            // Uncomment below line to easily 'upgrade' settings file with new settings and their defaults.
            // settings.Save(settingsPath);

            // Now that settings are loaded, determine if the user has elected to use the 
            // path containing the settings file as the 'current' directory.
            if (settings.CurrentDirectoryUseSettingsPath)
            {
                Environment.CurrentDirectory = environmentLocationSettings;
            }

            // Now that settings are loaded, determine if the user has elected to use a specific
            // (absolute or relative path) to be used as the 'current' directory.
            if (settings.CurrentDirectoryUseProvidedPath)
            {
                DirectoryInfo di;
                if (Path.IsPathRooted(settings.CurrentDirectoryProvidedPath))
                {
                    di = new DirectoryInfo(settings.CurrentDirectoryProvidedPath);
                }
                else
                {
                    di = new DirectoryInfo(Path.Combine(environmentLocationSettings, settings.CurrentDirectoryProvidedPath));
                };

                Environment.CurrentDirectory = di.FullName;
            }
        }

        /// <summary>
        /// When an external icon is specified in settings file, this will apply it
        /// </summary>
        private void InitializeIcon()
        {
            if (settings.WindowIconPath != "" && File.Exists(settings.WindowIconPath))
            {
                applicationIcon = new Icon(settings.WindowIconPath);
                this.Icon = applicationIcon;
                this.ExoskeletonNotification.Icon = this.Icon;
            }
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

            mappings = Classes.MimeTypeMappings.Load(mappingsPath);

            if (settings.WebServerSelfHost)
            {
                actualPort = startServer();
            }

            // if configured with startup commands, run them now...
            // we might use this to start a node process like a webserver
            foreach (string cmd in settings.StartupCommands)
            {
                Process.Start(cmd);
            }

            InitializeComponent();

            HostMenuStrip.Visible = settings.ScriptingMenuEnabled;
            HostToolStrip.Visible = settings.ScriptingToolStripEnabled;
            HostStatusStrip.Visible = settings.ScriptingStatusStripEnabled;

            hostWindows.Add(this);

            InitializeIcon();
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
            if (settings.ScriptingLoggerEnabled)
            {
                loggerForm = new LoggerForm(this, "Main");
                Rectangle workingArea = Screen.GetWorkingArea(this);
                loggerForm.Show();
                loggerForm.Location = new Point(workingArea.Right - loggerForm.Width, 100);
            }

            scriptInterface = new ScriptInterface(this, settings, loggerForm);

            this.HostWebBrowser.ScriptErrorsSuppressed = settings.WebBrowserScriptErrorsSuppressed;
            this.HostWebBrowser.WebBrowserShortcutsEnabled = settings.WebBrowserShortcutsEnabled;

            if (settings.WebBrowserDefaultUrl == "")
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("<h3>Exoskeleton default page</h3>");
                sb.Append("Please create a settings.xml file in the same folder as the executable");

                HostWebBrowser.DocumentText = sb.ToString();
            }
            else
            {
                string url = settings.WebBrowserDefaultUrl
                    .Replace("{port}", actualPort.ToString())
                    .Replace("{ExecutableLocation}", Path.GetDirectoryName(Application.ExecutablePath))
                    .Replace("{CurrentLocation}", environmentLocationCurrent)
                    .Replace("{SettingsLocation}", environmentLocationSettings);

                // For (only) filesystem based uri's, convert relative paths to absolute.
                Uri startingUri = null;
                try
                {
                    startingUri = new Uri(url);
                    if (startingUri.IsFile)
                    {
                        FileInfo fi = new FileInfo(url);
                        startingUri = new Uri(fi.FullName);
                    }
                }
                catch(Exception ex)
                {
                    FileInfo fi = new FileInfo(url);
                    startingUri = new Uri(fi.FullName);
                }

                try
                {
                    HostWebBrowser.Url = startingUri;
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error assigning host browser url", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (settings.WebBrowserAutoRefreshSecs > 0)
                {
                    RefreshTimer.Interval = settings.WebBrowserAutoRefreshSecs * 1000;
                    RefreshTimer.Enabled = true;
                }
            }

            HostWebBrowser.ObjectForScripting = scriptInterface;
            HostWebBrowser.IsWebBrowserContextMenuEnabled = settings.WebBrowserContextMenu;

            HostWebBrowser.Focus();
        }

        private string wrapEventData(string data)
        {
            List<string> wrapped = new List<string> { data };
            return JsonConvert.SerializeObject(wrapped.ToArray());
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
                PackageAndMulticast("shutdown", null);
            }


            if (settings.WebServerSelfHost && simpleServer != null)
            {
                try
                {
                    simpleServer.Stop();
                }
                catch { }
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

        #region WebBrowser Event Handlers and Eventing

        private void HostWebBrowser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (settings.WindowAllowFullscreenF11 && e.KeyCode == Keys.F11)
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
            if (settings.WebBrowserRefreshOnFirstLoad && 
                CheckForStaleCache(HostWebBrowser.Url.ToString()))
            {
                HostWebBrowser.Refresh(WebBrowserRefreshOption.Completely);
            }
        }

        public bool CheckForStaleCache(string url)
        {
            // if cache is stale, return true but assume caller is
            // going to refresh and add to our cacheRefreshed dictionary
            if (!cacheRefreshed.Keys.Contains(url))
            {
                cacheRefreshed[url] = true;
                return true;
            }

            return false;
        }

        #endregion

        #region IPrimaryHostWindow implementation methods

        public void RemoveHostWindow(IHostWindow hostWindow)
        {
            hostWindows.Remove(hostWindow);
        }

        #endregion

        #region IHostWindow implementation methods

        #region IHostWindow : Main UI methods

        public Form GetForm()
        {
            return this;
        }

        public void SetWindowTitle(string title)
        {
            this.Text = title;
        }

        public void OpenNewWindow(string caption, string url, int width, int height)
        {
            if (!url.StartsWith("@") && url.Contains("{port}"))
            {
                url = settings.WebBrowserBaseUrl.Replace("{port}", actualPort.ToString()) + url;
            }
            ChildWindow childWindow = new ChildWindow(this, caption, url, settings, width, height);
            hostWindows.Add(childWindow);
            childWindow.Show();
        }

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

        #region IHostWindow : Eventing and InvokeScript

        /// <summary>
        /// Preferred method to multicast an event from .net.  This will perform additional serialization to wrap
        /// parameters similarly to how javascript does it.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        public void PackageAndMulticast(string name, dynamic[] data)
        {
            string wrappedData = null;

            List<string> args = new List<string>();
            args.Add("multicast." + name);

            if (data != null)
            {
                for(int idx=0; idx < data.Length; idx++)
                {
                    data[idx] = JsonConvert.SerializeObject(data[idx]);
                }
                // now that we have array of strings (of serialized objects or values), serialize that.
                wrappedData = JsonConvert.SerializeObject(data);

                args.Add(wrappedData);
            }

            // now tell each IHostWindow to emit that event with those params
            foreach (IHostWindow hostWindow in hostWindows)
            {
                hostWindow.InvokeScript("exoskeletonEmitEvent", args.ToArray());
            }
        }

        /// <summary>
        /// Used for passing event data as serialized array of serialized data items.  
        /// Requires the javascript event handler to deserialize (JSON.parse()) data.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        public void PackageAndUnicast(string name, dynamic[] data)
        {
            string wrappedData = null;

            List<string> args = new List<string>();
            args.Add(name);

            if (data != null)
            {
                for (int idx = 0; idx < data.Length; idx++)
                {
                    data[idx] = JsonConvert.SerializeObject(data[idx]);
                }
                // now that we have array of strings (of serialized objects or values), serialize that.
                wrappedData = JsonConvert.SerializeObject(data);

                args.Add(wrappedData);
            }

            this.InvokeScript("exoskeletonEmitEvent", args.ToArray());
        }

        /// <summary>
        /// Used only as javascript API implementation and expects data to be already wrapped.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        public void MulticastEvent(string name, string data)
        {
            List<string> args = new List<string>();
            args.Add("multicast." + name);
            if (data != null)
            {
                args.Add(data);
            }

            foreach (IHostWindow hostWindow in hostWindows)
            {
                hostWindow.InvokeScript("exoskeletonEmitEvent", args.ToArray());
            }
        }

        /// <summary>
        /// Interface method so that host can broadcast events only to its own container.
        /// </summary>
        /// <param name="name">Event name</param>
        /// <param name="data">String encoded event data</param>
        public void UnicastEvent(string name, string data)
        {
            List<string> args = new List<string>();
            args.Add(name);
            if (data != null)
            {
                args.Add(data);
            }

            InvokeScript("exoskeletonEmitEvent", args.ToArray());
        }

        public object InvokeScript(string name, string[] args)
        {
            if (HostWebBrowser.Document == null) return null;

            return HostWebBrowser.Document.InvokeScript(name, args);
        }

        #endregion

        #region IHostWindow : Menu Management

        /// <summary>
        /// Enables visibility of the window's menustrip
        /// </summary>
        public void ShowMenu()
        {
            this.HostMenuStrip.Visible = true;
        }

        /// <summary>
        /// Hides visibility of the window's menustrip
        /// </summary>
        public void HideMenu()
        {
            this.HostMenuStrip.Visible = false;
        }

        public void InitializeMenuStrip()
        {
            HostMenuStrip.Items.Clear();
        }

        public void AddMenu(string menuName, string emitEventName)
        {
            ToolStripMenuItem newItem = new ToolStripMenuItem
            {
                Name = menuName,
                Tag = emitEventName,
                Text = menuName            };

            if (emitEventName != "")
            {
                newItem.Click += menuItem_Click;
            }

            HostMenuStrip.Items.Add(newItem); 
        }

        public void AddMenuItem(string menuName, string menuItemName, string emitEventName, string shortcutKeys)
        {
            ToolStripItem[] results = HostMenuStrip.Items.Find(menuName, true);
            if (results.Length > 0)
            {
                if (menuItemName == "-")
                {
                    ((ToolStripMenuItem)results[0]).DropDownItems.Add(new ToolStripSeparator());
                }
                else
                {
                    ToolStripMenuItem newItem = new ToolStripMenuItem
                    {
                        Name = menuItemName,
                        Tag = emitEventName,
                        Text = menuItemName
                    };

                    if (shortcutKeys != "")
                    {
                        string[] keyCodes = JsonConvert.DeserializeObject<string[]>(shortcutKeys);
                        Keys keys=0;

                        foreach(string keyCode in keyCodes)
                        {
                            keys = keys | (Keys)Enum.Parse(typeof(Keys), keyCode, true);
                        }
                        newItem.ShortcutKeys = keys;
                    }

                    if (emitEventName != "")
                    {
                        newItem.Click += menuItem_Click;
                    }

                    ((ToolStripMenuItem)results[0]).DropDownItems.Add(newItem);
                }
            }
        }

        /// <summary>
        /// Single Menu Handler to be used for all menu item click events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;

            // payload (2nd param) may evolve over time
            this.UnicastEvent((string) item.Tag, item.Name);
        }

        #endregion

        #region IHostWindow : Toolstrip Management

        /// <summary>
        /// Enables visibility of the window's toolstrip
        /// </summary>
        public void ShowToolstrip()
        {
            this.HostToolStrip.Visible = true;
        }

        /// <summary>
        /// Hides visibility of the window's toolstrip
        /// </summary>
        public void HideToolstrip()
        {
            this.HostToolStrip.Visible = false;
        }

        /// <summary>
        /// Clears the toolstrip labels
        /// </summary>
        public void InitializeToolstrip()
        {
            HostToolStrip.Items.Clear();
        }

        /// <summary>
        /// Add a button to the hostwindow toolstrip 
        /// </summary>
        /// <param name="text">Tooltip text to display on the toolstrip button</param>
        /// <param name="eventName">Name of event to fire when button is clicked</param>
        /// <param name="imagePath">Filepath of (rougly 32x32 px) image to display on button.</param>
        public void AddToolStripButton(string text, string eventName, string imagePath)
        {
            ToolStripButton tsb = new ToolStripButton();

            tsb.Tag = eventName;
            tsb.Text = text;
            tsb.ToolTipText = text;
            tsb.DisplayStyle = ToolStripItemDisplayStyle.Image;

            if (eventName != "")
            {
                tsb.Image = Image.FromFile(imagePath);
            }
            tsb.Click += toolStripButton_Click;

            HostToolStrip.Items.Add(tsb);
        }

        /// <summary>
        /// Adds a visual separator for toolstrip control groups
        /// </summary>
        public void AddToolStripSeparator()
        {
            ToolStripSeparator sep = new ToolStripSeparator();
            HostToolStrip.Items.Add(sep);
        }

        /// <summary>
        /// Single ToolStripButton Handler to be used for all button click events 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton_Click(object sender, EventArgs e)
        {
            ToolStripButton item = (ToolStripButton)sender;

            // payload (2nd param) may evolve over time
            this.UnicastEvent((string)item.Tag, item.Text);
        }

        #endregion

        #region IHostWindow : Statusstrip Management

        /// <summary>
        /// Enables visibility of the window's status strip
        /// </summary>
        public void ShowStatusstrip()
        {
            this.HostStatusStrip.Visible = true;
        }

        /// <summary>
        /// Hides visibility of the window's status strip
        /// </summary>
        public void HideStatusstrip()
        {
            this.HostStatusStrip.Visible = false;
        }

        /// <summary>
        /// Clears the text of both status strip labels
        /// </summary>
        public void InitializeStatusstrip()
        {
            SetLeftStatusstripLabel("");
            SetRightStatusstripLabel("");
        }

        /// <summary>
        /// Update the text displayed in the left toolstrip label
        /// </summary>
        /// <param name="text"></param>
        public void SetLeftStatusstripLabel(string text)
        {
            toolStripLeftLabel.Text = text;
        }

        /// <summary>
        /// Update the text displayed in the right toolstrip label
        /// </summary>
        /// <param name="text"></param>
        public void SetRightStatusstripLabel(string text)
        {
            toolStripRightLabel.Text = text;
        }

        #endregion

        /// <summary>
        /// Returns the 'active' settings class instance.
        /// </summary>
        /// <returns>The 'active' settings class instance.</returns>
        public Settings GetCurrentSettings()
        {
            return settings;
        }

        /// <summary>
        /// Returns the important exoskeleton environment locations. (Current, Settings, Executable)
        /// </summary>
        /// <returns>Dynamic object containing Executable, Settings, and Current locations.</returns>
        public dynamic GetLocations()
        {
            return new
            {
                Executable = this.environmentLocationExecutable,
                Settings = this.environmentLocationSettings,
                Current = this.environmentLocationCurrent
            };
        }

        /// <summary>
        /// This method can be called to shut down the exoskeleton application.
        /// </summary>
        public void Shutdown()
        {
            this.Close();
        }

        #endregion

        #region UI Event Handlers

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            HostWebBrowser.Refresh(WebBrowserRefreshOption.Completely);
        }

        #endregion

    }
}
