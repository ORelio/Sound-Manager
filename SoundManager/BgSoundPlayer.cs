using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Media;
using System.IO;
using System.Drawing;
using System.Threading;
using Microsoft.Win32;
using SharpTools;

namespace SoundManager
{
    /// <summary>
    /// Hidden form for playing login, logoff, lock, unlock, shutdown sound events.
    /// Builtin playback of these sound events was removed in Windows 8 so we need a background process for that.
    /// This class is NOT useful on Windows XP/Vista/7 and not compatible with Windows XP (No ShutdownBlockReason API).
    /// </summary>
    public class BgSoundPlayer : Form
    {
        private static readonly string LastBootFile = String.Concat(Program.DataFolder, Path.DirectorySeparatorChar, "LastBootTime.ini");
        private static readonly RegistryKey SystemStartup = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        private static readonly RegistryKey StartupDelay = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Serialize");
        private static readonly string StartupCommandExe = String.Concat("\"", Application.ExecutablePath, "\"");
        private static readonly string StartupCommand = String.Concat(StartupCommandExe, " ", Program.ArgumentBgSoundPlayer);
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
                bool registryKeyPresent = (StartupCommand == (SystemStartup.GetValue(Program.InternalName) as string));
                bool taskPresent = false;

                try
                {
                    TaskScheduler.TaskScheduler ts = new TaskScheduler.TaskScheduler();
                    ts.Connect();
                    taskPresent = (ts.GetFolder("\\").GetTask(Program.InternalName) != null);
                }
                catch (FileNotFoundException) { /* Task not present */ }

                return registryKeyPresent || taskPresent;
            }
            set
            {
                TaskScheduler.TaskScheduler ts = new TaskScheduler.TaskScheduler();
                ts.Connect();

                if (value)
                {
                    //Create registry keys - Currently disabled due to startup delay on registry keys, replaced by scheduled task
                    //SystemStartup.SetValue(Program.InternalName, StartupCommand);
                    //StartupDelay.SetValue(StartupDelayInMSec, 0, RegistryValueKind.DWord);

                    //Create scheduled task - Runs sooner on logon compared to registry keys
                    TaskScheduler.ITaskDefinition task = ts.NewTask(0);
                    TaskScheduler.ILogonTrigger trigger = (TaskScheduler.ILogonTrigger)task.Triggers.Create(TaskScheduler._TASK_TRIGGER_TYPE2.TASK_TRIGGER_LOGON);
                    trigger.UserId = System.Security.Principal.WindowsIdentity.GetCurrent().User.Value;
                    TaskScheduler.IExecAction action = (TaskScheduler.IExecAction)task.Actions.Create(TaskScheduler._TASK_ACTION_TYPE.TASK_ACTION_EXEC);
                    action.Path = StartupCommandExe;
                    action.Arguments = Program.ArgumentBgSoundPlayer;
                    task.Settings.DisallowStartIfOnBatteries = false;
                    task.Settings.StopIfGoingOnBatteries = false;
                    task.Settings.ExecutionTimeLimit = "PT0S";
                    task.Settings.Priority = 5; // Normal
                    ts.GetFolder("\\").RegisterTaskDefinition(Program.InternalName, task, (int)TaskScheduler._TASK_CREATION.TASK_CREATE_OR_UPDATE, null, null, TaskScheduler._TASK_LOGON_TYPE.TASK_LOGON_INTERACTIVE_TOKEN, "");
                }
                else
                {
                    //Remove scheduled task
                    try { ts.GetFolder("\\").DeleteTask(Program.InternalName, 0); }
                    catch (FileNotFoundException) { /* Task was not present */ }
                }

                //Remove registry keys set by previous versions
                SystemStartup.DeleteValue(Program.InternalName, false);
                StartupDelay.DeleteValue(StartupDelayInMSec, false);
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
            SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch);
        }

        /// <summary>
        /// Detect user logging off and play the appropriate logoff/shutdown sound
        /// </summary>
        private void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            if (e.Reason == SessionEndReasons.SystemShutdown && File.Exists(soundShutdown.FilePath))
            {
                ShutdownBlockReasonCreate(this.Handle, Translations.Get("playing_shutdown_sound"));
                File.Delete(LastBootFile); // Force Startup sound next time
                PlaySound(soundShutdown);
            }
            else
            {
                ShutdownBlockReasonCreate(this.Handle, Translations.Get("playing_logoff_sound"));
                PlaySound(soundLogoff);
            }
            ShutdownBlockReasonDestroy(this.Handle);
        }

        /// <summary>
        /// Detect user leaving and resuming session
        /// </summary>
        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock && File.Exists(soundLogoff.FilePath))
            {
                PlaySound(soundLogoff);
            }
            else if (e.Reason == SessionSwitchReason.SessionUnlock && File.Exists(soundLogon.FilePath))
            {
                PlaySound(soundLogon);
            }
        }
    }
}
