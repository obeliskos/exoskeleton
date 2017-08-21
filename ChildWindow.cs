using Exoskeleton.Classes;
using Exoskeleton.Classes.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exoskeleton
{
    public partial class ChildWindow : Form, IHostWindow
    {
        private ScriptInterface scriptInterface;
        private Settings settings;
        private static Dictionary<string, bool> cacheRefreshed = new Dictionary<string, bool>();
        private bool fullscreen = false;
        private Uri uri;

        #region Form Level Constructor and Events

        public ChildWindow(string caption, Uri uri, Settings settings, LoggerForm logger, int? width, int? height)
        {
            scriptInterface = new ScriptInterface(this, settings, logger);
            this.settings = settings;
            this.uri = uri;

            InitializeComponent();

            if (caption != null) this.Text = caption;
            if (width.HasValue) this.Width = width.Value;
            if (height.HasValue) this.Height = height.Value;

            ChildWebBrowser.ObjectForScripting = scriptInterface;
            ChildWebBrowser.IsWebBrowserContextMenuEnabled = settings.WebBrowserContextMenu;
        }

        private void ChildWindow_Load(object sender, EventArgs e)
        {
            ChildWebBrowser.Url = uri;
        }

        private void ChildWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Window is closing, so no longer need to multicast to us
            MainForm.FormInstance.RemoveHostWindow(this);
        }

        #endregion

        #region Form Level ScriptingHandlers

        public void SetWindowTitle(string title)
        {
            this.Text = title;
        }

        public void ShowNotification(int timeout, string tipTitle, string tipText, ToolTipIcon toolTipIcon)
        {
            MainForm.FormInstance.ShowNotification(timeout, tipTitle, tipText, toolTipIcon);
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

            ChildWebBrowser.Focus();
        }

        public void ExitFullscreen()
        {
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.WindowState = FormWindowState.Normal;

            ChildWebBrowser.Focus();
        }

        public void OpenNewWindow(string caption, string url, int width, int height)
        {
            MainForm.FormInstance.OpenNewWindow(caption, url, width, height);
        }

        #endregion

        #region WebBrowser events and utility methods

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

        private void ChildWebBrowser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.F11)
            {
                ToggleFullscreen();
            }
        }

        private void ChildWebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (settings.WebBrowserRefreshOnFirstLoad && 
                CheckForStaleCache(ChildWebBrowser.Url.ToString()))
            {
                ChildWebBrowser.Refresh(WebBrowserRefreshOption.Completely);
            }
        }

        public object WebInvokeScript(string name, params object[] args)
        {
            return ChildWebBrowser.Document.InvokeScript(name, args);
        }

        #endregion
    }
}
