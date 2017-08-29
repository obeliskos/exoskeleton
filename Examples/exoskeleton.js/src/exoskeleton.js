/**
 *  exoskeleton.js
 *  @author Obeliskos
 *
 *  A wrapper framework for com object functionality exposed by the exoskeleon shell.
 *  This module self-establishes an instance variable 'exoskeleton'.
 */
(function (root, factory) {
    if (typeof define === 'function' && define.amd) {
        // AMD
        define([], factory);
    } else if (typeof exports === 'object') {
        // Node, CommonJS-like
        module.exports = factory();
    } else {
        // Browser globals (root is window)
        root.Exoskeleton = factory();
    }
}(this, function () {
    return (function () {
        'use strict';

        function overrideConsole() {
            window.console = {
                log: function (text) {
                    if (!exoskeleton.logger) return;

                    exoskeleton.logger.logText(text);
                },
                info: function (text) {
                    if (!exoskeleton.logger) return;

                    exoskeleton.logger.logInfo("", text);
                },
                warn: function (text) {
                    if (!exoskeleton.logger) return;

                    exoskeleton.logger.logWarning("", text);
                },
                error: function (text) {
                    if (!exoskeleton.logger) return;

                    exoskeleton.logger.logError(text);
                },
                dir: function (obj) {
                    if (!exoskeleton.logger) return;

                    exoskeleton.logger.logText(obj ? JSON.stringify(obj, null, 2) : "null");
                }
            };
        }

        /**
         * Exoskeleton main javascript facade interface to the C# API exposed via COM.
         *
         * @constructor Exoskeleton
         */
        function Exoskeleton() {
            var self = this;

            this.exo = window.external;

            this.events = new ExoEventEmitter(this.exo);

            if (!window || !window.external) {
                throw new Error("Exoskeleton could not find the COM interface exposed by exoskeleton");
                return;
            }

            // establish global function for c# to broadcast events to
            window.exoskeletonEmitEvent = function (eventName, eventData) {
                self.events.emit(eventName, eventData);
            };

            // let's also assume control over errors raised and pipe them through our own logger
            window.onerror = function (msg, url, line, col, error) {
                self.logger.logError(msg, url, line, col, error);
                return true;
            };

            overrideConsole();

            // go ahead and instance the keystore adapter for peristable key/value store 
            // and / or to use as a LokiJS persistence adapter.
            this.KeyStoreAdapter = new KeyStoreAdapter(this.exo.File);

            if (this.exo.Media) this.media = new Media(this.exo.Media);
            if (this.exo.Com) this.com = new Com(this.exo.Com);
            if (this.exo.File) this.file = new File(this.exo.File);
            if (this.exo.Main) this.main = new Main(this.exo.Main);
            if (this.exo.Proc) this.proc = new Proc(this.exo.Proc);
            if (this.exo.Session) this.session = new Session(this.exo.Session);
            if (this.exo.System) this.system = new System(this.exo.System);
            if (this.exo.Logger) this.logger = new Logger(this.exo.Logger);
            if (this.exo.Net) this.net = new Net(this.exo.Logger);
            if (this.exo.Enc) this.enc = new Enc(this.exo.Enc);
        }

        /**
         * Initiates shutdown of this exoskeleton app by notifying the container.
         * @memberOf Exoskeleton
         */
        Exoskeleton.prototype.shutdown = function () {
            this.exo.Shutdown();
        };

        // #region Media

        /**
         * Media API class facade.
         *
         * @param {object} exoMedia - reference to the real 'Media' COM API class.
         * @constructor Media
         */
        function Media(exoMedia) {
            this.exoMedia = exoMedia;
        }

        /**
         * Invokes text-to-speech to speak the provided message.
         * @param {string} message - The message to speak.
         * @memberof Media
         * @example
         * exoskeleton.media.speak("this is a test");
         */
        Media.prototype.speak = function (message) {
            this.exoMedia.Speak(message);
        };

        /**
         * Invokes text-to-speech to synchronously speak the provided message.
         * @param {string} message - The message to speak.
         * @memberof Media
         * @example
         * exoskeleton.media.speakSync("this is a test");
         */
        Media.prototype.speakSync = function (message) {
            this.exoMedia.SpeakSync(message);
        };

        // #endregion

        // #region Com

        /**
         * Com API class facade.
         * @param {object} exoCom - reference to the real 'Com' COM API class.
         * @constructor Com
         */
        function Com(exoCom) {
            this.exoCom = exoCom;
        }

        /**
         * Allows creation of c# singleton for further operations.
         * @param {string} comObjectName - Com class type name to instance.
         * @memberof Com
         * @example
         * exoskeleton.com.createInstance("SAPI.SpVoice");
         */
        Com.prototype.createInstance = function (comObjectName) {
            this.exoCom.CreateInstance(comObjectName);
        };

        /**
         * Allows invocation of a method on the global singleton com object instance.
         * @param {string} methodName - Com interface method to invoke.
         * @param {any[]} methodParams - Parameters to pass to com interface method.
         * @memberof Com
         * @example
         * exoskeleton.com.invokeMethod("Speak", ["this is a test message scripting activex from java script", 1])
         */
        Com.prototype.invokeMethod = function (methodName, methodParams) {
            this.exoCom.InvokeMethod(methodName, JSON.stringify(methodParams));
        };

        /**
         * Activates instance to Com type, calls a single method (with params) and then disposes instance.
         * @param {string} comObjectName - Com class type name to instance.
         * @param {string} methodName - Com interface method to invoke.
         * @param {any[]} methodParams - Parameters to pass to com interface method.
         * @memberof Com
         * @example
         * exoskeleton.com.createAndInvokeMethod ("SAPI.SpVoice", "Speak",
         *     ["this is a test message scripting activex from java script", 1]);
         */
        Com.prototype.createAndInvokeMethod = function (comObjectName, methodName, methodParams) {
            this.exoCom.CreateAndInvokeMethod(comObjectName, methodName, JSON.stringify(methodParams));
        };

        // #endregion

        // #region File

        /**
         * File API class facade.
         * @param {object} exoFile - reference to the real 'File' COM API class.
         * @constructor File
         */
        function File(exoFile) {
            this.exoFile = exoFile;
        }

        /**
         * Starts our container singleton watcher on path specified by user.
         * Events will be emitted as multicast, so if multiple forms load
         * multiple watchers, they should distinguish themselves with the
         * eventBaseName parameter.
         * @param {string} path - Path to 'watch'.
         * @param {string} eventBaseName - Optional base name to emit with.
         * @memberof File
         * @example
         * // The following will generate 'download.created', 'download.deleted', and 'download.changed' events.
         * exoskeleton.file.startWatcher("C:\downloads", "download");
         * exoskeleton.events.on("download.created", function(data) {
         *   // data params is json encoded string (for now).
         *   exoskeleton.logger.logInfo("download.created", data);
         * });
         */
        File.prototype.startWatcher = function (path, eventBaseName) {
            this.exoFile.StartWatcher(path, eventBaseName);
        };

        /**
         * Disables the watcher singleton.
         * @memberof File
         * @example
         * exoskeleton.file.stopWatcher();
         */
        File.prototype.stopWatcher = function () {
            this.exoFile.StopWatcher();
        }

        /**
         * Gets information about each of the mounted drives.
         * @memberof File
         * @example
         * var driveInfo = exoskeleton.file.getDriveInfo();
         * driveInfo.forEach(function(drive) {
         *   console.log(drive.Name + ":" + drive.AvailableFreeSpace);
         * });
         */
        File.prototype.getDriveInfo = function () {
            return JSON.parse(this.exoFile.GetDriveInfo());
        };

        /**
         * Gets a list of logical drives.
         * @memberof File
         * @example
         * var result = exoskeleton.file.getLogicalDrives();
         * console.log(result);
         * // logs : C:\,D:\,Z:\
         */
        File.prototype.getLogicalDrives = function () {
            return JSON.parse(this.exoFile.GetLogicalDrives());
        };

        /**
         * Returns the directory portion of the path without the filename.
         * @param {string} path - The full pathname to get directory portion of.
         * @memberof File
         * @example
         * console.log(exoskeleton.file.getDirectoryName("c:\\downloads\\myfile.txt"));
         * // logs : "c:\downloads"
         */
        File.prototype.getDirectoryName = function (path) {
            return this.exoFile.GetDirectoryName(path);
        };

        /**
         * Returns the filename portion of the path without the directory.
         * @param {string} path - The full pathname to get filename portion of.
         * @memberof File
         * @example
         * console.log(exoskeleton.file.getFileName("c:\\downloads\\myfile.txt"));
         * // logs : "myfile.txt"
         */
        File.prototype.getFileName = function (path) {
            return this.exoFile.GetFileName(path);
        };

        /**
         * Gets the file extension of the fully qualified path.
         * @param {string} path - The filepath to get extension of.
         * @memberof File
         * @example
         * console.log(exoskeleton.file.getExtension("c:\\downloads\\myfile.txt"));
         * // logs : ".txt"
         */
        File.prototype.getExtension = function (path) {
            return this.exoFile.GetExtension(path);
        };

        /**
         * Combine multiple paths into one.
         * @param {string[]} paths - Array of paths to combine.
         * @returns {string} - Combined path string.
         * @memberof File
         * @example
         * var fullyQualifiedPath = exoskeleton.file.combinePaths(["c:\\downloads", "myfile.txt"]);
         * console.log(fullyQualifiedPath);
         * // "c:\downloads\myfile.txt"
         */
        File.prototype.combinePaths = function (paths) {
            return this.exoFile.CombinePaths(paths);
        };

        /**
         * Gets DirectoryInfo for the specified directory path.
         * @param {string} path - Name of the directory to get information for.
         * @returns {object} - Object containing directory info as properties.
         * @memberof File
         * @example
         * console.dir(exoskeleton.file.getDirectoryInfo("c:\\downloads"));
         */
        File.prototype.getDirectoryInfo = function (path) {
            return JSON.parse(this.exoFile.GetDirectoryInfo(path));
        };

        /**
         * Gets subdirectory names of a parent directory.
         * @param {string} parentDir - Directory to list subdirectories for.
         * @returns {string[]} - string array of subdirectories.
         * @memberof File
         * @example
         * var result = exoskeleton.file.getDirectories("c:\\downloads");
         * console.dir(result);
         * // might log:
         * // [
         * // "c:\\downloads\\subdir1",
         * // "c:\\downloads\\subdir2",
         * // "c:\\downloads\\subdir3"
         * // ]
         */
        File.prototype.getDirectories = function (parentDir) {
            return JSON.parse(this.exoFile.GetDirectories(parentDir));
        };

        /**
         * Creates all directories and subdirectories in the specified path unless they already exist.
         * @param {string} path - The directory to create.
         * @memberof File
         * @example
         * exoskeleton.file.createDirectory("c:\\downloads\\subdir4");
         */
        File.prototype.createDirectory = function (path) {
            this.exoFile.CreateDirectory(path);
        };

        /**
         * Deletes an empty directory.
         * @param {string} path - The name of the empty directory to delete.
         * @memberof File
         * @example
         * exoskeleton.file.deleteDirectory("c:\\downloads\\subdir4");
         */
        File.prototype.deleteDirectory = function (path) {
            this.exoFile.DeleteDirectory(path);
        };

        /**
         * Gets the directory where the exoskeleton executable was loaded from.
         * @memberof File
         */
        File.prototype.getExecutableDirectory = function () {
            return this.exoFile.GetExecutableDirectory()
        };

        /**
         * Gets FileInfo for the specified filename.
         * @param {string} filename - The filename to get information on.
         * @returns {object} - Json object representation of FileInfo class.
         * @memberof File
         */
        File.prototype.getFileInfo = function (filename) {
            return this.exoFile.GetFileInfo(filename);
        };

        /**
         * Gets list of files matching a pattern within a parent directory.
         * @param {string} parentDir - Parent directory to search within.
         * @param {string} searchPattern - Optional wildcard search pattern to filter on.
         * @memberof File
         */
        File.prototype.getFiles = function (parentDir, searchPattern) {
            return JSON.parse(this.exoFile.GetFiles(parentDir, searchPattern));
        };

        /**
         * Opens a text file, reads all lines of the file and returns text as a string.
         * @param {string} filename - The file to read from.
         * @returns {string} - file contents as string.
         * @memberof File
         */
        File.prototype.loadFile = function (filename) {
            return this.exoFile.LoadFile(filename);
        };

        /**
         * Writes a text file with the provided contents string.
         * If the file already exists, it will be overwritten.
         * @param {string} filename - Filename to write to.
         * @param {string} contents - Contents to write into file.
         * @memberof File
         */
        File.prototype.saveFile = function (filename, contents) {
            this.exoFile.SaveFile(filename, contents);
        };

        /**
         * Copies an existing file to a new file.  Overwriting is not allowed.
         * @param {string} source - Filename to copy from.
         * @param {string} dest - Filename to copy to (must not already exist).
         * @memberof File
         */
        File.prototype.copyFile = function (source, dest) {
            this.exoFile.CopyFile(source, dest);
        };

        /**
         * Deletes the specified file.
         * @param {string} filename - Name of file to delete. Wildcard characters are not supported.
         * @memberof File
         */
        File.prototype.deleteFile = function (filename) {
            this.exoFile.DeleteFile(filename);
        };

        // #endregion

        // #region Main

        /**
         * Main API class facade.
         * @param {object} exoMain - reference to the real 'Main' COM API class.
         * @constructor Main
         */
        function Main(exoMain) {
            this.exoMain = exoMain;
        }

        /**
         * Updates the window title for the host container.
         * @param {string} title - Text to apply to window title.
         * @memberof Main
         */
        Main.prototype.setWindowTitle = function (title) {
            this.exoMain.SetWindowTitle(title);
        };

        /**
         * Signals the host container to enter fullscreen mode.
         * @memberof Main
         */
        Main.prototype.fullscreen = function () {
            this.exoMain.Fullscreen();
        };

        /**
         * Signals the host container to exit fullscreen mode.
         * @memberof Main
         */
        Main.prototype.exitFullscreen = function () {
            this.exoMain.ExitFullscreen();
        };

        /**
         * Displays a windows system tray notification.
         * @param {string} title - The notification title.
         * @param {string} message - The notification message.
         * @memberof Main
         */
        Main.prototype.showNotification = function (title, message) {
            this.exoMain.ShowNotification(title, message);
        };

        /**
         * Opens a new host container with the url and settings provided.
         * @param {string} caption - Window caption to apply to new window.
         * @param {string} url - Url to load within the new window.
         * @param {int} width - Width (in pixels) to size new window to.
         * @param {int} height - Height (in pixels) to size new window to.
         * @memberof Main
         */
        Main.prototype.openNewWindow = function (caption, url, width, height) {
            this.exoMain.OpenNewWindow(caption, url, width, height);
        };

        // #endregion

        // #region Proc

        /**
         * Proc API class facade.
         * @param {object} exoProc - reference to the real 'Proc' COM API class.
         * @constructor Proc
         */
        function Proc(exoProc) {
            this.exoProc = exoProc;
        }

        /**
         * Starts a process resource by specifying the name of a document or application file.
         * @param {string} procPath - Program to execute.
         * @memberof Proc
         */
        Proc.prototype.startPath = function (procPath) {
            this.exoProc.StartPath(procPath);
        };

        /**
         * Starts a process resource by providing information in a ProcessStartInfo format.
         * @param {string} processStartInfo - Serialized javascript object closely resembling a c# ProcessStartInfo object.
         * @memberof Proc
         */
        Proc.prototype.start = function (processStartInfo) {
            return this.exoProc.Start(JSON.stringify(processStartInfo));
        };

        /**
         * Gets a list of running processes.
         * @memberof Proc
         */
        Proc.prototype.getProcesses = function () {
            return JSON.parse(this.exoProc.GetProcesses());
        };

        /**
         * Gets a list of processes of the provided name.
         * @param {string} name - name of process to get list of.
         * @memberof Proc
         */
        Proc.prototype.getProcessesByName = function (name) {
            return JSON.parse(this.exoProc.GetProcessesByName(name));
        };

        /**
         * Kills a running process.
         * @param {int} id - The id of the process to kill.
         * @memberof Proc
         */
        Proc.prototype.killProcessById = function (id) {
            return this.exoProc.KillProcessById(id);
        };

        // #endregion

        // #region Session

        /**
         * Session API class facade.
         * @param {object} exoSession - reference to the real 'Session' COM API class.
         * @constructor Session
         */
        function Session(exoSession) {
            this.exoSession = exoSession;
        }

        /**
         * Looks up the (string) Value for the Session key provided.
         * @param {string} key - The key name to lookup a value for in the session store.
         * @returns {string} - The value associated with key in string form.
         * @memberof Session
         */
        Session.prototype.get = function (key) {
            return this.exoSession.Get(key);
        };

        /**
         * Assigns a key/value setting within the session store.
         * @param {string} key - The name of the session variable to set.
         * @param {string} value - The string value to assign to session variable.
         * @memberof Session
         */
        Session.prototype.set = function (key, value) {
            this.exoSession.Set(key, value);
        };

        /**
         * Looks up the (object) Value for the Session key provided.
         * @param {string} key - The key name to lookup a value for in the session store.
         * @returns {object} - The value associated with key parsed into object.
         * @memberof Session
         */
        Session.prototype.getObject = function (key) {
            var result = this.exoSession.Get(key);
            return result ? JSON.parse(result) : result;
        };

        /**
         * Assigns a key/value setting within the session store by serializing it.
         * @param {string} key - The name of the session variable to set.
         * @param {object} value - The object value to assign to session variable.
         * @memberof Session
         */
        Session.prototype.setObject = function (key, value) {
            this.exoSession.Set(key, JSON.stringify(value));
        };

        /**
         * Obtains a string list of all keys currently in the session store.
         * @returns {string[]} - An array of string 'keys' within the session store.
         * @memberof Session
         */
        Session.prototype.list = function () {
            return JSON.parse(this.exoSession.list());
        };

        // #endregion

        // #region System

        /**
         * System API class facade.
         * @param {object} exoSystem - reference to the real 'System' COM API class.
         * @constructor System
         */
        function System(exoSystem) {
            this.exoSystem = exoSystem;
        }

        /**
         * Get information about the system which this program is being run on.
         * @returns {object} - Json system information object.
         * @memberof System
         */
        System.prototype.getSystemInfo = function () {
            return JSON.parse(this.exoSystem.GetSystemInfo());
        };

        /**
         * Retrieves a single environment variable value.
         * @param {string} varName - The name of the environment variable to retrieve value for.
         * @returns {string=} - The string value of the environment variable (if found).
         * @memberof System
         */
        System.prototype.getEnvironmentVariable = function (varName) {
            return this.exoSystem.GetEnvironmentVariable(varName);
        };

        /**
         * Returns a list of all environment variables as properties and property values.
         * @returns {object} - Json hash object with properties representing variables.
         * @memberof System
         */
        System.prototype.getEnvironmentVariables = function () {
            return this.exoSystem.GetEnvironmentVariables();
        };

        /**
         * Sets an environment variable only within this process or child processes.
         * @param {string} varName - The name of the environment variable.
         * @param {string} varValue - The value to assign to the environment variable.
         * @memberof System
         */
        System.prototype.setEnvironmentVariable = function (varName, varValue) {
            return this.exoSystem.SetEnvironmentVariable(varName, varValue);
        };

        /**
         * Finds an existing application window by either class or name and focuses it.
         * @param {string=} className - The class name of the window, or null.
         * @param {string=} windowName - The window name of the window, or null.
         * @memberof System
         */
        System.prototype.focusWindow = function (className, windowName) {
            this.exoSystem.FocusWindow(className, windowName);
        };

        /**
         * Finds a window and sends keycodes to it.
         * @param {string=} className - The class name of the window, or null.
         * @param {string=} windowName - The window name of the window, or null.
         * @param {string[]} keys - String array of keys or keycodes to send.
         * @returns {bool} - Whether the window was found.
         * @memberof System
         */
        System.prototype.focusAndSendKeys = function (className, windowName, keys) {
            return this.exoSystem.FocusAndSendKeys(className, windowName, keys);
        }

        // #endregion

        // #region Logger

        /**
         * Logger API class facade.
         * @param {object} exoLogger - reference to the real COM API Logger class.
         * @constructor Logger
         */
        function Logger(exoLogger) {
            this.exoLogger = exoLogger;
        }

        /**
         * Logs an "info" message to the logger's message list.
         * @param {string} source - Descriptive 'source' of the info.
         * @param {string} message - Info detail message
         * @memberof Logger
         */
        Logger.prototype.logInfo = function (source, message) {
            this.exoLogger.LogInfo(source, message);
        };

        /**
         * Logs a "warning" message to the logger's message list.
         * @param {string} source - Descriptive 'source' of the warning.
         * @param {string} message - Detailed warning message.
         * @memberof Logger
         */
        Logger.prototype.logWarning = function (source, message) {
            this.exoLogger.LogWarning(source, message);
        };

        /**
         * Logs an "error" message to the logger's message list.
         * @param {string} msg - Message to log.
         * @param {string} url - The url of the script where the error occurred.
         * @param {string} line - Line number of the javascript where the error occurred.
         * @param {string} col - Column number of the javascript where the error occurred.
         * @param {string} error - Detailed informatino about the error.
         * @memberof Logger
         */
        Logger.prototype.logError = function (msg, url, line, col, error) {
            this.exoLogger.LogError(msg, url, line, col, error);
        };

        /**
         * Logs text to the logger's console.
         * @param {string} message - Text to append to the console.
         * @memberof Logger
         */
        Logger.prototype.logText = function (message) {
            this.exoLogger.LogText(message);
        };

        // #endregion Logger

        // #region Net

        /**
         * Net API class facade.
         * @param {object} exoNet - reference to the real COM 'Net' API class.
         * @constructor Net
         */
        function Net(exoNet) {
            this.exoNet = exoNet;
        }

        /**
         * Downloads from an internet url and saves to disk.
         * @param {string} url - The internet url to download from.
         * @param {string} dest - Destination filename on disk.
         * @param {bool} async - Whether to wait until finished before returning.
         * @memberof Net
         */
        Net.prototype.downloadFile = function (url, dest, async) {
            return this.exoNet.DownloadFile(url, dest, async);
        };

        /**
         * Fetches text-based resource at the provided url and returns a string of its content.
         * @param {string} url - Internet url of text based resource.
         * @returns {string} - String containing text within the retrieved resource.
         * @memberof Net
         */
        Net.prototype.readUrl = function (url) {
            return this.exoNet.ReadUrl(url);
        };

        // #endregion

        // #region Enc

        /**
         * Enc API class facade.
         * @param {object} exoEnc - reference to the real COM 'Enc' API class.
         * @constructor Enc
         */
        function Enc(exoEnc) {
            this.exoEnc = exoEnc;
        }

        /**
         * Encrypts a string using the provided password.
         * @param {string} data - The string to encrypt.
         * @param {string} password - The password to encrypt with.
         * @memberof Enc
         */
        Enc.prototype.encrypt = function (data, password) {
            return this.exoEnc.Encrypt(data, password);
        };

        /**
         * Decrypts a string using the provided password.
         * @param {string} data - The string to decrypt.
         * @param {string} password - The password to decrypt with.
         * @memberof Enc
         */
        Enc.prototype.decrypt = function (data, password) {
            return this.exoEnc.Decrypt(data, password);
        };

        /**
         * Create encrypted copies of the specified file(s) using the provided password.
         * @param {string} filemask - filename or wildcard pattern of files to encrypt.
         * @param {string} password - The password to encrypt with.
         * @memberof Enc
         */
        Enc.prototype.encryptFiles = function (filemask, password) {
            return this.exoEnc.EncryptFiles(filemask, password);
        };

        /**
         * Create decrypted copies of the specified file(s) using the provided password.
         * @param {string} filemask - filename or wildcard pattern of files to decrypt.
         * @param {string} password - The password to decrypt with.
         * @memberof Enc
         */
        Enc.prototype.DecryptFiles = function(filemask, password) {
            return this.exoEnc.DecryptFiles(filemask, password);
        };

        /**
         * Creates ana MD5 Hash for the file specified by the provided filename.
         * @param {string} filename - The filename of the file to calculate a hash file.
         * @memberof Enc
         */
        Enc.prototype.CreateMD5Hash = function(filename) {
            return this.exoEnc.GetBase64EncodedMD5Hash(filename);
        };

        /**
         * Creates ana SH1 Hash for the file specified by the provided filename.
         * @param {string} filename : The filename of the file to calculate a hash file.
         * @memberof Enc
         */
        Enc.prototype.CreateSHA1Hash = function(filename) {
            return this.exoEnc.GetBase64EncodedSHA1Hash(filename);
        };

        /**
         * Creates ana SHA256 Hash for the file specified by the provided filename.
         * @param {string} filename - The filename of the file to calculate a hash file.
         * @memberof Enc
         */
        Enc.prototype.CreateSHA256Hash = function(filename) {
            return this.exoEnc.GetBase64EncodedSHA256Hash(filename);
        };

        /**
         * Creates MD5, SHA1, and SHA256 hashes for the file(s) specified.
         * @param {string} filemask - The filename or filemask for file(s) to calculate hashes for.
         * @memberof Enc
         */
        Enc.prototype.HashFiles = function (filemask) {
            return this.exoEnc.HashFiles(filemask);
        };

        // #endregion

        // #region KeyStoreAdapter

        /**
         * Persistent Key/Value store which can also be used as a loki database persistence adapter.
         * @param {any} exoskeleton - pass in reference to exoskeleton singleton
         * @constructor KeyStoreAdapter
         */
        function KeyStoreAdapter(exoFile) {
            this.exoFile = exoFile;
        }

        /**
         * An expected method provided for lokijs to load a database from.
         * @param {string} dbname - the name (of the file) to load the database from.
         * @param {function} callback - an optional callback to invoke when loading is complete.
         * @memberof KeyStoreAdapter
         */
        KeyStoreAdapter.prototype.loadDatabase = function (dbname, callback) {
            var result = this.exoFile.LoadFile(dbname);
            if (typeof callback === 'function') {
                callback(result);
            }
        };

        /**
         * Used to load a value asynchronously from the keystore.
         * @param {string} dbname - the key name (filename) to load the value (contents) from.
         * @param {function} callback - the callback to invoke with value param when loading is complete.
         * @memberof KeyStoreAdapter
         */
        KeyStoreAdapter.prototype.loadKey = KeyStoreAdapter.prototype.loadDatabase;

        /**
         * An expected method provided for lokijs to save a database from.
         * @param {string} dbname - the name (of the file) to load the database from.
         * @param {function} callback - an optional callback to invoke when loading is complete.
         * @memberof KeyStoreAdapter
         */
        KeyStoreAdapter.prototype.saveDatabase = function (dbname, dbstring, callback) {
            // synchronous? for now
            this.exoFile.SaveFile(dbname, dbstring);

            if (typeof callback === 'function') {
                callback(null);
            }
        };

        /**
         * Used to asynchronously save a key/value into the (file based) keystore.
         * @param {string} dbname - the name (of the file) to save the key/value to.
         * @param {function} callback - an optional callback to invoke when saving is complete.
         * @memberof KeyStoreAdapter
         */
        KeyStoreAdapter.prototype.saveKey = KeyStoreAdapter.prototype.saveDatabase;

        // #endregion

        // #region ExoEventEmitter

        /**
         * Event emitter for listening to or emitting events within your page(s)
         *
         * This is exposed via exoskeleton.events.
         * May be used internally to your container or used to emit or listen to
         * 'multicast' events which are propagated to all host windows.
         *
         * Event names starting with 'local.' are not to be multicast,
         * otherwise the event will be multicast with an eventname prefixed
         * with 'multicast.'.
         *
         * @param {object} exo - Instance to the exoskeleton com interface
         * @param {object} options - options to configure event emitter with.
         * @param {bool=} [options.asyncListeners=false] - whether events will be emitted asynchronously.
         * @constructor ExoEventEmitter
         */
        function ExoEventEmitter(exo, options) {
            this.exo = exo;

            options = options || {};
            if (options.asyncListeners) {
                this.asyncListeners = options.asyncListeners;
            }
        }

        /**
         * Hashobject for storing the registered events and callbacks
         */
        ExoEventEmitter.prototype.events = {};

        /**
         * Whether events should be emitted immediately (true) or whenever thread is yielded (false).
         */
        ExoEventEmitter.prototype.asyncListeners = false;

        /**
         * Used to register a listener to an event.
         * @param {string} eventName - name of the event to listen for.
         * @param {function} listener - a callback to invoke when event is emitted.
         */
        ExoEventEmitter.prototype.on = function (eventName, listener) {
            var event;
            var self = this;

            if (Array.isArray(eventName)) {
                eventName.forEach(function (currentEventName) {
                    self.on(currentEventName, listener);
                });
                return listener;
            }

            event = this.events[eventName];
            if (!event) {
                event = this.events[eventName] = [];
            }
            event.push(listener);
            return listener;
        };

        /**
         * Used to emit a specific event, possibly with additional parameter data.
         * @param {string} eventName - the name of the event to emit.
         * @param {...any} args - additional parameter data to pass into listener callbacks.
         */
        ExoEventEmitter.prototype.emit = function (eventName) {
            var self = this;
            var selfArgs = Array.prototype.slice.call(arguments, 1);
            if (eventName && this.events[eventName]) {
                this.events[eventName].forEach(function (listener) {
                    if (self.asyncListeners) {
                        setTimeout(function () {
                            listener.apply(self, selfArgs);
                        }, 1);
                    } else {
                        listener.apply(self, selfArgs);
                    }

                });
            }

            // events whose name starts with 'local.' or 'multicast.' will not be (re)multicast
            if (eventName.indexOf("local.") !== 0 && eventName.indexOf("multicast.") !== 0) {
                this.exo.MulticastEvent(eventName, JSON.stringify(selfArgs));
            }
        };

        // #endregion

        // although we instance these for our own use, lets expose the constructors to these classes
        Exoskeleton.EventEmitter = ExoEventEmitter;
        Exoskeleton.KeyStoreAdapter = KeyStoreAdapter;

        return Exoskeleton;
    }());
}));

var exoskeleton = new Exoskeleton();