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
        public void OpenNewWindow(string caption, string url, int width, int height)
        {
            host.OpenNewWindow(caption, url, width, height);
        }

        /// <summary>
        /// Display an 'OpenFileDialog'
        /// </summary>
        /// <param name="dialogOptions">Optional object containing 'OpenFileDialog' properties to initialize dialog with.</param>
        /// <returns>'OpenFileDialog' properties after dialog was dismissed, or null if cancelled.</returns>
        public string ShowOpenFileDialog(string dialogOptions)
        {
            return host.ShowOpenFileDialog(dialogOptions);
        }

        /// <summary>
        /// Display a 'SaveFileDialog'
        /// <param name="dialogOptions">Optional object containing 'SaveFileDialog' properties to initialize dialog with.</param>
        /// </summary>
        /// <returns>'SaveFileDialog' properties after dialog was dismissed, or null if cancelled.</returns>
        public string ShowSaveFileDialog(string dialogOptions)
        {
            return host.ShowSaveFileDialog(dialogOptions);
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
            return host.ShowMessageBox(text, caption, buttons, icon);
        }

        /// <summary>
        /// Returns the currently active settings, converted to a json string.
        /// </summary>
        /// <returns>Application Settings, serialized as json.</returns>
        public string GetApplicationSettings()
        {
            Settings settings = host.GetCurrentSettings();

            string json = JsonConvert.SerializeObject(settings);

            return json;
        }

        /// <summary>
        /// Returns the important exoskeleton environment locations. (Current, Settings, Executable)
        /// </summary>
        /// <returns>Serialized locations object.</returns>
        public string GetLocations()
        {
            dynamic locs = host.GetLocations();
            string json = JsonConvert.SerializeObject(locs);
            return json;
        }

        /// <summary>
        /// Process all Windows messages currently in the message queue.
        /// </summary>
        public void DoEvents()
        {
            Application.DoEvents();
        }

        /// <summary>
        /// Converts a .NET date to a unix date format, usable by javascript.
        /// </summary>
        /// <param name="date">A serialized .net DateTime object</param>
        /// <returns>Unix time as number of milliseconds since 1/1/1970.</returns>
        public long ConvertDateToUnix(string date)
        {
            DateTime dt = DateTime.Parse(date);
            TimeSpan ts = dt - new DateTime(1970, 1, 1);
            return (long) ts.TotalMilliseconds;
        }

        /// <summary>
        /// Converts a javascript unix epoch time to a .net formatted date
        /// </summary>
        /// <param name="date">Javascript date getTime() value</param>
        /// <param name="fmt">.NET ToString() format string</param>
        /// <returns></returns>
        public string FormatUnixDate(long date, string fmt)
        {
            DateTime dt = new DateTime(1970, 1, 1);
            dt = dt.AddMilliseconds(date);

            return dt.ToString(fmt);
        }
    }
}
