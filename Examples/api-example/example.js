window.addEventListener("load", function load(event){
    window.removeEventListener("load", load, false); //remove listener, no longer needed
    // override or edit this to be notified of shutdown;
    exoskeleton.events.on("multicast.shutdown", function() {
       exo.media.speakSync("example java script shutdown");
       return true;
    });
    // setup a listener in case they test eventing in 'open in new window' sample
    exoskeleton.events.on("multicast.TestEvent", function (data) {
        console.info("Received the 'multicast.TestEvent' event");
    });
},false);

function runFullscreen() {
    exoskeleton.main.fullscreen();
}

function runExitFullscreen() {
    exoskeleton.main.exitFullscreen();
}

function runSetWindowTitle() {
    var el = document.getElementById("txtTitle");

    exoskeleton.main.setWindowTitle(el.value);
}

function runShowNotification() {
    exoskeleton.main.showNotification("hello exo", "this is some notification raised by javascript");
}

function runOpenNewWindow() {
    exoskeleton.main.openNewWindow("Eventing sample child page / window", "/eventing.html", 800, 480);
}

function runSaySomething() {
    var el = document.getElementById("txtSpeechMessage");
    exoskeleton.media.speak(el.value);
}

function runLogInfo() {
    console.info("some info");
}

function runLogWarning() {
    console.warn("some warning");
}

function runLogError() {
    throw (new Error("some user exception"));
}

function runLogText() {
    exoskeleton.logger.logText("some text to send to logger console");
}

function runGetSystemInfo() {
    console.dir(exoskeleton.system.getSystemInfo());
}

function runGetDrives() {
    var result = exoskeleton.file.getLogicalDrives();
    var el = document.getElementById("selDrives");

    // ideally you would use jQuery or other helper libraries, we will just do basic javascript for demo and not even clear it first
    for (var i = 0; i < result.length; i++) {
        var opt = document.createElement('option');
        opt.value = result[i];
        opt.innerHTML = result[i];
        el.appendChild(opt);
    }
}

function runGetDriveInfo() {
    var result = exoskeleton.file.getDriveInfo();

    console.dir(result);
}

function runGetDirectories() {
    var el = document.getElementById("selRootDirectories");

    // get selected drive
    var driveEl = document.getElementById("selDrives");
    var driveString = driveEl.options[driveEl.selectedIndex].value;

    var result = exoskeleton.file.getDirectories(driveString);

    // ideally you would use jQuery or other helper libraries, we will just do basic javascript for demo and not even clear it first
    for (var i = 0; i < result.length; i++) {
        var opt = document.createElement('option');
        opt.value = result[i];
        opt.innerHTML = result[i];
        el.appendChild(opt);
    }
}

function runGetFiles() {
    var el = document.getElementById("selFiles");

    // get selected dir
    var dirEl = document.getElementById("selRootDirectories");
    var dirString = dirEl.options[dirEl.selectedIndex].value;

    var result = exoskeleton.file.getFiles(dirString, "*.*");

    // ideally you would use jQuery or other helper libraries, we will just do basic javascript for demo and not even clear it first
    for (var i = 0; i < result.length; i++) {
        var opt = document.createElement('option');
        opt.value = result[i];
        opt.innerHTML = result[i];
        el.appendChild(opt);
    }
}

function runGetCurrentDirectory() {
    var result = exoskeleton.file.getCurrentDirectory();
    var el = document.getElementById("txtCurrentDir");

    el.value = result;
}

function runSaveFile() {
    exoskeleton.file.saveTextFile("test.txt", "this is a test");
}

function runLoadFile() {
    var result = exoskeleton.file.loadTextFile("test.txt");

    if (result) {
        alert(result);
    }
    else {
        alert('file does not seem to exist yet, try the save button first?');
    }
}

function runProcStart() {
    exoskeleton.proc.start("calc.exe");
}

function runSetSessionKey() {
    var test = {
        a: 1,
        first: 'thor',
        items: ['mjolnir', 'beer']
    };

    exoskeleton.session.setObject("test", test);
}

function runGetSessionKey() {
    var result = exoskeleton.session.getObject("test");

    console.dir(result);
}