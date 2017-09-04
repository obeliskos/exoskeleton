window.addEventListener("load", function load(event){
    window.removeEventListener("load", load, false); //remove listener, no longer needed
    // override or edit this to be notified of shutdown;
    exoskeleton.events.on("multicast.shutdown", function() {
       exoskeleton.media.speakSync("example java script shutdown");
       return true;
    });
    // setup a listener in case they test eventing in 'open in new window' sample
    exoskeleton.events.on("multicast.TestEvent", function (data1, data2) {
        console.info("Received the 'multicast.TestEvent' event", "data1:" + data1 + " data2:" + JSON.stringify(data2));
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

function runSaySomethingWithActiveX() {
    // the first param is the COM object name
    // the second param is the method on that object
    // the third param is array of arguments to method
    // (for speak method first param is message to speak and second is 1 for async or 0 for synchronous)
    exoskeleton.com.createAndInvokeMethod("SAPI.SpVoice", "Speak",
        ["this is a test message scripting activex from java script", 1]);
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

function runGetEnvironmentVariable() {
    var result = exoskeleton.system.getEnvironmentVariable("EXOVAR");

    if (!result) {
        alert("Variable not found, try clicking set variable to set it");
    }
    else {
        alert(result);
    }
}

function runGetEnvironmentVariables() {
    var resultJson = exoskeleton.system.getEnvironmentVariables();
    var environmentVars = JSON.parse(resultJson);

    var msg = Object.keys(environmentVars).length + " variables : " + "\n";

    Object.keys(environmentVars).forEach(function(keyname) {
        msg += keyname + " : " + environmentVars[keyname] + "\n";
    });

    alert(msg);
}

function runSetEnvironmentVariable() {
    exoskeleton.system.setEnvironmentVariable("EXOVAR", "Test Value");

    alert("variable set");
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

function runGetExecutableDirectory() {
    var result = exoskeleton.file.getExecutableDirectory();
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
    exoskeleton.proc.startPath("calc.exe");
}

function runProcStartInfo() {
    // you may provide any properties available on a c# ProcessStartInfo class instance
    var startInfo = {
        FileName : "mspaint.exe",
        Arguments : ".\\sitewrap-example\\robot.jpg"
    }

    var result = exoskeleton.proc.start(startInfo);

    // on success, null will be returned.  
    // on error, a string description of exception
    if (result) {
        alert(result);
    }
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