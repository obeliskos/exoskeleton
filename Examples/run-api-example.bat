@echo off

if exist ..\bin\Debug\Exoskeleton.exe (
  echo Using 'Debug' build exoskeleton binary...
  start ..\bin\Debug\Exoskeleton.exe api-example.xos
) else (
  if exist ..\bin\Release\Exoskeleton.exe (
    echo Using 'Release' build exoskeleton binary...
    start ..\bin\Release\Exoskeleton.exe api-example.xos
  ) else (
    echo Using 'Prebuilt' build exoskeleton binary...
    start ..\Prebuilt\Exoskeleton.exe api-example.xos
  )
)
