using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exoskeleton.Classes.API
{
    /// <summary>
    /// Expose registered logger's methods to javascript
    /// </summary>
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class ScriptLogger: IDisposable
    {
        private IHostWindow host = null;
        private ILogWindow logger = null;

        public ScriptLogger(IHostWindow host)
        {
            this.host = host;
            this.logger = this.host.Logger;
        }

        public void Dispose()
        {
            // Clear all logger entries for the host window?
            logger = null;
        }

        /// <summary>
        /// Logs an "info" message to the logger's message list.
        /// </summary>
        /// <param name="source">Descriptive 'source' of the info.</param>
        /// <param name="message">Info detail message</param>
        public void LogInfo(string source, string message)
        {
            if (logger == null) return;

            logger.LogInfo(host, source, message);
        }

        /// <summary>
        /// Logs a "warning" message to the logger's message list.
        /// </summary>
        /// <param name="source">Descriptive 'source' of the warning.</param>
        /// <param name="message">Detailed warning message.</param>
        public void LogWarning(string source, string message)
        {
            if (logger == null) return;

            logger.logWarning(host, source, message);
        }

        /// <summary>
        /// Logs an "error" message to the logger's message list.
        /// </summary>
        /// <param name="msg">Message to log.</param>
        /// <param name="url">The url of the script where the error occurred.</param>
        /// <param name="line">Line number of the javascript where the error occurred.</param>
        /// <param name="col">Column number of the javascript where the error occurred.</param>
        /// <param name="error">Detailed informatino about the error.</param>
        public void LogError(string msg, string url, string line, string col, string error)
        {
            if (logger == null) return;

            logger.logError(host, msg, url, line, col, error);
        }

        /// <summary>
        /// Logs text to the logger's console.
        /// </summary>
        /// <param name="message">Text to append to the console.</param>
        public void LogText(string message)
        {
            if (logger == null) return;

            logger.logText(host, message==null?message:message.Replace("\n", Environment.NewLine));
        }
    }
}
