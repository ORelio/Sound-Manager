using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Security.AccessControl;

namespace SharpTools
{
    /// <summary>
    /// Check Account Logon properties
    /// By ORelio - (c) 2025 - Available under the CDDL-1.0 license
    /// </summary>
    public static class AccountProperties
    {
        private const int LOGON32_PROVIDER_DEFAULT = 0;
        private const int LOGON32_LOGON_INTERACTIVE = 2;
        private const int WIN32_ERROR_EMPTY_PASSWORD = 1327;

        // http://www.pinvoke.net/default.aspx/advapi32/LogonUser.html
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool LogonUser(string username, string domain, string password, int logonType, int logonProvider, out IntPtr token);

        // http://www.pinvoke.net/default.aspx/kernel32/CloseHandle.html
        [DllImport("kernel32", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr handle);

        /// <summary>
        /// Check if the specified account has a password set
        /// </summary>
        /// <remarks>
        /// Works by trying to log in as the current user without a password
        /// </remarks>
        /// <seealso>https://stackoverflow.com/questions/6556594/how-to-check-if-windows-user-has-a-password-set</seealso>
        public static bool HasPassword(string username)
        {
            IntPtr logonToken;
            bool logonSuccess = LogonUser(username, null, "", LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, out logonToken);
            int error = Marshal.GetLastWin32Error();
            if (logonToken != IntPtr.Zero)
                CloseHandle(logonToken);
            return !(logonSuccess || error == WIN32_ERROR_EMPTY_PASSWORD);
        }

        private static readonly RegistryKey RegistryHKLM64bits = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
        private static readonly RegistryKey Winlogon = RegistryHKLM64bits.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon", RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.QueryValues);
        private const string Winlogon_AutologonEnable = "AutoAdminLogon";
        private const string Winlogon_AutologonAccount = "DefaultUserName";

        /// <summary>
        /// Check if the current account has autologon enabled
        /// </summary>
        /// <remarks>
        /// Works by inspecting Winlogon settings in the registry
        /// </remarks>
        public static bool HasAutoLogon(string username)
        {
            string enabled = Winlogon.GetValue(Winlogon_AutologonEnable, "0") as string;
            if (enabled != "1")
                return false;
            string account = Winlogon.GetValue(Winlogon_AutologonAccount, "") as string;
            return account.ToLowerInvariant() == username.ToLowerInvariant();
        }
    }
}
