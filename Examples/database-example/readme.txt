This example shows how you can use lokijs (https://github.com/techfort/LokiJS) javascript database to 
save your database to the filesystem using exoskelton file api.

Our exoskeleton.js includes a KeyStoreAdapter which is a valid lokijs persistence adapter.

Our example will autoload the database, initialize it on database creation, and automatically save 
at an interval we define.

Our example also listens for the exoskeleton 'shutdown' event and flushes any pending database changes.

We don't currently use the loki-indexed-adapter.js but indexeddb is available if self-hosting so the 
script is included for experimentation.  Download full lokijs from above or npm for new apps.
