using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        void ToggleFullscreen();
        void EnterFullscreen();
        void ExitFullscreen();
        void SetWindowTitle(string title);
        void OpenNewWindow(string caption, string url, int width, int height);
        object WebInvokeScript(string name, params object[] args);
    }
}
