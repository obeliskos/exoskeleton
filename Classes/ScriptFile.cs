using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Exoskeleton.Classes
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class ScriptFile
    {
        public string getLogicalDrives()
        {
            try
            {
                string[] result = Directory.GetLogicalDrives();

                return JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string getDirectories(string parentDir)
        {
            try
            {
                string[] result = Directory.GetDirectories(parentDir);

                return JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string getFiles(string parentDir, string searchPattern)
        {
            try
            {
                string[] result = (searchPattern == null ? Directory.GetFiles(parentDir):Directory.GetFiles(parentDir, searchPattern));

                return JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string getCurrentDirectory()
        {
            return System.IO.Path.GetDirectoryName(Application.ExecutablePath);
        }

        public void saveFile(string filename, string contents)
        {
            System.IO.File.WriteAllText(filename, contents);
        }

        public string loadFile(string filename)
        {
            string result = null;

            try
            {
                result = System.IO.File.ReadAllText(filename);
            }
            catch (Exception) { }

            return result;
        }

    }
}
