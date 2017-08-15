@echo off

if exist ..\bin\Debug\Exoskeleton.exe (
  echo Using 'Debug' build exoskeleton binary...
  ..\bin\Debug\Exoskeleton.exe api-example.xml
) else (
  if exist ..\bin\Release\Exoskeleton.exe (
    echo Using 'Release' build exoskeleton binary...
    ..\bin\Release\Exoskeleton.exe api-example.xml
  ) else (
    echo Using 'Prebuilt' build exoskeleton binary...
    ..\Prebuilt\Exoskeleton.exe api-example.xml
  )
)
