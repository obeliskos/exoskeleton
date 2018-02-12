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
using System.Runtime.InteropServices;

namespace Exoskeleton
{
    /// <summary>
    /// This windows form will serve as the app host container for our exoskelton app.
    /// Each 'app' can be customized with their own settings file to configure them or we will use default settings.
    /// We will set up a mime mapping xml file to use when self hosting.
    /// The Classes\ScriptInterface.cs class will serve as a scripting object for our exeskeleton app's javascript.
    /// </summary>
    public partial class MainForm : Form, IPrimaryHostWindow, IHostWindow
    {
        #region PInvoke stuff for system menu

        // P/Invoke constants
        private const int WM_SYSCOMMAND = 0x112;
        private const int MF_STRING = 0x0;
        private const int MF_SEPARATOR = 0x800;

        // P/Invoke declarations
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool AppendMenu(IntPtr hMenu, int uFlags, int uIDNewItem, string lpNewItem);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool InsertMenu(IntPtr hMenu, int uPosition, int uFlags, int uIDNewItem, string lpNewItem);

        // ID for the About item on the system menu
        private int SYSMENU_LOGGER_ID = 0x1;

        #endregion

        public string Title { get; set; } = "Main";
        public Settings Settings { get; set; } = null;
        public ILogWindow Logger { get; set; } = null;
        public Dictionary<string, ImageList> ImageListDictionary { get; set; }

        /// <summary>
        /// If multiple instances are running with same default port, actual port will be next available.
        /// </summary>
        public int ActualPort { get; set; } = 0;

        private MimeTypeMappings mappings;
        private GlobalSettings globalSettings;
        private LoggerForm loggerForm = null;
        private ScriptInterface scriptInterface;
        private List<IHostWindow> hostWindows = new List<IHostWindow>();
        private Dictionary<string, bool> cacheRefreshed = new Dictionary<string, bool>();

        public ScriptInterface GetScriptInterface { get { return scriptInterface; } }

        private string environmentLocationSettings;
        private string environmentLocationCurrent { get { return Environment.CurrentDirectory; } }
        private string environmentLocationExecutable = Path.GetDirectoryName(Application.ExecutablePath);

        private string settingsPath;
        private string mappingsPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "exo-mappings.xml");
        private string globalSettingsPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "exo-global-settings.xml");

        private Icon applicationIcon;
        public Icon DefaultIcon { get; set; }

        private SimpleHTTPServer simpleServer = null;
        private bool fullscreen = false;

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
            int argCount = Environment.GetCommandLineArgs().Count();
            bool argsExist = argCount > 1;
            bool explicitSettings = false;
                
            // If there is at least one command line argument and it is an exoskeleton config file, use it.
            // If not, we will expect there to be a settings.xml file in the current directory or create one there ourselves.
            if (argsExist)
            {
                string firstArgument = Environment.GetCommandLineArgs()[1];
                string firstArgExt = Path.GetExtension(firstArgument).ToLower();

                if (firstArgExt == ".xos" || firstArgExt == ".xml")
                {
                    settingsFile = firstArgument;
                    settingsFile.Replace("/", "\\");
                    explicitSettings = true;
                }
            }

            FileInfo fi = new FileInfo(settingsFile);
            bool createIfNotExists = false;
            environmentLocationSettings = fi.DirectoryName;
            settingsPath = fi.FullName;

            // If settings were not found (either via explicit cl param or implicit settings.xos in currdir)
            if (!fi.Exists)
            {
                if (!explicitSettings && globalSettings.EnableLaunchHistory)
                {
                    HistorySelectorForm hsf = new HistorySelectorForm(this, globalSettings);
                    DialogResult dr = hsf.ShowDialog();

                    if (dr == DialogResult.Cancel)
                    {
                        Application.Exit();
                        Application.DoEvents();
                        Environment.Exit(0);
                        return;
                    }

                    if (HistorySelectorForm.SelectedSettingsFile != "")
                    {
                        settingsPath = HistorySelectorForm.SelectedSettingsFile;
                        fi = new FileInfo(settingsPath);
                    }
                }

                if (!fi.Exists)
                {
                    DialogResult dr = MessageBox.Show(
                        "Could not find : " + Environment.NewLine +
                        settingsPath + Environment.NewLine + "Do you want to create that settings file?",
                        "Could not find settings file",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                    createIfNotExists = (dr == DialogResult.OK);
                }
            }

            this.Settings = Settings.Load(settingsPath, createIfNotExists);

            fi = new FileInfo(settingsPath);
            environmentLocationSettings = fi.DirectoryName;

            // Uncomment below line to easily 'upgrade' settings file with new settings and their defaults.
            //if (fi.Exists) Settings.Save(settingsPath);

            if (globalSettings.EnableLaunchHistory && File.Exists(settingsPath))
            {
                AddLaunchHistory(
                    settingsPath,
                    this.Settings.ApplicationShortName,
                    this.Settings.ApplicationDescription,
                    this.Settings.WindowIconPath
                );
            }

            // Now that settings are loaded, determine if the user has elected to use the 
            // path containing the settings file as the 'current' directory.
            if (this.Settings.CurrentDirectoryUseSettingsPath)
            {
                Environment.CurrentDirectory = environmentLocationSettings;
            }

            // Now that settings are loaded, determine if the user has elected to use a specific
            // (absolute or relative path) to be used as the 'current' directory.
            if (this.Settings.CurrentDirectoryUseProvidedPath)
            {
                DirectoryInfo di;
                if (Path.IsPathRooted(this.Settings.CurrentDirectoryProvidedPath))
                {
                    di = new DirectoryInfo(this.Settings.CurrentDirectoryProvidedPath);
                }
                else
                {
                    di = new DirectoryInfo(Path.Combine(environmentLocationSettings, 
                        this.Settings.CurrentDirectoryProvidedPath));
                };

                Environment.CurrentDirectory = di.FullName;
            }
        }

        public void AddLaunchHistory(string settingsPath, string shortName, string description, string iconPath)
        {
            FileInfo fi = new FileInfo(settingsPath);

            string revisedIconPath = iconPath
                .Replace("{CurrentLocation}", Environment.CurrentDirectory)
                .Replace("{SettingsLocation}", fi.DirectoryName);

            globalSettings.AddLaunchHistory(
                settingsPath,
                shortName,
                description,
                revisedIconPath
            );
            globalSettings.Save(globalSettingsPath);
        }

        public void RemoveLaunchHistory(string settingsPath)
        {
            ExoAppLaunchInfo ali = (
                from ah in globalSettings.AppHistory
                where ah.SettingsPath == settingsPath
                select ah
            ).FirstOrDefault();

            if (ali != null)
            {
                globalSettings.AppHistory.Remove(ali);
                globalSettings.Save(globalSettingsPath);
            }
        }

        /// <summary>
        /// When an external icon is specified in settings file, this will apply it
        /// </summary>
        private void InitializeIcon()
        {
            if (String.IsNullOrEmpty(this.Settings.WindowIconPath)) return;

            string resolvedIconPath = ResolveExoUrlPath(this.Settings.WindowIconPath);

            if (File.Exists(resolvedIconPath))
            {
                applicationIcon = new Icon(resolvedIconPath);
                this.Icon = applicationIcon;
                this.ExoskeletonNotification.Icon = this.Icon;
            }
        }

        /// <summary>
        /// Override of OnHandleCreated to show Logger menu option (if enabled) in system menu.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (Settings.ScriptingLoggerEnabled)
            {
                // Get a handle to a copy of this form's system (window) menu
                IntPtr hSysMenu = GetSystemMenu(this.Handle, false);

                // Add a separator
                AppendMenu(hSysMenu, MF_SEPARATOR, 0, string.Empty);

                // Add the About menu item
                AppendMenu(hSysMenu, MF_STRING, SYSMENU_LOGGER_ID, "&Logger…");
            }

        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // Test if the About item was selected from the system menu
            if ((m.Msg == WM_SYSCOMMAND) && ((int)m.WParam == SYSMENU_LOGGER_ID))
            {
                showLogger();
            }
        }


        /// <summary>
        /// Form constructor logic where we will establish settings, registry keys, 
        /// (optionally) start self-hosting web server and invoke any configured startup processes 
        /// (in case you want to load a nodejs web server, for example).
        /// </summary>
        public MainForm()
        {
            mappings = Classes.MimeTypeMappings.Load(mappingsPath);
            globalSettings = Classes.GlobalSettings.Load(globalSettingsPath);

            // Preserve default exoskeleton icon for app history launcher, if needed
            this.DefaultIcon = this.Icon;

            this.ImageListDictionary = new Dictionary<string, ImageList>();

            InitializeSettings();
            SetupRegistryKeys();

            if (this.Settings.WebServerSelfHost)
            {
                startServer();
            }

            // if configured with startup commands, run them now...
            // we might use this to start a node process like a webserver
            foreach (string cmd in this.Settings.StartupCommands)
            {
                Process.Start(cmd);
            }

            InitializeComponent();

            if (this.Settings.DefaultToNativeUi)
            {
                SwitchToNativeUi();
            }

            HostMenuStrip.Visible = this.Settings.ScriptingMenuEnabled;
            HostToolStrip.Visible = this.Settings.ScriptingToolStripEnabled;
            HostStatusStrip.Visible = this.Settings.ScriptingStatusStripEnabled;

            hostWindows.Add(this);

            InitializeIcon();
            this.Width = this.Settings.WindowWidth;
            this.Height = this.Settings.WindowHeight;
            this.Text = this.Settings.WindowTitle;
        }

        public string ResolveExoUrlPath(string url)
        {
            string result = url
                .Replace("{port}", ActualPort.ToString())
                .Replace("{ExecutableLocation}", Path.GetDirectoryName(Application.ExecutablePath))
                .Replace("{CurrentLocation}", environmentLocationCurrent)
                .Replace("{SettingsLocation}", environmentLocationSettings);

            return result;
        }

        /// <summary>
        /// Window load event where we will set up WebBrowser control to load our app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            if (this.Settings.ScriptingLoggerEnabled)
            {
                loggerForm = new LoggerForm();
                loggerForm.AttachHost(this, Title, this.Icon);
                Logger = loggerForm;
                loggerForm.Show();
            }
            else
            {
                consoleLogToolStripMenuItem.Visible = false;
                notificationIconToolStripSeparator.Visible = false;
            }

            scriptInterface = new ScriptInterface(this, loggerForm);

            this.HostWebBrowser.ScriptErrorsSuppressed = this.Settings.WebBrowserScriptErrorsSuppressed;
            this.HostWebBrowser.WebBrowserShortcutsEnabled = this.Settings.WebBrowserShortcutsEnabled;

            if (this.Settings.WebBrowserDefaultUrl == "")
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("<h3>Exoskeleton default page</h3>");
                sb.Append("Please create a settings.xml file in the same folder as the executable");

                HostWebBrowser.DocumentText = sb.ToString();
            }
            else
            {
                string url = ResolveExoUrlPath(this.Settings.WebBrowserDefaultUrl);

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
                catch
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

                if (this.Settings.WebBrowserAutoRefreshSecs > 0)
                {
                    RefreshTimer.Interval = this.Settings.WebBrowserAutoRefreshSecs * 1000;
                    RefreshTimer.Enabled = true;
                }
            }

            HostWebBrowser.ObjectForScripting = scriptInterface;
            HostWebBrowser.IsWebBrowserContextMenuEnabled = this.Settings.WebBrowserContextMenu;

            HostWebBrowser.Focus();
        }

        public void SwitchToNativeUi()
        {
            WebBrowser wb = HostWebBrowser;

            HostWebBrowser.Visible = false;
            HostWebBrowser.Dock = DockStyle.None;
            HostWebBrowser.Height = 1;
            HostWebBrowser.Width = 1;
            HostPanel.Controls.Remove(HostWebBrowser);
            this.Controls.Add(wb);
        }

        public void SwitchToMixedUi(string browserParentPanel)
        {
            WebBrowser wb = HostWebBrowser;

            HostPanel.Controls.Remove(HostWebBrowser);
            HostWebBrowser.Visible = true;
            HostWebBrowser.Dock = DockStyle.Fill;

            Panel pnl = this.Controls.Find(browserParentPanel, true).FirstOrDefault() as Panel;
            pnl.Controls.Clear();
            pnl.Controls.Add(wb);
        }

        public void SwitchToWebUi()
        {
            WebBrowser wb = HostWebBrowser;
            wb.Parent.Controls.Remove(wb);

            HostPanel.Controls.Clear();
            HostPanel.Controls.Add(wb);
            HostWebBrowser.Dock = DockStyle.Fill;
            HostWebBrowser.Visible = true;
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
            if (this.Settings.ScriptingEnabled)
            {
                PackageAndMulticast("shutdown", null);
            }


            if (this.Settings.WebServerSelfHost && simpleServer != null)
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
            ActualPort = SimpleHTTPServer.GetOpenPort(this.Settings.WebServerListenPort);

            string hostDirectory = ResolveExoUrlPath(this.Settings.WebServerHostDirectory);
            simpleServer = new SimpleHTTPServer(this, this.Settings, hostDirectory, ActualPort, mappings);

            return ActualPort;
        }
        #endregion

        #region WebBrowser Event Handlers and Eventing

        private void HostWebBrowser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (this.Settings.WindowAllowFullscreenF11 && e.KeyCode == Keys.F11)
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
            if (this.Settings.WebBrowserRefreshOnFirstLoad && 
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

        /// <summary>
        /// Used to marshall a web service request through the Host Web Browser via InvokeScript.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public dynamic ProcessServiceRequest(dynamic request)
        {
            if (!this.Settings.WebServerServicesEnabled) return null;

            string[] requestParams = { JsonConvert.SerializeObject(request) };

            if (HostWebBrowser.InvokeRequired)
            {
                string ijson = (string) this.Invoke(
                    new Func<object>(() => 
                        HostWebBrowser.Document.InvokeScript("_exoskeletonProcessServiceRequest", requestParams))
                );

                if (ijson == null) return null;

                dynamic dyno = JsonConvert.DeserializeObject(ijson);
                return dyno;
            }

            string json = (string) HostWebBrowser.Document.InvokeScript("_exoskeletonProcessServiceRequest", requestParams);

            if (json == null) return null;

            return JsonConvert.DeserializeObject(json);
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

        public void OpenNewWindow(string caption, string url, int width, int height, string mode = "")
        {
            if (!url.StartsWith("@") && url.Contains("{port}"))
            {
                url = this.Settings.WebBrowserBaseUrl.Replace("{port}", ActualPort.ToString()) + url;
            }

            ChildWindow childWindow = new ChildWindow(this, Logger, this.Settings, 
                caption, url, width, height, mode);

            hostWindows.Add(childWindow);
            childWindow.Show();
        }

        public void ShowNotification(int timeout, string tipTitle, string tipText, ToolTipIcon toolTipIcon)
        {
            ExoskeletonNotification.BalloonTipTitle = tipTitle;
            ExoskeletonNotification.BalloonTipText = tipText;
            ExoskeletonNotification.BalloonTipIcon = toolTipIcon;
            ExoskeletonNotification.ShowBalloonTip(timeout);
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
        public void PackageAndMulticast(string name, dynamic data)
        {
            // InvokeScript handles params as string array
            // our event name will be first (0) element
            // out serialized event data will be second (1) element
            string[] wrappedJson = { "multicast." + name, JsonConvert.SerializeObject(data) };

            // now tell each IHostWindow to emit that event with those params
            foreach (IHostWindow hostWindow in hostWindows)
            {
                hostWindow.InvokeScript("_exoskeletonEmitEvent", wrappedJson);
            }
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
                hostWindow.InvokeScript("_exoskeletonEmitEvent", args.ToArray());
            }
        }

        /// <summary>
        /// Internal method used for unicasting event and passing data.
        /// This is only public so that Scripting Interface c# can utilize it.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        public void PackageAndUnicast(string name, dynamic data)
        {
            // InvokeScript handles params as string array
            // our event name will be first (0) element
            // out serialized event data will be second (1) element
            string[] wrappedJson = { name, JsonConvert.SerializeObject(data) };

            this.InvokeScript("_exoskeletonEmitEvent", wrappedJson);
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
            this.PackageAndUnicast((string) item.Tag, item.Name);
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
            this.PackageAndUnicast((string)item.Tag, item.Text);
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
        /// Used by FormLayoutBase (Form API) for NativeUiOnly apps.
        /// </summary>
        /// <returns></returns>
        public Panel GetHostPanel()
        {
            return this.HostPanel;
        }

        /// <summary>
        /// Returns the 'active' settings class instance.
        /// </summary>
        /// <returns>The 'active' settings class instance.</returns>
        public Settings GetCurrentSettings()
        {
            return this.Settings;
        }

        /// <summary>
        /// Returns the important exoskeleton environment locations. (Current, Settings, Executable)
        /// Also including web server url locations (for use when self hosting)
        /// </summary>
        /// <returns>Dynamic object containing Executable, Settings, and Current locations.</returns>
        public dynamic GetLocations()
        {
            string webServerRoot = Settings.WebServerSelfHost ?
                ResolveExoUrlPath(Settings.WebBrowserBaseUrl) :
                null;

            int webServerListenPort = Settings.WebServerListenPort;

            int webServerActualPort = Settings.WebServerSelfHost ?
                ActualPort : 0;

            return new
            {
                Executable = this.environmentLocationExecutable,
                Settings = this.environmentLocationSettings,
                Current = this.environmentLocationCurrent,
                Documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Music = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                Pictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                Videos = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                WebServerSelfHost = Settings.WebServerSelfHost,
                WebServerRoot = webServerRoot,
                WebServerListenPort = webServerListenPort,
                WebServerActualPort = webServerActualPort
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

        private void showLogger()
        {
            if (!this.Settings.ScriptingLoggerEnabled) return;

            Rectangle workingArea = Screen.GetWorkingArea(this);
            loggerForm.Location = new Point(workingArea.Right - loggerForm.RestoreBounds.Width, 100);
            loggerForm.WindowState = FormWindowState.Normal;
            loggerForm.ShowInTaskbar = true;
        }
        private void consoleLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showLogger();
        }

        private void ExoskeletonNotification_BalloonTipClicked(object sender, EventArgs e)
        {
            NotifyIcon ni = (NotifyIcon)sender;

            // Somewhat hackish way to handle balloon tip click event differently
            // when it came from logger... in that case restore logger so its visible.
            if (ni.BalloonTipTitle.StartsWith("Console "))
            {
                showLogger();
            }
        }

        #endregion

    }
}
