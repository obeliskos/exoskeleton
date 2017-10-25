# Exoskeleton
A .net, windows-specific native web app hosting framework.

# Overview
This is an experiment creating a windows native app hosting framework implemented in .net.  This application can host and 
expose an api for your html and javascript apps to invoke via javascript object which maps to a 'Com-Visible' C# object hierarchy.
Since this applicaton is written in C# and uses the .NET WebBrowser control, it should be noted that the underlying web 
control is based on Internet Explorer 11.

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


# Building
This application was built using Visual Studio 2017 Community.  A pre-built binary is included in the 'Prebuilt' folder if you just want to experiment.

# App Configuration
By default when you run exoskeleton (with no command-line arguments) we will look for a 'settings.xos' file in the current working directory 
and, if none exists, we will create a default one which you can customize.  If self-hosting, we will also look for an existing 'mappings.xml' file 
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
