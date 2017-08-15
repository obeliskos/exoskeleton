/**
 * example.js
 *
 * This will be the main javascript source file for our database example.
 * 
 * We will establish app (window) initialization and app shutdown functionality,
 * setup database, and setup ui dom events.
 */
 
// exoskeleton will try to invoke this method when the app closes...
// we will use this to flush any pending database changes
window.exoskeletonShutdown = function() {
   shutdownApp();
   return true;
}

window.addEventListener("load", function load(event){
    window.removeEventListener("load", load, false);
    initializeApp();
},false);

var db;

function initializeApp() {
  // we will initialize our lokijs database instance with autoload and autosave logic,
  // storing database using exoskeleton file storage.  
  
  // If our app was served up from a webserver (selfhost or other), we could 
  // alternatively use the loki indexeddb adapter for storing in browser storage.
  db = new loki("./database-example/database-example.json", {
    autoload: true,
    autoloadCallback: initializeDatabase,
    autosave: true,
    adapter: ExoskeletonKeystoreAdapter 
  });
}

function initializeDatabase() {
  var coll = db.getCollection('users');

  if (coll === null) {
    coll = db.addCollection('users');
    coll.insert({name:'odin', age: 50});
    coll.insert({name:'thor', age: 30});
    coll.insert({name:'loki', age: 25});
  }
  
  refreshUsers();
}

/**
 * When your app is closed, exoskeleton will invoke the exeskeletonShutdown()
 * function above, which will call this method to tell the database to flush any changes to disk.
 */
function shutdownApp() {
  if (db) {
    db.close();
  }
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

function newClicked() {
  $("#lblUsersLoki").text("");
  $("#txtUsersName").val("");
  $("#txtUsersAge").val("");
}

function deleteClicked() {
  var userId = parseInt($("#selUsers option:selected").val(), 10);
  db.getCollection("users").remove(userId);
  refreshUsers();
}

function saveClicked() {
  var lokiId = $("#lblUsersLoki").text().trim();
  var name = $("#txtUsersName").val();
  var age = $("#txtUsersAge").val();
  var doc;
  var users = db.getCollection("users");

  if (lokiId === "") {
    users.insert({ name: name, age: age });
    refreshUsers();
  }
  else {
    var result = users.findOne({ $loki: parseInt(lokiId, 10)});
    doc = result;
    doc.name = name;
    doc.age = parseInt(age, 10);
    users.update(doc);
    refreshUsers();
  }
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
