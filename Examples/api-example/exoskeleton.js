// exoskeleton.js - wrapper framework for com object functionality exposed by the exoskeleon shell

// global reference to c# ole scripting object class
var exo = (window && window.external) ? window.external : {};

// wrapper object in case we want to abstract JSON parsing or fixup parameters
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

/*
if (window && window.external) {
    // let's also assume control over errors raised and pipe them through our own logger
    window.onerror = function (msg, url, line, col, error) {
        exoskeleton.logger.logError(msg + " (line " + line + " col " + col + ")");
        return true;
    };
}
*/
