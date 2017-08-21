using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exoskeleton
{
    public interface IHostWindow
    {
        void ShowNotification(int timeout, string tipTitle, string tipText, ToolTipIcon toolTipIcon);
        void ToggleFullscreen();
        void EnterFullscreen();
        void ExitFullscreen();
        void SetWindowTitle(string title);
        void OpenNewWindow(string caption, string url, int width, int height);
        object WebInvokeScript(string name, params object[] args);
    }
}
