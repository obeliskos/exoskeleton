/**
 * example.js
 *
 * This will be the main javascript source file for our database example.
 * 
 * We will establish app (window) initialization and app shutdown functionality,
 * setup database, and setup ui dom events.
 */
 
window.addEventListener("load", function load(event){
    window.removeEventListener("load", load, false);
    initializeApp();
},false);

var db;

var locations = exoskeleton.getLocations();
var defPath = exoskeleton.file.combinePaths([
    locations.Current,
    "definitions"
]);

function initializeApp() {
  // we will initialize our lokijs database instance with autoload and autosave logic,
  // storing database using exoskeleton file storage.  
  
  // If our app was served up from a webserver (selfhost or other), we could 
  // alternatively use the loki indexeddb adapter for storing in browser storage.
  db = new loki("database-example.json", {
    autoload: true,
    autoloadCallback: initializeDatabase,
    autosave: true,
    adapter: exoskeleton.keystore,
    serializationMethod: "pretty"
  });
  
  exoskeleton.events.on("multicast.shutdown", function() {
    db.close();
  });

  // let's initialize our 'UserEditor' dialog
  exoskeleton.dialog.initialize("UserEditor", {
      Text: "User Editor Dialog",
      Font: "Microsoft Sans Serif, 14pt",
      Width: 500,
      Height: 400
  });

  var userEditorDef = exoskeleton.file.combinePaths([
      defPath,
      "UserEditor.json"
  ]);

  exoskeleton.dialog.loadDefinition("UserEditor", userEditorDef);

}

function initializeDatabase() {
  var coll = db.getCollection('users');

  if (coll === null) {
    coll = db.addCollection('users');
    coll.insert({ name:'john', age: 44 });
    coll.insert({ name:'mary', age: 30 });
    coll.insert({ name:'luke', age: 28 });
    coll.insert({ name: 'lisa', age: 24 });
  }
  
  refreshUsers();
}

function userSelected() {
  var userId = parseInt($("#selUsers option:selected").val(), 10);

  // make server request for selected loki doc
  var results = db.getCollection("users").find({"$loki":userId});

  if (results.length === 0) return;

  // must have found it, so update form fields
  $("#lblUsersLoki").text(results[0].$loki);
  $("#txtUsersName").val(results[0].name);
  $("#txtUsersAge").val(results[0].age);
}

function editClicked() {
    var selectedId = $("#selUsers option:selected").val();
    if (!selectedId) return;

    var userId = parseInt($("#selUsers option:selected").val(), 10);

    var user = db.getCollection("users").get(userId);

    exoskeleton.dialog.applyControlProperties("UserEditor", {
        CaptionLabel: { Text: "Edit user information :" },
        IdTextBox: { Text: user.$loki },
        NameTextBox: { Text: user.name },
        AgeNumeric: { Value: user.age }
    });

    var dialogResult = exoskeleton.dialog.showDialog("UserEditor");

    if (dialogResult.Result === "OK") {
        user.name = dialogResult.Controls.NameTextBox.Text;
        user.age = dialogResult.Controls.AgeNumeric.Value;

        db.getCollection("users").update(user);

        refreshUsers();
    }
}

function newClicked() {
    exoskeleton.dialog.applyControlProperties("UserEditor", {
        CaptionLabel: { Text: "Add user information :" },
        IdTextBox: { Text: "" },
        NameTextBox: { Text: "" },
        AgeNumeric: { Value: 18 }
    });

    var dialogResult = exoskeleton.dialog.showDialog("UserEditor");

    if (dialogResult.Result === "OK") {
        var users = db.getCollection("users");

        users.insert({
            name: dialogResult.Controls.NameTextBox.Text,
            age: dialogResult.Controls.AgeNumeric.Value
        });

        refreshUsers();
    }
}

function deleteClicked() {
    var userId = parseInt($("#selUsers option:selected").val(), 10);

    var user = db.getCollection("users").get(userId);

    var result = exoskeleton.dialog.showMessageBox(
        "Are you sure you want to delete user '" + user.name + "'",
        "Delete User confirmation",
        "OKCancel",
        "Exclamation"
    );

    if (result !== "OK") return;

    db.getCollection("users").remove(userId);

    refreshUsers();
}

function refreshUsers() {
  $("#selUsers").empty();
  var result = db.getCollection("users").find();
  result.forEach(function(obj){
      $("#selUsers").append($('<option>', {
          value: obj.$loki,
          text: obj.name
      }));
  });
}
