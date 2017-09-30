using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exoskeleton.Classes.API
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class ScriptUtility : IDisposable
    {
        public ScriptUtility()
        {

        }

        public void Dispose()
        {
        }

        /// <summary>
        /// Converts a .NET date to a unix date format, usable by javascript.
        /// </summary>
        /// <param name="date">A serialized .net DateTime object</param>
        /// <returns>Unix time as number of milliseconds since 1/1/1970.</returns>
        public long ConvertDateToUnix(string date)
        {
            DateTime dt = DateTime.Parse(date);
            TimeSpan ts = dt - new DateTime(1970, 1, 1);
            return (long)ts.TotalMilliseconds;
        }

        /// <summary>
        /// Converts a javascript unix epoch time to a .net formatted date
        /// </summary>
        /// <param name="date">Javascript date getTime() value</param>
        /// <param name="fmt">.NET ToString() format string</param>
        /// <returns></returns>
        public string FormatUnixDate(long date, string fmt)
        {
            DateTime dt = new DateTime(1970, 1, 1);
            dt = dt.AddMilliseconds(date);

            return dt.ToLocalTime().ToString(fmt);
        }

    }
}
