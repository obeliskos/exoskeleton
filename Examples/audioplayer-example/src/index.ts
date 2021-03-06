/// <reference path="../definitions/exoskeleton.d.ts" />
/// <reference path="../node_modules/@types/lokijs/index.d.ts" />

/**
 * Audio Player - Exoskeleton example for treeview, listview and 'MixedUi' app.
 *
 * MixedUi apps are mostly and initially 'NativeUiApps' except they expect you to define
 * a panel to host the web browser within.  So in order to create a mixed ui app, you usually would :
 * - Set the 'DefaultToNativeUi' setting to 'true' in your xos file
 * - apply your main form ui definition as $host form, ensuring it includes a panel to host
 * - show main form
 * - call exoskeleton.main.switchToMixedUi(), passing the name of the panel already established
 *
 * One benefit of NativeUiOnly is that menus, toolbars, and statusbars are available on the host
 * windows, providing full native ui functionality in one place.  New forms or dialogs do not
 * support their own menus, toolbars, or status bars.
 */

declare var exoskeleton: Exoskeleton;
declare var loki = Loki;

class AudioPlayerApp {
    db?: Loki;
    defpath = "";
    audio: HTMLAudioElement | null | undefined;
    AnchorStyles = ExoskeletonAnchorStyles;
    locations = exoskeleton.getLocations();
    imagesPath: string = "";
    settingsIconPath: string = "";
    exitIconPath: string = "";
    imageListPaths : string[] = [];
    
    constructor() {
        // event handler to bootstrap after scripts are loaded
        window.addEventListener("load", (event) => {
            setTimeout(this.initializeApp, 100);
        },false);
    }

    initializeApp() {
        this.imagesPath = (!exoskeleton.file)?null:exoskeleton.file.combinePaths([
            this.locations.Current,
            "images"
        ]);

        this.settingsIconPath = (!exoskeleton.file)?null:exoskeleton.file.combinePaths([this.imagesPath, "settings_64.png"]);
        this.exitIconPath = (!exoskeleton.file)?null:exoskeleton.file.combinePaths([this.imagesPath, "exit.png"]);
        this.imageListPaths = (!exoskeleton.file)?null:exoskeleton.file.combinePathsArray(this.imagesPath, [
            "folder.ico",
            "speaker.ico"
        ]);
        
        // set up event handler on audio element which fires when song ends
        this.audio = document.getElementById("audioPlayer") as HTMLAudioElement;
        if (this.audio) {
            this.audio.volume = 0.5;
            this.audio.addEventListener('ended', playNextFile, false);
        }
    
        if (!exoskeleton.media || !exoskeleton.file || !exoskeleton.form || !exoskeleton.statusbar) {
            return;
        }

        exoskeleton.media.createImageList("ImageListSmall", {
            ImageSize: [32, 32],
            ColorDepth: 32
        });
        exoskeleton.media.loadImageList("ImageListSmall", this.imageListPaths);
    
        this.defpath = exoskeleton.file.combinePaths([this.locations.Current, "definitions", "MainForm.json"]);
    
        exoskeleton.form.initialize("$host", {
            Text: "Exoskeleton Audio Player",
            Font: "Segoe UI, 14pt, style=Bold",
            Width: 960,
            Height: 640
        });
    
        exoskeleton.form.loadDefinition("$host", this.defpath);
        exoskeleton.form.show("$host");
    
        exoskeleton.statusbar.initialize();
        exoskeleton.statusbar.setLeftLabel("Welcome to Exoskeleton audio player!");
    
        // If our app was served up from a webserver (selfhost or other), we could 
        // alternatively use the loki indexeddb adapter for storing in browser storage.
        this.db = new loki("player-settings.json", {
            autoload: true,
            autoloadCallback: initializeDatabase,
            autosave: true,
            adapter: exoskeleton.keystore,
            serializationMethod: "pretty"
        });
    
        exoskeleton.events.on("multicast.shutdown", () => {
            if (this.db) this.db.close();
        });
    
        exoskeleton.events.on("SourcesForm.SaveButton.Click", function () {
            if (exoskeleton.form) exoskeleton.form.close("SourcesForm");
            refreshSourcesList();
        });
    
        exoskeleton.events.on("$host.DirTreeView.AfterSelect", function (data) {
            refreshFileList(data.Tag);
            expandTreeNode(data);
        });
    
        exoskeleton.events.on("$host.FileListView.Click", fileClickHandler);
    
        exoskeleton.events.on("SettingsForm.SourcesListBox.SelectedIndexChanged", SettingsForm.listitemSelected);
        exoskeleton.events.on("SettingsForm.AddSourceButton.Click", SettingsForm.addSource);
        exoskeleton.events.on("SettingsForm.EditSourceButton.Click", SettingsForm.editSource);
        exoskeleton.events.on("SettingsForm.DeleteSourceButton.Click", SettingsForm.deleteSource);
        exoskeleton.events.on("SettingsForm.SaveSourceButton.Click", SettingsForm.saveSource);
        exoskeleton.events.on("SettingsForm.SaveButton.Click", SettingsForm.saveSettings);
        exoskeleton.events.on("SettingsForm.CancelButton.Click", SettingsForm.cancel);
    
        document.body.style.zoom = "100%";

        if (exoskeleton.main) exoskeleton.main.switchToMixedUi("AudioPanel");
    
        initializeToolbar();
    
        // If this was sent an audio file path on the url (via shell:sendto)
        if (exoskeleton.system) {
            var si = exoskeleton.system.getSystemInfo();
            if (si.CommandLineArguments.length > 1) {
                var filename = si.CommandLineArguments[si.CommandLineArguments.length - 1];
                var ext = exoskeleton.file.getExtension(filename).toLowerCase();
        
                if (exoskeleton.file.getExtension(filename).toLowerCase() === ".mp3") {
                    playFile(filename);
                }
                }
        }
    }

    initializeDatabase() {
        var sources = db.getCollection('sources');
    
        if (sources === null) {
            sources = db.addCollection('sources');
        }
    
        var settings = db.getCollection('settings');
        if (settings === null) {
            settings = db.addCollection('settings');
            // ie 11 audio element seems to support these extensions ok
            settings.insert({ name: "file-extensions", value: ".mp3,.mp4,.m4a" });
        }
    
        if (sources.count() === 0) {
            if (!exoskeleton.dialog || !exoskeleton.form) return;
    
            var dr = exoskeleton.dialog.showMessageBox(
              "You have no sources configured, do you wish to configure them now",
              "Missing music source locations",
              "YesNo",
              "Question"
            );
    
            if (dr == "Yes") {
                SettingsForm.show();
    
                exoskeleton.form.applyControlProperties("SettingsForm", {
                    SourceNameTextBox: { Text: "My Music (User)", Enabled: true },
                    SourceLocationTextBox: { Text: locations.Music, Enabled: true }
                });
            }
        }
        else {
            refreshSourcesList();
        }
    }
}

let app = new AudioPlayerApp();

/**
 * Loads a javascript JSON database and handles saving changes.
 * If the database does not yet exist on disk, we will initialize a new default one.
 */

function initializeToolbar() {
    exoskeleton.toolbar.initialize();
    exoskeleton.toolbar.addButton("New", "$host.SettingsButton.Click", settingsIconPath);
    exoskeleton.toolbar.addSeparator();
    exoskeleton.toolbar.addButton("Exit", "ExitEvent", exitIconPath);

    exoskeleton.events.on("ExitEvent", function () {
        // our multicast.shutdown event will fire and save db if needed 
        exoskeleton.shutdown();
    });

    exoskeleton.events.on("$host.SettingsButton.Click", SettingsForm.show);

    exoskeleton.events.on("$host.PlayOnSelectCheckBox.CheckedChanged", function (props) {
        playOnSelect = props.Checked;
    });
}

var SettingsForm = {
    selectedSourceId: null,
    show: function () {
        var settings = db.getCollection('settings');
        var fileExtensions = settings.findOne({ name: "file-extensions" });

        exoskeleton.form.initialize("SettingsForm", {
            Text: "Exoskeleton Audio Player - Settings",
            Font: "Segoe UI, 14pt, style=Bold",
            Width: 900,
            Height: 600
        });
        defpath = exoskeleton.file.combinePaths([locations.Current, "definitions", "SettingsForm.json"]);
        exoskeleton.form.loadDefinition("SettingsForm", defpath);

        SettingsForm.loadSourcesList();

        console.log("Extensions: " + fileExtensions.value);

        exoskeleton.form.applyControlProperties("SettingsForm", {
            ExtensionsTextBox: {
                Text: fileExtensions.value
            }
        });

        exoskeleton.form.show("SettingsForm");
    },
    loadSourcesList: function () {
        var sources = db.getCollection("sources").find();

        exoskeleton.form.applyControlPayload("SettingsForm", "SourcesListBox", {
            DataSource: sources
        });
        exoskeleton.form.applyControlProperties("SettingsForm", {
            SourcesListBox: {
                DisplayMember: "name",
                ValueMember: "location"
            }
        });

        refreshSourcesList();
        SettingsForm.disableEditing();
    },
    listitemSelected: function (data) {
        SettingsForm.selectedSourceId = data.Selected.$loki;

        exoskeleton.form.applyControlProperties("SettingsForm", {
            SourceNameTextBox: { Text: data.Selected.name },
            SourceLocationTextBox: { Text: data.Selected.location }
        });

        SettingsForm.disableEditing();
    },
    disableEditing: function () {
        exoskeleton.form.applyControlProperties("SettingsForm", {
            SourceNameTextBox: { Enabled: false },
            SourceLocationTextBox: { Enabled: false }
        });
    },
    enableEditing: function () {
        exoskeleton.form.applyControlProperties("SettingsForm", {
            SourceNameTextBox: { Enabled: true },
            SourceLocationTextBox: { Enabled: true }
        });
    },
    addSource: function () {
        SettingsForm.selectedSourceId = null;
        exoskeleton.form.applyControlProperties("SettingsForm", {
            SourceNameTextBox: { Text: "" },
            SourceLocationTextBox: { Text: "" }
        });

        SettingsForm.enableEditing();
    },
    editSource: function () {
        SettingsForm.enableEditing();
    },
    deleteSource: function () {
        if (!SettingsForm.selectedSourceId) {
            exoskeleton.dialog.showMessageBox(
              "You need to select a source to delete", "Unable to delete source", "OK", "Exclamation"
            );

            return;
        }

        db.getCollection("sources").remove(SettingsForm.selectedSourceId);
        SettingsForm.loadSourcesList();
    },
    saveSource: function () {
        var properties = exoskeleton.form.generateDynamicResponse("SettingsForm");
        var sources = db.getCollection("sources");

        if (SettingsForm.selectedSourceId) {
            var selectedSourceId = properties.SourcesListBox.Selected.$loki;
            var selectedSource = sources.get(selectedSourceId);
            if (selectedSource != null) {
                selectedSource.name = properties.SourceNameTextBox.Text;
                selectedSource.location = properties.SourceLocationTextBox.Text;
                sources.update(selectedSource);
            }
        }
        else {
            var loc = {
                name: properties.SourceNameTextBox.Text,
                location: properties.SourceLocationTextBox.Text
            };

            sources.insert(loc);
        }

        SettingsForm.loadSourcesList();
    },
    saveSettings: function () {
        var properties = exoskeleton.form.generateDynamicResponse("SettingsForm");

        var settings = db.getCollection("settings");
        var fileExtensions = settings.findOne({ name: "file-extensions" });

        fileExtensions.value = properties.ExtensionsTextBox.Text;
        settings.update(fileExtensions);

        exoskeleton.form.close("SettingsForm");
    },
    cancel: function () {
        exoskeleton.form.close("SettingsForm");
    }
};

// load or reload treeview from database
function refreshSourcesList() {
    var sources = db.getCollection('sources');
    var nodes = sources.find().map(function (loc) {
        return {
            Text: loc.name,
            Tag: loc.location
        }
    });

    var payload = {
        Nodes: nodes
    };

    exoskeleton.form.applyControlPayload("$host", "DirTreeView", payload);
}

// lazy load tree nodes on select
function expandTreeNode(node) {
    var dirNames = exoskeleton.file.getDirectories(node.Tag);
    var nodes = dirNames.map(function (loc) {
        return {
            Text: exoskeleton.file.getFileName(loc),
            Tag: loc
        }
    });

    nodes.sort(function (n1, n2) {
        if (n1.Text === n2.Text) return 0;
        if (n1.Text > n2.Text) return 1;
        return -1;
    });

    exoskeleton.form.applyControlPayload("$host", "DirTreeView", {
        GraftPath: {
            ParentPath: node.FullPaths,
            Nodes: nodes
        }
    });
}

// Displays files/songs in the selected treenode location
function refreshFileList(dirName) {
    allowNext = false;
    var files = exoskeleton.file.getFilesEndingWith(dirName, ".mp3,.mp4,.m4a");
    var items = [];

    files.forEach(function (file) {
        items.push({
            Text: exoskeleton.file.getFileName(file),
            ImageIndex: 1,
            Tag: file
        });
    });

    items.sort(function (f1, f2) {
        if (f1.Text === f2.Text) return 0;
        if (f1.Text > f2.Text) return 1;
        return -1;
    });
  
    controlProperties = { View: "Details" };
    controlPayload = {
        Columns: [{ Text: 'TrackName', Width: 560 }],
        Items: items
    };

    exoskeleton.form.applyControlProperties("$host", { FileListView: controlProperties });
    exoskeleton.form.applyControlPayload("$host", "FileListView", controlPayload);
}

// When a song is clicked this will play it in the audio element
function fileClickHandler(data) {
    allowNext = true;
    playFile(data.SelectedItems[0].Tag);
}

// when a song ends this event handler will be called to play the next song in the list
function playNextFile() {
    if (!allowNext) return;

    var fileListProperties = exoskeleton.form.getControlProperties("$host", "FileListView");
    var selectedIndex = fileListProperties.SelectedIndices[0];
    var itemCount = fileListProperties.ItemCount;

    if (++selectedIndex >= itemCount) return;

    exoskeleton.form.applyControlPayload("$host", "FileListView", { SelectedIndices: [selectedIndex] });

    fileListProperties = exoskeleton.form.getControlProperties("$host", "FileListView");
    playFile(fileListProperties.SelectedItems[0].Tag);
}

// Provides the html 5 audio element with a file name or url and begins playing it
function playFile(filename) {
    var basename = exoskeleton.file.getFileName(filename);

    exoskeleton.statusbar.setLeftLabel("Now Playing : " + basename);

    var uri = exoskeleton.file.filePathToUri(filename);

    var audio = document.getElementById("audioPlayer");
    audio.src = uri;
    audio.load();
    audio.play();
}
