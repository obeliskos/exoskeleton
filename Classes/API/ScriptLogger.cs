using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exoskeleton.Classes.API
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class ScriptLogger
    {
        private LoggerForm logger = null;

        public ScriptLogger(LoggerForm logger)
        {
            this.logger = logger;
        }

        public void logInfo(string source, string message)
        {
            logger.LogInfo(source, message);
        }

        public void logError(string msg, string url, string line, string col, string error)
        {
            logger.logError(msg, url, line, col, error);
        }

        public void logWarning(string source, string message)
        {
            logger.logWarning(source, message);
        }

        public void logText(string message)
        {
            logger.logText(message==null?message:message.Replace("\n", Environment.NewLine));
        }
    }
}
