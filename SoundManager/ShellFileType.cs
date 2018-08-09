using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace SharpTools
{
    /// <summary>
    /// Manage shell file type information for the Windows operating system.
    /// By ORelio (c) 2015 - CDDL 1.0
    /// </summary>
    public class ShellFileType : IDisposable
    {
        private const string UserExtensionsPathPrefix = @"Software\\Classes\\";
        private const string UserChoicePathTemplate = @"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.{0}\UserChoice";

        private static RegistryKey ClassesRoot = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Default);
        private static RegistryKey CurrentUser = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default);

        /// <summary>
        /// Utility method for notifying the operating system that file types were updated
        /// </summary>
        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void SHChangeNotify(int wEventId, int uFlags, IntPtr dwItem1, IntPtr dwItem2);

        //Extension eg .txt
        public readonly string Extension;

        //Extension related items
        public string ContentType { get; set; }
        public string PerceivedType { get; set; }
        private string _progId;
        private bool _progIdChanged = false;
        public string ProgId
        {
            get
            {
                return _progId;
            }
            set
            {
                _progIdChanged = true;
                _progId = value;
            }
        }

        //ProgId related items, can be shared between extensions
        public string Description { get; set; }
        public string DefaultIcon { get; set; }
        public string DefaultAction { get; set; }
        public readonly Dictionary<string, MenuItem> MenuItems;

        //Init ShellFileType item with readonly list
        public ShellFileType(string extension, string progId = null)
        {
            ProgId = progId;
            Extension = extension;
            MenuItems = new Dictionary<string, MenuItem>();
        }

        /// <summary>
        /// Represents a file type shell menu action
        /// </summary>
        public class MenuItem
        {
            //Elements of the menu item
            public string DisplayName { get; set; }
            public string Command { get; set; }

            //Init FileTypeMenuItem object
            public MenuItem(string displayName, string command)
            {
                DisplayName = displayName;
                Command = command;
            }
        }

        /// <summary>
        /// Get a file type item representing a given file extension from the windows registry
        /// </summary>
        /// <param name="extension">extension, without the preceding dot, eg "txt"</param>
        /// <exception cref="KeyNotFoundException">Thrown if the extension does not exist</exception>
        /// <returns>File type representing the extension</returns>
        public static ShellFileType GetType(string extension)
        {
            //Basic checks and registry key reading
            if (extension == null)
                throw new ArgumentNullException("The given extension must not be null");
            if (extension.Length > 0 && !extension.All(c => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9')))
                throw new ArgumentException("The given extension must be non-empty and composed of letters or digits only");
            extension = extension.ToLower();
            RegistryKey result = CurrentUser.OpenSubKey(UserExtensionsPathPrefix + '.' + extension);
            if (result == null)
                result = ClassesRoot.OpenSubKey("." + extension);
            if (result == null)
                throw new KeyNotFoundException("The given extension does not exists in registry");

            //Read basic information about the file extension
            ShellFileType fileType = new ShellFileType(extension);
            fileType.ProgId = result.GetValue(null) as string;
            fileType.ContentType = result.GetValue("Content Type") as string;
            fileType.PerceivedType = result.GetValue("PerceivedType") as string;

            //Retrieve ProgId from alternative locations
            RegistryKey altProgIds = result.OpenSubKey("OpenWithProgids");
            if (fileType.ProgId == null && altProgIds != null)
                if ((fileType.ProgId = altProgIds.GetValue(null) as string) == null)
                    if (altProgIds.GetValueNames().Length > 0)
                        fileType.ProgId = altProgIds.GetValueNames()[0];

            //Update ProgId if the current user has chosen a specifig program for this file extension
            RegistryKey userChoice = CurrentUser.OpenSubKey(String.Format(UserChoicePathTemplate, extension));
            if (userChoice != null)
                fileType.ProgId = userChoice.GetValue("Progid", fileType.ProgId) as string;
            fileType._progIdChanged = false;

            //Read program-related information about file extension
            if (fileType.ProgId == null)
                return fileType;
            RegistryKey progInfo = CurrentUser.OpenSubKey(UserExtensionsPathPrefix + fileType.ProgId);
            if (progInfo == null)
                progInfo = ClassesRoot.OpenSubKey(fileType.ProgId);
            if (progInfo == null)
                return fileType;
            fileType.Description = progInfo.GetValue(null) as string;

            //Read file type icon
            RegistryKey iconInfo = progInfo.OpenSubKey("DefaultIcon");
            if (iconInfo != null)
                fileType.DefaultIcon = iconInfo.GetValue(null) as string;

            //Read default shell menu action
            RegistryKey shellInfo = progInfo.OpenSubKey("shell");
            if (shellInfo == null)
                return fileType;
            fileType.DefaultAction = shellInfo.GetValue(null) as string;

            //Read shell menu actions
            foreach (string actionName in shellInfo.GetSubKeyNames())
            {
                RegistryKey shellAction = shellInfo.OpenSubKey(actionName);
                if (shellAction != null)
                {
                    string actionDisplayName = shellAction.GetValue(null) as string;
                    RegistryKey shellCommand = shellAction.OpenSubKey("command");
                    if (shellCommand != null)
                    {
                        string actionCommand = shellCommand.GetValue(null) as string;
                        fileType.MenuItems[actionName] = new ShellFileType.MenuItem(actionDisplayName, actionCommand);
                    }
                }
            }

            //All data have been read for registry
            return fileType;
        }

        /// <summary>
        /// Get a file type item representing a given file extension a create a default one
        /// </summary>
        /// <param name="extension">extension, without the preceding dot, eg "txt"</param>
        /// <returns>File type representing the extension</returns>
        public static ShellFileType GetOrCreateType(string extension)
        {
            try
            {
                return GetType(extension);
            }
            catch (KeyNotFoundException)
            {
                return new ShellFileType(extension, extension + "_auto_file");
            }
        }

        /// <summary>
        /// Save the given type to the registry
        /// </summary>
        /// <param name="fileType">File type to save in registry</param>
        /// <exception cref="ArgumentException">Thrown if the provided file type is invalid</exception>
        public static void SaveType(ShellFileType fileType)
        {
            //Basic checks on provided file type
            if (fileType == null || String.IsNullOrEmpty(fileType.Extension)
                || !fileType.Extension.All(c => (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9')))
                throw new ArgumentException("The provided file type object is invalid.");

            //Delete user choice if ProgId was changed
            if (fileType._progIdChanged)
                CurrentUser.DeleteSubKey(String.Format(UserChoicePathTemplate, fileType.Extension), false);

            //Update extension-related info
            RegistryKey result = CurrentUser.CreateSubKey(UserExtensionsPathPrefix + "." + fileType.Extension);
            if (fileType._progIdChanged)
                result.DeleteValue(null, false);
            result.DeleteValue("Content Type", false);
            result.DeleteValue("PerceivedType", false);
            if (!String.IsNullOrWhiteSpace(fileType.ProgId)
                && (fileType._progIdChanged
                    || String.IsNullOrWhiteSpace(result.GetValue(null) as string)))
                result.SetValue(null, fileType.ProgId.Trim());
            if (!String.IsNullOrWhiteSpace(fileType.ContentType))
                result.SetValue("Content Type", fileType.ContentType.Trim());
            if (!String.IsNullOrWhiteSpace(fileType.PerceivedType))
                result.SetValue("PerceivedType", fileType.PerceivedType.Trim());

            //Noting more to do if no program id
            if (String.IsNullOrEmpty(fileType.ProgId))
                return;

            //Update program-related info
            RegistryKey progInfo = CurrentUser.CreateSubKey(UserExtensionsPathPrefix + fileType.ProgId);
            progInfo.DeleteValue(null, false);
            if (!String.IsNullOrWhiteSpace(fileType.Description))
                progInfo.SetValue(null, fileType.Description.Trim());

            //Update icon info
            RegistryKey iconInfo = progInfo.CreateSubKey("DefaultIcon");
            iconInfo.DeleteValue(null, false);
            if (!String.IsNullOrWhiteSpace(fileType.DefaultIcon))
                iconInfo.SetValue(null, fileType.DefaultIcon);

            //Update default shell menu action
            RegistryKey shellInfo = progInfo.CreateSubKey("shell");
            shellInfo.DeleteValue(null, false);
            if (!String.IsNullOrWhiteSpace(fileType.DefaultAction))
                shellInfo.SetValue(null, fileType.DefaultAction);

            //Delete removed shell menu actions
            foreach (string actionName in shellInfo.GetSubKeyNames())
                if (!fileType.MenuItems.ContainsKey(actionName))
                    shellInfo.DeleteSubKeyTree(actionName);

            //Update shell menu actions
            foreach (var menuItem in fileType.MenuItems)
            {
                RegistryKey shellAction = shellInfo.CreateSubKey(menuItem.Key);
                shellAction.DeleteValue(null, false);
                if (!String.IsNullOrWhiteSpace(menuItem.Value.DisplayName))
                    shellAction.SetValue(null, menuItem.Value.DisplayName.Trim());
                RegistryKey shellCommand = shellAction.CreateSubKey("command");
                shellCommand.DeleteValue(null, false);
                if (!String.IsNullOrWhiteSpace(menuItem.Value.Command))
                    shellCommand.SetValue(null, menuItem.Value.Command.Trim());
            }

            //Notify the operating system that file type was updated
            SHChangeNotify(0x08000000, 0x0000, (IntPtr)null, (IntPtr)null);
        }

        /// <summary>
        /// Add or update a menu item to the specified file extension
        /// </summary>
        /// <param name="actionName">action name, will overwrite action if already exists</param>
        /// <param name="actionDisplayName">display name of the action visible in shell menu</param>
        /// <param name="actionCommand">command to associate to the given action name</param>
        /// <param name="fileExtension">file type to add or update action</param>
        /// <param name="isDefault">set to true to set the action as the default action</param>
        public static void AddAction(string fileExtension, string actionName, string actionDisplayName, string actionCommand, bool isDefault = false)
        {
            using (ShellFileType fileType = ShellFileType.GetOrCreateType(fileExtension))
            {
                fileType.MenuItems[actionName] = new MenuItem(actionDisplayName, actionCommand);
                if (isDefault) { fileType.DefaultAction = actionName; }
            }
        }

        /// <summary>
        /// Add or update the same menu item to several file extensions at once
        /// </summary>
        /// <param name="actionName">action name, will overwrite action if already exists</param>
        /// <param name="actionDisplayName">display name of the action visible in shell menu</param>
        /// <param name="actionCommand">command to associate to the given action name</param>
        /// <param name="fileExtensions">file types to add or update action</param>
        /// <param name="isDefault">set to true to set the action as the default action</param>
        public static void AddAction(IEnumerable<string> fileExtensions, string actionName,
            string actionDisplayName, string actionCommand, bool isDefault = false)
        {
            foreach (string fileExtension in fileExtensions)
                AddAction(fileExtension, actionName, actionDisplayName, actionCommand, isDefault);
        }

        /// <summary>
        /// Remove the same menu item from the specified file extensions
        /// </summary>
        /// <param name="actionName">internal action name tp remove</param>
        /// <param name="fileExtensions">file type to remove action from</param>
        public static void RemoveAction(string fileExtension, string actionName)
        {
            try
            {
                using (ShellFileType fileType = ShellFileType.GetType(fileExtension))
                {
                    fileType.MenuItems.Remove(actionName);
                }
            }
            catch (KeyNotFoundException) { /* Nothing to remove */ }
        }

        /// <summary>
        /// Remove the same menu item from several file extensions at once
        /// </summary>
        /// <param name="actionName">internal action name tp remove</param>
        /// <param name="fileExtensions">file types to remove action from</param>
        public static void RemoveAction(IEnumerable<string> fileExtensions, string actionName)
        {
            foreach (string fileExtension in fileExtensions)
                RemoveAction(fileExtension, actionName);
        }

        /// <summary>
        /// Check if the provided menu item exists for the provided file extension
        /// </summary>
        /// <param name="fileExtension">File extension</param>
        /// <param name="actionName">Action name</param>
        /// <returns>True if the action exists for the specified file extension</returns>
        public static bool ActionExists(string fileExtension, string actionName)
        {
            try
            {
                return ShellFileType.GetType(fileExtension).MenuItems.ContainsKey(actionName);
            }
            catch (KeyNotFoundException) { return false; }
        }

        /// <summary>
        /// Check if the provided menu item exists for all the provided file extensions
        /// </summary>
        /// <param name="fileExtensions">File extensions</param>
        /// <param name="actionName">Action name</param>
        /// <returns>True if the action exists for all the specified file extensions</returns>
        public static bool ActionExistsAll(IEnumerable<string> fileExtensions, string actionName)
        {
            foreach (string fileExtension in fileExtensions)
                if (!ActionExists(fileExtension, actionName))
                    return false;
            return true;
        }

        /// <summary>
        /// Check if the provided menu item exists for at least one of the provided file extensions
        /// </summary>
        /// <param name="fileExtensions">File extensions</param>
        /// <param name="actionName">Action name</param>
        /// <returns>True if the action exists for at least one of the specified file extensions</returns>
        public static bool ActionExistsAny(IEnumerable<string> fileExtensions, string actionName)
        {
            foreach (string fileExtension in fileExtensions)
                if (ActionExists(fileExtension, actionName))
                    return true;
            return false;
        }

        /// <summary>
        /// Save changes to the registry immediately
        /// </summary>
        public void Save()
        {
            SaveType(this);
        }

        /// <summary>
        /// Save changes to the registry before disposing the object
        /// </summary>
        public void Dispose()
        {
            SaveType(this);
        }
    }
}
