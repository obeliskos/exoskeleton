/**
 * WebService example - Exoskeleton example for implementing javascript web services.
 *
 * Webservices in exoskeleton are implemented by invoking javascript hosted in your main window.
 * You need your settings configured to :
 * - Self host (WebServerSelfHost = true)
 * - Enable webservices (WebServerServicesEnabled = true)
 * - Configure WebServerServicesExtension setting to filter which requests will be sent to your
 *   web service process function.
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

/**
 * Loads a javascript JSON database and handles saving changes.
 * If the database does not yet exist on disk, we will initialize a new default one.
 */
function initializeDatabase() {
    var contacts = db.getCollection('contacts');

    if (contacts === null) {
        contacts = db.addCollection('contacts');
        contacts.insert({
            name: 'john',
            contactType: 'School',
            phoneNumber: '555-1212',
            email: 'john@gmail.com',
            notes: ''
        });
        contacts.insert({
            name: 'mary',
            contactType: 'Church',
            phoneNumber: '555-1213',
            email: 'may@aol.com',
            notes: 'needs to upgrade isp'
        });
        contacts.insert({
            name: 'luke',
            contactType: 'Friend',
            phoneNumber: '555-1214',
            email: 'luke@yahoo.com',
            notes: 'skype:ldhzrd'
        });
        contacts.insert({
            name: 'lisa',
            contactType: 'Family',
            phoneNumber: '555-1215',
            email: 'lisa@outlook.com',
            notes: 'needs to get unlimited data plan'
        });
        contacts.insert({
            name: 'george',
            contactType: 'School',
            phoneNumber: '555-1216',
            email: 'george@gmail.com',
            notes: 'runs meetups @12:30'
        })
    }
}

function initializeApp() {
  // If our app was served up from a webserver (selfhost or other), we could 
  // alternatively use the loki indexeddb adapter for storing in browser storage.
  db = new loki("contacts-database.json", {
      autoload: true,
      autoloadCallback: initializeDatabase,
      autosave: true,
      adapter: exoskeleton.keystore,
      serializationMethod: "pretty"
  });

  exoskeleton.events.on("multicast.shutdown", function () {
      db.close();
  });
}

var start;

function launchClientPage() {
    window.open("client-testpage.htm");
}

function exoskeletonProcessServiceRequest(request) {
    request = JSON.parse(request);

    // GetContactList - returns array of all contact names
    if (request.Filename === "GetContactList.svc") {
        var results = db.getCollection("contacts").find().map(function (obj) { return obj.name });

        var response = {
            ContentType: "application/json",
            Response: JSON.stringify(results)
        }

        return JSON.stringify(response);
    }

    // GetContact - will be provided a contact name to look up and will return that object as json.
    if (request.Filename === "GetContact.svc") {
        // We expect this will be a post, 
        // so our expected 'name' param will be in 'BodyParams' instead of 'QueryParams'
        var contactName = request.BodyParams.name;

        var contact = db.getCollection("contacts").findOne({ name: contactName });

        var responseText = JSON.stringify(contact);

        var response = {
            ContentType: "application/json",
            Response: responseText
        }

        return JSON.stringify(response);
    }

    console.error("Unknown service request.  Service requested : " + request.Filename);

    return null;
}