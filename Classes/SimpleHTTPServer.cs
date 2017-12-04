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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        private MimeTypeMappings _mappings;
        private IPrimaryHostWindow _primaryHost;
        private Settings _settings;

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
        public SimpleHTTPServer(IPrimaryHostWindow primaryHost, Settings settings,
            string path, int port, MimeTypeMappings mappings)
        {
            _primaryHost = primaryHost;
            _settings = settings;
            _mappings = mappings;
            this.Initialize(path, port);
        }

        /// <summary>
        /// Construct server with suitable port.
        /// </summary>
        /// <param name="path">Directory path to serve.</param>
        public SimpleHTTPServer(IPrimaryHostWindow primaryHost, Settings settings, 
            string path, MimeTypeMappings mappings)
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
                string rawUrl;

                try
                {
                    HttpListenerContext context = _listener.GetContext();
                    rawUrl = context.Request.RawUrl;
                    Process(context);
                }
                catch
                {
                    return;
                }
            }
        }

        private void Process(HttpListenerContext context)
        {
            string abspath = System.Net.WebUtility.UrlDecode(context.Request.Url.AbsolutePath);
            string filename = abspath.Substring(1);

            // If WebServices are enabled, check to see if it is a webservice request
            if (_settings.WebServerServicesEnabled && 
                filename.ToLower().EndsWith(_settings.WebServerServicesExtension.ToLower()))
            {
                string body = null;
                string[] bodyParams = null;
                JObject bodyParamObject = new JObject();

                // If this is an HTTP POST, decode the body params
                if (context.Request.HasEntityBody)
                {
                    body = new StreamReader(context.Request.InputStream).ReadToEnd();

                    bodyParams = body.Split('&');
                    for (int idx = 0; idx < bodyParams.Length; idx++)
                    {
                        string[] keyValue = System.Net.WebUtility.UrlDecode(bodyParams[idx]).Split('=');
                        bodyParamObject[keyValue[0]] = keyValue[1];
                    }
                }

                // Decode any query string params
                JObject queryObject = new JObject();
                foreach(dynamic key in context.Request.QueryString.Keys)
                {
                    queryObject[key] = context.Request.QueryString[key];
                }

                // Formulate request to pass to webbrowser javascript
                var emitServicePayload = new
                {
                    context.Request.IsLocal,
                    Filename = filename,
                    AbsolutePath = abspath,
                    HttpMethod = context.Request.HttpMethod,
                    QueryParams = queryObject,
                    context.Request.HasEntityBody,
                    BodyParams = bodyParamObject,
                    RawUrl = System.Net.WebUtility.UrlDecode(context.Request.RawUrl),
                    Url = System.Net.WebUtility.UrlDecode(context.Request.Url.ToString()),
                    context.Request.UrlReferrer,
                    context.Request.ContentLength64,
                    context.Request.UserAgent,
                    context.Request.UserHostName,
                    context.Request.RequestTraceIdentifier
                };

                // Pass to primary host to invoke the service handler via javascript and get a response
                dynamic response = _primaryHost.ProcessServiceRequest(emitServicePayload);

                context.Response.ContentType = response.ContentType; // "application/octet-stream";
                context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
                context.Response.AddHeader("Last-Modified", DateTime.Now.ToString("r"));
                context.Response.AddHeader("X-Powered-By", "Exoskeleton");

                // We expect the service javascript has returned a response property to stream as output
                using (StreamWriter writer = new StreamWriter(context.Response.OutputStream))
                {
                    writer.Write(response.Response);
                }

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.OutputStream.Flush();
                context.Response.OutputStream.Close();

                return;
            }

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
