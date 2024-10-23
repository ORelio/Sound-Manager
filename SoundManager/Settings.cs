﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SharpTools;

namespace SoundManager
{
    /// <summary>
    /// Holds global application settings
    /// </summary>
    static class Settings
    {
        /// <summary>
        /// Specify whether the Windows startup sound patch feature is enabled
        /// </summary>
        public static bool PatchStartupSound { get; set; }

        /// <summary>
        /// Specify whether a missing sound should be replaced by the default system sound when loading a sound archive file
        /// </summary>
        public static bool MissingSoundUseDefault { get; set; }

        /// <summary>
        /// Specify whether the program should convert proprietary files into the SoundManager file format. If enabled, move the proprietary file to recycle bin.
        /// </summary>
        public static bool ConvertProprietaryFiles { get; set; }

        /// <summary>
        /// Specify whether the program should prefer Startup/Shutdown instead of Logon/Logoff. Only play Logon/Logoff when switching users like Windows XP does.
        /// </summary>
        public static bool PreferStartupSoundOnLogon { get; set; }

        /// <summary>
        /// Static class initializer to automatically load settings
        /// </summary>
        static Settings()
        {
            Load();
        }

        /// <summary>
        /// Load the INI file with application settings or load default settings
        /// </summary>
        public static void Load()
        {
            if (File.Exists(RuntimeConfig.SettingsFile))
            {
                var settingsRaw = INIFile.ParseFile(RuntimeConfig.SettingsFile);
                foreach (var settingsSection in settingsRaw)
                {
                    switch (settingsSection.Key.ToLower())
                    {
                        case "main":
                            foreach (var setting in settingsSection.Value)
                            {
                                switch (setting.Key.ToLower())
                                {
                                    case "win7patch": // old setting name
                                    case "patchstartupsound":
                                        PatchStartupSound = INIFile.Str2Bool(setting.Value);
                                        break;

                                    case "usedefaultonmissingsound":
                                        MissingSoundUseDefault = INIFile.Str2Bool(setting.Value);
                                        break;

                                    case "convertproprietaryfiles":
                                        ConvertProprietaryFiles = INIFile.Str2Bool(setting.Value);
                                        break;

                                    case "preferstartupsoundonlogon":
                                        PreferStartupSoundOnLogon = INIFile.Str2Bool(setting.Value);
                                        break;
                                }
                            }
                            break;
                    }
                }
            }
            else
            {
                PatchStartupSound = ImageresPatcher.IsPatchingRequired;
                MissingSoundUseDefault = true;
            }
        }

        /// <summary>
        /// Write the INI file with application settings
        /// </summary>
        public static void Save()
        {
            var settings = new Dictionary<string, Dictionary<string, string>>();

            settings["Main"] = new Dictionary<string, string>();
            settings["Main"]["PatchStartupSound"] = PatchStartupSound.ToString();
            settings["Main"]["UseDefaultOnMissingSound"] = MissingSoundUseDefault.ToString();
            settings["Main"]["ConvertProprietaryFiles"] = ConvertProprietaryFiles.ToString();
            settings["Main"]["PreferStartupSoundOnLogon"] = PreferStartupSoundOnLogon.ToString();

            INIFile.WriteFile(RuntimeConfig.SettingsFile, settings, RuntimeConfig.AppInternalName + " Configuration File", false);
        }
    }
}
