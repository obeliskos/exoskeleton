var xo = {
    editorJS: null,
    editorOutput: null,
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
    xo.setupEditor(theme);

    setTimeout(xo.windowResize, 100);
});
    
xo.windowResize = function () {
    setTimeout(function () {
        try {
            xo.editorJS.setSize("100%", $(window).height() - 270);
            xo.editorOutput.setSize("100%", $(window).height() - 270);
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
