@echo off

if exist ..\bin\Debug\Exoskeleton.exe (
  echo Using 'Debug' build exoskeleton binary...
  start ..\bin\Debug\Exoskeleton.exe database-example.xml
) else (
  if exist ..\bin\Release\Exoskeleton.exe (
    echo Using 'Release' build exoskeleton binary...
    start ..\bin\Release\Exoskeleton.exe database-example.xml
  ) else (
    echo Using 'Prebuilt' build exoskeleton binary...
    start ..\Prebuilt\Exoskeleton.exe database-example.xml
  )
)
