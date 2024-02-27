using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Media;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Security.AccessControl;
using Microsoft.Win32;
using SharpTools;

namespace SoundManager
{
    /// <summary>
    /// Hidden form for playing startup, login, logoff, lock, unlock, shutdown sound events.
    /// Builtin playback of these sound events was removed in Windows 8 so we need a background process for that.
    /// Although built-in startup sound can be played on Windows 10+, it is not modifiable and other events are still missing.
    /// This class is NOT useful on Windows XP/Vista/7 and not compatible with Windows XP (No ShutdownBlockReason API, older Task Scheduler API).
    /// </summary>
    public class BgSoundPlayer : Form
    {
        private static readonly string LastBootFile = String.Concat(RuntimeConfig.LocalDataFolder, Path.DirectorySeparatorChar, "LastBootTime.ini");
        private static readonly RegistryKey RegistryHKLM64bits = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
        private static readonly RegistryKey SystemStartup = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        private static readonly RegistryKey StartupDelay = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Serialize");
        private static readonly RegistryKey BootAnimation = RegistryHKLM64bits.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Authentication\\LogonUI\\BootAnimation", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.SetValue);
        private static readonly RegistryKey EditionOverrides = RegistryHKLM64bits.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\EditionOverrides", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.SetValue);
        private static readonly string StartupCommandExe = String.Concat("\"", Application.ExecutablePath, "\"");
        private static readonly string StartupCommand = String.Concat(StartupCommandExe, " ", RuntimeConfig.CmdArgumentBgSoundPlayer);
        private static readonly string SidCurrentUser = System.Security.Principal.WindowsIdentity.GetCurrent().User.Value;
        private static readonly string ScheduledTaskBaseName = RuntimeConfig.AppInternalName;
        private static readonly string ScheduledTaskNameCurrentUser = String.Concat(ScheduledTaskBaseName, "-", SidCurrentUser);
        private const string StartupDelay_StartupDelayInMSec = "StartupDelayInMSec";
        private const string BootAnimation_DisableStartupSound = "DisableStartupSound";
        private const string EditionOverrides_DisableStartupSound = "UserSetting_DisableStartupSound";

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
        /// Check if the current Windows version has a built-in startup sound enabled by default that we need to toggle
        /// </summary>
        private static bool ShouldToggleBuiltinStartupSound
        {
            get
            {
                return WindowsVersion.FriendlyName.ToLowerInvariant().Contains("windows 11");
            }
        }

        /// <summary>
        /// Check registration of the background sound player on system startup
        /// </summary>
        /// <returns>TRUE when registered on system startup</returns>
        public static bool IsRegisteredForStartup()
        {
            bool registryKeyPresent = (StartupCommand == (SystemStartup.GetValue(RuntimeConfig.AppInternalName) as string));
            bool taskPresent = false;

            try
            {
                TaskScheduler.TaskScheduler ts = new TaskScheduler.TaskScheduler();
                ts.Connect();
                taskPresent = (ts.GetFolder("\\").GetTask(ScheduledTaskNameCurrentUser) != null);
            }
            catch (FileNotFoundException) { /* Task not present */ }
            catch (UnauthorizedAccessException) { /* Task is present but wrong permissions */ }

            return registryKeyPresent || taskPresent;
        }

        /// <summary>
        /// Register, unregister the background sound player on system startup
        /// </summary>
        /// <param name="registered">TRUE to register for startup, FALSE to unregister</param>
        /// <param name="interactive">TRUE when user changes the setting interactively, FALSE during setup/uninstall</param>
        public static void SetRegisteredForStartup(bool registered, bool interactive = false)
        {
            TaskScheduler.TaskScheduler ts = new TaskScheduler.TaskScheduler();
            ts.Connect();

            if (registered)
            {
                //Create registry keys - Currently disabled due to startup delay on registry keys, replaced by scheduled task
                //SystemStartup.SetValue(Program.InternalName, StartupCommand);
                //StartupDelay.SetValue(StartupDelayInMSec, 0, RegistryValueKind.DWord);

                //Create scheduled task - Runs sooner on logon compared to registry keys

                string taskSecurityDescriptor = String.Concat("O:", SidCurrentUser, "D:(A;;FA;;;", SidCurrentUser, ")");
                TaskScheduler.ITaskDefinition task = ts.NewTask(0);
                TaskScheduler.ILogonTrigger trigger = (TaskScheduler.ILogonTrigger)task.Triggers.Create(TaskScheduler._TASK_TRIGGER_TYPE2.TASK_TRIGGER_LOGON);
                trigger.UserId = SidCurrentUser;
                TaskScheduler.IExecAction action = (TaskScheduler.IExecAction)task.Actions.Create(TaskScheduler._TASK_ACTION_TYPE.TASK_ACTION_EXEC);
                action.Path = StartupCommandExe;
                action.Arguments = RuntimeConfig.CmdArgumentBgSoundPlayer;
                task.Settings.DisallowStartIfOnBatteries = false;
                task.Settings.StopIfGoingOnBatteries = false;
                task.Settings.ExecutionTimeLimit = "PT0S";
                task.Settings.Priority = 5; // Normal

                try
                {
                    ts.GetFolder("\\").RegisterTaskDefinition(
                        ScheduledTaskNameCurrentUser,
                        task,
                        (int)TaskScheduler._TASK_CREATION.TASK_CREATE_OR_UPDATE,
                        null,
                        null,
                        TaskScheduler._TASK_LOGON_TYPE.TASK_LOGON_INTERACTIVE_TOKEN,
                        taskSecurityDescriptor
                    );
                }
                catch (UnauthorizedAccessException)
                {
                    //Not allowed to create the scheduled task because it already exists with wrong permissions
                    //Should not happen since tasks are per-user, but better warn the user.
                    if (interactive)
                        throw;
                }

                if (ShouldToggleBuiltinStartupSound)
                {
                    //Disable built-in startup sound
                    BootAnimation.SetValue(BootAnimation_DisableStartupSound, 1);
                    EditionOverrides.SetValue(EditionOverrides_DisableStartupSound, 1);
                }
            }
            else
            {
                //Remove scheduled task for the current user
                try { ts.GetFolder("\\").DeleteTask(ScheduledTaskNameCurrentUser, 0); }
                catch (FileNotFoundException) { /* Task was not present */ }
                catch (UnauthorizedAccessException) /* Insufficient privileges */
                {
                    //Should not happen since tasks are per-user, but better warn the user.
                    if (interactive)
                        throw;
                }

                //Also remove tasks for other users when performing Uninstall as Admin
                if (!interactive && FileSystemAdmin.IsAdmin())
                {
                    TaskScheduler.IRegisteredTaskCollection tasks = ts.GetFolder("\\").GetTasks(0);
                    for (int i = 1; i <= tasks.Count; i++)
                        if (tasks[i].Path.StartsWith("\\" + ScheduledTaskBaseName))
                            ts.GetFolder("\\").DeleteTask(tasks[i].Path.Substring(1), 0);
                }

                //Re-Enable built-in startup sound from Windows 11+
                if (ShouldToggleBuiltinStartupSound)
                {
                    BootAnimation.SetValue(BootAnimation_DisableStartupSound, 0);
                    EditionOverrides.SetValue(EditionOverrides_DisableStartupSound, 0);
                }
            }

            //Remove registry keys set by previous versions of this program
            SystemStartup.DeleteValue(RuntimeConfig.AppInternalName, false);
            StartupDelay.DeleteValue(StartupDelay_StartupDelayInMSec, false);

            //Attempt to remove generic scheduled task set by previous versions of this program
            try { ts.GetFolder("\\").DeleteTask(RuntimeConfig.AppInternalName, 0); }
            catch (FileNotFoundException) { /* Task was not present */ }
            catch (UnauthorizedAccessException) { /* Insufficient privileges */ }
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
        /// Check if the user has not reached the Desktop yet
        /// </summary>
        private static bool IsScreenLocked()
        {
            switch (WindowManager.GetActiveWindowExeName().ToLower())
            {
                case "idle.exe":
                case "lockapp.exe":
                case "logonui.exe":
                    return true;
                default:
                    return false;
            }
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
            this.Text = RuntimeConfig.AppDisplayName;
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
            SoundEvent soundToPlay = soundLogon;
            if (bootTime != lastBootTime && File.Exists(soundStartup.FilePath))
            {
                File.WriteAllText(LastBootFile, bootTime);
                soundToPlay = soundStartup;
            }
            while (IsScreenLocked())
                Thread.Sleep(100);
            PlaySound(soundToPlay);

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
