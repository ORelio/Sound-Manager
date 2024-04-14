using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Security;

namespace SharpTools
{
    /// <summary>
    /// This class reimplements System.Drawing.Icon.ExtractAssociatedIcon with added support for Network Shares
    /// Based on https://stackoverflow.com/questions/1842226/how-to-get-the-associated-icon-from-a-network-share-file
    /// </summary>
    public static class IconExtractor
    {
        /// <summary>
        /// Returns an icon representation of an image contained in the specified file.
        /// This function is identical to System.Drawing.Icon.ExtractAssociatedIcon, except this version allows UNC paths.
        /// </summary>
        /// <param name="filePath">The path to the file that contains an image.</param>
        /// <returns>The System.Drawing.Icon representation of the image contained in the specified file.</returns>
        /// <exception cref="System.ArgumentException">filePath does not indicate a valid file.</exception>
        public static Icon ExtractAssociatedIcon(String filePath)
        {
            try
            {
                // By default, use the built-in .NET method as usual
                return Icon.ExtractAssociatedIcon(filePath);
            }
            catch (ArgumentException)
            {
                // If ExtractAssociatedIcon() refuses to process the argument, try again using the Win32 API directly
                int index = 0;

                Uri uri;
                if (String.IsNullOrEmpty(filePath))
                {
                    throw new ArgumentException(String.Format("'{0}' is not valid for '{1}'", "null", "filePath"), "filePath");
                }
                try
                {
                    uri = new Uri(filePath);
                }
                catch (UriFormatException)
                {
                    filePath = Path.GetFullPath(filePath);
                    uri = new Uri(filePath);
                }
                //if (uri.IsUnc)
                //{
                //  throw new ArgumentException(String.Format("'{0}' is not valid for '{1}'", filePath, "filePath"), "filePath");
                //}
                if (uri.IsFile)
                {
                    if (!File.Exists(filePath))
                    {
                        //IntSecurity.DemandReadFileIO(filePath);
                        throw new FileNotFoundException(filePath);
                    }

                    StringBuilder iconPath = new StringBuilder(260);
                    iconPath.Append(filePath);

                    IntPtr handle = SafeNativeMethods.ExtractAssociatedIcon(new HandleRef(null, IntPtr.Zero), iconPath, ref index);
                    if (handle != IntPtr.Zero)
                    {
                        //IntSecurity.ObjectFromWin32Handle.Demand();
                        return Icon.FromHandle(handle);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// This class suppresses stack walks for unmanaged code permission. 
        /// (System.Security.SuppressUnmanagedCodeSecurityAttribute is applied to this class.) 
        /// This class is for methods that are safe for anyone to call. 
        /// Callers of these methods are not required to perform a full security review to make sure that the 
        /// usage is secure because the methods are harmless for any caller.
        /// </summary>
        [SuppressUnmanagedCodeSecurity]
        internal static class SafeNativeMethods
        {
            [DllImport("shell32.dll", EntryPoint = "ExtractAssociatedIcon", CharSet = CharSet.Auto)]
            internal static extern IntPtr ExtractAssociatedIcon(HandleRef hInst, StringBuilder iconPath, ref int index);
        }
    }
}
