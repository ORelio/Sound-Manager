namespace SoundManager
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.soundContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.soundContextMenu_Play = new System.Windows.Forms.ToolStripMenuItem();
            this.soundContextMenu_Change = new System.Windows.Forms.ToolStripMenuItem();
            this.soundContextMenu_OpenLocation = new System.Windows.Forms.ToolStripMenuItem();
            this.soundContextMenu_Reset = new System.Windows.Forms.ToolStripMenuItem();
            this.soundContextMenu_Remove = new System.Windows.Forms.ToolStripMenuItem();
            this.soundContextMenu_Disable = new System.Windows.Forms.ToolStripMenuItem();
            this.imageContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.imageContextMenu_Change = new System.Windows.Forms.ToolStripMenuItem();
            this.imageContextMenu_Remove = new System.Windows.Forms.ToolStripMenuItem();
            this.mainTabs = new System.Windows.Forms.TabControl();
            this.tabPageScheme = new System.Windows.Forms.TabPage();
            this.soundInfoNameBox = new System.Windows.Forms.TextBox();
            this.soundInfoNameText = new System.Windows.Forms.Label();
            this.soundInfoAuthorBox = new System.Windows.Forms.TextBox();
            this.soundInfoAuthorText = new System.Windows.Forms.Label();
            this.soundInfoAboutBox = new System.Windows.Forms.TextBox();
            this.soundInfoAboutText = new System.Windows.Forms.Label();
            this.soundImageText = new System.Windows.Forms.Label();
            this.soundImage = new System.Windows.Forms.PictureBox();
            this.soundList = new System.Windows.Forms.ListView();
            this.tabPageSettings = new System.Windows.Forms.TabPage();
            this.groupBoxSystemInfo = new System.Windows.Forms.GroupBox();
            this.labelSystemSupportStatus = new System.Windows.Forms.Label();
            this.labelSystemInfo = new System.Windows.Forms.Label();
            this.groupBoxMaintenance = new System.Windows.Forms.GroupBox();
            this.buttonUninstall = new System.Windows.Forms.Button();
            this.buttonReinstall = new System.Windows.Forms.Button();
            this.groupBoxSystemIntegration = new System.Windows.Forms.GroupBox();
            this.checkBoxMissingSoundsUseDefault = new System.Windows.Forms.CheckBox();
            this.checkBoxFileAssoc = new System.Windows.Forms.CheckBox();
            this.checkBoxBgSoundPlayer = new System.Windows.Forms.CheckBox();
            this.checkBoxPatchImageres = new System.Windows.Forms.CheckBox();
            this.groupBoxImport = new System.Windows.Forms.GroupBox();
            this.buttonImport = new System.Windows.Forms.Button();
            this.comboBoxSystemSchemes = new System.Windows.Forms.ComboBox();
            this.tabPageAbout = new System.Windows.Forms.TabPage();
            this.programLogo = new System.Windows.Forms.PictureBox();
            this.buttonDownloadSchemes = new System.Windows.Forms.Button();
            this.buttonWebsite = new System.Windows.Forms.Button();
            this.buttonHelp = new System.Windows.Forms.Button();
            this.labelProgramDescription = new System.Windows.Forms.Label();
            this.labelTranslationAuthor = new System.Windows.Forms.Label();
            this.labelProgramVersionAuthor = new System.Windows.Forms.Label();
            this.labelProgramName = new System.Windows.Forms.Label();
            this.buttonOpen = new System.Windows.Forms.Button();
            this.buttonExport = new System.Windows.Forms.Button();
            this.buttonExit = new System.Windows.Forms.Button();
            this.buttonReset = new System.Windows.Forms.Button();
            this.toolTipHandler = new System.Windows.Forms.ToolTip(this.components);
            this.buttonConfigFile = new System.Windows.Forms.Button();
            this.soundContextMenu.SuspendLayout();
            this.imageContextMenu.SuspendLayout();
            this.mainTabs.SuspendLayout();
            this.tabPageScheme.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.soundImage)).BeginInit();
            this.tabPageSettings.SuspendLayout();
            this.groupBoxSystemInfo.SuspendLayout();
            this.groupBoxMaintenance.SuspendLayout();
            this.groupBoxSystemIntegration.SuspendLayout();
            this.groupBoxImport.SuspendLayout();
            this.tabPageAbout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.programLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // soundContextMenu
            // 
            this.soundContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.soundContextMenu_Play,
            this.soundContextMenu_Change,
            this.soundContextMenu_Reset,
            this.soundContextMenu_Remove,
            this.soundContextMenu_Disable,
            this.soundContextMenu_OpenLocation});
            this.soundContextMenu.Name = "soundContextMenu";
            this.soundContextMenu.Size = new System.Drawing.Size(205, 136);
            // 
            // soundContextMenu_Play
            // 
            this.soundContextMenu_Play.Name = "soundContextMenu_Play";
            this.soundContextMenu_Play.Size = new System.Drawing.Size(204, 22);
            this.soundContextMenu_Play.Text = "PLAY_SOUND";
            this.soundContextMenu_Play.Click += new System.EventHandler(this.soundContextMenu_Play_Click);
            // 
            // soundContextMenu_Change
            // 
            this.soundContextMenu_Change.Name = "soundContextMenu_Change";
            this.soundContextMenu_Change.Size = new System.Drawing.Size(204, 22);
            this.soundContextMenu_Change.Text = "CHANGE_SOUND";
            this.soundContextMenu_Change.Click += new System.EventHandler(this.soundContextMenu_Change_Click);
            // 
            // soundContextMenu_OpenLocation
            // 
            this.soundContextMenu_OpenLocation.Name = "soundContextMenu_OpenLocation";
            this.soundContextMenu_OpenLocation.Size = new System.Drawing.Size(204, 22);
            this.soundContextMenu_OpenLocation.Text = "OPEN_FILE_LOCATION";
            this.soundContextMenu_OpenLocation.Click += new System.EventHandler(this.soundContextMenu_OpenLocation_Click);
            // 
            // soundContextMenu_Reset
            // 
            this.soundContextMenu_Reset.Name = "soundContextMenu_Reset";
            this.soundContextMenu_Reset.Size = new System.Drawing.Size(204, 22);
            this.soundContextMenu_Reset.Text = "LOAD_DEFAULT_SOUND";
            this.soundContextMenu_Reset.Click += new System.EventHandler(this.soundContextMenu_Reset_Click);
            // 
            // soundContextMenu_Remove
            // 
            this.soundContextMenu_Remove.Name = "soundContextMenu_Remove";
            this.soundContextMenu_Remove.Size = new System.Drawing.Size(204, 22);
            this.soundContextMenu_Remove.Text = "REMOVE_SOUND";
            this.soundContextMenu_Remove.Click += new System.EventHandler(this.soundContextMenu_Remove_Click);
            //
            // soundContextMenu_Disable
            //
            this.soundContextMenu_Disable.Name = "soundContextMenu_Disable";
            this.soundContextMenu_Disable.Size = new System.Drawing.Size(204, 22);
            this.soundContextMenu_Disable.Text = "DISABLE_SOUND";
            this.soundContextMenu_Disable.Click += new System.EventHandler(this.soundContextMenu_Disable_Click);
            // 
            // imageContextMenu
            // 
            this.imageContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.imageContextMenu_Change,
            this.imageContextMenu_Remove});
            this.imageContextMenu.Name = "imageContextMenu";
            this.imageContextMenu.Size = new System.Drawing.Size(164, 48);
            // 
            // imageContextMenu_Change
            // 
            this.imageContextMenu_Change.Name = "imageContextMenu_Change";
            this.imageContextMenu_Change.Size = new System.Drawing.Size(163, 22);
            this.imageContextMenu_Change.Text = "CHANGE_IMAGE";
            this.imageContextMenu_Change.Click += new System.EventHandler(this.imageContextMenu_Change_Click);
            // 
            // imageContextMenu_Remove
            // 
            this.imageContextMenu_Remove.Name = "imageContextMenu_Remove";
            this.imageContextMenu_Remove.Size = new System.Drawing.Size(163, 22);
            this.imageContextMenu_Remove.Text = "REMOVE_IMAGE";
            this.imageContextMenu_Remove.Click += new System.EventHandler(this.imageContextMenu_Remove_Click);
            // 
            // mainTabs
            // 
            this.mainTabs.Controls.Add(this.tabPageScheme);
            this.mainTabs.Controls.Add(this.tabPageSettings);
            this.mainTabs.Controls.Add(this.tabPageAbout);
            this.mainTabs.Location = new System.Drawing.Point(5, 7);
            this.mainTabs.Name = "mainTabs";
            this.mainTabs.SelectedIndex = 0;
            this.mainTabs.Size = new System.Drawing.Size(415, 436);
            this.mainTabs.TabIndex = 0;
            this.mainTabs.SelectedIndexChanged += new System.EventHandler(this.mainTabs_SelectedIndexChanged);
            // 
            // tabPageScheme
            // 
            this.tabPageScheme.Controls.Add(this.soundInfoNameBox);
            this.tabPageScheme.Controls.Add(this.soundInfoNameText);
            this.tabPageScheme.Controls.Add(this.soundInfoAuthorBox);
            this.tabPageScheme.Controls.Add(this.soundInfoAuthorText);
            this.tabPageScheme.Controls.Add(this.soundInfoAboutBox);
            this.tabPageScheme.Controls.Add(this.soundInfoAboutText);
            this.tabPageScheme.Controls.Add(this.soundImageText);
            this.tabPageScheme.Controls.Add(this.soundImage);
            this.tabPageScheme.Controls.Add(this.soundList);
            this.tabPageScheme.Location = new System.Drawing.Point(4, 22);
            this.tabPageScheme.Name = "tabPageScheme";
            this.tabPageScheme.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageScheme.Size = new System.Drawing.Size(407, 410);
            this.tabPageScheme.TabIndex = 0;
            this.tabPageScheme.Text = "TAB_CURRENT_SCHEME";
            this.tabPageScheme.UseVisualStyleBackColor = true;
            // 
            // soundInfoNameBox
            // 
            this.soundInfoNameBox.Location = new System.Drawing.Point(173, 37);
            this.soundInfoNameBox.Name = "soundInfoNameBox";
            this.soundInfoNameBox.Size = new System.Drawing.Size(228, 20);
            this.soundInfoNameBox.TabIndex = 2;
            this.soundInfoNameBox.LostFocus += new System.EventHandler(this.soundInfo_Box_LostFocus);
            // 
            // soundInfoNameText
            // 
            this.soundInfoNameText.BackColor = System.Drawing.Color.LightGray;
            this.soundInfoNameText.Location = new System.Drawing.Point(118, 37);
            this.soundInfoNameText.Name = "soundInfoNameText";
            this.soundInfoNameText.Size = new System.Drawing.Size(52, 20);
            this.soundInfoNameText.TabIndex = 1;
            this.soundInfoNameText.Text = "META_NAME";
            this.soundInfoNameText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // soundInfoAuthorBox
            // 
            this.soundInfoAuthorBox.Location = new System.Drawing.Point(173, 63);
            this.soundInfoAuthorBox.Name = "soundInfoAuthorBox";
            this.soundInfoAuthorBox.Size = new System.Drawing.Size(228, 20);
            this.soundInfoAuthorBox.TabIndex = 4;
            this.soundInfoAuthorBox.LostFocus += new System.EventHandler(this.soundInfo_Box_LostFocus);
            // 
            // soundInfoAuthorText
            // 
            this.soundInfoAuthorText.BackColor = System.Drawing.Color.LightGray;
            this.soundInfoAuthorText.Location = new System.Drawing.Point(118, 63);
            this.soundInfoAuthorText.Name = "soundInfoAuthorText";
            this.soundInfoAuthorText.Size = new System.Drawing.Size(52, 20);
            this.soundInfoAuthorText.TabIndex = 3;
            this.soundInfoAuthorText.Text = "META_AUTHOR";
            this.soundInfoAuthorText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // soundInfoAboutBox
            // 
            this.soundInfoAboutBox.Location = new System.Drawing.Point(173, 89);
            this.soundInfoAboutBox.Name = "soundInfoAboutBox";
            this.soundInfoAboutBox.Size = new System.Drawing.Size(228, 20);
            this.soundInfoAboutBox.TabIndex = 6;
            this.soundInfoAboutBox.LostFocus += new System.EventHandler(this.soundInfo_Box_LostFocus);
            this.soundInfoAboutBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.soundInfoAboutBox_DoubleClick);
            // 
            // soundInfoAboutText
            // 
            this.soundInfoAboutText.BackColor = System.Drawing.Color.LightGray;
            this.soundInfoAboutText.Location = new System.Drawing.Point(118, 89);
            this.soundInfoAboutText.Name = "soundInfoAboutText";
            this.soundInfoAboutText.Size = new System.Drawing.Size(52, 20);
            this.soundInfoAboutText.TabIndex = 5;
            this.soundInfoAboutText.Text = "META_ABOUT";
            this.soundInfoAboutText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // soundImageText
            // 
            this.soundImageText.BackColor = System.Drawing.Color.LightGray;
            this.soundImageText.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.soundImageText.Location = new System.Drawing.Point(10, 11);
            this.soundImageText.Name = "soundImageText";
            this.soundImageText.Size = new System.Drawing.Size(98, 98);
            this.soundImageText.TabIndex = 0;
            this.soundImageText.Text = "UI_NO_IMAGE";
            this.soundImageText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.soundImageText.MouseClick += new System.Windows.Forms.MouseEventHandler(this.soundImage_Click);
            // 
            // soundImage
            // 
            this.soundImage.BackColor = System.Drawing.SystemColors.ControlDark;
            this.soundImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.soundImage.Location = new System.Drawing.Point(9, 10);
            this.soundImage.Name = "soundImage";
            this.soundImage.Size = new System.Drawing.Size(100, 100);
            this.soundImage.TabIndex = 10;
            this.soundImage.TabStop = false;
            this.soundImage.MouseClick += new System.Windows.Forms.MouseEventHandler(this.soundImage_Click);
            // 
            // soundList
            // 
            this.soundList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.soundList.Location = new System.Drawing.Point(11, 119);
            this.soundList.Name = "soundList";
            this.soundList.Size = new System.Drawing.Size(401, 297);
            this.soundList.TabIndex = 7;
            this.soundList.UseCompatibleStateImageBehavior = false;
            this.soundList.SelectedIndexChanged += new System.EventHandler(this.soundList_SelectedIndexChanged);
            this.soundList.MouseClick += new System.Windows.Forms.MouseEventHandler(this.soundList_MouseClick);
            this.soundList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.soundList_MouseDoubleClick);
            // 
            // tabPageSettings
            // 
            this.tabPageSettings.Controls.Add(this.groupBoxSystemInfo);
            this.tabPageSettings.Controls.Add(this.groupBoxMaintenance);
            this.tabPageSettings.Controls.Add(this.groupBoxSystemIntegration);
            this.tabPageSettings.Controls.Add(this.groupBoxImport);
            this.tabPageSettings.Location = new System.Drawing.Point(4, 22);
            this.tabPageSettings.Name = "tabPageSettings";
            this.tabPageSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSettings.Size = new System.Drawing.Size(407, 410);
            this.tabPageSettings.TabIndex = 1;
            this.tabPageSettings.Text = "UI_SETTINGS";
            this.tabPageSettings.UseVisualStyleBackColor = true;
            // 
            // groupBoxSystemInfo
            // 
            this.groupBoxSystemInfo.Controls.Add(this.labelSystemSupportStatus);
            this.groupBoxSystemInfo.Controls.Add(this.labelSystemInfo);
            this.groupBoxSystemInfo.Location = new System.Drawing.Point(7, 6);
            this.groupBoxSystemInfo.Name = "groupBoxSystemInfo";
            this.groupBoxSystemInfo.Size = new System.Drawing.Size(394, 88);
            this.groupBoxSystemInfo.TabIndex = 0;
            this.groupBoxSystemInfo.TabStop = false;
            this.groupBoxSystemInfo.Text = "BOX_SYSYEM_INFO";
            // 
            // labelSystemSupportStatus
            // 
            this.labelSystemSupportStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSystemSupportStatus.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.labelSystemSupportStatus.Location = new System.Drawing.Point(19, 42);
            this.labelSystemSupportStatus.Name = "labelSystemSupportStatus";
            this.labelSystemSupportStatus.Size = new System.Drawing.Size(359, 38);
            this.labelSystemSupportStatus.TabIndex = 1;
            this.labelSystemSupportStatus.Text = "LABEL_SYSTEM_SUPPORT_STATUS";
            this.labelSystemSupportStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelSystemInfo
            // 
            this.labelSystemInfo.Location = new System.Drawing.Point(19, 21);
            this.labelSystemInfo.Name = "labelSystemInfo";
            this.labelSystemInfo.Size = new System.Drawing.Size(359, 22);
            this.labelSystemInfo.TabIndex = 0;
            this.labelSystemInfo.Text = "LABEL_SYSTEM_INFO";
            this.labelSystemInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBoxMaintenance
            // 
            this.groupBoxMaintenance.Controls.Add(this.buttonConfigFile);
            this.groupBoxMaintenance.Controls.Add(this.buttonUninstall);
            this.groupBoxMaintenance.Controls.Add(this.buttonReinstall);
            this.groupBoxMaintenance.Location = new System.Drawing.Point(6, 327);
            this.groupBoxMaintenance.Name = "groupBoxMaintenance";
            this.groupBoxMaintenance.Size = new System.Drawing.Size(394, 73);
            this.groupBoxMaintenance.TabIndex = 3;
            this.groupBoxMaintenance.TabStop = false;
            this.groupBoxMaintenance.Text = "BOX_MAINTENANCE";
            // 
            // buttonUninstall
            // 
            this.buttonUninstall.Location = new System.Drawing.Point(105, 28);
            this.buttonUninstall.Name = "buttonUninstall";
            this.buttonUninstall.Size = new System.Drawing.Size(80, 23);
            this.buttonUninstall.TabIndex = 1;
            this.buttonUninstall.Text = "BUTTON_UNINSTALL";
            this.buttonUninstall.UseVisualStyleBackColor = true;
            this.buttonUninstall.Click += new System.EventHandler(this.buttonUninstall_Click);
            // 
            // buttonReinstall
            // 
            this.buttonReinstall.Location = new System.Drawing.Point(19, 28);
            this.buttonReinstall.Name = "buttonReinstall";
            this.buttonReinstall.Size = new System.Drawing.Size(80, 23);
            this.buttonReinstall.TabIndex = 0;
            this.buttonReinstall.Text = "BUTTON_REINSTALL";
            this.buttonReinstall.UseVisualStyleBackColor = true;
            this.buttonReinstall.Click += new System.EventHandler(this.buttonReinstall_Click);
            // 
            // groupBoxSystemIntegration
            // 
            this.groupBoxSystemIntegration.Controls.Add(this.checkBoxMissingSoundsUseDefault);
            this.groupBoxSystemIntegration.Controls.Add(this.checkBoxFileAssoc);
            this.groupBoxSystemIntegration.Controls.Add(this.checkBoxBgSoundPlayer);
            this.groupBoxSystemIntegration.Controls.Add(this.checkBoxPatchImageres);
            this.groupBoxSystemIntegration.Location = new System.Drawing.Point(6, 188);
            this.groupBoxSystemIntegration.Name = "groupBoxSystemIntegration";
            this.groupBoxSystemIntegration.Size = new System.Drawing.Size(394, 133);
            this.groupBoxSystemIntegration.TabIndex = 2;
            this.groupBoxSystemIntegration.TabStop = false;
            this.groupBoxSystemIntegration.Text = "BOX_SYSTEM_INTEGRATION";
            // 
            // checkBoxMissingSoundsUseDefault
            // 
            this.checkBoxMissingSoundsUseDefault.AutoSize = true;
            this.checkBoxMissingSoundsUseDefault.Location = new System.Drawing.Point(19, 98);
            this.checkBoxMissingSoundsUseDefault.Name = "checkBoxMissingSoundsUseDefault";
            this.checkBoxMissingSoundsUseDefault.Size = new System.Drawing.Size(276, 17);
            this.checkBoxMissingSoundsUseDefault.TabIndex = 3;
            this.checkBoxMissingSoundsUseDefault.Text = "CHECK_BOX_MISSING_SOUNDS_USE_DEFAULT";
            this.checkBoxMissingSoundsUseDefault.UseVisualStyleBackColor = true;
            // 
            // checkBoxFileAssoc
            // 
            this.checkBoxFileAssoc.AutoSize = true;
            this.checkBoxFileAssoc.Location = new System.Drawing.Point(19, 28);
            this.checkBoxFileAssoc.Name = "checkBoxFileAssoc";
            this.checkBoxFileAssoc.Size = new System.Drawing.Size(160, 17);
            this.checkBoxFileAssoc.TabIndex = 0;
            this.checkBoxFileAssoc.Text = "CHECK_BOX_FILE_ASSOC";
            this.checkBoxFileAssoc.UseVisualStyleBackColor = true;
            // 
            // checkBoxBgSoundPlayer
            // 
            this.checkBoxBgSoundPlayer.AutoSize = true;
            this.checkBoxBgSoundPlayer.Location = new System.Drawing.Point(19, 74);
            this.checkBoxBgSoundPlayer.Name = "checkBoxBgSoundPlayer";
            this.checkBoxBgSoundPlayer.Size = new System.Drawing.Size(204, 17);
            this.checkBoxBgSoundPlayer.TabIndex = 2;
            this.checkBoxBgSoundPlayer.Text = "CHECK_BOX_BG_SOUND_PLAYER";
            this.checkBoxBgSoundPlayer.UseVisualStyleBackColor = true;
            // 
            // checkBoxPatchImageres
            // 
            this.checkBoxPatchImageres.AutoSize = true;
            this.checkBoxPatchImageres.Location = new System.Drawing.Point(19, 51);
            this.checkBoxPatchImageres.Name = "checkBoxPatchImageres";
            this.checkBoxPatchImageres.Size = new System.Drawing.Size(194, 17);
            this.checkBoxPatchImageres.TabIndex = 1;
            this.checkBoxPatchImageres.Text = "CHECK_BOX_IMAGERES_PATCH";
            this.checkBoxPatchImageres.UseVisualStyleBackColor = true;
            // 
            // groupBoxImport
            // 
            this.groupBoxImport.Controls.Add(this.buttonImport);
            this.groupBoxImport.Controls.Add(this.comboBoxSystemSchemes);
            this.groupBoxImport.Location = new System.Drawing.Point(6, 100);
            this.groupBoxImport.Name = "groupBoxImport";
            this.groupBoxImport.Size = new System.Drawing.Size(394, 82);
            this.groupBoxImport.TabIndex = 1;
            this.groupBoxImport.TabStop = false;
            this.groupBoxImport.Text = "BOX_IMPORT_SYSTEM_SCHEME";
            // 
            // buttonImport
            // 
            this.buttonImport.Location = new System.Drawing.Point(298, 33);
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.Size = new System.Drawing.Size(80, 23);
            this.buttonImport.TabIndex = 1;
            this.buttonImport.Text = "BUTTON_IMPORT_SYSTEM_SCHEME";
            this.buttonImport.UseVisualStyleBackColor = true;
            this.buttonImport.Click += new System.EventHandler(this.buttonImportSystemScheme_Click);
            // 
            // comboBoxSystemSchemes
            // 
            this.comboBoxSystemSchemes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSystemSchemes.FormattingEnabled = true;
            this.comboBoxSystemSchemes.Location = new System.Drawing.Point(19, 34);
            this.comboBoxSystemSchemes.Name = "comboBoxSystemSchemes";
            this.comboBoxSystemSchemes.Size = new System.Drawing.Size(267, 21);
            this.comboBoxSystemSchemes.TabIndex = 0;
            // 
            // tabPageAbout
            // 
            this.tabPageAbout.Controls.Add(this.programLogo);
            this.tabPageAbout.Controls.Add(this.buttonDownloadSchemes);
            this.tabPageAbout.Controls.Add(this.buttonWebsite);
            this.tabPageAbout.Controls.Add(this.buttonHelp);
            this.tabPageAbout.Controls.Add(this.labelProgramDescription);
            this.tabPageAbout.Controls.Add(this.labelTranslationAuthor);
            this.tabPageAbout.Controls.Add(this.labelProgramVersionAuthor);
            this.tabPageAbout.Controls.Add(this.labelProgramName);
            this.tabPageAbout.Location = new System.Drawing.Point(4, 22);
            this.tabPageAbout.Name = "tabPageAbout";
            this.tabPageAbout.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAbout.Size = new System.Drawing.Size(407, 410);
            this.tabPageAbout.TabIndex = 2;
            this.tabPageAbout.Text = "UI_ABOUT";
            this.tabPageAbout.UseVisualStyleBackColor = true;
            // 
            // programLogo
            // 
            this.programLogo.BackgroundImage = global::SoundManager.SoundIcons.SoundManagerLogo;
            this.programLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.programLogo.Location = new System.Drawing.Point(6, 17);
            this.programLogo.Name = "programLogo";
            this.programLogo.Size = new System.Drawing.Size(395, 128);
            this.programLogo.TabIndex = 7;
            this.programLogo.TabStop = false;
            // 
            // buttonDownloadSchemes
            // 
            this.buttonDownloadSchemes.Location = new System.Drawing.Point(115, 346);
            this.buttonDownloadSchemes.Name = "buttonDownloadSchemes";
            this.buttonDownloadSchemes.Size = new System.Drawing.Size(180, 23);
            this.buttonDownloadSchemes.TabIndex = 6;
            this.buttonDownloadSchemes.Text = "BUTTON_DOWNLOAD_SCHEMES";
            this.buttonDownloadSchemes.UseVisualStyleBackColor = true;
            this.buttonDownloadSchemes.Click += new System.EventHandler(this.buttonDownloadSchemes_Click);
            // 
            // buttonWebsite
            // 
            this.buttonWebsite.Location = new System.Drawing.Point(221, 304);
            this.buttonWebsite.Name = "buttonWebsite";
            this.buttonWebsite.Size = new System.Drawing.Size(100, 23);
            this.buttonWebsite.TabIndex = 5;
            this.buttonWebsite.Text = "BUTTON_WEBSITE";
            this.buttonWebsite.UseVisualStyleBackColor = true;
            this.buttonWebsite.Click += new System.EventHandler(this.buttonWebsite_Click);
            // 
            // buttonHelp
            // 
            this.buttonHelp.Location = new System.Drawing.Point(89, 304);
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Size = new System.Drawing.Size(100, 23);
            this.buttonHelp.TabIndex = 4;
            this.buttonHelp.Text = "BUTTON_HELP";
            this.buttonHelp.UseVisualStyleBackColor = true;
            this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
            // 
            // labelProgramDescription
            // 
            this.labelProgramDescription.Location = new System.Drawing.Point(6, 216);
            this.labelProgramDescription.Name = "labelProgramDescription";
            this.labelProgramDescription.Size = new System.Drawing.Size(395, 71);
            this.labelProgramDescription.TabIndex = 3;
            this.labelProgramDescription.Text = "APP DESCRIPTION";
            this.labelProgramDescription.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelTranslationAuthor
            // 
            this.labelTranslationAuthor.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.labelTranslationAuthor.Location = new System.Drawing.Point(6, 196);
            this.labelTranslationAuthor.Name = "labelTranslationAuthor";
            this.labelTranslationAuthor.Size = new System.Drawing.Size(395, 20);
            this.labelTranslationAuthor.TabIndex = 2;
            this.labelTranslationAuthor.Text = "Translated by AUTHOR";
            this.labelTranslationAuthor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelProgramVersionAuthor
            // 
            this.labelProgramVersionAuthor.Location = new System.Drawing.Point(6, 178);
            this.labelProgramVersionAuthor.Name = "labelProgramVersionAuthor";
            this.labelProgramVersionAuthor.Size = new System.Drawing.Size(395, 20);
            this.labelProgramVersionAuthor.TabIndex = 1;
            this.labelProgramVersionAuthor.Text = "Version XXX - by AUTHOR";
            this.labelProgramVersionAuthor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelProgramName
            // 
            this.labelProgramName.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelProgramName.Location = new System.Drawing.Point(6, 150);
            this.labelProgramName.Name = "labelProgramName";
            this.labelProgramName.Size = new System.Drawing.Size(395, 30);
            this.labelProgramName.TabIndex = 0;
            this.labelProgramName.Text = "PROGRAM_NAME";
            this.labelProgramName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonOpen
            // 
            this.buttonOpen.Location = new System.Drawing.Point(9, 451);
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(80, 23);
            this.buttonOpen.TabIndex = 1;
            this.buttonOpen.Text = "BUTTON_IMPORT";
            this.buttonOpen.UseVisualStyleBackColor = true;
            this.buttonOpen.Click += new System.EventHandler(this.buttonImport_Click);
            // 
            // buttonExport
            // 
            this.buttonExport.Location = new System.Drawing.Point(95, 451);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(80, 23);
            this.buttonExport.TabIndex = 2;
            this.buttonExport.Text = "BUTTON_EXPORT";
            this.buttonExport.UseVisualStyleBackColor = true;
            this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // buttonExit
            // 
            this.buttonExit.Location = new System.Drawing.Point(336, 451);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(80, 23);
            this.buttonExit.TabIndex = 4;
            this.buttonExit.Text = "BUTTON_EXIT";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // buttonReset
            // 
            this.buttonReset.Location = new System.Drawing.Point(181, 451);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(80, 23);
            this.buttonReset.TabIndex = 3;
            this.buttonReset.Text = "BUTTON_RESET";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
            // 
            // buttonConfigFile
            // 
            this.buttonConfigFile.Location = new System.Drawing.Point(191, 28);
            this.buttonConfigFile.Name = "buttonConfigFile";
            this.buttonConfigFile.Size = new System.Drawing.Size(100, 23);
            this.buttonConfigFile.TabIndex = 2;
            this.buttonConfigFile.Text = "BUTTON_CONFIG_FILE";
            this.buttonConfigFile.UseVisualStyleBackColor = true;
            this.buttonConfigFile.Click += new System.EventHandler(this.buttonConfigFile_Click);
            // 
            // FormMain
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 482);
            this.Controls.Add(this.buttonReset);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.buttonExport);
            this.Controls.Add(this.buttonOpen);
            this.Controls.Add(this.mainTabs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "APP_NAME";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.window_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.window_DragEnter);
            this.soundContextMenu.ResumeLayout(false);
            this.imageContextMenu.ResumeLayout(false);
            this.mainTabs.ResumeLayout(false);
            this.tabPageScheme.ResumeLayout(false);
            this.tabPageScheme.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.soundImage)).EndInit();
            this.tabPageSettings.ResumeLayout(false);
            this.groupBoxSystemInfo.ResumeLayout(false);
            this.groupBoxMaintenance.ResumeLayout(false);
            this.groupBoxSystemIntegration.ResumeLayout(false);
            this.groupBoxSystemIntegration.PerformLayout();
            this.groupBoxImport.ResumeLayout(false);
            this.tabPageAbout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.programLogo)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.ContextMenuStrip soundContextMenu;
        private System.Windows.Forms.ToolStripMenuItem soundContextMenu_Change;
        private System.Windows.Forms.ToolStripMenuItem soundContextMenu_Play;
        private System.Windows.Forms.ToolStripMenuItem soundContextMenu_Reset;
        private System.Windows.Forms.ToolStripMenuItem soundContextMenu_Remove;
        private System.Windows.Forms.ToolStripMenuItem soundContextMenu_Disable;
        private System.Windows.Forms.ToolStripMenuItem soundContextMenu_OpenLocation;
        private System.Windows.Forms.ContextMenuStrip imageContextMenu;
        private System.Windows.Forms.ToolStripMenuItem imageContextMenu_Change;
        private System.Windows.Forms.ToolStripMenuItem imageContextMenu_Remove;
        private System.Windows.Forms.TabControl mainTabs;
        private System.Windows.Forms.TabPage tabPageScheme;
        private System.Windows.Forms.TextBox soundInfoNameBox;
        private System.Windows.Forms.Label soundInfoNameText;
        private System.Windows.Forms.TextBox soundInfoAuthorBox;
        private System.Windows.Forms.Label soundInfoAuthorText;
        private System.Windows.Forms.TextBox soundInfoAboutBox;
        private System.Windows.Forms.Label soundInfoAboutText;
        private System.Windows.Forms.Label soundImageText;
        private System.Windows.Forms.PictureBox soundImage;
        private System.Windows.Forms.ListView soundList;
        private System.Windows.Forms.TabPage tabPageSettings;
        private System.Windows.Forms.TabPage tabPageAbout;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.Button buttonOpen;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.GroupBox groupBoxImport;
        private System.Windows.Forms.Button buttonImport;
        private System.Windows.Forms.ComboBox comboBoxSystemSchemes;
        private System.Windows.Forms.GroupBox groupBoxSystemIntegration;
        private System.Windows.Forms.CheckBox checkBoxPatchImageres;
        private System.Windows.Forms.CheckBox checkBoxBgSoundPlayer;
        private System.Windows.Forms.CheckBox checkBoxFileAssoc;
        private System.Windows.Forms.GroupBox groupBoxMaintenance;
        private System.Windows.Forms.Button buttonReinstall;
        private System.Windows.Forms.Button buttonUninstall;
        private System.Windows.Forms.Label labelProgramName;
        private System.Windows.Forms.Label labelProgramVersionAuthor;
        private System.Windows.Forms.Label labelTranslationAuthor;
        private System.Windows.Forms.Label labelProgramDescription;
        private System.Windows.Forms.Button buttonWebsite;
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.GroupBox groupBoxSystemInfo;
        private System.Windows.Forms.Label labelSystemInfo;
        private System.Windows.Forms.Label labelSystemSupportStatus;
        private System.Windows.Forms.CheckBox checkBoxMissingSoundsUseDefault;
        private System.Windows.Forms.Button buttonDownloadSchemes;
        private System.Windows.Forms.PictureBox programLogo;
        private System.Windows.Forms.ToolTip toolTipHandler;
        private System.Windows.Forms.Button buttonConfigFile;
    }
}

