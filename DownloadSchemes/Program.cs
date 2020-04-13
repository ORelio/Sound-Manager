using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Net;
using System.IO;
using SharpTools;
using System.Diagnostics;

namespace DownloadSchemes
{
    static class Program
    {
        const string RepoUrl = "https://github.com/ORelio/Sound-Manager-Schemes/";
        const string FileUrlFormat = "{0}raw/master/{1}";
        const string SoundManagerExe = "SoundManager.exe";
        const string SoundSchemeIcon = "SoundScheme.ico";

        static readonly string TempListFile = Path.Combine(Path.GetTempPath(), "schemes-list.html");
        static readonly string WindowsNtVersion = String.Format("{0}.{1}", WindowsVersion.WinMajorVersion, WindowsVersion.WinMinorVersion);
        static readonly string SchemesFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), Translations.Get("app_name"));

        static readonly Dictionary<string, string> SchemesPerNtVersion = new Dictionary<string, string>
        {
            { "5.1", "Windows-XP.ths" },
            { "5.2", "Windows-XP.ths" },
            { "6.0", "Windows-Vista-7.ths" },
            { "6.1", "Windows-Vista-7.ths" },
            { "6.2", "Windows-8.ths" },
            { "6.3", "Windows-8.ths" },
            { "10.0", "Windows-10.ths" },
        };

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Enable TLS 1.0, 1.1, 1.2, by default .NET 4.0 will enable TLS 1.0 only
            // https://stackoverflow.com/questions/47269609/
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Retrieve HTML listing
            FormDownload formDownload = new FormDownload(RepoUrl, TempListFile);
            Application.Run(formDownload);

            if (!formDownload.Success)
                return;

            // Create destination Folder
            if (!Directory.Exists(SchemesFolder))
            {
                Directory.CreateDirectory(SchemesFolder);

                // Set custom folder icon
                if (File.Exists(SoundSchemeIcon))
                {
                    string desktopIniFile = Path.Combine(SchemesFolder, "desktop.ini");
                    File.WriteAllLines(desktopIniFile, new[] {
                        "[.ShellClassInfo]",
                        "IconFile=" + Path.GetFullPath(SoundSchemeIcon),
                        "IconIndex=0"
                    });
                    DirectoryInfo dirInfo = new DirectoryInfo(SchemesFolder);
                    dirInfo.Attributes |= FileAttributes.System;
                    FileInfo fileInfo = new FileInfo(desktopIniFile);
                    fileInfo.Attributes |= FileAttributes.Hidden;
                }
            }

            // Find THS file names in HTML listing and build URL => LocalPath pairs
            IEnumerable<KeyValuePair<string, string>> schemes
                = File.ReadAllText(TempListFile)
                    .Split(new[] { '>', '<' })
                    .Where(item => item.EndsWith(".ths"))
                    .Select(filename => new KeyValuePair<string, string>(
                        String.Format(FileUrlFormat, RepoUrl, filename),
                        Path.Combine(SchemesFolder, filename)));
            File.Delete(TempListFile);

            // Download all sound schemes
            formDownload = new FormDownload(schemes);
            Application.Run(formDownload);

            if (!formDownload.Success)
                return;

            // Offer to apply the default sound scheme for the current OS
            if (SchemesPerNtVersion.ContainsKey(WindowsNtVersion))
            {
                string schemeFile = Path.Combine(SchemesFolder, SchemesPerNtVersion[WindowsNtVersion]);
                if (File.Exists(SoundManagerExe) && File.Exists(schemeFile))
                {
                    Process.Start(SoundManagerExe, String.Format("\"{0}\"", schemeFile));
                }
            }
        }
    }
}
