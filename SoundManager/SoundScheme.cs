using System;
using System.IO;
using System.Media;
using System.Collections.Generic;
using Microsoft.Win32;
using NAudio.Wave;
using SharpTools;
using System.Security.AccessControl;
using System.Security.Principal;

namespace SoundManager
{
    /// <summary>
    /// Manage the SoundManager Windows sound scheme
    /// </summary>
    public class SoundScheme
    {
        private static readonly string SchemeManager = Program.InternalName;
        private const string SchemeDefault = ".Default";
        private const string SchemeCurrent = ".Current";

        private const string RegSchemes = "AppEvents\\Schemes";
        private const string RegNames = RegSchemes + "\\Names\\";
        private const string RegApps = RegSchemes + "\\Apps\\";

        private static readonly RegistryKey RegCurrentUser = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default);

        // ======================== //
        // == Sound scheme object == //
        // ======================== //

        private string displayName;
        private string internalName;

        /// <summary>
        /// Represents a system sound scheme
        /// </summary>
        /// <param name="displayName">Display name</param>
        /// <param name="internalName">Internal name</param>
        private SoundScheme(string displayName, string internalName)
        {
            this.displayName = displayName;
            this.internalName = internalName;
        }

        /// <summary>
        /// Get display name for the sound scheme
        /// </summary>
        public override string ToString()
        {
            return displayName;
        }

        // ============================ //
        // == Sound scheme management == //
        // ============================ //

        /// <summary>
        /// Create the "SoundManager" sound scheme in registry
        /// </summary>
        public static void Setup()
        {
            RegistryKey name = RegCurrentUser.CreateSubKey(RegNames + SchemeManager);
            name.SetValue(null, Program.DisplayName);
            name.Close();

            foreach (SoundEvent soundEvent in SoundEvent.GetAll())
            {
                foreach (string registryKey in soundEvent.RegistryKeys)
                {
                    string eventKeyPath = RegApps + registryKey + '\\' + SchemeManager;
                    RegistryKey eventKey = RegCurrentUser.OpenSubKey(eventKeyPath, true) ?? RegCurrentUser.CreateSubKey(eventKeyPath);
                    eventKey.SetValue(null, soundEvent.FilePath);
                    eventKey.Close();
                }
            }

            //Windows 7 : Also backup imageres.dll when creating sound scheme
            if (ImageresPatcher.IsWindowsVista7 && FileSystemAdmin.IsAdmin())
                ImageresPatcher.Backup();
        }

        /// <summary>
        /// Copy default sound from the specified sound scheme to the SoundManager sound scheme
        /// </summary>
        /// <param name="source">If not specified, sounds are copied from the default sound sheme</param>
        public static void CopyDefault(SoundEvent soundEvent, SoundScheme source = null)
        {
            bool defaultFileFound = false;

            string originalScheme = SchemeDefault;
            if (source != null)
                originalScheme = source.internalName;

            foreach (string registryKey in soundEvent.RegistryKeys)
            {
                RegistryKey defaultSoundKey = RegCurrentUser.OpenSubKey(RegApps + registryKey + '\\' + originalScheme);
                string defaultSoundPath = null;
                if (defaultSoundKey != null)
                    defaultSoundPath = Environment.ExpandEnvironmentVariables(defaultSoundKey.GetValue(null) as string ?? "");
                if (!Directory.Exists(Path.GetDirectoryName(soundEvent.FilePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(soundEvent.FilePath));
                if (File.Exists(defaultSoundPath))
                {
                    Update(soundEvent, defaultSoundPath);
                    defaultFileFound = true;
                }
            }
            if (soundEvent.Imageres && ImageresPatcher.IsWindowsVista7)
            {
                if (FileSystemAdmin.IsAdmin())
                    ImageresPatcher.Restore();
                ImageresPatcher.ExtractDefault(soundEvent.FilePath);
            }
            else if (!defaultFileFound)
            {
                Remove(soundEvent);
            }
        }

        /// <summary>
        /// Get sound file path for the specified event from registry.
        /// Will return file path for the currently applied sound scheme, which is not necessarily the SoundManager sound scheme.
        /// </summary>
        /// <param name="soundEvent">SoundEvent to query</param>
        /// <returns>Sound file path or NULL if no file was found</returns>
        public static string GetCurrentFile(SoundEvent soundEvent)
        {
            if (soundEvent != null)
            {
                foreach (string registryKey in soundEvent.RegistryKeys)
                {
                    RegistryKey currentSoundKey = RegCurrentUser.OpenSubKey(RegApps + registryKey + '\\' + SchemeCurrent);
                    string currentSoundPath = null;
                    if (currentSoundKey != null)
                        currentSoundPath = Environment.ExpandEnvironmentVariables(currentSoundKey.GetValue(null) as string ?? "");
                    if (currentSoundPath != null && File.Exists(currentSoundPath))
                        return currentSoundPath;
                }
            }
            return null;
        }

        /// <summary>
        /// Get the list of all sound schemes in registry
        /// </summary>
        /// <returns>A list of sound shemes, key is display name, value is internal name</returns>
        public static SoundScheme[] GetSchemeList()
        {
            List<SoundScheme> schemes = new List<SoundScheme>();
            RegistryKey names = RegCurrentUser.OpenSubKey(RegNames);
            if (names != null)
            {
                foreach (string internalName in names.GetSubKeyNames())
                {
                    if (!String.IsNullOrEmpty(internalName))
                    {
                        RegistryKey schemeName = names.OpenSubKey(internalName);
                        if (schemeName != null)
                        {
                            string displayName = schemeName.GetValue(null) as string;
                            if (String.IsNullOrWhiteSpace(displayName) || displayName[0] == '@')
                                displayName = internalName;
                            schemes.Add(new SoundScheme(displayName, internalName));
                        }
                    }
                }
            }
            return schemes.ToArray();
        }

        /// <summary>
        /// Get the default sound scheme
        /// </summary>
        public static SoundScheme GetSchemeDefault()
        {
            return new SoundScheme(SchemeDefault, SchemeDefault);
        }

        /// <summary>
        /// Get the "SoundManager" sound scheme
        /// </summary>
        public static SoundScheme GetSchemeSoundManager()
        {
            return new SoundScheme(SchemeManager, SchemeManager);
        }

        /// <summary>
        /// Remove the sound associated with a sound event from the SoundManager sound scheme
        /// </summary>
        /// <param name="soundEvent">Sound event to remove</param>
        public static void Remove(SoundEvent soundEvent)
        {
            Update(soundEvent, null);
        }

        /// <summary>
        /// Check if sound conversion is supported on the current Windows version for the Update method
        /// </summary>
        public static bool CanConvertSounds
        {
            get
            {
                return WindowsVersion.WinMajorVersion > 6
                    || (WindowsVersion.WinMajorVersion == 6 && WindowsVersion.WinMinorVersion >= 1);
            }
        }

        /// <summary>
        /// Update the sound event with a new sound file in the SoundManager sound scheme
        /// </summary>
        /// <param name="soundEvent">Sound event to update</param>
        /// <param name="soundFile">New sound file. Null value or non-existing file has the same effect as calling Remove().</param>
        /// <remarks>If soundFile is set to soundEvent.FilePath, the sound file is not updated but triggers additional steps such as refreshing imageres.dll</remarks>
        public static void Update(SoundEvent soundEvent, string soundFile)
        {
            if (soundFile != soundEvent.FilePath)
            {
                if (File.Exists(soundFile))
                {
                    MediaFoundationReader soundReader = null;
                    if (CanConvertSounds)
                        soundReader = new MediaFoundationReader(soundFile);

                    if (CanConvertSounds && soundReader.TotalTime > TimeSpan.FromSeconds(30))
                    {
                        throw new InvalidOperationException(Translations.Get("sound_file_too_long"));
                    }
                    else
                    {
                        try
                        {
                            // Check for WAV format by trying to play it and directly copy the WAV file
                            SoundPlayer player = new SoundPlayer(soundFile);
                            player.Play();
                            player.Stop();
                            File.Copy(soundFile, soundEvent.FilePath, true);
                            File.SetAttributes(soundEvent.FilePath, FileAttributes.Normal);
                        }
                        catch (InvalidOperationException playException)
                        {
                            if (CanConvertSounds)
                            {
                                // Transcode non-native input file formats into WAV format that Windows can play
                                using (WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(soundReader))
                                {
                                    try
                                    {
                                        WaveFileWriter.CreateWaveFile(soundEvent.FilePath, pcm);
                                    }
                                    catch
                                    {
                                        File.Delete(soundEvent.FilePath);
                                        throw;
                                    }
                                }
                            }
                            else
                            {
                                File.Delete(soundEvent.FilePath);
                                throw playException;
                            }
                        }
                    }
                    if (soundReader != null)
                        soundReader.Dispose();
                }
                else File.Delete(soundEvent.FilePath);
            }

            //Windows 7 : Also patch imageres.dll when updating startup sound
            if (soundEvent.Imageres && ImageresPatcher.IsWindowsVista7 && FileSystemAdmin.IsAdmin())
                ImageresPatcher.Patch(soundEvent.FilePath);

            //Windows 10 : Also make sure file read access is set for All Application Packages, otherwise UWP UI parts will not be able to play the sound event
            if (File.Exists(soundEvent.FilePath) && WindowsVersion.WinMajorVersion >= 10)
            {
                FileInfo fileInfo = new FileInfo(soundEvent.FilePath);
                FileSecurity fileSecurity = fileInfo.GetAccessControl();
                fileSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier("S-1-15-2-1"), FileSystemRights.ReadAndExecute, AccessControlType.Allow));
                fileInfo.SetAccessControl(fileSecurity);
            }
        }

        /// <summary>
        /// Remove the "SoundManager" sound scheme from registry
        /// </summary>
        public static void Uninstall()
        {
            string currentScheme = RegCurrentUser.OpenSubKey(RegSchemes).GetValue(null) as string;
            if (currentScheme == SchemeManager)
                Apply(GetSchemeDefault(), true);

            RegistryKey apps = RegCurrentUser.OpenSubKey(RegApps);
            foreach (string appName in apps.GetSubKeyNames())
            {
                RegistryKey app = apps.OpenSubKey(appName);
                foreach (string soundName in app.GetSubKeyNames())
                {
                    RegistryKey sound = app.OpenSubKey(soundName, true);
                    if (sound.OpenSubKey(SchemeManager) != null)
                        sound.DeleteSubKey(SchemeManager);
                    sound.Close();
                }
            }

            if (RegCurrentUser.OpenSubKey(RegNames + SchemeManager) != null)
                RegCurrentUser.DeleteSubKey(RegNames + SchemeManager);

            //Windows 7 : Also restore imageres.dll when removing sound scheme
            try
            {
                if (ImageresPatcher.IsWindowsVista7 && FileSystemAdmin.IsAdmin())
                    ImageresPatcher.Restore();
            }
            catch (FileNotFoundException) { /* No imageres backup to restore */ }
            catch (UnauthorizedAccessException) { /* Insufficient privileges or file locked */ }
        }

        /// <summary>
        /// Apply the specified sound scheme in registry
        /// </summary>
        /// <param name="scheme">Sound scheme to apply</param>
        /// <param name="missingSoundsUseDefault">When a sound is missing, use the default system sound instead.</param>
        public static void Apply(SoundScheme scheme, bool missingSoundsUseDefault)
        {
            string schemeName = SchemeDefault;
            if (scheme != null)
                schemeName = scheme.internalName;

            RegistryKey nameKey = RegCurrentUser.OpenSubKey(RegNames + schemeName);
            if (nameKey == null)
                throw new ArgumentException("The specified scheme does not exist in Registry.", "schemeName");
            else nameKey.Close();

            RegistryKey apps = RegCurrentUser.OpenSubKey(RegApps);
            foreach (string appName in apps.GetSubKeyNames())
            {
                RegistryKey app = apps.OpenSubKey(appName);
                foreach (string soundName in app.GetSubKeyNames())
                {
                    RegistryKey sound = app.OpenSubKey(soundName, true);
                    RegistryKey schemeSound = sound.OpenSubKey(schemeName);
                    RegistryKey defaultSound = sound.OpenSubKey(SchemeDefault);
                    RegistryKey currentSound = sound.OpenSubKey(SchemeCurrent, true) ?? sound.CreateSubKey(SchemeCurrent);

                    string soundPath = null;
                    if (schemeSound != null)
                        soundPath = schemeSound.GetValue(null) as string;
                    if ((soundPath == null || !File.Exists(Environment.ExpandEnvironmentVariables(soundPath))) && defaultSound != null && missingSoundsUseDefault)
                        soundPath = defaultSound.GetValue(null) as string;
                    if (soundPath != null && !File.Exists(Environment.ExpandEnvironmentVariables(soundPath)))
                        soundPath = null;

                    currentSound.SetValue(null, soundPath ?? "");
                    currentSound.Close();
                    sound.Close();
                }
            }

            RegistryKey currentScheme = RegCurrentUser.OpenSubKey(RegSchemes, true);
            currentScheme.SetValue(null, schemeName);
            currentScheme.Close();
        }
    }
}
