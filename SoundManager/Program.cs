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
    /// By ORelio - (c) 2009-2025 - Available under the CDDL-1.0 license
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (RuntimeConfig.Version.ToLowerInvariant().Contains("test"))
                ExceptionLogger.StartLogging(Application.ExecutablePath + ".debug.log", RuntimeConfig.Version);

            string importFile = null;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length > 0)
            {
                switch (args[0].ToLowerInvariant())
                {
                    case RuntimeConfig.CmdArgumentSetup:
                        Setup(forceResetSounds: false, systemIntegration: true, offerImportCurrentScheme: false);
                        Environment.Exit(0);
                        break;

                    case RuntimeConfig.CmdArgumentUninstall:
                        Uninstall();
                        Environment.Exit(0);
                        break;

                    case RuntimeConfig.CmdArgumentBgSoundPlayer:
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
                            Console.Error.WriteLine(String.Concat(RuntimeConfig.AppInternalName, " <", RuntimeConfig.CmdArgumentSetup, '|', RuntimeConfig.CmdArgumentUninstall, '|', RuntimeConfig.CmdArgumentBgSoundPlayer, '>'));
                            Environment.Exit(1);
                        }
                        break;
                }
            }

            if (ImageresPatcher.IsPatchingPossible && !FileSystemAdmin.IsAdmin() && Settings.PatchStartupSound)
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
                        Translations.Get("startup_patch_not_elevated_text"),
                        Translations.Get("startup_patch_not_elevated_title"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }

            Setup(forceResetSounds: false, systemIntegration: false, offerImportCurrentScheme: importFile == null);
            Application.Run(new FormMain(importFile));
        }

        /// <summary>
        /// Setup will create and apply the SoundManager sound scheme, create data directory
        /// </summary>
        /// <param name="forceResetSounds">Also reset all sounds to their default values</param>
        /// <param name="systemIntegration">Also setup maximum system integration</param>
        /// <param name="offerImportCurrentScheme">Offer to import the active scheme if changed externally</param>
        public static void Setup(bool forceResetSounds, bool systemIntegration, bool offerImportCurrentScheme)
        {
            bool createDataDir = !Directory.Exists(RuntimeConfig.LocalDataFolder);

            if (createDataDir)
            {
                Directory.CreateDirectory(RuntimeConfig.LocalDataFolder);
                Directory.CreateDirectory(SoundEvent.DataDirectory);
            }

            SoundScheme activeScheme = SoundScheme.GetActiveScheme();
            if (offerImportCurrentScheme && SoundScheme.AlreadySetup() && activeScheme != null && !activeScheme.IsSchemeManager)
            {
                if (MessageBox.Show(
                    String.Format("{0}\n\n{1}", Translations.Get("auto_import_offer_text"), activeScheme.ToString()),
                    Translations.Get("auto_import_offer_title"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    forceResetSounds = true;
                }
            }

            SoundScheme.Setup();

            if (forceResetSounds || createDataDir)
            {
                SchemeMeta.ResetAll();
                if (activeScheme != null && !activeScheme.IsDefault)
                {
                    SchemeMeta.Name = activeScheme.ToString();
                    SchemeMeta.Author = "";
                    SchemeMeta.About = "";
                }
                foreach (SoundEvent soundEvent in SoundEvent.GetAll())
                    SoundScheme.CopyDefault(soundEvent, activeScheme);
            }

            SoundScheme.Apply(SoundScheme.GetSchemeSoundManager(), true);

            if (systemIntegration)
            {
                SoundArchive.AssocFiles();
                if (BgSoundPlayer.RequiredForThisWindowsVersion)
                {
                    SystemStartupSound.Enabled = false;
                    BgSoundPlayer.SetRegisteredForStartup(true);
                    Process.Start(Application.ExecutablePath, RuntimeConfig.CmdArgumentBgSoundPlayer);
                }
            }
        }

        /// <summary>
        /// Uninstall will remove application data from the user directory, sound scheme from the registry, and disable all system integration
        /// </summary>
        /// <returns></returns>
        public static void Uninstall()
        {
            if (BgSoundPlayer.IsRegisteredForStartup())
            {
                SystemStartupSound.Enabled = SystemStartupSound.DefaultEnabled;
                BgSoundPlayer.SetRegisteredForStartup(false);
                foreach (Process process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Application.ExecutablePath)))
                    if (process.Id != Process.GetCurrentProcess().Id)
                        process.Kill();
            }
            SoundArchive.UnAssocFiles();
            SoundScheme.Uninstall();
            if (Directory.Exists(SoundEvent.DataDirectory))
                Directory.Delete(SoundEvent.DataDirectory, true);
            if (Directory.Exists(RuntimeConfig.LocalDataFolder))
                Directory.Delete(RuntimeConfig.LocalDataFolder, true);
        }
    }
}
