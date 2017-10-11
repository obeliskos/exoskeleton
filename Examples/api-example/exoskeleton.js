/**
 *  exoskeleton.js is a wrapper interface for accessing com object functionality exposed by the exoskeleon shell.
 *  @author Obeliskos
 *
 *  
 */
(function (root, factory) {
    if (typeof define === 'function' && define.amd) { // jshint ignore:line
        // AMD
        define([], factory); // jshint ignore:line
    } else if (typeof exports === 'object') {
        // Node, CommonJS-like
        module.exports = factory(); // jshint ignore:line
    } else {
        // Browser globals (root is window)
        root.Exoskeleton = factory();
    }
}(this, function () {
    return (function () {
        'use strict';

        function overrideConsole() {
            window.console = {
                log: function (text) {
                    if (!exoskeleton.logger) return;

                    exoskeleton.logger.logText(text);
                },
                info: function (text, data) {
                    if (!exoskeleton.logger) return;

                    exoskeleton.logger.logInfo(data, text);
                },
                warn: function (text) {
                    if (!exoskeleton.logger) return;

                    exoskeleton.logger.logWarning("", text);
                },
                error: function (text) {
                    if (!exoskeleton.logger) return;

                    exoskeleton.logger.logError(text);
                },
                dir: function (obj) {
                    if (!exoskeleton.logger) return;

                    exoskeleton.logger.logText(obj ? JSON.stringify(obj, null, 2) : "null");
                }
            };
        }

        // Due to .net com stuggles with marshalling types, we will stringify every arg
        // before stringifying the whole 
        function wrapArguments(argArray) {
            if (argArray === null) return null;
            if (!(argArray instanceof Array)) return null;
            if (argArray.length === 0) return null;

            for (var idx = 0; idx < argArray.length; idx++) {
                argArray[idx] = JSON.stringify(argArray[idx]);
            }

            return JSON.stringify(argArray);
        }

        // should come in as 1-element array of encoded data, sliced off arguments
        function unwrapArguments(wrapArgs) {
            if (wrapArgs.length === 0) return null;

            if (typeof wrapArgs[0] !== "string") {
                wrapArgs = null;
            }
            else {
                wrapArgs = JSON.parse(wrapArgs[0]);
                for (var idx = 0; idx < wrapArgs.length; idx++) {
                    wrapArgs[idx] = JSON.parse(wrapArgs[idx]);
                }
            }

            return wrapArgs;
        }

        /**
         * Exoskeleton main javascript facade interface to the C# API exposed via COM.
         *
         * @constructor Exoskeleton
         */
        function Exoskeleton() {
            var self = this;

            this.exo = window.external;

            this.events = new ExoEventEmitter(this.exo);

            if (!window || !window.external) {
                throw new Error("Exoskeleton could not find the COM interface exposed by exoskeleton");
            }

            // establish global function for c# to broadcast events to
            window.exoskeletonEmitEvent = function (eventName, eventData) {
                self.events.emit(eventName, eventData);
            };

            // let's also assume control over errors raised and pipe them through our own logger
            window.onerror = function (msg, url, line, col, error) {
                self.logger.logError(msg, url, line, col, error);
                return true;
            };

            overrideConsole();

            if (this.exo.Media) this.media = new Media(this.exo.Media);
            if (this.exo.Com) this.com = new Com(this.exo.Com);
            if (this.exo.File) this.file = new File(this.exo.File);
            if (this.exo.Main) this.main = new Main(this.exo.Main);
            if (this.exo.Proc) this.proc = new Proc(this.exo.Proc);
            if (this.exo.Session) this.session = new Session(this.exo.Session);
            if (this.exo.System) this.system = new System(this.exo.System);
            if (this.exo.Logger) this.logger = new Logger(this.exo.Logger);
            if (this.exo.Net) this.net = new Net(this.exo.Net);
            if (this.exo.Enc) this.enc = new Enc(this.exo.Enc);
            if (this.exo.Menu) this.menu = new Menu(this.exo.Menu);
            if (this.exo.Toolbar) this.toolbar = new Toolbar(this.exo.Toolbar);
            if (this.exo.Statusbar) this.statusbar = new Statusbar(this.exo.Statusbar);
            if (this.exo.Util) this.util = new Util(this.exo.Util);
            if (this.exo.Dialog) this.dialog = new Dialog(this.exo.Dialog);

            // go ahead and instance the keystore adapter for peristable key/value store 
            // and / or to use as a LokiJS persistence adapter.
            this.keystore = new KeyStoreAdapter(this.exo.File);
        }

        /**
         * Returns the currently active settings (xos file), converted to a json string.
         * @returns {object} The current application settings
         * @memberof Exoskeleton
         * @instance
         * @example
         * var settings = exoskeleton.getApplicationSettings();
         *
         * if (settings.ScriptingMediaEnabled) {
	     *   exoskeleton.speech.speak("hello");
         * }
         */
        Exoskeleton.prototype.getApplicationSettings = function () {
            return JSON.parse(this.exo.GetApplicationSettings());
        };

        /**
         * Returns the important exoskeleton environment locations. (Current, Settings, Executable)
         * @returns {object} Object containing 'Executable', 'Settings' and 'Current' properties.
         * @memberof Exoskeleton
         * @instance
         * @example
         * var locations = exoskeleton.getLocations();
         * console.log("current directory : " + locations.Current);
         * console.log("location of (active) settings file : " + locations.Settings);
         * console.log("location of (active) exoskeleton executable : " + locations.Executable);
         */
        Exoskeleton.prototype.getLocations = function () {
            return JSON.parse(this.exo.GetLocations());
        };

        /**
         * Initiates shutdown of this exoskeleton app by notifying the container.
         * @memberOf Exoskeleton
         */
        Exoskeleton.prototype.shutdown = function () {
            this.exo.Shutdown();
        };

        // #region Com

        /**
         * Com API class for interacting with COM Objects registered on the system.
         * @param {object} exoCom - reference to the real 'Com' COM API class.
         * @constructor Com
         */
        function Com(exoCom) {
            this.exoCom = exoCom;
        }

        /**
         * Allows creation of c# singleton for further operations.
         * @param {string} comObjectName - Com class type name to instance.
         * @memberof Com
         * @instance
         * @example
         * exoskeleton.com.createInstance("SAPI.SpVoice");
         */
        Com.prototype.createInstance = function (comObjectName) {
            this.exoCom.CreateInstance(comObjectName);
        };

        /**
         * Allows invocation of a method on the global singleton com object instance.
         * @param {string} methodName - Com interface method to invoke.
         * @param {any[]} methodParams - Parameters to pass to com interface method.
         * @memberof Com
         * @instance
         * @example
         * exoskeleton.com.createInstance("SAPI.SpVoice");
         * exoskeleton.com.invokeMethod("Speak", ["this is a test message scripting activex from java script", 1])
         */
        Com.prototype.invokeMethod = function (methodName, methodParams) {
            this.exoCom.InvokeMethod(methodName, JSON.stringify(methodParams));
        };

        /**
         * Activates instance to Com type, calls a single method (with params) and then disposes instance.
         * @param {string} comObjectName - Com class type name to instance.
         * @param {string} methodName - Com interface method to invoke.
         * @param {any[]} methodParams - Parameters to pass to com interface method.
         * @memberof Com
         * @instance
         * @example
         * exoskeleton.com.createAndInvokeMethod ("SAPI.SpVoice", "Speak",
         *     ["this is a test message scripting activex from java script", 1]);
         */
        Com.prototype.createAndInvokeMethod = function (comObjectName, methodName, methodParams) {
            this.exoCom.CreateAndInvokeMethod(comObjectName, methodName, JSON.stringify(methodParams));
        };

        // #endregion

        // #region Dialog

        /**
         * Dialog API class for creating and interfacing with WinForms dialogs.
         * Several predefined dialogs exist or you can compose your own using
         * a global dialog singleton.
         * @param {any} exoDialog
         * @constructor Dialog
         */
        function Dialog(exoDialog) {
            this.exoDialog = exoDialog;
        }

        /**
         * Adds a CheckBox to the Dialog singleton Form.
         * @param {object|string} checkbox - initial properties to assign to checkbox
         * @param {string=} parentName - optional name of parent control to nest within
         * @memberof Dialog
         * @instance
         * @example
         * exoskeleton.dialog.addCheckbox({
         *   Name: "StudentCheckbox",
         *   Text: "Student",
         *   Checked: true,
         *   Top: 100,
         *   Left: 10
         * }, "TopPanel");
         */
        Dialog.prototype.addCheckBox = function (checkbox, parentName) {
            if (typeof checkbox === "object") {
                checkbox = JSON.stringify(checkbox);
            }

            if (typeof parentName === "undefined") {
                parentName = null;
            }

            this.exoDialog.AddCheckBox(checkbox, parentName);
        };

        /**
         * Adds a ComboBox to the Dialog singleton Form.
         * @param {object|string} comboBox - initial properties to assign to ComboBox
         * @param {string=} parentName - optional name of parent control to nest within
         * @memberof Dialog
         * @instance
         * @example
         * exoskeleton.dialog.addComboBox({
         *   Name: "CountryDropDown",
         *   Top: 10,
         *   Left: 10,
         *   Items: ['United States', 'United Kingdom', 'Germany', 'France', 'Japan'],
         *   SelectedItem : 'United States'
         * }, "AddressPanel");
         */
        Dialog.prototype.addComboBox = function (comboBox, parentName) {
            if (typeof comboBox === "object") {
                comboBox = JSON.stringify(comboBox);
            }

            if (typeof parentName === "undefined") {
                parentName = null;
            }

            this.exoDialog.AddComboBox(comboBox, parentName);
        };

        /**
         * Adds a 'dismiss dialog' Button to the Dialog singleton Form.
         * @param {object|string} button - initial properties to assign to button
         * @param {string} dialogResult - dismiss result ('OK', 'Cancel', 'Yes', 'No')
         * @param {string=} parentName - optional name of parent control to nest within
         * @memberof Dialog
         * @instance
         * @example
         * exoskeleton.dialog.addDialogButton({
         *   Text: "OK",
         *   Top: 10,
         *   Left: 200
         * }, "OK", "BottomPanel");
         */
        Dialog.prototype.addDialogButton = function (button, dialogResult, parentName) {
            if (typeof button === "object") {
                button = JSON.stringify(button);
            }

            if (typeof parentName === "undefined") {
                parentName = null;
            }

            this.exoDialog.AddDialogButton(button, dialogResult, parentName);
        };

        /**
         * Adds a Label to the Dialog singleton Form.
         * @param {object|string} label - initial properties to assign to label
         * @param {string=} parentName - optional name of parent control to nest within
         * @memberof Dialog
         * @instance
         * @example
         * exoskeleton.dialog.addLabel({
         *   Name: "AddressTextBox",
         *   Text: "Address:",
         *   Top: 100,
         *   Left: 10
         * }, "AddressPanel");
         */
        Dialog.prototype.addLabel = function (label, parentName) {
            if (typeof label === "object") {
                label = JSON.stringify(label);
            }

            if (typeof parentName === "undefined") {
                parentName = null;
            }

            this.exoDialog.AddLabel(label, parentName);
        };

        /**
         * Adds a ListBox to the Dialog singleton Form.
         * @param {object|string} listbox - initial properties to assign to ListBox
         * @param {string=} parentName - optional name of parent control to nest within
         * @memberof Dialog
         * @instance
         * @example
         * exoskeleton.dialog.addListBox({
         *   Name: "CountryList",
         *   Top: 10,
         *   Left: 10,
         *   Dock: 'Fill'
         *   Items: ['United States', 'United Kingdom', 'Germany', 'France', 'Japan'],
         *   SelectedItem : 'United States'
         * }, "AddressPanel");
         */
        Dialog.prototype.addListBox = function (listbox, parentName) {
            if (typeof listbox === "object") {
                listbox = JSON.stringify(listbox);
            }

            if (typeof parentName === "undefined") {
                parentName = null;
            }

            this.exoDialog.AddListBox(listbox, parentName);
        };

        /**
         * Adds a NumericUpDown control to the Dialog singleton Form.
         * @param {object|string} numericUpDown - initial properties to assign to ListBox
         * @param {string=} parentName - optional name of parent control to nest within
         * @memberof Dialog
         * @instance
         * @example
         * exoskeleton.dialog.addNumericUpDown({
         *   Name: "AgeNumeric",
         *   Top: 10,
         *   Left: 10,
         *   Minimum: 13,
         *   Maximum: 120
         * }, "UserInfoPanel");
         */
        Dialog.prototype.addNumericUpDown = function (numericUpDown, parentName) {
            if (typeof numericUpDown === "object") {
                numericUpDown = JSON.stringify(numericUpDown);
            }

            if (typeof parentName === "undefined") {
                parentName = null;
            }

            this.exoDialog.AddNumericUpDown(numericUpDown, parentName);
        };

        /**
         * Adds a Panel to the Dialog singleton Form for layout and nesting purposes.
         * @param {object|string} panel - initial properties to assign to panel
         * @param {string=} parentName - optional name of parent control to nest within
         * @memberof Dialog
         * @instance
         * @example
         * exoskeleton.dialog.addPanel({
         *   Name: "FillPanel",
         *   Dock: 'Fill'
         * });
         * exoskeleton.dialog.addPanel({
         *   Name: "TopPanel",
         *   Dock: 'Top',
         *   Height: 100
         * });
         * exoskeleton.dialog.addPanel({
         *   Name: "BottomPanel",
         *   Dock: 'Bottom',
         *   Height: 100
         * });
         */
        Dialog.prototype.addPanel = function (panel, parentName) {
            if (typeof panel === "object") {
                panel = JSON.stringify(panel);
            }

            if (typeof parentName === "undefined") {
                parentName = null;
            }

            this.exoDialog.AddPanel(panel, parentName);
        };

        /**
         * Adds a PropertyGrid to the Dialog singleton Form.
         * @param {object|string} propertyGrid - initial properties to assign to PropertyGrid
         * @param {object} objectJson - object to display in property grid
         * @param {string=} parentName - optional name of parent control to nest within
         * @memberof Dialog
         * @instance
         * @example
         * exoskeleton.dialog.addPropertyGrid({
         *   Name: "AddressProperties",
         *   Top: 10,
         *   Left: 10,
         *   Dock: 'Fill'
         * }, { a: 1, b: 2, c: { x: 9, y: 10, z: 11 } }, "AddressPanel");
         */
        Dialog.prototype.addPropertyGrid = function (propertyGrid, objectJson, parentName) {
            if (typeof propertyGrid === "object") {
                propertyGrid = JSON.stringify(propertyGrid);
            }

            if (typeof objectJson === "object") {
                objectJson = JSON.stringify(objectJson);
            }

            if (typeof parentName === "undefined") {
                parentName = null;
            }

            this.exoDialog.AddPropertyGrid(propertyGrid, objectJson, parentName);
        };

        /**
         * Adds a RadioButton to the Dialog singleton Form.
         * @param {object} radioButton - initial properties to assign to RadioButton
         * @param {string=} parentName - optional name of parent control to nest within
         * @memberof Dialog
         * @instance
         * @example
         * exoskeleton.dialog.addRadioButton({
         *   Name: "GenderMale", Text: "Male", Top: 40, Left: 100, Checked: true
         * });
         * exoskeleton.dialog.addRadioButton({
         *   Name: "GenderFemale", Text: "Female", Top: 40, Left: 140, Checked: false
         * });
         */
        Dialog.prototype.addRadioButton = function (radioButton, parentName) {
            if (typeof radioButton === "object") {
                radioButton = JSON.stringify(radioButton);
            }

            if (typeof parentName === "undefined") {
                parentName = null;
            }

            this.exoDialog.AddRadioButton(radioButton, parentName);
        };

        /**
         * Adds a TextBox to the Dialog singleton Form.
         * @param {object} textbox - initial properties to assign to TextBox
         * @param {string=} parentName - optional name of parent control to nest within
         * @memberof Dialog
         * @instance
         * @example
         * exoskeleton.dialog.addTextBox({
         *   Name: "Street (1)",
         *   Top: 40,
         *   Left: 100,
         *   Text: user.addr1
         * }, "AddressPanel");
         */
        Dialog.prototype.addTextBox = function (textbox, parentName) {
            if (typeof textbox === "object") {
                textbox = JSON.stringify(textbox);
            }

            if (typeof parentName === "undefined") {
                parentName = null;
            }

            this.exoDialog.AddTextBox(textbox, parentName);
        };

        /**
         * Initialize global dialog singleton.
         * @param {string} title - Caption to display on dialog window
         * @param {int} width - sets the width of the dialog
         * @param {int} height - sets the height of the dialog
         * @memberof Dialog
         * @instance
         * @example
         * exoskeleton.dialog.initialize("Verify address information", 600, 400);
         */
        Dialog.prototype.initialize = function (title, width, height) {
            this.exoDialog.Initialize(title, width, height);
        };

        /**
         * Displays the singleton dialog which you have just configured.
         * @returns {object} An object containing dialog result and form content.
         * @memberof Dialog
         * @instance
         * @example
         * var dialogResult = exoskeleton.dialog.showDialog();
         * if (dialogResult.Result === 'OK') {
         *   console.log(dialogResult.UserNameTextBox.Text);
         * }
         */
        Dialog.prototype.showDialog = function () {
            var resultJson = this.exoDialog.ShowDialog();

            return JSON.parse(resultJson);
        };

        /**
         * Displays a predefined dialog allowing user to input a string.
         * @param {string} title - The caption to display on the input dialog window
         * @param {string} prompt - The text to display above, and describing the textbox
         * @param {string=} defaultText - optional text to initialize textbox with.
         * @memberof Dialog
         * @instance
         * @example
         * var phoneNumber = exoskeleton.dialog.showInputDialog(
         *   "Confirm Information",
         *   "Enter your telephone number",
         *   user.PhoneNumber
         * );
         *
         * if (phoneNumber !== null) {
         *   user.PhoneNumber = phoneNumber;
         * }
         */
        Dialog.prototype.showInputDialog = function (title, prompt, defaultText) {
            if (typeof title === "undefined") {
                title = "Enter text:";
            }
            if (typeof prompt === "undefined") {
                prompt = "";
            }
            if (typeof defaultText === "undefined") {
                defaultText = "";
            }

            var result = this.exoDialog.ShowInputDialog(title, prompt, defaultText);

            if (typeof result === "undefined") {
                result = null;
            }

            return result;
        };

        /**
         * Displays a predefined dialog allowing user to select item(s) from a list.
         * @param {string} title - The caption to display on the input dialog window
         * @param {string} prompt - The text to display above, and describing the textbox
         * @param {string[]} values - An array of strings to load picklist with.
         * @param {string|string[]=} selectedItem - string or string[] (if multi) to default selection to.
         * @param {bool} multiselect - Whether to allow multiple selections
         * @memberof Dialog
         * @instance
         * @example
         * // display picklist with no default selection and no multiselection
         * var result = exoskeleton.dialog.showPickList(
         *   "Country Selection",
         *   "Enter your country of residence",
         *   ["United States", "United Kingdom", "Germany", "France", "Australia", "Japan", "China", "India"]
         * );
         *
         * // since no multiselect, result is a string and not array of strings
         * if (result !== null) {
         *   console.log("user picked : " + result);
         * }
         */
        Dialog.prototype.showPickList = function (title, prompt, values, selectedItem, multiselect) {
            if (typeof title === "undefined") {
                title = "Item Picklist";
            }
            if (typeof prompt === "undefined") {
                prompt = "Enter an item";
            }
            if (typeof values === "undefined") {
                values = [];
            }
            values = JSON.stringify(values);

            if (typeof selectedItem === "undefined") {
                selectedItem = null;
            }
            if (!multiselect) {
                multiselect = false;
            }

            var result = this.exoDialog.ShowPickList(title, prompt, values, selectedItem, multiselect);

            if (typeof result === 'undefined') {
                result = null;
            }

            return multiselect ? JSON.parse(result) : result;
        };

        // #endregion

        // #region Enc

        /**
         * Enc API class for performing various encryption and hashing tasks.
         * @param {object} exoEnc - reference to the real COM 'Enc' API class.
         * @constructor Enc
         */
        function Enc(exoEnc) {
            this.exoEnc = exoEnc;
        }

        /**
         * Encrypts a string using the provided password.
         * @param {string} data - The string to encrypt.
         * @param {string} password - The password to encrypt with.
         * @returns {string} encrypted string result
         * @memberof Enc
         * @instance
         * @example
         * var encryptedString = exoskeleton.enc.encrypt("some secret msg", "s0m3p4ssw0rd");
         */
        Enc.prototype.encrypt = function (data, password) {
            return this.exoEnc.Encrypt(data, password);
        };

        /**
         * Decrypts a string using the provided password.
         * @param {string} data - The string to decrypt.
         * @param {string} password - The password to decrypt with.
         * @returns {string} decrypted string result
         * @memberof Enc
         * @instance
         * @example
         * var originalString = "some secret msg";
         * var encryptedString = exoskeleton.enc.encrypt(originalString, "s0m3p4ssw0rd");
         * var decryptedString = exoskeleton.enc.decrypt(encryptedString, "s0m3p4ssw0rd");
         * console.log("original: " + originalString);
         * console.log("encrypted: " + encryptedString);
         * console.log("decrypted: " + decryptedString);
         */
        Enc.prototype.decrypt = function (data, password) {
            return this.exoEnc.Decrypt(data, password);
        };

        /**
         * Create encrypted copies of the specified file(s) using the provided password.
         * @param {string=} directory - Directory where file(s) to be encrypted reside (or current directory if null).
         * @param {string} filemask - filename or wildcard pattern of files to encrypt.
         * @param {string} password - The password to encrypt with.
         * @memberof Enc
         * @instance
         * @example
         * // creates encrypted file(s) with '.enx' suffix
         * exoskeleton.enc.encryptFiles("c:\\source", "readme.txt", "s0m3p4ssw0rd");
         * exoskeleton.enc.encryptFiles("c:\\source", "*.doc", "s0m3p4ssw0rd");
         */
        Enc.prototype.encryptFiles = function (directory, filemask, password) {
            this.exoEnc.EncryptFiles(directory, filemask, password);
        };

        /**
         * Create decrypted copies of the specified file(s) using the provided password.
         * @param {string=} directory - Directory where file(s) to be decrypted reside (or current directory if null).
         * @param {string} filemask - filename or wildcard pattern of files to decrypt.
         * @param {string} password - The password to decrypt with.
         * @memberof Enc
         * @instance
         * @example
         * // creates decrypted file(s) without the '.enx' suffix
         * exoskeleton.enc.decryptFiles("c:\\source", "readme.txt.enx", "s0m3p4ssw0rd");
         * exoskeleton.enc.decryptFiles("c:\\source", "*.doc.enx", "s0m3p4ssw0rd");
         */
        Enc.prototype.decryptFiles = function(directory, filemask, password) {
            this.exoEnc.DecryptFiles(directory, filemask, password);
        };

        /**
         * Creates ana MD5 Hash for the file specified by the provided filename.
         * @param {string} filename - The filename of the file to calculate a hash file.
         * @returns {string} the hex string encoded md5 hash
         * @memberof Enc
         * @instance
         * @example
         * var hash = exoskeleton.enc.createMD5Hash("c:\\source\\readme.txt");
         */
        Enc.prototype.createMD5Hash = function(filename) {
            return this.exoEnc.GetBase64EncodedMD5Hash(filename);
        };

        /**
         * Creates ana SH1 Hash for the file specified by the provided filename.
         * @param {string} filename : The filename of the file to calculate a hash file.
         * @returns {string} the hex string encoded sha1 hash
         * @memberof Enc
         * @instance
         * @example
         * var hash = exoskeleton.enc.createSHA1Hash("c:\\source\\readme.txt");
         */
        Enc.prototype.createSHA1Hash = function(filename) {
            return this.exoEnc.GetBase64EncodedSHA1Hash(filename);
        };

        /**
         * Creates ana SHA256 Hash for the file specified by the provided filename.
         * @param {string} filename - The filename of the file to calculate a hash file.
         * @returns {string} the hex string encoded sha256 hash
         * @memberof Enc
         * @instance
         * @example
         * var hash = exoskeleton.enc.createSHA256Hash("c:\\source\\readme.txt");
         */
        Enc.prototype.createSHA256Hash = function(filename) {
            return this.exoEnc.GetBase64EncodedSHA256Hash(filename);
        };

        /**
         * Creates MD5, SHA1, and SHA256 hashes for the file(s) specified.
         * @param {string} path - Directory to look in for files to hash.
         * @param {string} searchPattern - Filename or wildcard of file(s) to hash.
         * @returns {object[]} array of custom objects containing hash info
         * @memberof Enc
         * @instance
         * @example
         * var detailedHashInfo = exoskeleton.enc.hashFiles("c:\\source", "*.doc");
         * console.log(detailedHashInfo.length);
         * console.log(detailedHashInfo[0].sha256);
         */
        Enc.prototype.hashFiles = function (path, searchPattern) {
            return JSON.parse(this.exoEnc.HashFiles(path, searchPattern));
        };

        // #endregion

        // #region File

        /**
         * File API class for interfacing with .NET File and Directory classes.
         * @param {object} exoFile - reference to the real 'File' COM API class.
         * @constructor File
         */
        function File(exoFile) {
            this.exoFile = exoFile;
        }

        /**
         * Combine multiple paths into one.
         * @param {string[]} paths - Array of paths to combine.
         * @returns {string} - Combined path string.
         * @memberof File
         * @instance
         * @example
         * var fullyQualifiedPath = exoskeleton.file.combinePaths(["c:\\downloads", "myfile.txt"]);
         * console.log(fullyQualifiedPath);
         * // "c:\downloads\myfile.txt"
         */
        File.prototype.combinePaths = function (paths) {
            var pathsJson = JSON.stringify(paths);
            return this.exoFile.CombinePaths(pathsJson);
        };

        /**
         * Copies an existing file to a new file.  Overwriting is not allowed.
         * @param {string} source - Filename to copy from.
         * @param {string} dest - Filename to copy to (must not already exist).
         * @memberof File
         * @instance
         * @example
         * exoskeleton.file.copyFile("c:\\myfolder\\file1.txt", "c:\\myfolder\\file1.txt.bak");
         */
        File.prototype.copyFile = function (source, dest) {
            this.exoFile.CopyFile(source, dest);
        };

        /**
         * Creates all directories and subdirectories in the specified path unless they already exist.
         * @param {string} path - The directory to create.
         * @memberof File
         * @instance
         * @example
         * exoskeleton.file.createDirectory("c:\\downloads\\subdir");
         */
        File.prototype.createDirectory = function (path) {
            this.exoFile.CreateDirectory(path);
        };

        /**
         * Deletes an empty directory.
         * @param {string} path - The name of the empty directory to delete.
         * @memberof File
         * @instance
         * @example
         * exoskeleton.file.deleteDirectory("c:\\downloads\\subdir");
         */
        File.prototype.deleteDirectory = function (path) {
            this.exoFile.DeleteDirectory(path);
        };

        /**
         * Deletes the specified file.
         * @param {string} filename - Name of file to delete. Wildcard characters are not supported.
         * @memberof File
         * @instance
         * @example
         * exoskeleton.file.deleteFile("c:\\myfolder\\file1.txt.bak");
         */
        File.prototype.deleteFile = function (filename) {
            this.exoFile.DeleteFile(filename);
        };

        /**
         * Gets subdirectory names of a parent directory.
         * @param {string} parentDir - Directory to list subdirectories for.
         * @returns {string[]} - string array of subdirectories.
         * @memberof File
         * @instance
         * @example
         * var result = exoskeleton.file.getDirectories("c:\\downloads");
         * console.dir(result);
         * // might log:
         * // [
         * // "c:\\downloads\\subdir1",
         * // "c:\\downloads\\subdir2",
         * // "c:\\downloads\\subdir3"
         * // ]
         */
        File.prototype.getDirectories = function (parentDir) {
            return JSON.parse(this.exoFile.GetDirectories(parentDir));
        };

        /**
         * Gets DirectoryInfo for the specified directory path.
         * @param {string} path - Name of the directory to get information for.
         * @returns {object} - Object containing directory info as properties.
         * @memberof File
         * @instance
         * @example
         * console.dir(exoskeleton.file.getDirectoryInfo("c:\\downloads"));
         */
        File.prototype.getDirectoryInfo = function (path) {
            return JSON.parse(this.exoFile.GetDirectoryInfo(path));
        };

        /**
         * Returns the directory portion of the path without the filename.
         * @param {string} path - The full pathname to get directory portion of.
         * @memberof File
         * @instance
         * @returns {string} the directory portion of the path provided.
         * @example
         * console.log(exoskeleton.file.getDirectoryName("c:\\downloads\\myfile.txt"));
         * // logs : "c:\downloads"
         */
        File.prototype.getDirectoryName = function (path) {
            return this.exoFile.GetDirectoryName(path);
        };

        /**
         * Gets information about each of the mounted drives.
         * @memberof File
         * @instance
         * @returns {object} array of serialized DriveInfo objects
         * @example
         * var driveInfo = exoskeleton.file.getDriveInfo();
         * driveInfo.forEach(function(drive) {
         *   console.log(drive.Name + ":" + drive.AvailableFreeSpace);
         * });
         */
        File.prototype.getDriveInfo = function () {
            return JSON.parse(this.exoFile.GetDriveInfo());
        };

        /**
         * Gets the file extension of the fully qualified path.
         * @param {string} path - The filepath to get extension of.
         * @memberof File
         * @instance
         * @returns {string} the file extension portion of the path provided.
         * @example
         * console.log(exoskeleton.file.getExtension("c:\\downloads\\myfile.txt"));
         * // logs : ".txt"
         */
        File.prototype.getExtension = function (path) {
            return this.exoFile.GetExtension(path);
        };

        /**
         * Gets FileInfo for the specified filename.
         * @param {string} filename - The filename to get information on.
         * @returns {object} - Json object representation of FileInfo class.
         * @memberof File
         * @instance
         */
        File.prototype.getFileInfo = function (filename) {
            return JSON.parse(this.exoFile.GetFileInfo(filename));
        };

        /**
         * Gets list of files matching a pattern within a parent directory.
         * @param {string} parentDir - Parent directory to search within.
         * @param {string} searchPattern - Optional wildcard search pattern to filter on.
         * @returns {string[]} array of filenames matching privided  searchPattern
         * @memberof File
         * @instance
         */
        File.prototype.getFiles = function (parentDir, searchPattern) {
            return JSON.parse(this.exoFile.GetFiles(parentDir, searchPattern));
        };

        /**
         * Returns the filename portion of the path without the directory.
         * @param {string} path - The full pathname to get filename portion of.
         * @memberof File
         * @instance
         * @returns {string} the filename portion of the path provided.
         * @example
         * console.log(exoskeleton.file.getFileName("c:\\downloads\\myfile.txt"));
         * // logs : "myfile.txt"
         */
        File.prototype.getFileName = function (path) {
            return this.exoFile.GetFileName(path);
        };

        /**
         * Gets a list of logical drives.
         * @memberof File
         * @instance
         * @returns {string[]} array of drive names
         * @example
         * var result = exoskeleton.file.getLogicalDrives();
         * console.log(result);
         * // logs : C:\,D:\,Z:\
         */
        File.prototype.getLogicalDrives = function () {
            return JSON.parse(this.exoFile.GetLogicalDrives());
        };

        /**
         * Opens a text file, reads all lines of the file and returns text as a string.
         * @param {string} filename - The file to read from.
         * @returns {string} - file contents as string.
         * @memberof File
         * @instance
         */
        File.prototype.loadFile = function (filename) {
            return this.exoFile.LoadFile(filename);
        };

        /**
         * Reads a binary file into a Base64 string.  Can be used for encoding data urls.
         * @param {string} filename - The file to read from.
         * @returns {string} Base64 encoded binary content.
         * @memberof File
         * @instance
         * @example
         * var img1 = document.getElementById("img1");
         * var pic1 = exoskeleton.file.loadFileBase64("c:\\images\\pic1.png");
         * img1.src = 'data:image/png;base64,' + pic1;
         */
        File.prototype.loadFileBase64 = function (filename) {
            return this.exoFile.LoadFileBase64(filename);
        };

        /**
         * Writes a text file with the provided contents string.
         * If the file already exists, it will be overwritten.
         * @param {string} filename - Filename to write to.
         * @param {string} contents - Contents to write into file.
         * @memberof File
         * @instance
         */
        File.prototype.saveFile = function (filename, contents) {
            this.exoFile.SaveFile(filename, contents);
        };

        /**
         * Writes a binary file with bytes derived from the provided base64 string.
         * @param {string} filename - Filename to write to.
         * @param {string} contents - Base64 encoded binary content.
         * @memberof File
         * @instance
         * @example
         * var canvas = document.getElementById('canvas');
         * var b64Text = canvas.toDataURL();
         * b64Text = b64Text.replace('data:;image/png;base64,','');
         * exoskeleton.file.saveFileBase64('canvas1.png', b64Text);
         */
        File.prototype.saveFileBase64 = function (filename, contents) {
            this.exoFile.SaveFileBase64(filename, contents);
        };

        /**
         * Starts our container singleton watcher on path specified by user.
         * Events will be emitted as multicast, so if multiple forms load
         * multiple watchers, they should distinguish themselves with the
         * eventBaseName parameter.
         * @param {string} path - Path to 'watch'.
         * @param {string} eventBaseName - Optional base name to emit with.
         * @memberof File
         * @instance
         * @example
         * // The following will generate 'download.created', 'download.deleted', and 'download.changed' events.
         * exoskeleton.file.startWatcher("C:\downloads", "download");
         * exoskeleton.events.on("multicast.download.created", function(data) {
         *   // data params is json encoded string (for now).
         *   exoskeleton.logger.logInfo("multicast.download.created event received", data);
         * });
         */
        File.prototype.startWatcher = function (path, eventBaseName) {
            this.exoFile.StartWatcher(path, eventBaseName);
        };

        /**
         * Disables the watcher singleton.
         * @memberof File
         * @instance
         * @example
         * exoskeleton.file.stopWatcher();
         */
        File.prototype.stopWatcher = function () {
            this.exoFile.StopWatcher();
        };

        // #endregion

        // #region Logger

        /**
         * Logger API class for dealing with exoskeleton logger.
         * @param {object} exoLogger - reference to the real COM API Logger class.
         * @constructor Logger
         */
        function Logger(exoLogger) {
            this.exoLogger = exoLogger;
        }

        /**
         * Logs an "info" message to the logger's message list.
         * @param {string} source - Descriptive 'source' of the info.
         * @param {string} message - Info detail message
         * @memberof Logger
         * @instance
         * @example
         * exoskeleton.logger.logInfo("myfunction", "something interesting happened");
         */
        Logger.prototype.logInfo = function (source, message) {
            this.exoLogger.LogInfo(source, message);
        };

        /**
         * Logs a "warning" message to the logger's message list.
         * @param {string} source - Descriptive 'source' of the warning.
         * @param {string} message - Detailed warning message.
         * @memberof Logger
         * @instance
         * @example
         * exoskeleton.logger.logInfo("myfunction", "something odd happened");
         */
        Logger.prototype.logWarning = function (source, message) {
            this.exoLogger.LogWarning(source, message);
        };

        /**
         * Logs an "error" message to the logger's message list.
         * @param {string} msg - Message to log.
         * @param {string=} url - The url of the script where the error occurred.
         * @param {string=} line - Line number of the javascript where the error occurred.
         * @param {string=} col - Column number of the javascript where the error occurred.
         * @param {string=} error - Detailed informatino about the error.
         * @memberof Logger
         * @instance
         * @example
         * exoskeleton.logger.logError("something dangerous happened", "myfunction");
         */
        Logger.prototype.logError = function (msg, url, line, col, error) {
            this.exoLogger.LogError(msg, url, line, col, error);
        };

        /**
         * Logs text to the logger's console.
         * @param {string} message - Text to append to the console.
         * @memberof Logger
         * @instance
         * @example
         * var now = new DateTime();
         * exoskeleton.logger.logText("started processing at : " + now);
         */
        Logger.prototype.logText = function (message) {
            this.exoLogger.LogText(message);
        };

        /**
         * Logs an object to the logger's console.
         * @param {object} obj - Object to serialize and pretty print to console.
         * @memberof Logger
         * @instance
         * @example
         * var obj = new { a: 1, b: 2 }
         * exoskeleton.logger.logObject(obj);
         */
        Logger.prototype.logObject = function (obj) {
            var json = JSON.stringify(obj, undefined, 2);

            this.exoLogger.LogText(json);
        };

        // #endregion Logger

        // #region Main

        /**
         * Main API class used for general MessageBox, FileDialog, Notifications, and Container utilitites.
         * @param {object} exoMain - reference to the real 'Main' COM API class.
         * @constructor Main
         */
        function Main(exoMain) {
            this.exoMain = exoMain;
        }

        /**
         * Process all Windows messages currently in the message queue.
         * @memberof Main
         * @instance
         * @example
         * exoskeleton.main.doEvents();
         */
        Main.prototype.doEvents = function () {
            this.exoMain.DoEvents();
        };

        /**
         * Signals the host container to exit fullscreen mode.
         * @memberof Main
         * @instance
         * @example
         * exoskeleton.main.exitFullscreen();
         */
        Main.prototype.exitFullscreen = function () {
            this.exoMain.ExitFullscreen();
        };

        /**
         * Signals the host container to enter fullscreen mode.
         * @memberof Main
         * @instance
         * @example
         * exoskeleton.main.fullscreen();
         */
        Main.prototype.fullscreen = function () {
            this.exoMain.Fullscreen();
        };

        /**
         * Opens a new host container with the url and settings provided.
         * @param {string} caption - Window caption to apply to new window.
         * @param {string} url - Url to load within the new window.
         * @param {int} width - Width (in pixels) to size new window to.
         * @param {int} height - Height (in pixels) to size new window to.
         * @memberof Main
         * @instance
         * @example
         * // relative pathname
         * exoskeleton.main.openNewWindow("My App Settings", "settings.htm", 800, 480);
         * // external internet address
         * exoskeleton.main.openNewWindow("My App Settings", "https://en.wikipedia.org/wiki/Main_Page", 800, 480);
         * // You might also pass raw html, prefixed with '@' symbol.
         * var html = "@<html><body><h3>Hello World</h3></body></html>";
         * exoskeleton.main.openNewWindow("My dynamic page", html, 400, 400);
         */
        Main.prototype.openNewWindow = function (caption, url, width, height) {
            this.exoMain.OpenNewWindow(caption, url, width, height);
        };

        /**
         * Updates the window title for the host container.
         * @param {string} title - Text to apply to window title.
         * @memberof Main
         * @instance
         * @example
         * exoskeleton.main.setWindowTitle("My Editor App - " + filename);
         */
        Main.prototype.setWindowTitle = function (title) {
            this.exoMain.SetWindowTitle(title);
        };

        /**
         * Allows user to pick a file to 'open' and returns that filename.  Although only a few options are
         * documented here, any 'OpenFileDialog' properties may be attempted to be passed.
         * @param {object=} dialogOptions - optional object containing 'OpenFileDialog' properties
         * @param {string=} dialogOptions.Title - title to display on open file dialog
         * @param {string=} dialogOptions.InitialDirectory - initial directory to pick file(s) from
         * @param {string=} dialogOptions.Filter - filtering options such as "txt files (*.txt)|*.txt|All files (*.*)|*.*"
         * @param {int=} dialogOptions.FilterIndex - the index of the filter currently selected in the file dialog box
         * @param {bool=} dialogOptions.Multiselect - whether to allow user to select multiple files
         * @returns {object=} 'OpenFileDialog' properties after dialog was dismissed, or null if cancelled.
         * @memberof Main
         * @instance
         * @example
         * // example passing a few (optional) dialog initialization settings
         * var dialogValues = exoskeleton.main.showOpenFileDialog({
         *   Title: "Select myapp data file to open",
         *   InitialDirectory: "c:\\mydatafolder",
         *   Filter: "dat files (*.dat)|*.dat|All files (*.*)|*.*"
         * });
         * // if user did not cancel
         * if (dialogValues) {
         *   console.log("selected file (name) : " + dialogValues.FileName);
         * }
         */
        Main.prototype.showOpenFileDialog = function (dialogOptions) {
            if (dialogOptions) {
                dialogOptions = JSON.stringify(dialogOptions);
            }

            var result = this.exoMain.ShowOpenFileDialog(dialogOptions);
            if (result) {
                result = JSON.parse(result);
            }
            return result;
        };

        /**
         * Displays a message box to the user and returns the button they clicked.
         * @param {string} text - Message to display to user.
         * @param {string} caption - Caption of message box window
         * @param {string=} buttons - "OK"||"OKCancel"||"YesNo"||"YesNoCancel"||"AbortRetryIgnore"||"RetryCancel"
         * @param {string=} icon - "None"||"Information"||"Question"||"Warning"||"Exclamation"||"Hand"||"Error"||"Stop"||"Asterisk"
         * @returns {string} Text (ToString) representation of button clicked.
         * @memberof Main
         * @instance
         * @example
         * var dialogResultString = exoskeleton.main.showMessageBox(
         *    "An error has occured", "MyApp error", "OKCancel", "Exclamation"
         * );
         * if (dialogResultString === "OK") {
         *   console.log("user clicked ok");
         * }
         */
        Main.prototype.showMessageBox = function (text, caption, buttons, icon) {
            return this.exoMain.ShowMessageBox(text, caption, buttons, icon);
        };

        /**
         * Displays a windows system tray notification.
         * @param {string} title - The notification title.
         * @param {string} message - The notification message.
         * @memberof Main
         * @instance
         * @example
         * exoskeleton.main.showNotification("my exo app", "some notification details");
         */
        Main.prototype.showNotification = function (title, message) {
            this.exoMain.ShowNotification(title, message);
        };

        /**
         * Allows user to pick a file to 'save' and returns that filename.  Although only a few options are
         * documented here, any 'SaveFileDialog' properties may be attempted to be passed.
         * @param {object=} dialogOptions - optional object containing 'OpenFileDialog' properties
         * @param {string=} dialogOptions.Title - title to display on save file dialog
         * @param {string=} dialogOptions.InitialDirectory - initial directory to pick file(s) from
         * @param {string=} dialogOptions.Filter - filtering options such as "txt files (*.txt)|*.txt|All files (*.*)|*.*"
         * @param {int=} dialogOptions.FilterIndex - the index of the filter currently selected in the file dialog box
         * @returns {object=} 'SaveFileDialog' properties after dialog was dismissed, or null if cancelled.
         * @memberof Main
         * @instance
         * @example
         * // example passing a few (optional) dialog initialization settings
         * var dialogValues = exoskeleton.main.showSaveFileDialog({
         *   Title: "Pick data file to save to",
         *   InitialDirectory: "c:\\mydatafolder",
         *   Filter: "dat files (*.dat)|*.dat|All files (*.*)|*.*"
         * });
         * // if user did not cancel
         * if (dialogValues) {
         *   console.log("selected file (name) : " + dialogValues.FileName);
         * }
         */
        Main.prototype.showSaveFileDialog = function (dialogOptions) {
            if (dialogOptions) {
                dialogOptions = JSON.stringify(dialogOptions);
            }

            var result = this.exoMain.ShowSaveFileDialog(dialogOptions);
            if (result) {
                result = JSON.parse(result);
            }
            return result;
        };

        /**
         * Signals the host container to toggle fullscreen mode.
         * @memberof Main
         * @instance
         * @example
         * exoskeleton.main.toggleFullscreen();
         */
        Main.prototype.toggleFullscreen = function () {
            this.exoMain.ToggleFullscreen();
        };

        // #endregion

        // #region Media

        /**
         * Media API class for speech and audio/video/image.
         *
         * @param {object} exoMedia - reference to the real 'Media' COM API class.
         * @constructor Media
         */
        function Media(exoMedia) {
            this.exoMedia = exoMedia;
        }

        /**
         * Invokes text-to-speech to speak the provided message.
         * @param {string} message - The message to speak.
         * @memberof Media
         * @instance
         * @example
         * exoskeleton.media.speak("this is a test");
         */
        Media.prototype.speak = function (message) {
            this.exoMedia.Speak(message);
        };

        /**
         * Invokes text-to-speech to synchronously speak the provided message.
         * @param {string} message - The message to speak.
         * @memberof Media
         * @instance
         * @example
         * exoskeleton.media.speakSync("this is a test");
         */
        Media.prototype.speakSync = function (message) {
            this.exoMedia.SpeakSync(message);
        };

        // #endregion

        // #region Menu

        /**
         * Menu API class used for populating the host container's menu bar.
         * @param {object} exoMenu - reference to the real 'Menu' COM API class.
         * @constructor Menu
         */
        function Menu(exoMenu) {
            this.exoMenu = exoMenu;
        }

        /**
         * Enables visibility of the window's menu
         * @memberof Menu
         * @instance
         * @example
         * exoskeleton.menu.show();
         */
        Menu.prototype.show = function () {
            this.exoMenu.Show();
        };

        /**
         * Hides visibility of the window's menu
         * @memberof Menu
         * @instance
         * @example
         * exoskeleton.menu.hide();
         */
        Menu.prototype.hide = function () {
            this.exoMenu.Hide();
        };

        /**
         * Removes all menu items for reinitialization.  Host window survives across inner page
         * (re)loads or redirects so menus would need to be (re)initialized on page loads.
         * @memberof Menu
         * @instance
         * @example
         * exoskeleton.menu.initialize();
         */
        Menu.prototype.initialize = function() {
            this.exoMenu.Initialize();
        };

        /**
         * Adds a top level menu
         * @param {string} menuName - Text to display on menu
         * @param {string=} emitEventName - The local event name to unicast when the menu is clicked.
         * @memberof Menu
         * @instance
         * @example
         * exoskeleton.menu.addMenu("File");
         * exoskeleton.menu.addMenu("About", "AboutClicked");
         */
        Menu.prototype.addMenu = function (menuName, emitEventName) {
            if (typeof emitEventName === 'undefined') {
                emitEventName = '';
            }
            this.exoMenu.AddMenu(menuName, emitEventName);
        };

        /**
         * Adds menu subitems to an existing menu or submenu.
         * Any event emitted on click will be passed the menu item text as parameter.
         * @param {string} menuName - The text of the parent menu or submenu to add subitem to
         * @param {string} menuItemName - The text of the new subitem to add
         * @param {string=} emitEventName - The optional (local) event name to unicast when the menu is clicked.
         * @param {string[]=} shortcutKeys - Optional array of shortcut key codes.
         * @memberof Menu
         * @instance
         * @example
         * exoskeleton.menu.initialize();
         * exoskeleton.menu.addMenu("File");
         * exoskeleton.menu.addMenuItem("File", "Open", "FileOpenEvent", ["Control", "Shift", "O"]);
         * exoskeleton.menu.addMenuItem("File", "New");
         * // for this example we will use the same common event name for submenu items
         * // these can also be different
         * exoskeleton.menu.addMenuItem("New", ".txt file", "FileNewEvent");
         * exoskeleton.menu.addMenuItem("New", ".png file", "FileNewEvent");
         * exoskeleton.on("FileOpenEvent", function() {
         *   alert('File/Open clicked');
         * });
         * exoskeleton.on("FileNewEvent", function(data) {
         *   alert('File/New/' + data + ' clicked');
         * });
         */
        Menu.prototype.addMenuItem = function (menuName, menuItemName, emitEventName, shortcutKeys) {
            if (typeof shortcutKeys === 'undefined') {
                shortcutKeys = '';
            }
            if (Array.isArray(shortcutKeys)) {
                shortcutKeys = JSON.stringify(shortcutKeys);
            }
            if (typeof emitEventName === 'undefined') {
                emitEventName = '';
            }
            this.exoMenu.AddMenuItem(menuName, menuItemName, emitEventName, shortcutKeys);
        };

        // #endregion

        // #region Net

        /**
         * Net API class for various network and http tasks.
         * @param {object} exoNet - reference to the real COM 'Net' API class.
         * @constructor Net
         */
        function Net(exoNet) {
            this.exoNet = exoNet;
        }

        /**
         * Downloads from an internet url and saves to disk.
         * @param {string} url - The internet url to download from.
         * @param {string} dest - Destination filename on disk.
         * @param {bool} async - Whether to wait until finished before returning.
         * @memberof Net
         * @instance
         * @example
         * exoskeleton.net.downloadFile("https://github.com/obeliskos/exoskeleton/archive/0.2.zip",
         *   "c:\\downloads\\0.2.zip", false);
         */
        Net.prototype.downloadFile = function (url, dest, async) {
            this.exoNet.DownloadFile(url, dest, async);
        };

        /**
         * Fetches text-based resource at the provided url and returns a string of its content.
         * @param {string} url - Internet url of text based resource.
         * @returns {string} - String containing text within the retrieved resource.
         * @memberof Net
         * @instance
         * @example
         * var readmeText = exoskeleton.net.readUrl("https://raw.githubusercontent.com/obeliskos/exoskeleton/master/README.md");
         * exoskeleton.logger.logText(readmeText);
         */
        Net.prototype.readUrl = function (url) {
            return this.exoNet.ReadUrl(url);
        };

        // #endregion

        // #region Proc

        /**
         * Proc API class for performing windows process related tasks.
         * @param {object} exoProc - reference to the real 'Proc' COM API class.
         * @constructor Proc
         */
        function Proc(exoProc) {
            this.exoProc = exoProc;
        }

        /**
         * Starts a process resource by specifying the name of a document or application file.
         * @param {string} procPath - Program to execute.
         * @returns {object} an object containing information about the newly created process.
         * @memberof Proc
         * @instance
         * @example
         * exoskeleton.proc.startPath("calc.exe");
         * exoskeleton.proc.startPath("c:\\windows\\system32\\notepad.exe");
         */
        Proc.prototype.startPath = function (procPath) {
            return JSON.parse(this.exoProc.StartPath(procPath));
        };

        /**
         * Starts a process resource by providing information in a ProcessStartInfo format.
         * @param {object} processStartInfo - Serialized javascript object closely resembling a c# ProcessStartInfo object.
         * @param {string=} processStartInfo.FileName - filename of program to load.
         * @param {string=} processStartInfo.Arguments - string containing arguments to launch with.
         * @param {bool=} processStartInfo.CreateNoWindow - whether to start the process in a new window.
         * @param {bool=} processStartInfo.ErrorDialog - whether to show error dialog if process could not be started.
         * @param {bool=} processStartInfo.LoadUserProfile - whether the windows user profile is to be loaded from the registry.
         * @param {bool=} processStartInfo.UseShellExecute - whether to use the operating system shell to start the process.
         * @param {bool=} processStartInfo.WorkingDirectory - working dir of proc when UseShellExecute is false. dir containing process when UseShellExecute is true.
         * @returns {object} an object containing information about the newly created process.
         * @memberof Proc
         * @instance
         * @example
         * exoskeleton.proc.start({
         *   FileName: "notepad.exe",
         *   Arguments: "c:\\docs\\readme.txt"
         * });
         */
        Proc.prototype.start = function (processStartInfo) {
            return JSON.parse(this.exoProc.Start(JSON.stringify(processStartInfo)));
        };

        /**
         * Gets a detailed list of running processes and their settings. (Takes a long time to run)
         * @memberof Proc
         * @instance
         * @returns {object[]} array of deserialized c# Process objects
         * @example
         * var procList = exoskeleton.proc.getProcesses();
         * procList.forEach(function(p) {
         *   // properties available should correlate to .net Process class member properties.
         *   console.log(p.ProcessName + " : " + p.Id);
         * });
         */
        Proc.prototype.getProcesses = function () {
            return JSON.parse(this.exoProc.GetProcesses());
        };

        /**
         * Gets a simplified list of running processes.
         * @memberof Proc
         * @instance
         * @returns {object[]} array of deserialized c# Process objects
         * @example
         * var procList = exoskeleton.proc.getProcessesSimplified();
         * procList.forEach(function(p) {
         *   // properties available are 'Id', 'ProcessName' and 'MainWindowTitle'.
         *   console.log(p.ProcessName + " : " + p.Id);
         * });
         */
        Proc.prototype.getProcessesSimplified = function () {
            return JSON.parse(this.exoProc.GetProcessesSimplified());
        };

        /**
         * Gets detailed process information by process id.
         * @param {int} id - the windows process id to get more info about.
         * @returns {object} a json object representation of a .net Process object
         * @memberof Proc
         * @instance
         */
        Proc.prototype.getProcessInfoById = function (id) {
            return JSON.parse(this.exoProc.GetProcessInfoById(id));
        };

        /**
         * Gets a list of processes of the provided name.
         * @param {string} name - name of process to get list of.
         * @returns {object[]} array of deserialized c# Process objects
         * @memberof Proc
         * @instance
         * @example
         * var procList = exoskeleton.proc.getProcessesByName("notepad");
         * procList.forEach(function(p) {
         *   console.log(p.ProcessName + " : " + p.Id);
         * });
         */
        Proc.prototype.getProcessesByName = function (name) {
            return JSON.parse(this.exoProc.GetProcessesByName(name));
        };

        /**
         * Kills a running process.
         * @param {int} id - The id of the process to kill.
         * @returns {bool} whether process was found or not.
         * @memberof Proc
         * @instance
         * @example
         * // The id passed can be looked up via calls to getProcesses or getProcessesByName.
         * exoskeleton.proc.killProcessById(1608);
         */
        Proc.prototype.killProcessById = function (id) {
            return this.exoProc.KillProcessById(id);
        };

        // #endregion

        // #region Session

        /**
         * Session API class for interfacing with the exoskeleton 'session' key/value storage.
         * @param {object} exoSession - reference to the real 'Session' COM API class.
         * @constructor Session
         */
        function Session(exoSession) {
            this.exoSession = exoSession;
        }

        /**
         * Looks up the (string) Value for the Session key provided.
         * @param {string} key - The key name to lookup a value for in the session store.
         * @returns {string} - The value associated with key in string form.
         * @memberof Session
         * @instance
         * @example
         * var result = exoskeleton.session.get("username");
         */
        Session.prototype.get = function (key) {
            return this.exoSession.Get(key);
        };

        /**
         * Looks up the (object) Value for the Session key provided.
         * @param {string} key - The key name to lookup a value for in the session store.
         * @returns {object} - The value associated with key parsed into object.
         * @memberof Session
         * @instance
         * @example
         * var userInfo = exoskeleton.session.getObject("UserInfo");
         * console.log(userInfo.name + userInfo.addr + userInfo.phone);
         */
        Session.prototype.getObject = function (key) {
            var result = this.exoSession.Get(key);
            return result ? JSON.parse(result) : result;
        };

        /**
         * Obtains a string list of all keys currently in the session store.
         * @returns {string[]} - An array of string 'keys' within the session store.
         * @memberof Session
         * @instance
         * @example
         * var result = exoskeleton.session.list();
         * result.forEach(function(keyname) {
         *   console.log(exoskeleton.session.get(keyname));
         * });
         */
        Session.prototype.list = function () {
            return JSON.parse(this.exoSession.list());
        };

        /**
         * Assigns a key/value setting within the session store.
         * @param {string} key - The name of the session variable to set.
         * @param {string} value - The string value to assign to session variable.
         * @memberof Session
         * @instance
         * @example
         * exoskeleton.session.set("username", "jdoe");
         */
        Session.prototype.set = function (key, value) {
            this.exoSession.Set(key, value);
        };

        /**
         * Assigns a key/value setting within the session store by serializing it.
         * @param {string} key - The name of the session variable to set.
         * @param {object} value - The object value to assign to session variable.
         * @memberof Session
         * @instance
         * @example
         * exoskeleton.session.setObject("UserInfo", {
         *   name: "jdoe",
         *   addr: "123 anystreet",
         *   phone: "555-1212"
         * });
         */
        Session.prototype.setObject = function (key, value) {
            this.exoSession.Set(key, JSON.stringify(value));
        };

        // #endregion

        // #region Statusbar

        /**
         * Statusbar API class used for manipulating the host container's status strip.
         */
        function Statusbar(exoStatusbar) {
            this.exoStatusbar = exoStatusbar;
        }

        /**
         * Enables visibility of the window's status bar.
         * @memberof Statusbar
         * @instance
         * @example
         * exoskeleton.statusbar.show();
         */
        Statusbar.prototype.show = function () {
            this.exoStatusbar.Show();
        };

        /**
         * Hides visibility of the window's status bar.
         * @memberof Statusbar
         * @instance
         * @example
         * exoskeleton.statusbar.hide();
         */
        Statusbar.prototype.hide = function () {
            this.exoStatusbar.Hide();
        };

        /**
         * Clears both left and right status labels
         * @memberof Statusbar
         * @instance
         * @example
         * exoskeleton.statusbar.initialize();
         */
        Statusbar.prototype.initialize = function () {
            this.exoStatusbar.Initialize();
        };

        /**
         * Sets the text to be displayed in the left status label
         * @param {string} text - text to display in left status label
         * @memberof Statusbar
         * @instance
         * @example
         * exoskeleton.statusbar.setLeftLabel("Welcome to my app");
         */
        Statusbar.prototype.setLeftLabel = function (text) {
            this.exoStatusbar.SetLeftLabel(text);
        };

        /**
         * Sets the text to be displayed in the right status label
         * @param {string} text - text to display in right status label
         * @memberof Statusbar
         * @instance
         * @example
         * exoskeleton.statusbar.setRightLabel("Started : " + new Date());
         */
        Statusbar.prototype.setRightLabel = function (text) {
            this.exoStatusbar.SetRightLabel(text);
        };

        // #endregion

        // #region System

        /**
         * System API class for getting system information, environment variables, registry, etc.
         * @param {object} exoSystem - reference to the real 'System' COM API class.
         * @constructor System
         */
        function System(exoSystem) {
            this.exoSystem = exoSystem;
        }

        /**
         * Get information about the system which this program is being run on.
         * @returns {object} - Json system information object.
         * @memberof System
         * @instance
         * @example
         * var si = exoskeleton.system.getSystemInfo();
         * console.dir(si);
         * console.log(si.MachineName);
         */
        System.prototype.getSystemInfo = function () {
            return JSON.parse(this.exoSystem.GetSystemInfo());
        };

        /**
         * Retrieves a single environment variable value.
         * @param {string} varName - The name of the environment variable to retrieve value for.
         * @returns {string=} The string value of the environment variable (if found).
         * @memberof System
         * @instance
         * @example
         * var path = exoskeleton.system.getEnvironmentVariable("PATH");
         */
        System.prototype.getEnvironmentVariable = function (varName) {
            return this.exoSystem.GetEnvironmentVariable(varName);
        };

        /**
         * Returns a list of all environment variables as properties and property values.
         * @returns {object} - Json hash object with properties representing variables.
         * @memberof System
         * @instance
         * @example
         * var envVariables = exoskeleton.system.getEnvironmentVariables();
         * Object.keys(envVariables, function(key) {
         *   console.log("key: " + key + " val : " + exoskeleton.system.getEnvironmentVariable(key));
         * });
         */
        System.prototype.getEnvironmentVariables = function () {
            return JSON.parse(this.exoSystem.GetEnvironmentVariables());
        };

        /**
         * Sets an environment variable only within this process or child processes.
         * @param {string} varName - The name of the environment variable.
         * @param {string} varValue - The value to assign to the environment variable.
         * @memberof System
         * @instance
         * @example
         * var now = new DateTime();
         * var path = exoskeleton.system.setEnvironmentVariable("LAUNCHTIME", now.toString());
         */
        System.prototype.setEnvironmentVariable = function (varName, varValue) {
            this.exoSystem.SetEnvironmentVariable(varName, varValue);
        };

        /**
         * Finds an existing application window by either class or name and focuses it.
         * @param {string=} className - The class name of the window, or null.
         * @param {string=} windowName - The window name of the window, or null.
         * @memberof System
         * @instance
         * @example
         * // assuming notepad is already running :
         * exoskeleton.system.focusWindow("notepad", null);
         * exoskeleton.system.focusWindow(null, "readme.txt - Notepad");
         */
        System.prototype.focusWindow = function (className, windowName) {
            this.exoSystem.FocusWindow(className, windowName);
        };

        /**
         * Finds a window and sends keycodes to it.
         * This uses .Net SendKeys(), for info on key codes read :
         * https://msdn.microsoft.com/en-us/library/system.windows.forms.sendkeys(v=vs.110).aspx
         * @param {string=} className - The class name of the window, or null.
         * @param {string=} windowName - The window name of the window, or null.
         * @param {string[]} keys - String array of keys or keycodes to send.
         * @returns {bool} - Whether the window was found.
         * @memberof System
         * @instance
         * @example
         * exoskeleton.system.focusAndSendKeys("notepad", null, ["t", "e", "s", "t", "{ENTER}"]);
         */
        System.prototype.focusAndSendKeys = function (className, windowName, keys) {
            var keysJson = JSON.stringify(keys);
            return this.exoSystem.FocusAndSendKeys(className, windowName, keysJson);
        };

        // #endregion

        // #region Toolbar

        /**
         * Toolbar API class used for populating the host container's tool strip.
         */
        function Toolbar(exoToolbar) {
            this.exoToolbar = exoToolbar;
        }

        /**
         * Enables visibility of the window's toolbar.
         * @memberof Toolbar
         * @instance
         * @example
         * exoskeleton.toolbar.show();
         */
        Toolbar.prototype.show = function () {
            this.exoToolbar.Show();
        };

        /**
         * Hides visibility of the window's toolbar.
         * @memberof Toolbar
         * @instance
         * @example
         * exoskeleton.toolbar.hide();
         */
        Toolbar.prototype.hide = function () {
            this.exoToolbar.Hide();
        };

        /**
         * Empties the host window toolstrip of all controls
         * @memberof Toolbar
         * @instance
         * @example
         * exoskeleton.toolbar.initialize();
         */
        Toolbar.prototype.initialize = function () {
            this.exoToolbar.Initialize();
        };

        /**
         * Adds a ToolStripButton to the host window toolstrip
         * @param {string} text - Text to display on the tooltip
         * @param {string} eventName - Name of the local event to raise when clicked
         * @param {string} imagePath - Filepath to the (roughly 32x32 px) image to display on the button
         * @memberof Toolbar
         * @instance
         * @example
         * exoskeleton.toolbar.initializeMenu();
         * exoskeleton.toolbar.addButton("Create new document", "NewDocEvent", "c:\\images\\new.png");
         * exoskeleton.toolbar.addButton("Exit", "ExitEvent", "c:\\images\\exit.png");
         * exoskeleton.events.on("NewDocEvent", function() {
         *   showCustomFileOpenDialog();
         * });
         * exoskeleton.events.on("ExitEvent", function() {
         *   exoskeleton.shutdown();
         * });
         */
        Toolbar.prototype.addButton = function (text, eventName, imagePath) {
            imagePath = imagePath || "";

            this.exoToolbar.AddButton(text, eventName, imagePath);
        };

        /**
         * Adds a visual separator for toolstrip control groups
         * @memberof Toolbar
         * @instance
         * @example
         * exoskeleton.toolbar.addSeparator();
         */
        Toolbar.prototype.addSeparator = function () {
            this.exoToolbar.AddSeparator();
        };

        // #endregion

        // #region Util

        /**
         * Util API class containing misc utility methods.
         */
        function Util(exoUtil) {
            this.exoUtil = exoUtil;
        }

        // #region KeyStoreAdapter

        /**
         * Persistent Key/Value store which can also be used as a loki database persistence adapter.
         * @param {any} exoFile - pass in reference to exoskeleton singleton
         * @constructor KeyStoreAdapter
         */
        function KeyStoreAdapter(exoFile) {
            this.exoFile = exoFile;
        }

        /**
         * Converts a .NET date to unix format for use with javascript.
         * @param {string} dateString - String representation of a (serialized) .net DateTime object
         * @returns {int} - Number of millseconds since 1/1/1970
         * @memberof Util
         * @instance
         * @example
         * // look up some directory info
         * var dirinfo = exoskeleton.file.getDirectoryInfo("c:\\myfolder");
         * // convert its last write time to unix (number of ms since 1/1/1970)
         * var unixTime = exoskeleton.main.convertDateToUnix(dirinfo.LastWriteTimeUtc);
         * // create a javascript date from unix format
         * var dt = new Date(unixTime);
         */
        Util.prototype.convertDateToUnix = function (dateString) {
            return this.exoUtil.ConvertDateToUnix(dateString);
        };

        /**
         * Converts a javascript unix epoch time to a .net formatted date.
         * See {@link https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings ms docs}
         * @param {number|Date} date - javascript date object or unix epoch time
         * @param {string} format - a .net ToString() format string to apply to date
         * @returns {string} The date formatted to your provided string format
         * @memberof Util
         * @instance
         * @example
         * var now = new Date();
         * // 24 hr date and time
         * var result = exoskeleton.main.formatUnixDate(now, "MM/dd/yy H:mm:ss");
         * alert(result);
         * // formatted date only
         * result = exoskeleton.main.formatUnixDate(now.getTime(), ""MMMM dd, yyyy");
         * alert(result);
         * // formatted time only
         * result = exoskeleton.main.formatUnixDate(now, "hh:mm:ss tt");
         */
        Util.prototype.formatUnixDate = function (date, format) {
            if (typeof date === "object" && date instanceof Date) {
                date = date.getTime();
            }

            return this.exoUtil.FormatUnixDate(date, format);
        };

        // #endregion

        /**
         * An expected method provided for lokijs to load a database from.
         * @param {string} dbname - the name (of the file) to load the database from.
         * @param {function} callback - an optional callback to invoke when loading is complete.
         * @memberof KeyStoreAdapter
         * @instance
         */
        KeyStoreAdapter.prototype.loadDatabase = function (dbname, callback) {
            var result = this.exoFile.loadFile(dbname);
            if (typeof callback === 'function') {
                callback(result);
            }
        };

        /**
         * Used to load a value asynchronously from the keystore.
         * @param {string} dbname - the key name (filename) to load the value (contents) from.
         * @param {function} callback - the callback to invoke with value param when loading is complete.
         * @memberof KeyStoreAdapter
         * @instance
         */
        KeyStoreAdapter.prototype.loadKey = KeyStoreAdapter.prototype.loadDatabase;

        /**
         * An expected method provided for lokijs to save a database from.
         * @param {string} dbname - the name (of the file) to load the database from.
         * @param {string} dbstring - the contents of the serialized database or value to save.
         * @param {function} callback - an optional callback to invoke when loading is complete.
         * @memberof KeyStoreAdapter
         * @instance
         */
        KeyStoreAdapter.prototype.saveDatabase = function (dbname, dbstring, callback) {
            // synchronous? for now
            this.exoFile.saveFile(dbname, dbstring);

            if (typeof callback === 'function') {
                callback(null);
            }
        };

        /**
         * Used to asynchronously save a key/value into the (file based) keystore.
         * @param {string} dbname - the name (of the file) to save the key/value to.
         * @param {function} callback - an optional callback to invoke when saving is complete.
         * @memberof KeyStoreAdapter
         * @instance
         */
        KeyStoreAdapter.prototype.saveKey = KeyStoreAdapter.prototype.saveDatabase;

        // #endregion

        // #region ExoEventEmitter

        /**
         * Event emitter for listening to or emitting events within your page(s)
         *
         * This is exposed via exoskeleton.events.
         * May be used internally to your container or used to emit or listen to
         * 'multicast' events which are propagated to all host windows.
         *
         * Event names starting with 'local.' are not to be multicast,
         * otherwise the event will be multicast with an eventname prefixed
         * with 'multicast.'.
         *
         * @param {object} exo - Instance to the exoskeleton com interface
         * @param {object} options - options to configure event emitter with.
         * @param {bool=} [options.asyncListeners=false] - whether events will be emitted asynchronously.
         * @constructor ExoEventEmitter
         */
        function ExoEventEmitter(exo, options) {
            this.exo = exo;

            options = options || {};
            if (options.asyncListeners) {
                this.asyncListeners = options.asyncListeners;
            }
        }

        /**
         * Hashobject for storing the registered events and callbacks
         */
        ExoEventEmitter.prototype.events = {};

        /**
         * Whether events should be emitted immediately (true) or whenever thread is yielded (false).
         */
        ExoEventEmitter.prototype.asyncListeners = false;

        /**
         * Clears all event listeners
         * @param {string|string[]=} eventName - event(s) to clear listeners, or all if nothing passed.
         * @memberof ExoEventEmitter
         * @instance
         * @example
         * // clear event listeners for all registered events
         * exoskeleton.events.clear();
         * // clear event listeners for a single event
         * exoskeleton.events.clear("FileMenuClicked");
         * // clear event listeners for multiple named events
         * exoskeleton.events.clear(["FileMenuClicked", "HelpMenuClicked"]);
         */
        ExoEventEmitter.prototype.clear = function (eventName) {
            var self = this;

            // if nothing passed, clear all
            if (!eventName) {
                this.events = {};
                return;
            }

            if (Array.isArray(eventName)) {
                eventName.forEach(function (currentEventName) {
                    self.clear(currentEventName);
                });
                return;
            }

            this.events[eventName] = [];
        };

        /**
         * Used to register a listener to an event.
         * @param {string} eventName - name of the event to listen for.
         * @param {function|function[]} listener - a callback to invoke when event is emitted.
         * @returns {function} returns the same listener passed in as a aparam.
         * @memberof ExoEventEmitter
         * @instance
         */
        ExoEventEmitter.prototype.on = function (eventName, listener) {
            var event;
            var self = this;

            if (Array.isArray(eventName)) {
                eventName.forEach(function (currentEventName) {
                    self.on(currentEventName, listener);
                });
                return listener;
            }

            event = this.events[eventName];
            if (!event) {
                event = this.events[eventName] = [];
            }
            event.push(listener);
            return listener;
        };

        /**
         * Used to emit a specific event, possibly with additional parameter data.
         * @param {string} eventName - the name of the event to emit.
         * @param {...string} args - additional parameter data to pass into listener callbacks.
         * @memberof ExoEventEmitter
         * @instance
         */
        ExoEventEmitter.prototype.emit = function (eventName) {
            var self = this;
            var selfArgs = Array.prototype.slice.call(arguments, 1);

            if (eventName && this.events[eventName]) {
                this.events[eventName].forEach(function (listener) {
                    // if locally emitting a multicast event, params will need to be unwrapped before applying them to listener
                    if (eventName.indexOf("multicast.") === 0) {
                        selfArgs = unwrapArguments(selfArgs);
                    }

                    if (self.asyncListeners) {
                        setTimeout(function () {
                            listener.apply(self, selfArgs);
                        }, 1);
                    } else {
                        listener.apply(self, selfArgs);
                    }

                });
            }

            // events whose name starts with 'local.' or 'multicast.' will not be (re)multicast
            if (eventName.indexOf("local.") !== 0 && eventName.indexOf("multicast.") !== 0) {
                this.exo.MulticastEvent(eventName, wrapArguments(selfArgs));
            }
        };

        // #endregion

        // although we instance these for our own use, lets expose the constructors to these classes
        Exoskeleton.EventEmitter = ExoEventEmitter;
        Exoskeleton.KeyStoreAdapter = KeyStoreAdapter;

        return Exoskeleton;
    }());
}));

var exoskeleton = new Exoskeleton(); // jshint ignore:line