/**
 * exoskeleton.js 
 * @author obeliskos <github@obeliskos.com>
 *
 * A wrapper framework for functionality exposed by the exoskeleon shell
 */

/**
 * Global reference to c# ole scripting object class
 *
 * Although this can be called directly, we will add 'exoskeleton' wrapper below.
 */
var exo = (window && window.external) ? window.external : {};

/**
 * Wrapper object to .net scripting object
 *
 * This will be used for abstracting JSON parsing and serialization as well
 * as for fixing up parameters in case we provide multiple overloads in c#.
 */
var exoskeleton = {
    media: {
        speak: function (message) {
            exo.media.speak(message);
        }
    },
    file: {
        saveTextFile: function (filename, contents) {
            exo.file.saveFile(filename, contents);
        },
        loadTextFile: function (filename) {
            return exo.file.loadFile(filename);
        },
        getCurrentDirectory: function () {
            return exo.file.getCurrentDirectory();
        },
        getLogicalDrives: function () {
            return JSON.parse(exo.file.getLogicalDrives());
        },
        getDirectories: function (parentDir) {
            return JSON.parse(exo.file.getDirectories(parentDir));
        },
        getFiles: function (parentDir, searchPattern) {
            return JSON.parse(exo.file.getFiles(parentDir, searchPattern));
        }
    },
    main: {
        showNotification: function (title, message) {
            exo.main.showNotification(title, message);
        },
        setWindowTitle: function (title) {
            exo.main.setWindowTitle(title);
        },
        fullscreen: function () {
            window.external.main.fullscreen();
        },
        exitFullscreen: function () {
            exo.main.exitFullscreen();
        }
    },
    proc: {
        start: function (programFilename) {
            exo.proc.start(programFilename);
        }
    }
};

/**
 * Key/Value file storage adapter for exoskeleton.
 *
 * This will also be a valid lokijs database adapter.
 * For now, this will just utilize synchronous c# calls.
 */
var ExoskeletonKeystoreAdapter = {
  loadDatabase: function(dbname, callback) {
    var result = exoskeleton.file.loadTextFile(dbname);
    callback(result);
  },
  saveDatabase: function(dbname, dbstring, callback) {
    exoskeleton.file.saveTextFile(dbname, dbstring);
    callback(null);
  }
};


if (window && window.external) {
    // let's also assume control over errors raised and pipe them through our own logger
    window.onerror = function (msg, url, line, col, error) {
        alert(msg + " " + " (line " + line + " col " + col + ") " + url);
        return true;
    };
}

