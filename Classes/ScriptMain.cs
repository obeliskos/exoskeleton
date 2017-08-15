using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exoskeleton.Classes
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class ScriptMain
    {
        public void fullscreen()
        {
            MainForm.formInstance.EnterFullscreen();
        }

        public void exitFullscreen()
        {
            MainForm.formInstance.ExitFullscreen();
        }

        public void setWindowTitle(string title)
        {
            MainForm.formInstance.Text = title;
        }

        public void showNotification(string title, string message)
        {
            MainForm.formInstance.ShowNotification(4000, title, message, System.Windows.Forms.ToolTipIcon.Info);
        }

    }
}
