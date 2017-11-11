/**
 * Contacts Database - Exoskeleton example for 'NativeUiOnly' app.
 *
 * NativeUiOnly do not display the host web browser within any of the host windows.
 * The host web browser is hidden and used only for loading mostly blank webpages which
 * register your javascript and invoke exoskeleton.form methods.  Since our host windows
 * are now available for layout, we use the formName '$host' to designate we want to affect
 * the host window itself and not a new form.
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
var saveIconPath = exoskeleton.file.combinePaths([imagesPath, "save.png"]);

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
            notes: ''
        });
        contacts.insert({
            name: 'luke',
            contactType: 'Friend',
            phoneNumber: '555-1214',
            email: 'luke@yahoo.com',
            notes: ''
        });
        contacts.insert({
            name: 'lisa',
            contactType: 'Family',
            phoneNumber: '555-1215',
            email: 'lisa@outlook.com',
            notes: ''
        });
    }

    var contactTypes = db.getCollection('contact types');

    if (contactTypes === null) {
        contactTypes = db.addCollection('contact types');
        contactTypes.insert({ name: 'Friend' });
        contactTypes.insert({ name: 'Family' });
        contactTypes.insert({ name: 'School' });
        contactTypes.insert({ name: 'Church' });
        contactTypes.insert({ name: 'Personal Associate' });
        contactTypes.insert({ name: 'Business Associate' });
    }

    exoskeleton.statusbar.setRightLabel(contacts.count() + " contacts.");

    refreshContactTypes();
    exoskeleton.events.on("$host.ContactList.SelectedIndexChanged", contactSelectedHandler);
    refreshContacts();
}

function initializeApp() {
  defpath = exoskeleton.file.combinePaths([locations.Current, "definitions", "MainForm.json"]);

  exoskeleton.form.initialize("$host", {
    Text: "Contacts - an Exoskeleton example 'NativeUI' application",
    Font: "Segoe UI, 12pt, style=Bold",
    Width: 960,
    Height: 640
  });

  exoskeleton.form.loadDefinition("$host", defpath);
  
  exoskeleton.form.applyControlProperties("$host", {
      SaveButton: { Anchor: AnchorStyles.Bottom | AnchorStyles.Right }
  });

  exoskeleton.form.show("$host");

  // initialize toolbar and (non-toolbar) button events (we will make them share events)
  initializeToolbar();

  exoskeleton.statusbar.initialize();
  exoskeleton.statusbar.setLeftLabel("Welcome to Contacts Database!");

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

  initializeMenu();
}

function initializeMenu() {
    exoskeleton.menu.initialize();
    exoskeleton.menu.addMenu("&File");
    exoskeleton.menu.addMenuItem("&File", "E&xit", "ExitEvent");
    exoskeleton.menu.addMenu("&Contact Types");
    exoskeleton.menu.addMenuItem("&Contact Types", "&Add...", "AddContactTypeEvent");
    exoskeleton.menu.addMenuItem("&Contact Types", "&Delete...", "DeleteContactTypeEvent");
    exoskeleton.menu.addMenu("&Help");
    exoskeleton.menu.addMenuItem("&Help", "&About", "AboutEvent", ["Control", "F1"]);

    exoskeleton.events.on("AddContactTypeEvent", addContactTypeHandler);
    exoskeleton.events.on("DeleteContactTypeEvent", deleteContactTypeHandler);

    exoskeleton.events.on("ExitEvent", function () {
        // our multicast.shutdown event will fire and save db if needed 
        // before host window closes
        exoskeleton.shutdown();
    });

    exoskeleton.events.on("AboutEvent", function () {
        var msg = "This application was written entirely using html/javascript + exoskeleton.  " +
            "This app functions as a 'NativeUI' example, where all UI components are native .NET controls " +
            "defined by JSON and scripted via javascript or native .NET or exoskeleton functionality. ";

        exoskeleton.dialog.showMessageBox(msg, "About Exoskeleton Interactive Console", "OK", "Information");
    });
}

function initializeToolbar() {
  exoskeleton.toolbar.initialize();
  exoskeleton.toolbar.addButton("New", "$host.NewButton.Click", newIconPath);
  exoskeleton.toolbar.addButton("Save", "$host.SaveButton.Click", saveIconPath);
  exoskeleton.toolbar.addSeparator();
  exoskeleton.toolbar.addButton("Delete", "$host.DeleteButton.Click", deleteIconPath);

  exoskeleton.events.on("$host.NewButton.Click", function () {
      newContact();
  });
  exoskeleton.events.on("$host.SaveButton.Click", function () {
      saveContact();
  });
  exoskeleton.events.on("$host.DeleteButton.Click", function () {
      deleteContact();
  });
}

function addContactTypeHandler() {
    var contactName = exoskeleton.dialog.promptInput(
        "Add new Contact Type",
        "Enter contact type name",
        ""
    );

    if (contactName !== null) {
        db.getCollection('contact types').insert({
            name: contactName
        });

        refreshContactTypes();
    }
}

function deleteContactTypeHandler() {
    var contactTypeCollection = db.getCollection('contact types');

    var listitems = contactTypeCollection.find().map(function (obj) {
        return obj.name;
    });

    // display listbox with no default selection and no multiselection
    var result = exoskeleton.dialog.promptList(
        "Delete Contact Type",
        "Select which contact type you want to delete :",
        listitems
    );

    // since no multiselect, result is a string and not array of strings
    if (result !== null) {
        var contactType = contactTypeCollection.findOne({ name: result });
        if (contactType != null) {
            contactTypeCollection.remove(contactType);
            refreshContactTypes();
        }
    }
}

function refreshContactTypes() {
    var datasource = db.getCollection('contact types').chain().map(function (obj) {
        return {
            Name: obj.name,
            Id: obj.$loki
        }
    }).data({ removeMeta: true });
    datasource.unshift({ Name: '', Id: 0 });

    exoskeleton.form.applyControlPayload("$host", "ContactTypeCombo", {
        DataSource: datasource,
        DataSourceKeepSelection: true
    });
}

function refreshContacts() {
    // create obj map of all contacts with { DisplayMember: name, ValueMember: $loki }
    var datasource = db.getCollection('contacts').chain().map(function (obj) {
        return {
            Name: obj.name,
            Id: obj.$loki
        }
    }).data({ removeMeta: true });

    // 'DataSource' is applied via payload
    exoskeleton.form.applyControlPayload("$host", "ContactList", {
        DataSource: datasource
    });

}

function newContact() {
    exoskeleton.form.applyControlProperties("$host", {
        IdTextBox: { Text: '' },
        NameTextBox: { Text: '' },
        PhoneNoText: { Text: '' },
        EmailText: { Text: '' },
        NotesText: { Text: '' },
        ContactTypeCombo: { SelectedItem: '' }
    });
}

function saveContact() {
    // need a bunch of control values so get everything via generateDynamicResponse()
    var controlProperties = exoskeleton.form.generateDynamicResponse("$host");

    var selectedId = controlProperties.IdTextBox.Text;
    var contact = {};
    if (selectedId !== "") {
        contact = db.getCollection("contacts").get(parseInt(selectedId, 10));
    }

    var refreshNeeded = false;
    if (selectedId === "") {
        // refresh if adding a contact
        refreshNeeded = true;
    }
    else {
        if (contact.name !== controlProperties.NameTextBox.Text) {
            // refresh if updating contact -and- contact name was changed
            refreshNeeded = true;
        }
    }

    contact.name = controlProperties.NameTextBox.Text;
    contact.contactType = controlProperties.ContactTypeCombo.Selected.Name;
    contact.phoneNumber = controlProperties.PhoneNoText.Text;
    contact.email = controlProperties.EmailText.Text;
    contact.notes = controlProperties.NotesText.Text;

    if (selectedId !== "") {
        db.getCollection("contacts").update(contact);
    }
    else {
        db.getCollection("contacts").insert(contact);
    }

    if (refreshNeeded) {
        refreshContacts();
    }
}

function deleteContact() {
    var contactListProps = exoskeleton.form.getControlProperties("$host", "ContactList");
    var contact = db.getCollection("contacts").get(contactListProps.Selected.Id);

    var result = exoskeleton.dialog.showMessageBox(
        "Are you sure you want to delete user '" + contact.name + "'",
        "Delete User confirmation",
        "OKCancel",
        "Exclamation"
    );

    if (result === "OK") {
        db.getCollection("contacts").remove(contact);
        refreshContacts();
    }
}

// ContactList ListBox.SelectedIndexChanged handler
function contactSelectedHandler(data) {
    if (!data) return;

    var contact = db.getCollection("contacts").get(data.Selected.Id);

    exoskeleton.form.applyControlProperties("$host", {
        IdTextBox: { Text: contact.$loki + '' },
        NameTextBox: { Text: contact.name },
        PhoneNoText: { Text: contact.phoneNumber },
        EmailText: { Text: contact.email },
        NotesText: { Text: contact.notes },
        ContactTypeCombo: { Text: contact.contactType }
    });

}