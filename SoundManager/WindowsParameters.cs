using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace SharpTools
{
    /// <summary>
    /// Windows API wrapper for SystemParametersInfo
    /// </summary>
    /// <remarks>
    /// Currently only implements the ScreenReader flag.
    /// https://learn.microsoft.com/en-us/windows/win32/winauto/screen-reader-parameter
    /// https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-systemparametersinfoa
    /// https://github.com/PowerShell/PowerShell/issues/11751#issuecomment-600120959
    /// </remarks>
    class WindowsParameters
    {
        private const int SPI_GETSCREENREADER = 0x0046;

        [DllImport("user32", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int SystemParametersInfo(
            uint uiAction,
            uint uiParam,
            IntPtr pvParam,
            uint fWinIni);

        /// <summary>
        /// Determine if the "Screen Reader" flag is set in Windows API
        /// </summary>
        /// <returns>TRUE if a screen reader is active</returns>
        public static bool IsScreenReaderActive
        {
            get
            {
                var ptr = IntPtr.Zero;
                try
                {
                    ptr = Marshal.AllocHGlobal(sizeof(int));
                    int hr = SystemParametersInfo(
                        SPI_GETSCREENREADER,
                        sizeof(int),
                        ptr,
                        0);

                    if (hr == 0)
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    }

                    return Marshal.ReadInt32(ptr) != 0;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    if (ptr != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(ptr);
                    }
                }
            }
        }
    }
}
