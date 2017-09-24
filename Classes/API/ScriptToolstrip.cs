using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exoskeleton.Classes.API
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class ScriptToolstrip : IDisposable
    {
        IHostWindow host;

        public ScriptToolstrip(IHostWindow host)
        {
            this.host = host;
        }

        /// <summary>
        /// Empties the host window toolstrip of all controls
        /// </summary>
        public void Initialize()
        {
            host.InitializeToolstrip();
        }

        /// <summary>
        /// Adds a ToolStripButton to the host window toolstrip
        /// </summary>
        /// <param name="text"></param>
        /// <param name="eventName"></param>
        /// <param name="imagePath"></param>
        public void AddButton(string text, string eventName, string imagePath)
        {
            host.AddToolStripButton(text, eventName, imagePath);
        }

        /// <summary>
        /// Adds a visual separator for toolstrip control groups
        /// </summary>
        public void AddSeparator()
        {
            host.AddToolStripSeparator();
        }

        public void Dispose()
        {

        }
    }
}
