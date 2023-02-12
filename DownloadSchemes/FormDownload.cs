using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Threading;

namespace SharpTools
{
    /// <summary>
    /// Perform file downloads while showing progress to the user
    /// By ORelio - (c) 2020-2023 - Available under the CDDL-1.0 license
    /// </summary>
    public partial class FormDownload : Form
    {
        private readonly List<KeyValuePair<string, string>> DownloadList;
        private readonly int DownloadTotalCount;
        WebClient DownloadClient = new WebClient();

        /// <summary>
        /// Holds execution status of the form. If set to false, an error occured and download was aborted.
        /// </summary>
        public bool Success = true;

        /// <summary>
        /// Create a new Download Form with a single file to download
        /// </summary>
        /// <param name="url">Source URL</param>
        /// <param name="destination">Destination path</param>
        public FormDownload(string url, string destination)
        {
            InitializeComponent();
            DownloadList = new List<KeyValuePair<string, string>>();
            DownloadList.Add(new KeyValuePair<string, string>(url, destination));
            DownloadTotalCount = DownloadList.Count;
            this.Load += new System.EventHandler(this.LaunchNextDownload);
        }

        /// <summary>
        /// Create a new Download Form with a list of files to download
        /// </summary>
        /// <param name="downloadList">List of files to download where key is remote URL, value is local destination path</param>
        public FormDownload(IEnumerable<KeyValuePair<string, string>> downloadList)
        {
            InitializeComponent();
            DownloadList = downloadList.ToList();
            DownloadTotalCount = DownloadList.Count;
            this.Load += new System.EventHandler(this.LaunchNextDownload);
        }

        /// <summary>
        /// Launch next file download on form load or download complete
        /// </summary>
        private void LaunchNextDownload(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<object, EventArgs>(LaunchNextDownload), new object[] { sender, e });
            }
            else
            {
                if (DownloadList.Count == 0)
                {
                    Close();
                }
                else
                {
                    KeyValuePair<string, string> item = DownloadList[0];
                    DownloadList.RemoveAt(0);
                    labelStatus.Text = Path.GetFileName(item.Value);
                    if (DownloadTotalCount > 1)
                        labelStatus.Text += String.Format(" ({0}/{1})", DownloadTotalCount - DownloadList.Count, DownloadTotalCount);
                    StartAsyncDownload(item.Key, item.Value, () => LaunchNextDownload(null, null), HandleDownloadError);
                }
            }
        }

        /// <summary>
        /// Start async file download for a single file
        /// </summary>
        /// <param name="url">URL to download from</param>
        /// <param name="destinationFile">Destination file on disk</param>
        /// <param name="downloadComplete">Optional callback when download is complete</param>
        /// <param name="downloadError">Optional callback when an error occured</param>
        private void StartAsyncDownload(string url, string destinationFile, Action downloadComplete = null, Action<Exception, string> downloadError = null)
        {
            if (InvokeRequired)
            {
                this.Invoke(
                    new Action<string, string, Action, Action<Exception, string>>(StartAsyncDownload),
                    new object[] { url, destinationFile, downloadComplete, downloadError }
                );
            }
            else
            {
                progressBarDownload.Value = 0;

                if (downloadComplete != null)
                {
                    DownloadClient = new WebClient();
                    DownloadClient.DownloadFileCompleted += new AsyncCompletedEventHandler(
                        (object sender, AsyncCompletedEventArgs e) =>
                        {
                            if (e.Error != null && downloadError != null)
                                downloadError(e.Error, destinationFile);
                            if (Success)
                                downloadComplete();
                        }
                    );
                }

                DownloadClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressChanged);
                DownloadClient.DownloadFileAsync(new Uri(url), destinationFile);
            }
        }

        /// <summary>
        /// Download progress callback: Update progress bar
        /// </summary>
        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (e.TotalBytesToReceive < 0 || e.BytesReceived < 0)
            {
                progressBarDownload.Value = 0;
            }
            else
            {
                double bytesIn = double.Parse(e.BytesReceived.ToString());
                double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                double percentage = bytesIn / totalBytes * 100;
                progressBarDownload.Value = int.Parse(Math.Truncate(percentage).ToString());
            }
        }

        /// <summary>
        /// Download error callback: Show error message and exit
        /// </summary>
        private void HandleDownloadError(Exception e, string outputFile)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<Exception, string>(HandleDownloadError), new object[] { e, outputFile });
            }
            else
            {
                if (File.Exists(outputFile))
                    File.Delete(outputFile);
                Exception inner = e;
                List<string> messages = new List<string>();
                do
                {
                    messages.Add(inner.Message);
                    inner = inner.InnerException;
                } while (inner != null);

                MessageBox.Show(String.Join("\n", messages), e.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Success = false;
                Close();
            }
        }

        /// <summary>
        /// Form close event: Operation cancelled by User
        /// </summary>
        private void FormDownload_Closed(object sender, FormClosedEventArgs e)
        {
            if (DownloadClient.IsBusy)
            {
                Success = false;
                DownloadClient.CancelAsync();
            }
        }
    }
}
