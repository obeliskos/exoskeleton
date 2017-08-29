using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Exoskeleton.Classes.API
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class ScriptNet : IDisposable
    {
        public void Dispose()
        {
        }

        /// <summary>
        /// Downloads from an internet url and saves to disk.
        /// </summary>
        /// <param name="url">The internet url to download from.</param>
        /// <param name="dest">Destination filename on disk.</param>
        /// <param name="async">Whether to wait until finished before returning.</param>
        public void DownloadFile(string url, string dest, bool? async)
        {
            WebClient wc = new WebClient();

            if (async.HasValue && async.Value)
            {
                wc.DownloadFileAsync(new Uri(url), dest);
            }
            else
            {
                wc.DownloadFile(url, dest);
            }
        }

        /// <summary>
        /// Fetches text-based resource at the provided url and returns a string of its content.
        /// </summary>
        /// <param name="url">Internet url of text based resource.</param>
        /// <returns>String containing text within the retrieved resource.</returns>
        public string ReadUrl(string url)
        {
            WebClient wc = new WebClient();
            byte[] raw = wc.DownloadData(url);

            string webData = Encoding.UTF8.GetString(raw);

            return webData;
        }
    }
}
