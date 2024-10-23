using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using SharpTools;

namespace SoundManager
{
    /// <summary>
    /// Holds constants used in various parts of the program.
    /// Some constants depend on the environment, calculated once on launch.
    /// </summary>
    public class RuntimeConfig
    {
        /// <summary>
        /// Display name of the program, e.g. "Sound Manager"
        /// </summary>
        public static readonly string AppDisplayName = Translations.Get("app_name");

        /// <summary>
        /// Internal name of the program, e.g. "SoundManager"
        /// </summary>
        public static readonly string AppInternalName = typeof(RuntimeConfig).Namespace;

        /// <summary>
        /// Path to the folder containing the executables (SoundManager.exe / DownloadShemes.exe)
        /// </summary>
        public static readonly string AppFolder = Path.GetDirectoryName(Application.ExecutablePath);

        /// <summary>
        /// Current version of the program
        /// </summary>
        public const string Version = "3.3.0";

        /// <summary>
        /// Command-line argument for performing automated post-setup operations
        /// </summary>
        public const string CmdArgumentSetup = "--setup";

        /// <summary>
        /// Command-line argument for performing automated pre-uninstall operations
        /// </summary>
        public const string CmdArgumentUninstall = "--uninstall";

        /// <summary>
        /// Command-line argument for running the background sound player for startup/shutdown sound
        /// </summary>
        public const string CmdArgumentBgSoundPlayer = "--bg-sounds";

        /// <summary>
        /// Minimum supported system version (Windows NT 5.1 = Windows XP)
        /// </summary>
        public const string SupportedWindowsVersionMin = "5.1";

        /// <summary>
        /// Maximum supported system version (Windows NT 10.0 = Windows 10 or Windows 11)
        /// </summary>
        public const string SupportedWindowsVersionMax = "10.0";

        /// <summary>
        /// Local folder for storing configuration and current sound scheme
        /// </summary>
        public static readonly string LocalDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            AppInternalName
        );

        /// <summary>
        /// Path to the local configuration file holding program Settings
        /// </summary>
        public static readonly string SettingsFile = Path.Combine(LocalDataFolder, AppInternalName + ".ini");

        /// <summary>
        /// GitHub User name for repository holding the sound schemes library
        /// </summary>
        public const string SchemesRepositoryUsername = "ORelio";

        /// <summary>
        /// GitHub Repository name holding the sound schemes library 
        /// </summary>
        public const string SchemesRepositoryName = "Sound-Manager-Schemes";

        /// <summary>
        /// Direct link to the GitHub Repository holding the sound schemes library
        /// </summary>
        public const string SchemesRepositoryUrl = "https://github.com/" + SchemesRepositoryUsername + "/" + SchemesRepositoryName + "/";

        /// <summary>
        /// Path to the SoundManager executable (SoundManager.exe)
        /// </summary>
        public static readonly string SoundManagerExe = Path.Combine(AppFolder, AppInternalName + ".exe");

        /// <summary>
        /// File name of the Sound Scheme icon (for file types association)
        /// </summary>
        public const string SoundSchemeIcon = "SoundScheme.ico";

        /// <summary>
        /// Path to the Uninstall program, (Uninstall.exe)
        /// </summary>
        public static readonly string UninstallProgramExe = Path.Combine(AppFolder, "Uninstall.exe");

        /// <summary>
        /// Determine whether the program is running in Portable mode
        /// </summary>
        public static readonly bool RunningInPortableMode = !File.Exists(UninstallProgramExe);

        /// <summary>
        /// Determine whether the program is running on Windows 11, which spoofs Windows 10.0 in its NT Kernel version
        /// </summary>
        public static readonly bool RunningWindows11 = WindowsVersion.FriendlyName.ToLowerInvariant().Contains("windows 11");

        /// <summary>
        /// Current system (Windows NT) version. Windows 11 spoofs Windows 10.0, so its version string is "10.0_11" to tell it apart from Windows 10. 
        /// </summary>
        public static readonly string WindowsNtVersion = String.Format("{0}.{1}", WindowsVersion.WinMajorVersion, WindowsVersion.WinMinorVersion) + (RunningWindows11 ? "_11" : "");

        /// <summary>
        /// Path to the local Sound Schemes Folder. Contains Schemes downloaded using the DownloadSchemes tool.
        /// </summary>
        public static readonly string SchemesFolder = RunningInPortableMode
            ? Path.Combine(AppFolder, "Schemes")
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), Translations.Get("app_name"));
    }
}
