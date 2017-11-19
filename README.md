# Exoskeleton
A .net, windows-specific native web app hosting framework.

# Overview
This is an experiment creating a windows native app hosting framework implemented in .net.  This application can host and 
expose an api for your html and javascript apps to invoke via javascript object which maps to a 'Com-Visible' C# object hierarchy.
Exoskeleton apps run in 'WebUi', 'NativeUi', or 'MixedUi' (combination of both).  

'WebUi' mode apps use the .NET web browser as a content area for your 'exoskeleton-enhanced' web pages. The .net web browser mode 
can only use the internet explorer engine and this limited to IE 11, which exoskeleton uses.  More notable perhaps is the limitation 
to ECMA 5 javascript engine.  If you enlist tranpilation toolchains to develop your javascript using the latest standards, you 
should target ecma 5 for the rendered javascript.

'NativeUi' mode apps hide the webbrowser but use its 'exoskeleton-enhanced' javascript to allow it populate the host windows form 
with .net controls.  Most controls support at least one event which it will unicast to your javascript exoskeleton event emitter.
You can therefore listen for those events from javascript and implement your event handlers which may, in turn, update the .net ui 
controls via exoskeleton.js.

'MixedUi' mode apps are primary and initially 'NativeUi' apps.  You set them up like normal native ui mode apps but you ensure your 
form definition has a panel defined who's sole purpose is to host the web browser.  Once your application initializes, applies the 
form definition and shows itself, you then can call exoskeleton.main.switchToMixedUi(panelName).

Even with 'WebUi' mode apps, you can enlist the 'form' and 'dialog' API classes to create .net popup windows and dialogs, but those 
popups do not support menus, toolbars, or statusbars.  Menus, toolbars, and statusbars exist only on the hostwindow itself which can 
be either 'WebUi' or 'NativeUi'.

This application can serve your app from : 
- the filesystem
- an exoskeleton self-hosted static web server, or
- an external web server (like node.js)

Exoskeleton.js functionality embedded within your javascript can run in any of the above methods when those pages are 
hosted in the exoskeleton container and permissions are configured to allow. 

The functionality available to your javascript apps roughly maps to a selection of .net api methods and custom defined methods 
The above list will be enhance over time, where needed.  You can browse the current state of the API by viewing the documentation.js docs here :

[Exoskeleton.js API Docs](https://rawgit.com/obeliskos/exoskeleton/master/Examples/exoskeleton.js/docs/index.html)

The API documented above allows configuring menus, toolbars, and statusbars on the container, creating winforms dialogs, 
forms, native dialogs (FontDialog, ColorDialog, OpenFileDialog, SaveFileDialog), system tray notifications, along with a 
wide variety of .net library calls categorized by category or namespace.

# Definitions
Exoskeleton Dialog and Form layout can be programmatically defined by adding one control at a time, but the recommended method for 
defining and laying out those .net controls is via 'definitions'. Definitions are writtent in JSON and define a set of controls, their 
properties, their visual nesting, and other payloads within a single object.  You can supply these via json objects defined in your 
javascript or stored in external files for reusability and separation of logic.

The typical initialization of a windows form generally may follow this process :
- initialize - clears the (named) form or dialog controls, as well as the .net side 'host' dictionary references for those controls.
- applyDefinition - converts your definition to .net control instantiations, visual placement, event handler wireup.
- applyFormProperties - used to update the 'values' or settings of controls dynamically. while this may be done via definition, you 
will likely want to separate this as definition may be needed once while setting their properties all at one may be done many times.
- applyControlProperties - like applyFormProperties, except for a single control.  Sometimes you will just need to update a single control.
- applyControlPayload - Some functionality must be provided via exoskeleton-specific methods which are driven off JSON 'payloads'. 
Currently payloads are only used for edge cases such as databinding ListBox and ComboBox, binding DataGridView, PictureBox filename,  
CheckListBox 'CheckedIndices'. Other functionality may be provided via payloads if they cannot be implemented via applying properties.

# Building
This application was built using Visual Studio 2017 Community.  A pre-built binary is included in the 'Prebuilt' folder if you just want to experiment.

# App Configuration
By default when you run exoskeleton (with no command-line arguments) we will look for a 'settings.xos' file in the current working directory 
and, if none exists, we will create a default one which you can customize.  This settings file allow you to specify how your 
application is to be configured and what permissions are to be made available to your javascript.  For debugging you might set the 
'ScriptingLoggerEnabled' option which will load a logger/console for each host window which can be used for diagnosing errors.

If self-hosting, we will also look for an existing 'mappings.xml' file 
in the same directory as the executable or create a default one if none exists.  The mappings.xml file is not application specific and contains 
xml-configurable mime type mapping cross references... should you need to serve up custom file types you can modify this file.

You may create multiple named settings files (for examples see the 'Examples' folder) and pass the name or path to that settings file as a command line 
argument to the exoskeleton binary.

```
exoskeleton myapp.xos
```

# Interactive Console and Examples
Exoskeleton provides several sample projects in the 'Examples' folder, one of which is an interactive console which 
can be used for learning and prototyping javascript use of the exoskeleton libraries and viewing their return values.
You should be able to run the examples by clicking on the batch files in the examples folder.

[logo]: https://github.com/obeliskos/exoskeleton/raw/master/images/console.png
![Screenshot](https://github.com/obeliskos/exoskeleton/raw/master/images/console.png)
