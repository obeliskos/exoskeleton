using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exoskeleton.Classes.API
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class ScriptInterface : IDisposable
    {
        public ScriptMain Main = null;
        public ScriptMedia Media = null;
        public ScriptFile File = null;
        public ScriptProcess Proc = null;
        public ScriptSession Session = null;
        public ScriptSystem System = null;
        public ScriptLogger Logger = null;
        public ScriptComObjects Com = null;
        public ScriptNet Net = null;
        public ScriptEncryption Enc = null;
        public ScriptMenu Menu = null;
        public ScriptToolstrip Toolbar = null;
        public ScriptStatusstrip Statusbar = null;

        public IHostWindow host;

        /// <summary>
        /// Constructor which determines (via settings) which api classes to expose via COM.
        /// </summary>
        /// <param name="host">Instance of the IHostWindow acting as our container.</param>
        /// <param name="exosettings">User defined settings to use.</param>
        /// <param name="loggerForm">Reference to our logger (if enabled)</param>
        public ScriptInterface(IHostWindow host, Settings exosettings, LoggerForm loggerForm)
        {
            this.host = host;

            this.Main = exosettings.ScriptingEnabled ? new ScriptMain(host) : null;
            this.Media = exosettings.ScriptingMediaEnabled ? new ScriptMedia() : null;
            this.File = exosettings.ScriptingFilesEnabled ? new ScriptFile(host) : null;
            this.Proc = exosettings.ScriptingProcessEnabled ? new ScriptProcess() : null;
            this.Session = new ScriptSession();
            this.System = exosettings.ScriptingSystemEnabled ? new ScriptSystem(exosettings) : null;
            this.Logger = exosettings.ScriptingLoggerEnabled ? new ScriptLogger(loggerForm) : null;
            this.Com = exosettings.ScriptingComObjectsEnabled ? new ScriptComObjects() : null;
            this.Net = exosettings.ScriptingNetEnabled ? new ScriptNet() : null;
            this.Enc = exosettings.ScriptingEncryptionEnabled ? new ScriptEncryption() : null;
            this.Menu = exosettings.ScriptingMenuEnabled ? new ScriptMenu(host) : null;
            this.Toolbar = exosettings.ScriptingToolStripEnabled ? new ScriptToolstrip(host) : null;
            this.Statusbar = exosettings.ScriptingStatusStripEnabled ? new ScriptStatusstrip(host) : null;
        }

        /// <summary>
        /// Implementing IDisposable on this and all api subclasses to allow each api class 
        /// to clean up any resources in their dispose method.
        /// </summary>
        public void Dispose() 
        {
            if (this.Main != null) this.Main.Dispose();
            if (this.Media != null) this.Media.Dispose();
            if (this.File != null) this.File.Dispose();
            if (this.Proc != null) this.Proc.Dispose();
            if (this.Session != null) this.Session.Dispose();
            if (this.System != null) this.System.Dispose();
            if (this.Logger != null) this.Logger.Dispose();
            if (this.Net != null) this.Net.Dispose();
            if (this.Enc != null) this.Enc.Dispose();
        }

        /// <summary>
        /// API Exposed method so that javscript can multicast.
        /// </summary>
        /// <param name="eventName">Event name to multicast</param>
        /// <param name="eventData">Optional string encoded event data to emit with.</param>
        public void MulticastEvent(string eventName, string eventDataJson)
        {
            host.MulticastEvent(eventName, eventDataJson);
        }

        /// <summary>
        /// API Exposed method so that javascript can close this exoskeleton application instance.
        /// </summary>
        public void Shutdown()
        {
            host.Shutdown();
        }
    }
}
