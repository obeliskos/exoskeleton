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
        void ToggleFullscreen();
        void EnterFullscreen();
        void ExitFullscreen();
        void SetWindowTitle(string title);
        void OpenNewWindow(string caption, string url, int width, int height);
        void ShowNotification(int timeout, string tipTitle, string tipText, ToolTipIcon toolTipIcon);

        string ShowOpenFileDialog(string dialogOptions);
        string ShowSaveFileDialog(string dialogOptions);
        string ShowMessageBox(string text, string caption, string buttons, string icon);

        void MulticastEvent(string name, string data);
        void UnicastEvent(string name, string data);
        void PackageAndMulticast(string name, dynamic[] data);
        void PackageAndUnicast(string name, dynamic[] data);
        object InvokeScript(string name, params string[] args);

        void InitializeMenuStrip();
        void AddMenu(string menuName, string emitEventName);
        void AddMenuItem(string menuName, string menuItemName, string emitEventName);

        void InitializeToolstrip();
        void AddToolStripButton(string text, string eventName, string imagePath);
        void AddToolStripSeparator();

        void InitializeStatusstrip();
        void SetLeftStatusstripLabel(string text);
        void SetRightStatusstripLabel(string text);

        Settings GetCurrentSettings();
        dynamic GetLocations();
        void Shutdown();
    }
}
