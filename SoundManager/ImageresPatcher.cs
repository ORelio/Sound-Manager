using System;
using System.IO;
using SharpTools;
using System.Security.Principal;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SoundManager
{
    /// <summary>
    /// Utility class for patching startup sound for Windows 7 embedded in imageres.dll
    /// </summary>
    /// <remarks>
    /// https://www.sevenforums.com/tutorials/63398-startup-sound-change-windows-7-a.html
    /// </remarks>
    static class ImageresPatcher
    {
        private static readonly int WaveResourceNumber = WindowsVersion.WinMinorVersion == 0 ? 5051 : 5080;
        private static readonly bool SystemIsWindowsVista7 = WindowsVersion.WinMajorVersion == 6 && WindowsVersion.WinMinorVersion <= 1;

        private static readonly string System32 = Environment.GetFolderPath(Environment.SpecialFolder.Windows) + (Environment.Is64BitOperatingSystem ? @"\Sysnative\" : @"\System32\");
        private static readonly string Imageres = System32 + "imageres.dll";
        private static readonly string ImageresBak = Imageres + ".bak";
        private static readonly string ImageresOld = Imageres + ".old";

        private static readonly string ResHackerExe = Path.Combine(RuntimeConfig.AppFolder, "ResHacker.exe");
        private static readonly string ResHackerIni = ResHackerExe.Replace(".exe", ".ini");
        private static readonly string ResHackerLog = ResHackerExe.Replace(".exe", ".log");

        private static readonly byte[] emptyWavFile = new byte[]
        {
            0x52, 0x49, 0x46, 0x46, 0x3e, 0x00, 0x00, 0x00, 0x57, 0x41, 0x56, 0x45, 0x66, 0x6d, 0x74, 0x20,
            0x12, 0x00, 0x00, 0x00, 0x01, 0x00, 0x02, 0x00, 0x44, 0xac, 0x00, 0x00, 0x10, 0xb1, 0x02, 0x00,
            0x04, 0x00, 0x10, 0x00, 0x00, 0x00, 0x66, 0x61, 0x63, 0x74, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x64, 0x61, 0x74, 0x61, 0x00, 0x00, 0x00, 0x00, 0x4c, 0x49, 0x53, 0x54, 0x04, 0x00,
            0x00, 0x00, 0x49, 0x4e, 0x46, 0x4f
        };

        /// <summary>
        /// Check whethere the operating system is Windows Vista or Windows 7 and requires Imageres patching
        /// </summary>
        /// <returns>TRUE if the operating system is Windows Vista or Windows 7</returns>
        public static bool IsWindowsVista7
        {
            get
            {
                return SystemIsWindowsVista7;
            }
        }

        /// <summary>
        /// Unlock Imageres and perform a backup
        /// </summary>
        public static void Backup()
        {
            if (SystemIsWindowsVista7)
            {
                if (File.Exists(Imageres))
                {
                    if (FileSystemAdmin.IsAdmin())
                    {
                        FileSystemAdmin.GrantAll(System32);
                        FileSystemAdmin.GrantAll(Imageres);

                        if (File.Exists(ImageresBak))
                        {
                            FileSystemAdmin.GrantAll(ImageresBak);
                        }
                        else
                        {
                            File.Move(Imageres, ImageresBak);
                            File.Copy(ImageresBak, Imageres);
                        }
                    }
                    else throw new UnauthorizedAccessException(Translations.Get("startup_patch_not_admin"));
                }
                else throw new FileNotFoundException(Translations.Get("startup_patch_no_imageres_dll"));
            }
            else throw new InvalidOperationException(Translations.Get("startup_patch_not_windows7"));
        }

        /// <summary>
        /// Restore imageres from backup
        /// </summary>
        public static void Restore()
        {
            if (SystemIsWindowsVista7)
            {
                if (File.Exists(ImageresBak))
                {
                    if (FileSystemAdmin.IsAdmin())
                    {
                        FileSystemAdmin.GrantAll(System32);
                        FileSystemAdmin.GrantAll(ImageresBak);
                        FileSystemAdmin.GrantAll(Imageres);

                        try
                        {
                            //Move in-use DLL so that we can restore the original one
                            File.Delete(ImageresOld);
                            File.Move(Imageres, ImageresOld);
                        }
                        catch (UnauthorizedAccessException) { }

                        File.Delete(Imageres);
                        File.Move(ImageresBak, Imageres);
                    }
                    else throw new UnauthorizedAccessException(Translations.Get("startup_patch_not_admin"));
                }
            }
            else throw new InvalidOperationException(Translations.Get("startup_patch_not_windows7"));
        }

        /// <summary>
        /// Patch imageres to update embedded startup sound
        /// </summary>
        /// <param name="replacementStartupSound">Replacement startup sound. Must be a PCM WAV file.</param>
        public static void Patch(string replacementStartupSound)
        {
            if (SystemIsWindowsVista7)
            {
                if (File.Exists(Imageres) || File.Exists(ImageresBak))
                {
                    if (FileSystemAdmin.IsAdmin())
                    {
                        if (File.Exists(ResHackerExe))
                        {
                            if (!File.Exists(ImageresBak))
                                Backup();

                            try
                            {
                                //Move in-use DLL so that we can create a new one
                                File.Delete(ImageresOld);
                                File.Move(Imageres, ImageresOld);
                            }
                            catch (UnauthorizedAccessException) { }

                            bool deleteAfterPatching = false;

                            if (!File.Exists(replacementStartupSound))
                            {
                                File.WriteAllBytes(replacementStartupSound, emptyWavFile);
                                deleteAfterPatching = true;
                            }

                            Process.Start(ResHackerExe, "-modify \"" + ImageresBak + "\", \"" + Imageres + "\", \"" + replacementStartupSound + "\", WAVE, " + WaveResourceNumber + ",").WaitForExit();
                            File.Delete(ResHackerIni);
                            File.Delete(ResHackerLog);

                            if (deleteAfterPatching)
                                File.Delete(replacementStartupSound);

                            if (!File.Exists(Imageres))
                                File.Copy(ImageresBak, Imageres);
                        }
                        else throw new FileNotFoundException(Translations.Get("startup_patch_no_resource_hacker"));
                    }
                    else throw new UnauthorizedAccessException(Translations.Get("startup_patch_not_admin"));
                }
                else throw new FileNotFoundException(Translations.Get("startup_patch_no_imageres_dll"));
            }
            else throw new InvalidOperationException(Translations.Get("startup_patch_not_windows7"));
        }

        /// <summary>
        /// Extract the default startup sound file
        /// </summary>
        /// <param name="outputFile">Output file path</param>
        public static void ExtractDefault(string outputFile)
        {
            if (SystemIsWindowsVista7)
            {
                if (File.Exists(Imageres) || File.Exists(ImageresBak))
                {
                    string sourceFile = ImageresBak;
                    if (!File.Exists(ImageresBak))
                        sourceFile = Imageres;

                    if (File.Exists(ResHackerExe))
                    {
                        Process.Start(ResHackerExe, "-extract \"" + sourceFile + "\", \"" + outputFile + "\", WAVE, " + WaveResourceNumber + ",").WaitForExit();
                    }
                    else throw new FileNotFoundException(Translations.Get("startup_patch_no_resource_hacker"));
                }
                else throw new FileNotFoundException(Translations.Get("startup_patch_no_imageres_dll"));
            }
            else throw new InvalidOperationException(Translations.Get("startup_patch_not_windows7"));
        }
    }
}
