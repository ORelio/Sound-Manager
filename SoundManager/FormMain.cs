using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Resources;
using System.Diagnostics;
using System.Windows.Forms;
using SharpTools;

namespace SoundManager
{
    /// <summary>
    /// The SoundManager user interface
    /// </summary>
    public partial class FormMain : Form
    {
        /// <summary>
        /// Holds path to the Uninstall program when this application is installed using a Setup program
        /// </summary>
        private static readonly string UninstallProgram = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Uninstall.exe");

        /// <summary>
        /// Holds GUI icons for each sound event
        /// </summary>
        private ResourceManager soundIcons = new ResourceManager("SoundManager.SoundIcons", typeof(SoundManager.SoundIcons).Assembly);

        /// <summary>
        /// Initialize window contents
        /// </summary>
        public FormMain(string importFile = null)
        {
            InitializeComponent();

            // Icon and translations

            this.Text = RuntimeConfig.AppDisplayName;
            this.Icon = IconExtractor.ExtractAssociatedIcon(Application.ExecutablePath);

            tabPageScheme.Text = Translations.Get("tab_current_scheme");
            tabPageSettings.Text = Translations.Get("tab_settings");
            soundImageText.Text = Translations.Get("no_image");
            soundInfoNameText.Text = Translations.Get("meta_name");
            soundInfoAuthorText.Text = Translations.Get("meta_author");
            soundInfoAboutText.Text = Translations.Get("meta_about");
            buttonOpen.Text = Translations.Get("button_open");
            buttonImport.Text = Translations.Get("button_import");
            buttonExport.Text = Translations.Get("button_export");
            buttonReset.Text = Translations.Get("button_reset");
            buttonExit.Text = Translations.Get("button_exit");
            imageContextMenu_Change.Text = Translations.Get("image_change");
            imageContextMenu_Remove.Text = Translations.Get("image_remove");
            soundContextMenu_Change.Text = Translations.Get("sound_change");
            soundContextMenu_OpenLocation.Text = Translations.Get("sound_open_location");
            soundContextMenu_Play.Text = Translations.Get("sound_play");
            soundContextMenu_Reset.Text = Translations.Get("sound_reset");
            soundContextMenu_Remove.Text = Translations.Get("sound_remove");
            soundContextMenu_Disable.Text = Translations.Get("system_event_disable");
            groupBoxImport.Text = Translations.Get("box_import_system_scheme");
            groupBoxSystemIntegration.Text = Translations.Get("box_system_integration");
            checkBoxPatchImageres.Text = Translations.Get("check_box_imageres_patch");
            checkBoxBgSoundPlayer.Text = Translations.Get("check_box_bg_sound_player");
            checkBoxFileAssoc.Text = Translations.Get("check_box_file_assoc");
            checkBoxMissingSoundsUseDefault.Text = Translations.Get("check_box_reset_missing_on_load");
            groupBoxMaintenance.Text = Translations.Get("box_maintenance");
            buttonReinstall.Text = Translations.Get("button_reinstall");
            buttonUninstall.Text = Translations.Get("button_uninstall");
            buttonConfigFile.Text = Translations.Get("button_config_file");
            tabPageAbout.Text = Translations.Get("tab_about");
            labelProgramName.Text = RuntimeConfig.AppDisplayName;
            labelProgramVersionAuthor.Text = "Version " + RuntimeConfig.Version + " - By ORelio";
            labelTranslationAuthor.Text = Translations.Get("translation_author");
            labelProgramDescription.Text = Translations.Get("app_desc");
            buttonHelp.Text = Translations.Get("button_help");
            buttonWebsite.Text = Translations.Get("button_website");
            buttonDownloadSchemes.Text = Translations.Get("button_download_schemes");
            groupBoxSystemInfo.Text = Translations.Get("box_system_info");

            soundInfoNameBox.AccessibleDescription = Translations.Get("meta_name_desc");
            soundInfoAuthorBox.AccessibleDescription = Translations.Get("meta_author_desc");
            soundInfoAboutBox.AccessibleDescription = Translations.Get("meta_about_desc");

            toolTipHandler.SetToolTip(soundInfoNameBox, Translations.Get("meta_name_desc"));
            toolTipHandler.SetToolTip(soundInfoAuthorBox, Translations.Get("meta_author_desc"));
            toolTipHandler.SetToolTip(soundInfoAboutBox, Translations.Get("meta_about_desc"));

            // System information

            labelSystemInfo.Text = String.Format(
                "{0} / Windows NT {1}.{2}",
                WindowsVersion.FriendlyName,
                WindowsVersion.WinMajorVersion,
                WindowsVersion.WinMinorVersion
            );

            labelSystemSupportStatus.Text =
                WindowsVersion.IsBetween(RuntimeConfig.SupportedWindowsVersionMin, RuntimeConfig.SupportedWindowsVersionMax)
                    ? Translations.Get("supported_system_version")
                    : Translations.Get("unsupported_system_version");

            // Load UI settings

            checkBoxPatchImageres.Enabled = ImageresPatcher.IsPatchingPossible;
            checkBoxPatchImageres.Checked = ImageresPatcher.IsPatchingPossible && Settings.PatchStartupSound;
            checkBoxPatchImageres.CheckedChanged += new System.EventHandler(this.checkBoxPatchImageres_CheckedChanged);

            checkBoxBgSoundPlayer.Enabled = BgSoundPlayer.RequiredForThisWindowsVersion;
            checkBoxBgSoundPlayer.Checked = BgSoundPlayer.RequiredForThisWindowsVersion && BgSoundPlayer.IsRegisteredForStartup();
            checkBoxBgSoundPlayer.CheckedChanged += new EventHandler(checkBoxBgSoundPlayer_CheckedChanged);

            checkBoxMissingSoundsUseDefault.Checked = Settings.MissingSoundUseDefault;
            checkBoxMissingSoundsUseDefault.CheckedChanged += new EventHandler(checkBoxMissingSoundsUseDefault_CheckedChanged);

            checkBoxFileAssoc.Checked = SoundArchive.FileAssociation;
            checkBoxFileAssoc.CheckedChanged += new System.EventHandler(this.checkBoxFileAssoc_CheckedChanged);

            // Image and scheme info

            RefreshSchemeMetadata();

            // Sound event list

            soundList.View = View.LargeIcon;
            soundList.MultiSelect = false;
            soundList.LargeImageList = new ImageList();
            soundList.LargeImageList.ImageSize = new Size(32, 32);
            soundList.ShowItemToolTips = true;

            foreach (SoundEvent soundEvent in SoundEvent.GetAll())
            {
                ListViewItem item = soundList.Items.Add(soundEvent.DisplayName);
                item.Tag = soundEvent;
                RefreshEventItem(item);
            }

            // Load system sound schemes list

            foreach (SoundScheme scheme in SoundScheme.GetSchemeList())
                if (scheme.ToString() != RuntimeConfig.AppDisplayName && scheme.ToString() != ".None")
                    comboBoxSystemSchemes.Items.Add(scheme);
            if (comboBoxSystemSchemes.Items.Count > 0)
                comboBoxSystemSchemes.SelectedIndex = 0;

            // Auto-import sound scheme passed as program argument

            if (importFile != null && File.Exists(importFile))
            {
                if (MessageBox.Show(
                    String.Concat(Translations.Get("scheme_load_prompt_text"), '\n', Path.GetFileName(importFile)),
                    Translations.Get("scheme_load_prompt_title"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ImportSoundScheme(importFile);
                    foreach (Process process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Application.ExecutablePath)))
                        if (process.Id != Process.GetCurrentProcess().Id && process.MainWindowTitle == this.Text)
                                process.CloseMainWindow();
                }
                else Environment.Exit(0);
            }
        }

        /// <summary>
        /// Reload scheme image and info
        /// </summary>
        private void RefreshSchemeMetadata()
        {
            SchemeMeta.ReloadFromDisk();
            soundImage.Image = SchemeMeta.Thumbnail;
            soundImageText.Visible = (soundImage.Image == null);

            soundInfoNameBox.Text = SchemeMeta.Name;
            soundInfoAuthorBox.Text = SchemeMeta.Author;
            soundInfoAboutBox.Text = SchemeMeta.About;
        }

        /// <summary>
        /// Refresh sound event icon and description in event list
        /// </summary>
        /// <param name="soundEvent">Sound Event to refresh</param>
        private void RefreshEventItem(ListViewItem item)
        {
            SoundEvent soundEvent = item.Tag as SoundEvent;
            string iconName = soundEvent.InternalName;
            Bitmap icon = soundIcons.GetObject(iconName, SoundManager.SoundIcons.Culture) as Bitmap;

            if (soundList.LargeImageList.Images.ContainsKey(iconName))
                soundList.LargeImageList.Images.RemoveByKey(iconName);

            if (icon != null)
            {
                if (soundEvent.Disabled)
                {
                    // Convert icon to grayscale
                    for (int x = 0; x < icon.Width; x++)
                        for (int y = 0; y < icon.Height; y++)
                            icon.SetPixel(x, y, icon.GetPixel(x, y).GetBrightness() >= 0.9 ? Color.White : Color.LightGray);
                }
                soundList.LargeImageList.Images.Add(iconName, icon);
            }

            item.Text = soundEvent.DisplayName;
            item.ToolTipText = soundEvent.Description;
            item.ImageKey = iconName;

            if (soundEvent.Disabled)
            {
                item.Text += String.Format("\n({0})", Translations.Get("system_event_disabled_label"));
                item.ToolTipText += String.Format(" ({0})", Translations.Get("system_event_disabled_desc"));
            }
        }

        /// <summary>
        /// Lock/Unlock buttons on tab change
        /// </summary>
        void mainTabs_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            buttonOpen.Enabled = mainTabs.SelectedIndex == 0;
            buttonExport.Enabled = mainTabs.SelectedIndex == 0;
            buttonReset.Enabled = mainTabs.SelectedIndex == 0;
        }

        /// <summary>
        /// Hovering the window with a file
        /// </summary>
        void window_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) && mainTabs.SelectedIndex == 0)
            {
                e.Effect = DragDropEffects.Copy;
            }
            else e.Effect = DragDropEffects.None;
        }

        /// <summary>
        /// Dropping a file
        /// </summary>
        void window_DragDrop(object sender, DragEventArgs e)
        {
            if (mainTabs.SelectedIndex == 0)
            {
                string[] files = (e.Data.GetData(DataFormats.FileDrop) as string[] ?? new string[] { });
                if (files.Length > 0)
                {
                    try
                    {
                        Image.FromFile(files[0]);
                        changeSchemeThumbnail(files[0]);
                    }
                    catch
                    {
                        string fileExtension = Path.GetExtension(files[0]).ToLower().Trim('.');
                        if (fileExtension == SoundArchive.FileExtension || fileExtension == SoundArchiveProprietary.FileExtension)
                        {
                            if (MessageBox.Show(
                                String.Concat(Translations.Get("scheme_load_prompt_text"), '\n', Path.GetFileName(files[0])),
                                Translations.Get("scheme_load_prompt_title"),
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                ImportSoundScheme(files[0]);
                            }
                        }
                        else if (soundList.SelectedItems.Count > 0)
                        {
                            SoundEvent soundEvent = soundList.SelectedItems[0].Tag as SoundEvent;
                            if (MessageBox.Show(
                                    Translations.Get("drag_drop_sound_confirm_text") + '\n' + soundEvent.DisplayName,
                                    Translations.Get("drag_drop_sound_confirm_title"),
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                replaceSoundEvent(soundEvent, files[0]);
                            }
                        }
                        else
                        {
                            MessageBox.Show(
                                Translations.Get("drag_drop_no_target_sound_text"),
                                Translations.Get("drag_drop_no_target_sound_title"),
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Show context menu for a sound event
        /// </summary>
        /// <param name="soundEvent">Associated sound event</param>
        /// <param name="positionOnScreen">Menu position on screen</param>
        private void ShowSoundContextMenu(SoundEvent soundEvent, Point positionOnScreen)
        {
            soundContextMenu_Disable.Text = Translations.Get(soundEvent.Disabled ? "system_event_enable" : "system_event_disable");
            soundContextMenu.Show(positionOnScreen);
        }

        /// <summary>
        /// Handle mouse clicks on sound list: context menu, play on single click, change on double click
        /// </summary>
        private void soundList_MouseClick(object sender, MouseEventArgs e)
        {
            if (soundList.FocusedItem != null
                && e.Button == MouseButtons.Right
                && soundList.FocusedItem.Bounds.Contains(e.Location) == true)
            {
                ShowSoundContextMenu(soundList.FocusedItem.Tag as SoundEvent, Cursor.Position);
            }
        }

        /// <summary>
        /// Change sound on double click
        /// </summary>
        private void soundList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            soundContextMenu_Change_Click(sender, e);
        }

        /// <summary>
        /// Play sound on item change
        /// </summary>
        private void soundList_SelectedIndexChanged(object sender, EventArgs e)
        {
            soundContextMenu_Play_Click(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handle mouse clicks on scheme thumbnail : context menu, replace on single click
        /// </summary>
        private void soundImage_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                imageContextMenu.Show(Cursor.Position);
            }
            else imageContextMenu_Change_Click(sender, e);
        }

        /// <summary>
        /// Open context menus using the Menu keyboard key, load & save keyboard shortcuts
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (mainTabs.SelectedIndex == 0)
            {
                if (keyData == Keys.Apps)
                {
                    if (soundList.Focused && soundList.FocusedItem != null)
                    {
                        ShowSoundContextMenu(soundList.FocusedItem.Tag as SoundEvent, new Point(
                            soundList.PointToScreen(Point.Empty).X + soundList.FocusedItem.Bounds.Location.X + soundList.FocusedItem.Bounds.Width / 2,
                            soundList.PointToScreen(Point.Empty).Y + soundList.FocusedItem.Bounds.Location.Y + soundList.FocusedItem.Bounds.Height / 2
                        ));
                    }
                    else if (!soundInfoNameBox.Focused && !soundInfoAuthorBox.Focused && !soundInfoAboutBox.Focused
                        && !buttonOpen.Focused && !buttonExport.Focused && !buttonReset.Focused && !buttonExit.Focused
                        && mainTabs.SelectedIndex == 0)
                    {
                        imageContextMenu.Show(new Point(
                            soundImage.PointToScreen(Point.Empty).X + soundImage.Bounds.Location.X + soundImage.Bounds.Width / 2,
                            soundImage.PointToScreen(Point.Empty).Y + soundImage.Bounds.Location.Y + soundImage.Bounds.Width / 2
                        ));
                    }
                }
                else if (keyData == (Keys.Control | Keys.O))
                {
                    buttonImport_Click(this, EventArgs.Empty);
                }
                else if (keyData == (Keys.Control | Keys.S))
                {
                    buttonExport_Click(this, EventArgs.Empty);
                }
                else return base.ProcessCmdKey(ref msg, keyData);
            }
            else return base.ProcessCmdKey(ref msg, keyData);
            return true;
        }

        /// <summary>
        /// Replace a sound event
        /// </summary>
        private void soundContextMenu_Change_Click(object sender, EventArgs e)
        {
            if (soundList.FocusedItem != null)
            {
                SoundEvent soundEvent = soundList.FocusedItem.Tag as SoundEvent;
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = Translations.Get(SoundScheme.CanConvertSounds ? "browse_media_files" : "browse_wave_files")
                    + (SoundScheme.CanConvertSounds
                        ? "|*.wav;*.mp3;*.wma;*.ogg;*.aac;*.cda;*.m4a;*.flac;*.ac3;*.dts;*.mp4;*.avi;*.wmv;*.mkv;*.flv"
                        : "|*.wav");
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    replaceSoundEvent(soundEvent, dlg.FileName);
                }
            }
        }

        /// <summary>
        /// Browse for a new scheme thumbnail
        /// </summary>
        private void imageContextMenu_Change_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = Translations.Get("browse_image_files") + "|*.bmp;*.png;*.jpg;*.jpeg;*.gif;*.tiff";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                changeSchemeThumbnail(dlg.FileName);
            }
        }

        /// <summary>
        /// Replace the scheme thumbnail
        /// </summary>
        private void changeSchemeThumbnail(string imageFile)
        {
            try
            {
                SchemeMeta.Thumbnail = Image.FromFile(imageFile);
                RefreshSchemeMetadata();
            }
            catch (Exception imageException)
            {
                MessageBox.Show(
                    Translations.Get("image_load_failed_text") + '\n' + imageException.Message,
                    Translations.Get("image_load_failed_title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Replace the sound file associated with the specified sound event
        /// </summary>
        private void replaceSoundEvent(SoundEvent soundEvent, string soundFile)
        {
            try
            {
                SoundScheme.Update(soundEvent, soundFile);
                soundContextMenu_Play_Click(this, EventArgs.Empty);
                SoundScheme.Apply(SoundScheme.GetSchemeSoundManager(), Settings.MissingSoundUseDefault);
            }
            catch (Exception loadException)
            {
                MessageBox.Show(
                    Translations.Get("sound_load_failed_text") + '\n' + loadException.Message,
                    Translations.Get("sound_load_failed_title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Import a sound archive file
        /// </summary>
        private void buttonImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (Directory.Exists(RuntimeConfig.SchemesFolder))
                dlg.InitialDirectory = RuntimeConfig.SchemesFolder;
            dlg.Filter = String.Concat(Translations.Get("browse_scheme_files"), "|*.", SoundArchive.FileExtension, ";*." + SoundArchiveProprietary.FileExtension);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                ImportSoundScheme(dlg.FileName);
            }
        }

        /// <summary>
        /// Import the specified sound archive file
        /// </summary>
        /// <param name="schemePath"></param>
        private void ImportSoundScheme(string schemePath)
        {
            try
            {
                SoundArchive.Import(schemePath);
                RefreshSchemeMetadata();
                soundList.Select();
                soundContextMenu_Play_Click(this, EventArgs.Empty);
            }
            catch (Exception importException)
            {
                MessageBox.Show(
                    Translations.Get("scheme_load_failed_text") + '\n' + importException.Message,
                    Translations.Get("scheme_load_failed_title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Export to a sound archive file
        /// </summary>
        private void buttonExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = String.Concat(Translations.Get("browse_scheme_files"), "|*.", SoundArchive.FileExtension);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    SoundArchive.Export(dlg.FileName);
                }
                catch (Exception exportException)
                {
                    MessageBox.Show(
                        Translations.Get("scheme_export_failed_text") + '\n' + exportException.Message,
                        Translations.Get("scheme_export_failed_title"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        /// <summary>
        /// Play a sound event
        /// </summary>
        private void soundContextMenu_Play_Click(object sender, EventArgs e)
        {
            if (soundList.FocusedItem != null)
            {
                SoundEvent soundEvent = soundList.FocusedItem.Tag as SoundEvent;
                if (File.Exists(soundEvent.FilePath))
                {
                    try
                    {
                        System.Media.SoundPlayer player = new System.Media.SoundPlayer(soundEvent.FilePath);
                        player.Play();
                    }
                    catch (InvalidOperationException)
                    {
                        //Not a WAV file
                        File.Delete(soundEvent.FilePath);
                    }
                }
                else // No sound file associated with this event
                {
                    // The ListView UI element triggers the "Select" sound event when selected.
                    // If we do not play anything, Windows attempts to play the "Select" sound event, but we do not want that.
                    // As a workaround, play a random WAV file but immediately stop it so the sound will not be heard.
                    SoundEvent eventWithFile = SoundEvent.GetAll().FirstOrDefault(evt => File.Exists(evt.FilePath));
                    if (eventWithFile != null) // If no event has a file, then "Select" has no file as well so we are done.
                    {
                        string dummyFilePath = eventWithFile.FilePath;
                        try
                        {
                            System.Media.SoundPlayer dummyPlayer = new System.Media.SoundPlayer(dummyFilePath);
                            dummyPlayer.Play();
                            dummyPlayer.Stop();
                        }
                        catch { /* Avoid crashing if file is invalid */ }
                    }
                }
            }
        }

        /// <summary>
        /// Show sound event file in Windows Explorer
        /// </summary>
        private void soundContextMenu_OpenLocation_Click(object sender, EventArgs e)
        {
            if (soundList.FocusedItem != null)
            {
                SoundEvent soundEvent = soundList.FocusedItem.Tag as SoundEvent;
                try
                {
                    if (File.Exists(soundEvent.FilePath))
                    {
                        Process.Start("explorer", "/select, \"" + soundEvent.FilePath + '"');
                    }
                    else
                    {
                        Process.Start("explorer", '"' + SoundEvent.DataDirectory + '"');
                    }
                }
                catch (Exception browseException)
                {
                    MessageBox.Show(
                        Translations.Get("browse_failed_text") + '\n' + browseException.Message,
                        Translations.Get("browse_failed_title"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        /// <summary>
        /// Reset a sound event to the default system sound
        /// </summary>
        private void soundContextMenu_Reset_Click(object sender, EventArgs e)
        {
            if (soundList.FocusedItem != null)
            {
                SoundEvent soundEvent = soundList.FocusedItem.Tag as SoundEvent;
                SoundScheme.CopyDefault(soundEvent);
                soundContextMenu_Play_Click(sender, e);
            }
        }

        /// <summary>
        /// Reset all sound events to the default system sounds
        /// </summary>
        private void buttonReset_Click(object sender, EventArgs e)
        {
            loadSystemScheme(true, null);
        }

        /// <summary>
        /// Reset all sound events to the specified system sound sheme
        /// </summary>
        private void buttonImportSystemScheme_Click(object sender, EventArgs e)
        {
            loadSystemScheme(false, comboBoxSystemSchemes.SelectedItem as SoundScheme);
            mainTabs.SelectedIndex = 0;
            soundContextMenu_Play_Click(sender, e);
        }

        /// <summary>
        /// Reset all sound events to the specified system sound sheme
        /// </summary>
        private void loadSystemScheme(bool warning, SoundScheme scheme)
        {
            if (!warning || MessageBox.Show(
                    Translations.Get("reset_warn_text"),
                    Translations.Get("reset_warn_title"),
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning
                ) == DialogResult.OK)
            {
                try
                {
                    foreach (SoundEvent soundEvent in SoundEvent.GetAll())
                        SoundScheme.CopyDefault(soundEvent, scheme);
                    SchemeMeta.ResetAll();
                    if (scheme != null)
                    {
                        SchemeMeta.Name = scheme.ToString();
                        SchemeMeta.Author = "";
                        SchemeMeta.About = "";
                    }
                    RefreshSchemeMetadata();
                    imageContextMenu_Remove_Click(this, EventArgs.Empty);
                }
                catch (Exception error)
                {
                    MessageBox.Show(
                        error.Message,
                        error.GetType().Name,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Remove a sound event
        /// </summary>
        private void soundContextMenu_Remove_Click(object sender, EventArgs e)
        {
            if (soundList.FocusedItem != null)
            {
                SoundEvent soundEvent = soundList.FocusedItem.Tag as SoundEvent;
                SoundScheme.Remove(soundEvent);
            }
        }

        /// <summary>
        /// Disable or Enable a sound event
        /// </summary>
        private void soundContextMenu_Disable_Click(object sender, EventArgs e)
        {
            if (soundList.FocusedItem != null)
            {
                if (Settings.DisabledSoundEvents.Count != 0
                    || MessageBox.Show(
                        Translations.Get("system_event_disable_confirm_text"),
                        Translations.Get("system_event_disable_confirm_title"),
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Question) == DialogResult.OK
                    )
                {
                    SoundEvent soundEvent = soundList.FocusedItem.Tag as SoundEvent;
                    soundEvent.Disabled = !soundEvent.Disabled;
                    RefreshEventItem(soundList.FocusedItem);
                }
            }
        }

        /// <summary>
        /// Remove the thumbnail
        /// </summary>
        private void imageContextMenu_Remove_Click(object sender, EventArgs e)
        {
            SchemeMeta.Thumbnail = null;
            RefreshSchemeMetadata();
        }

        /// <summary>
        /// Exit the application
        /// </summary>
        private void buttonExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Save sound info when a field loses focus
        /// </summary>
        private void soundInfo_Box_LostFocus(object sender, EventArgs e)
        {
            if (sender == soundInfoNameBox)
                SchemeMeta.Name = soundInfoNameBox.Text;
            else if (sender == soundInfoAuthorBox)
                SchemeMeta.Author = soundInfoAuthorBox.Text;
            else if (sender == soundInfoAboutBox)
                SchemeMeta.About = soundInfoAboutBox.Text;
        }

        /// <summary>
        /// Open URL that may be present in the About field
        /// </summary>
        private void soundInfoAboutBox_DoubleClick(object sender, MouseEventArgs e)
        {
            if (Uri.IsWellFormedUriString(soundInfoAboutBox.Text, UriKind.Absolute)
                && MessageBox.Show(
                    Translations.Get("webpage_open_prompt_text") + '\n' + soundInfoAboutBox.Text,
                    Translations.Get("webpage_open_prompt_title"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                ) == DialogResult.Yes
            )
            {
                Process.Start(soundInfoAboutBox.Text);
            }
        }

        /// <summary>
        /// Change "Patch the built-in system startup sound" setting
        /// </summary>
        private void checkBoxPatchImageres_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxPatchImageres.Checked && ImageresPatcher.IsPatchingNotRecommended)
            {
                if (MessageBox.Show(
                    Translations.Get("startup_patch_not_recommended_text"),
                    Translations.Get("startup_patch_not_recommended_title"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                ) != DialogResult.Yes)
                {
                    checkBoxPatchImageres.Checked = false;
                    return;
                }
            }

            if (FileSystemAdmin.IsAdmin())
            {
                try
                {
                    if (checkBoxPatchImageres.Checked)
                    {
                        SoundEvent startupSound = SoundEvent.Get(SoundEvent.EventType.Startup);
                        if (File.Exists(startupSound.FilePath))
                            ImageresPatcher.Patch(startupSound.FilePath);
                    }
                    else
                    {
                        ImageresPatcher.Restore();
                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show(
                        error.Message,
                        error.GetType().Name,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }

            if (!FileSystemAdmin.IsAdmin() && checkBoxPatchImageres.Checked)
            {
                MessageBox.Show(
                    Translations.Get("startup_patch_not_elevated_text"),
                    Translations.Get("startup_patch_not_elevated_title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }

            Settings.PatchStartupSound = checkBoxPatchImageres.Checked;
            Settings.Save();

            UpdateStartupSoundSetting();
        }

        /// <summary>
        /// Change "Start with Windows to play the system start sound" setting
        /// </summary>
        private void checkBoxBgSoundPlayer_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                BgSoundPlayer.SetRegisteredForStartup(checkBoxBgSoundPlayer.Checked, true);
                UpdateStartupSoundSetting();
            }
            catch (Exception error)
            {
                MessageBox.Show(
                    error.Message,
                    error.GetType().Name,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Determine whether the built-in system startup sound should be enabled and apply the appropriate setting
        /// </summary>
        private void UpdateStartupSoundSetting()
        {
            if (Settings.PatchStartupSound)
            {
                // When patching the startup sound, we want it to play regardless of other settings
                SystemStartupSound.Enabled = true;
            }
            else if (BgSoundPlayer.IsRegisteredForStartup())
            {
                // When using the background sound player and NOT patching the startup sound, mute the built-in startup sound and play the custom one using BgSoundPlayer
                SystemStartupSound.Enabled = false;
            }
            else
            {
                // When using none of the above, restore the system default behavior regarding startup sound
                SystemStartupSound.Enabled = SystemStartupSound.DefaultEnabled;
            }
        }

        /// <summary>
        /// Change the File Association setting
        /// </summary>
        private void checkBoxFileAssoc_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxFileAssoc.Checked)
                SoundArchive.AssocFiles();
            else SoundArchive.UnAssocFiles();
        }

        /// <summary>
        /// Change "Reset missing sounds when loading a sound scheme file" setting
        /// </summary>
        private void checkBoxMissingSoundsUseDefault_CheckedChanged(object sender, EventArgs e)
        {
            Settings.MissingSoundUseDefault = checkBoxMissingSoundsUseDefault.Checked;
            Settings.Save();
        }

        /// <summary>
        /// Reinstall the application
        /// </summary>
        private void buttonReinstall_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                    Translations.Get("reinstall_confirm_text"),
                    Translations.Get("reinstall_confirm_title"),
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning
                ) == DialogResult.OK
            )
            {
                Program.Uninstall();
                Program.Setup(true, true);
                Process.Start(Application.ExecutablePath);
                Close();
            }
        }

        /// <summary>
        /// Uninstall the application
        /// </summary>
        private void buttonUninstall_Click(object sender, EventArgs e)
        {
            if (File.Exists(UninstallProgram))
            {
                try
                {
                    Process.Start(UninstallProgram);
                    Close();
                }
                catch { /* Failed to start or user cancelled UAC prompt */ }
            }
            else
            {
                if (MessageBox.Show(
                        Translations.Get("uninstall_confirm_text"),
                        Translations.Get("uninstall_confirm_title"),
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Warning
                    ) == DialogResult.OK
                )
                {
                    Program.Uninstall();
                    Close();
                }
            }
        }

        /// <summary>
        /// Manually edit configuration file
        /// </summary>
        private void buttonConfigFile_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                        Translations.Get("config_file_confirm_text"),
                        Translations.Get("config_file_confirm_title"),
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Warning
                    ) == DialogResult.OK
                )
            {
                if (!File.Exists(RuntimeConfig.SettingsFile))
                    Settings.Save();
                Process.Start("notepad", "\"" + RuntimeConfig.SettingsFile + "\"");
                Close();
            }
        }

        /// <summary>
        /// Launch program help
        /// </summary>
        private void buttonHelp_Click(object sender, EventArgs e)
        {
            string helpFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), Translations.Get("help_file"));
            if (File.Exists(helpFile))
            {
                Process.Start("notepad", "\"" + helpFile + "\"");
            }
            else
            {
                MessageBox.Show(
                    Translations.Get("help_file_not_found_text"),
                    Translations.Get("help_file_not_found_title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        /// <summary>
        /// Launch program website
        /// </summary>
        private void buttonWebsite_Click(object sender, EventArgs e)
        {
            if (Translations.SystemLanguage == "fra")
            {
                Process.Start("https://microzoom.fr/gestionnaire-de-sons");
            }
            else Process.Start("https://github.com/ORelio/Sound-Manager");
        }

        /// <summary>
        /// Launch sound scheme download tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDownloadSchemes_Click(object sender, EventArgs e)
        {
            if (File.Exists("DownloadSchemes.exe"))
            {
                Process.Start("DownloadSchemes.exe");
            }
            else
            {
                Process.Start("https://github.com/ORelio/Sound-Manager-Schemes");
            }
        }
    }
}
