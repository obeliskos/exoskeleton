using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exoskeleton.Classes.API
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class ScriptMain
    {
        public IHostWindow host = null;

        public ScriptMain(IHostWindow host)
        {
            this.host = host;
        }

        public void fullscreen()
        {
            host.EnterFullscreen();
        }

        public void exitFullscreen()
        {
            host.ExitFullscreen();
        }

        public void setWindowTitle(string title)
        {
            host.SetWindowTitle(title);
        }

        public void showNotification(string title, string message)
        {
            host.ShowNotification(4000, title, message, System.Windows.Forms.ToolTipIcon.Info);
        }

        public void openNewWindow(string caption, string url, int width, int height)
        {
            host.OpenNewWindow(caption, url, width, height);
        }
    }
}
