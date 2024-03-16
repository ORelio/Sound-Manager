using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ionic.Zip;
using System.IO;
using SharpTools;
using System.Windows.Forms;

namespace SoundManager
{
    /// <summary>
    /// Sound archive file import/export routines
    /// </summary>
    static class SoundArchive
    {
        private static readonly string FileIconPath = Path.Combine(RuntimeConfig.AppFolder, "SoundScheme.ico");
        private static readonly string FileExtCommand = String.Concat("\"", Application.ExecutablePath, "\" \"%1\"");
        private const string FileExtAction = "import";
        public const string FileExtension = "ths";

        /// <summary>
        /// Try extracting a file from the provided ZipFile
        /// </summary>
        /// <param name="zip">ZipFile to extract from</param>
        /// <param name="fileName">File name to extract</param>
        /// <param name="outputDir">Output directory. Existing files are overwritten silently.</param>
        /// <param name="outputFileName">Output file name, if different from file name in archive</param>
        /// <returns>TRUE if the file was found and extracted</returns>
        private static bool TryExtract(ZipFile zip, string fileName, string outputDir, string outputFileName = null)
        {
            if (zip.ContainsEntry(fileName))
            {
                zip.Entries
                    .First(entry => entry.FileName == fileName)
                    .Extract(outputDir, ExtractExistingFileAction.OverwriteSilently);
                File.SetAttributes(Path.Combine(outputDir, fileName), FileAttributes.Normal);
                if (outputFileName != null)
                {
                    string currentFilePath = Path.Combine(outputDir, fileName);
                    string outputFilePath = Path.Combine(outputDir, outputFileName);
                    File.Delete(outputFilePath);
                    File.Move(currentFilePath, outputFilePath);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Import the specified Zip file into the SoundManager sound scheme
        /// </summary>
        /// <param name="zipfile">Input Zip file</param>
        public static void Import(string zipfile)
        {
            string tempFileToDelete = null;

            // Convert proprietary sound archive to our file format?
            // When "Convert Archive" setting is enabled, replace proprietary file with converted file, moving the original to recycle bin
            // When "Convert Archive" setting is disabled, convert to a temporary file
            if (SoundArchiveProprietary.IsProprietary(zipfile))
            {
                string outfile = zipfile.Substring(0, zipfile.Length - SoundArchiveProprietary.FileExtension.Length) + SoundArchive.FileExtension;
                if (!Settings.ConvertProprietaryFiles)
                    outfile = tempFileToDelete = Path.GetTempFileName();
                SoundArchiveProprietary.ConvertArchive(zipfile, outfile);
                if (Settings.ConvertProprietaryFiles)
                    FileRecycler.MoveToRecycleBin(zipfile);
                zipfile = outfile;
            }

            // Import sound archive
            using (ZipFile zip = ZipFile.Read(zipfile))
            {
                foreach (SoundEvent soundEvent in SoundEvent.GetAll())
                {
                    if (TryExtract(zip, soundEvent.FileName, SoundEvent.DataDirectory)
                        || TryExtract(zip, soundEvent.LegacyFileName, SoundEvent.DataDirectory, soundEvent.FileName))
                    {
                        SoundScheme.Update(soundEvent, soundEvent.FilePath);
                    }
                    else SoundScheme.Remove(soundEvent);
                }
                SchemeMeta.ResetAll();
                if (!TryExtract(zip, Path.GetFileName(SchemeMeta.SchemeImageFilePath), SoundEvent.DataDirectory))
                    TryExtract(zip, "visuel.bmp", SoundEvent.DataDirectory, Path.GetFileName(SchemeMeta.SchemeImageFilePath));
                if (!TryExtract(zip, Path.GetFileName(SchemeMeta.SchemeInfoFilePath), SoundEvent.DataDirectory))
                    TryExtract(zip, "infos.ini", SoundEvent.DataDirectory, Path.GetFileName(SchemeMeta.SchemeInfoFilePath));
                SchemeMeta.ReloadFromDisk();
                SoundScheme.Apply(SoundScheme.GetSchemeSoundManager(), Settings.MissingSoundUseDefault);
            }

            // Delete temporary converted file
            if (tempFileToDelete != null && File.Exists(tempFileToDelete))
                File.Delete(tempFileToDelete);
        }

        /// <summary>
        /// Export the SoundManager sound scheme to the specified Zip file
        /// </summary>
        /// <param name="zipfile">Output Zip file</param>
        public static void Export(string zipfile)
        {
            using (ZipFile zip = new ZipFile())
            {
                foreach (SoundEvent soundEvent in SoundEvent.GetAll())
                {
                    if (File.Exists(soundEvent.FilePath))
                        zip.AddFile(soundEvent.FilePath, "");
                }
                if (File.Exists(SchemeMeta.SchemeImageFilePath))
                    zip.AddFile(SchemeMeta.SchemeImageFilePath, "");
                if (File.Exists(SchemeMeta.SchemeInfoFilePath))
                    zip.AddFile(SchemeMeta.SchemeInfoFilePath, "");
                zip.Save(zipfile);
            }
        }

        /// <summary>
        /// Associate SoundArchive file extensions to the program
        /// </summary>
        public static void AssocFiles()
        {
            AssocFileExtension(FileExtension, FileIconPath, Translations.Get("scheme_file_desc"));
            AssocFileExtension(SoundArchiveProprietary.FileExtension, SoundArchiveProprietary.FileIconPath, Translations.Get("scheme_file_proprietary_desc"));
        }

        /// <summary>
        /// Associate a SoundArchive file extension to the program
        /// </summary>
        /// <param name="fileExtension">File extension without leading dot</param>
        /// <param name="fileIconPath">Full path to file icon</param>
        /// <param name="fileDescription">Description shown in file explorer for this file type</param>
        private static void AssocFileExtension(string fileExtension, string fileIconPath, string fileDescription)
        {
            ShellFileType fileType = ShellFileType.GetOrCreateType(fileExtension);
            fileType.DefaultIcon = fileIconPath;
            fileType.Description = fileDescription;
            fileType.MenuItems[FileExtAction] = new ShellFileType.MenuItem(Translations.Get("button_open"), FileExtCommand);
            fileType.DefaultAction = FileExtAction;
            fileType.Save();
        }

        /// <summary>
        /// Remove association to SoundArchive file extensions
        /// </summary>
        public static void UnAssocFiles()
        {
            UnAssocFileExtension(FileExtension);
            UnAssocFileExtension(SoundArchiveProprietary.FileExtension);
        }

        /// <summary>
        /// Remove association for a SoundArchive file extension
        /// </summary>
        /// <param name="fileExtension">File extension without leading dot</param>
        private static void UnAssocFileExtension(string fileExtension)
        {
            try
            {
                ShellFileType fileType = ShellFileType.GetType(fileExtension);
                fileType.DefaultIcon = null;
                fileType.DefaultAction = null;
                fileType.Description = null;
                fileType.MenuItems.Clear();
                fileType.Save();
            }
            catch (KeyNotFoundException)
            {
                // Missing file type, nothing to remove
            }
        }

        /// <summary>
        /// Check file association to SoundArchive files
        /// </summary>
        public static bool FileAssociation
        {
            get
            {
                return IsFileExtensionAssociated(FileExtension)
                    && IsFileExtensionAssociated(SoundArchiveProprietary.FileExtension);
            }
        }

        /// <summary>
        /// Check file association to a SoundArchive file type
        /// </summary>
        /// <param name="fileExtension">File extension without leading dot</param>
        /// <returns>TRUE if file type is associated with SoundManager</returns>
        private static bool IsFileExtensionAssociated(string fileExtension)
        {
            try
            {
                ShellFileType fileType = ShellFileType.GetType(fileExtension);
                return (FileExtAction == fileType.DefaultAction
                    && fileType.MenuItems.ContainsKey(FileExtAction)
                    && fileType.MenuItems[FileExtAction].Command == FileExtCommand);
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }
    }
}
