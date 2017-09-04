using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Exoskeleton.Classes;

namespace Exoskeleton
{
    /// <summary>
    /// Exoskelton host window interface.  
    /// This allows MainForm and ChildWindow instances to be treated similarly from ScriptInterface.
    /// </summary>
    public interface IHostWindow
    {
        void ShowNotification(int timeout, string tipTitle, string tipText, ToolTipIcon toolTipIcon);
        void MulticastEvent(string name, string data);
        void PackageAndMulticast(string name, dynamic[] data);
        void ToggleFullscreen();
        void EnterFullscreen();
        void ExitFullscreen();
        void SetWindowTitle(string title);
        void OpenNewWindow(string caption, string url, int width, int height);
        object WebInvokeScript(string name, params string[] args);
        string ShowOpenFileDialog(string dialogOptions);
        string ShowSaveFileDialog(string dialogOptions);
        string ShowMessageBox(string text, string caption, string buttons, string icon);
        Settings GetCurrentSettings();
        dynamic GetLocations();
    }
}
