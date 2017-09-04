@echo off

REM Mixing up a bit by putting settings file in 'app' directory
REM Because of that (and the configuration of that in settings.xos),
REM the relative paths will be relative to the markdown-viewer folder up to root readme.md

REM This might be an example i wish to work out how to associate with MD files.

if exist ..\bin\Debug\Exoskeleton.exe (
  echo Using 'Debug' build exoskeleton binary...
  start ..\bin\Debug\Exoskeleton.exe markdown-viewer\settings.xos ..\..\readme.md
) else (
  if exist ..\bin\Release\Exoskeleton.exe (
    echo Using 'Release' build exoskeleton binary...
    start ..\bin\Release\Exoskeleton.exe markdown-viewer\settings.xos ..\..\readme.md
  ) else (
    echo Using 'Prebuilt' build exoskeleton binary...
    start ..\Prebuilt\Exoskeleton.exe markdown-viewer\settings.xos ..\..\readme.md
  )
)
