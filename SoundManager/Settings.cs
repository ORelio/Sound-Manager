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
        private static readonly string ConfigFile = Path.Combine(RuntimeConfig.LocalDataFolder, RuntimeConfig.AppInternalName + ".ini");

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
            if (File.Exists(ConfigFile))
            {
                var settingsRaw = INIFile.ParseFile(ConfigFile);
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
            var config = new Dictionary<string, Dictionary<string, string>>();

            config["main"] = new Dictionary<string, string>();
            config["main"]["patchstartupsound"] = PatchStartupSound.ToString();
            config["main"]["usedefaultonmissingsound"] = MissingSoundUseDefault.ToString();
            config["main"]["convertproprietaryfiles"] = ConvertProprietaryFiles.ToString();

            INIFile.WriteFile(ConfigFile, config, RuntimeConfig.AppInternalName + " Configuration File");
        }
    }
}
