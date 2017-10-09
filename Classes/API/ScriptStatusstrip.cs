using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exoskeleton.Classes.API
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class ScriptStatusstrip : IDisposable
    {
        IHostWindow host;

        public ScriptStatusstrip(IHostWindow host)
        {
            this.host = host;
        }

        public void Show()
        {
            host.ShowStatusstrip();
        }

        public void Hide()
        {
            host.HideStatusstrip();
        }

        public void Initialize()
        {
            host.InitializeStatusstrip();
        }

        public void SetLeftLabel(string text)
        {
            host.SetLeftStatusstripLabel(text);
        }

        public void SetRightLabel(string text)
        {
            host.SetRightStatusstripLabel(text);
        }

        public void Dispose()
        {

        }
    }
}
