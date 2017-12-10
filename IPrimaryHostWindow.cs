using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exoskeleton
{
    public interface IPrimaryHostWindow : IHostWindow
    {
        Icon DefaultIcon { get; set; }
        string ResolveExoUrlPath(string url);
        void AddLaunchHistory(string settingsPath, string shortName, string description, string iconPath);
        void RemoveLaunchHistory(string settingsPath);
        void RemoveHostWindow(IHostWindow hostWindow);
        int ActualPort { get; set; }
        dynamic ProcessServiceRequest(dynamic request);
    }
}
