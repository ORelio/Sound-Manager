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
        private static readonly string LastBootFile = Path.Combine(RuntimeConfig.LocalDataFolder, "LastBootTime.ini");
        private static readonly RegistryKey SystemStartup = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        private static readonly RegistryKey StartupDelay = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Serialize");
        private static readonly string StartupCommandExe = String.Concat("\"", Application.ExecutablePath, "\"");
        private static readonly string StartupCommand = String.Concat(StartupCommandExe, " ", RuntimeConfig.CmdArgumentBgSoundPlayer);
        private static readonly string SidCurrentUser = System.Security.Principal.WindowsIdentity.GetCurrent().User.Value;
        private static readonly string ScheduledTaskBaseName = RuntimeConfig.AppInternalName;
        private static readonly string ScheduledTaskNameCurrentUser = String.Concat(ScheduledTaskBaseName, "-", SidCurrentUser);
        private const string StartupDelay_StartupDelayInMSec = "StartupDelayInMSec";

        /// <summary>
        /// Check if the background sound player is required for the current Windows version
        /// </summary>
        public static bool RequiredForThisWindowsVersion
        {
            get
            {
                return WindowsVersion.IsAtLeast8;
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

        private SoundEvent soundStartup = SoundEvent.Get(SoundEvent.EventType.Startup);
        private SoundEvent soundShutdown = SoundEvent.Get(SoundEvent.EventType.Shutdown);
        private SoundEvent soundLogon = SoundEvent.Get(SoundEvent.EventType.Logon);
        private SoundEvent soundLogoff = SoundEvent.Get(SoundEvent.EventType.Logoff);

        /// <summary>
        /// Instantiate a new Background Sound Player.
        /// Will play the startup/logon sound and create a hidden window, which is required by the ShutdownBlockReason API.
        /// </summary>
        public BgSoundPlayer()
        {
            //this.Text = RuntimeConfig.AppDisplayName; // Window title disabled, see below (issue #8)
            this.Icon = IconExtractor.ExtractAssociatedIcon(Application.ExecutablePath);

            // Hide the window in such a way Windows will still consider it eligible for ShutdownBlockReason
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow; // Remove from Alt+Tab
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new System.Drawing.Point(-9999, -9999);
            this.Size = new System.Drawing.Size(1, 1);
            this.GotFocus += new EventHandler(WindowFocused); // Evade focus (issue #8)
            this.Text = "�"; // Avoid screen readers saying the window title loud (issue #8)

            // Determine system startup time
            string bootTime = GetBootTimestamp().ToString();
            bootTime = bootTime.Substring(0, bootTime.Length - 1) + '0';
            string lastBootTime = "";
            if (File.Exists(LastBootFile))
                lastBootTime = File.ReadAllText(LastBootFile);

            // Determine default logon sound to play
            SoundEvent soundToPlay =
                Settings.PreferStartupSoundOnLogon && File.Exists(soundStartup.FilePath)
                    ? soundStartup
                    : soundLogon;

            // Handle case where system has rebooted - startup sound
            if (bootTime != lastBootTime)
            {
                File.WriteAllText(LastBootFile, bootTime);
                if (SystemStartupSound.Enabled)
                {
                    soundToPlay = null; // Built-in system startup sound will play already
                }
                else if (File.Exists(soundStartup.FilePath))
                {
                    soundToPlay = soundStartup; // We need to emulate the startup sound
                }
            }

            // Play sound event?
            if (soundToPlay != null)
            {
                while (IsScreenLocked())
                    Thread.Sleep(100);
                PlaySound(soundToPlay);
            }

            SystemEvents.SessionEnding += new SessionEndingEventHandler(SystemEvents_SessionEnding);
            SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch);
        }

        /// <summary>
        /// Detect user logging off and play the appropriate logoff/shutdown sound
        /// </summary>
        private void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            this.Text = RuntimeConfig.AppDisplayName;

            if ((e.Reason == SessionEndReasons.SystemShutdown || Settings.PreferStartupSoundOnLogon) && File.Exists(soundShutdown.FilePath))
            {
                ShutdownBlockReasonCreate(this.Handle, Translations.Get("playing_shutdown_sound"));
                if (e.Reason == SessionEndReasons.SystemShutdown)
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

        /// <summary>
        /// Detect when the background sound player window is focused
        /// </summary>
        private void WindowFocused(object sender, EventArgs e)
        {
            // Refuse focus - Some screen readers may pick up the window (issue #8)
            // Try switching focus to desktop, or as fallback, toggle the minimized state
            try
            {
                // Cannot stay minimized because it may show a window title next to the task bar
                this.WindowState = FormWindowState.Minimized;
                this.WindowState = FormWindowState.Normal;

                // Switch focus to the Windows Desktop's folderView
                IntPtr desktop = IntPtr.Zero;
                if (WindowManager.GetDesktopWindow(ref desktop))
                    WindowManager.SetForegroundWindow(desktop);
            }
            catch
            {
                // Avoid crashes linked to this workaround
            }
        }

        /// <summary>
        /// Override "Show without activation" property to not focus the window on launch
        /// </summary>
        protected override bool ShowWithoutActivation
        {
            get
            {
                return true;
            }
        }
    }
}
