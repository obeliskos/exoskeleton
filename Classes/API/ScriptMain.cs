using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exoskeleton.Classes.API
{
    /// <summary>
    /// General user interface utility methods.
    /// </summary>
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class ScriptMain: IDisposable
    {
        public IHostWindow host = null;

        public ScriptMain(IHostWindow host)
        {
            this.host = host;
        }

        public void Dispose()
        {
        }

        /// <summary>
        /// Updates the window title for the host container.
        /// </summary>
        /// <param name="title">Text to apply to window title.</param>
        public void SetWindowTitle(string title)
        {
            host.SetWindowTitle(title);
        }

        /// <summary>
        /// Signals the host container to enter fullscreen mode.
        /// </summary>
        public void Fullscreen()
        {
            host.EnterFullscreen();
            Application.DoEvents();
        }

        /// <summary>
        /// Signals the host container to exit fullscreen mode.
        /// </summary>
        public void ExitFullscreen()
        {
            host.ExitFullscreen();
            Application.DoEvents();
        }

        /// <summary>
        /// Signals the host container to toggle fullscreen mode.
        /// </summary>
        public void ToggleFullscreen()
        {
            host.ToggleFullscreen();
            Application.DoEvents();
        }

        /// <summary>
        /// Displays a windows system tray notification.
        /// </summary>
        /// <param name="title">The notification title.</param>
        /// <param name="message">The notification message.</param>
        public void ShowNotification(string title, string message)
        {
            host.ShowNotification(4000, title, message, System.Windows.Forms.ToolTipIcon.Info);
        }

        /// <summary>
        /// Opens a new host container with the url and settings provided.
        /// </summary>
        /// <param name="caption">Window caption to apply to new window.</param>
        /// <param name="url">Url to load within the new window.</param>
        /// <param name="width">Width (in pixels) to size new window to.</param>
        /// <param name="height">Height (in pixels) to size new window to.</param>
        public void OpenNewWindow(string caption, string url, int width, int height, string mode)
        {
            host.OpenNewWindow(caption, url, width, height, mode);
        }

        public void SwitchToNativeUi()
        {
            host.SwitchToNativeUi();
        }

        public void SwitchToWebUi()
        {
            host.SwitchToWebUi();
        }

        // Allows updating properties of the host window Form object
        public void ApplyFormProperties(string formProperties)
        {
            JsonConvert.PopulateObject(formProperties, host.GetForm());
        }

        /// <summary>
        /// Process all Windows messages currently in the message queue.
        /// </summary>
        public void DoEvents()
        {
            Application.DoEvents();
        }

    }
}
