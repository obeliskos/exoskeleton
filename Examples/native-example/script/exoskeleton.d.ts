/**
 *  exoskeleton.js is a wrapper interface for accessing com object functionality exposed by the exoskeleon shell.
 *  @author Obeliskos
 *
 *  @todo
 *    - create callback interfaces to help callers determine parameters supported in those callbacks
 */
interface IHash<T> {
    [key: string]: T;
}
declare enum ExoskeletonAnchorStyles {
    "None" = 0,
    "Top" = 1,
    "Bottom" = 2,
    "Left" = 4,
    "Right" = 8
}
declare type ExoskeletonLocations = {
    Executable: string;
    Settings: string;
    Current: string;
    Documents: string;
    Music: string;
    Pictures: string;
    Videos: string;
    WebServerSelfHost: boolean;
    WebServerHostDirectory: string;
    WebServerRoot: string;
    WebServerListenPort: number;
    WebServerActualPort: number;
    WebBrowserBaseUrl: string;
    WebBrowserDefaultUrl: string;
    WebBrowserDefaultUrlIsFile: boolean;
    WindowIconPath: string;
};
interface eventedCallback {
    (data: any, ...args: any[]): void;
}
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
 * @param {boolean=} [options.asyncListeners=false] - whether events will be emitted asynchronously.
 * @constructor ExoEventEmitter
 */
declare class ExoEventEmitter {
    exo: any;
    /**
     * Whether events should be emitted immediately (true) or whenever thread is yielded (false).
     */
    asyncListeners: boolean;
    /**
     * Hashobject for storing the registered events and callbacks
     */
    events: IHash<any>;
    constructor(exo: any, options?: any);
    /**
     * Clears all event listeners
     * @param {string|string[]=} eventName - event(s) to clear listeners, or all if nothing passed.
     * @memberof ExoEventEmitter
     * @instance
     * @example
     * // clear event listeners for all registered events
     * exoskeleton.events.clear();
     * // clear event listeners for a single event
     * exoskeleton.events.clear("FileMenuClicked");
     * // clear event listeners for multiple named events
     * exoskeleton.events.clear(["FileMenuClicked", "HelpMenuClicked"]);
     */
    clear(eventName: string): void;
    /**
     * Used to register a listener to an event.
     * @param {string} eventName - name of the event to listen for.
     * @param {function|function[]} listener - a callback to invoke when event is emitted.
     * @returns {function} returns the same listener passed in as a aparam.
     * @memberof ExoEventEmitter
     * @instance
     */
    on(eventName: string, listener: eventedCallback): eventedCallback;
    /**
     * Used to emit a specific event, possibly with additional parameter data.
     * @param {string} eventName - the name of the event to emit.
     * @param {...string} args - additional parameter data to pass into listener callbacks.
     * @memberof ExoEventEmitter
     * @instance
     */
    emit(eventName: string, eventData: any): void;
    emitMulticast(eventName: string, eventData: any): void;
}
/**
 * Persistent Key/Value store which can also be used as a loki database persistence adapter.
 */
declare class KeyStoreAdapter {
    exoFile: ExoFile;
    constructor(exoFile: ExoFile);
    /**
     * An expected method provided for lokijs to load a database from.
     * @param {string} dbname - the name (of the file) to load the database from.
     * @param {function} callback - an optional callback to invoke when loading is complete.
     * @memberof KeyStoreAdapter
     * @instance
     */
    loadDatabase(dbname: string, callback: Function): void;
    /**
     * (Alias) Used to load a value asynchronously from the keystore.
     * @param {string} dbname - the key name (filename) to load the value (contents) from.
     * @param {function} callback - the callback to invoke with value param when loading is complete.
     * @memberof KeyStoreAdapter
     * @instance
     */
    loadKey(dbname: string, callback: Function): void;
    /**
     * An expected method provided for lokijs to save a database from.
     * @param {string} dbname - the name (of the file) to load the database from.
     * @param {string} dbstring - the contents of the serialized database or value to save.
     * @param {function} callback - an optional callback to invoke when loading is complete.
     * @memberof KeyStoreAdapter
     * @instance
     */
    saveDatabase(dbname: string, dbstring: string, callback: Function): void;
    /**
     * (Alias) Used to asynchronously save a key/value into the (file based) keystore.
     * @param {string} dbname - the name (of the file) to save the key/value to.
     * @param {function} callback - an optional callback to invoke when saving is complete.
     * @memberof KeyStoreAdapter
     * @instance
     */
    saveKey(dbname: string, dbstring: string, callback: Function): void;
}
/**
 * Media API class for speech and audio/video/image.
 */
declare class ExoMedia {
    exoMedia: any;
    constructor(exoMedia: any);
    /**
     * Provisions a new (empty) named imagelist
     *
     * @param {string} name - name to associate with imagelist
     * @param {object=} properties - allows configuring properties on imagelist
     * @memberof Media
     * @instance
     * @example
     * exoskeleton.media.createImageList("listViewIcons", {
     *   ColorDepth: 32,
     *   ImageSize: "128, 128",
     *   TransparentColor: "Transparent"
     * });
     */
    createImageList(name: string, properties: any): void;
    /**
     * Determines if an imagelist exists by the given name
     * @param {string} name - name of the imagelist to check for existence of
     * @returns {bool} whether the imagelist exists
     * @memberof Media
     * @instance
     */
    imageListExists(name: string): any;
    /**
     * Populates an image list with images
     *
     * @param {string} name - name of the imagelist to load
     * @param {array} filenameList - list of absolute pathnames to images
     * @memberof Media
     * @instance
     * @example
     * exoskeleton.media.loadImageList("listViewImages", [
     *   "c:\imgs\img1.png",
     *   "c:\imgs\img2.jpg",
     *   "c:\imgs\img3.bmp",
     *   "c:\imgs\img4.gif"
     * ]);
     */
    loadImageList(name: string, filenameList: string[]): void;
    /**
     * Clears a named imagelist but leaves it created for future use.
     *
     * @param {string} name - the name of the imagelist to clear
     * @memberof Media
     * @instance
     * @example
     * exoskeleton.media.clearImageList("listViewImages");
     */
    clearImageList(name: string): void;
    /**
     * Removes a named imagelist from memory.
     *
     * @param {string} name - the name of the imagelist to remove
     * @memberof Media
     * @instance
     * @example
     * exoskeleton.media.removeImageList(name);
     */
    removeImageList(name: string): void;
    /**
     * Removes all named imagelists from memory.
     *
     * @memberof Media
     * @instance
     * @example
     * exoskeleton.media.removeAllImageLists();
     */
    removeAllImageLists(): void;
    /**
     * Invokes text-to-speech to speak the provided message.
     * @param {string} message - The message to speak.
     * @memberof Media
     * @instance
     * @example
     * exoskeleton.media.speak("this is a test");
     */
    speak(message: string): void;
    /**
     * Invokes text-to-speech to synchronously speak the provided message.
     * @param {string} message - The message to speak.
     * @memberof Media
     * @instance
     * @example
     * exoskeleton.media.speakSync("this is a test");
     */
    speakSync(message: string): void;
}
/**
 * Com API class for interacting with COM Objects registered on the system.
 */
declare class ExoCom {
    exoCom: any;
    constructor(exoCom: any);
    /**
     * Allows creation of c# singleton for further operations.
     * @param {string} comObjectName - Com class type name to instance.
     * @memberof Com
     * @instance
     * @example
     * exoskeleton.com.createInstance("SAPI.SpVoice");
     */
    createInstance(comObjectName: string): void;
    /**
     * Allows invocation of a method on the global singleton com object instance.
     * @param {string} methodName - Com interface method to invoke.
     * @param {any[]} methodParams - Parameters to pass to com interface method.
     * @memberof Com
     * @instance
     * @example
     * exoskeleton.com.createInstance("SAPI.SpVoice");
     * exoskeleton.com.invokeMethod("Speak", ["this is a test message scripting activex from java script", 1])
     */
    invokeMethod(methodName: string, methodParams: any[]): void;
    /**
     * Activates instance to Com type, calls a single method (with params) and then disposes instance.
     * @param {string} comObjectName - Com class type name to instance.
     * @param {string} methodName - Com interface method to invoke.
     * @param {any[]} methodParams - Parameters to pass to com interface method.
     * @memberof Com
     * @instance
     * @example
     * exoskeleton.com.createAndInvokeMethod ("SAPI.SpVoice", "Speak",
     *     ["this is a test message scripting activex from java script", 1]);
     */
    createAndInvokeMethod(comObjectName: string, methodName: string, methodParams: any[]): void;
}
/**
 * File API class for interfacing with .NET File and Directory classes.
 */
declare class ExoFile {
    exoFile: any;
    constructor(exoFile: any);
    /**
     * Combine multiple paths into one.
     * @param {string[]} paths - Array of paths to combine.
     * @returns {string} - Combined path string.
     * @memberof File
     * @instance
     * @example
     * var fullyQualifiedPath = exoskeleton.file.combinePaths(["c:\\downloads", "myfile.txt"]);
     * console.log(fullyQualifiedPath);
     * // "c:\downloads\myfile.txt"
     */
    combinePaths(paths: string[]): any;
    /**
     * Combines multiple paths with a single base/source path.
     * @param {string} source - base path to join paths with.
     * @param {string[]} paths - array of paths to join against base path.
     * @returns {string[]} - Array of combined path strings.
     * @memberof File
     * @instance
     * @example
     * var paths = exoskeleton.file.combinePathsArray("c:\images", ['img1.jpg', 'img2.jpg', 'img3.jpg']);
     * console.log(paths);
     * // logs: ['c:\images\img1.jpg','c:\images\img2.jpg','c:\images\img3.jpg']
     */
    combinePathsArray(source: string, paths: string[]): any;
    /**
     * Copies an existing file to a new file.  Overwriting is not allowed.
     * @param {string} source - Filename to copy from.
     * @param {string} dest - Filename to copy to (must not already exist).
     * @memberof File
     * @instance
     * @example
     * exoskeleton.file.copyFile("c:\\myfolder\\file1.txt", "c:\\myfolder\\file1.txt.bak");
     */
    copyFile(source: string, dest: string): void;
    /**
     * Creates all directories and subdirectories in the specified path unless they already exist.
     * @param {string} path - The directory to create.
     * @memberof File
     * @instance
     * @example
     * exoskeleton.file.createDirectory("c:\\downloads\\subdir");
     */
    createDirectory(path: string): void;
    /**
     * Deletes an empty directory (if recursive is false),
     * or recursively deletes subfolders and files (if recursive is true).
     * @param {string} path - The name of the empty directory to delete.
     * @param {boolean} recursive - Whether delete should remove files and subdirectories.
     * @memberof File
     * @instance
     * @example
     * // this will only work if the 'subdir' folder is empty
     * exoskeleton.file.deleteDirectory("c:\\downloads\\subdir");
     * // this will wipe out the whole 'subdir' folder even if it has files or subdirectories
     * exoskeleton.file.deleteDirectory("c:\\downloads\\subdir", true);
     */
    deleteDirectory(path: string, recursive: boolean): void;
    /**
     * Deletes the specified file.
     * @param {string} filename - Name of file to delete. Wildcard characters are not supported.
     * @memberof File
     * @instance
     * @example
     * exoskeleton.file.deleteFile("c:\\myfolder\\file1.txt.bak");
     */
    deleteFile(filename: string): void;
    /**
     * Converts a filepath to a Uri
     * @param filepath {string} path to convert to uri
     * @memberof File
     * @instance
     * @returns {string} uri string
     */
    filePathToUri(filepath: string): any;
    /**
     * Gets subdirectory names of a parent directory.
     * @param {string} parentDir - Directory to list subdirectories for.
     * @returns {string[]} - string array of subdirectories.
     * @memberof File
     * @instance
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
    getDirectories(parentDir: string): string[];
    /**
     * Gets DirectoryInfo for the specified directory path.
     * @param {string} path - Name of the directory to get information for.
     * @returns {object} - Object containing directory info as properties.
     * @memberof File
     * @instance
     * @example
     * console.dir(exoskeleton.file.getDirectoryInfo("c:\\downloads"));
     */
    getDirectoryInfo(path: string): any;
    /**
     * Returns the directory portion of the path without the filename.
     * @param {string} path - The full pathname to get directory portion of.
     * @memberof File
     * @instance
     * @returns {string} the directory portion of the path provided.
     * @example
     * console.log(exoskeleton.file.getDirectoryName("c:\\downloads\\myfile.txt"));
     * // logs : "c:\downloads"
     */
    getDirectoryName(path: string): string;
    /**
     * Gets information about each of the mounted drives.
     * @memberof File
     * @instance
     * @returns {object} array of serialized DriveInfo objects
     * @example
     * var driveInfo = exoskeleton.file.getDriveInfo();
     * driveInfo.forEach(function(drive) {
     *   console.log(drive.Name + ":" + drive.AvailableFreeSpace);
     * });
     */
    getDriveInfo(): any[];
    /**
     * Gets the file extension of the fully qualified path.
     * @param {string} path - The filepath to get extension of.
     * @memberof File
     * @instance
     * @returns {string} the file extension portion of the path provided.
     * @example
     * console.log(exoskeleton.file.getExtension("c:\\downloads\\myfile.txt"));
     * // logs : ".txt"
     */
    getExtension(path: string): string;
    /**
     * Gets FileInfo for the specified filename.
     * @param {string} filename - The filename to get information on.
     * @returns {object} - Json object representation of FileInfo class.
     * @memberof File
     * @instance
     */
    getFileInfo(filename: string): any;
    /**
     * Gets list of files matching a pattern within a parent directory.
     * @param {string} parentDir - Parent directory to search within.
     * @param {string} searchPattern - Optional wildcard search pattern to filter on.
     * @returns {string[]} array of filenames matching privided  searchPattern
     * @memberof File
     * @instance
     * @example
     * var results = exoskeleton.file.getFiles("c:\\downloads\\music", "*.mp3");
     */
    getFiles(parentDir: string, searchPattern: string): string[];
    /**
     * Gets list of files ending in any of the extension/suffix strings provided.
     * @param {string} parentDir - Parent directory to search within.
     * @param {Array|string} extensions - Array or comma-delimited list of extension strings (not wildcards).
     * @returns {string[]} array of filenames matching the provided search extensions.
     * @memberof File
     * @instance
     * @example
     * var results = exoskeleton.file.getFilesEndingWith("c:\\downloads\\music", ".mp3,.m4a");
     */
    getFilesEndingWith(parentDir: string, extensions: string | string[]): string[];
    /**
     * Returns the filename portion of the path without the directory.
     * @param {string} path - The full pathname to get filename portion of.
     * @memberof File
     * @instance
     * @returns {string} the filename portion of the path provided.
     * @example
     * console.log(exoskeleton.file.getFileName("c:\\downloads\\myfile.txt"));
     * // logs : "myfile.txt"
     */
    getFileName(path: string): string;
    /**
     * Resolves an absolute path from a path containing inner relative paths
     * @param {string} path A path containing possible inner relative paths
     * @returns {string} The computed absolute path
     * @example
     * console.log(exoskeleton.file.getFullPath("d:\archives\january\..\march\test.txt");
     * // logs : "d:\archives\march\test.txt"
     */
    getFullPath(path: string): string;
    /**
     * Gets a list of logical drives.
     * @memberof File
     * @instance
     * @returns {string[]} array of drive names
     * @example
     * var result = exoskeleton.file.getLogicalDrives();
     * console.log(result);
     * // logs : C:\,D:\,Z:\
     */
    getLogicalDrives(): string[];
    /**
     * Opens a text file, reads all lines of the file and returns text as a string.
     * @param {string} filename - The file to read from.
     * @returns {string} - file contents as string.
     * @memberof File
     * @instance
     */
    loadFile(filename: string): string;
    /**
     * Reads a binary file into a Base64 string.  Can be used for encoding data urls.
     * @param {string} filename - The file to read from.
     * @returns {string} Base64 encoded binary content.
     * @memberof File
     * @instance
     * @example
     * var img1 = document.getElementById("img1");
     * var pic1 = exoskeleton.file.loadFileBase64("c:\\images\\pic1.png");
     * img1.src = 'data:image/png;base64,' + pic1;
     */
    loadFileBase64(filename: string): string;
    /**
     * Writes a text file with the provided contents string.
     * If the file already exists, it will be overwritten.
     * @param {string} filename - Filename to write to.
     * @param {string} contents - Contents to write into file.
     * @memberof File
     * @instance
     */
    saveFile(filename: string, contents: string): void;
    /**
     * Writes a binary file with bytes derived from the provided base64 string.
     * @param {string} filename - Filename to write to.
     * @param {string} contents - Base64 encoded binary content.
     * @memberof File
     * @instance
     * @example
     * var canvas = document.getElementById('canvas');
     * var b64Text = canvas.toDataURL();
     * b64Text = b64Text.replace('data:;image/png;base64,','');
     * exoskeleton.file.saveFileBase64('canvas1.png', b64Text);
     */
    saveFileBase64(filename: string, contents: string): void;
    /**
     * Starts our container singleton watcher on path specified by user.
     * Events will be emitted as multicast, so if multiple forms load
     * multiple watchers, they should distinguish themselves with the
     * eventBaseName parameter.
     * @param {string} path - Path to 'watch'.
     * @param {string} eventBaseName - Optional base name to emit with.
     * @memberof File
     * @instance
     * @example
     * // The following will generate 'download.created', 'download.deleted', and 'download.changed' events.
     * exoskeleton.file.startWatcher("C:\downloads", "download");
     * exoskeleton.events.on("multicast.download.created", function(data) {
     *   // data params is json encoded string (for now).
     *   exoskeleton.logger.logInfo("multicast.download.created event received", data);
     * });
     */
    startWatcher(path: string, eventBaseName: string): void;
    /**
     * Disables the watcher singleton.
     * @memberof File
     * @instance
     * @example
     * exoskeleton.file.stopWatcher();
     */
    stopWatcher(): void;
}
/**
 * Main API class used for general MessageBox, FileDialog, Notifications, and Container utilitites.
 */
declare class ExoMain {
    exoMain: any;
    constructor(exoMain: any);
    /**
     * Allows updating properties of the host window (.Net) Form object
     * See: {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.form(v=vs.110).aspx ms docs}
     * @param {object} formProperties - object containing properties to apply to host window's Form
     * @memberof Main
     * @instance
     * @example
     * exoskeleton.main.applyFormProperties({
     *   WindowState: "Maximized",
     *   Opacity: 0.9
     * });
     */
    applyFormProperties(formProperties: any): void;
    /**
     * Process all Windows messages currently in the message queue.
     * @memberof Main
     * @instance
     * @example
     * exoskeleton.main.doEvents();
     */
    doEvents(): void;
    /**
     * Signals the host container to exit fullscreen mode.
     * @memberof Main
     * @instance
     * @example
     * exoskeleton.main.exitFullscreen();
     */
    exitFullscreen(): void;
    /**
     * Signals the host container to enter fullscreen mode.
     * @memberof Main
     * @instance
     * @example
     * exoskeleton.main.fullscreen();
     */
    fullscreen(): void;
    /**
     * Opens a new host container with the url and settings provided.
     * @param {string} caption - Window caption to apply to new window.
     * @param {string} url - Url to load within the new window.
     * @param {int=} width - Width (in pixels) to size new window to.
     * @param {int=} height - Height (in pixels) to size new window to.
     * @param {string=} mode - Optionally override default UI mode to 'native' or 'web'
     * @memberof Main
     * @instance
     * @example
     * // relative pathname
     * exoskeleton.main.openNewWindow("My App Settings", "settings.htm", 800, 480);
     * // external internet address
     * exoskeleton.main.openNewWindow("My App Settings", "https://en.wikipedia.org/wiki/Main_Page", 800, 480);
     * // You might also pass raw html, prefixed with '@' symbol.
     * var html = "@<html><body><h3>Hello World</h3></body></html>";
     * exoskeleton.main.openNewWindow("My dynamic page", html, 400, 400);
     */
    openNewWindow(caption: string, url: string, width?: number | null, height?: number | null, mode?: string | null): void;
    /**
     * (Advanced) method to dynamically switch into native ui mode.
     * By default exoskeleton uses web ui mode within its hosted webbrowser.
     * That can be overridden in settings to Native UI mode as 'default' for
     * all opened windows.  That can be overridden in calls to openNewWindow.
     * Finally, even after host form is loaded, this method along with
     * switchToWebUi() can be used to toggle back and forth dynamically.
     * @memberof Main
     * @instance
     * @example
     * exoskeleton.main.switchToNativeUi();
     */
    switchToNativeUi(): void;
    /**
     * (Advanced) method to dynamically switch into mixed ui mode.
     * MixedUi mode uses native ui layout but leaves the webbrowser visible.
     * The webbrowser will be relocated to the panel named by the parameter.
     * The web browser will will be assigned a DockStyle of 'Fill'.
     * MixedUi mode is a 'late' mode, requiring you to have already
     * applied your form layout/definition and established the panel to be
     * used for locating the browser to.
     * controls
     * @param {string=} browserParentPanel - optional name of panel to locate and show browser within.
     * @memberof Main
     * @instance
     * @example
     * // Typically, to use mixed ui mode, you would :
     * // - use the 'DefaultToNativeUi' setting in your xos file,
     * // - initialize and apply your form definition (containing the panel named below), and finally
     * // - switch into mixedui (as below) once your form layout is complete.
     * exoskeleton.main.switchToMixedUi("BottomRightPanel");
     */
    switchToMixedUi(browserParentPanel?: string | null): void;
    /**
     * (Advanced) method to dynamically switch into web ui mode.
     * By default exoskeleton uses web ui mode within its hosted webbrowser.
     * That can be overridden in settings to Native UI mode as 'default' for
     * all opened windows.  That can be overridden in calls to openNewWindow.
     * Finally, even after host form is loaded, this method along with
     * switchToNativeUi() can be used to toggle back and forth dynamically.
     * @memberof Main
     * @instance
     * @example
     * exoskeleton.main.switchToWebUi();
     */
    switchToWebUi(): void;
    /**
     * Updates the window title for the host container.
     * @param {string} title - Text to apply to window title.
     * @memberof Main
     * @instance
     * @example
     * exoskeleton.main.setWindowTitle("My Editor App - " + filename);
     */
    setWindowTitle(title: string): void;
    /**
     * Displays a windows system tray notification.
     * @param {string} title - The notification title.
     * @param {string} message - The notification message.
     * @memberof Main
     * @instance
     * @example
     * exoskeleton.main.showNotification("my exo app", "some notification details");
     */
    showNotification(title: string, message: string): void;
    /**
     * Signals the host container to toggle fullscreen mode.
     * @memberof Main
     * @instance
     * @example
     * exoskeleton.main.toggleFullscreen();
     */
    toggleFullscreen(): void;
}
/**
 * Proc API class for performing windows process related tasks.
 */
declare class ExoProc {
    exoProc: any;
    constructor(exoProc: any);
    /**
     * Starts a process resource by specifying the name of a document or application file.
     * @param {string} procPath - Program to execute.
     * @returns {object} an object containing information about the newly created process.
     * @memberof Proc
     * @instance
     * @example
     * exoskeleton.proc.startPath("calc.exe");
     * exoskeleton.proc.startPath("c:\\windows\\system32\\notepad.exe");
     */
    startPath(procPath: string): any;
    /**
     * Starts a process resource by providing information in a ProcessStartInfo format.
     * @param {object} processStartInfo - Serialized javascript object closely resembling a c# ProcessStartInfo object.
     * @param {string=} processStartInfo.FileName - filename of program to load.
     * @param {string=} processStartInfo.Arguments - string containing arguments to launch with.
     * @param {boolean=} processStartInfo.CreateNoWindow - whether to start the process in a new window.
     * @param {boolean=} processStartInfo.ErrorDialog - whether to show error dialog if process could not be started.
     * @param {boolean=} processStartInfo.LoadUserProfile - whether the windows user profile is to be loaded from the registry.
     * @param {boolean=} processStartInfo.UseShellExecute - whether to use the operating system shell to start the process.
     * @param {boolean=} processStartInfo.WorkingDirectory - working dir of proc when UseShellExecute is false. dir containing process when UseShellExecute is true.
     * @returns {object} an object containing information about the newly created process.
     * @memberof Proc
     * @instance
     * @example
     * exoskeleton.proc.start({
     *   FileName: "notepad.exe",
     *   Arguments: "c:\\docs\\readme.txt"
     * });
     */
    start(processStartInfo: any): any;
    /**
     * Gets a detailed list of running processes and their settings. (Takes a long time to run)
     * @memberof Proc
     * @instance
     * @returns {object[]} array of deserialized c# Process objects
     * @example
     * var procList = exoskeleton.proc.getProcesses();
     * procList.forEach(function(p: any) {
     *   // properties available should correlate to .net Process class member properties.
     *   console.log(p.ProcessName + " : " + p.Id);
     * });
     */
    getProcesses(): any[];
    /**
     * Gets a simplified list of running processes.
     * @memberof Proc
     * @instance
     * @returns {object[]} array of deserialized c# Process objects
     * @example
     * var procList = exoskeleton.proc.getProcessesSimplified();
     * procList.forEach(function(p) {
     *   // properties available are 'Id', 'ProcessName' and 'MainWindowTitle'.
     *   console.log(p.ProcessName + " : " + p.Id);
     * });
     */
    getProcessesSimplified(): any;
    /**
     * Gets detailed process information by process id.
     * @param {int} id - the windows process id to get more info about.
     * @returns {object} a json object representation of a .net Process object
     * @memberof Proc
     * @instance
     */
    getProcessInfoById(id: number): any;
    /**
     * Gets a list of processes of the provided name.
     * @param {string} name - name of process to get list of.
     * @returns {object[]} array of deserialized c# Process objects
     * @memberof Proc
     * @instance
     * @example
     * var procList = exoskeleton.proc.getProcessesByName("notepad");
     * procList.forEach(function(p) {
     *   console.log(p.ProcessName + " : " + p.Id);
     * });
     */
    getProcessesByName(name: string): any[];
    /**
     * Kills a running process.
     * @param {int} id - The id of the process to kill.
     * @returns {boolean} whether process was found or not.
     * @memberof Proc
     * @instance
     * @example
     * // The id passed can be looked up via calls to getProcesses or getProcessesByName.
     * exoskeleton.proc.killProcessById(1608);
     */
    killProcessById(id: number): boolean;
}
/**
 * Session API class for interfacing with the exoskeleton 'session' key/value storage.
 */
declare class ExoSession {
    exoSession: any;
    constructor(exoSession: any);
    /**
     * Looks up the (string) Value for the Session key provided.
     * @param {string} key - The key name to lookup a value for in the session store.
     * @returns {string} - The value associated with key in string form.
     * @memberof Session
     * @instance
     * @example
     * var result = exoskeleton.session.get("username");
     */
    get(key: string): string;
    /**
     * Looks up the (object) Value for the Session key provided.
     * @param {string} key - The key name to lookup a value for in the session store.
     * @returns {object} - The value associated with key parsed into object.
     * @memberof Session
     * @instance
     * @example
     * var userInfo = exoskeleton.session.getObject("UserInfo");
     * console.log(userInfo.name + userInfo.addr + userInfo.phone);
     */
    getObject(key: string): any;
    /**
     * Obtains a string list of all keys currently in the session store.
     * @returns {string[]} - An array of string 'keys' within the session store.
     * @memberof Session
     * @instance
     * @example
     * var result = exoskeleton.session.list();
     * result.forEach(function(keyname) {
     *   console.log(exoskeleton.session.get(keyname));
     * });
     */
    list(): string[];
    /**
     * Assigns a key/value setting within the session store.
     * @param {string} key - The name of the session variable to set.
     * @param {string} value - The string value to assign to session variable.
     * @memberof Session
     * @instance
     * @example
     * exoskeleton.session.set("username", "jdoe");
     */
    set(key: string, value: string): void;
    /**
     * Assigns a key/value setting within the session store by serializing it.
     * @param {string} key - The name of the session variable to set.
     * @param {object} value - The object value to assign to session variable.
     * @memberof Session
     * @instance
     * @example
     * exoskeleton.session.setObject("UserInfo", {
     *   name: "jdoe",
     *   addr: "123 anystreet",
     *   phone: "555-1212"
     * });
     */
    setObject(key: string, value: any): void;
}
/**
 * System API class for getting system information, environment variables, registry, etc.
 */
declare class ExoSystem {
    exoSystem: any;
    constructor(exoSystem: any);
    /**
     * Get information about the system which this program is being run on.
     * @returns {object} - Json system information object.
     * @memberof System
     * @instance
     * @example
     * var si = exoskeleton.system.getSystemInfo();
     * console.dir(si);
     * console.log(si.MachineName);
     */
    getSystemInfo(): any;
    /**
     * Retrieves a single environment variable value.
     * @param {string} varName - The name of the environment variable to retrieve value for.
     * @returns {string=} The string value of the environment variable (if found).
     * @memberof System
     * @instance
     * @example
     * var path = exoskeleton.system.getEnvironmentVariable("PATH");
     */
    getEnvironmentVariable(varName: string): string | null;
    /**
     * Returns a list of all environment variables as properties and property values.
     * @returns {object} - Json hash object with properties representing variables.
     * @memberof System
     * @instance
     * @example
     * var envVariables = exoskeleton.system.getEnvironmentVariables();
     * Object.keys(envVariables, function(key) {
     *   console.log("key: " + key + " val : " + exoskeleton.system.getEnvironmentVariable(key));
     * });
     */
    getEnvironmentVariables(): any;
    /**
     * Sets an environment variable only within this process or child processes.
     * @param {string} varName - The name of the environment variable.
     * @param {string} varValue - The value to assign to the environment variable.
     * @memberof System
     * @instance
     * @example
     * var now = new DateTime();
     * var path = exoskeleton.system.setEnvironmentVariable("LAUNCHTIME", now.toString());
     */
    setEnvironmentVariable(varName: string, varValue: string): void;
    /**
     * Finds an existing application window by either class or name and focuses it.
     * @param {string=} className - The class name of the window, or null.
     * @param {string=} windowName - The window name of the window, or null.
     * @memberof System
     * @instance
     * @example
     * // assuming notepad is already running :
     * exoskeleton.system.focusWindow("notepad", null);
     * exoskeleton.system.focusWindow(null, "readme.txt - Notepad");
     */
    focusWindow(className?: string | null, windowName?: string | null): void;
    /**
     * Finds a window and sends keycodes to it.
     * This uses .Net SendKeys(), for info on key codes read :
     * https://msdn.microsoft.com/en-us/library/system.windows.forms.sendkeys(v=vs.110).aspx
     * @param {string=} className - The class name of the window, or null.
     * @param {string=} windowName - The window name of the window, or null.
     * @param {string[]} keys - String array of keys or keycodes to send.
     * @returns {boolean} - Whether the window was found.
     * @memberof System
     * @instance
     * @example
     * exoskeleton.system.focusAndSendKeys("notepad", null, ["t", "e", "s", "t", "{ENTER}"]);
     */
    focusAndSendKeys(className?: string | null, windowName?: string | null, keys?: string[]): boolean;
}
/**
 * Logger API class for dealing with exoskeleton logger.
 */
declare class ExoLogger {
    exoLogger: any;
    constructor(exoLogger: any);
    /**
     * Logs an "info" message to the logger's message list.
     * @param {string} source - Descriptive 'source' of the info.
     * @param {string} message - Info detail message
     * @memberof Logger
     * @instance
     * @example
     * exoskeleton.logger.logInfo("myfunction", "something interesting happened");
     */
    logInfo(source: string, message: string): void;
    /**
     * Logs a "warning" message to the logger's message list.
     * @param {string} source - Descriptive 'source' of the warning.
     * @param {string} message - Detailed warning message.
     * @memberof Logger
     * @instance
     * @example
     * exoskeleton.logger.logInfo("myfunction", "something odd happened");
     */
    logWarning(source: string, message: string): void;
    /**
     * Logs an "error" message to the logger's message list.
     * @param {string} msg - Message to log.
     * @param {string=} url - The url of the script where the error occurred.
     * @param {string=} line - Line number of the javascript where the error occurred.
     * @param {string=} col - Column number of the javascript where the error occurred.
     * @param {string=} error - Detailed informatino about the error.
     * @memberof Logger
     * @instance
     * @example
     * exoskeleton.logger.logError("something dangerous happened", "myfunction");
     */
    logError(msg: string, url?: string | null, line?: string | null, col?: string | null, error?: string | null): void;
    /**
     * Logs text to the logger's console.
     * @param {string} message - Text to append to the console.
     * @memberof Logger
     * @instance
     * @example
     * var now = new DateTime();
     * exoskeleton.logger.logText("started processing at : " + now);
     */
    logText(message: string): void;
    /**
     * Logs an object to the logger's console.
     * @param {object} obj - Object to serialize and pretty print to console.
     * @memberof Logger
     * @instance
     * @example
     * var obj = new { a: 1, b: 2 }
     * exoskeleton.logger.logObject(obj);
     */
    logObject(obj: any): void;
}
/**
 * Net API class for various network and http tasks.
 */
declare class ExoNet {
    exoNet: any;
    constructor(exoNet: any);
    /**
     * Downloads from an internet url and saves to disk.
     * @param {string} url - The internet url to download from.
     * @param {string} dest - Destination filename on disk.
     * @param {boolean} async - Whether to wait until finished before returning.
     * @memberof Net
     * @instance
     * @example
     * exoskeleton.net.downloadFile("https://github.com/obeliskos/exoskeleton/archive/0.2.zip",
     *   "c:\\downloads\\0.2.zip", false);
     */
    downloadFile(url: string, dest: string, async: boolean): void;
    /**
     * Fetches text-based resource at the provided url and returns a string of its content.
     * @param {string} url - Internet url of text based resource.
     * @returns {string} - String containing text within the retrieved resource.
     * @memberof Net
     * @instance
     * @example
     * var readmeText = exoskeleton.net.readUrl("https://raw.githubusercontent.com/obeliskos/exoskeleton/master/README.md");
     * exoskeleton.logger.logText(readmeText);
     */
    readUrl(url: string): string;
}
/**
 * Enc API class for performing various encryption and hashing tasks.
 */
declare class ExoEnc {
    exoEnc: any;
    constructor(exoEnc: any);
    /**
     * Encrypts a string using the provided password.
     * @param {string} data - The string to encrypt.
     * @param {string} password - The password to encrypt with.
     * @returns {string} encrypted string result
     * @memberof Enc
     * @instance
     * @example
     * var encryptedString = exoskeleton.enc.encrypt("some secret msg", "s0m3p4ssw0rd");
     */
    encrypt(data: string, password: string): string;
    /**
     * Decrypts a string using the provided password.
     * @param {string} data - The string to decrypt.
     * @param {string} password - The password to decrypt with.
     * @returns {string} decrypted string result
     * @memberof Enc
     * @instance
     * @example
     * var originalString = "some secret msg";
     * var encryptedString = exoskeleton.enc.encrypt(originalString, "s0m3p4ssw0rd");
     * var decryptedString = exoskeleton.enc.decrypt(encryptedString, "s0m3p4ssw0rd");
     * console.log("original: " + originalString);
     * console.log("encrypted: " + encryptedString);
     * console.log("decrypted: " + decryptedString);
     */
    decrypt(data: string, password: string): string;
    /**
     * Create encrypted copies of the specified file(s) using the provided password.
     * @param {string=} directory - Directory where file(s) to be encrypted reside (or current directory if null).
     * @param {string} filemask - filename or wildcard pattern of files to encrypt.
     * @param {string} password - The password to encrypt with.
     * @memberof Enc
     * @instance
     * @example
     * // creates encrypted file(s) with '.enx' suffix
     * exoskeleton.enc.encryptFiles("c:\\source", "readme.txt", "s0m3p4ssw0rd");
     * exoskeleton.enc.encryptFiles("c:\\source", "*.doc", "s0m3p4ssw0rd");
     */
    encryptFiles(directory: string | null, filemask: string, password: string): void;
    /**
     * Create decrypted copies of the specified file(s) using the provided password.
     * @param {string=} directory - Directory where file(s) to be decrypted reside (or current directory if null).
     * @param {string} filemask - filename or wildcard pattern of files to decrypt.
     * @param {string} password - The password to decrypt with.
     * @memberof Enc
     * @instance
     * @example
     * // creates decrypted file(s) without the '.enx' suffix
     * exoskeleton.enc.decryptFiles("c:\\source", "readme.txt.enx", "s0m3p4ssw0rd");
     * exoskeleton.enc.decryptFiles("c:\\source", "*.doc.enx", "s0m3p4ssw0rd");
     */
    decryptFiles(directory: string | null, filemask: string, password: string): void;
    /**
     * Creates ana MD5 Hash for the file specified by the provided filename.
     * @param {string} filename - The filename of the file to calculate a hash file.
     * @returns {string} the hex string encoded md5 hash
     * @memberof Enc
     * @instance
     * @example
     * var hash = exoskeleton.enc.createMD5Hash("c:\\source\\readme.txt");
     */
    createMD5Hash(filename: string): string;
    /**
     * Creates ana SH1 Hash for the file specified by the provided filename.
     * @param {string} filename : The filename of the file to calculate a hash file.
     * @returns {string} the hex string encoded sha1 hash
     * @memberof Enc
     * @instance
     * @example
     * var hash = exoskeleton.enc.createSHA1Hash("c:\\source\\readme.txt");
     */
    createSHA1Hash(filename: string): string;
    /**
     * Creates ana SHA256 Hash for the file specified by the provided filename.
     * @param {string} filename - The filename of the file to calculate a hash file.
     * @returns {string} the hex string encoded sha256 hash
     * @memberof Enc
     * @instance
     * @example
     * var hash = exoskeleton.enc.createSHA256Hash("c:\\source\\readme.txt");
     */
    createSHA256Hash(filename: string): string;
    /**
     * Creates MD5, SHA1, and SHA256 hashes for the file(s) specified.
     * @param {string} path - Directory to look in for files to hash.
     * @param {string} searchPattern - Filename or wildcard of file(s) to hash.
     * @returns {object[]} array of custom objects containing hash info
     * @memberof Enc
     * @instance
     * @example
     * var detailedHashInfo = exoskeleton.enc.hashFiles("c:\\source", "*.doc");
     * console.log(detailedHashInfo.length);
     * console.log(detailedHashInfo[0].sha256);
     */
    hashFiles(path: string, searchPattern: string): any[];
}
/**
 * Menu API class used for populating the host container's menu bar.
 */
declare class ExoMenu {
    exoMenu: any;
    constructor(exoMenu: any);
    /**
     * Enables visibility of the window's menu
     * @memberof Menu
     * @instance
     * @example
     * exoskeleton.menu.show();
     */
    show(): void;
    /**
     * Hides visibility of the window's menu
     * @memberof Menu
     * @instance
     * @example
     * exoskeleton.menu.hide();
     */
    hide(): void;
    /**
     * Removes all menu items for reinitialization.  Host window survives across inner page
     * (re)loads or redirects so menus would need to be (re)initialized on page loads.
     * @memberof Menu
     * @instance
     * @example
     * exoskeleton.menu.initialize();
     */
    initialize(): void;
    /**
     * Adds a top level menu
     * @param {string} menuName - Text to display on menu
     * @param {string=} emitEventName - The local event name to unicast when the menu is clicked.
     * @memberof Menu
     * @instance
     * @example
     * exoskeleton.menu.addMenu("File");
     * exoskeleton.menu.addMenu("About", "AboutClicked");
     */
    addMenu(menuName: string, emitEventName?: string | null): void;
    /**
     * Adds menu subitems to an existing menu or submenu.
     * Any event emitted on click will be passed the menu item text as parameter.
     * @param {string} menuName - The text of the parent menu or submenu to add subitem to
     * @param {string} menuItemName - The text of the new subitem to add
     * @param {string=} emitEventName - The optional (local) event name to unicast when the menu is clicked.
     * @param {string[]=} shortcutKeys - Optional array of shortcut key codes.
     * @memberof Menu
     * @instance
     * @example
     * exoskeleton.menu.initialize();
     * exoskeleton.menu.addMenu("File");
     * exoskeleton.menu.addMenuItem("File", "Open", "FileOpenEvent", ["Control", "Shift", "O"]);
     * exoskeleton.menu.addMenuItem("File", "New");
     * // for this example we will use the same common event name for submenu items
     * // these can also be different
     * exoskeleton.menu.addMenuItem("New", ".txt file", "FileNewEvent");
     * exoskeleton.menu.addMenuItem("New", ".png file", "FileNewEvent");
     * exoskeleton.on("FileOpenEvent", function() {
     *   alert('File/Open clicked');
     * });
     * exoskeleton.on("FileNewEvent", function(data) {
     *   alert('File/New/' + data + ' clicked');
     * });
     */
    addMenuItem(menuName: string, menuItemName: string, emitEventName?: string | null, shortcutKeys?: string | string[] | null): void;
}
/**
 * Toolbar API class used for populating the host container's tool strip.
 */
declare class ExoToolbar {
    exoToolbar: any;
    constructor(exoToolbar: any);
    /**
     * Enables visibility of the window's toolbar.
     * @memberof Toolbar
     * @instance
     * @example
     * exoskeleton.toolbar.show();
     */
    show(): void;
    /**
     * Hides visibility of the window's toolbar.
     * @memberof Toolbar
     * @instance
     * @example
     * exoskeleton.toolbar.hide();
     */
    hide(): void;
    /**
     * Empties the host window toolstrip of all controls
     * @memberof Toolbar
     * @instance
     * @example
     * exoskeleton.toolbar.initialize();
     */
    initialize(): void;
    /**
     * Adds a ToolStripButton to the host window toolstrip
     * @param {string} text - Text to display on the tooltip
     * @param {string} eventName - Name of the local event to raise when clicked
     * @param {string} imagePath - Filepath to the (roughly 32x32 px) image to display on the button
     * @memberof Toolbar
     * @instance
     * @example
     * exoskeleton.toolbar.initializeMenu();
     * exoskeleton.toolbar.addButton("Create new document", "NewDocEvent", "c:\\images\\new.png");
     * exoskeleton.toolbar.addButton("Exit", "ExitEvent", "c:\\images\\exit.png");
     * exoskeleton.events.on("NewDocEvent", function() {
     *   showCustomFileOpenDialog();
     * });
     * exoskeleton.events.on("ExitEvent", function() {
     *   exoskeleton.shutdown();
     * });
     */
    addButton(text: string, eventName: string, imagePath: string): void;
    /**
     * Adds a visual separator for toolstrip control groups
     * @memberof Toolbar
     * @instance
     * @example
     * exoskeleton.toolbar.addSeparator();
     */
    addSeparator(): void;
}
/**
 * Statusbar API class used for manipulating the host container's status strip.
 */
declare class ExoStatusbar {
    exoStatusbar: any;
    constructor(exoStatusbar: any);
    /**
     * Enables visibility of the window's status bar.
     * @memberof Statusbar
     * @instance
     * @example
     * exoskeleton.statusbar.show();
     */
    show(): void;
    /**
     * Hides visibility of the window's status bar.
     * @memberof Statusbar
     * @instance
     * @example
     * exoskeleton.statusbar.hide();
     */
    hide(): void;
    /**
     * Clears both left and right status labels
     * @memberof Statusbar
     * @instance
     * @example
     * exoskeleton.statusbar.initialize();
     */
    initialize(): void;
    /**
     * Sets the text to be displayed in the left status label
     * @param {string} text - text to display in left status label
     * @memberof Statusbar
     * @instance
     * @example
     * exoskeleton.statusbar.setLeftLabel("Welcome to my app");
     */
    setLeftLabel(text: string): void;
    /**
     * Sets the text to be displayed in the right status label
     * @param {string} text - text to display in right status label
     * @memberof Statusbar
     * @instance
     * @example
     * exoskeleton.statusbar.setRightLabel("Started : " + new Date());
     */
    setRightLabel(text: string): void;
}
/**
 * Util API class containing misc utility methods.
 */
declare class ExoUtil {
    exoUtil: any;
    constructor(exoUtil: any);
    /**
     * Converts a .NET date to unix format for use with javascript.
     * @param {string} dateString - String representation of a (serialized) .net DateTime object
     * @returns {int} - Number of millseconds since 1/1/1970
     * @memberof Util
     * @instance
     * @example
     * // look up some directory info
     * var dirinfo = exoskeleton.file.getDirectoryInfo("c:\\myfolder");
     * // convert its last write time to unix (number of ms since 1/1/1970)
     * var unixTime = exoskeleton.main.convertDateToUnix(dirinfo.LastWriteTimeUtc);
     * // create a javascript date from unix format
     * var dt = new Date(unixTime);
     */
    convertDateToUnix(dateString: string): number;
    /**
     * Converts a javascript unix epoch time to a .net formatted date.
     * See {@link https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings ms docs}
     * @param {number|Date} date - javascript date object or unix epoch time
     * @param {string} format - a .net ToString() format string to apply to date
     * @returns {string} The date formatted to your provided string format
     * @memberof Util
     * @instance
     * @example
     * var now = new Date();
     * // 24 hr date and time
     * var result = exoskeleton.util.formatUnixDate(now, "MM/dd/yy H:mm:ss");
     * alert(result);
     * // formatted date only
     * result = exoskeleton.util.formatUnixDate(now.getTime(), "MMMM dd, yyyy");
     * alert(result);
     * // formatted time only
     * result = exoskeleton.util.formatUnixDate(now, "hh:mm:ss tt");
     */
    formatUnixDate(date: number | Date, format: string): string;
}
/**
    * Dialog API class for creating and interfacing with WinForms dialogs.
    * This API exposes native .NET dialogs, several exoskeleton 'prompt' dialogs or you can compose your own using a global dialog singleton.
    * Dialogs are modal and executed synchronously, so your javascript will wait until dialog is dismissed before
    * continuing or receiving its returned result.  Dialogs do not support javascript eventing.
    */
declare class ExoDialog {
    exoDialog: any;
    constructor(exoDialog: any);
    /**
     * Adds a CheckBox to a named exoskeleton dialog.
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.checkbox(v=vs.110).aspx ms docs}
     * @param {string} dialogName - unique name to dialog/form to refer to your dialog.
     * @param {object|string} checkbox - initial properties to assign to checkbox
     * @param {string=} parentName - optional name of parent control to nest within
     * @memberof Dialog
     * @instance
     * @example
     * exoskeleton.dialog.addCheckbox("ExampleDialog", {
     *   Name: "StudentCheckbox",
     *   Text: "Student",
     *   Checked: true,
     *   Top: 100,
     *   Left: 10
     * }, "TopPanel");
     */
    addCheckBox(dialogName: string, checkbox: any, parentName?: string | null): void;
    /**
     * Adds a CheckedListBox to a named exoskeleton dialog.
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.checkedlistbox(v=vs.110).aspx ms docs}
     * @param {string} dialogName - unique name to dialog/form to refer to your dialog.
     * @param {object|string} listbox - initial properties to assign to ListBox
     * @param {string=} parentName - optional name of parent control to nest within
     * @param {object=} payload - for checkedlistbox, this allows specifying 'CheckedIndices'
     * @memberof Dialog
     * @instance
     * @example
     * exoskeleton.dialog.addCheckedListBox("ExampleDialog", {
     *   Name: "CountryChecklist",
     *   Top: 10,
     *   Left: 10,
     *   Dock: 'Fill'
     *   Items: ['United States', 'United Kingdom', 'Germany', 'France', 'Japan']
     * }, "AddressPanel", { CheckedIndices: [0, 2] });
     */
    addCheckedListBox(dialogName: string, checkedlistbox: any, parentName?: string | null, payload?: any): void;
    /**
     * Adds a ComboBox to a named exoskeleton dialog.
     * If the combo box items will remain static, you can set items with 'Items' property.
     * If the items are to be dynamic, you should set 'DisplayMember' and 'ValueMember'
     * properties and databind by separate applyControlPayload object with 'DataSource' object array.
     * Passing an additional 'DataSourceKeepSelection' payload property will attempt to retain the
     * currently selected item across the rebinding.
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.combobox(v=vs.110).aspx ms docs}
     * @param {string} dialogName - unique name to dialog/form to refer to your dialog.
     * @param {object|string} comboBox - initial properties to assign to ComboBox
     * @param {string=} parentName - optional name of parent control to nest within
     * @param {string=} payload - can be used to pass 'DataSource' and 'DataSourceKeepSelection' properties
     * @memberof Dialog
     * @instance
     * @example
     * exoskeleton.dialog.addComboBox("ExampleDialog", {
     *   Name: "CountryDropDown",
     *   Top: 10,
     *   Left: 10,
     *   Items: ['United States', 'United Kingdom', 'Germany', 'France', 'Japan'],
     *   SelectedItem : 'United States'
     * }, "AddressPanel");
     */
    addComboBox(dialogName: string, comboBox: any, parentName?: string | null, payload?: any): void;
    /**
     * Adds a DataGridView to a named exoskeleton dialog.
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.datagridview(v=vs.110).aspx ms docs}
     * @param {string} dialogName - unique name to dialog/form to refer to your dialog.
     * @param {object} gridView - initial properties to assign to DataGridView
     * @param {object[]} objectArray - array of 'similar' objects to display in grid view
     * @param {string=} parentName - optional name of parent control to nest within
     * @param {object=} payload - for checkedlistbox, this allows specifying 'CheckedIndices'
     * @memberof Dialog
     * @instance
     * @example
     * var users = [
     *   { name: "john", age: 24, address: "123 alpha street" },
     *   { name: "mary", age: 22, address: "222 gamma street" },
     *   { name: "tom", age: 28, address: "587 delta street" },
     *   { name: "jane", age: 26, address: "428 beta street" }
     * ];
     *
     * var result = exoskeleton.dialog.addDataGridView("ExampleDialog", {
     *   Name: "UserGridView",
     *   Dock: 'Fill',
     *   ReadOnly: true,
     *   AllowUserToAddRows: false,
     *   SelectionMode: 'FullRowSelect'
     * }, "DetailsPanel", { ObjectArray: users });
     */
    addDataGridView(dialogName: string, gridView: any, parentName?: string | null, payload?: any): void;
    /**
     * Adds a DateTimePicker to a named exoskeleton dialog.
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.datetimepicker(v=vs.110).aspx ms docs}
     * @param {string} dialogName - unique name to dialog/form to refer to your dialog.
     * @param {object} dateTimePicker - initial properties to assign to DateTimePicker
     * @param {string=} parentName - optional name of parent control to nest within
     * @memberof Dialog
     * @instance
     * @example
     * exoskeleton.dialog.addDateTimePicker("ExampleDialog", {
     *   Name: "StartDate",
     *   Top: 100,
     *   Left: 100,
     *   Value: "12/13/2014"
     * }, "AddressPanel");
     */
    addDateTimePicker(dialogName: any, dateTimePicker: any, parentName?: string | null): void;
    /**
     * Adds a 'dismiss dialog' Button to a named exoskeleton dialog.
     * This is a standard .net button with hardcoded event handler to dismiss dialog.
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.button(v=vs.110).aspx ms docs}
     * @param {string} dialogName - unique name to dialog/form to refer to your dialog.
     * @param {object|string} button - initial properties to assign to button
     * @param {string=} parentName - optional name of parent control to nest within
     * @param {object=} payload - for checkedlistbox, this allows specifying 'CheckedIndices'
     * @memberof Dialog
     * @instance
     * @example
     * exoskeleton.dialog.addDialogButton("ExampleDialog", {
     *   Text: "OK",
     *   Top: 10,
     *   Left: 200
     * }, "BottomPanel", { DialogResult: "OK" });
     */
    addDialogButton(dialogName: string, button: any, parentName?: string | null, payload?: any): void;
    /**
     * Adds a Label to a named exoskeleton dialog.
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.label(v=vs.110).aspx ms docs}
     * @param {string} dialogName - unique name to dialog/form to refer to your dialog.
     * @param {object|string} label - initial properties to assign to label
     * @param {string=} parentName - optional name of parent control to nest within
     * @memberof Dialog
     * @instance
     * @example
     * exoskeleton.dialog.addLabel("ExampleDialog", {
     *   Name: "AddressTextBox",
     *   Text: "Address:",
     *   Top: 100,
     *   Left: 10
     * }, "AddressPanel");
     */
    addLabel(dialogName: string, label: any, parentName?: string | null): void;
    /**
     * Adds a ListBox to a named exoskeleton dialog.
     * If the list box items will remain static, you can set items with 'Items' property.
     * If the items are to be dynamic, you should set 'DisplayMember' and 'ValueMember'
     * properties and databind by separate applyControlPayload object with 'DataSource' object array.
     * Passing an additional 'DataSourceKeepSelection' payload property will attempt to retain the
     * currently selected item across the rebinding.
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.listbox(v=vs.110).aspx ms docs}
     * @param {string} dialogName - unique name to dialog/form to refer to your dialog.
     * @param {object|string} listbox - initial properties to assign to ListBox
     * @param {string=} parentName - optional name of parent control to nest within
     * @param {string=} payload - can be used to pass 'DataSource' and 'DataSourceKeepSelection' properties
     * @memberof Dialog
     * @instance
     * @example
     * exoskeleton.dialog.addListBox("ExampleDialog", {
     *   Name: "CountryList",
     *   Top: 10,
     *   Left: 10,
     *   Dock: 'Fill'
     *   Items: ['United States', 'United Kingdom', 'Germany', 'France', 'Japan'],
     *   SelectedItem : 'United States'
     * }, "AddressPanel");
     */
    addListBox(dialogName: string, listbox: any, parentName?: string | null, payload?: any): void;
    /**
     * Adds a ListView to a named exoskeleton dialog.
     * ListView has the following view modes (defined by 'View' property) :
     * - LargeIcon: 'Thumbnail-like' list using 'Items' payload property (no subitems).
     * - Details - Tabular list using 'ItemArrays' 2-dimensional payload property for subitems.
     * - SmallIcon - 'Left-to-Right, Top-to-Bottom' list needing only 'Items' payload property.
     * - List - 'Top-to-Bottom' list needing only 'Items' payload property.
     * - Tile - 'L->R, T->B' list with large icons, displaying subitems as multiple lines (2D 'ItemArrays')
     *
     * The various allowable payloads include :
     * - 'Columns' - In a tabular view mode these define the column captions/sizes ('Text', 'Width')
     * - 'Items' - Items array supports single dimension array where each array element is a different list item.
     * - 'ItemArrays' - ItemArrays support 2-dimension array allowing multiple columns for each 'row'.
     * - 'AppendItems' - AppendItems is the same as 'Items' but the list will not be cleared before adding those (new) items.
     * - 'AppendItemArrays' - AppendItemArrays will not clear the listview before appending the new items.
     *
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.listview(v=vs.110).aspx ms docs}.
     * @param {string} dialogName - unique name to dialog/form to refer to your dialog.
     * @param {object|string} listview - initial properties to assign to ListView
     * @param {string=} parentName - optional name of parent control to nest within
     * @param {string=} payload - can be used to pass 'Columns', 'Items' and/or 'ItemArrays' properties
     * @memberof Dialog
     * @instance
     * @example
     * // LargeIcon view mode - depends on already established named imagelist
     * var controlProperties = { Name: "CountryList", Top: 10, Left: 10, Dock: 'Fill', View: "LargeIcon" };
     * var controlPayload = {
     *   LargeImageList: "LargeListIcons",
     *   Items: [{ Text: "One", ImageIndex: 0 }, { Text: "Two", ImageIndex: 1 }]
     * };
     * exoskeleton.dialog.addListView("ExampleDialog", controlProperties, "AddressPanel", controlPayload);
     *
     * // Details view mode example
     * controlProperties = { Name: "CountryList", Top: 10, Left: 10, Dock: 'Fill', View: "Details", FullRowSelect: true }
     * var controlPayload = {
     *   SmallImageList: "SmallListIcons",
     *   Columns: [{ Text: "Country", Width:100 }, { Text:"Info1", Width: 120 }, { Text: "Info2", Width: 100 }],
     *   ItemArrays: [
     *     [{ Text: "Item One", ImageIndex: 0 }, { Text: "Stuff about One" }, { Text: "More stuff about One" }],
     *     [{ Text: "Item Two", ImageIndex: 1 }, { Text: "Stuff about Two" }, { Text: "More stuff about Two" }],
     *     [{ Text: "Item Three", ImageIndex: 2 }, { Text: "Stuff about Three" }, { Text: "More stuff about Three" }]
     *   ]
     * };
     * exoskeleton.dialog.addListView("ExampleDialog", controlProperties, "AddressPanel", controlPayload);
     */
    addListView(dialogName: string, listview: any, parentName?: string | null, payload?: any): void;
    /**
     * Adds a MaskedTextBox to a named exoskeleton dialog.
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.maskedtextbox(v=vs.110).aspx ms docs}
     * @param {string} dialogName - unique name to dialog/form to refer to your dialog.
     * @param {object} maskedtextbox - object containing properties to apply to the MaskedTextBox
     * @param {string=} parentName - optional name of parent control to nest within
     * @memberof Dialog
     * @instance
     * @example
     * exoskeleton.dialog.addMaskedTextBox("ExampleDialog", {
     *   Name: "PhoneNumberMaskedEdit",
     *   Mask: "(999)-000-0000",
     *   Top: 124,
     *   Left: 48,
     *   Width: 100
     * });
     */
    addMaskedTextBox(dialogName: string, maskedtextbox: any, parentName?: string | null): void;
    /**
     * Adds a MonthCalendar to a named exoskeleton dialog.
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.monthcalendar(v=vs.110).aspx ms docs}
     * @param {string} dialogName - unique name to dialog/form to refer to your dialog.
     * @param {object} monthcalendar - object containing properties to apply to the MonthCalendar
     * @param {string=} parentName - optional name of parent control to nest within
     * @memberof Dialog
     * @instance
     * @example
     * exoskeleton.dialog.addMonthCalendar("ExampleDialog", {
     *   Name: "MonthCalendar",
     *   ShowToday: true,
     *   ShowTodayCircle: true,
     *   MonthlyBoldedDates: ["1/1/2017", "1/15/2017"],  // bolds 1st and 15th days of every month,
     *   AnnuallyBoldedDates: ["3/20/2017", "6/1/2017", "9/22/2017", "12/22/2017"],
     *   Top: 40,
     *   Left: 40
     * });
     */
    addMonthCalendar(dialogName: string, monthcalendar: any, parentName?: string | null): void;
    /**
     * Adds a NumericUpDown control to a named exoskeleton dialog.
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.numericupdown(v=vs.110).aspx ms docs}
     * @param {string} dialogName - unique name to dialog/form to refer to your dialog.
     * @param {object|string} numericUpDown - initial properties to assign to ListBox
     * @param {string=} parentName - optional name of parent control to nest within
     * @memberof Dialog
     * @instance
     * @example
     * exoskeleton.dialog.addNumericUpDown("ExampleDialog", {
     *   Name: "AgeNumeric",
     *   Top: 10,
     *   Left: 10,
     *   Minimum: 13,
     *   Maximum: 120
     * }, "UserInfoPanel");
     */
    addNumericUpDown(dialogName: string, numericUpDown: any, parentName?: string | null): void;
    /**
     * Adds a Panel to a named exoskeleton dialog for layout and nesting purposes.
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.panel(v=vs.110).aspx ms docs}
     * @param {string} dialogName - unique name to dialog/form to refer to your dialog.
     * @param {object|string} panel - initial properties to assign to panel
     * @param {string=} parentName - optional name of parent control to nest within
     * @memberof Dialog
     * @instance
     * @example
     * exoskeleton.dialog.addPanel({
     *   Name: "FillPanel",
     *   Dock: 'Fill'
     * });
     * exoskeleton.dialog.addPanel({
     *   Name: "TopPanel",
     *   Dock: 'Top',
     *   Height: 100
     * });
     * exoskeleton.dialog.addPanel("ExampleDialog", {
     *   Name: "BottomPanel",
     *   Dock: 'Bottom',
     *   Height: 100
     * });
     */
    addPanel(dialogName: string, panel: any, parentName?: string | null): void;
    /**
     * Adds a PictureBox to a named exoskeleton form for layout and nesting purposes.
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.picturebox(v=vs.110).aspx ms docs}
     * @param {string} formName - unique name to dialog/form to refer to your dialog.
     * @param {object|string} picbox - initial properties to assign to PictureBox
     * @param {string=} parentName - optional name of parent control to nest within
     * @param {object} payload - used to pass 'ImagePath' as string property of this object
     * @memberof Dialog
     * @instance
     * @example
     * exoskeleton.dialog.addPictureBox("SampleForm", {
     *   Name: "LogoPicbox",
     *   Location: "10, 10",
     *   Size: "64, 64"
     * }, false, { ImagePath: "C:\\Images\\pic1.png" });
     */
    addPictureBox(formName: string, picbox: any, parentName?: string | null, payload?: any): void;
    /**
     * Adds a RadioButton to a named exoskeleton dialog.
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.radiobutton(v=vs.110).aspx ms docs}
     * @param {string} dialogName - unique name to dialog/form to refer to your dialog.
     * @param {object} radioButton - initial properties to assign to RadioButton
     * @param {string=} parentName - optional name of parent control to nest within
     * @memberof Dialog
     * @instance
     * @example
     * exoskeleton.dialog.addRadioButton("ExampleDialog", {
     *   Name: "GenderMale", Text: "Male", Top: 40, Left: 100, Checked: true
     * });
     * exoskeleton.dialog.addRadioButton("ExampleDialog", {
     *   Name: "GenderFemale", Text: "Female", Top: 40, Left: 140, Checked: false
     * });
     */
    addRadioButton(dialogName: string, radioButton: any, parentName?: string | null): void;
    /**
     * Adds a TextBox to a named exoskeleton dialog.
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.textbox(v=vs.110).aspx ms docs}
     * @param {string} dialogName - unique name to dialog/form to refer to your dialog.
     * @param {object} textbox - initial properties to assign to TextBox
     * @param {string=} parentName - optional name of parent control to nest within
     * @memberof Dialog
     * @instance
     * @example
     * exoskeleton.dialog.addTextBox("ExampleDialog", {
     *   Name: "Street (1)",
     *   Top: 40,
     *   Left: 100,
     *   Text: user.addr1
     * }, "AddressPanel");
     */
    addTextBox(dialogName: string, textbox: any, parentName?: string | null): void;
    /**
     * Adds a TreeView to a named exoskeleton form.
     * Emits 'NodeMouseClick' and 'NodeMouseDoubleClick' events if 'emitEvents' is true.
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.treeview(v=vs.110).aspx ms docs}
     * @param {string} dialogName - unique name to dialog/form to refer to your dialog.
     * @param {object} textbox - initial properties to assign to TextBox
     * @param {string=} parentName - optional name of parent control to nest within
     * @param {object} payload - used to pass 'ImageList' and 'Nodes' collection
     * @memberof Dialog
     * @instance
     * @example
     */
    addTreeView(dialogName: string, treeview: any, parentName?: string | null, payload?: any): void;
    /**
     * Applies a payload to a control which have already been added to a named dialog.
     * Can be used for applying certain values to control which are not able to be applied
     *   just by setting control properties.
     * @param {string} formName - unique name to dialog/form to refer to your dialog.
     * @param {string} controlName - unique name of control to apply payload to.
     * @param {object} payload - object containing payload to apply
     * @memberof Dialog
     * @instance
     * @example
     * exoskeleton.dialog.applyControlPayload("SampleForm", "LogoPicbox", {
     *   ImagePath: "C:\\images\\pic1.png"
     * });
     */
    applyControlPayload(formName: string, controlName: string, payload: any): void;
    /**
     * Applies property values to controls which have already been added.
     * Can be used for separating control layout and data initialization.
     * @param {string} dialogName - unique name to dialog/form to refer to your dialog.
     * @param {object} controlValues - object containing properties to apply to dialog controls.
     * @memberof Dialog
     * @instance
     * @example
     * exoskeleton.dialog.applyControlProperties({
     *   NameTextBox : { Text: "TestUser" },
     *   EmployeeCheckBox : { Checked: false }
     * });
     */
    applyControlProperties(dialogName: string, controlValues: any): void;
    /**
     * Applies a dialog defintion to a named exoskeleton dialog.
     * Dialog definitions allow representation of a series of controls, nesting, and property attributes
     * within a single json object definition.
     * @param {string} dialogName - unique name to dialog/form to refer to your dialog.
     * @param {object} definition - object containing dialog definition.
     * @memberof Dialog
     * @instance
     * @example
     * exoskeleton.dialog.initialize("MyListDialog", { Text: "Test Dialog", Width: 400, Height: 300 });
     * exoskeleton.dialog.applyDefinition("MyListDialog", {
     *   SampleList: {
     *     Type: "ListBox",
     *     Properties: {
     *       Dock: "Fill",
     *       Items: ["one", "two", "three"]
     *     }
     *   },
     *   BottomPanel: { Type: "Panel", Properties: { Dock: "Bottom", Height: 30 } },
     *   OkButton: {
     *     Type: "DialogButton",
     *     Parent: "BottomPanel",
     *     DialogResult: "OK",
     *     Properties: { Text: "OK", Width: 100, Height: 30 }
     *   }
     * });
     * var result = exoskeleton.dialog.showDialog("MyListDialog");
     */
    applyDefinition(dialogName: string, definition: any): void;
    /**
     * Allows you to modify properties on a defined dialog Form object instance.
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.form(v=vs.110).aspx ms docs}
     * @param {string} dialogName - unique name to your defined dialog.
     * @param {object} formJson - object containing .NET 'Form' properties/values
     * @memberof Dialog
     * @instance
     * @example
     * exoskeleton.dialog.applyDialogProperties("ExampleForm", { Text = "Add User dialog" });
     */
    applyDialogProperties(dialogName: string, formJson: any): void;
    /**
     * Initialize a named exoskeleton dialog.
     * @param {string} dialogName - unique name to dialog/form to refer to your dialog.
     * @param {string} formJson - object containing properties to initialize dialog form with.
     * @memberof Dialog
     * @instance
     * @example
     * exoskeleton.dialog.initialize("AddressDialog", {
     *   Text: "Verify address information",
     *   Width: 600,
     *   Height: 400
     * });
     */
    initialize(dialogName: string, formJson: string): void;
    /**
     * Loads a dialog definition from a file and applies it.
     * @param {string} dialogName - unique name to dialog/form to refer to your dialog.
     * @param {string} filename - filename of the file containing a dialog/form definition.
     * @memberof Dialog
     * @instance
     * @example
     * var locations = exoskeleton.getLocations();
     * var defpath = exoskeleton.file.combinePaths([locations.Current, "definitions", "Address.json"]);
     * exoskeleton.dialog.loadDefinition("AddressDialog", defpath);
     */
    loadDefinition(dialogName: string, filename: string): void;
    /**
     * Displays a predefined dialog allowing user to select item(s) from a checklist.
     * @param {string} title - The caption to display on the input dialog window
     * @param {string} prompt - The text to display above, and describing the textbox
     * @param {string[]} values - An array of strings to load checkedlistbox with.
     * @param {int[]=} checkedIndices - Optional array of item indices to default to checked state.
     * @memberof Dialog
     * @instance
     * @example
     * // display picklist with no default selection and no multiselection
     * var result = exoskeleton.dialog.promptCheckedList(
     *   "Country Selection",
     *   "Enter all countries of residence",
     *   ["United States", "United Kingdom", "Germany", "France", "Australia", "Japan", "China", "India"],
     *   [0, 2, 3]
     * );
     *
     * // since no multiselect, result is a string and not array of strings
     * if (result !== null) {
     *   console.log("user picked : " + result);
     * }
     */
    promptCheckedList(title: string, prompt: string, values: string[], checkedIndices?: number[]): any;
    /**
     * Allows display and row selection of an array of similar objects within a .net DataGridView
     * @param {string} title - The caption to display on the input dialog window
     * @param {string} prompt - The text to display above, and describing the textbox
     * @param {object[]} objectArray - An array of object to display and/or select in grid.
     * @param {boolean} autoSizeColumns - (default:true) Whether to automatically size columns.
     * @returns {int[]} Array of selected row indices
     * @memberof Dialog
     * @instance
     * @example
     * var users = [
     *   { name: "john", age: 24, address: "123 alpha street" },
     *   { name: "mary", age: 22, address: "222 gamma street" },
     *   { name: "tom", age: 28, address: "587 delta street" },
     *   { name: "jane", age: 26, address: "428 beta street" }
     * ];
     *
     * var result = exoskeleton.dialog.promptDataGridView("Users", "Select users to invite :", users);
     *
     * // result might contain [0,1,2] for john, mary, and jane indices
     * if (result) {
     *   result.forEach(function(idx) {
     *     console.log("invited: " + users[idx].name);
     *   });
     * }
     */
    promptDataGridView(title: string, prompt: string, objectArray: any[], autoSizeColumns: boolean): number[];
    /**
     * Displays a predefined dialog allowing user to pick a date from a calendar.
     * @param {string} title - The caption to display on the input dialog window
     * @param {string} prompt - The text to display above, and describing the textbox
     * @param {string=} defaultValue - A string encoded date to default selection to.
     * @memberof Dialog
     * @instance
     * @example
     * // display picklist with no default selection and no multiselection
     * var result = exoskeleton.dialog.promptDatePicker(
     *   "Date Selection Example",
     *   "Please choose a start date :",
     *   "12/21/2017"
     * );
     *
     * if (result !== null) {
     *   console.log(".net date" + result.Value);
     *   var jsDate = new Date(result.UniversalEpoch);
     *   console.log("javascript date : " + jsDate);
     * }
     */
    promptDatePicker(title: string, prompt: string, defaultValue?: string | null): any;
    /**
     * Displays a predefined dialog allowing user to input a string.
     * @param {string} title - The caption to display on the input dialog window
     * @param {string} prompt - The text to display above, and describing the textbox
     * @param {string=} defaultText - optional text to initialize textbox with.
     * @memberof Dialog
     * @instance
     * @example
     * var phoneNumber = exoskeleton.dialog.promptInput(
     *   "Confirm Information",
     *   "Enter your telephone number",
     *   user.PhoneNumber
     * );
     *
     * if (phoneNumber !== null) {
     *   user.PhoneNumber = phoneNumber;
     * }
     */
    promptInput(title: string, prompt: string, defaultText?: string | null): any;
    /**
     * Displays a predefined dialog allowing user to select item(s) from a list.
     * @param {string} title - The caption to display on the input dialog window
     * @param {string} prompt - The text to display above, and describing the textbox
     * @param {string[]} values - An array of strings to load listbox with.
     * @param {string|string[]=} selectedItem - string or string[] (if multi) to default selection to.
     * @param {boolean} multiselect - Whether to allow multiple selections
     * @memberof Dialog
     * @instance
     * @example
     * // display listbox with no default selection and no multiselection
     * var result = exoskeleton.dialog.promptList(
     *   "Country Selection",
     *   "Enter your country of residence",
     *   ["United States", "United Kingdom", "Germany", "France", "Australia", "Japan", "China", "India"]
     * );
     *
     * // since no multiselect, result is a string and not array of strings
     * if (result !== null) {
     *   console.log("user picked : " + result);
     * }
     */
    promptList(title: string, prompt: string, values: string[], selectedItem?: string | string[] | null, multiselect?: boolean): any;
    /**
     * Displays a predefined dialog allowing user create an instance of a .net datatype,
     * inspect and modify it, and view it's serialized result.
     * This method is mostly for diagnostic purposes during development.
     * @param {string} title - The title to display on the input dialog window
     * @param {string} caption - The text to display above, and describing the textbox
     * @param {object} objectJson - An object containing properties to apply to new typed instance.
     * @param {string} assemblyName - Name of assembly where type is defined.
     * @param {string} typeName - A name of a .NET datatype.
     * @memberof Dialog
     * @instance
     * @example
     * // Let's just inspect an unitialized ListBox object
     * var result = exoskeleton.dialog.promptPropertyGrid(
     *   "PropertyGrid Inspector",
     *   "Some Listbox",
     *   {}, // we could apply some properties, but we won't for this example
     *   "System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
     *   "System.Windows.Forms.ListBox"
     * );
     *
     * // we can inspect the structure of the object and what properties look like when serialized
     * xo.logObject(result);
     */
    promptPropertyGrid(title: string, caption: string, objectJson: any, assemblyName?: string | null, typeName?: string | null): any;
    /**
     * Displays a named exoskeleton dialog which you have previously created.
     * @param {string} dialogName - unique name to dialog/form to refer to your dialog.
     * @returns {object} An object containing dialog result and form content.
     * @memberof Dialog
     * @instance
     * @example
     * var dialogResult = exoskeleton.dialog.showDialog("AddressDialog");
     * if (dialogResult.Result === 'OK') {
     *   console.log(dialogResult.UserNameTextBox.Text);
     * }
     */
    showDialog(dialogName: string): any;
    /**
     * Display a dialog to allow the user to select a color.
     * See: {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.colordialog(v=vs.110).aspx ms docs}
     * @param {object} dialogOptions - initial properties to assign to ColorDialog
     * @memberof Dialog
     * @instance
     * @example
     * var result = exoskeleton.dialog.showColorDialog();
     * if (result) {
     *   console.log("color : " + result.Color);
     *   console.log("hex color : " + result.HexColor);
     * }
     */
    showColorDialog(dialogOptions: any): any;
    /**
     * Display a dialog to allow the user to select a font.
     * See: {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.fontdialog(v=vs.110).aspx ms docs}
     * @param {object} dialogOptions - initial properties to assign to ColorDialog
     * @memberof Dialog
     * @instance
     * @example
     * var result = exoskeleton.dialog.showFontDialog();
     * if (result) {
     *   console.log("font : " + result.Font);
     *   console.log("fontJson : " + result.FontJson);
     * }
     */
    showFontDialog(dialogOptions: any): any;
    /**
     * Allows user to pick a file to 'open' and returns that filename.  Although only a few options are
     * documented here, any 'OpenFileDialog' properties may be attempted to be passed.
     * See: {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.openfiledialog(v=vs.110).aspx ms docs}
     * @param {object=} dialogOptions - optional object containing 'OpenFileDialog' properties
     * @param {string=} dialogOptions.Title - title to display on open file dialog
     * @param {string=} dialogOptions.InitialDirectory - initial directory to pick file(s) from
     * @param {string=} dialogOptions.Filter - filtering options such as "txt files (*.txt)|*.txt|All files (*.*)|*.*"
     * @param {int=} dialogOptions.FilterIndex - the index of the filter currently selected in the file dialog box
     * @param {boolean=} dialogOptions.Multiselect - whether to allow user to select multiple files
     * @returns {object=} 'OpenFileDialog' properties after dialog was dismissed, or null if cancelled.
     * @memberof Dialog
     * @instance
     * @example
     * // example passing a few (optional) dialog initialization settings
     * var dialogValues = exoskeleton.dialog.showOpenFileDialog({
     *   Title: "Select myapp data file to open",
     *   InitialDirectory: "c:\\mydatafolder",
     *   Filter: "dat files (*.dat)|*.dat|All files (*.*)|*.*"
     * });
     * // if user did not cancel
     * if (dialogValues) {
     *   console.log("selected file (name) : " + dialogValues.FileName);
     * }
     */
    showOpenFileDialog(dialogOptions: any): any;
    /**
     * Displays a message box to the user and returns the button they clicked.
     * @param {string} text - Message to display to user.
     * @param {string} caption - Caption of message box window
     * @param {string=} buttons - "OK"||"OKCancel"||"YesNo"||"YesNoCancel"||"AbortRetryIgnore"||"RetryCancel"
     * @param {string=} icon - "None"||"Information"||"Question"||"Warning"||"Exclamation"||"Hand"||"Error"||"Stop"||"Asterisk"
     * @returns {string} Text (ToString) representation of button clicked.
     * @memberof Dialog
     * @instance
     * @example
     * var dialogResultString = exoskeleton.dialog.showMessageBox(
     *    "An error has occured", "MyApp error", "OKCancel", "Exclamation"
     * );
     * if (dialogResultString === "OK") {
     *   console.log("user clicked ok");
     * }
     */
    showMessageBox(text: string, caption: string, buttons?: string | null, icon?: string | null): string;
    /**
     * Allows user to pick a file to 'save' and returns that filename.  Although only a few options are
     * documented here, any 'SaveFileDialog' properties may be attempted to be passed.
     * See: {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.savefiledialog(v=vs.110).aspx ms docs}
     * @param {object=} dialogOptions - optional object containing 'OpenFileDialog' properties
     * @param {string=} dialogOptions.Title - title to display on save file dialog
     * @param {string=} dialogOptions.InitialDirectory - initial directory to pick file(s) from
     * @param {string=} dialogOptions.Filter - filtering options such as "txt files (*.txt)|*.txt|All files (*.*)|*.*"
     * @param {int=} dialogOptions.FilterIndex - the index of the filter currently selected in the file dialog box
     * @returns {object=} 'SaveFileDialog' properties after dialog was dismissed, or null if cancelled.
     * @memberof Dialog
     * @instance
     * @example
     * // example passing a few (optional) dialog initialization settings
     * var dialogValues = exoskeleton.dialog.showSaveFileDialog({
     *   Title: "Pick data file to save to",
     *   InitialDirectory: "c:\\mydatafolder",
     *   Filter: "dat files (*.dat)|*.dat|All files (*.*)|*.*"
     * });
     * // if user did not cancel
     * if (dialogValues) {
     *   console.log("selected file (name) : " + dialogValues.FileName);
     * }
     */
    showSaveFileDialog(dialogOptions: any): any;
}
/**
 * Form API class for creating and interfacing with WinForms.
 * This API exposes native .NET Form objects.
 * Forms are more dynamic and interactive than dialogs.
 * They support events which javascript can listen for and make exoskeleton calls,
 * including updating the form programmatically. Forms are not modal and can be resized.
 * When your javascript shows a form your javascript will not wait for it to close before
 * continuing.
 */
declare class ExoForm {
    exoForm: any;
    constructor(exoForm: any);
    /**
     * Adds a CheckBox to a named exoskeleton form.
     * Emits 'CheckedChanged' event if 'emitEvents' is true.
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.checkbox(v=vs.110).aspx ms docs}
     * @param {string} formName - unique name to dialog/form to refer to your dialog.
     * @param {object|string} checkbox - initial properties to assign to checkbox
     * @param {string=} parentName - optional name of parent control to nest within
     * @param {boolean=} emitEvents - whether this control should emit event(s)
     * @memberof Form
     * @instance
     * @example
     * exoskeleton.form.addCheckbox("SampleForm", {
     *   Name: "StudentCheckbox",
     *   Text: "Student",
     *   Checked: true,
     *   Top: 100,
     *   Left: 10
     * }, "TopPanel", true);
     *
     * // optionally listen to events because we passed 'true' above to emit them.
     * exoekeleton.events.on("SampleForm.StudentCheckbox.CheckedChanged", function (data) {
     *   console.log(data.Checked?"Is a student":"Not a student");
     * });
     */
    addCheckBox(formName: string, checkbox: any, parentName?: string | null, emitEvents?: boolean): void;
    /**
     * Adds a CheckedListBox to a named exoskeleton form.
     * Emits 'ItemCheck' event if 'emitEvents' is true.
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.checkedlistbox(v=vs.110).aspx ms docs}
     * @param {string} formName - unique name to dialog/form to refer to your dialog.
     * @param {object|string} checkedlistbox - initial properties to assign to ListBox
     * @param {string=} parentName - optional name of parent control to nest within
     * @param {boolean=} emitEvents - whether this control should emit event(s)
     * @param {object=} payload - can used for initializing default 'CheckedIndices'
     * @memberof Form
     * @instance
     * @example
     * exoskeleton.form.addCheckedListBox("SampleForm", {
     *   Name: "CountryChecklist",
     *   Top: 10,
     *   Left: 10,
     *   Dock: 'Fill'
     *   Items: ['United States', 'United Kingdom', 'Germany', 'France', 'Japan']
     * }, "AddressPanel", false, { CheckedIndices: [0, 2] });
     *
     * // if we had set emitEvents to true, we would listen for 'SampleForm.CountryCheckList.ItemCheck'
     */
    addCheckedListBox(formName: string, checkedlistbox: any, parentName?: string | null, emitEvents?: boolean, payload?: any): void;
    /**
     * Adds a ComboBox to a named exoskeleton form.
     * Emits 'SelectedIndexChanged' event if 'emitEvents' is true.
     * If the combo box items will remain static, you can set items with 'Items' property.
     * If the items are to be dynamic, you should set 'DisplayMember' and 'ValueMember'
     * properties and databind by separate applyControlPayload object with 'DataSource' object array.
     * Passing an additional 'DataSourceKeepSelection' payload property will attempt to retain the
     * currently selected item across the rebinding.
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.combobox(v=vs.110).aspx ms docs}
     * @param {string} formName - unique name to dialog/form to refer to your dialog.
     * @param {object|string} comboBox - initial properties to assign to ComboBox
     * @param {string=} parentName - optional name of parent control to nest within
     * @param {boolean=} emitEvents - whether this control should emit event(s)
     * @param {string=} payload - can be used to pass 'DataSource' and 'DataSourceKeepSelection' properties
     * @memberof Form
     * @instance
     * @example
     * exoskeleton.form.addComboBox("SampleForm", {
     *   Name: "CountryDropDown",
     *   Top: 10,
     *   Left: 10,
     *   Items: ['United States', 'United Kingdom', 'Germany', 'France', 'Japan'],
     *   SelectedItem : 'United States'
     * }, "AddressPanel", true);
     *
     * exoskeleton.events.on("SampleForm.CountryDropDown.SelectedIndexChanged", function(data) {
     *   console.log(data.Selected);
     * });
     */
    addComboBox(formName: string, comboBox: any, parentName?: string | null, emitEvents?: boolean, payload?: any): void;
    /**
     * Adds a DataGridView to a named exoskeleton form.
     * Emits 'SelectionChanged' event if 'emitEvents' is true.
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.datagridview(v=vs.110).aspx ms docs}
     * @param {string} formName - unique name to dialog/form to refer to your dialog.
     * @param {object} gridView - initial properties to assign to DataGridView
     * @param {string=} parentName - optional name of parent control to nest within
     * @param {boolean=} emitEvents - whether this control should emit event(s)
     * @param {object=} payload - used for initializing 'ObjectArray' you want to display in grid
     * @memberof Form
     * @instance
     * @example
     * var users = [
     *   { name: "john", age: 24, address: "123 alpha street" },
     *   { name: "mary", age: 22, address: "222 gamma street" },
     *   { name: "tom", age: 28, address: "587 delta street" },
     *   { name: "jane", age: 26, address: "428 beta street" }
     * ];
     *
     * var result = exoskeleton.form.addDataGridView("SampleForm", {
     *   Name: "UserGridView",
     *   Dock: 'Fill',
     *   ReadOnly: true,
     *   AllowUserToAddRows: false,
     *   SelectionMode: 'FullRowSelect'
     * }, "DetailsPanel", false, { ObjectArray: users });
     *
     * // normally you might use event buttons to trigger code which 'reads' values
     * // from form. So for this example will not emit events, but we could have.
     */
    addDataGridView(formName: string, gridView: any, parentName?: string | null, emitEvents?: boolean, payload?: any): void;
    /**
     * Adds a DateTimePicker to a named exoskeleton form.
     * Emits 'ValueChanged' event if 'emitEvents' is true.
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.datetimepicker(v=vs.110).aspx ms docs}
     * @param {string} formName - unique name to dialog/form to refer to your dialog.
     * @param {object} dateTimePicker - initial properties to assign to DateTimePicker
     * @param {string=} parentName - optional name of parent control to nest within
     * @param {boolean=} emitEvents - whether this control should emit event(s)
     * @memberof Form
     * @instance
     * @example
     * exoskeleton.form.addDateTimePicker("SampleForm", {
     *   Name: "StartDate",
     *   Top: 100,
     *   Left: 100,
     *   Value: "12/13/2014"
     * }, "AddressPanel", true);
     *
     * exoskeleton.events.on("SampleForm.StartDate.ValueChanged", function(data) {
     *   console.log("Short date string : " + data.Short);
     *   Date dt = new Date(data.UniversalEpoch);
     *   console.log("Date converted to javascript : " + dt);
     * });
     */
    addDateTimePicker(formName: string, dateTimePicker: any, parentName?: string | null, emitEvents?: boolean): void;
    /**
     * Adds a Button to a named exoskeleton form which will raise an Unicast event.
     * This is a standard .net button with hardcoded event handler to raise 'Click' events.
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.button(v=vs.110).aspx ms docs}
     * @param {string} formName - unique name to dialog/form to refer to your dialog.
     * @param {object|string} button - initial properties to assign to button
     * @param {string=} parentName - optional name of parent control to nest within
     * @memberof Form
     * @instance
     * @example
     * // event buttons will always emit their 'Click' event, so it's not even a param
     * exoskeleton.form.addEventButton("SampleForm", {
     *   Name: "SaveButton",
     *   Text: "Save",
     *   Top: 10,
     *   Left: 200
     * }, "BottomPanel");
     *
     * exoskeleton.events.on("SampleForm.SaveButton.Click", function() {
     *   console.log('Button clicked!');
     * });
     */
    addEventButton(formName: string, button: any, parentName?: string | null): void;
    /**
     * Adds a Label to a named exoskeleton form.
     * Emits 'Click' event if 'emitEvents' is true.
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.label(v=vs.110).aspx ms docs}
     * @param {string} formName - unique name to dialog/form to refer to your dialog.
     * @param {object|string} label - initial properties to assign to label
     * @param {string=} parentName - optional name of parent control to nest within
     * @param {boolean=} emitEvents - whether this control should emit event(s)
     * @memberof Form
     * @instance
     * @example
     * exoskeleton.form.addLabel("SampleForm", {
     *   Name: "AddressLabel",
     *   Text: "Address:",
     *   Top: 100,
     *   Left: 10
     * }, "AddressPanel");
     */
    addLabel(formName: string, label: any, parentName?: string | null, emitEvents?: boolean): void;
    /**
     * Adds a ListBox to a named exoskeleton form.
     * Emits 'SelectedIndexChanged' event if 'emitEvents' is true.
     * If the list box items will remain static, you can set items with 'Items' property.
     * If the items are to be dynamic, you should set 'DisplayMember' and 'ValueMember'
     * properties and databind by separate applyControlPayload object with 'DataSource' object array.
     * Passing an additional 'DataSourceKeepSelection' payload property will attempt to retain the
     * currently selected item across the rebinding.
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.listbox(v=vs.110).aspx ms docs}
     * @param {string} formName - unique name to dialog/form to refer to your dialog.
     * @param {object|string} listbox - initial properties to assign to ListBox
     * @param {string=} parentName - optional name of parent control to nest within
     * @param {boolean=} emitEvents - whether this control should emit event(s)
     * @param {string=} payload - can be used to pass 'DataSource' and 'DataSourceKeepSelection' properties
     * @memberof Form
     * @instance
     * @example
     * exoskeleton.form.addListBox("SampleForm", {
     *   Name: "CountryList",
     *   Top: 10,
     *   Left: 10,
     *   Dock: 'Fill'
     *   Items: ['United States', 'United Kingdom', 'Germany', 'France', 'Japan'],
     *   SelectedItem : 'United States'
     * }, "AddressPanel");
     */
    addListBox(formName: string, listbox: any, parentName?: string | null, emitEvents?: boolean, payload?: any): void;
    /**
     * Adds a ListView to a named exoskeleton form.
     * Emits 'Click' and 'DoubleClick' events if 'emitEvents' is true.
     * ListView has the following view modes (defined by 'View' property) :
     * - LargeIcon: 'Thumbnail-like' list using 'Items' payload property (no subitems).
     * - Details - Tabular list using 'ItemArrays' 2-dimensional payload property for subitems.
     * - SmallIcon - 'Left-to-Right, Top-to-Bottom' list needing only 'Items' payload property.
     * - List - 'Top-to-Bottom' list needing only 'Items' payload property.
     * - Tile - 'L->R, T->B' list with large icons, displaying subitems as multiple lines (2D 'ItemArrays')
     *
     * The various allowable payloads include :
     * - 'Columns' - In a tabular view mode these define the column captions/sizes ('Text', 'Width')
     * - 'Items' - Items array supports single dimension array where each array element is a different list item.
     * - 'ItemArrays' - ItemArrays support 2-dimension array allowing multiple columns for each 'row'.
     * - 'AppendItems' - AppendItems is the same as 'Items' but the list will not be cleared before adding those (new) items.
     * - 'AppendItemArrays' - AppendItemArrays will not clear the listview before appending the new items.
     *
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.listview(v=vs.110).aspx ms docs}.
     * @param {string} formName - unique name to dialog/form to refer to your form.
     * @param {object|string} listview - initial properties to assign to ListView
     * @param {string=} parentName - optional name of parent control to nest within
     * @param {boolean=} emitEvents - whether this control should emit event(s)
     * @param {string=} payload - can be used to pass 'Columns', 'Items' and/or 'ItemArrays' properties
     * @memberof Form
     * @instance
     * @example
     * // LargeIcon view mode - depends on already established named imagelist
     * var controlProperties = { Name: "CountryList", Top: 10, Left: 10, Dock: 'Fill', View: "LargeIcon" };
     * var controlPayload = {
     *   LargeImageList: "LargeListIcons",
     *   Items: [{ Text: "One", ImageIndex: 0 }, { Text: "Two", ImageIndex: 1 }]
     * };
     * exoskeleton.form.addListView("ExampleForm", controlProperties, "AddressPanel", false, controlPayload);
     *
     * // Details view mode example
     * controlProperties = { Name: "CountryList", Top: 10, Left: 10, Dock: 'Fill', View: "Details", FullRowSelect: true }
     * var controlPayload = {
     *   SmallImageList: "SmallListIcons",
     *   Columns: [{ Text: "Country", Width:100 }, { Text:"Info1", Width: 120 }, { Text: "Info2", Width: 100 }],
     *   ItemArrays: [
     *     [{ Text: "Item One", ImageIndex: 0 }, { Text: "Stuff about One" }, { Text: "More stuff about One" } ],
     *     [{ Text: "Item Two", ImageIndex: 1 }, { Text: "Stuff about Two" }, { Text: "More stuff about Two" } ],
     *     [{ Text: "Item Three", ImageIndex: 2 }, { Text: "Stuff about Three" }, { Text: "More stuff about Three" }]
     *   ]
     * };
     * exoskeleton.form.addListView("ExampleForm", controlProperties, "AddressPanel", false, controlPayload);
     */
    addListView(formName: string, listview: any, parentName?: string | null, emitEvents?: boolean, payload?: any): void;
    /**
     * Adds a MaskedTextBox to a named exoskeleton form.
     * Emits 'TextChanged' event if 'emitEvents' is true.
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.maskedtextbox(v=vs.110).aspx ms docs}
     * @param {string} formName - unique name to dialog/form to refer to your dialog.
     * @param {object} maskedtextbox - object containing properties to apply to the MaskedTextBox
     * @param {string=} parentName - optional name of parent control to nest within
     * @param {boolean=} emitEvents - whether this control should emit event(s)
     * @memberof Form
     * @instance
     * @example
     * exoskeleton.form.addMaskedTextBox("SampleForm", {
     *   Name: "PhoneNumberMaskedEdit",
     *   Mask: "(999)-000-0000",
     *   Top: 124,
     *   Left: 48,
     *   Width: 100
     * }, "ContactInfoPanel");
     */
    addMaskedTextBox(formName: string, maskedtextbox: any, parentName?: string | null, emitEvents?: boolean): void;
    /**
     * Adds a MonthCalendar to a named exoskeleton form.
     * Emits 'DateChanged' event if 'emitEvents' is true.
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.monthcalendar(v=vs.110).aspx ms docs}
     * @param {string} formName - unique name to dialog/form to refer to your dialog.
     * @param {object} monthcalendar - object containing properties to apply to the MonthCalendar
     * @param {string=} parentName - optional name of parent control to nest within
     * @param {boolean=} emitEvents - whether this control should emit event(s)
     * @memberof Form
     * @instance
     * @example
     * // Since no parentName value is passed, it will be added directly to the form
     * exoskeleton.form.addMonthCalendar("SampleForm", {
     *   Name: "MonthCalendar",
     *   ShowToday: true,
     *   ShowTodayCircle: true,
     *   MonthlyBoldedDates: ["1/1/2017", "1/15/2017"],  // bolds 1st and 15th days of every month,
     *   AnnuallyBoldedDates: ["3/20/2017", "6/1/2017", "9/22/2017", "12/22/2017"],
     *   Top: 40,
     *   Left: 40
     * });
     */
    addMonthCalendar(formName: string, monthcalendar: any, parentName?: string | null, emitEvents?: boolean): void;
    /**
     * Adds a NumericUpDown control to a named exoskeleton form.
     * Emits 'ValueChanged' event if 'emitEvents' is true.
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.numericupdown(v=vs.110).aspx ms docs}
     * @param {string} formName - unique name to dialog/form to refer to your dialog.
     * @param {object|string} numericUpDown - initial properties to assign to ListBox
     * @param {string=} parentName - optional name of parent control to nest within
     * @param {boolean=} emitEvents - whether this control should emit event(s)
     * @memberof Form
     * @instance
     * @example
     * exoskeleton.form.addNumericUpDown("SampleForm", {
     *   Name: "AgeNumeric",
     *   Top: 10,
     *   Left: 10,
     *   Minimum: 13,
     *   Maximum: 120
     * }, "UserInfoPanel");
     */
    addNumericUpDown(formName: string, numericUpDown: any, parentName?: string | null, emitEvents?: boolean): void;
    /**
     * Adds a Panel to a named exoskeleton form for layout and nesting purposes.
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.panel(v=vs.110).aspx ms docs}
     * @param {string} formName - unique name to dialog/form to refer to your dialog.
     * @param {object|string} panel - initial properties to assign to panel
     * @param {string=} parentName - optional name of parent control to nest within
     * @param {boolean=} emitEvents - whether this control should emit event(s)
     * @memberof Form
     * @instance
     * @example
     * exoskeleton.form.addPanel("SampleForm", {
     *   Name: "FillPanel",
     *   Dock: 'Fill'
     * });
     * exoskeleton.form.addPanel("SampleForm", {
     *   Name: "TopPanel",
     *   Dock: 'Top',
     *   Height: 100
     * });
     * exoskeleton.form.addPanel("SampleForm", {
     *   Name: "BottomPanel",
     *   Dock: 'Bottom',
     *   Height: 100
     * });
     */
    addPanel(formName: string, panel: any, parentName?: string | null, emitEvents?: boolean): void;
    /**
     * Adds a PictureBox to a named exoskeleton form for layout and nesting purposes.
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.picturebox(v=vs.110).aspx ms docs}
     * @param {string} formName - unique name to dialog/form to refer to your dialog.
     * @param {object|string} picbox - initial properties to assign to PictureBox
     * @param {string=} parentName - optional name of parent control to nest within
     * @param {boolean=} emitEvents - whether this control should emit 'Click' event
     * @param {object} payload - used to pass 'ImagePath' as string property of this object
     * @memberof Form
     * @instance
     * @example
     * exoskeleton.form.addPictureBox("SampleForm", {
     *   Name: "LogoPicbox",
     *   Location: "10, 10",
     *   Size: "64, 64"
     * }, false, { ImagePath: "C:\\Images\\pic1.png" });
     */
    addPictureBox(formName: string, picbox: any, parentName?: string | null, emitEvents?: boolean, payload?: any): void;
    /**
     * Adds a RadioButton to a named exoskeleton form.
     * Emits 'CheckedChanged' event if 'emitEvents' is true.
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.radiobutton(v=vs.110).aspx ms docs}
     * @param {string} formName - unique name to dialog/form to refer to your dialog.
     * @param {object} radioButton - initial properties to assign to RadioButton
     * @param {string=} parentName - optional name of parent control to nest within
     * @param {boolean=} emitEvents - whether this control should emit event(s)
     * @memberof Form
     * @instance
     * @example
     * exoskeleton.form.addRadioButton("SampleForm", {
     *   Name: "GenderMale", Text: "Male", Top: 40, Left: 100, Checked: true
     * }, null, true);
     *
     * exoskeleton.form.addRadioButton("SampleForm", {
     *   Name: "GenderFemale", Text: "Female", Top: 40, Left: 140, Checked: false
     * });
     *
     * // since only 2 radio buttons, we will just listen to one of the radio buttons
     * exoskeleton.events.on("SampleForm.GenderMale.CheckedChanged", function(data) {
     *   console.log(data.Checked?"male":"female");
     * });
     */
    addRadioButton(formName: string, radioButton: any, parentName?: string | null, emitEvents?: boolean): void;
    /**
     * Adds a TextBox to a named exoskeleton form.
     * Emits 'TextChanged' event if 'emitEvents' is true.
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.textbox(v=vs.110).aspx ms docs}
     * @param {string} formName - unique name to dialog/form to refer to your dialog.
     * @param {object} textbox - initial properties to assign to TextBox
     * @param {string=} parentName - optional name of parent control to nest within
     * @param {boolean=} emitEvents - whether this control should emit event(s)
     * @memberof Form
     * @instance
     * @example
     * exoskeleton.form.addTextBox("SampleForm", {
     *   Name: "Street (1)",
     *   Top: 40,
     *   Left: 100,
     *   Text: user.addr1
     * }, "AddressPanel");
     */
    addTextBox(formName: string, textbox: any, parentName?: string | null, emitEvents?: boolean): void;
    /**
     * Adds a TreeView to a named exoskeleton form.
     * Emits 'NodeMouseClick', 'AfterSelect' and 'NodeMouseDoubleClick' events if 'emitEvents' is true.
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.treeview(v=vs.110).aspx ms docs}
     * @param {string} formName - unique name to dialog/form to refer to your dialog.
     * @param {object} textbox - initial properties to assign to TextBox
     * @param {string=} parentName - optional name of parent control to nest within
     * @param {boolean=} emitEvents - whether this control should emit event(s)
     * @param {object} payload - used to pass 'ImageList' and 'Nodes' collection
     * @memberof Form
     * @instance
     * @example
     */
    addTreeView(formName: string, treeview: any, parentName?: string | null, emitEvents?: boolean, payload?: any): void;
    /**
     * Applies a payload to a control which have already been added to a named form.
     * Can be used for applying certain values to control which are not able to be applied
     *   just by setting control properties.
     * @param {string} formName - unique name to dialog/form to refer to your dialog.
     * @param {string} controlName - unique name of control to apply payload to.
     * @param {object} payload - object containing payload to apply
     * @memberof Form
     * @instance
     * @example
     * exoskeleton.form.applyControlPayload("SampleForm", "CountryCheckListBox", {
     *   CheckedIndices: [0, 3]
     * });
     */
    applyControlPayload(formName: string, controlName: string, payload: any): void;
    /**
     * Applies property values to controls which have already been added to a named form.
     * Can be used for separating control layout and data initialization.
     * @param {string} formName - unique name to dialog/form to refer to your dialog.
     * @param {object} controlValues - object containing properties to apply to dialog controls.
     * @memberof Form
     * @instance
     * @example
     * exoskeleton.form.applyControlProperties("SampleForm", {
     *   NameTextBox : { Text: "TestUser" },
     *   EmployeeCheckBox : { Checked: false }
     * });
     */
    applyControlProperties(formName: string, controlValues: any): void;
    /**
     * Applies a form defintion to a named form.
     * Dialog definitions allow representation of a series of controls, nesting, and property attributes
     * within a single json object definition.
     * @param {string} formName - unique name to dialog/form to refer to your dialog.
     * @param {object} definition - object containing dialog definition.
     * @memberof Form
     * @instance
     * @example
     * exoskeleton.form.initialize("ExampleListDialog", { Text: "Test Dialog", Width: 400, Height: 300 });
     * exoskeleton.form.applyDefinition("ExampleListDialog", {
     *   SampleList: {
     *     Type: "ListBox",
     *     Properties: {
     *       Dock: "Fill",
     *       Items: ["one", "two", "three"]
     *     }
     *   },
     *   BottomPanel: { Type: "Panel", Properties: { Dock: "Bottom", Height: 30 } },
     *   OkButton: {
     *     Type: "EventButton",
     *     Parent: "BottomPanel",
     *     EventName: "SaveEvent",
     *     Properties: { Text: "Save", Width: 100, Height: 30 }
     *   }
     * });
     * var result = exoskeleton.form.show("ExampleListDialog");
     */
    applyDefinition(formName: string, definition: any): void;
    /**
     * Allows you to modify properties on a defined Form after it has been initialized.
     * See {@link https://msdn.microsoft.com/en-us/library/system.windows.forms.form(v=vs.110).aspx ms docs}
     * @param {string} formName - unique name to your defined form.
     * @param {object} formJson - object containing 'form' level properties/values
     * @memberof Form
     * @instance
     * @example
     * exoskeleton.form.applyFormProperties("ExampleForm", { WindowState = "Minimized" });
     */
    applyFormProperties(formName: any, formJson: any): void;
    /**
     * Closes a named form.
     * @param {string} formName - unique name to dialog/form to refer to your dialog.
     * @memberof Form
     * @instance
     * @example
     * exoskeleton.form.close("ExampleForm");
     */
    close(formName: string): void;
    /**
     * Initialize or reinitialize a named form.
     * @param {string} formName - unique name to dialog/form to refer to your dialog.
     * @param {string} formJson - object containing properties to initialize dialog form with.
     * @memberof Form
     * @instance
     * @example
     * exoskeleton.form.initialize("UserEditorForm", {
     *   Text: "Manage users",
     *   Width: 600,
     *   Height: 400
     * });
     */
    initialize(formName: string, formJson: any): void;
    /**
     * Obtains object with control values for all controls on form.
     * @param {string} formName - name of form to get control values for.
     * @returns {object} - hashobject keyed on control name, with simplified object values for value
     * @memberof Form
     * @instance
     * @example
     * var result = exoskeleton.form.generateDynamicResponse("ExampleForm");
     */
    generateDynamicResponse(formName?: string | null): any;
    /**
     * Obtains object this simplified control values for a given form name and control name
     * @param {string} formName - name of form your control is on
     * @param {string} controlName - name to control on specified form.
     * @returns {object} - object containing simplified properties and their values.
     * @memberof Form
     * @instance
     * @example
     * var countryValues = exoskeleton.form.getControlProperties("UserEditorForm", "CountryCheckListBox");
     * console.log(countryValues.CheckedIndices);
     */
    getControlProperties(formName?: string | null, controlName?: string | null): any;
    /**
     * Loads a dialog definition from a file and applies it.
     * @param {string} formName - unique name to dialog/form to refer to your dialog.
     * @param {string} filename - filename of the file containing a dialog/form definition.
     * @memberof Form
     * @instance
     * @example
     * var locations = exoskeleton.getLocations();
     * var defpath = exoskeleton.file.combinePaths([locations.Current, "definitions", "UserEditor.json"]);
     * exoskeleton.form.loadDefinition("UserEditor", defpath);
     */
    loadDefinition(formName: string, filename: string): void;
    /**
     * Displays a named form which you previously configured.
     * @param {string} formName - unique name to dialog/form to refer to your dialog.
     * @memberof Form
     * @instance
     * @example
     * exoskeleton.form.show("UserEditorForm");
     */
    show(formName: string): void;
}
declare class Exoskeleton {
    exo: any;
    keystore: KeyStoreAdapter;
    events: ExoEventEmitter;
    serviceProcessors: any[];
    media?: ExoMedia;
    com?: ExoCom;
    file?: ExoFile;
    main?: ExoMain;
    proc?: ExoProc;
    session?: ExoSession;
    system?: ExoSystem;
    logger?: ExoLogger;
    net?: ExoNet;
    enc?: ExoEnc;
    menu?: ExoMenu;
    toolbar?: ExoToolbar;
    statusbar?: ExoStatusbar;
    util?: ExoUtil;
    dialog?: ExoDialog;
    form?: ExoForm;
    /**
    * Exoskeleton main javascript facade interface to the C# API exposed via COM.
    *
    * @constructor Exoskeleton
    */
    constructor();
    private overrideConsole;
    /**
    * Returns the currently active settings (xos file), converted to a json string, without resolving urls.
    * @returns {object} The current application settings
    * @memberof Exoskeleton
    * @instance
    * @example
    * var settings = exoskeleton.getApplicationSettings();
    *
    * if (settings.ScriptingMediaEnabled) {
    *   exoskeleton.speech.speak("hello");
    * }
    */
    getApplicationSettings(): any;
    /**
     * Returns the important exoskeleton environment locations. (Current, Settings, Executable)
     * @returns {object} Object containing resolved paths to common locations in Windows and Exoskeleton.
     * @memberof Exoskeleton
     * @instance
     * @example
     * var locations = exoskeleton.getLocations();
     * console.log("current directory : " + locations.Current);
     * console.log("location of (active) settings file : " + locations.Settings);
     * console.log("location of (active) exoskeleton executable : " + locations.Executable);
     */
    getLocations(): ExoskeletonLocations;
    /**
     * Returns the version number for the exoskeleton host you are running in.
     * @returns {string} The version of exoskeleton you are hosted within.
     * @memberof Exoskeleton
     * @instance
     * @example
     * var version = exoskeleton.getVersion();
     * console.log("host exo version : " + version);
     */
    getVersion(): any;
    /**
     * Registers a javascript function for processing exoskeleton web service requests.
     * This requires enabling web services in your settings file.
     * @param {function} processor - Your web service processor function.
     * @memberof Exoskeleton
     * @instance
     * @example
     * function myServiceProcessor (request) {
     *   if (request.AbsolutePath === "/version.svc") {
     *     return {
     *       ContentType: 'application/json',
     *       Response: JSON.stringify({ name: 'myapp', major: 1, minor: 4, patch: 7 })
     *     }
     *   }
     *
     *   return null;
     * }
     *
     * exoskeleton.registerServiceProcessor(myServiceProcessor);
     */
    registerServiceProcessor(processor: any): void;
    /**
     * Initiates shutdown of this exoskeleton app by notifying the container.
     * @memberOf Exoskeleton
     */
    shutdown: () => void;
}
declare var exoskeleton: Exoskeleton;
