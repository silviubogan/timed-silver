namespace cs_timed_silver
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblFirstStart = new cs_timed_silver.EnhancedLabel();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.cbDarkMode = new cs_timed_silver.EnhancedCheckBox();
            this.cbFullScreen = new cs_timed_silver.EnhancedCheckBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmiFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiNew = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpenFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSave = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tss1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiOpenContainingFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRestoreToPreviousDay = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRecentFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAdvanced = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiClearOpenFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiReloadOpenFileFromFileSystem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSelectiveOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.tss2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRemoveAllTimers = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMultipleSelection = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiView = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiListView = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDataGridView = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiHorizontalSplitView = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiVerticalSplitView = new System.Windows.Forms.ToolStripMenuItem();
            this.tss3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiViewGroupListView = new System.Windows.Forms.ToolStripMenuItem();
            this.tss4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiFocusListView = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFocusDataGridView = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFocusGroupListView = new System.Windows.Forms.ToolStripMenuItem();
            this.tss5 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiToolbars = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiListViewZoomToolbar = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDataGridZoomToolbar = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiGroupListZoom = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiGlobalZoom = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiStatusBar = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiUseAdvancedViews = new System.Windows.Forms.ToolStripMenuItem();
            this.tss6 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiSplitViewHorizontally = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSplitViewVertically = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRemoveSplit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiReplaceViewWith = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiReplaceWithListView = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiReplaceWithDataGrid = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiReplaceWithGroupList = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFocusedSplitViewOrientation = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiHorizontalOrientation = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiVerticalOrientation = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiKeepOnTop = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiKeepOnTopAlways = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiKeepOnTopUntilNextRestart = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiKeepOnTopFor5Minutes = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiKeepOnTopFor15Minutes = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiKeepOnTopFor1Hour = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiViewHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tsDataGridZoom = new cs_timed_silver.ZoomToolStrip();
            this.tsListViewZoom = new cs_timed_silver.ZoomToolStrip();
            this.tsGroupListZoom = new cs_timed_silver.ZoomToolStrip();
            this.tsmiSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsSort = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.tscbAutomaticSort = new System.Windows.Forms.ToolStripComboBox();
            this.tsGlobalZoom = new cs_timed_silver.ZoomToolStrip();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.viewsGrid1 = new cs_timed_silver.ViewsGrid();
            this.tableLayoutPanel1.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.tsSort.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.lblFirstStart, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.elementHost1, 0, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(7, 25);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(622, 390);
            this.tableLayoutPanel1.TabIndex = 11;
            // 
            // lblFirstStart
            // 
            this.lblFirstStart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFirstStart.BackColor = System.Drawing.Color.Yellow;
            this.lblFirstStart.Location = new System.Drawing.Point(3, 0);
            this.lblFirstStart.Name = "lblFirstStart";
            this.lblFirstStart.Size = new System.Drawing.Size(619, 23);
            this.lblFirstStart.TabIndex = 11;
            this.lblFirstStart.Text = "Please use the menu File > Save as... to create a data file and start using the p" +
    "rogram.";
            this.lblFirstStart.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.AutoScroll = true;
            this.toolStripContainer1.ContentPanel.Controls.Add(this.cbDarkMode);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.tableLayoutPanel1);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.cbFullScreen);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(638, 424);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(638, 475);
            this.toolStripContainer1.TabIndex = 13;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menuStrip1);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tsDataGridZoom);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tsListViewZoom);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tsGroupListZoom);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tsSort);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tsGlobalZoom);
            this.toolStripContainer1.TopToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            // 
            // cbDarkMode
            // 
            this.cbDarkMode.AutoSize = true;
            this.cbDarkMode.Location = new System.Drawing.Point(100, 3);
            this.cbDarkMode.Name = "cbDarkMode";
            this.cbDarkMode.Padding = new System.Windows.Forms.Padding(1);
            this.cbDarkMode.Size = new System.Drawing.Size(80, 19);
            this.cbDarkMode.TabIndex = 12;
            this.cbDarkMode.Text = "&Dark mode";
            this.cbDarkMode.UseVisualStyleBackColor = true;
            this.cbDarkMode.CheckedChanged += new System.EventHandler(this.cbDarkMode_CheckedChanged);
            // 
            // cbFullScreen
            // 
            this.cbFullScreen.AutoSize = true;
            this.cbFullScreen.Location = new System.Drawing.Point(7, 3);
            this.cbFullScreen.Name = "cbFullScreen";
            this.cbFullScreen.Padding = new System.Windows.Forms.Padding(1);
            this.cbFullScreen.Size = new System.Drawing.Size(79, 19);
            this.cbFullScreen.TabIndex = 1;
            this.cbFullScreen.Text = "F&ull-screen";
            this.cbFullScreen.UseVisualStyleBackColor = true;
            this.cbFullScreen.CheckedChanged += new System.EventHandler(this.cbFullScreen_CheckedChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiFile,
            this.tsmiEdit,
            this.tsmiView,
            this.tsmiWindow,
            this.tsmiHelp});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(638, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsmiFile
            // 
            this.tsmiFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiNew,
            this.tsmiOpenFile,
            this.tsmiSave,
            this.tsmiSaveAs,
            this.tsmiSettings,
            this.tss1,
            this.tsmiOpenContainingFolder,
            this.tsmiRestoreToPreviousDay,
            this.tsmiRecentFiles,
            this.tsmiAdvanced,
            this.tss2,
            this.tsmiExit});
            this.tsmiFile.Name = "tsmiFile";
            this.tsmiFile.Size = new System.Drawing.Size(37, 20);
            this.tsmiFile.Text = "&File";
            this.tsmiFile.DropDownOpening += new System.EventHandler(this.tsmiFile_DropDownOpening);
            // 
            // tsmiNew
            // 
            this.tsmiNew.Enabled = false;
            this.tsmiNew.Image = ((System.Drawing.Image)(resources.GetObject("tsmiNew.Image")));
            this.tsmiNew.Name = "tsmiNew";
            this.tsmiNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.tsmiNew.Size = new System.Drawing.Size(285, 22);
            this.tsmiNew.Text = "New";
            this.tsmiNew.Click += new System.EventHandler(this.tsmiClose_Click);
            // 
            // tsmiOpenFile
            // 
            this.tsmiOpenFile.Image = ((System.Drawing.Image)(resources.GetObject("tsmiOpenFile.Image")));
            this.tsmiOpenFile.Name = "tsmiOpenFile";
            this.tsmiOpenFile.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.tsmiOpenFile.Size = new System.Drawing.Size(285, 22);
            this.tsmiOpenFile.Text = "Open...";
            this.tsmiOpenFile.Click += new System.EventHandler(this.tsmiOpenFile_Click);
            // 
            // tsmiSave
            // 
            this.tsmiSave.Image = ((System.Drawing.Image)(resources.GetObject("tsmiSave.Image")));
            this.tsmiSave.Name = "tsmiSave";
            this.tsmiSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.tsmiSave.Size = new System.Drawing.Size(285, 22);
            this.tsmiSave.Text = "Save";
            this.tsmiSave.Click += new System.EventHandler(this.tsmiSave_Click);
            // 
            // tsmiSaveAs
            // 
            this.tsmiSaveAs.Image = ((System.Drawing.Image)(resources.GetObject("tsmiSaveAs.Image")));
            this.tsmiSaveAs.Name = "tsmiSaveAs";
            this.tsmiSaveAs.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.tsmiSaveAs.Size = new System.Drawing.Size(285, 22);
            this.tsmiSaveAs.Text = "Save as...";
            this.tsmiSaveAs.Click += new System.EventHandler(this.tsmiSaveAs_Click);
            // 
            // tsmiSettings
            // 
            this.tsmiSettings.Image = ((System.Drawing.Image)(resources.GetObject("tsmiSettings.Image")));
            this.tsmiSettings.Name = "tsmiSettings";
            this.tsmiSettings.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.tsmiSettings.Size = new System.Drawing.Size(285, 22);
            this.tsmiSettings.Text = "S&ettings...";
            this.tsmiSettings.Click += new System.EventHandler(this.tsmiSettings_Click);
            // 
            // tss1
            // 
            this.tss1.Name = "tss1";
            this.tss1.Size = new System.Drawing.Size(282, 6);
            // 
            // tsmiOpenContainingFolder
            // 
            this.tsmiOpenContainingFolder.Enabled = false;
            this.tsmiOpenContainingFolder.Name = "tsmiOpenContainingFolder";
            this.tsmiOpenContainingFolder.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.O)));
            this.tsmiOpenContainingFolder.Size = new System.Drawing.Size(285, 22);
            this.tsmiOpenContainingFolder.Text = "&Open Containing Folder...";
            this.tsmiOpenContainingFolder.Click += new System.EventHandler(this.tsmiOpenContainingFolder_Click);
            // 
            // tsmiRestoreToPreviousDay
            // 
            this.tsmiRestoreToPreviousDay.Name = "tsmiRestoreToPreviousDay";
            this.tsmiRestoreToPreviousDay.Size = new System.Drawing.Size(285, 22);
            this.tsmiRestoreToPreviousDay.Text = "Restore to &Previous Day...";
            this.tsmiRestoreToPreviousDay.Click += new System.EventHandler(this.tsmiRestoreToPreviousDay_Click);
            // 
            // tsmiRecentFiles
            // 
            this.tsmiRecentFiles.Name = "tsmiRecentFiles";
            this.tsmiRecentFiles.Size = new System.Drawing.Size(285, 22);
            this.tsmiRecentFiles.Text = "&Recent Files...";
            // 
            // tsmiAdvanced
            // 
            this.tsmiAdvanced.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiClearOpenFile,
            this.tsmiReloadOpenFileFromFileSystem,
            this.tsmiSelectiveOpen});
            this.tsmiAdvanced.Name = "tsmiAdvanced";
            this.tsmiAdvanced.Size = new System.Drawing.Size(285, 22);
            this.tsmiAdvanced.Text = "&Advanced...";
            // 
            // tsmiClearOpenFile
            // 
            this.tsmiClearOpenFile.Name = "tsmiClearOpenFile";
            this.tsmiClearOpenFile.Size = new System.Drawing.Size(256, 22);
            this.tsmiClearOpenFile.Text = "&Clear open file...";
            this.tsmiClearOpenFile.Click += new System.EventHandler(this.TsmiClearOpenFile_Click);
            // 
            // tsmiReloadOpenFileFromFileSystem
            // 
            this.tsmiReloadOpenFileFromFileSystem.Name = "tsmiReloadOpenFileFromFileSystem";
            this.tsmiReloadOpenFileFromFileSystem.Size = new System.Drawing.Size(256, 22);
            this.tsmiReloadOpenFileFromFileSystem.Text = "Reload &open file from file system...";
            this.tsmiReloadOpenFileFromFileSystem.Click += new System.EventHandler(this.TsmiReloadOpenFileFromFileSystem_Click);
            // 
            // tsmiSelectiveOpen
            // 
            this.tsmiSelectiveOpen.Name = "tsmiSelectiveOpen";
            this.tsmiSelectiveOpen.Size = new System.Drawing.Size(256, 22);
            this.tsmiSelectiveOpen.Text = "&Import...";
            this.tsmiSelectiveOpen.Click += new System.EventHandler(this.tsmiSelectiveOpen_Click);
            // 
            // tss2
            // 
            this.tss2.Name = "tss2";
            this.tss2.Size = new System.Drawing.Size(282, 6);
            // 
            // tsmiExit
            // 
            this.tsmiExit.Image = ((System.Drawing.Image)(resources.GetObject("tsmiExit.Image")));
            this.tsmiExit.Name = "tsmiExit";
            this.tsmiExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.tsmiExit.Size = new System.Drawing.Size(285, 22);
            this.tsmiExit.Text = "E&xit";
            this.tsmiExit.Click += new System.EventHandler(this.tsmiExit_Click);
            // 
            // tsmiEdit
            // 
            this.tsmiEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiRemoveAllTimers,
            this.tsmiMultipleSelection,
            this.tsmiSelectAll});
            this.tsmiEdit.Name = "tsmiEdit";
            this.tsmiEdit.Size = new System.Drawing.Size(39, 20);
            this.tsmiEdit.Text = "&Edit";
            // 
            // tsmiRemoveAllTimers
            // 
            this.tsmiRemoveAllTimers.Image = ((System.Drawing.Image)(resources.GetObject("tsmiRemoveAllTimers.Image")));
            this.tsmiRemoveAllTimers.Name = "tsmiRemoveAllTimers";
            this.tsmiRemoveAllTimers.Size = new System.Drawing.Size(226, 22);
            this.tsmiRemoveAllTimers.Text = "&Remove all timers...";
            this.tsmiRemoveAllTimers.Click += new System.EventHandler(this.tsmiRemoveAllTimers_Click);
            // 
            // tsmiMultipleSelection
            // 
            this.tsmiMultipleSelection.CheckOnClick = true;
            this.tsmiMultipleSelection.Name = "tsmiMultipleSelection";
            this.tsmiMultipleSelection.Size = new System.Drawing.Size(226, 22);
            this.tsmiMultipleSelection.Text = "&Multiple selection in list view";
            this.tsmiMultipleSelection.CheckedChanged += new System.EventHandler(this.tsmiMultipleSelection_CheckedChanged);
            // 
            // tsmiView
            // 
            this.tsmiView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiListView,
            this.tsmiDataGridView,
            this.tsmiHorizontalSplitView,
            this.tsmiVerticalSplitView,
            this.tss3,
            this.tsmiViewGroupListView,
            this.tss4,
            this.tsmiFocusListView,
            this.tsmiFocusDataGridView,
            this.tsmiFocusGroupListView,
            this.tss5,
            this.tsmiToolbars,
            this.tsmiStatusBar,
            this.tsmiUseAdvancedViews,
            this.tss6,
            this.tsmiSplitViewHorizontally,
            this.tsmiSplitViewVertically,
            this.tsmiRemoveSplit,
            this.tsmiReplaceViewWith,
            this.tsmiFocusedSplitViewOrientation});
            this.tsmiView.Name = "tsmiView";
            this.tsmiView.Size = new System.Drawing.Size(44, 20);
            this.tsmiView.Text = "&View";
            // 
            // tsmiListView
            // 
            this.tsmiListView.CheckOnClick = true;
            this.tsmiListView.Name = "tsmiListView";
            this.tsmiListView.Size = new System.Drawing.Size(240, 22);
            this.tsmiListView.Text = "&List view";
            this.tsmiListView.Click += new System.EventHandler(this.tsmiViewListView_Click);
            // 
            // tsmiDataGridView
            // 
            this.tsmiDataGridView.CheckOnClick = true;
            this.tsmiDataGridView.Name = "tsmiDataGridView";
            this.tsmiDataGridView.Size = new System.Drawing.Size(240, 22);
            this.tsmiDataGridView.Text = "&Data grid view";
            this.tsmiDataGridView.Click += new System.EventHandler(this.tsmiViewDataGridView_Click);
            // 
            // tsmiHorizontalSplitView
            // 
            this.tsmiHorizontalSplitView.CheckOnClick = true;
            this.tsmiHorizontalSplitView.Name = "tsmiHorizontalSplitView";
            this.tsmiHorizontalSplitView.Size = new System.Drawing.Size(240, 22);
            this.tsmiHorizontalSplitView.Text = "&Horizontal split view";
            this.tsmiHorizontalSplitView.Click += new System.EventHandler(this.tsmiHorizontalSplitView_Click);
            // 
            // tsmiVerticalSplitView
            // 
            this.tsmiVerticalSplitView.CheckOnClick = true;
            this.tsmiVerticalSplitView.Name = "tsmiVerticalSplitView";
            this.tsmiVerticalSplitView.Size = new System.Drawing.Size(240, 22);
            this.tsmiVerticalSplitView.Text = "&Vertical split view";
            this.tsmiVerticalSplitView.Click += new System.EventHandler(this.tsmiVerticalSplitView_Click);
            // 
            // tss3
            // 
            this.tss3.Name = "tss3";
            this.tss3.Size = new System.Drawing.Size(237, 6);
            // 
            // tsmiViewGroupListView
            // 
            this.tsmiViewGroupListView.CheckOnClick = true;
            this.tsmiViewGroupListView.Name = "tsmiViewGroupListView";
            this.tsmiViewGroupListView.Size = new System.Drawing.Size(240, 22);
            this.tsmiViewGroupListView.Text = "Show &group list view";
            this.tsmiViewGroupListView.Click += new System.EventHandler(this.tsmiViewGroupListView_Click);
            // 
            // tss4
            // 
            this.tss4.Name = "tss4";
            this.tss4.Size = new System.Drawing.Size(237, 6);
            // 
            // tsmiFocusListView
            // 
            this.tsmiFocusListView.Name = "tsmiFocusListView";
            this.tsmiFocusListView.Size = new System.Drawing.Size(240, 22);
            this.tsmiFocusListView.Text = "&Focus list view";
            this.tsmiFocusListView.Click += new System.EventHandler(this.tsmiFocusListView_Click);
            // 
            // tsmiFocusDataGridView
            // 
            this.tsmiFocusDataGridView.Name = "tsmiFocusDataGridView";
            this.tsmiFocusDataGridView.Size = new System.Drawing.Size(240, 22);
            this.tsmiFocusDataGridView.Text = "F&ocus data grid view";
            this.tsmiFocusDataGridView.Click += new System.EventHandler(this.tsmiFocusDataGridView_Click);
            // 
            // tsmiFocusGroupListView
            // 
            this.tsmiFocusGroupListView.Name = "tsmiFocusGroupListView";
            this.tsmiFocusGroupListView.Size = new System.Drawing.Size(240, 22);
            this.tsmiFocusGroupListView.Text = "Fo&cus group list view";
            this.tsmiFocusGroupListView.Click += new System.EventHandler(this.tsmiFocusGroupListView_Click);
            // 
            // tss5
            // 
            this.tss5.Name = "tss5";
            this.tss5.Size = new System.Drawing.Size(237, 6);
            // 
            // tsmiToolbars
            // 
            this.tsmiToolbars.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiListViewZoomToolbar,
            this.tsmiDataGridZoomToolbar,
            this.tsmiGroupListZoom,
            this.tsmiGlobalZoom});
            this.tsmiToolbars.Name = "tsmiToolbars";
            this.tsmiToolbars.Size = new System.Drawing.Size(240, 22);
            this.tsmiToolbars.Text = "&Toolbars";
            // 
            // tsmiListViewZoomToolbar
            // 
            this.tsmiListViewZoomToolbar.CheckOnClick = true;
            this.tsmiListViewZoomToolbar.Name = "tsmiListViewZoomToolbar";
            this.tsmiListViewZoomToolbar.Size = new System.Drawing.Size(158, 22);
            this.tsmiListViewZoomToolbar.Text = "L&ist view zoom";
            this.tsmiListViewZoomToolbar.Click += new System.EventHandler(this.tsmiListViewZoomToolbar_Click);
            // 
            // tsmiDataGridZoomToolbar
            // 
            this.tsmiDataGridZoomToolbar.CheckOnClick = true;
            this.tsmiDataGridZoomToolbar.Name = "tsmiDataGridZoomToolbar";
            this.tsmiDataGridZoomToolbar.Size = new System.Drawing.Size(158, 22);
            this.tsmiDataGridZoomToolbar.Text = "Dat&a grid zoom";
            this.tsmiDataGridZoomToolbar.Click += new System.EventHandler(this.tsmiDataGridZoomToolbar_Click);
            // 
            // tsmiGroupListZoom
            // 
            this.tsmiGroupListZoom.CheckOnClick = true;
            this.tsmiGroupListZoom.Name = "tsmiGroupListZoom";
            this.tsmiGroupListZoom.Size = new System.Drawing.Size(158, 22);
            this.tsmiGroupListZoom.Text = "&Group list zoom";
            this.tsmiGroupListZoom.Click += new System.EventHandler(this.tsmiGroupListZoom_Click);
            // 
            // tsmiGlobalZoom
            // 
            this.tsmiGlobalZoom.Checked = true;
            this.tsmiGlobalZoom.CheckOnClick = true;
            this.tsmiGlobalZoom.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiGlobalZoom.Name = "tsmiGlobalZoom";
            this.tsmiGlobalZoom.Size = new System.Drawing.Size(158, 22);
            this.tsmiGlobalZoom.Text = "Global &zoom";
            this.tsmiGlobalZoom.Click += new System.EventHandler(this.tsmiGlobalZoom_Click);
            // 
            // tsmiStatusBar
            // 
            this.tsmiStatusBar.Checked = true;
            this.tsmiStatusBar.CheckOnClick = true;
            this.tsmiStatusBar.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiStatusBar.Name = "tsmiStatusBar";
            this.tsmiStatusBar.Size = new System.Drawing.Size(240, 22);
            this.tsmiStatusBar.Text = "Status &bar";
            this.tsmiStatusBar.Click += new System.EventHandler(this.TsmiStatusBar_Click);
            // 
            // tsmiUseAdvancedViews
            // 
            this.tsmiUseAdvancedViews.CheckOnClick = true;
            this.tsmiUseAdvancedViews.Name = "tsmiUseAdvancedViews";
            this.tsmiUseAdvancedViews.Size = new System.Drawing.Size(240, 22);
            this.tsmiUseAdvancedViews.Text = "[BETA] &Use advanced views";
            this.tsmiUseAdvancedViews.Visible = false;
            this.tsmiUseAdvancedViews.Click += new System.EventHandler(this.tsmiUseAdvancedViews_Click);
            // 
            // tss6
            // 
            this.tss6.Name = "tss6";
            this.tss6.Size = new System.Drawing.Size(237, 6);
            this.tss6.Visible = false;
            // 
            // tsmiSplitViewHorizontally
            // 
            this.tsmiSplitViewHorizontally.Name = "tsmiSplitViewHorizontally";
            this.tsmiSplitViewHorizontally.Size = new System.Drawing.Size(240, 22);
            this.tsmiSplitViewHorizontally.Text = "&Split focused view horizontally";
            this.tsmiSplitViewHorizontally.Visible = false;
            this.tsmiSplitViewHorizontally.Click += new System.EventHandler(this.tsmiSplitViewHorizontally_Click);
            // 
            // tsmiSplitViewVertically
            // 
            this.tsmiSplitViewVertically.Name = "tsmiSplitViewVertically";
            this.tsmiSplitViewVertically.Size = new System.Drawing.Size(240, 22);
            this.tsmiSplitViewVertically.Text = "S&plit focused view vertically";
            this.tsmiSplitViewVertically.Visible = false;
            this.tsmiSplitViewVertically.Click += new System.EventHandler(this.tsmiSplitViewVertically_Click);
            // 
            // tsmiRemoveSplit
            // 
            this.tsmiRemoveSplit.Name = "tsmiRemoveSplit";
            this.tsmiRemoveSplit.Size = new System.Drawing.Size(240, 22);
            this.tsmiRemoveSplit.Text = "&Remove focused split";
            this.tsmiRemoveSplit.Visible = false;
            this.tsmiRemoveSplit.Click += new System.EventHandler(this.tsmiRemoveSplit_Click);
            // 
            // tsmiReplaceViewWith
            // 
            this.tsmiReplaceViewWith.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiReplaceWithListView,
            this.tsmiReplaceWithDataGrid,
            this.tsmiReplaceWithGroupList});
            this.tsmiReplaceViewWith.Name = "tsmiReplaceViewWith";
            this.tsmiReplaceViewWith.Size = new System.Drawing.Size(240, 22);
            this.tsmiReplaceViewWith.Text = "R&eplace focused view with...";
            this.tsmiReplaceViewWith.Visible = false;
            // 
            // tsmiReplaceWithListView
            // 
            this.tsmiReplaceWithListView.Name = "tsmiReplaceWithListView";
            this.tsmiReplaceWithListView.Size = new System.Drawing.Size(125, 22);
            this.tsmiReplaceWithListView.Text = "List view";
            this.tsmiReplaceWithListView.Click += new System.EventHandler(this.tsmiReplaceWithListView_Click);
            // 
            // tsmiReplaceWithDataGrid
            // 
            this.tsmiReplaceWithDataGrid.Name = "tsmiReplaceWithDataGrid";
            this.tsmiReplaceWithDataGrid.Size = new System.Drawing.Size(125, 22);
            this.tsmiReplaceWithDataGrid.Text = "Data grid";
            this.tsmiReplaceWithDataGrid.Click += new System.EventHandler(this.tsmiReplaceWithDataGrid_Click);
            // 
            // tsmiReplaceWithGroupList
            // 
            this.tsmiReplaceWithGroupList.Name = "tsmiReplaceWithGroupList";
            this.tsmiReplaceWithGroupList.Size = new System.Drawing.Size(125, 22);
            this.tsmiReplaceWithGroupList.Text = "Group list";
            this.tsmiReplaceWithGroupList.Click += new System.EventHandler(this.tsmiReplaceWithGroupList_Click);
            // 
            // tsmiFocusedSplitViewOrientation
            // 
            this.tsmiFocusedSplitViewOrientation.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiHorizontalOrientation,
            this.tsmiVerticalOrientation});
            this.tsmiFocusedSplitViewOrientation.Name = "tsmiFocusedSplitViewOrientation";
            this.tsmiFocusedSplitViewOrientation.Size = new System.Drawing.Size(240, 22);
            this.tsmiFocusedSplitViewOrientation.Text = "Fo&cused split view orientation...";
            this.tsmiFocusedSplitViewOrientation.Visible = false;
            // 
            // tsmiHorizontalOrientation
            // 
            this.tsmiHorizontalOrientation.Name = "tsmiHorizontalOrientation";
            this.tsmiHorizontalOrientation.Size = new System.Drawing.Size(129, 22);
            this.tsmiHorizontalOrientation.Text = "Hor&izontal";
            this.tsmiHorizontalOrientation.Click += new System.EventHandler(this.tsmiHorizontalOrientation_Click);
            // 
            // tsmiVerticalOrientation
            // 
            this.tsmiVerticalOrientation.Name = "tsmiVerticalOrientation";
            this.tsmiVerticalOrientation.Size = new System.Drawing.Size(129, 22);
            this.tsmiVerticalOrientation.Text = "&Vertical";
            this.tsmiVerticalOrientation.Click += new System.EventHandler(this.tsmiVerticalOrientation_Click);
            // 
            // tsmiWindow
            // 
            this.tsmiWindow.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiKeepOnTop});
            this.tsmiWindow.Name = "tsmiWindow";
            this.tsmiWindow.Size = new System.Drawing.Size(63, 20);
            this.tsmiWindow.Text = "&Window";
            // 
            // tsmiKeepOnTop
            // 
            this.tsmiKeepOnTop.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiKeepOnTopAlways,
            this.tsmiKeepOnTopUntilNextRestart,
            this.tsmiKeepOnTopFor5Minutes,
            this.tsmiKeepOnTopFor15Minutes,
            this.tsmiKeepOnTopFor1Hour});
            this.tsmiKeepOnTop.Image = ((System.Drawing.Image)(resources.GetObject("tsmiKeepOnTop.Image")));
            this.tsmiKeepOnTop.Name = "tsmiKeepOnTop";
            this.tsmiKeepOnTop.Size = new System.Drawing.Size(147, 22);
            this.tsmiKeepOnTop.Text = "&Keep on top...";
            // 
            // tsmiKeepOnTopAlways
            // 
            this.tsmiKeepOnTopAlways.CheckOnClick = true;
            this.tsmiKeepOnTopAlways.Name = "tsmiKeepOnTopAlways";
            this.tsmiKeepOnTopAlways.Size = new System.Drawing.Size(160, 22);
            this.tsmiKeepOnTopAlways.Text = "&Always";
            this.tsmiKeepOnTopAlways.Click += new System.EventHandler(this.tsmiKeepOnTopAlways_Click);
            // 
            // tsmiKeepOnTopUntilNextRestart
            // 
            this.tsmiKeepOnTopUntilNextRestart.CheckOnClick = true;
            this.tsmiKeepOnTopUntilNextRestart.Name = "tsmiKeepOnTopUntilNextRestart";
            this.tsmiKeepOnTopUntilNextRestart.Size = new System.Drawing.Size(160, 22);
            this.tsmiKeepOnTopUntilNextRestart.Text = "&Until next restart";
            this.tsmiKeepOnTopUntilNextRestart.Click += new System.EventHandler(this.tsmiKeepOnTopUntilNextRestart_Click);
            // 
            // tsmiKeepOnTopFor5Minutes
            // 
            this.tsmiKeepOnTopFor5Minutes.Name = "tsmiKeepOnTopFor5Minutes";
            this.tsmiKeepOnTopFor5Minutes.Size = new System.Drawing.Size(160, 22);
            this.tsmiKeepOnTopFor5Minutes.Text = "&For 5 minutes";
            this.tsmiKeepOnTopFor5Minutes.Visible = false;
            // 
            // tsmiKeepOnTopFor15Minutes
            // 
            this.tsmiKeepOnTopFor15Minutes.Name = "tsmiKeepOnTopFor15Minutes";
            this.tsmiKeepOnTopFor15Minutes.Size = new System.Drawing.Size(160, 22);
            this.tsmiKeepOnTopFor15Minutes.Text = "F&or 15 minutes";
            this.tsmiKeepOnTopFor15Minutes.Visible = false;
            // 
            // tsmiKeepOnTopFor1Hour
            // 
            this.tsmiKeepOnTopFor1Hour.Name = "tsmiKeepOnTopFor1Hour";
            this.tsmiKeepOnTopFor1Hour.Size = new System.Drawing.Size(160, 22);
            this.tsmiKeepOnTopFor1Hour.Text = "Fo&r 1 hour";
            this.tsmiKeepOnTopFor1Hour.Visible = false;
            // 
            // tsmiHelp
            // 
            this.tsmiHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiViewHelp,
            this.tsmiAbout});
            this.tsmiHelp.Name = "tsmiHelp";
            this.tsmiHelp.Size = new System.Drawing.Size(44, 20);
            this.tsmiHelp.Text = "&Help";
            // 
            // tsmiViewHelp
            // 
            this.tsmiViewHelp.Image = ((System.Drawing.Image)(resources.GetObject("tsmiViewHelp.Image")));
            this.tsmiViewHelp.Name = "tsmiViewHelp";
            this.tsmiViewHelp.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.tsmiViewHelp.Size = new System.Drawing.Size(153, 22);
            this.tsmiViewHelp.Text = "&View help...";
            this.tsmiViewHelp.Click += new System.EventHandler(this.tsmiViewHelp_Click);
            // 
            // tsmiAbout
            // 
            this.tsmiAbout.Image = ((System.Drawing.Image)(resources.GetObject("tsmiAbout.Image")));
            this.tsmiAbout.Name = "tsmiAbout";
            this.tsmiAbout.Size = new System.Drawing.Size(153, 22);
            this.tsmiAbout.Text = "&About...";
            this.tsmiAbout.Click += new System.EventHandler(this.tsmiAbout_Click);
            // 
            // tsDataGridZoom
            // 
            this.tsDataGridZoom.Dock = System.Windows.Forms.DockStyle.None;
            this.tsDataGridZoom.Location = new System.Drawing.Point(3, 24);
            this.tsDataGridZoom.Name = "tsDataGridZoom";
            this.tsDataGridZoom.Size = new System.Drawing.Size(538, 25);
            this.tsDataGridZoom.TabIndex = 4;
            this.tsDataGridZoom.Visible = false;
            // 
            // tsListViewZoom
            // 
            this.tsListViewZoom.Dock = System.Windows.Forms.DockStyle.None;
            this.tsListViewZoom.Location = new System.Drawing.Point(3, 24);
            this.tsListViewZoom.Name = "tsListViewZoom";
            this.tsListViewZoom.Size = new System.Drawing.Size(538, 25);
            this.tsListViewZoom.TabIndex = 3;
            this.tsListViewZoom.Visible = false;
            // 
            // tsGroupListZoom
            // 
            this.tsGroupListZoom.Dock = System.Windows.Forms.DockStyle.None;
            this.tsGroupListZoom.Location = new System.Drawing.Point(3, 24);
            this.tsGroupListZoom.Name = "tsGroupListZoom";
            this.tsGroupListZoom.Size = new System.Drawing.Size(538, 25);
            this.tsGroupListZoom.TabIndex = 7;
            this.tsGroupListZoom.Visible = false;
            // 
            // tsSort
            // 
            this.tsSort.Dock = System.Windows.Forms.DockStyle.None;
            this.tsSort.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.tscbAutomaticSort});
            this.tsSort.Location = new System.Drawing.Point(541, 24);
            this.tsSort.Name = "tsSort";
            this.tsSort.Size = new System.Drawing.Size(97, 27);
            this.tsSort.TabIndex = 5;
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(28, 24);
            this.toolStripLabel1.Text = "Sort";
            // 
            // tscbAutomaticSort
            // 
            this.tscbAutomaticSort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscbAutomaticSort.DropDownWidth = 175;
            this.tscbAutomaticSort.Items.AddRange(new object[] {
            "No automatic sort",
            "By closest ringing moment",
            "Alphabetically"});
            this.tscbAutomaticSort.Name = "tscbAutomaticSort";
            this.tscbAutomaticSort.Size = new System.Drawing.Size(175, 23);
            this.tscbAutomaticSort.Tag = "";
            this.tscbAutomaticSort.SelectedIndexChanged += new System.EventHandler(this.tscbAutomaticSort_SelectedIndexChanged);
            // 
            // tsGlobalZoom
            // 
            this.tsGlobalZoom.Dock = System.Windows.Forms.DockStyle.None;
            this.tsGlobalZoom.Location = new System.Drawing.Point(3, 24);
            this.tsGlobalZoom.Name = "tsGlobalZoom";
            this.tsGlobalZoom.Size = new System.Drawing.Size(538, 25);
            this.tsGlobalZoom.TabIndex = 6;
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(23, 23);
            // 
            // toolTip1
            // 
            this.toolTip1.ShowAlways = true;
            this.toolTip1.StripAmpersands = true;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "xml";
            this.saveFileDialog1.FileName = "TimedSilver.xml";
            this.saveFileDialog1.Filter = "XML files|*.xml|All files|*.*";
            // 
            // elementHost1
            // 
            this.elementHost1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elementHost1.Location = new System.Drawing.Point(3, 26);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(619, 331);
            this.elementHost1.TabIndex = 12;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = this.viewsGrid1;
            // 
            // tsmiSelectAll
            // 
            this.tsmiSelectAll.Name = "tsmiSelectAll";
            this.tsmiSelectAll.Size = new System.Drawing.Size(44, 20);
            this.tsmiSelectAll.Text = "Select &All";
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(638, 475);
            this.Controls.Add(this.toolStripContainer1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Timed Silver";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.PerformLayout();
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tsSort.ResumeLayout(false);
            this.tsSort.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        internal System.Windows.Forms.NotifyIcon notifyIcon1;
        private EnhancedCheckBox cbFullScreen;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmiHelp;
        private System.Windows.Forms.ToolStripMenuItem tsmiViewHelp;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem tsmiFile;
        internal System.Windows.Forms.SaveFileDialog saveFileDialog1;
        internal EnhancedCheckBox cbDarkMode;
        private System.Windows.Forms.ToolStripMenuItem tsmiWindow;
        private System.Windows.Forms.ToolStripMenuItem tsmiKeepOnTop;
        private System.Windows.Forms.ToolStripMenuItem tsmiKeepOnTopAlways;
        private System.Windows.Forms.ToolStripMenuItem tsmiKeepOnTopUntilNextRestart;
        private System.Windows.Forms.ToolStripMenuItem tsmiKeepOnTopFor5Minutes;
        private System.Windows.Forms.ToolStripMenuItem tsmiKeepOnTopFor15Minutes;
        private System.Windows.Forms.ToolStripMenuItem tsmiKeepOnTopFor1Hour;
        private System.Windows.Forms.ToolStripSeparator tss1;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenFile;
        private System.Windows.Forms.ToolStripMenuItem tsmiSave;
        private System.Windows.Forms.ToolStripMenuItem tsmiSaveAs;
        private System.Windows.Forms.ToolStripSeparator tss2;
        private System.Windows.Forms.ToolStripMenuItem tsmiExit;
        private System.Windows.Forms.ToolStripMenuItem tsmiEdit;
        private System.Windows.Forms.ToolStripMenuItem tsmiRemoveAllTimers;
        private EnhancedLabel lblFirstStart;
        private System.Windows.Forms.ToolStripMenuItem tsmiView;
        private System.Windows.Forms.ToolStripSeparator tss6;
        private System.Windows.Forms.ToolStripSeparator tss5;
        private System.Windows.Forms.ToolStripMenuItem tsmiListViewZoomToolbar;
        private System.Windows.Forms.ToolStripMenuItem tsmiDataGridZoomToolbar;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        internal ZoomToolStrip tsDataGridZoom;
        private System.Windows.Forms.ToolStrip tsSort;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox tscbAutomaticSort;
        private System.Windows.Forms.ToolStripMenuItem tsmiAbout;
        internal System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStripSeparator tss4;
        private System.Windows.Forms.ToolStripMenuItem tsmiSplitViewHorizontally;
        private System.Windows.Forms.ToolStripMenuItem tsmiSplitViewVertically;
        private System.Windows.Forms.ToolStripMenuItem tsmiRemoveSplit;
        private System.Windows.Forms.ToolStripMenuItem tsmiReplaceViewWith;
        private System.Windows.Forms.ToolStripMenuItem tsmiReplaceWithListView;
        private System.Windows.Forms.ToolStripMenuItem tsmiReplaceWithDataGrid;
        private System.Windows.Forms.ToolStripMenuItem tsmiReplaceWithGroupList;
        private System.Windows.Forms.ToolStripMenuItem tsmiUseAdvancedViews;
        internal ZoomToolStrip tsListViewZoom;
        private System.Windows.Forms.ToolStripSeparator tss3;
        private System.Windows.Forms.ToolStripMenuItem tsmiFocusedSplitViewOrientation;
        private System.Windows.Forms.ToolStripMenuItem tsmiHorizontalOrientation;
        private System.Windows.Forms.ToolStripMenuItem tsmiVerticalOrientation;
        private ZoomToolStrip tsGlobalZoom;
        internal System.Windows.Forms.ToolStripMenuItem tsmiViewGroupListView;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenContainingFolder;
        private System.Windows.Forms.ToolStripMenuItem tsmiGroupListZoom;
        private System.Windows.Forms.ToolStripMenuItem tsmiGlobalZoom;
        internal System.Windows.Forms.ToolStripMenuItem tsmiNew;
        internal System.Windows.Forms.ToolStripMenuItem tsmiSettings;
        internal System.Windows.Forms.ToolStripMenuItem tsmiDataGridView;
        internal System.Windows.Forms.ToolStripMenuItem tsmiListView;
        internal System.Windows.Forms.ToolStripMenuItem tsmiHorizontalSplitView;
        internal System.Windows.Forms.ToolStripMenuItem tsmiVerticalSplitView;
        internal System.Windows.Forms.ToolStripMenuItem tsmiFocusListView;
        internal System.Windows.Forms.ToolStripMenuItem tsmiFocusDataGridView;
        internal System.Windows.Forms.ToolStripMenuItem tsmiToolbars;
        internal System.Windows.Forms.ToolStripMenuItem tsmiRecentFiles;
        internal System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        internal System.Windows.Forms.ToolStripMenuItem tsmiMultipleSelection;
        private System.Windows.Forms.ToolStripMenuItem tsmiRestoreToPreviousDay;
        internal System.Windows.Forms.ToolStripMenuItem tsmiFocusGroupListView;
        internal ZoomToolStrip tsGroupListZoom;
        private System.Windows.Forms.ToolStripMenuItem tsmiAdvanced;
        private System.Windows.Forms.ToolStripMenuItem tsmiSelectiveOpen;
        private System.Windows.Forms.ToolStripMenuItem tsmiClearOpenFile;
        private System.Windows.Forms.ToolStripMenuItem tsmiReloadOpenFileFromFileSystem;
        private System.Windows.Forms.ToolStripMenuItem tsmiStatusBar;
        private System.Windows.Forms.ToolStripMenuItem tsmiSelectAll;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
        internal ViewsGrid viewsGrid1;
    }
}

