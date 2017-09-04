using Exoskeleton.Classes;
using Exoskeleton.Classes.API;
using Newtonsoft.Json;
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

            ChildWebBrowser.ScriptErrorsSuppressed = settings.WebBrowserScriptErrorsSuppressed;
            ChildWebBrowser.WebBrowserShortcutsEnabled = settings.WebBrowserShortcutsEnabled;

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
            this.scriptInterface.Dispose();
        }

        public void PackageAndMulticast(string name, dynamic[] data)
        {
            MainForm.FormInstance.PackageAndMulticast(name, data);
        }

        public void MulticastEvent(string name, string data)
        {
            MainForm.FormInstance.MulticastEvent(name, data);
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
            if (settings.WindowAllowFullscreenF11  && e.KeyCode == Keys.F11)
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

        public object WebInvokeScript(string name, params string[] args)
        {
            return ChildWebBrowser.Document.InvokeScript(name, args);
        }

        #endregion

        #region MessageBox and Dialog Handlers

        /// <summary>
        /// Display an 'OpenFileDialog'
        /// </summary>
        /// <param name="dialogOptions">Optional object containing 'OpenFileDialog' properties to initialize dialog with.</param>
        /// <returns>'OpenFileDialog' properties after dialog was dismissed, or null if cancelled.</returns>
        public string ShowOpenFileDialog(string dialogOptions)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dialogOptions != null && dialogOptions != "")
            {
                dlg = JsonConvert.DeserializeObject<OpenFileDialog>(dialogOptions);
            }
            DialogResult dr = dlg.ShowDialog();
            if (dr != DialogResult.OK) return null;

            var result = JsonConvert.SerializeObject(dlg);
            return result;
        }

        /// <summary>
        /// Display a 'SaveFileDialog'
        /// <param name="dialogOptions">Optional object containing 'SaveFileDialog' properties to initialize dialog with.</param>
        /// </summary>
        /// <returns>'SaveFileDialog' properties after dialog was dismissed, or null if cancelled.</returns>
        public string ShowSaveFileDialog(string dialogOptions)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            if (dialogOptions != null && dialogOptions != "")
            {
                dlg = JsonConvert.DeserializeObject<SaveFileDialog>(dialogOptions);
            }
            DialogResult dr = dlg.ShowDialog();
            if (dr != DialogResult.OK) return null;

            var result = JsonConvert.SerializeObject(dlg);
            return result;
        }

        /// <summary>
        /// Displays a message box to the user and returns the button they clicked.
        /// </summary>
        /// <param name="text">Message to display to user.</param>
        /// <param name="caption">Caption of message box window.</param>
        /// <param name="buttons">String representation of a MessageBoxButtons enum.</param>
        /// <param name="icon">string representation of a MessageBoxIcon enum.</param>
        /// <returns>Text (ToString) representation of button clicked.</returns>
        public string ShowMessageBox(string text, string caption, string buttons, string icon)
        {
            MessageBoxButtons _buttons = MessageBoxButtons.OK;
            if (buttons != null && buttons != "")
            {
                _buttons = (MessageBoxButtons)Enum.Parse(typeof(MessageBoxButtons), buttons);
            }
            MessageBoxIcon _icon = MessageBoxIcon.Information;
            if (icon != null && icon != "")
            {
                _icon = (MessageBoxIcon)Enum.Parse(typeof(MessageBoxIcon), icon);
            }
            DialogResult dr = MessageBox.Show(text, caption, _buttons, _icon);
            return dr.ToString();
        }

        #endregion

        #region Interface Memebers

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
            return MainForm.FormInstance.GetLocations();
        }

        #endregion
    }
}
