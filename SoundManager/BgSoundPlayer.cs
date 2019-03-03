using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Media;
using System.IO;
using SharpTools;
using Microsoft.Win32;
using System.Drawing;

namespace SoundManager
{
    /// <summary>
    /// Hidden form for playing login/logoff sound events.
    /// Builtin playback of these sound events was removed in Windows 8 so we need a background process for that.
    /// This class is NOT useful on Windows XP/Vista/7 and not compatible with Windows XP (No ShutdownBlockReason API).
    /// </summary>
    public class BgSoundPlayer : Form
    {
        private static readonly string LastBootFile = String.Concat(Program.DataFolder, Path.DirectorySeparatorChar, "LastBootTime.ini");
        private static readonly RegistryKey SystemStartup = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        private static readonly RegistryKey StartupDelay = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Serialize");
        private static readonly string StartupCommand = String.Concat("\"", Application.ExecutablePath, "\" ", Program.ArgumentBgSoundPlayer);
        private const string StartupDelayInMSec = "StartupDelayInMSec";

        /// <summary>
        /// Check if the background sound player is required for the current Windows version
        /// </summary>
        public static bool RequiredForThisWindowsVersion
        {
            get
            {
                return (WindowsVersion.WinMajorVersion == 6 && WindowsVersion.WinMinorVersion >= 2)
                    || WindowsVersion.WinMajorVersion >= 10;
            }
        }

        /// <summary>
        /// Register, unregister, check registration of the background sound player on system startup
        /// </summary>
        public static bool RegisteredForStartup
        {
            get
            {
                return StartupCommand == (SystemStartup.GetValue(Program.InternalName) as string);
            }
            set
            {
                if (value)
                {
                    SystemStartup.SetValue(Program.InternalName, StartupCommand);
                    StartupDelay.SetValue(StartupDelayInMSec, 0, RegistryValueKind.DWord);
                }
                else
                {
                    SystemStartup.DeleteValue(Program.InternalName, false);
                    StartupDelay.DeleteValue(StartupDelayInMSec, false);
                }
            }
        }

        [DllImport("kernel32")]
        private static extern UInt64 GetTickCount64();

        [DllImport("user32.dll")]
        private static extern bool ShutdownBlockReasonCreate(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] string pwszReason);

        [DllImport("user32.dll")]
        private static extern bool ShutdownBlockReasonDestroy(IntPtr wndHandle);

        /// <summary>
        /// Get boot unix timestamp in seconds
        /// </summary>
        private static long GetBootTimestamp()
        {
            long remainder;
            return Math.DivRem(((long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds - (long)GetTickCount64()), 1000, out remainder);
        }

        /// <summary>
        /// Play the specified system event sound.
        /// The sound currently associated with the event is played, which is not necessarily from the SoundManager scheme.
        /// </summary>
        /// <param name="soundEvent">System event to play</param>
        private static void PlaySound(SoundEvent soundEvent)
        {
            if (soundEvent != null)
            {
                string soundFile = SoundScheme.GetCurrentFile(soundEvent);
                if (soundFile != null && File.Exists(soundFile))
                {
                    try
                    {
                        new SoundPlayer(soundFile).PlaySync();
                    }
                    catch (InvalidOperationException) { /* Invalid WAV file */ }
                }
            }
        }

        private SoundEvent soundStartup;
        private SoundEvent soundShutdown;
        private SoundEvent soundLogon;
        private SoundEvent soundLogoff;

        /// <summary>
        /// Instantiate a new Background Sound Player.
        /// Will play the startup/logon sound and create a hidden window, which is required by the ShutdownBlockReason API.
        /// </summary>
        public BgSoundPlayer()
        {
            this.Text = Program.DisplayName;
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new System.Drawing.Point(-2000, -2000);
            this.Size = new System.Drawing.Size(1, 1);

            foreach (SoundEvent soundEvent in SoundEvent.GetAll())
            {
                switch (soundEvent.FileName.Replace(".wav", "").ToLower())
                {
                    case "startup":
                        soundStartup = soundEvent;
                        break;
                    case "shutdown":
                        soundShutdown = soundEvent;
                        break;
                    case "logon":
                        soundLogon = soundEvent;
                        break;
                    case "logoff":
                        soundLogoff = soundEvent;
                        break;
                }
            }

            string bootTime = GetBootTimestamp().ToString();
            bootTime = bootTime.Substring(0, bootTime.Length - 1) + '0';
            string lastBootTime = "";
            if (File.Exists(LastBootFile))
                lastBootTime = File.ReadAllText(LastBootFile);
            if (bootTime != lastBootTime && File.Exists(soundStartup.FilePath))
            {
                File.WriteAllText(LastBootFile, bootTime);
                PlaySound(soundStartup);
            }
            else PlaySound(soundLogon);

            SystemEvents.SessionEnding += new SessionEndingEventHandler(SystemEvents_SessionEnding);
        }

        /// <summary>
        /// Detect user logging off and play the appropriate logoff/shutdown sound
        /// </summary>
        private void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            if (e.Reason == SessionEndReasons.SystemShutdown && File.Exists(soundShutdown.FilePath))
            {
                ShutdownBlockReasonCreate(this.Handle, Translations.Get("playing_shutdown_sound"));
                PlaySound(soundShutdown);
            }
            else
            {
                ShutdownBlockReasonCreate(this.Handle, Translations.Get("playing_logoff_sound"));
                PlaySound(soundLogoff);
            }
            ShutdownBlockReasonDestroy(this.Handle);
        }
    }
}
