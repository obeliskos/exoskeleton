using Exoskeleton.Classes;
using Exoskeleton.Classes.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exoskeleton
{
    public partial class ChildWindow : Form, IHostWindow
    {
        public Settings Settings { get; set; }
        public ILogWindow Logger { get; set; }
        public string Title { get; set; }
        public Dictionary<string, ImageList> ImageListDictionary { get; set; }

        private IPrimaryHostWindow parent;
        private ScriptInterface scriptInterface;
        private static Dictionary<string, bool> cacheRefreshed = new Dictionary<string, bool>();
        private bool fullscreen = false;
        private string uri;

        public ScriptInterface GetScriptInterface { get { return scriptInterface; } }

        #region Form Level Constructor and Events

        public ChildWindow(IPrimaryHostWindow parent, ILogWindow logger, Settings settings, 
            string caption, string uri, int? width, int? height, string mode)
        {
            this.parent = parent;
            this.Settings = settings;
            this.uri = uri;
            this.Title = caption;

            this.ImageListDictionary = new Dictionary<string, ImageList>();

            Logger = logger;
            Logger.AttachHost(this, caption, null);

            if (!String.IsNullOrEmpty(settings.WindowIconPath))
            {
                string resolvedIconPath = parent.ResolveExoUrlPath(settings.WindowIconPath);
                this.Icon = new Icon(resolvedIconPath);
            }

            InitializeComponent();

            scriptInterface = new ScriptInterface(this, Logger);

            // The default mode that the form is compiled with is web ui mode.
            // This can be overriden in settings to native ui
            // You can override either at run time with 'mode' param
            // API should pass either 'native' or 'web'
            if (!String.IsNullOrEmpty(mode))
            {
                if (mode == "native")
                {
                    SwitchToNativeUi();
                }
            }
            else if (settings.DefaultToNativeUi)
            {
                SwitchToNativeUi();
            }

            HostMenuStrip.Visible = settings.ScriptingMenuEnabled;
            HostToolStrip.Visible = settings.ScriptingToolStripEnabled;
            HostStatusStrip.Visible = settings.ScriptingStatusStripEnabled;

            if (caption != null) this.Text = caption;
            if (width.HasValue) this.Width = width.Value;
            if (height.HasValue) this.Height = height.Value;

            HostWebBrowser.ScriptErrorsSuppressed = settings.WebBrowserScriptErrorsSuppressed;
            HostWebBrowser.WebBrowserShortcutsEnabled = settings.WebBrowserShortcutsEnabled;

            HostWebBrowser.ObjectForScripting = scriptInterface;
            HostWebBrowser.IsWebBrowserContextMenuEnabled = settings.WebBrowserContextMenu;
        }

        private void ChildWindow_Load(object sender, EventArgs e)
        {
            if (uri.StartsWith("@"))
            {
                HostWebBrowser.DocumentText = uri.Substring(1);
            }
            else
            {
                string baseUrl = parent.ResolveExoUrlPath(this.Settings.WebBrowserBaseUrl);

                Uri baseUri = new Uri(baseUrl);
                HostWebBrowser.Url = new Uri(baseUri, uri);
            }
        }

        private void ChildWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            parent.RemoveHostWindow(this);
            this.scriptInterface.Dispose();
        }

        /// <summary>
        /// Internal method used for multicasting event and passing data.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        public void PackageAndMulticast(string name, dynamic data)
        {
            parent.PackageAndMulticast(name, data);
        }

        /// <summary>
        /// This could be used by scripting to multicast out of their window.
        /// They will be packaging and passing serialized data.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        public void MulticastEvent(string name, string data)
        {
            parent.MulticastEvent(name, data);
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

        #endregion

        #region Form Level ScriptingHandlers

        public Form GetForm()
        {
            return this;
        }

        /// <summary>
        /// Used by FormLayoutBase (Form API) for NativeUiOnly apps.
        /// </summary>
        /// <returns></returns>
        public Panel GetHostPanel()
        {
            return this.HostPanel;
        }

        public void SetWindowTitle(string title)
        {
            this.Text = title;
        }

        public void ShowNotification(int timeout, string tipTitle, string tipText, ToolTipIcon toolTipIcon)
        {
            parent.ShowNotification(timeout, tipTitle, tipText, toolTipIcon);
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

        public void OpenNewWindow(string caption, string url, int width, int height, string mode = "")
        {
            parent.OpenNewWindow(caption, url, width, height, mode);
        }

        #endregion

        #region WebBrowser events and utility methods

        public bool CheckForStaleCache(string url)
        {
            if (url.StartsWith("@"))
            {
                return false;
            }

            // if cache is stale, return true but assume caller is
            // going to refresh and add to our cacheRefreshed dictionary
            if (!cacheRefreshed.Keys.Contains(url))
            {
                cacheRefreshed[url] = true;
                return true;
            }

            return false;
        }

        private void ChildWebBrowser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (this.Settings.WindowAllowFullscreenF11  && e.KeyCode == Keys.F11)
            {
                ToggleFullscreen();
            }
        }

        private void ChildWebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (HostWebBrowser.Url.AbsolutePath == "blank")
            {
                return;
            }

            if (this.Settings.WebBrowserRefreshOnFirstLoad && 
                CheckForStaleCache(HostWebBrowser.Url.ToString()))
            {
                HostWebBrowser.Refresh(WebBrowserRefreshOption.Completely);
            }
        }

        public object InvokeScript(string name, params string[] args)
        {
            return HostWebBrowser.Document.InvokeScript(name, args);
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
                Text = menuName
            };

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
                        Keys keys = 0;

                        foreach (string keyCode in keyCodes)
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
            this.PackageAndUnicast((string)item.Tag, item.Name);
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
            this.HostToolStrip.Visible = false ;
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

            if (eventName != "")
            {
                tsb.Image = Image.FromFile(eventName);
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
        /// Returns the important exoskeleton environment locations. (Current, Settings, Executable)
        /// </summary>
        /// <returns>Dynamic object containing Executable, Settings, and Current locations.</returns>
        public dynamic GetLocations()
        {
            return parent.GetLocations();
        }

        public void Shutdown()
        {
            parent.Shutdown();
        }
    }
}
