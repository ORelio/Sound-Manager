using Microsoft.Win32;

namespace SharpTools
{
    /// <summary>
    /// Retrieve information about the current Windows version
    /// </summary>
    /// <remarks>
    /// Environment.OSVersion does not work with Windows 10.
    /// It returns 6.2 which is Windows 8
    /// </remarks>
    /// <seealso>
    /// https://stackoverflow.com/a/37755503
    /// </seealso>
    class WindowsVersion
    {
        private const string CurrentVersionRegKey = "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion";

        /// <summary>
        /// Try retrieving a registry key
        /// </summary>
        /// <param name="path">key path</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value (output)</param>
        /// <returns>TRUE if successfully retrieved</returns>
        private static bool TryGetRegistryKey(string path, string key, out dynamic value)
        {
            value = null;
            try
            {
                var rk = Registry.LocalMachine.OpenSubKey(path);
                if (rk == null) return false;
                value = rk.GetValue(key);
                return value != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the Windows major version number for this computer.
        /// </summary>
        public static uint WinMajorVersion
        {
            get
            {
                dynamic major;
                // The 'CurrentMajorVersionNumber' string value in the CurrentVersion key is new for Windows 10, 
                // and will most likely (hopefully) be there for some time before MS decides to change this - again...
                if (TryGetRegistryKey(CurrentVersionRegKey, "CurrentMajorVersionNumber", out major))
                {
                    return (uint) major;
                }

                // When the 'CurrentMajorVersionNumber' value is not present we fallback to reading the previous key used for this: 'CurrentVersion'
                dynamic version;
                if (!TryGetRegistryKey(CurrentVersionRegKey, "CurrentVersion", out version))
                    return 0;

                var versionParts = ((string) version).Split('.');
                if (versionParts.Length != 2) return 0;
                uint majorAsUInt;
                return uint.TryParse(versionParts[0], out majorAsUInt) ? majorAsUInt : 0;
            }
        }

        /// <summary>
        /// Returns the Windows minor version number for this computer.
        /// </summary>
        public static uint WinMinorVersion
        {
            get
            {
                dynamic minor;
                // The 'CurrentMinorVersionNumber' string value in the CurrentVersion key is new for Windows 10, 
                // and will most likely (hopefully) be there for some time before MS decides to change this - again...
                if (TryGetRegistryKey(CurrentVersionRegKey, "CurrentMinorVersionNumber",
                    out minor))
                {
                    return (uint) minor;
                }

                // When the 'CurrentMinorVersionNumber' value is not present we fallback to reading the previous key used for this: 'CurrentVersion'
                dynamic version;
                if (!TryGetRegistryKey(CurrentVersionRegKey, "CurrentVersion", out version))
                    return 0;

                var versionParts = ((string) version).Split('.');
                if (versionParts.Length != 2) return 0;
                uint minorAsUInt;
                return uint.TryParse(versionParts[1], out minorAsUInt) ? minorAsUInt : 0;
            }
        }

        /// <summary>
        /// Returns whether or not the current computer is a server or not.
        /// </summary>
        public static uint IsServer
        {
            get
            {
                dynamic installationType;
                if (TryGetRegistryKey(CurrentVersionRegKey, "InstallationType",
                    out installationType))
                {
                    return (uint) (installationType.Equals("Client") ? 0 : 1);
                }

                return 0;
            }
        }

        /// <summary>
        /// Returns whether the current computer is running Mono instead of .NET framework (likely Mac and Linux)
        /// </summary>
        public static bool IsMono
        {
            get
            {
                return System.Type.GetType("Mono.Runtime") != null;
            }
        }

        /// <summary>
        /// Get friendly name of the system version
        /// </summary>
        public static string FriendlyName
        {
            get
            {
                dynamic ProductName;
                dynamic CSDVersion;
                TryGetRegistryKey(CurrentVersionRegKey, "ProductName", out ProductName);
                TryGetRegistryKey(CurrentVersionRegKey, "CSDVersion", out CSDVersion);
                if (ProductName != null)
                {
                    return (ProductName.StartsWith("Microsoft") ? "" : "Microsoft ") + ProductName.ToString() +
                                (CSDVersion != null ? " " + CSDVersion.ToString() : "");
                }
                return "";
            }
        }

        /// <summary>
        /// Check if the current Windows version is between the specified bounds (inclusive)
        /// </summary>
        /// <param name="versionMin">Minimum version in "M.m" format (Major, minor)</param>
        /// <param name="versionMax">Maximum version in "M.m" format (Major, minor)</param>
        /// <returns>TRUE if the current version is between the specified bounds</returns>
        public static bool IsBetween(string versionMin, string versionMax)
        {
            string[] versionMinParts = versionMin.Split('.');
            string[] versionMaxParts = versionMax.Split('.');

            if (versionMinParts.Length == 2 && versionMinParts.Length == 2)
            {
                uint minMajor, minMinor, maxMajor, maxMinor;
                if (uint.TryParse(versionMinParts[0], out minMajor)
                    && uint.TryParse(versionMinParts[1], out minMinor)
                    && uint.TryParse(versionMaxParts[0], out maxMajor)
                    && uint.TryParse(versionMaxParts[1], out maxMinor))
                {
                    return IsBetween(minMajor, minMinor, maxMajor, maxMinor);
                }
            }

            return false;
        }

        /// <summary>
        /// Check if the current Windows version is between the specified bounds (inclusive)
        /// </summary>
        /// <param name="minMajor">Major version of the minimum version</param>
        /// <param name="minMinor">Minor version of the minimum version</param>
        /// <param name="maxMajor">Major version of the maximum version</param>
        /// <param name="maxMinor">Minor version of the maximum version</param>
        /// <returns>TRUE if the version is between the specified bounds</returns>
        public static bool IsBetween(uint minMajor, uint minMinor, uint maxMajor, uint maxMinor)
        {
            uint winMajor = WinMajorVersion;
            uint winMinor = WinMinorVersion;

            if (winMajor < minMajor)
            {
                return false;
            }
            else if (winMajor == minMajor)
            {
                return winMinor >= minMinor;
            }
            else if (winMajor < maxMajor)
            {
                return true;
            }
            else if (winMajor == maxMajor)
            {
                return winMinor <= maxMinor;
            }
            else return false;
        }
    }
}
