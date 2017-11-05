REM Utility batch script so i can edit exoskeleton.js in its 'source' and 
REM have it copied out to all my examples which use it.

copy /Y exoskeleton.js\src\exoskeleton.js api-example
copy /Y exoskeleton.js\src\exoskeleton.js database-example\script
copy /Y exoskeleton.js\src\exoskeleton.js interactive-console\script
copy /Y exoskeleton.js\src\exoskeleton.js markdown-viewer\script
