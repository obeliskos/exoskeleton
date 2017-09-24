var xo = {
    currentFilepath: "",
    locations: null,
    settings: null,
    editorJS: null,
    editorOutput: null,
    themes: ["default", "3024-day", "3024-night", "abcdef", "ambiance", "base16-dark", "base16-light", "bespin",
        "blackboard", "cobalt", "colorforth", "dracula", "duotone-dark", "duotone-light", "eclipse", "elegant",
        "erlang-dark", "hopscotch", "icecoder", "isotope", "lesser-dark", "liquibyte", "material", "mbo",
        "mdn-like", "midnight", "monokai", "neat", "neo", "night", "panda-syntax", "paraiso-dark", "paraiso-light",
        "pastel-on-dark", "railscasts", "rubyblue", "seti", "solarized dark", "solarized light", "the-matrix",
        "tomorrow-night-bright", "tomorrow-night-eighties", "ttcn", "twilight", "vibrant-ink", "xq-dark",
        "xq-light", "yeti", "zenburn"]
}

$(document).ready(function () {
    $(window).resize(xo.windowResize);

    $("body").css("background-color", "#222");

    var theme = localStorage["xo.editorTheme"];
    if (theme) {
        $("#select-theme option").filter(function () {
            return $(this).text() === theme;
        }).prop('selected', true);
    }

    xo.locations = exoskeleton.main.getLocations();
    xo.settings = exoskeleton.main.getApplicationSettings();

    xo.initializeMenu();
    xo.initializeToolbar();
    xo.initializeStatusbar();
    exoskeleton.main.doEvents();

    xo.setupEditor(theme);

    setTimeout(xo.windowResize, 100);

});

xo.initializeMenu = function () {
    exoskeleton.menu.initialize();
    exoskeleton.menu.addMenu("&File");
    exoskeleton.menu.addMenuItem("&File", "&New", "NewEvent");
    exoskeleton.menu.addMenuItem("&File", "&Open...", "OpenEvent");
    exoskeleton.menu.addMenuItem("&File", "&Save", "SaveEvent");
    exoskeleton.menu.addMenuItem("&File", "Save &as...", "SaveEvent");
    exoskeleton.menu.addMenuItem("&File", "-");
    exoskeleton.menu.addMenuItem("&File", "&Explore samples folder...", "ExploreSamplesEvent");
    exoskeleton.menu.addMenuItem("&File", "-");
    exoskeleton.menu.addMenuItem("&File", "E&xit", "ExitEvent");
    exoskeleton.menu.addMenu("&Themes");
    xo.themes.forEach(function (name) {
        exoskeleton.menu.addMenuItem("&Themes", name, "SetThemeEvent");
    });
    exoskeleton.menu.addMenu("&Help");
    exoskeleton.menu.addMenuItem("&Help", "&View local Exoskeleton.js help docs", "LocalHelpEvent");
    exoskeleton.menu.addMenuItem("&Help", "Browse Exoskeleton &GitHub page", "ShowGithubEvent");
    exoskeleton.menu.addMenuItem("&Help", "-");
    exoskeleton.menu.addMenuItem("&Help", "&About", "AboutEvent");

    exoskeleton.events.on("NewEvent", xo.newHandler);
    exoskeleton.events.on("OpenEvent", xo.openHandler);
    exoskeleton.events.on("SaveEvent", xo.saveHandler);

    exoskeleton.events.on("LocalHelpEvent", function () {
        exoskeleton.proc.start({
            FileName: "..\\exoskeleton.js\\docs\\index.html"
        });
    });
    exoskeleton.events.on("ShowGithubEvent", function () {
        exoskeleton.proc.start({
            FileName: "https://github.com/obeliskos/exoskeleton"
        });
    });
    exoskeleton.events.on("ExploreSamplesEvent", function () {
        var sampleDirectory = exoskeleton.file.combinePaths([
            xo.locations.Current,
            "appdata"
        ]);

        exoskeleton.proc.start({
            FileName: "explorer.exe",
            Arguments: sampleDirectory
        });
    });

    exoskeleton.events.on("ExitEvent", function () {
        exoskeleton.shutdown();
    });

    exoskeleton.events.on("AboutEvent", function () {
        var msg = "This application was written entirely using html/javascript + exoskeleton.  " +
            "It's dual purpose is to act not only as example itself, but an ide for loading existing example " +
            "exoskeleton.js scripts and creating your own new scripts.  You might then include them in a real " +
            "exoskeleton application, separate from this interactive console app.";

        exoskeleton.main.showMessageBox(msg, "About Exoskeleton Interactive Console", "OK", "Information");
    });

    exoskeleton.events.on("SetThemeEvent", function (menuText) {
        if (localStorage) {
            localStorage["xo.editorTheme"] = menuText;
        }

        xo.applyTheme(menuText);
    });
};  

xo.initializeToolbar = function () {
    var imagesPath = exoskeleton.file.combinePaths([
        xo.locations.Current,
        "images"
    ]);

    var newIconPath = exoskeleton.file.combinePaths([imagesPath,"new.png"]);
    var openIconPath = exoskeleton.file.combinePaths([imagesPath, "open.png"]);
    var exitIconPath = exoskeleton.file.combinePaths([imagesPath,"door_exit.png"]);
    var fullscreenIconPath = exoskeleton.file.combinePaths([imagesPath,"fullscreen.png"]);
    var runIconPath = exoskeleton.file.combinePaths([imagesPath,"run.png"]);
    var saveIconPath = exoskeleton.file.combinePaths([imagesPath,"save.png"]);
    var saveAsIconPath = exoskeleton.file.combinePaths([imagesPath,"saveas.png"]);
    var helpIconPath = exoskeleton.file.combinePaths([imagesPath,"help.png"]);

    exoskeleton.toolbar.initialize();
    exoskeleton.toolbar.addButton("New", "NewScriptEvent", newIconPath);
    exoskeleton.toolbar.addButton("Open", "OpenEvent", openIconPath);
    exoskeleton.toolbar.addSeparator();
    exoskeleton.toolbar.addButton("Save", "SaveEvent", saveIconPath);
    exoskeleton.toolbar.addButton("Save as...", "SaveAsScriptEvent", saveAsIconPath);
    exoskeleton.toolbar.addSeparator();
    exoskeleton.toolbar.addSeparator();
    exoskeleton.toolbar.addButton("Run", "RunScriptEvent", runIconPath);
    exoskeleton.toolbar.addButton("Toggle Fullscreen", "ToggleFullscreenEvent", fullscreenIconPath);
    exoskeleton.toolbar.addSeparator();
    exoskeleton.toolbar.addButton("Help", "LocalHelpEvent", helpIconPath);
    exoskeleton.toolbar.addSeparator();
    exoskeleton.toolbar.addButton("Exit", "ExitEvent", exitIconPath);

    exoskeleton.events.on("ToggleFullscreenEvent", function () {
        exoskeleton.main.toggleFullscreen();
    });
    exoskeleton.events.on("RunScriptEvent", xo.runScript);
    exoskeleton.events.on("NewScriptEvent", xo.newHandler);
    exoskeleton.events.on("SaveAsScriptEvent", function () {
        // emulate menu save as click
        xo.saveHandler("Save &as...");
    });

};

xo.initializeStatusbar = function () {
    exoskeleton.statusbar.initialize();
    exoskeleton.statusbar.setLeftLabel("Welcome to Exoskeleton Interactive Console!");

    var now = new Date();
    var formattedDate = exoskeleton.main.formatUnixDate(now, "MM/dd/yy  hh:mm:ss tt");
    exoskeleton.statusbar.setRightLabel("Started : " + formattedDate);
};

xo.newHandler = function () {
    xo.currentFilepath = "";
    exoskeleton.main.setWindowTitle(xo.settings.WindowTitle + " - (new)");
    xo.editorJS.setValue("");
};

xo.openHandler = function () {
    var sampleDirectory = exoskeleton.file.combinePaths([
        xo.locations.Current,
        "appdata"
    ]);

    var result = exoskeleton.main.showOpenFileDialog({
        InitialDirectory: sampleDirectory,
        Filter: "exo files (*.exo)|*.exo|javascript files (*.js)|*.js|All files (*.*)|*.*"
    });

    if (result) {
        xo.currentFilepath = result.FileName;
        var shortFilename = exoskeleton.file.getFileName(result.FileName);

        // Update window caption
        exoskeleton.main.setWindowTitle(xo.settings.WindowTitle + " - " + shortFilename);
        // load script text from disk into var
        var content = exoskeleton.file.loadFile(xo.currentFilepath);
        // apply script text from var into codemirror editor
        xo.editorJS.setValue(content);

    }
};

xo.saveHandler = function (data) {
    var saveFilename = "";
    var sampleDirectory = exoskeleton.file.combinePaths([
        xo.locations.Current,
        "appdata"
    ]);

    data = data.replace("&", "");

    // if saving to existing filename, just remember filename
    if (data === "Save" && xo.currentFilepath !== "") {
        saveFilename = xo.currentFilepath;
    }

    // if 'saving as' or no known existing filename, ask user to pick filename
    if (data !== "Save" || xo.currentFilepath === "") {
        var dialogValues = exoskeleton.main.showSaveFileDialog({
            Title: "Pick data file to save to",
            InitialDirectory: sampleDirectory,
            Filter: "exo files (*.exo)|*.exo|javascript files (*.js)|*.js|All files (*.*)|*.*"
        });

        if (dialogValues) {
            saveFilename = dialogValues.FileName;
        }
    }

    // if we picked a filename or assumed current one, save file
    if (saveFilename) {
        xo.currentFilepath = saveFilename;

        var shortFilename = exoskeleton.file.getFileName(saveFilename);
        var content = xo.editorJS.getValue();

        exoskeleton.file.saveFile(saveFilename, content);

        var now = new Date();
        var formattedDate = exoskeleton.main.formatUnixDate(now, "hh:mm:ss tt");
        exoskeleton.statusbar.setLeftLabel("Saved " + shortFilename + " (" + formattedDate + ")");

        exoskeleton.main.setWindowTitle(xo.settings.WindowTitle + " - " + shortFilename);
    }
};

xo.windowResize = function () {
    setTimeout(function () {
        try {
            xo.editorJS.setSize("100%", $(window).height() - 180);
            xo.editorOutput.setSize("100%", $(window).height() - 180);
        }
        catch (e) {}
    }, 100);

    $("#divInspect").height($(window).height() - 250);
}

xo.tabMode = function(mode) {
    $("#divTextOutput").hide();
    $("#divSaveLoad").hide();
    $("#divAbout").hide();

    switch(mode) {
        case 1: $("#divTextOutput").show();
            break;
        case 2: $("#divSaveLoad").show();
            xo.refreshScriptList();
            break;
        case 3: $("#divAbout").show();
            break;
    }
}

xo.refreshScriptList = function () {
    var settings = exoskeleton.system.getSystemInfo();
    var appdata = exoskeleton.file.combinePaths([
        settings.HostedRoot,
        "appdata"
    ]);
    var files = exoskeleton.file.getFiles(appdata, "*.exo");

    $("#select-savedscripts").empty();

    files.forEach(function (filename) {
        $('#select-savedscripts').append($('<option>', {
            value: filename,
            text: exoskeleton.file.getFileName(filename)
        }));
    });
}

xo.fileSelected = function () {
    var filename = $("#select-savedscripts option:selected").text();
}

xo.newScript = function () {
    xo.editorJS.setValue("// my script description");
    $("#input-filename").val("myscript.exo");
};

xo.loadScript = function () {
    var fullpath = $("#select-savedscripts option:selected").val();
    var filename = $("#select-savedscripts option:selected").text();

    $("#input-filename").val(filename);

    var content = exoskeleton.file.loadFile(fullpath);
    xo.editorJS.setValue(content);

};

xo.saveScript = function () {
    var filename = $("#input-filename").val();

    if (filename === "") {
        exoskeleton.main.showMessageBox("You are required to enter a filename.", "Exoskeleton Interactive Console", "OK", "Warning");
        return;
    }

    var settings = exoskeleton.system.getSystemInfo();
    var newfilename = exoskeleton.file.combinePaths([
        settings.HostedRoot,
        "appdata",
        filename
    ]);

    var fi = exoskeleton.file.getFileInfo(newfilename);
    if (fi.Exists) {
        var dr = exoskeleton.main.showMessageBox(
            "Do you want to overwrite this file",
            "File already exists", "OKCancel", "Question");

        if (dr !== "OK") return;
    }

    var content = xo.editorJS.getValue();

    exoskeleton.file.saveFile(newfilename, content);

    xo.refreshScriptList();

    exoskeleton.main.showMessageBox("Saved " + filename, "Exoskeleton Interactive Console", "OK", "Information");
};

xo.setupEditor = function (theme) {
    theme = theme || "abcdef";

    xo.editorJS = CodeMirror.fromTextArea(document.getElementById("jsedit"), {
        smartIndent: false,
        lineNumbers: true,
        theme: theme,
        mode: "javascript",
        foldGutter: true,
        gutters: ["CodeMirror-linenumbers", "CodeMirror-foldgutter"],
        extraKeys: {
            "Ctrl-Q": function (cm) {
                cm.foldCode(cm.getCursor());
            },
            "F11": function (cm) {
                cm.setOption("fullScreen", !cm.getOption("fullScreen"));
            },
            "Ctrl-Space": "autocomplete",
            "Esc": function (cm) {
                if (cm.getOption("fullScreen")) cm.setOption("fullScreen", false);
            }
        }
    });

    xo.editorOutput = CodeMirror.fromTextArea(document.getElementById("lsoutput"), {
        smartIndent: false,
        lineNumbers: true,
        theme: theme,
        mode: "javascript",
        foldGutter: true,
        gutters: ["CodeMirror-linenumbers", "CodeMirror-foldgutter"],
        extraKeys: {
            "Ctrl-Q": function (cm) {
                cm.foldCode(cm.getCursor());
            },
            "F11": function (cm) {
                cm.setOption("fullScreen", !cm.getOption("fullScreen"));
            },
            "Esc": function (cm) {
                if (cm.getOption("fullScreen")) cm.setOption("fullScreen", false);
            }
        }
    });

    var initCode = "// Welcome to the exoskeleton interactive console\r\n";
    initCode += "// For more examples click on the save/load button at the top right.\r\n\r\n";
    initCode += "// let's get system info!\r\n";
    initCode += "var info = exoskeleton.system.getSystemInfo();\r\n\r\n";
    initCode += "// you can use the xo object to log to the output panel\r\n";
    initCode += "xo.logObject(info)\r\n";

    xo.editorJS.setValue(initCode);
    xo.editorOutput.setValue("Click 'Run' to launch your script.");
};

xo.logText = function(message) {
    var oldText = xo.editorOutput.getValue();
    oldText += message + "\r\n";
    xo.editorOutput.setValue(oldText);
};

// Dump object as serialized json to output
xo.logObject = function (obj, isPretty) {
    if (typeof (isPretty) === 'undefined') {
        isPretty = true;
    }

    var oldText = xo.editorOutput.getValue();
    oldText += (isPretty) ? (JSON.stringify(obj, undefined, 2) + "\r\n") : (JSON.stringify(obj) + "\r\n");

    xo.editorOutput.setValue(oldText);
};

xo.runScript = function () {
    $("#divCode").empty();
    xo.editorOutput.setValue("");

    var s = document.createElement("script");
    var ls = xo.editorJS.getValue();

    s.innerHTML = ls;

    xo.tabMode(1);

    // give dom a chance to clean out by waiting a bit?
    setTimeout(function () {
        document.getElementById("divCode").appendChild(s);
    }, 250);

};

xo.errorHandler = function (evt) {
    alertify.error('load error');
};

xo.toggleVisibility = function (id) {
    var e = document.getElementById(id);
    e.style.display = (e.style.display === 'block') ? 'none' : 'block';
};

xo.applyTheme = function (theme) {
    xo.editorJS.setOption("theme", theme);
    xo.editorOutput.setOption("theme", theme);
};

xo.selectTheme = function () {
    var theme = $("#select-theme option:selected").val();
    if (localStorage) {
        localStorage["xo.editorTheme"] = theme;
    }

    xo.applyTheme(theme);
};
