using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using SharpTools;

namespace SharpTools
{
    /// <summary>
    /// Download or display a hierarchized list of available files and let the user make their selection
    /// Once closed, the form makes available the list of items selected by the user
    /// By ORelio - (c) 2023 - Available under the CDDL-1.0 license
    /// </summary>
    public partial class FormSelectFiles : Form
    {
        private const string RootNodeKey = "root";
        private readonly string RootNodeText = "Everything";
        private readonly Func<IEnumerable<string>> DownloadAction = null;
        private readonly string DownloadMessage = null;
        private readonly string SelectionMessage = null;
        private readonly HashSet<string> DefaultItems = new HashSet<string>();
        private int ProcessCheckEvents = 0;

        /// <summary>
        /// Holds execution status of the form. If set to false, an error occurred in urlProvider.
        /// </summary>
        public bool Success = true;

        /// <summary>
        /// Holds the set of URLs selected by the user
        /// </summary>
        public readonly List<string> Results = new List<string>();

        /// <summary>
        /// Holds the set of URLds NOT selected by the user
        /// </summary>
        public readonly List<string> UnselectedResults = new List<string>();

        /// <summary>
        /// Initialize a new form (internal)
        /// </summary>
        /// <param name="title">Window title</param>
        /// <param name="message">Message shown in the window</param>
        /// <param name="messageDl">Message shown in the window while running the URL provider</param>
        /// <param name="rootNode">Label of the root node comprising all the items</param>
        /// <param name="okText">Label on the OK button</param>
        /// <param name="cancelText">Label on the CANCEL button</param>
        /// <param name="selected">Set of items to select by default</param>
        private FormSelectFiles(string title, string message, string messageDl, string rootNode, string okText, string cancelText, IEnumerable<string> selected)
        {
            InitializeComponent();
            if (title != null)
                this.Text = title;
            if (message != null)
                this.SelectionMessage = message;
            if (messageDl != null)
                this.DownloadMessage = messageDl;
            if (rootNode != null)
                RootNodeText = rootNode;
            if (okText != null)
                this.buttonOK.Text = okText;
            if (cancelText != null)
                this.buttonCancel.Text = cancelText;
            if (selected != null)
                foreach (string item in selected)
                    if (!DefaultItems.Contains(item))
                        DefaultItems.Add(item);
        }

        /// <summary>
        /// Initialize a new form using the provided set of URLs
        /// </summary>
        /// <param name="urls">URLs to dispay in the form as a hierarchized list of files</param>
        /// <param name="title">Window title</param>
        /// <param name="message">Message shown in the window</param>
        /// <param name="rootNode">Label of the root node comprising all the items</param>
        /// <param name="okText">Label on the OK button</param>
        /// <param name="cancelText">Label on the CANCEL button</param>
        /// <param name="selected">Set of items to select by default</param>
        public FormSelectFiles(IEnumerable<string> urls, string title, string message, string rootNode, string okText, string cancelText, IEnumerable<string> selected)
            : this(title, message, null, rootNode, okText, cancelText, selected)
        {
            labelDescription.Text = message;
            BuildItemList(urls);
        }

        /// <summary>
        /// Initialize a new form using and URL provided callback.
        /// The form will show that URLs are being retrieved while running the callback.
        /// </summary>
        /// <param name="urlProvider">Callback returning a set of URLs to dispay in the form as a hierarchized list of files</param>
        /// <param name="title">Window title</param>
        /// <param name="message">Message shown in the window</param>
        /// <param name="messageDl">Message shown in the window while running the URL provider</param>
        /// <param name="rootNode">Label of the root node comprising all the items</param>
        /// <param name="okText">Label on the OK button</param>
        /// <param name="cancelText">Label on the CANCEL button</param>
        /// <param name="selected">Set of items to select by default</param>
        public FormSelectFiles(Func<IEnumerable<string>> urlProvider, string title, string message, string messageDl, string rootNode, string okText, string cancelText, IEnumerable<string> selected)
            : this(title, message, messageDl, rootNode, okText, cancelText, selected)
        {
            labelDescription.Text = "";
            DownloadAction = urlProvider;
        }

        /// <summary>
        /// Form initialization. Runs the callback in background if needed.
        /// </summary>
        private void FormSelectFiles_Load(object sender, EventArgs e)
        {
            if (DownloadAction != null)
            {
                buttonOK.Enabled = false;
                labelDescription.Text = DownloadMessage;
                new Thread(() => {
                    try
                    {
                        IEnumerable<string> urls = DownloadAction();
                        BuildItemList(urls);
                    }
                    catch (Exception error)
                    {
                        this.Invoke(new Action(Hide));

                        Exception inner = error;
                        List<string> messages = new List<string>();
                        do
                        {
                            messages.Add(inner.Message);
                            inner = inner.InnerException;
                        } while (inner != null);

                        MessageBox.Show(String.Join("\n", messages), error.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Success = false;
                        this.Invoke(new Action(Close));
                    }
                }).Start();
            }
        }

        /// <summary>
        /// Populate the Tree View from the provided set of URLs
        /// </summary>
        /// <param name="urls">URLs to populate in the Tree View</param>
        private void BuildItemList(IEnumerable<string> urls)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<IEnumerable<string>>(BuildItemList), new object[] { urls });
            }
            else
            {
                if (urls == null || urls.Count() == 0)
                    throw new ArgumentException("urls list is empty", "urls");

                var commonPrefix = new string(
                    urls.First()
                        .Substring(0, urls.Min(s => s.Length))
                        .TakeWhile((c, i) => urls.All(s => s[i] == c)).ToArray());

                TreeNode rootNode = treeViewFiles.Nodes.Add(RootNodeKey, RootNodeText);

                foreach (string url in urls)
                {
                    string fileName = System.IO.Path.GetFileName(url);
                    string[] path = url.Substring(commonPrefix.Length).Split('/');
                    path[path.Length - 1] = System.IO.Path.GetFileNameWithoutExtension(url).Replace('-', ' ');
                    TreeNode node = rootNode;
                    for (int i = 0; i < path.Length; i++)
                    {
                        if (!node.Nodes.ContainsKey(path[i]))
                            node.Nodes.Add(path[i], path[i]);
                        node = node.Nodes[path[i]];
                        if (i == path.Length - 1)
                        {
                            node.Tag = url;
                            if (DefaultItems.Contains(fileName))
                                node.Checked = true;
                        }
                    }
                }

                treeViewFiles.Nodes[0].Expand();
                buttonOK.Enabled = true;
                labelDescription.Text = SelectionMessage;

                //Fix TriStateTreeView checkboxes not updating
                UpdateCheckBoxes(rootNode);
                treeViewFiles.AfterCheck += new TreeViewEventHandler((o, e) => {
                    if (ProcessCheckEvents == 0 && e.Node.Nodes.Count == 0)
                    {
                        ProcessCheckEvents++;
                        UpdateCheckBoxes(rootNode);
                        ProcessCheckEvents--;
                    }
                });
            }
        }

        /// <summary>
        /// Update check boxes when all or no child items are checked
        /// </summary>
        /// <param name="rootNode">Root node to start from</param>
        /// <returns>TRUE or FALSE if all childs are (un)checked</returns>
        private bool? UpdateCheckBoxes(TreeNode rootNode)
        {
            if (rootNode.Nodes.Count > 0)
            {
                bool foundOneUndetermined = false;
                bool foundOneChecked = false;
                bool foundOneNotChecked = false;

                foreach (TreeNode node in rootNode.Nodes)
                {
                    bool? childStatus = UpdateCheckBoxes(node);
                    if (childStatus.HasValue)
                    {
                        if (childStatus.Value)
                            foundOneChecked = true;
                        else foundOneNotChecked = true;
                    }
                    else foundOneUndetermined = true;
                }

                if (!foundOneUndetermined)
                {
                    if (foundOneChecked && !foundOneNotChecked)
                        return rootNode.Checked = true;
                    if (!foundOneChecked && foundOneNotChecked)
                        return rootNode.Checked = false;
                }

                return null; // undetermined or mixed nodes below
            }
            else return rootNode.Checked;
        }

        /// <summary>
        /// Populate Results from the file list
        /// </summary>
        /// <param name="rootNode">Root node to start from</param>
        private void PopulateSelectedItems(TreeNode rootNode)
        {
            string rootTag = rootNode.Tag as string;
            if (!String.IsNullOrEmpty(rootTag))
            {
                if (rootNode.Checked)
                    Results.Add(rootTag);
                else UnselectedResults.Add(rootTag);
            }
            foreach (TreeNode node in rootNode.Nodes)
                PopulateSelectedItems(node);
        }

        /// <summary>
        /// When user clicks the OK button
        /// </summary>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            PopulateSelectedItems(treeViewFiles.Nodes[RootNodeKey]);
            Close();
        }

        /// <summary>
        /// When user clicks the Cancel button
        /// </summary>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
