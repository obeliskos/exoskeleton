@echo off

REM Putting settings file in app directory as 'usual' practice

if exist ..\bin\Debug\Exoskeleton.exe (
  echo Using 'Debug' build exoskeleton binary...
  start ..\bin\Debug\Exoskeleton.exe webservice-example\settings.xos
) else (
  if exist ..\bin\Release\Exoskeleton.exe (
    echo Using 'Release' build exoskeleton binary...
    start ..\bin\Release\Exoskeleton.exe webservice-example\settings.xos
  ) else (
    echo Using 'Prebuilt' build exoskeleton binary...
    start ..\Prebuilt\Exoskeleton.exe webservice-example\settings.xos
  )
)
