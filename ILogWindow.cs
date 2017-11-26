using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exoskeleton
{
    public interface ILogWindow
    {
        void AttachHost(IHostWindow host, string title, Icon icon);

        void LogInfo(IHostWindow host, string source, string message);
        void logError(IHostWindow host, string msg, string url, string line, string col, string error);
        void logWarning(IHostWindow host, string source, string message);
        void logText(IHostWindow host, string message);
    }
}
