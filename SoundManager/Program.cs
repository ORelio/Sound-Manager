using System;
using System.Windows.Forms;
using System.Linq;
using SharpTools;
using System.Diagnostics;
using System.IO;

namespace SoundManager
{
    /// <summary>
    /// Application allowing to create, load and share Windows sound schemes
    /// By ORelio - (c) 2009-2019 - Available under the CDDL-1.0 license
    /// </summary>
    public static class Program
    {
        public static readonly string DisplayName = Translations.Get("app_name");
        public static readonly string InternalName = typeof(Program).Namespace;
        public const string Version = "3.0.1";

        public const string WindowsVersionMin = "5.1";
        public const string WindowsVersionMax = "10.0";

        public const string ArgumentSetup = "--setup";
        public const string ArgumentUninstall = "--uninstall";
        public const string ArgumentBgSoundPlayer = "--bg-sounds";

        public static readonly string DataFolder = String.Concat(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            Path.DirectorySeparatorChar,
            typeof(Program).Namespace
        );

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string importFile = null;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length > 0)
            {
                switch (args[0].ToLowerInvariant())
                {
                    case ArgumentSetup:
                        Setup(false, true);
                        Environment.Exit(0);
                        break;

                    case ArgumentUninstall:
                        Uninstall();
                        Environment.Exit(0);
                        break;

                    case ArgumentBgSoundPlayer:
                        Application.Run(new BgSoundPlayer());
                        Environment.Exit(0);
                        break;

                    default:
                        if (File.Exists(args[0]))
                        {
                            importFile = args[0];
                        }
                        else
                        {
                            Console.Error.WriteLine(String.Concat(Program.InternalName, " <", ArgumentSetup, '|', ArgumentUninstall, '|', ArgumentBgSoundPlayer, '>'));
                            Environment.Exit(1);
                        }
                        break;
                }
            }

            if (ImageresPatcher.IsWindowsVista7 && !FileSystemAdmin.IsAdmin() && Settings.WinVista7PatchEnabled)
            {
                try
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo(Application.ExecutablePath);
                    if (args.Length > 0)
                        startInfo.Arguments = "\"" + args[0] + "\"";
                    startInfo.Verb = "runas";
                    Process.Start(startInfo);
                    Environment.Exit(0);
                }
                catch
                {
                    MessageBox.Show(
                        Translations.Get("windows7_not_elevated_text"),
                        Translations.Get("windows7_not_elevated_title"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }

            Setup(false, false);
            Application.Run(new FormMain(importFile));
        }

        /// <summary>
        /// Setup will create and apply the SoundManager sound scheme, create data directory
        /// </summary>
        /// <param name="forceResetSounds">Also reset all sounds to their default values</param>
        /// <param name="systemIntegration">Also setup maximum system integration</param>
        public static void Setup(bool forceResetSounds, bool systemIntegration)
        {
            bool createDataDir = !Directory.Exists(DataFolder);

            if (createDataDir)
                Directory.CreateDirectory(DataFolder);

            SoundScheme.Setup();

            if (forceResetSounds || createDataDir)
                foreach (SoundEvent soundEvent in SoundEvent.GetAll())
                    SoundScheme.CopyDefault(soundEvent);

            SoundScheme.Apply(SoundScheme.GetSchemeSoundManager(), true);

            if (systemIntegration)
            {
                SoundArchive.AssocFiles();
                if (BgSoundPlayer.RequiredForThisWindowsVersion)
                {
                    BgSoundPlayer.RegisteredForStartup = true;
                    Process.Start(Application.ExecutablePath, ArgumentBgSoundPlayer);
                }
            }
        }

        /// <summary>
        /// Uninstall will remove application data from the user directory, sound scheme from the registry, and disable all system integration
        /// </summary>
        /// <returns></returns>
        public static void Uninstall()
        {
            if (BgSoundPlayer.RegisteredForStartup)
            {
                BgSoundPlayer.RegisteredForStartup = false;
                foreach (Process process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Application.ExecutablePath)))
                    if (process.Id != Process.GetCurrentProcess().Id)
                        process.Kill();
            }
            SoundArchive.UnAssocFiles();
            SoundScheme.Uninstall();
            if (Directory.Exists(SoundEvent.DataDirectory))
                Directory.Delete(SoundEvent.DataDirectory, true);
            if (Directory.Exists(DataFolder))
                Directory.Delete(DataFolder, true);
        }
    }
}
