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
        private IPrimaryHostWindow parent;
        private ScriptInterface scriptInterface;
        private Settings settings;
        private static Dictionary<string, bool> cacheRefreshed = new Dictionary<string, bool>();
        private bool fullscreen = false;
        private string uri;
        LoggerForm logger;

        #region Form Level Constructor and Events

        public ChildWindow(IPrimaryHostWindow parent, string caption, string uri, Settings settings, int? width, int? height)
        {
            this.parent = parent;

            if (settings.ScriptingLoggerEnabled)
            {
                logger = new LoggerForm(this, caption);
                //    Rectangle workingArea = Screen.GetWorkingArea(this);
                logger.Show();
                //    logger.Location = new Point(workingArea.Right - logger.Width, 100 + 100 * hostWindows.Count);
            }

            scriptInterface = new ScriptInterface(this, settings, logger);

            this.settings = settings;

            this.uri = uri;

            InitializeComponent();

            if (settings.WindowIconPath != "" && File.Exists(settings.WindowIconPath))
            {
                this.Icon = new Icon(settings.WindowIconPath);
            }


            HostMenuStrip.Visible = settings.ScriptingMenuEnabled;
            HostToolStrip.Visible = settings.ScriptingToolStripEnabled;
            HostStatusStrip.Visible = settings.ScriptingStatusStripEnabled;

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
            if (uri.StartsWith("@"))
            {
                ChildWebBrowser.DocumentText = uri.Substring(1);
            }
            else
            {
                ChildWebBrowser.Url = new Uri(uri);
            }
        }

        private void ChildWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            parent.RemoveHostWindow(this);
            this.scriptInterface.Dispose();
        }

        public void PackageAndMulticast(string name, dynamic[] data)
        {
            parent.PackageAndMulticast(name, data);
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

        public void MulticastEvent(string name, string data)
        {
            parent.MulticastEvent(name, data);
        }

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

        #endregion

        #region Form Level ScriptingHandlers

        public Form GetForm()
        {
            return this;
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
            parent.OpenNewWindow(caption, url, width, height);
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
            if (settings.WindowAllowFullscreenF11  && e.KeyCode == Keys.F11)
            {
                ToggleFullscreen();
            }
        }

        private void ChildWebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (ChildWebBrowser.Url.AbsolutePath == "blank")
            {
                return;
            }

            if (settings.WebBrowserRefreshOnFirstLoad && 
                CheckForStaleCache(ChildWebBrowser.Url.ToString()))
            {
                ChildWebBrowser.Refresh(WebBrowserRefreshOption.Completely);
            }
        }

        public object InvokeScript(string name, params string[] args)
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

        #endregion

        // Need to determine if i want to attempt to implement base class or mvp instead of just interfaces.  
        // Winforms designers may not play well with abstract base classes.

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
            this.UnicastEvent((string)item.Tag, item.Name);
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
            return parent.GetLocations();
        }

        public void Shutdown()
        {
            parent.Shutdown();
        }
    }
}
