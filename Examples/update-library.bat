REM Utility batch script so i can edit exoskeleton.js in its 'source' and 
REM have it copied out to all my examples which use it.

copy /Y exoskeleton.js\dist\exoskeleton.js api-example
copy /Y exoskeleton.js\dist\exoskeleton.js database-example\script
copy /Y exoskeleton.js\dist\exoskeleton.js interactive-console\script
copy /Y exoskeleton.js\dist\exoskeleton.js markdown-viewer\script
copy /Y exoskeleton.js\dist\exoskeleton.js native-example\script
copy /Y exoskeleton.js\dist\exoskeleton.js radio-example\script
copy /Y exoskeleton.js\dist\exoskeleton.js webservice-example\script
copy /Y exoskeleton.js\dist\exoskeleton.js audioplayer-example\dist
copy /Y exoskeleton.js\dist\exoskeleton.d.ts api-example
copy /Y exoskeleton.js\dist\exoskeleton.d.ts database-example\script
copy /Y exoskeleton.js\dist\exoskeleton.d.ts interactive-console\script
copy /Y exoskeleton.js\dist\exoskeleton.d.ts markdown-viewer\script
copy /Y exoskeleton.js\dist\exoskeleton.d.ts native-example\script
copy /Y exoskeleton.js\dist\exoskeleton.d.ts radio-example\script
copy /Y exoskeleton.js\dist\exoskeleton.d.ts webservice-example\script
copy /Y exoskeleton.js\dist\exoskeleton.d.ts audioplayer-example\definitions
pause