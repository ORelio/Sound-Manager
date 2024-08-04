using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace SharpTools
{
    /// <summary>
    /// Read or replace a resource inside an PE file - EXE or DLL
    /// By ORelio (c) 2024 - CDDL 1.0
    /// </summary>
    /// <remarks>
    /// For full-fledged resource library, see https://github.com/resourcelib/resourcelib
    /// WinAPI bindings signatures from https://github.com/resourcelib/resourcelib/blob/master/Source/ResourceLib/Kernel32.cs
    /// </remarks>
    public class NativeResource
    {
        /// <summary>
        /// Extract a resource from a PE file (EXE or DLL) to file
        /// </summary>
        /// <param name="dllFile">Path to DLL file</param>
        /// <param name="resourceType">Type of resource, e.g. "WAVE"</param>
        /// <param name="resourceId">ID of resource, e.g. 5080</param>
        /// <param name="resLocale">Language ID of resource, e.g. 0 or 1033</param>
        /// <returns>TRUE if successfully extracted the resource</returns>
        public static bool Extract(string dllFile, string resourceType, uint resourceId, ushort resLocale, string outputFile)
        {
            byte[] resData = Extract(dllFile, resourceType, resourceId, resLocale);
            if (resData != null && resData.Length > 0)
            {
                File.WriteAllBytes(outputFile, resData);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Extract a resource from a PE file (EXE or DLL) to memory
        /// </summary>
        /// <param name="dllFile">Path to DLL file</param>
        /// <param name="resourceType">Type of resource, e.g. "WAVE"</param>
        /// <param name="resourceId">ID of resource, e.g. 5080</param>
        /// <param name="resLocale">Language ID of resource, e.g. 0 or 1033</param>
        /// <returns>Resource data or NULL if failed to extract</returns>
        public static byte[] Extract(string dllFile, string resourceType, uint resourceId, ushort resLocale)
        {
            byte[] result = null;
            IntPtr hModule = LoadLibraryEx(dllFile, IntPtr.Zero, DONT_RESOLVE_DLL_REFERENCES | LOAD_LIBRARY_AS_DATAFILE);
            if (hModule != IntPtr.Zero)
            {
                IntPtr strType = Marshal.StringToHGlobalUni(resourceType);
                IntPtr hResInfo = FindResourceEx(hModule, strType, resourceId, resLocale);
                Marshal.FreeHGlobal(strType);
                if (hResInfo != IntPtr.Zero)
                {
                    IntPtr hData = LoadResource(hModule, hResInfo);
                    if (hData != IntPtr.Zero)
                    {
                        int resSize = SizeofResource(hModule, hResInfo);
                        if (resSize > 0)
                        {
                            byte[] resData = new byte[resSize];
                            Marshal.Copy(hData, resData, 0, resSize);
                            result = resData;
                        }
                    }
                }
                FreeLibrary(hModule);
            }
            return result;
        }

        /// <summary>
        /// Replace a resource in a PE file (EXE or DLL) from specified file
        /// </summary>
        /// <param name="dllFile">Path to DLL file</param>
        /// <param name="resourceType">Type of resource, e.g. "WAVE"</param>
        /// <param name="resourceId">ID of resource, e.g. 5080</param>
        /// <param name="resLocale">Language ID of resource, e.g. 0 or 1033</param>
        /// <param name="resFile">Path to resource file to insert in DLL</param>
        /// <returns>TRUE if successfully replaced the resource</returns>
        public static bool Replace(string dllFile, string resourceType, uint resourceId, ushort resLocale, string resFile)
        {
            return Replace(dllFile, resourceType, resourceId, resLocale, File.ReadAllBytes(resFile));
        }

        /// <summary>
        /// Replace a resource in a PE file (EXE or DLL) using specified data
        /// </summary>
        /// <param name="dllFile">Path to DLL file</param>
        /// <param name="resourceType">Type of resource, e.g. "WAVE"</param>
        /// <param name="resourceId">ID of resource, e.g. 5080</param>
        /// <param name="resLocale">Language ID of resource, e.g. 0 or 1033</param>
        /// <param name="resFile">Resource data to insert in DLL</param>
        /// <returns>TRUE if successfully replaced the resource</returns>
        public static bool Replace(string dllFile, string resourceType, uint resourceId, ushort resLocale, byte[] resData)
        {
            IntPtr hUpdate = BeginUpdateResource(dllFile, false);
            if (hUpdate != IntPtr.Zero)
            {
                IntPtr strType = Marshal.StringToHGlobalUni(resourceType);
                bool updated = UpdateResource(hUpdate, strType, resourceId, resLocale, resData, (uint)resData.Length);
                Marshal.FreeHGlobal(strType);
                if (updated)
                {
                    return EndUpdateResource(hUpdate, false);
                }
            }
            return false;
        }

        /* WINAPI - READ RESOURCE */

        private const uint LOAD_LIBRARY_AS_DATAFILE = 0x00000002;
        private const uint DONT_RESOLVE_DLL_REFERENCES = 0x00000001;

        /// <summary>
        /// Load a DLL library
        /// </summary>
        /// <param name="lpFileName">Path to the DLL library</param>
        /// <param name="hFile">Reserved. Must be NULL</param>
        /// <param name="dwFlags">Load flags</param>
        /// <returns>Handle to the DLL or IntPtr.Zero in case of failure</returns>
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "LoadLibraryExW")]
        private static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

        /// <summary>
        /// Find a resource inside a library
        /// </summary>
        /// <param name="hModule">Handle to the DLL</param>
        /// <param name="lpszType">Resource type (string pointer, e.g. "WAVE")</param>
        /// <param name="lpszName">Resource ID</param>
        /// <param name="wLanguage">Resource language ID</param>
        /// <returns>Handle to the resource or IntPtr.Zero if not found</returns>
        [DllImport("kernel32.dll", EntryPoint = "FindResourceExW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr FindResourceEx(IntPtr hModule, IntPtr lpszType, uint lpszName, UInt16 wLanguage);

        /// <summary>
        /// Get pointer to resource data from resource handle
        /// </summary>
        /// <param name="hModule">Handle to the DLL</param>
        /// <param name="hResInfo">Handle to the resource</param>
        /// <returns>Pointer to the resource data or IntPtr.Zero in case of failure</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

        /// <summary>
        /// Get size of resource data from resource handle
        /// </summary>
        /// <param name="hModule">Handle to the DLL</param>
        /// <param name="hResInfo">Handle to the resource</param>
        /// <returns>Size of the resource data or zero in case of failure</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int SizeofResource(IntPtr hModule, IntPtr hResInfo);

        /// <summary>
        /// Unload a DLL library
        /// </summary>
        /// <param name="hModule">Handle to the DLL</param>
        /// <returns>True if successfully unloaded</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);

        /* WINAPI - WRITE RESOURCE */

        /// <summary>
        /// Load a DLL for updating
        /// </summary>
        /// <param name="pFileName">Path to the DLL</param>
        /// <param name="bDeleteExistingResources">TRUE to delete existing resources</param>
        /// <returns>Update handle for the DLL or IntPtr.Zero in case of failure</returns>
        [DllImport("kernel32.dll", EntryPoint = "BeginUpdateResourceW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr BeginUpdateResource(string pFileName, bool bDeleteExistingResources);

        /// <summary>
        /// Replace a resource in a DLL
        /// </summary>
        /// <param name="hUpdate">Update handle for the DLL</param>
        /// <param name="lpType">Resource type (string pointer, e.g. "WAVE")</param>
        /// <param name="lpName">Resource ID</param>
        /// <param name="wLanguage">Resource language ID</param>
        /// <param name="lpData">Resource data</param>
        /// <param name="cbData">Size of Resource data</param>
        /// <returns>TRUE if successfully updated (warning: Call EndUpdateResource after all changes to actually write changes to disk)</returns>
        [DllImport("kernel32.dll", EntryPoint = "UpdateResourceW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool UpdateResource(IntPtr hUpdate, IntPtr lpType, uint lpName, UInt16 wLanguage, byte[] lpData, UInt32 cbData);

        /// <summary>
        /// Write a DLL after updating its resources in memory
        /// </summary>
        /// <param name="hUpdate">Update handle for the DLL</param>
        /// <param name="fDiscard">TRUE to discard the changes (cancel updating), FALSE to actually write changes to disk</param>
        /// <returns>TRUE if successfully updated</returns>
        [DllImport("kernel32.dll", EntryPoint = "EndUpdateResourceW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool EndUpdateResource(IntPtr hUpdate, bool fDiscard);
    }
}
