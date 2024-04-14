using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Diagnostics;
using SharpTools;
using SoundManager;

namespace DownloadSchemes
{
    /// <summary>
    /// Download utility for retrieving Sound Schemes from the GitHub repository
    /// By ORelio - (c) 2020-2024 - Available under the CDDL-1.0 license
    /// </summary>
    static class Program
    {
        static readonly string SchemesListTempFile = Path.Combine(Path.GetTempPath(), RuntimeConfig.SchemesRepositoryName.ToLowerInvariant() + ".txt");

        static readonly Dictionary<string, string> SchemesPerNtVersion = new Dictionary<string, string>
        {
            { "5.1", "Windows-XP.ths" },
            { "5.2", "Windows-XP.ths" },
            { "6.0", "Windows-Vista-7.ths" },
            { "6.1", "Windows-Vista-7.ths" },
            { "6.2", "Windows-8.ths" },
            { "6.3", "Windows-8.ths" },
            { "10.0", "Windows-10.ths" },
            { "10.0_11", "Windows-11.ths" },
        };

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // Enable TLS 1.0, 1.1, 1.2, by default .NET 4.0 will enable TLS 1.0 only
                // https://stackoverflow.com/questions/47269609/
                ServicePointManager.SecurityProtocol |= (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
            }
            catch (NotSupportedException)
            {
                FailureOfferWebpage("download_schemes_no_tls_title", "download_schemes_no_tls_text", RuntimeConfig.SchemesRepositoryUrl);
                return;
            }

            // Build a list of schemes to select by default
            // On first launch, the pre-selected scheme is the best suited one for the current OS version
            // On subsequent launches, already downloaded schemes are pre-selected: un-checking them will delete the files
            List<string> selectedSchemes = new List<string>();
            if (Directory.Exists(RuntimeConfig.SchemesFolder))
                foreach (string scheme in Directory.GetFiles(RuntimeConfig.SchemesFolder))
                    selectedSchemes.Add(Path.GetFileName(scheme));
            else if (SchemesPerNtVersion.ContainsKey(RuntimeConfig.WindowsNtVersion))
                selectedSchemes.Add(SchemesPerNtVersion[RuntimeConfig.WindowsNtVersion]);

            // Prompt user for list of schemes to download
            FormSelectFiles formSelectFiles = new FormSelectFiles(
                FetchSchemeList,
                Translations.Get("download_schemes_selection_title"),
                Translations.Get("download_schemes_selection_text"),
                Translations.Get("download_schemes_selection_fetching_list"),
                Translations.Get("download_schemes_selection_everything"),
                Translations.Get("button_ok"),
                Translations.Get("button_cancel"),
                selectedSchemes
            );
            formSelectFiles.Icon = IconExtractor.ExtractAssociatedIcon(Application.ExecutablePath);
            Application.Run(formSelectFiles);

            // Failure? Open in web browser?
            if (!formSelectFiles.Success)
            {
                FailureOfferWebpage("download_schemes_failed_title", "download_schemes_failed_text", RuntimeConfig.SchemesRepositoryUrl);
                return;
            }

            // Delete files un-selected by the user
            if (formSelectFiles.UnselectedResults.Count > 0)
            {
                foreach (string url in formSelectFiles.UnselectedResults)
                {
                    string localPath = Path.Combine(RuntimeConfig.SchemesFolder, Path.GetFileName(url));
                    if (File.Exists(localPath))
                        File.Delete(localPath);
                }
            }

            // Download user-selected files
            if (formSelectFiles.Results.Count > 0)
            {
                // Create destination Folder
                if (!Directory.Exists(RuntimeConfig.SchemesFolder))
                {
                    Directory.CreateDirectory(RuntimeConfig.SchemesFolder);

                    // Set custom folder icon
                    if (File.Exists(RuntimeConfig.SoundSchemeIcon) && !RuntimeConfig.RunningInPortableMode)
                    {
                        string desktopIniFile = Path.Combine(RuntimeConfig.SchemesFolder, "desktop.ini");
                        File.WriteAllLines(desktopIniFile, new[] {
                            "[.ShellClassInfo]",
                            "IconFile=" + Path.GetFullPath(RuntimeConfig.SoundSchemeIcon),
                            "IconIndex=0"
                        });
                        DirectoryInfo dirInfo = new DirectoryInfo(RuntimeConfig.SchemesFolder);
                        dirInfo.Attributes |= FileAttributes.System;
                        FileInfo fileInfo = new FileInfo(desktopIniFile);
                        fileInfo.Attributes |= FileAttributes.Hidden;
                    }
                }

                // List (URL => LocalPath) pairs to download, ignoring files that already exist locally
                List<KeyValuePair<string, string>> schemeDownloads = new List<KeyValuePair<string,string>>();
                foreach (string url in formSelectFiles.Results)
                {
                    string localPath = Path.Combine(RuntimeConfig.SchemesFolder, Path.GetFileName(url));
                    if (!File.Exists(localPath))
                        schemeDownloads.Add(new KeyValuePair<string, string>(url, localPath));
                }

                // Download all sound schemes
                FormDownload formDownload = new FormDownload(schemeDownloads);
                Application.Run(formDownload);

                // Failure? Open in web browser?
                if (!formDownload.Success)
                {
                    FailureOfferWebpage("download_schemes_failed_title", "download_schemes_failed_text", RuntimeConfig.SchemesRepositoryUrl);
                    return;
                }

                // If the user selected only one sound scheme, offer to apply it
                string schemeFile = null;
                if (schemeDownloads.Count == 1)
                    schemeFile = schemeDownloads[0].Value;

                // If the sound scheme for the current OS is in the set of downloaded files, offer to apply it
                if (SchemesPerNtVersion.ContainsKey(RuntimeConfig.WindowsNtVersion)
                    && schemeDownloads.Any(scheme => (Path.GetFileName(scheme.Key) == SchemesPerNtVersion[RuntimeConfig.WindowsNtVersion])))
                {
                    schemeFile = Path.Combine(RuntimeConfig.SchemesFolder, SchemesPerNtVersion[RuntimeConfig.WindowsNtVersion]);
                }

                // Run SoundManager with the scheme file to offer, or default to opening the Schemes folder
                if (schemeFile != null && File.Exists(RuntimeConfig.SoundManagerExe) && File.Exists(schemeFile))
                {
                    Process.Start(RuntimeConfig.SoundManagerExe, String.Format("\"{0}\"", schemeFile));
                }
                else if (schemeDownloads.Count > 0)
                {
                    Process.Start("explorer", '"' + RuntimeConfig.SchemesFolder + '"');
                }
            }
        }

        /// <summary>
        /// Offer to open an URL using a Warning Yes/No dialog if an error occurs
        /// </summary>
        /// <param name="title_translation">name of dialog title translation</param>
        /// <param name="text_translation">name of dialog text translation</param>
        /// <param name="url">Url to show in web browser</param>
        static void FailureOfferWebpage(string title_translation, string text_translation, string url)
        {
            if (MessageBox.Show(
                Translations.Get(text_translation),
                Translations.Get(title_translation),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Process.Start(url);
            }
        }

        /// <summary>
        /// Retrieve the list of sound scheme URLs from the GitHub repository, with 1-hour caching due to API rate-limit.
        /// </summary>
        /// <returns>List of sound scheme URLs</returns>
        static IEnumerable<string> FetchSchemeList()
        {
            if (File.Exists(SchemesListTempFile) && File.GetLastWriteTime(SchemesListTempFile) >= DateTime.Now.AddHours(-1))
            {
                return File.ReadAllLines(SchemesListTempFile);
            }
            else
            {
                IEnumerable<string> urls = GitHubApi.ListFilesInRepo(RuntimeConfig.SchemesRepositoryUsername, RuntimeConfig.SchemesRepositoryName, "/", true).Where(item => item.EndsWith(".ths"));
                File.WriteAllLines(SchemesListTempFile, urls);
                return urls;
            }
        }
    }
}
