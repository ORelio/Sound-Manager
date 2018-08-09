using System;
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
        private static readonly string ConfigFile = String.Concat(Program.DataFolder, Path.DirectorySeparatorChar, Program.InternalName, ".ini");

        /// <summary>
        /// Specify whether the Windows Vista/7 startup sound patch feature is enabled
        /// </summary>
        public static bool WinVista7PatchEnabled { get; set; }

        /// <summary>
        /// Specify whether a missing sound should be replaced by the default system sound when loading a sound archive file
        /// </summary>
        public static bool MissingSoundUseDefault { get; set; }

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
                                    case "win7patch":
                                        WinVista7PatchEnabled = INIFile.Str2Bool(setting.Value);
                                        break;

                                    case "usedefaultonmissingsound":
                                        MissingSoundUseDefault = INIFile.Str2Bool(setting.Value);
                                        break;
                                }
                            }
                            break;
                    }
                }
            }
            else
            {
                WinVista7PatchEnabled = true;
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
            config["main"]["win7patch"] = WinVista7PatchEnabled.ToString();
            config["main"]["usedefaultonmissingsound"] = MissingSoundUseDefault.ToString();

            INIFile.WriteFile(ConfigFile, config, Program.InternalName + " Configuration File");
        }
    }
}
