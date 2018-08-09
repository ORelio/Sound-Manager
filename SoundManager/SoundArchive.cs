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
        private static readonly string FileIconPath = String.Concat(Path.GetDirectoryName(Application.ExecutablePath), Path.DirectorySeparatorChar, "SoundScheme.ico");
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
                if (outputFileName != null)
                {
                    string tempFilePath = String.Concat(outputDir, Path.DirectorySeparatorChar, fileName);
                    string outputFilePath = String.Concat(outputDir, Path.DirectorySeparatorChar, outputFileName);
                    File.Delete(outputFilePath);
                    File.Move(tempFilePath, outputFilePath);
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
        /// Associate the SoundArchive file extension to the program
        /// </summary>
        public static void AssocFiles()
        {
            ShellFileType fileType = ShellFileType.GetOrCreateType(FileExtension);
            fileType.DefaultIcon = FileIconPath;
            fileType.Description = Translations.Get("scheme_file_desc");
            fileType.MenuItems[FileExtAction] = new ShellFileType.MenuItem(Translations.Get("button_open"), FileExtCommand);
            fileType.DefaultAction = FileExtAction;
            fileType.Save();
        }

        /// <summary>
        /// Remove association to the SoundArchive file extension
        /// </summary>
        public static void UnAssocFiles()
        {
            try
            {
                ShellFileType fileType = ShellFileType.GetType(FileExtension);
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
                try
                {
                    ShellFileType fileType = ShellFileType.GetType(FileExtension);
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
}
