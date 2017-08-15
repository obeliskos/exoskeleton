# Exoskeleton
A .net, windows-specific native web app hosting framework

# Overview
This is an experiment creating a minimal, windows native app hosting framework.  It is implemented in .net and exposes an 
api for your html and javascript apps to invoke via javascript object which maps to a 'Com-Visible' C# object hierarchy.
This application can optionally serve your app from a self-hosted web server, from filesystem, or from external webserver with all methods 
able to utilize the exosketon scripting api 'client-side' to control the exoskeleton host.

Some functionality available to your apps include (each category can be individually enabled/disabled) :
- exoskeleton.main : set window title, enter and exit fullscreen mode, and display notifications.
- exoskeleton.file : object can be used to load and save files, read directories, get logical drives, etc.
- exoskeleton.proc :  launch external processes
- exoskeleton.media : currently used for text-to-speech functionality.

The above list will be enhance over time, where needed.

# Building
This application was build using Visual Studio 2017 Community.  A pre-built binary is included in the 'Prebuilt' folder if you just want to experiment.

# App Configuration
By default when you run exoskeleton (with no command-line arguments) we will look for a 'settings.xml' file in the current working directory 
and, if none exists, we will create a default one which you can customize.  If self-hosting, we will also look for an existing 'mappings.xml' file 
in the same directory as the executable or create a default one if none exists.  The mappings.xml file is not application specific and contains 
xml-configurable mime type mapping cross references... should you need to serve up custom file types you can modify this file.

You may create multiple named settings files (for examples see the 'Examples' folder) and pass the name or path to that settings file as a command line 
argument to the exoskeleton binary.

```
exoskeleton myapp.xml
```
