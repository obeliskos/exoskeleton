/**
 * WebService example - Exoskeleton example for implementing javascript web services.
 *
 * Webservices in exoskeleton are implemented by registering javascript 'serviceProcessor' functions.
 * 
 * You need your settings configured to :
 * - Self host (WebServerSelfHost = true)
 * - Enable webservices (WebServerServicesEnabled = true)
 * - Configure WebServerServicesExtension setting to filter which requests will be sent to your
 *   web service process function.
 *
 * Requests for files ending with pattern defined in 'WebServerServicesExtension' ('.svc' for this example)
 * will be sent to any registered serviceProcessor functions until one of them returns a non-null response.
 * Your app can implement a single serviceProcessor function and manage all possible routing yourself,
 * or you can implement separate service processors to execute sequentially until a response is provided.
 */

var db;
var locations = exoskeleton.getLocations();

// page load event handler to bootstrap app initialization after scripts are loaded
window.addEventListener("load", function load(event) {
    window.removeEventListener("load", load, false);

    // The following delay is long enough that it will not fire twice
    // if you have the 'WebBrowserRefreshOnFirstLoad' set to true in your settings file.
    setTimeout(initializeApp, 100);
},false);

/**
 * Initializes our contacts database.
 * If the database did not exist on disk, we will seed it with collection(s) and data.
 * We will use this data to implement several of our webservices.
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

/**
 * Initialize this example application.  This will :
 * - Register serviceProcessor(s)
 * - Initialize our contacts database
 * - Add listener to 'multicast.shutdown' event to flush any changes to database
 */
function initializeApp() {

  // Here we will register our service processor function(s) with exoskeleton.
  // You will probably only need to implement one processor to handle all 
  // implemented webserice endpoints, but you can add several.  
  // If a processor does not provide a response (returns null), the request
  // will be passed to the next registered service processor.  
  exoskeleton.registerServiceProcessor(exampleServiceProcessor);
  exoskeleton.registerServiceProcessor(aboutServiceProcessor);

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

/**
 * Launches a client page outside of exoskeleton to prove the services are available to external systems or web pages.
 * While the web page we are launching is served up from exoskeleton webserver it has no connection to exoskeleton.js
 * or the exoskeleton COM containers.
 */
function launchClientPage() {
    // Warn user if necessary about port mismatch.
    // This might happen if, for instance you run multiple self hosting applications with the same base port.
    // In that case, when launching apps where that port is already in use, exoskeleton will use next available port as actual.
    if (locations.WebServerListenPort != locations.WebServerActualPort) {
        console.Warn("Webserver actually started on port " + locations.WebServerActualPort);
        console.log("Since the webserver started on a different port than 8080 ");
        console.log("The client page's ajax requests will not reach our service process function.");
        console.log("Consider setting the 'WebServerListenPort' to an unused port in your settings file.");
        console.log("You might also pass the actual port as a url parameter to client pages");
    }

    window.open("client-testpage.htm");
}

/**
 * The main serviceProcessor function for this example.
 * This service processor will handle requests for contact list and details.
 * @param {any} request - an exoskeleton request object
 * @returns {any} an exoskeleton response object or null if unhandled
 */
function exampleServiceProcessor(request) {

    // GetContactList - returns array of all contact names
    // This route (subdirectory) could be anything, like this convention.
    // This service has no params and will be called via HTTP GET.  
    // If we added params while leaving as HTTP GET those params would
    // be available under request.QueryParams object.
    if (request.AbsolutePath === "/Contacts/GetContactList.svc") {
        var results = db.getCollection("contacts").find().map(function (obj) { return obj.name });

        var response = {
            ContentType: "application/json",
            Response: JSON.stringify(results)
        }

        return response;
    }

    // GetContact - will be provided a contact name to look up and will return that object as json.
    // We can also just use the root location if we want, as long as the extension matches, we will 
    // receive the request for processing.
    if (request.AbsolutePath === "/GetContact.svc") {
        // We expect this will be a post (request.HasEntityBody === true) with post params,
        // so our expected 'name' param will be in request.BodyParams instead of request.QueryParams
        var contactName = request.BodyParams.name;

        var contact = db.getCollection("contacts").findOne({ name: contactName });

        var response = {
            ContentType: "application/json",
            Response: JSON.stringify(contact)
        }

        return response;
    }

    // The AbsolutePath requested must not have been one of our two endpoints, so indicate
    // that this was unhandled by this processor by returning null.
    return null;
}

/**
 * An additional service processor to demonstrate you can have more than one.
 * Arbitrarily this service processor implements a service returning html.
 * You might have a single processor and have it route all requests or
 * chain them by registering multiple processors.
 * @param {any} request
 */
function aboutServiceProcessor(request) {
    if (request.AbsolutePath === "/About/Us.svc") {
        var response = {
            ContentType: "text/html",
            Response: "<h3>About Us</h3><p>This is some html content about us which was retreived from an exoskeleton webservice.</p>"
        }

        return response;
    }
}
