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
    public class ScriptFile : IDisposable
    {
        private FileSystemWatcher watcher = null;
        private string watcherBaseName = "watcher";
        private IHostWindow host = null;

        public ScriptFile(IHostWindow host)
        {
            this.host = host;
        }

        public void Dispose()
        {
            if (watcher != null)
            {
                StopWatcher();
            }
        }

        #region Watcher (singleton) management

        /// <summary>
        /// Starts our global singleton watcher on path specified by user.
        /// Events will be emitted as multicast, so if multiple forms load
        /// multiple watchers, they should distinguish themselves with the
        /// eventBaseName parameter.
        /// </summary>
        /// <param name="path">Path to 'watch'</param>
        /// <param name="eventBaseName">Optional base name to emit with.</param>
        public void StartWatcher(string path, string eventBaseName)
        {
            if (eventBaseName != null)
            {
                watcherBaseName = eventBaseName;
            }

            watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                   | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = "*.*";
            watcher.Changed += watcher_Changed;
            watcher.Created += watcher_Created;
            watcher.Deleted += watcher_Deleted;
            watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Disables the watcher singleton.
        /// </summary>
        public void StopWatcher()
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
        }

        /// <summary>
        /// 'Deleted' event handler for our watcher singleton (if started).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            string data = JsonConvert.SerializeObject(e);

            host.MulticastEvent(watcherBaseName + ".deleted", data);
        }

        /// <summary>
        /// 'Created' event handler for our watcher singleton (if started)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void watcher_Created(object sender, FileSystemEventArgs e)
        {
            string data = JsonConvert.SerializeObject(e);

            host.MulticastEvent(watcherBaseName + ".created", data);
        }

        /// <summary>
        /// 'Changed' event handler for our watcher singleton (if started)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            string data = JsonConvert.SerializeObject(e);

            host.MulticastEvent(watcherBaseName + ".changed", data);
        }

        #endregion

        #region Drives

        /// <summary>
        /// Gets information about each of the mounted drives.
        /// </summary>
        /// <returns>Json encoded list of dynamic objects containing drive info</returns>
        public string GetDriveInfo()
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

        /// <summary>
        /// Gets a list of logical drives.
        /// </summary>
        /// <returns>Json encoded array of drive name strings</returns>
        public string GetLogicalDrives()
        {
            string[] result = Directory.GetLogicalDrives();

            return JsonConvert.SerializeObject(result);
        }

        #endregion

        #region Directory Actions

        /// <summary>
        /// Returns the directory portion of the path without the filename.
        /// </summary>
        /// <param name="path">The full pathname to get directory portion of.</param>
        /// <returns></returns>
        public string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }

        /// <summary>
        /// Returns the filename portion of the path without the directory.
        /// </summary>
        /// <param name="path">The full pathname to get filename portion of.</param>
        /// <returns></returns>
        public string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        /// <summary>
        /// Gets the file extension of the fully qualified path.
        /// </summary>
        /// <param name="path">The filepath to get extension of.</param>
        /// <returns></returns>
        public string GetExtension(string path)
        {
            return Path.GetExtension(path);
        }

        /// <summary>
        /// Combine multiple paths into one.  
        /// </summary>
        /// <param name="pathsJson">Json encoded array of paths to combine.</param>
        /// <returns></returns>
        public string CombinePaths(string pathsJson)
        {
            List<string> paths = JsonConvert.DeserializeObject<List<string>>(pathsJson);
            return Path.Combine(paths.ToArray());
        }

        /// <summary>
        /// Gets DirectoryInfo for the specified directory path.
        /// </summary>
        /// <param name="path">Name of the directory to get information for.</param>
        /// <returns>Json encoded object containing information about the directory.</returns>
        public dynamic GetDirectoryInfo(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);

            var ddi = new
            {
                Attributes = di.Attributes,
                CreationTime = di.CreationTime,
                CreationTimeUtc = di.CreationTimeUtc,
                Exists = di.Exists,
                Extension = di.Extension,
                FullName = di.FullName,
                LastAccessTime = di.LastAccessTime,
                LastAccessTimeUtc = di.LastAccessTimeUtc,
                LastWriteTime = di.LastWriteTime,
                LastWriteTimeUtc = di.LastWriteTimeUtc,
                Name = di.Name
            };

            string json = JsonConvert.SerializeObject(ddi);

            return json;
        }

        /// <summary>
        /// Gets subdirectory names of a parent directory.
        /// </summary>
        /// <param name="parentDir">Directory to list subdirectories for.</param>
        /// <returns>Json encoded string array of subdirectories.</returns>
        public string GetDirectories(string parentDir)
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

        /// <summary>
        /// Creates all directories and subdirectories in the specified path unless they already exist.
        /// </summary>
        /// <param name="path">The directory to create.</param>
        public void CreateDirectory(string path)
        {
             Directory.CreateDirectory(path);
        }

        /// <summary>
        /// Deletes an empty directory.
        /// </summary>
        /// <param name="path">The name of the empty directory to delete.</param>
        public void DeleteDirectory(string path)
        {
            Directory.Delete(path);
        }

        /// <summary>
        /// Gets the directory where the exoskeleton executable was loaded from.
        /// </summary>
        /// <returns></returns>
        public string GetExecutableDirectory()
        {
            return Path.GetDirectoryName(Application.ExecutablePath);
        }

        #endregion

        #region Files

        /// <summary>
        /// Gets FileInfo for the specified filename.
        /// </summary>
        /// <param name="filename">The filename to get information on.</param>
        /// <returns>Json encoded FileInfo object.</returns>
        public string GetFileInfo(string filename)
        {
            FileInfo fi = new FileInfo(filename);
            dynamic dfi = new
            {
                Attributes = fi.Attributes,
                CreationTime = fi.CreationTime,
                CreationTimeUtc = fi.CreationTimeUtc,
                DirectoryName = fi.DirectoryName,
                Exists = fi.Exists,
                Extension = fi.Extension,
                FullName = fi.FullName,
                IsReadOnly = fi.IsReadOnly,
                LastAccessTime = fi.LastAccessTime,
                LastAccessTimeUtc = fi.LastAccessTimeUtc,
                LastWriteTime = fi.LastWriteTime,
                LastWriteTimeUtc = fi.LastWriteTimeUtc,
                Length = fi.Length,
                Name = fi.Name
            };

            string json = JsonConvert.SerializeObject(dfi);
            return json;
        }

        /// <summary>
        /// Gets list of files matching a pattern within a parent directory.
        /// </summary>
        /// <param name="parentDir">Parent directory to search within.</param>
        /// <param name="searchPattern">Optional wildcard search pattern to filter on.</param>
        /// <returns></returns>
        public string GetFiles(string parentDir, string searchPattern)
        {
            if (searchPattern == null)
            {
                searchPattern = "*.*";
            }

            // Any potentially thrown Exceptions will be propagated as javascript exceptions
            string[] result = (searchPattern == null ? Directory.GetFiles(parentDir):
                Directory.GetFiles(parentDir, searchPattern));

            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Opens a text file, reads all lines of the file and returns text as a string.
        /// </summary>
        /// <param name="filename">The file to read from.</param>
        /// <returns></returns>
        public string LoadFile(string filename)
        {
            string result = null;

            try
            {
                result = File.ReadAllText(filename);
            }
            catch (Exception) { }

            return result;
        }

        /// <summary>
        /// Writes a text file with the provided contents string. 
        /// If the file already exists, it will be overwritten.
        /// </summary>
        /// <param name="filename">Filename to write to.</param>
        /// <param name="contents">Contents to write into file.</param>
        public void SaveFile(string filename, string contents)
        {
            File.WriteAllText(filename, contents);
        }

        /// <summary>
        /// Copies an existing file to a new file.  Overwriting is not allowed.
        /// </summary>
        /// <param name="source">Filename to copy from.</param>
        /// <param name="dest">Filename to copy to (must not already exist).</param>
        public void CopyFile(string source, string dest)
        {
            File.Copy(source, dest);
        }

        /// <summary>
        /// Deletes the specified file.
        /// </summary>
        /// <param name="filename">Name of file to delete. Wildcard characters are not supported.</param>
        public void DeleteFile(string filename)
        {
            File.Delete(filename);
        }

        #endregion

    }
}
