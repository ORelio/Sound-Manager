using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;

namespace SharpTools
{
    /// <summary>
    /// Programatically interact with on-screen windows using Windows API
    /// By ORelio - (c) 2013-2022 - Available under the CDDL-1.0 license
    /// </summary>
    public static class WindowManager
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int GetWindowModuleFileName(IntPtr hWnd, StringBuilder text, int count);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr handle, out RECT lpRect);

        private struct POINTAPI
        {
            public int x;
            public int y;
        }

        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        private struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public POINTAPI ptMinPosition;
            public POINTAPI ptMaxPosition;
            public RECT rcNormalPosition;
        }

        /// <summary>
        /// Get window handle by window name or exe name
        /// </summary>
        /// <param name="name">Window name to find (case insensitive)</param>
        /// <param name="window">Will contain the window handle, if found</param>
        /// <param name="use_exe_name_instead">Use exe name instead of window name</param>
        /// <param name="partial_match">If true, will match windows or processes that *contains* the provided name</param>
        /// <returns>Returns TRUE if the handle was found</returns>
        public static bool GetProcessMainWindow(string name, ref IntPtr window, bool use_exe_name_instead = false, bool partial_match = false)
        {
            foreach (Process p in Process.GetProcesses())
            {
                try
                {
                    //Get window title OR exe name
                    IntPtr temp_window = p.MainWindowHandle;
                    string to_compare = use_exe_name_instead ? GetWindowExeName(temp_window) : GetWindowTitle(temp_window);
                    if (to_compare == null) { continue; }

                    //Convert strings to lower
                    to_compare = to_compare.ToLower();
                    name = name.ToLower();

                    //Compare and return if found
                    if (partial_match ? to_compare.Contains(name) : to_compare == name)
                    {
                        window = temp_window;
                        return true;
                    }
                }
                catch { }
            }
            return false; //Process not found
        }

        /// <summary>
        /// Close the specified window
        /// </summary>
        /// <param name="window">Window handle to close</param>
        public static void CloseWindow(IntPtr window)
        {
            const UInt32 WM_CLOSE = 0x0010;
            SendMessage(window, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }

        /// <summary>
        /// Get the title of a window
        /// </summary>
        /// <param name="window">Window handle to get title from</param>
        /// <returns>Returns the title or NULL if an error occured</returns>
        public static string GetWindowTitle(IntPtr window)
        {
            const int nChars = 1024;
            StringBuilder Buff = new StringBuilder(nChars);
            if (GetWindowText(window, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }

        /// <summary>
        /// Get the exe filename of the process attached to a window
        /// </summary>
        /// <param name="window">Window handle to get exe name from</param>
        /// <returns>Return the requested exe name or NULL if an error occured</returns>
        public static string GetWindowExeName(IntPtr window)
        {
            try
            {
                uint pid;
                GetWindowThreadProcessId(window, out pid);
                System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById((int)pid);
                try
                {
                    return System.IO.Path.GetFileName(p.MainModule.FileName);
                }
                catch
                {
                    return p.ProcessName + ".exe";
                }
            }
            catch
            {
                const int nChars = 1024;
                StringBuilder Buff = new StringBuilder(nChars);
                if (GetWindowModuleFileName(window, Buff, nChars) > 0)
                {
                    try
                    {
                        return System.IO.Path.GetFileName(Buff.ToString());
                    }
                    catch { return Buff.ToString(); }
                }
                return null;
            }
        }

        /// <summary>
        /// Move a window to the specified coordinates
        /// </summary>
        /// <param name="window">Window handle to move</param>
        /// <param name="new_x">New X coordinate</param>
        /// <param name="new_y">New Y coordinate</param>
        public static void moveWindow(IntPtr window, int new_x, int new_y)
        {
            const UInt32 SWP_NOSIZE = 1;
            const UInt32 SWP_NOZORDER = 4;
            const UInt32 SWP_SHOWWINDOW = 0x0040;
            SetWindowPos(window, IntPtr.Zero, new_x, new_y, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);
        }

        /// <summary>
        /// Move and resize a window
        /// </summary>
        /// <param name="window">Window handle to move and/or resize</param>
        /// <param name="new_x">New X coordinate</param>
        /// <param name="new_y">New Y coordinate</param>
        /// <param name="new_width">New width</param>
        /// <param name="new_height">New height</param>
        public static void setWindowBound(IntPtr window, int new_x, int new_y, int new_width, int new_height)
        {
            MoveWindow(window, new_x, new_y, new_width, new_height, true);
        }

        /// <summary>
        /// Get dimensions and position of the specified window
        /// </summary>
        /// <param name="window">Window handle to get bounds from</param>
        /// <returns>Window Bounds</returns>
        public static Rectangle getWindowBounds(IntPtr window)
        {
            RECT winRect = new RECT();
            GetWindowRect(window, out winRect);
            return new Rectangle(winRect.left, winRect.top, winRect.right - winRect.left + 1, winRect.bottom - winRect.top + 1);
        }

        /// <summary>
        /// Set the "always on top" attribute of a window
        /// </summary>
        /// <param name="window">Window handle to affect</param>
        /// <param name="ontop">True for Always on Top, false for normal window</param>
        public static void setWindowOnTop(IntPtr window, bool ontop)
        {
            IntPtr HWND_TOPMOST = new IntPtr(-1);
            IntPtr HWND_NOTOPMOST = new IntPtr(-2);
            const UInt32 SWP_NOSIZE = 0x0001;
            const UInt32 SWP_NOMOVE = 0x0002;
            const UInt32 SWP_SHOWWINDOW = 0x0040;
            SetWindowPos(window, (ontop ? HWND_TOPMOST : HWND_NOTOPMOST), 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
        }

        /// <summary>
        /// Change the state of a window (minimized, maximized, normal)
        /// </summary>
        /// <param name="window">Window handle to affect</param>
        /// <param name="newstate">New state for the window</param>
        public static void setWindowState(IntPtr window, System.Windows.Forms.FormWindowState newstate)
        {
            const int SW_SHOWNORMAL = 1;
            const int SW_SHOWMINIMIZED = 2;
            const int SW_SHOWMAXIMIZED = 3;

            int state = 1;

            switch (newstate)
            {
                case System.Windows.Forms.FormWindowState.Normal: state = SW_SHOWNORMAL; break;
                case System.Windows.Forms.FormWindowState.Maximized: state = SW_SHOWMAXIMIZED; break;
                case System.Windows.Forms.FormWindowState.Minimized: state = SW_SHOWMINIMIZED; break;
            }

            ShowWindowAsync(window, state);
        }

        /// <summary>
        /// Get executable name of the foreground window
        /// </summary>
        /// <returns>Exe name or NULL if could not find window</returns>
        public static string GetActiveWindowExeName()
        {
            try
            {
                IntPtr hwnd = GetForegroundWindow();
                uint pid;
                GetWindowThreadProcessId(hwnd, out pid);
                System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById((int)pid);
                try
                {
                    return System.IO.Path.GetFileName(p.MainModule.FileName);
                }
                catch
                {
                    return p.ProcessName + ".exe";
                }
            }
            catch
            {
                const int nChars = 1024;
                IntPtr handle = IntPtr.Zero;
                StringBuilder Buff = new StringBuilder(nChars);
                handle = GetForegroundWindow();

                if (GetWindowModuleFileName(handle, Buff, nChars) > 0)
                {
                    try
                    {
                        return System.IO.Path.GetFileName(Buff.ToString());
                    }
                    catch { return Buff.ToString(); }
                }
                return null;
            }
        }

        /// <summary>
        /// Check for a fullscreen app window (media player, game...)
        /// </summary>
        public static bool isFullScreenAppActive
        {
            get
            {
                IntPtr foreWindow = GetForegroundWindow();
                WINDOWPLACEMENT forePlacement = new WINDOWPLACEMENT();
                forePlacement.length = Marshal.SizeOf(forePlacement);
                GetWindowPlacement(foreWindow, ref forePlacement);

                RECT bounds = forePlacement.rcNormalPosition;
                int width = bounds.right - bounds.left;
                int height = bounds.bottom - bounds.top;
                System.Drawing.Rectangle resolution =
                    System.Windows.Forms.Screen.PrimaryScreen.Bounds;

                return (width == resolution.Width && height == resolution.Height);
            }
        }

        /// <summary>
        /// Kill a process by Exe name
        /// </summary>
        public static void killExe(string exeName)
        {
            ProcessStartInfo P = new ProcessStartInfo("taskkill", "/f /im " + exeName);
            P.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(P).WaitForExit();
        }

        /// <summary>
        /// Check for another instance of this program
        /// </summary>
        public static bool IsSingleInstance
        {
            get
            {
                Process currentProcess = Process.GetCurrentProcess();

                var runningProcess = (from process in Process.GetProcesses()
                                      where
                                        process.Id != currentProcess.Id &&
                                        process.ProcessName.Equals(
                                          currentProcess.ProcessName,
                                          StringComparison.Ordinal)
                                      select process).FirstOrDefault();

                if (runningProcess != null)
                {
                    return false;
                }

                return true;
            }
        }
    }
}
