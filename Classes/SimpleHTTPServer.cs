using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace Exoskeleton.Classes
{
    /// <summary>
    /// Taken from gist at : https://gist.github.com/aksakalli/9191056
    /// </summary>
    class SimpleHTTPServer 
    {
        public readonly string[] _indexFiles = { 
            "index.html", 
            "index.htm", 
            "default.html", 
            "default.htm",
            "SandboxLoaderWJS.htm" 
        };

        private Thread _serverThread;
        private string _rootDirectory;
        private HttpListener _listener;
        private int _port;
        MimeTypeMappings _mappings;

        public int Port
        {
            get { return _port; }
            private set { }
        }

        /// <summary>
        /// Construct server with given port.
        /// </summary>
        /// <param name="path">Directory path to serve.</param>
        /// <param name="port">Port of the server.</param>
        public SimpleHTTPServer(string path, int port, MimeTypeMappings mappings)
        {
            _mappings = mappings;
            this.Initialize(path, port);
        }

        /// <summary>
        /// Construct server with suitable port.
        /// </summary>
        /// <param name="path">Directory path to serve.</param>
        public SimpleHTTPServer(string path, MimeTypeMappings mappings)
        {
            _mappings = mappings;

            //get an empty port
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            this.Initialize(path, port);
        }

        public static int GetOpenPort(int startPort = 2555)
        {
            int portStartIndex = startPort;
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] tcpEndPoints = properties.GetActiveTcpListeners();

            List<int> usedPorts = tcpEndPoints.Select(p => p.Port).ToList<int>();
            int unusedPort = 0;

            unusedPort = Enumerable.Range(portStartIndex, 99).Where(port => !usedPorts.Contains(port)).FirstOrDefault();
            return unusedPort;
        }
        
        /// <summary>
        /// Stop server and dispose all functions.
        /// </summary>
        public void Stop()
        {
            _serverThread.Abort();
            _listener.Stop();
        }

        private void Listen()
        {
            _listener = new HttpListener();

            // Allow user to override prefixes via config file
            _listener.Prefixes.Add("http://localhost:" + _port.ToString() + "/");

            _listener.Start();
            while (true)
            {
                try
                {
                    HttpListenerContext context = _listener.GetContext();
                    Process(context);
                }
                catch { }
            }
        }

        private void Process(HttpListenerContext context)
        {
            string filename = context.Request.Url.AbsolutePath;
            Console.WriteLine(filename);
            filename = filename.Substring(1);
            filename = filename.Replace("%20", " ");

            if (string.IsNullOrEmpty(filename))
            {
                foreach (string indexFile in _indexFiles)
                {
                    if (File.Exists(Path.Combine(_rootDirectory, indexFile)))
                    {
                        filename = indexFile;
                        break;
                    }
                }
            }

            filename = Path.Combine(_rootDirectory, filename);

            if (File.Exists(filename))
            {
                try
                {
                    Stream input = new FileStream(filename, FileMode.Open, FileAccess.Read);

                    //Adding permanent http response headers
                    string mime;

                    // since dictionary is no longer case insensitive, we should force lower casing everywhere
                    filename = filename.ToLower();

                    context.Response.ContentType = _mappings.Mappings.TryGetValue(Path.GetExtension(filename), out mime) ? mime : "application/octet-stream";
                    context.Response.ContentLength64 = input.Length;
                    context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
                    context.Response.AddHeader("Last-Modified", System.IO.File.GetLastWriteTime(filename).ToString("r"));
                    context.Response.AddHeader("X-Powered-By", "Exoskeleton");
                    
                    byte[] buffer = new byte[1024 * 16];
                    int nbytes;
                    while ((nbytes = input.Read(buffer, 0, buffer.Length)) > 0)
                        context.Response.OutputStream.Write(buffer, 0, nbytes);
                    input.Close();
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    context.Response.OutputStream.Flush();
                }
                catch {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }

            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }

            context.Response.OutputStream.Close();
        }

        private void Initialize(string path, int port)
        {
            this._rootDirectory = path;
            this._port = port;
            _serverThread = new Thread(this.Listen);
            _serverThread.Start();
        }

    }
}
