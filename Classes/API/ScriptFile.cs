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
            watcherBaseName = (eventBaseName==null)?"watcher":watcherBaseName;

            watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                   | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = "*.*";
            watcher.Changed += watcher_Changed;
            watcher.Created += watcher_Created;
            watcher.Deleted += watcher_Deleted;
            watcher.Renamed += watcher_Renamed;
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
        /// 'Created' event handler for our watcher singleton (if started)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void watcher_Created(object sender, FileSystemEventArgs e)
        {
            dynamic[] data = new dynamic[] { JsonConvert.SerializeObject(e) };

            host.PackageAndMulticast(watcherBaseName + ".created", data);
        }

        /// <summary>
        /// 'Changed' event handler for our watcher singleton (if started)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            dynamic[] data = new dynamic[] { JsonConvert.SerializeObject(e) };

            host.PackageAndMulticast(watcherBaseName + ".changed", data);
        }

        /// <summary>
        /// 'Deleted' event handler for our watcher singleton (if started).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            dynamic[] data = new dynamic[] { JsonConvert.SerializeObject(e) };

            host.PackageAndMulticast(watcherBaseName + ".deleted", data);
        }

        /// <summary>
        /// 'Renamed' event handler for our watcher singleton (if started).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void watcher_Renamed(object sender, RenamedEventArgs e)
        {
            dynamic[] data = new dynamic[] { JsonConvert.SerializeObject(e) };

            host.PackageAndMulticast(watcherBaseName + ".renamed", data);
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
        /// <returns>A single combined path.</returns>
        public string CombinePaths(string pathsJson)
        {
            List<string> paths = JsonConvert.DeserializeObject<List<string>>(pathsJson);
            return Path.Combine(paths.ToArray());
        }

        /// <summary>
        /// Combines multiple paths to a single source path.
        /// </summary>
        /// <param name="source">Base folder to combine with</param>
        /// <param name="pathsJson">Json encoded array of paths to combine with source.</param>
        /// <returns>List of combined paths.</returns>
        public string CombinePathsArray(string source, string pathsJson)
        {
            List<string> paths = JsonConvert.DeserializeObject<List<string>>(pathsJson);

            List<string> combinedPaths = new List<string>();
            foreach(string path in paths)
            {
                combinedPaths.Add(Path.Combine(source, path));
            }

            return JsonConvert.SerializeObject(combinedPaths);
        }

        /// <summary>
        /// Used for resolving paths comtaining inner relative paths
        /// </summary>
        /// <param name="path">Path containing possible inner relative paths</param>
        /// <returns></returns>
        public string GetFullPath(string path)
        {
            return Path.GetFullPath(path);
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

            // Some properties (their accessors) may throw exception if their value is non-applicable in the 
            // process current state so we will ignore errors by providing our own error delegate.
            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.Error += delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
            {
                args.ErrorContext.Handled = true;
            };

            string json = JsonConvert.SerializeObject(ddi, jss);

            return json;
        }

        /// <summary>
        /// Gets subdirectory names of a parent directory.
        /// </summary>
        /// <param name="parentDir">Directory to list subdirectories for.</param>
        /// <returns>Json encoded string array of subdirectories.</returns>
        public string GetDirectories(string parentDir)
        {
            string[] result = Directory.GetDirectories(parentDir);

            return JsonConvert.SerializeObject(result);
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
        public void DeleteDirectory(string path, bool recursive)
        {
            Directory.Delete(path, recursive);
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

            long? length = fi.Exists?fi.Length: (long?) null;

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
                Length = length,
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
        /// Gets list of files ending in any of the extensions provided
        /// </summary>
        /// <param name="parentDir">Parent directory to search within.</param>
        /// <param name="extensions">Comma delimited list of ending string (not wildcards)</param>
        /// <returns>string array of matching files</returns>
        public string GetFilesEndingWith(string parentDir, string extensions)
        {
            string[] patterns = extensions.Split(',');

            var files = Directory
                .GetFiles(parentDir)
                .Where(file => patterns.Any(file.ToLower().EndsWith))
                .ToList();

            return JsonConvert.SerializeObject(files);
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
        /// Reads a binary file into a Base64 string.
        /// </summary>
        /// <param name="filename">The file to read from.</param>
        /// <returns></returns>
        public string LoadFileBase64(string filename)
        {
            return Convert.ToBase64String(File.ReadAllBytes(filename));
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
        /// Writes a binary file with bytes derived from the provided base64 string.
        /// </summary>
        /// <param name="filename">Filename to write to.</param>
        /// <param name="contents">Base64 encoded binary content</param>
        public void SaveFileBase64(string filename, string contents)
        {
            File.WriteAllBytes(filename, Convert.FromBase64String(contents));
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

        public string FilePathToUri(string filepath)
        {
            return (new Uri(filepath)).AbsoluteUri;
        }

        #endregion

    }
}
