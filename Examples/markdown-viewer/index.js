$(document).ready(function () {
    var converter = new showdown.Converter();
    var text, html;

    // enable preset to more closely resemble how github renders
    converter.setFlavor('github');
    
    var systeminfo = exoskeleton.system.getSystemInfo();
    var locations = exoskeleton.getLocations();
    var filename = systeminfo.CommandLineArguments[systeminfo.CommandLineArguments.length - 1];
    var ext = exoskeleton.file.getExtension(filename).toLowerCase();

    if (exoskeleton.file.getExtension(filename).toLowerCase() === ".md") {
        text = exoskeleton.file.loadFile(filename);
        html = converter.makeHtml(text);
        document.getElementsByTagName("BODY")[0].innerHTML = html; 
    }
    else {
        var result = exoskeleton.dialog.showOpenFileDialog({
            InitialDirectory: locations.Current,
            Filter: "md files (*.md)|*.md|All files (*.*)|*.*"
        });

        if (result) {
            text = exoskeleton.file.loadFile(result.FileName);
            html = converter.makeHtml(text);
            document.getElementsByTagName("BODY")[0].innerHTML = html; 
        }
    }    

});
