using System;
using Microsoft.Win32;
using System.Security.AccessControl;
using SharpTools;

namespace SoundManager
{
    /// <summary>
    /// Wrapper around the Enable/Disable startup sound Windows setting (mmsys.cpl > Sounds > [X] Play Windows Startup Sound)
    /// By ORelio - (c) 2018-2024 - Available under the CDDL-1.0 license
    /// </summary>
    static class SystemStartupSound
    {
        private static readonly RegistryKey RegistryHKLM64bits = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
        private static readonly RegistryKey BootAnimation = RegistryHKLM64bits.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Authentication\\LogonUI\\BootAnimation", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.SetValue | RegistryRights.QueryValues); // Windows Vista - 10
        private static readonly RegistryKey EditionOverrides = RegistryHKLM64bits.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\EditionOverrides", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.SetValue | RegistryRights.QueryValues); // Windows 11
        private const string BootAnimation_DisableStartupSound = "DisableStartupSound";
        private const string EditionOverrides_DisableStartupSound = "UserSetting_DisableStartupSound";

        /// <summary>
        /// Enable or disable built-in Windows Startup sound
        /// </summary>
        public static bool Enabled
        {
            get
            {
                return !GetSetDisableStartupSound();
            }
            set
            {
                GetSetDisableStartupSound(!value);
            }
        }

        /// <summary>
        /// Get default enable status for the current Windows version.
        /// The startup sound is enabled by default except on Windows 8 and 10.
        /// </summary>
        public static bool DefaultEnabled
        {
            get
            {
                return !(WindowsVersion.Is8 || WindowsVersion.Is10);
            }
        }

        /// <summary>
        /// Get or set the Disable Startup Sound registry key
        /// </summary>
        /// <param name="setValue"></param>
        /// <returns></returns>
        private static bool GetSetDisableStartupSound(bool? disabled = null)
        {
            // Old Windows versions do not have the Disable Startup Sound registry setting
            if (!WindowsVersion.IsAtLeastVista)
                return false;

            // Default disable status when registry value is missing
            int regDefault = DefaultEnabled ? 0 : 1;

            // Determine registry key. Windows 11 uses a different registry key
            RegistryKey regKey = BootAnimation;
            string regValueName = BootAnimation_DisableStartupSound;
            if (WindowsVersion.Is11)
            {
                regKey = EditionOverrides;
                regValueName = EditionOverrides_DisableStartupSound;
            }

            // Set disable status
            if (disabled.HasValue)
                regKey.SetValue(regValueName, disabled.Value ? 1 : 0, RegistryValueKind.DWord);

            // Retrieve disable status
            int? val = regKey.GetValue(regValueName) as int?;
            if (val.HasValue)
                return val.Value != 0;
            return regDefault != 0;
        }
    }
}
