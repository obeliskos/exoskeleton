/**
 * Radio - Exoskeleton example for 'MixedUi' app.
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

// event handler to bootstrap after scripts are loaded
window.addEventListener("load", function load(event) {
    window.removeEventListener("load", load, false);

    // The following delay is long enough that it will not fire twice
    // if you have the 'WebBrowserRefreshOnFirstLoad' set to true.
    setTimeout(initializeApp, 100);
},false);

// global vars
var db, defpath;
var AnchorStyles = exoskeleton.enums.AnchorStyles;
var locations = exoskeleton.getLocations();
var imagesPath = exoskeleton.file.combinePaths([
    locations.Current,
    "images"
]);
var newIconPath = exoskeleton.file.combinePaths([imagesPath, "new.png"]);
var deleteIconPath = exoskeleton.file.combinePaths([imagesPath, "delete.png"]);
var editIconPath = exoskeleton.file.combinePaths([imagesPath, "edit.png"]);
var exitIconPath = exoskeleton.file.combinePaths([imagesPath, "exit.png"]);

var selectedStationId = 0;
var playOnSelect = true;

/**
 * Loads a javascript JSON database and handles saving changes.
 * If the database does not yet exist on disk, we will initialize a new default one.
 */
function initializeDatabase() {
    var stations = db.getCollection('stations');

    if (stations === null) {
        stations = db.addCollection('stations');

        // initialize some default stations if running for first time
        stations.insert([
            {
                "name": "North Sea Surf Radio",
                "url": "http://5.135.142.83:8426/;"
            },
            {
                "name": "Matt's Movie Trax",
                "url": "http://37.59.38.180:11732/;"
            },
            {
                "name": "Another Music Project (IDM Electronic)",
                "url": "http://radio.anothermusicproject.com:8000/idm"
            },
            {
                "name": "Streaming Soundtracks",
                "url": "http://162.213.197.54:80/;"
            },
            {
                "name": "Chroma Nature Sounds",
                "url": "http://148.251.184.14:8024/;"
            },
            {
                "name": "Radio Rivendale",
                "url": "http://rivendell.byte-storm.com:8000/128kbit.mp3"
            },
            {
                "name": "Soma Underground 80s",
                "url": "http://ice1.somafm.com/u80s-128-mp3"
            },
            {
                "name": "Soma FM Groove Salad",
                "url": "http://ice1.somafm.com/groovesalad-128-mp3"
            },
            {
                "name": "Soma FM Secret Agent",
                "url": "http://ice1.somafm.com/secretagent-128-mp3"
            },
            {
                "name": "Soma FM DefCon",
                "url": "http://ice1.somafm.com/defcon-128-mp3"
            },
            {
                "name": "Soma FM Suburbs of Goa",
                "url": "http://ice1.somafm.com/suburbsofgoa-128-mp3"
            },
            {
                "name": "Soma FM Space Station",
                "url": "http://ice1.somafm.com/spacestation-128-mp3"
            },
            {
                "name": "Soma FM Dubstep",
                "url": "http://ice1.somafm.com/dubstep-128-mp3"
            },
            {
                "name": "Soma FM Earwaves",
                "url": "http://ice1.somafm.com/earwaves-128-mp3"
            },
            {
                "name": "Soma FM Silent",
                "url": "http://ice1.somafm.com/silent-128-mp3"
            },
            {
                "name": "Soma FM DroneZone",
                "url": "http://ice1.somafm.com/dronezone-128-mp3"
            },
            {
                "name": "WFYI HD1 NPR News",
                "url": "http://wfyi-iad.streamguys1.com:80/live"
            },
            {
                "name": "Best of Art Bell",
                "url": "http://stream1.u7radio.org:8000/;"
            },
            {
                "name": "No Agenda stream (Adam Curry & John C. Dvorak)",
                "url": "http://listen.noagendastream.com/noagenda"
            }
        ]);
    }

    exoskeleton.statusbar.setRightLabel(stations.count() + " stations.");

    exoskeleton.events.on("$host.StationList.SelectedIndexChanged", stationSelectedHandler);

    refreshStations();
}

function initializeApp() {
  var audio = document.getElementById("audioPlayer");
  audio.volume = 0.5;

  defpath = exoskeleton.file.combinePaths([locations.Current, "definitions", "MainForm.json"]);

  exoskeleton.form.initialize("$host", {
    Text: "Exoskeleton Radio - Mixed UI Example",
    Font: "Segoe UI, 14pt, style=Bold",
    Width: 600,
    Height: 640
  });

  exoskeleton.form.loadDefinition("$host", defpath);
  
  exoskeleton.dialog.initialize("StationEditor", {
    Text: "Edit Station Information",
    Font: "Segoe UI, 14pt",
    Width: 620,
    Height: 300
  });

  exoskeleton.form.show("$host");

  // Pre-Load the Station Editor dialog definition (one time only)
  defpath = exoskeleton.file.combinePaths([locations.Current, "definitions", "StationEditor.json"]);
  exoskeleton.dialog.loadDefinition("StationEditor", defpath);

  exoskeleton.statusbar.initialize();
  exoskeleton.statusbar.setLeftLabel("Welcome to MixedUi radio example!");

  // If our app was served up from a webserver (selfhost or other), we could 
  // alternatively use the loki indexeddb adapter for storing in browser storage.
  db = new loki("station-database.json", {
      autoload: true,
      autoloadCallback: initializeDatabase,
      autosave: true,
      adapter: exoskeleton.keystore,
      serializationMethod: "pretty"
  });

  exoskeleton.events.on("multicast.shutdown", function () {
      db.close();
  });

  document.body.style = "zoom:100%";
  exoskeleton.main.switchToMixedUi("BottomPanel");

  initializeToolbar();
}

function initializeToolbar() {
  exoskeleton.toolbar.initialize();
  exoskeleton.toolbar.addButton("New", "$host.NewButton.Click", newIconPath);
  exoskeleton.toolbar.addButton("Edit", "$host.EditButton.Click", editIconPath);
  exoskeleton.toolbar.addSeparator();
  exoskeleton.toolbar.addButton("Delete", "$host.DeleteButton.Click", deleteIconPath);
  exoskeleton.toolbar.addSeparator();
  exoskeleton.toolbar.addButton("Exit", "ExitEvent", exitIconPath);

  exoskeleton.events.on("ExitEvent", function () {
      // our multicast.shutdown event will fire and save db if needed 
      exoskeleton.shutdown();
  });

  exoskeleton.events.on("$host.NewButton.Click", function () {
    exoskeleton.dialog.applyControlProperties("StationEditor", {
          IdTextBox: { Text: "" },
          NameTextBox: { Text: "" },
          StationUrlText: { Text: "" }
    });

    var dialogResult = exoskeleton.dialog.showDialog("StationEditor");
    if (dialogResult.Result === 'OK') {
        addStation(dialogResult.Controls);
    }
  });

  exoskeleton.events.on("$host.EditButton.Click", function () {
      if (!selectedStationId) return;

      var station = db.getCollection("stations").findOne({ $loki: selectedStationId });
      if (!station) {
          throw new Error("selectedStationId has invalid value :" + selectedStationId);
      }

      exoskeleton.dialog.applyControlProperties("StationEditor", {
          IdTextBox: { Text: station.$loki },
          NameTextBox: { Text: station.name },
          StationUrlText: { Text: station.url }
      });

      var dialogResult = exoskeleton.dialog.showDialog("StationEditor");
      if (dialogResult.Result === 'OK') {
          station.name = dialogResult.Controls.NameTextBox.Text;
          station.url = dialogResult.Controls.StationUrlText.Text;
          db.getCollection("stations").update(station);

          refreshStations();
      }
  });

  exoskeleton.events.on("$host.DeleteButton.Click", function () {
      if (!selectedStationId) return;

      var station = db.getCollection("stations").findOne({ $loki: selectedStationId });
      if (!station) {
          throw new Error("selectedStationId has invalid value :" + selectedStationId);
      }

      db.getCollection("stations").remove(station);

      refreshStations();
  });

  exoskeleton.events.on("$host.PlayOnSelectCheckBox.CheckedChanged", function (props) {
      playOnSelect = props.Checked;
  });
}

function addStation(controlValues) {
    var stations = db.getCollection("stations");

    stations.insert({
        name: controlValues.NameTextBox.Text,
        url: controlValues.StationUrlText.Text
    });

    refreshStations();
}

function refreshStations() {

    // create obj map of all contacts with { DisplayMember: name, ValueMember: $loki }
    var datasource = db.getCollection('stations').chain().map(function (obj) {
        return {
            Name: obj.name,
            Id: obj.$loki
        }
    }).data({ removeMeta: true });
    datasource.unshift({ Name: '', Id: -1 });

    // 'DataSource' is applied via payload
    exoskeleton.form.applyControlPayload("$host", "StationList", {
        DataSource: datasource
    });

}

// ContactList ListBox.SelectedIndexChanged handler
function stationSelectedHandler(data) {
    if (!data) return;
    if (data.Selected.Id === -1) return;

    selectedStationId = data.Selected.Id;

    if (!playOnSelect) return;

    var station = db.getCollection("stations").get(data.Selected.Id);

    loadStation(station.url);
}

function loadStation(url) {
    var audio = document.getElementById("audioPlayer");
    audio.src = url;
    audio.load();
    audio.play();
}

