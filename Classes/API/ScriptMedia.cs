using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exoskeleton.Classes.API
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class ScriptMedia : IDisposable
    {
        IHostWindow host;

        public ScriptMedia(IHostWindow host)
        {
            this.host = host;
        }

        public void Dispose()
        {
        }

        public void Speak(string textToSpeak)
        {
            Task.Run(() =>
            {
                string StringToRead = textToSpeak;

                Type t = Type.GetTypeFromProgID("SAPI.SpVoice");
                object voice = Activator.CreateInstance(t);
                object[] sargs = new Object[2];
                sargs[0] = StringToRead;
                sargs[1] = 1; // 0 synchronous, 1 asynchronous
                t.InvokeMember("Speak", BindingFlags.InvokeMethod, null, voice, sargs);
                object[] wargs = new Object[1];
                wargs[0] = 999000; // 10 second timeout?
                t.InvokeMember("WaitUntilDone", BindingFlags.InvokeMethod, null, voice, wargs);

                return 0;
            });
        }

        public void SpeakSync(string textToSpeak)
        {
            string StringToRead = textToSpeak;

            Type t = Type.GetTypeFromProgID("SAPI.SpVoice");
            object voice = Activator.CreateInstance(t);
            object[] sargs = new Object[2];
            sargs[0] = StringToRead;
            sargs[1] = 0; // 0 synchronous, 1 asynchronous
            t.InvokeMember("Speak", BindingFlags.InvokeMethod, null, voice, sargs);
            object[] wargs = new Object[1];
            wargs[0] = 999000; // 10 second timeout?
            t.InvokeMember("WaitUntilDone", BindingFlags.InvokeMethod, null, voice, wargs);
        }

        #region ImageList Management

        /// <summary>
        /// Instantiates a new named ImageList to use for forms or dialogs
        /// </summary>
        /// <param name="name">name to refer to image list</param>
        /// <param name="properties">(optional) json containing properties to apply to image list.</param>
        public void CreateImageList(string name, string properties)
        {
            // Encountered issues with calling JsonConvert.PopulateObject so for now i will manually
            // populate only 'ColorDepth' and 'ImageSize' properties.

            dynamic propertiesDynamic = JsonConvert.DeserializeObject(properties);
            JObject pdef = (JObject) propertiesDynamic;

            ImageList newImageList = new ImageList();

            if (pdef["ImageSize"] != null)
            {
                int[] size = JsonConvert.DeserializeObject<int[]>(pdef["ImageSize"].ToString());

                newImageList.ImageSize = new Size(size[0], size[1]);
            }

            if (pdef["ColorDepth"] != null)
            {
                int cd = (int)pdef["ColorDepth"];

                switch (cd) {
                    case 4: newImageList.ColorDepth = ColorDepth.Depth4Bit; break;
                    case 8: newImageList.ColorDepth = ColorDepth.Depth8Bit; break;
                    case 16: newImageList.ColorDepth = ColorDepth.Depth16Bit; break;
                    case 24: newImageList.ColorDepth = ColorDepth.Depth24Bit; break;
                    case 32: newImageList.ColorDepth = ColorDepth.Depth32Bit; break;
                    default: newImageList.ColorDepth = ColorDepth.Depth16Bit; break;
                }
            }

            host.ImageListDictionary[name] = newImageList;
        }

        /// <summary>
        /// Determines if an imagelist exists by the given name
        /// </summary>
        /// <param name="name">name of the imagelist to check for existence of</param>
        /// <returns>boolean true/false for if the imagelist exists</returns>
        public bool ImageListExists(string name)
        {
            return host.ImageListDictionary.Keys.Contains(name);
        }

        /// <summary>
        /// Loads a named imagelist with 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="filenameList"></param>
        public void LoadImageList(string name, string filenameList)
        {
            ImageList selectedImageList = host.ImageListDictionary[name];

            string[] filenames = JsonConvert.DeserializeObject<string[]>(filenameList);

            foreach (string filename in filenames)
            {
                FileInfo fi = new FileInfo(filename);
                selectedImageList.Images.Add(fi.Name, Image.FromFile(filename));
            }
        }

        public void ClearImageList(string name)
        {
            ImageList selectedImageList = host.ImageListDictionary[name];
            selectedImageList.Images.Clear();
        }

        public void RemoveImageList(string name)
        {
            host.ImageListDictionary.Remove(name);
        }

        public void RemoveAllImageLists()
        {
            List<string> keys = host.ImageListDictionary.Keys.ToList();
            foreach(string key in keys)
            {
                host.ImageListDictionary.Remove(key);
            }
        }

        #endregion
    }
}
