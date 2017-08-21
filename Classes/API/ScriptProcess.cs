using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exoskeleton.Classes.API
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class ScriptProcess
    {
        public void start(string procPath)
        {
            Process.Start(procPath);
        }
    }
}
