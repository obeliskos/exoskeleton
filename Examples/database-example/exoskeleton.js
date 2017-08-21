// exoskeleton.js - wrapper framework for com object functionality exposed by the exoskeleon shell

// global reference to c# ole scripting object class
var exo = (window && window.external) ? window.external : {};

// package any classes needed later by the 'exoskeleton' singleton
(function (root, factory) {
    if (typeof define === 'function' && define.amd) {
        // AMD
        define([], factory);
    } else if (typeof exports === 'object') {
        // Node, CommonJS-like
        module.exports = factory();
    } else {
        // Browser globals (root is window)
        root.ExoClasses = factory();
    }
}(this, function () {
    return (function () {
        function ExoClasses() { }

        /**
         * Persistent Key/Value store which can also be used as a loki database persistence adapter.
         * @param {any} exoskeleton : pass in reference to exoskeleton singleton
         */
        function KeyStoreAdapter(exoskeleton) {
            this.exoskeleton = exoskeleton;
        }

        KeyStoreAdapter.prototype.loadDatabase = function (dbname, callback) {
            var result = this.exoskeleton.file.loadFile(dbname);
            if (typeof callback === 'function') {
                callback(result);
            }
        };

        KeyStoreAdapter.prototype.saveDatabase = function (dbname, dbstring, callback) {
            // synchronous? for now
            this.exoskeleton.file.saveFile(dbname, dbstring);

            if (typeof callback === 'function') {
                callback(null);
            }
        };

        /**
         * Event emitter for listening to or emitting events within your page(s)
         *
         * While we currently support only one main exoskeleton hosted page/frame, this
         * may be enhanced in the future to support eventing across all page frames.
         */
        function ExoEventEmitter(exo) {
            this.exo = exo;
        }

        ExoEventEmitter.prototype.events = {};

        ExoEventEmitter.prototype.asyncListeners = false;

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
            if (eventName.indexOf("local.") != 0 && eventName.indexOf("multicast.") != 0) {
                exo.multicast(eventName, JSON.stringify(selfArgs));
            }
        };

        ExoClasses.EventEmitter = ExoEventEmitter;
        ExoClasses.KeyStoreAdapter = KeyStoreAdapter;

        return ExoClasses;
    }());
}));

// singleton wrapper for exo object, in case we want to do JSON result parsing or parameter encoding
var exoskeleton = {
    media: {
        speak: function (message) {
            if (!exo.media) return;

            exo.media.speak(message);
        }
    },
    file: {
        getLogicalDrives: function () {
            if (!exo.file) return;

            return JSON.parse(exo.file.getLogicalDrives());
        },
        getDriveInfo: function () {
            if (!exo.file) return;

            return JSON.parse(exo.file.getDriveInfo());
        },
        getCurrentDirectory: function () {
            if (!exo.file) return;

            return exo.file.getCurrentDirectory()
        },
        getDirectories: function (parentDir) {
            if (!exo.file) return;

            return JSON.parse(exo.file.getDirectories(parentDir));
        },
        createDirectory: function (path) {
            if (!exo.file) return;

            exo.file.createDirectory(path);
        },
        deleteDirectory: function (path) {
            if (!exo.file) return;

            exo.file.deleteDirectory(path);
        },
        getFiles: function (parentDir, searchPattern) {
            if (!exo.file) return;

            return JSON.parse(exo.file.getFiles(parentDir, searchPattern));
        },
        saveFile: function (filename, contents) {
            if (!exo.file) return;

            exo.file.saveFile(filename, contents);
        },
        loadFile: function (filename) {
            if (!exo.file) return;

            return exo.file.loadFile(filename);
        },
        copyFile: function (source, dest) {
            if (!exo.file) return;

            exo.file.copyFile(source, dest);
        },
        deleteFile: function (filename) {
            if (!exo.file) return;

            exo.file.deleteFile(filename);
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
            exo.main.fullscreen();
        },
        exitFullscreen: function () {
            exo.main.exitFullscreen();
        },
        openNewWindow: function (caption, url, width, height) {
            exo.main.openNewWindow(caption, url, width, height);
        }
    },
    proc: {
        start: function (procPath) {
            if (!exo.proc) return;

            exo.proc.start(procPath);
        }
    },
    session: {
        set: function (key, value) {
            if (!exo.session) return;

            exo.session.set(key, value);
        },
        get: function (key) {
            if (!exo.session) return;

            return exo.session.get(key);
        },
        setObject: function (key, value) {
            if (!exo.session) return;

            exo.session.set(key, JSON.stringify(value));
        },
        getObject: function (key) {
            if (!exo.session) return;

            var result = exo.session.get(key);
            return result?JSON.parse(result):result;
        },
        list: function () {
            if (!exo.session) return;

            return JSON.parse(exo.session.list());
        }
    },
    system: {
        getSystemInfo: function () {
            if (!exo.system) return;

            return JSON.parse(exo.system.getSystemInfo());
        }
    },
    logger: {
        logError: function (msg, url, line, col, error) {
            if (!exo.logger) return;

            exo.logger.logError(msg, url, line, col, error);
        },
        logInfo: function (source, message) {
            if (!exo.logger) return;

            exo.logger.logInfo(source, message);
        },
        logWarning: function (source, message) {
            if (!exo.logger) return;

            exo.logger.logWarning(source, message);
        },
        logText: function (message) {
            if (!exo.logger) return;

            exo.logger.logText(message);
        }
    },
    events: new ExoClasses.EventEmitter(exo),
    shutdown: function () {
        exo.shutdown();
    }
};

window.exoskeletonEmitEvent = function (eventName, eventData) {
    exoskeleton.events.emit(eventName, eventData);
};

if (window && window.external) {
    // let's also assume control over errors raised and pipe them through our own logger
    window.onerror = function (msg, url, line, col, error) {
        exoskeleton.logger.logError(msg, url, line, col, error);
        return true;
    };
}

// and override console functionality to use our logger
var console = {
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

        exoskeleton.logger.logText(obj?JSON.stringify(obj, null, 2):"null");
    }
};

