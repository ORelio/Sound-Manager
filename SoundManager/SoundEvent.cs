using System;
using System.Collections.Generic;
using System.IO;
using SharpTools;

namespace SoundManager
{
    /// <summary>
    /// Describes all sound events handled by the application
    /// </summary>
    public class SoundEvent
    {
        private static readonly SoundEvent[] allEvents;
        public static readonly string DataDirectory = String.Concat(RuntimeConfig.LocalDataFolder, Path.DirectorySeparatorChar, "Media");

        /// <summary>
        /// Get all supported event types supported by the application
        /// </summary>
        /// <returns>Event types</returns>
        public static SoundEvent[] GetAll()
        {
            return allEvents;
        }

        private string _displayName;
        private string _description;
        private string _filePath;
        private string _fileName;
        private string _legacyFileName;
        private string[] _regKeys;
        private bool _imageres;

        /// <summary>
        /// Create a new sound event
        /// </summary>
        /// <param name="name">Sound name. Used for naming sound files and displaying translations</param>
        /// <param name="regKeys">Registry keys where the sound event is stored (some Windows versions may use different keys)</param>
        /// <param name="legacyFilename">Legacy file name for old sound archive format. Taken from Windows XP FR</param>
        /// <param name="imageres">Specify whether the sound is stored in imageres.dll on Windows 7</param>
        private SoundEvent(string name, string[] regKeys, string legacyFilename = null, bool imageres = false)
        {
            this._displayName = Translations.Get("event_" + name.ToLower() + "_name");
            this._description = Translations.Get("event_" + name.ToLower() + "_desc");
            this._filePath = String.Concat(DataDirectory, Path.DirectorySeparatorChar, name, ".wav");
            this._legacyFileName = "Windows XP " + legacyFilename + ".wav";
            this._fileName = name + ".wav";
            this._regKeys = regKeys;
            this._imageres = imageres;
        }

        /// <summary>
        /// Display name of the sound event
        /// </summary>
        public string DisplayName { get { return _displayName; } }

        /// <summary>
        /// Text description of the sound event
        /// </summary>
        public string Description { get { return _description; } }

        /// <summary>
        /// File path for the sound event
        /// </summary>
        public string FilePath { get { return _filePath; } }

        /// <summary>
        /// File name for the sound event
        /// </summary>
        public string FileName { get { return _fileName; } }

        /// <summary>
        /// Legacy file name from old sound archive format (may be null)
        /// </summary>
        public string LegacyFileName { get { return _legacyFileName; } }

        /// <summary>
        /// Registry keys holding the sound event
        /// </summary>
        public string[] RegistryKeys { get { return _regKeys; } }

        /// <summary>
        /// Specify whether the sound is stored in imageres.dll on Windows 7
        /// </summary>
        public bool Imageres { get { return _imageres; } }

        /// <summary>
        /// Initialize all sound events
        /// </summary>
        /// <remarks>
        /// Windows 7 startup sound is embedded in imageres.dll which requires patching to update the sound resource, see ImageresPatcher.cs for more details
        /// Windows 7 balloon sound is mistakenly read from Explorer instead of .Default, see https://winaero.com/blog/fix-windows-plays-no-sound-for-tray-balloon-tips-notifications/
        /// Windows 8 and greater does not play the startup and shutdown sounds at all, these need to be reimplemented, see BgSoundPlayer.cs
        /// Windows 10 has various notification sounds and no more "balloon" sound, the sound event is associated with default notification
        /// </remarks>
        static SoundEvent()
        {
            allEvents = new []{
                // ===========================================================================================================================
                //              Sound Name                   Registry Key(s)                      Old file name (XP FR)        imageres.dll
                // ===========================================================================================================================
                new SoundEvent("Startup",           new []{ ".Default\\SystemStart" },           "Démarrage",                    true),
                new SoundEvent("Shutdown",          new []{ ".Default\\SystemExit" },            "Arrêt du système"                  ),
                new SoundEvent("Logon",             new []{ ".Default\\WindowsLogon" },          "Ouverture de session"              ),
                new SoundEvent("Logoff",            new []{ ".Default\\WindowsLogoff" },         "Fermeture de session"              ),
                new SoundEvent("Information",       new []{ ".Default\\SystemAsterisk" },        "Erreur"                            ),
                new SoundEvent("Question",          new []{ ".Default\\SystemQuestion" },        null                                ),
                new SoundEvent("Warning",           new []{ ".Default\\SystemExclamation" },     "Exclamation"                       ),
                new SoundEvent("Error",             new []{ ".Default\\SystemHand" },            "Arrêt critique"                    ),
                new SoundEvent("DeviceConnect",     new []{ ".Default\\DeviceConnect" },         "Insertion d'un matériel"           ),
                new SoundEvent("DeviceDisconnect",  new []{ ".Default\\DeviceDisconnect" },      "Suppression d'un matériel"         ),
                new SoundEvent("DeviceFail",        new []{ ".Default\\DeviceFail" },            "Échec d'un matériel"               ),
                new SoundEvent("Default",           new []{ ".Default\\.Default" },              "Ding"                              ),
                new SoundEvent("Balloon",           new []{ ".Default\\SystemNotification",
                                                            ".Default\\Notification.Default",
                                                            "Explorer\\SystemNotification", },   "Infobulle"                         ),
                new SoundEvent("Navigate",          new []{ "Explorer\\Navigating" },            "Menu Démarrer"                     ),
                new SoundEvent("RecycleBin",        new []{ "Explorer\\EmptyRecycleBin" },       "Corbeille"                         ),
                new SoundEvent("UAC",               new []{ ".Default\\WindowsUAC" },            null                                ),
                new SoundEvent("BatteryLow",        new []{ ".Default\\LowBatteryAlarm" },       null                                ),
                new SoundEvent("BatteryCritical",   new []{ ".Default\\CriticalBatteryAlarm" },  null                                ),
                new SoundEvent("Email",             new []{ ".Default\\MailBeep" },              null                                ),
                new SoundEvent("Print",             new []{ ".Default\\PrintComplete" },         null                                ),
                new SoundEvent("AppOpen",           new []{ ".Default\\Open" },                  null                                ),
                new SoundEvent("AppClose",          new []{ ".Default\\Close" },                 null                                ),
                new SoundEvent("Minimize",          new []{ ".Default\\Minimize" },              null                                ),
                new SoundEvent("UnMinimize",        new []{ ".Default\\RestoreUp" },             null                                ),
                new SoundEvent("Maximize",          new []{ ".Default\\Maximize" },              null                                ),
                new SoundEvent("UnMaximize",        new []{ ".Default\\RestoreDown" },           null                                ),
                new SoundEvent("Menu",              new []{ ".Default\\MenuPopup" },             null                                ),
                new SoundEvent("MenuCommand",       new []{ ".Default\\MenuCommand" },           null                                ),
                new SoundEvent("Select",            new []{ ".Default\\CCSelect" },              null                                )
                // ==========================================================================================================================
                //       Sound names above should not be modified to retain compatibility with sound archives and BgSoundPlayer
                // ==========================================================================================================================
            };
        }
    }
}
