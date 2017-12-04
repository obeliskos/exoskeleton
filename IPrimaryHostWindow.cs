using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exoskeleton
{
    public interface IPrimaryHostWindow : IHostWindow
    {
        string ResolveExoUrlPath(string url);
        void RemoveHostWindow(IHostWindow hostWindow);
        int ActualPort { get; set; }
        dynamic ProcessServiceRequest(dynamic request);
    }
}
