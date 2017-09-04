$(document).ready(function () {
    var converter = new showdown.Converter();
    var text, html;

    var systeminfo = exoskeleton.system.getSystemInfo();
    var filename = systeminfo.CommandLineArguments[systeminfo.CommandLineArguments.length - 1];
    var ext = exoskeleton.file.getExtension(filename).toLowerCase();

    if (exoskeleton.file.getExtension(filename).toLowerCase() === ".md") {
        text = exoskeleton.file.loadFile(filename);
        html = converter.makeHtml(text);
        document.getElementsByTagName("BODY")[0].innerHTML = html; 
    }
    else {
        var result = exoskeleton.main.showOpenFileDialog({
            InitialDirectory: systeminfo.HostedRoot,
            Filter: "md files (*.md)|*.md|All files (*.*)|*.*"
        });

        if (result) {
            text = exoskeleton.file.loadFile(result.FileName);
            html = converter.makeHtml(text);
            document.getElementsByTagName("BODY")[0].innerHTML = html; 
        }
    }    

});
