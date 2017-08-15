window.exoskeletonShutdown = function() {
   exo.media.speakSync("example java script shutdown");
   return true;
}

window.addEventListener("load", function load(event){
    window.removeEventListener("load", load, false); //remove listener, no longer needed
    // override or edit this to be notified of shutdown;
},false);

function doFullscreen() {
    exoskeleton.main.fullscreen();
}

function doExitFullscreen() {
    exoskeleton.main.exitFullscreen();
}

function doSetWindowTitle() {
    var el = document.getElementById("txtTitle");

    exoskeleton.main.setWindowTitle(el.value);
}

function doShowNotification() {
    exoskeleton.main.showNotification("hello exo", "this is some notification raised by javascript");
}

function doSaySomething() {
    var el = document.getElementById("txtSpeechMessage");
    exoskeleton.media.speak(el.value);
}

function doGetDrives() {
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

function doGetDirectories() {
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

function doGetFiles() {
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

function doGetCurrentDirectory() {
    var result = exoskeleton.file.getCurrentDirectory();
    var el = document.getElementById("txtCurrentDir");

    el.value = result;
}

function doSaveFile() {
    exoskeleton.file.saveTextFile("test.txt", "this is a test");
}

function doLoadFile() {
    var result = exoskeleton.file.loadTextFile("test.txt");

    if (result) {
        alert(result);
    }
    else {
        alert('file does not seem to exist yet, try the save button first?');
    }
}

function doProcStart() {
    exoskeleton.proc.start("calc.exe");
}
