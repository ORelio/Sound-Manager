using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpTools;

namespace SoundManager
{
    /// <summary>
    /// Describes all sound events handled by the application
    /// </summary>
    public class SoundEvent
    {
        private static readonly SoundEvent[] allEvents;
        public static readonly string DataDirectory = Path.Combine(RuntimeConfig.LocalDataFolder, "Media");
        public enum EventType { Startup, Shutdown, Logon, Logoff }; // Events needing special treatment

        /// <summary>
        /// Get all supported event types supported by the application
        /// </summary>
        /// <returns>Event objects</returns>
        public static SoundEvent[] GetAll()
        {
            return allEvents;
        }

        /// <summary>
        /// Get sound event by type, for events requiring special treatment
        /// </summary>
        /// <param name="eventType">Event type missing in newer Windows version</param>
        /// <returns>SoundEvent object</returns>
        public static SoundEvent Get(EventType eventType)
        {
            //Assertion: All EventTypes must exist in allEvents, so no need for FirstOrDefault.
            return allEvents.First(e => e.Type == eventType);
        }

        private string _displayName;
        private string _description;
        private string _filePath;
        private string _fileName;
        private string _legacyFileName;
        private string[] _regKeys;
        private EventType? _eventType;

        /// <summary>
        /// Create a new sound event
        /// </summary>
        /// <param name="name">Sound name. Used for naming sound files and displaying translations</param>
        /// <param name="regKeys">Registry keys where the sound event is stored (some Windows versions may use different keys)</param>
        /// <param name="legacyFilename">Legacy file name for old sound archive format. Taken from Windows XP FR</param>
        /// <param name="eventType">Specify the sound event type corresponding to this item, for events needing special treatment</param>
        private SoundEvent(string name, string[] regKeys, string legacyFilename, EventType? eventType)
        {
            this._displayName = Translations.Get("event_" + name.ToLower() + "_name");
            this._description = Translations.Get("event_" + name.ToLower() + "_desc");
            this._filePath = Path.Combine(DataDirectory, name + ".wav");
            this._legacyFileName = "Windows XP " + legacyFilename + ".wav";
            this._fileName = name + ".wav";
            this._regKeys = regKeys;
            this._eventType = eventType;
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
        /// Specify the sound event type corresponding to this item, for events needing special treatment
        /// </summary>
        public EventType? Type { get { return _eventType; } }

        /// <summary>
        /// Initialize all sound events
        /// </summary>
        /// <remarks>
        /// Windows Vista and greater startup sound is embedded in imageres.dll which requires patching to update the sound resource, see ImageresPatcher.cs for more details
        /// Windows Vista to 8 balloon sound is mistakenly read from Explorer instead of .Default, see https://winaero.com/blog/fix-windows-plays-no-sound-for-tray-balloon-tips-notifications/
        /// Windows 8 and greater does not play the logon, logoff, shutdown sounds at all, these need to be reimplemented, see BgSoundPlayer.cs
        /// Windows 10 has various notification sounds and no more "balloon" sound, the sound event is associated with default notification
        /// </remarks>
        static SoundEvent()
        {
            allEvents = new []{
                // ================================================================================================================================================================
                //              Sound Name                   Registry Key(s)                      Old file name (XP FR)          Event Type (for events needing special treatment)
                // ================================================================================================================================================================
                new SoundEvent("Startup",           new []{ ".Default\\SystemStart" },           "Démarrage",                    EventType.Startup ),
                new SoundEvent("Shutdown",          new []{ ".Default\\SystemExit" },            "Arrêt du système",             EventType.Shutdown),
                new SoundEvent("Logon",             new []{ ".Default\\WindowsLogon" },          "Ouverture de session",         EventType.Logon   ),
                new SoundEvent("Logoff",            new []{ ".Default\\WindowsLogoff" },         "Fermeture de session",         EventType.Logoff  ),
                new SoundEvent("Information",       new []{ ".Default\\SystemAsterisk" },        "Erreur",                       null              ),
                new SoundEvent("Question",          new []{ ".Default\\SystemQuestion" },        null,                           null              ),
                new SoundEvent("Warning",           new []{ ".Default\\SystemExclamation" },     "Exclamation",                  null              ),
                new SoundEvent("Error",             new []{ ".Default\\SystemHand" },            "Arrêt critique",               null              ),
                new SoundEvent("DeviceConnect",     new []{ ".Default\\DeviceConnect" },         "Insertion d'un matériel",      null              ),
                new SoundEvent("DeviceDisconnect",  new []{ ".Default\\DeviceDisconnect" },      "Suppression d'un matériel",    null              ),
                new SoundEvent("DeviceFail",        new []{ ".Default\\DeviceFail" },            "Échec d'un matériel",          null              ),
                new SoundEvent("Default",           new []{ ".Default\\.Default" },              "Ding",                         null              ),
                new SoundEvent("Balloon",           new []{ ".Default\\SystemNotification",
                                                            ".Default\\Notification.Default",
                                                            "Explorer\\SystemNotification", },   "Infobulle",                    null              ),
                new SoundEvent("Navigate",          new []{ "Explorer\\Navigating" },            "Menu Démarrer",                null              ),
                new SoundEvent("RecycleBin",        new []{ "Explorer\\EmptyRecycleBin" },       "Corbeille",                    null              ),
                new SoundEvent("UAC",               new []{ ".Default\\WindowsUAC" },            null,                           null              ),
                new SoundEvent("BatteryLow",        new []{ ".Default\\LowBatteryAlarm" },       null,                           null              ),
                new SoundEvent("BatteryCritical",   new []{ ".Default\\CriticalBatteryAlarm" },  null,                           null              ),
                new SoundEvent("Email",             new []{ ".Default\\MailBeep" },              null,                           null              ),
                new SoundEvent("Print",             new []{ ".Default\\PrintComplete" },         null,                           null              ),
                new SoundEvent("AppOpen",           new []{ ".Default\\Open" },                  null,                           null              ),
                new SoundEvent("AppClose",          new []{ ".Default\\Close" },                 null,                           null              ),
                new SoundEvent("Minimize",          new []{ ".Default\\Minimize" },              null,                           null              ),
                new SoundEvent("UnMinimize",        new []{ ".Default\\RestoreUp" },             null,                           null              ),
                new SoundEvent("Maximize",          new []{ ".Default\\Maximize" },              null,                           null              ),
                new SoundEvent("UnMaximize",        new []{ ".Default\\RestoreDown" },           null,                           null              ),
                new SoundEvent("Menu",              new []{ ".Default\\MenuPopup" },             null,                           null              ),
                new SoundEvent("MenuCommand",       new []{ ".Default\\MenuCommand" },           null,                           null              ),
                new SoundEvent("Select",            new []{ ".Default\\CCSelect" },              null,                           null              ),
                // ====================================================================================================================================================
                //              Sound names above should not be modified to retain compatibility with sound archives and BgSoundPlayer
                // ====================================================================================================================================================
            };
        }
    }
}
