using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Exoskeleton.Classes.API
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class ScriptMedia : IDisposable
    {
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
    }
}
