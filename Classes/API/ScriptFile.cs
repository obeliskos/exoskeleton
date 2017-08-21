using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Exoskeleton.Classes.API
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class ScriptFile
    {
        #region Drives

        public string getDriveInfo()
        {
            DriveInfo[] result = DriveInfo.GetDrives();
            List<dynamic> formattedResult = new List<dynamic>();
            foreach(DriveInfo di in result)
            {
                formattedResult.Add(new
                {
                    AvailableFreeSpace = di.AvailableFreeSpace,
                    DriveFormat = di.DriveFormat,
                    DriveType = di.DriveType,
                    IsReady = di.IsReady,
                    Name = di.Name,
                    RootDirectory = di.RootDirectory,
                    TotalFreeSpace = di.TotalFreeSpace,
                    TotalSize = di.TotalSize,
                    VolumeLabel = di.VolumeLabel
                });
            }

            return JsonConvert.SerializeObject(formattedResult);
        }

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

        #endregion

        #region Directory Actions

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

        public void createDirectory(string path)
        {
             Directory.CreateDirectory(path);
        }

        public void deleteDirectory(string path)
        {
            Directory.Delete(path);
        }

        public string getCurrentDirectory()
        {
            return Path.GetDirectoryName(Application.ExecutablePath);
        }

        #endregion

        #region Files

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

        public string loadFile(string filename)
        {
            string result = null;

            try
            {
                result = File.ReadAllText(filename);
            }
            catch (Exception) { }

            return result;
        }

        public void saveFile(string filename, string contents)
        {
            File.WriteAllText(filename, contents);
        }

        public void copyFile(string source, string dest)
        {
            File.Copy(source, dest);
        }

        public void deleteFile(string filename)
        {
            File.Delete(filename);
        }

        #endregion

    }
}
