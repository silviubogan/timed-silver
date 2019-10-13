using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Shell;
using System.Linq;
using System.Windows.Forms.Integration;

namespace cs_timed_silver
{
    public partial class MainForm : Form, ISaveable, ISuspendableLayout, IMultipleViewTypesForm, IAllowDropForm
    {
        internal bool OnLoadUpdateViewType = true;
        internal bool OnLoadCreateBasicViewDataWithView = true;

        internal FocusedClocksViewProvider MyFocusedClocksViewProvider;
        internal ClocksViewProvider MyClocksViewProvider;
        //internal ViewDataFactory MyViewDataFactory;

        internal ElementHost statusBarHost1;
        internal CustomStatusBar MyCustomStatusBar;

        internal RecentFilesMenuStrip recentFilesMenuStrip1;
        
        //internal List<IClocksView> MyClocksViews =>
        //    MyClocksViewProvider.GetAllExistingClocksViews(MyViewDataFactory);

        internal TimeOutFormsManager MyTimeOutFormsManager;

        [Obsolete]
        public bool EasyGroupListViewVisible
        {
            get
            {
                return MyClocksViewProvider.EasyGroupListViewVisible;
            }
            set
            {
                MyClocksViewProvider.EasyGroupListViewVisible = value;
            }
        }

        internal void UpdateTsmiNewEnabled()
        {
            if (MyDataFile.FilePath == "")
            {
                tsmiNew.Enabled = MyDataFile.IsUnsaved;
            }
            else
            {
                tsmiNew.Enabled = true;
            }
        }

        internal EasyViewType _SelectedViewType;
        public EasyViewType SelectedViewType
        {
            get
            {
                return _SelectedViewType;
            }
            set
            {
                if (_SelectedViewType != value)
                {
                    _SelectedViewType = value;
                    ForceUpdateViewType(value);
                }
            }
        }
        
        [Obsolete]
        internal IClocksView LastFocusedTimersView
        {
            get
            {
                return MyFocusedClocksViewProvider.FocusedClocksView;
            }
            set
            {
                if (MyFocusedClocksViewProvider.FocusedClocksView != value)
                {
                    MyFocusedClocksViewProvider.FocusedClocksView = value;
                }
            }
        }

        public void ForceUpdateViewType(EasyViewType value)
        {
            Utils.SuspendLayoutRecursively(this);

            // Update the views data structure and Controls
            if (value == EasyViewType.HorizontalSplit ||
                value == EasyViewType.VerticalSplit)
            {
                MyClocksViewProvider.ForceUpdateViewType_Split(
                    value == EasyViewType.HorizontalSplit ?
                        Orientation.Horizontal : Orientation.Vertical,
                    tsmiViewGroupListView.Checked);
            }
            else if (value == EasyViewType.List)
            {
                MyClocksViewProvider.ForceUpdateViewType_List(tsmiViewGroupListView.Checked);
            }
            else // if (value == EasyViewType.DataGrid)
            {
                MyClocksViewProvider.ForceUpdateViewType_DataGrid(tsmiViewGroupListView.Checked);
            }

            //UpdateRootSplitterViewControl();


            // Apply the dark theme setting to possible new Controls created above
            ForceUpdateOfDarkThemeSetting(cbDarkMode.Checked);


            // Update the menu items and tool bars to reflect the new views
            tsmiDataGridView.Checked = value == EasyViewType.DataGrid;
            tsmiHorizontalSplitView.Checked = value == EasyViewType.HorizontalSplit;
            tsmiVerticalSplitView.Checked = value == EasyViewType.VerticalSplit;
            tsmiListView.Checked = value == EasyViewType.List;
            
            tsDataGridZoom.SetItemsEnabled(
                value == EasyViewType.DataGrid ||
                value == EasyViewType.HorizontalSplit ||
                value == EasyViewType.VerticalSplit);
            tsListViewZoom.SetItemsEnabled(
                value == EasyViewType.List ||
                value == EasyViewType.HorizontalSplit ||
                value == EasyViewType.VerticalSplit);
            tsGroupListZoom.SetItemsEnabled(tsmiViewGroupListView.Checked);


            // Apply zoom percents from settings to possible new views created above
            if (value == EasyViewType.List)
            {
                PropagateGlobalZoomFactor();
            }
            else if (value == EasyViewType.DataGrid)
            {
                PropagateDataGridGlobalZoomFactor();
            }
            else if (value == EasyViewType.HorizontalSplit ||
                     value == EasyViewType.VerticalSplit)
            {
                PropagateGlobalZoomFactor();
                PropagateDataGridGlobalZoomFactor();
            }

            if (tsmiViewGroupListView.Checked)
            {
                PropagateGroupListZoomPercent();
            }

            ApplyEasyViewsSizesFromSettings(MyDataFile);

            Utils.ResumeLayoutRecursively(this, true);

            // Not understood case:
            //if (MyClocksViewProvider.RootSplitterView == null)
            //{
            //    throw new NotImplementedException();
            //}
        }

        internal void UpdateFormTitle()
        {
            if (MyDataFile.FilePath == "")
            {
                if (MyDataFile.IsUnsaved)
                {
                    Text = $"Timed Silver - new file *";
                }
                else
                {
                    Text = $"Timed Silver - new file";
                }
            }
            else
            {
                string baseFileName = Utils.BaseFileNameInPath(MyDataFile.FilePath);

                if (MyDataFile.IsUnsaved)
                {
                    Text = $"Timed Silver - {baseFileName} *";
                }
                else
                {
                    Text = $"Timed Silver - {baseFileName}";
                }
            }
        }



        internal MyToolStripProfessionalRenderer DarkModeMenuStripRenderer;

        internal DataFile PreviousDataFile = null;
        internal DataFile _MyDataFile = null;
        internal DataFile MyDataFile
        {
            get
            {
                return _MyDataFile;
            }
            set
            {
                if (_MyDataFile != value)
                {
                    PreviousDataFile = _MyDataFile;
                    _MyDataFile = value;
                    OnMyDataFileChanged();
                }
            }
        }

        private void OnMyDataFileChanged()
        {
            if (PreviousDataFile != null)
            {
                PreviousDataFile.FilePathChanged -= MyDataFile_FilePathChanged;
                PreviousDataFile.IsUnsavedChanged -= MyDataFile_IsDirtyChanged;
                PreviousDataFile.FileClosed -= MyDataFile_FileClosed;
                PreviousDataFile.FileLoaded -= MyDataFile_FileLoaded;

                PreviousDataFile.Settings.SettingValueChange -= Settings_SettingValueChange;

                PreviousDataFile.ClockVMCollection.Model.BeforeAppliedFilterChange -= ClockCollection_BeforeAppliedFilterChange;
                PreviousDataFile.ClockVMCollection.Model.AfterAppliedFilterChange -= ClockCollection_AfterAppliedFilterChange;
                PreviousDataFile.ClockVMCollection.Model.GlobalProgressChanged -= ClockCollection_GlobalProgressChanged;
            }
            if (MyDataFile != null)
            {
                MyDataFile.FilePathChanged += MyDataFile_FilePathChanged;
                MyDataFile.IsUnsavedChanged += MyDataFile_IsDirtyChanged;
                MyDataFile.FileClosed += MyDataFile_FileClosed;
                MyDataFile.FileLoaded += MyDataFile_FileLoaded;

                MyDataFile.Settings.SettingValueChange += Settings_SettingValueChange;

                MyDataFile.ClockVMCollection.Model.BeforeAppliedFilterChange += ClockCollection_BeforeAppliedFilterChange;
                MyDataFile.ClockVMCollection.Model.AfterAppliedFilterChange += ClockCollection_AfterAppliedFilterChange;
                MyDataFile.ClockVMCollection.Model.GlobalProgressChanged += ClockCollection_GlobalProgressChanged;
            }
        }

        private void ClockCollection_GlobalProgressChanged(object sender, DoubleEventArgs e)
        {
            if (e.Value == -1D)
            {
                // remove progress from task bar
                SetTaskBarProgressPercent(-1);
            }
            else if (e.Value == 0D)
            {
                SetTaskBarProgressPercent(0);
            }
            else
            {
                // set progress to percent: remainingSum/resetToSum * 100
                SetTaskBarProgressPercent(e.Value);
            }
        }

        private void ClockCollection_AfterAppliedFilterChange(object sender, EventArgs e)
        {
        }

        private void ClockCollection_BeforeAppliedFilterChange(object sender, EventArgs e)
        {
        }

        internal void ApplySetting(SettingDataM sd)
        {
            switch (sd.Name)
            {
                case "ZoomGroupListToolStrip":
                    ClockGroupListView tglv = MyClocksViewProvider.GetExistingOrNewClockGroupListView();
                    break;

                case "AudioFilePath":
                    MyDataFile.MultiAudioPlayer.AudioFilePath = MySettingsForm.LastAudioFilePath;
                    break;

                case "RingingDuration":
                    PropagateNewRingingDuration();
                    break;

                case "RoundedCorners":
                    PropagateRoundedCornersSetting();
                    break;

                case "AlwaysMute":
                    MyDataFile.MultiAudioPlayer.Mute = (bool)MyDataFile.Settings.GetValue("AlwaysMute");
                    break;

                case "RecentAudioFilePaths":
                    MySettingsForm.audioFileChooser1.recentFilesMenuStrip1.
                        LoadRecentPathsFromString(MyDataFile.Settings.GetValue("RecentAudioFilePaths") as string);
                    break;

                case "RecentImageFilePaths":
                    MySettingsForm.imageFileChooser1.recentFilesMenuStrip1.
                        LoadRecentPathsFromString(
                            MyDataFile.Settings.GetValue("RecentImageFilePaths") as string);
                    break;

                case "MainFormAlwaysOnTop":
                    SetKeepOnTopAlways(tsmiKeepOnTopAlways.Checked);
                    break;

                case "MainFormWindowState":
                    FormWindowState s = Utils.StrToEnum<FormWindowState>((string)MyDataFile.Settings.GetValue("MainFormWindowState"));
                    WindowState = s;
                    break;

                case "MainFormRectangle":
                    object objFormR = MyDataFile.Settings.GetValue("MainFormRectangle");
                    if (objFormR != null)
                    {
                        DesktopBounds = (Rectangle)objFormR;
                    }
                    break;

                case "ShowGroupListView":
                    if (!tsmiViewGroupListView.Checked && (bool)MyDataFile.Settings.GetValue("ShowGroupListView"))
                    {
                        tsmiViewGroupListView.Checked = true;
                        tsmiViewGroupListView_Click(null, EventArgs.Empty);
                    }
                    else if (tsmiViewGroupListView.Checked && !(bool)MyDataFile.Settings.GetValue("ShowGroupListView"))
                    {
                        tsmiViewGroupListView.Checked = false;
                        tsmiViewGroupListView_Click(null, EventArgs.Empty);
                    }
                    break;

                case "ViewType":
                    var vt = MyDataFile.Settings.GetValue("ViewType") as string;
                    if (OnLoadUpdateViewType)
                    {
                        try
                        {
                            if (SelectedViewType != Utils.StrToEnum<EasyViewType>(vt))
                            {
                                SelectedViewType = Utils.StrToEnum<EasyViewType>(vt);
                            }
                            else
                            {
                                ForceUpdateViewType(SelectedViewType);
                            }
                        }
                        catch (Exception)
                        {
                            SelectedViewType = EasyViewType.List;
                        }
                        // TODO: [VISUAL] Ctrl-S does not work when a TImrControl is focused? Test this.
                    }
                    break;

                case "DarkMode":
                    cbDarkMode.Checked = (bool)MyDataFile.Settings.GetValue("DarkMode");
                    ForceUpdateOfDarkThemeSetting(cbDarkMode.Checked);
                    break;

                case "GlobalZoomPercent":
                    MyClocksViewProvider.EasyClockListViewZoomPercent = Convert.ToDecimal(MyDataFile.Settings.GetValue("GlobalZoomPercent"));
                    PropagateGlobalZoomFactor();
                    break;

                case "GlobalDataGridZoomPercent":
                    MyClocksViewProvider.EasyClockDataGridViewZoomPercent = Convert.ToDecimal(MyDataFile.Settings.GetValue("GlobalDataGridZoomPercent"));
                    PropagateDataGridGlobalZoomFactor();
                    break;

                case "GroupListListViewViewType":
                    ClockGroupListView eglv = MyClocksViewProvider.GetExistingOrNewClockGroupListView();
                    string view = MyDataFile.Settings.GetValue("GroupListListViewViewType").ToString();
                    switch (view)
                    {
                        case "Large icons":
                            eglv.MyViewComboBox.SelectedIndex = 0;
                            break;

                        case "Stack":
                            eglv.MyViewComboBox.SelectedIndex = 1;
                            break;

                        case "Grid":
                            eglv.MyViewComboBox.SelectedIndex = 2;
                            break;

                        default:
                            break;
                    }
                    break;

                case "GroupListZoomPercent":
                    MyClocksViewProvider.EasyClockGroupListViewZoomPercent = Convert.ToDecimal(MyDataFile.Settings.GetValue("GroupListZoomPercent"));
                    //GroupListZoomPercent = tsGroupListZoom.tstb.Value;
                    PropagateGroupListZoomPercent();
                    break;

                case "AutosortMode":
                    if (!string.IsNullOrEmpty((string)MyDataFile.Settings.GetValue("AutosortMode")))
                    {
                        MyDataFile.ClockVMCollection.Model.AutosortMode = Utils.StrToEnum<AutosortMode>(
                            (string)MyDataFile.Settings.GetValue("AutosortMode"));
                    }
                    ApplyAutosortSetting();
                    break;

                case "Filter":
                    ApplyFilter(new Filter(
                        MyDataFile.ClockVMCollection.Model,
                        (string)MyDataFile.Settings.GetValue("Filter")));
                    break;

                case "EasyViewGroupListWidthPercent":
                case "EasyViewClockListViewSizePercent":
                    //if (MyClocksViewProvider.RootSplitterView != null)
                    //{
                    //    ApplyEasyViewsSizesFromSettings(MyDataFile);
                    //}
                    break;

                case "EasyViewGroupListScrolledPercent":
                case "EasyViewClockListViewScrolledPercent":
                case "EasyViewClockDataGridScrolledPercent":
                    //ApplyScrollPercentsToControls();
                    break;

                case "ShowGlobalStatusBar":
                    if ((bool)MyDataFile.Settings.GetValue("ShowGlobalStatusBar"))
                    {
                        if (cbFullScreen.Checked)
                        {
                            tableLayoutPanel1.RowStyles[2].Height = 50;
                        }
                        else
                        {
                            tableLayoutPanel1.RowStyles[2].Height = 30;
                        }
                        statusBarHost1.Visible = true;
                        tsmiStatusBar.Checked = true;
                    }
                    else
                    {
                        tableLayoutPanel1.RowStyles[2].Height = 0;
                        statusBarHost1.Visible = false;
                        tsmiStatusBar.Checked = false;
                    }
                    break;

                case "AutoresizeTableColumns":
                    (elementHost1.Child as ViewsGrid).MyDataGrid.AutoresizeTableColumns =
                        (bool)MyDataFile.Settings.GetValue("AutoresizeTableColumns");
                    break;
            }
        }
        
        private void Settings_SettingValueChange(object sender, SettingValueChangedEventArgs e)
        {
            var sd = sender as SettingDataM;

            // TODO: why is this commented?:
            //if (e.IsInitialization)
            //{
            //}
            //else
            //{
                ApplySetting(sd);
            //}
        }

        internal EditToolStrip tsEdit;

        internal SettingsForm MySettingsForm;

        [Obsolete]
        internal Pointer RootSplitterViewPointer
        {
            get
            {
                return MyClocksViewProvider.RootSplitterViewPointer;
            }
        }
        
        internal bool _DraggingOver = false;
        internal bool DraggingOver
        {
            get
            {
                return _DraggingOver;
            }
            set
            {
                if (_DraggingOver != value)
                {
                    _DraggingOver = value;

                    UpdateDropIndicator();
                }
            }
        }

        internal ControlEnhancer MyControlEnhancer;

        internal OverlayControl MyOverlay;
        
        internal JumpList MyJumpList;

        internal MainForm(Program program = null)
        {
            if (System.Windows.Application.Current == null) // the case of unit testing
            {

            }
            else
            {
                MyJumpList = JumpList.GetJumpList(System.Windows.Application.Current);
                if (MyJumpList == null)
                {
                    JumpList.SetJumpList(System.Windows.Application.Current,
                        MyJumpList = new JumpList());
                }

                MyJumpList.JumpItems.Clear();
                MyJumpList.JumpItems.Add(new JumpTask()
                {
                    Arguments = "--new",
                    Title = "New file"
                });
                MyJumpList.JumpItems.Add(new JumpTask()
                {
                    Arguments = "--last",
                    Title = "Last opened file"
                });
                MyJumpList.Apply();
            }

            MyTimeOutFormsManager = new TimeOutFormsManager(this);

            MyClocksViewProvider = new ClocksViewProvider(this);

            MyFocusedClocksViewProvider = new FocusedClocksViewProvider();
            MyFocusedClocksViewProvider.FocusedClocksViewChanged += MyFocusedClocksViewProvider_FocusedClocksViewChanged;

            //MyViewDataFactory = new ViewDataFactory();

            InitializeComponent();

            //tsmiMultipleSelection.Image = Utils.IconResourceVersionBySize(Properties.Resources.multiselect, 16);

            //notifyIcon1.Text = $"Timed Silver v{Assembly.GetAssembly(typeof(MainForm)).GetName().Version.ToString(2)}";

            if (program != null)
            {
                program.ScreenMouseMove += Program_ScreenMouseMove;
            }

            tsEdit = new EditToolStrip(this);

            ToolStrips.AddRange(new ToolStrip[]
            {
                tsGroupListZoom,
                tsListViewZoom,
                tsDataGridZoom,
                tsGlobalZoom,
                tsSort,
                tsEdit
            });
            tsGroupListZoom.Program = tsListViewZoom.Program =
                tsDataGridZoom.Program = tsGlobalZoom.Program =
                    program;

            MyDataFile = new DataFile(this, true);
            viewsGrid1.DataFile = MyDataFile;

            MySettingsForm = new SettingsForm(this);
            MySettingsForm.audioFileChooser1.MainForm = this;

            recentFilesMenuStrip1 = new RecentFilesMenuStrip(this);
            recentFilesMenuStrip1.PathValidationRequested += RecentFilesMenuStrip1_PathValidationRequested;
            recentFilesMenuStrip1.SettingSaveRequested += RecentFilesMenuStrip1_SettingSaveRequested;
            tsmiRecentFiles.DropDown = recentFilesMenuStrip1;
            
            recentFilesMenuStrip1.LoadRecentPathsFromString(Properties.Settings.Default.RecentFilePaths);

            // before loading the inital file or applying the default settings AND after creating the MyDataFile and views
            //timerDataGridView1 = CreateNewTimerDataGridView();
            //timerGroupListView1 = CreateNewGroupListView();

            if (Properties.Settings.Default.FirstStart)
            {
                lblFirstStart.Visible = true;
            }
            else
            {
                lblFirstStart.Visible = false;
            }
            Properties.Settings.Default.FirstStart = false;
            Properties.Settings.Default.Save();

            DarkModeMenuStripRenderer = new MyToolStripProfessionalRenderer();

            Application.ApplicationExit += Application_ApplicationExit;

            MouseWheel += MainForm_MouseWheel;

            tsDataGridZoom.MyForm =
                tsListViewZoom.MyForm =
                tsGlobalZoom.MyForm =
                tsGroupListZoom.MyForm = this;

            // TODO: set unduplicated tooltip strings in designer, not here:

            toolTip1.SetToolTip(cbFullScreen, "Check if you want the program to occupy the full screen.");
            toolTip1.SetToolTip(cbDarkMode, "Check if you want to use the dark theme instead of the default theme.");

            // NOTE: tool tips on tick on the enhanced track bar in future?
            //toolTip1.SetToolTip(lbl2, strZoom);

            // TODO:dynamic strings in funcion to the position of teh toolstrips
            const string strListViewZoomIn = "Click on this button to zoom in on the timers in the list view below.";
            const string strListViewZoomOut = "Click on this button to zoom out on the timers in the list view below.";

            const string strDataGridZoomIn = "Click on this button to zoom in on the table below.";
            const string strDataGridZoomOut = "Click on this button to zoom out on the table below.";


            tsListViewZoom.tsbZoomIn.ToolTipText = strListViewZoomIn;
            tsListViewZoom.tsbZoomOut.ToolTipText = strListViewZoomOut;

            tsDataGridZoom.tsbZoomIn.ToolTipText = strDataGridZoomIn;
            tsDataGridZoom.tsbZoomOut.ToolTipText = strDataGridZoomOut;

            tsGroupListZoom.tsbZoomIn.ToolTipText = strDataGridZoomIn;
            tsGroupListZoom.tsbZoomOut.ToolTipText = strDataGridZoomOut;

            tsGlobalZoom.tsbZoomIn.ToolTipText = "Click on this button to zoom in on the timers below.";
            tsGlobalZoom.tsbZoomOut.ToolTipText = "Click on this button to zoom out on the timers below.";

            tsListViewZoom.PropertyName = "GlobalZoomPercent";
            tsDataGridZoom.PropertyName = "GlobalDataGridZoomPercent";
            tsGroupListZoom.PropertyName = "GroupListZoomPercent";

            tsListViewZoom.ValuePropagationRequested += TsListViewZoom_ValuePropagationRequested;
            tsListViewZoom.ZoomPercentChanged +=
                TsListViewZoom_ZoomPercentChanged;
            tsListViewZoom.SettingUpdateRequested +=
                TsListViewZoom_SettingUpdateRequested;
            tsListViewZoom.TimersView = MyClocksViewProvider.GetExistingOrNewClockListView();

            tsDataGridZoom.ValuePropagationRequested += TsDataGridZoom_ValuePropagationRequested;
            tsDataGridZoom.ZoomPercentChanged +=
                TsDataGridZoom_ZoomPercentChanged;
            tsDataGridZoom.SettingUpdateRequested +=
                TsDataGridZoom_SettingUpdateRequested;
            tsDataGridZoom.TimersView = MyClocksViewProvider.GetExistingOrNewClockDataGridView();

            tsGroupListZoom.ValuePropagationRequested += TsGroupListZoom_ValuePropagationRequested;
            tsGroupListZoom.ZoomPercentChanged +=
                TsGroupListZoom_ZoomPercentChanged;
            tsGroupListZoom.SettingUpdateRequested +=
                TsGroupListZoom_SettingUpdateRequested;
            tsGroupListZoom.TimersView = MyClocksViewProvider.GetExistingOrNewClockGroupListView();

            tsGlobalZoom.ValuePropagationRequested += TsGlobalZoom_ValuePropagationRequested;
            tsGlobalZoom.ZoomPercentChanged += TsGlobalZoom_ZoomPercentChanged;
            tsGlobalZoom.SettingUpdateRequested += TsGlobalZoom_SettingUpdateRequested;

            tsmiAbout.ToolTipText = "Click this button to see the version of the program and the credits including the credits for used resources.";

            tsListViewZoom.Title = "List view";
            tsDataGridZoom.Title = "Data grid";
            tsGroupListZoom.Title = "Group list";

            MyControlEnhancer = new ControlEnhancer(this);
            MyControlEnhancer.FocusedControlChanged +=
                Ce_FocusedControlChanged;
            MyControlEnhancer.ControlDragDrop +=
                MyControlEnhancer_ControlDragDrop;
            MyControlEnhancer.ControlDragEnter +=
                MyControlEnhancer_ControlDragEnter;
            MyControlEnhancer.ControlDragLeave +=
                MyControlEnhancer_ControlDragLeave;

            MyCustomStatusBar = new CustomStatusBar();

            statusBarHost1 = new ElementHost();
            statusBarHost1.Child = MyCustomStatusBar;
            statusBarHost1.Dock = DockStyle.Fill;

            tableLayoutPanel1.Controls.Add(statusBarHost1, 0, 2);
            tableLayoutPanel1.SetColumnSpan(statusBarHost1, 5);

            tsmiSelectAll.ShortcutKeyDisplayString = "Ctrl+A";
            tsmiSelectAll.ShortcutKeys = Keys.Control | Keys.A;
            tsmiSelectAll.Click += TsmiSelectAll_Click;
        }

        private void TsmiSelectAll_Click(object sender, EventArgs e)
        {
            var mf = System.Windows.Forms.Application.OpenForms[0] as MainForm;
            mf.tsmiMultipleSelection.Checked = true;
            MyDataFile.ClockVMCollection.Model.CheckAll();
        }

        private void MyFocusedClocksViewProvider_FocusedClocksViewChanged(object sender, EventArgs e)
        {
            tsGlobalZoom.TimersView = MyFocusedClocksViewProvider.FocusedClocksView;
            tsGlobalZoom.SetZoomPercentAsSystemIfValid(MyFocusedClocksViewProvider.GetZoomPercent());
        }

        /// <summary>
        /// TODO: In the case of DraggingOver, it's a hack that should be avoided.
        /// (The DragLeave event is not triggered for the MainForm in the case
        /// of very fast mouse movements.)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Program_ScreenMouseMove(object sender, EventArgs e)
        {
            if (!Bounds.Contains(Cursor.Position))
            {
                DraggingOver = false;
            }
        }

        private void MyControlEnhancer_ControlDragLeave(object sender, EventArgs e)
        {
            //DraggingOver = false;
        }

        private void MyControlEnhancer_ControlDragDrop(object sender, DragEventArgs e)
        {
            if (!this.HasOpenModalChildWindows())
            {
                HandleDragDrop(e);
            }
        }

        private void UpdateDropIndicator()
        {
            if (DraggingOver)
            {
                MyOverlay.Background = Utils.PrintClientRectangleToImage(this);
                MyOverlay.Visible = true;
                MyOverlay.BringToFront();
            }
            else
            {
                MyOverlay.Visible = false;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (MyOverlay != null)
            {
                MyOverlay.Size = ClientSize;
                UpdateOverlayFontSize();
            }
        }

        private void UpdateOverlayFontSize()
        {
            MyOverlay.Font = new Font(MyOverlay.Font.FontFamily,
                Math.Max(1, Width / 50));
        }
        
        internal void HandleDragEnter(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Link;

                var paths = e.Data.
                    GetData(DataFormats.FileDrop) as string[];

                string path = paths[0];

                bool knownExtension = path.EndsWith(".xml");

                if (!knownExtension)
                {
                    MyOverlay.DropHereText = "Unknown file extension";
                    MyOverlay.MyImage = SystemIcons.Warning.ToBitmap();
                }
                else
                {
                    MyOverlay.ResetDropHereText();
                    MyOverlay.ResetDropHereImage();
                }

                DraggingOver = true;
            }
            else
            {
                DraggingOver = false;
            }
        }

        protected override void OnDragLeave(EventArgs e)
        {
            if (!Bounds.Contains(Cursor.Position))
            {
                DraggingOver = false;
            }

            base.OnDragLeave(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            
            //DraggingOver = false;
        }

        internal void HandleDragDrop(DragEventArgs drgevent)
        {
            Utils.SuspendLayoutRecursively(this);

            string[] paths = null;

            if (drgevent.Effect != DragDropEffects.None &&
                drgevent.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // TODO: use a separate thread (Task) to read the file from XML to DataFile.
                // TODO [VISUAL]: at least show a loading indicator while loading the file, using Application.DoEvents from time to time to show a small animation.
                paths = drgevent.Data.
                    GetData(DataFormats.FileDrop) as string[];
            }

            DraggingOver = false;
            MyOverlay.ResetDropHereText();
            MyOverlay.ResetDropHereImage();

            // TODO: [VISUAL] do not accept folders, parse shortcut files, accept only XML files
            if (paths != null)
            {
                if (MyDataFile.FilePath != "" || MyDataFile.IsUnsaved)
                {
                    MyDataFile.CloseAndOpenOtherFile(paths[0]);
                }
                else
                {
                    MyDataFile.LoadFromFile(paths[0]);
                }
                Utils.ResumeLayoutRecursively(this, true);
            }

            Utils.ResumeLayoutRecursively(this, false);
        }

        internal bool IsBeingLoaded = true;

        protected override void OnLoad(EventArgs e)
        {
            MyClocksViewProvider.HandleSplitterMoved = false;

            LoadOverlay();
            MyOverlay.Size = ClientSize;
            UpdateOverlayFontSize();

            base.OnLoad(e);
        }

        /// NOTE: also called when Location changes
        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);

            UpdateMainFormRectangleSetting();

            if (!ApplyWithoutSetting && MyDataFile != null)
            {
                MyDataFile.Settings.SetValue("MainFormWindowState", WindowState.ToString());
            }
        }

        private void UpdateMainFormRectangleSetting()
        {
            if (!Visible)
            {
                MyDataFile.Settings.IsUnsavedLocked = true;
            }
            MyDataFile.Settings.SetValue("MainFormRectangle", DesktopBounds);
            if (!Visible)
            {
                MyDataFile.Settings.IsUnsavedLocked = false;
            }
        }

        private void MyControlEnhancer_ControlDragEnter(object sender, DragEventArgs e)
        {
            if (!this.HasOpenModalChildWindows())
            {
                HandleDragEnter(e);
            }
        }

        internal void LoadOverlay()
        {
            MyOverlay = new OverlayControl();
            MyOverlay.Visible = false;
            Controls.Add(MyOverlay);
            MyOverlay.Location = Point.Empty;
            MyOverlay.Size = DisplayRectangle.Size;
            UpdateOverlayFontSize();
        }

        private void Ce_FocusedControlChanged(object sender,
            FocusMovedEventArgs e)
        {
            ApplyFocusedControlChange(e.FocusedControl);
        }

        internal void ApplyFocusedControlChange(Control cc)
        {
            Control c = Utils.FindControlWithFocus(cc);

            while (c != null &&
                !Utils.TypeImplements(c.GetType(), typeof(IClocksView)))
            {
                c = c.Parent;
            }

            if (c != null)
            {
                MyFocusedClocksViewProvider.FocusedClocksView = c as IClocksView;
            }
        }

        private void RecentFilesMenuStrip1_SettingSaveRequested(object sender, StringEventArgs e)
        {
            Properties.Settings.Default.RecentFilePaths = e.StringValue;
            Properties.Settings.Default.Save();
        }

        private void RecentFilesMenuStrip1_PathValidationRequested(object sender, PathValidationEventArgs e)
        {
            e.Valid = File.Exists(e.Path) &&
                MyDataFile.CloseAndOpenOtherFile(e.Path);
        }

        private void TsGroupListZoom_SettingUpdateRequested(object sender, EventArgs e)
        {
            //MyDataFile.SetValue("GroupListZoomPercent", GroupListZoomPercent);
        }

        private void TsGroupListZoom_ZoomPercentChanged(object sender, EventArgs e)
        {
            MyDataFile.Settings.SetValue("GroupListZoomPercent", (decimal)tsGroupListZoom.tstb.Value);
        }

        private void TsGroupListZoom_ValuePropagationRequested(object sender, EventArgs e)
        {
            //PropagateGroupListZoomPercent();
        }

        internal void TsGlobalZoom_SettingUpdateRequested(object sender, EventArgs e)
        {
        }

        //internal ViewDataType GetFocusedViewType(ViewData vd = null)
        //{
        //    //if (vd != null)
        //    //{
        //    //    if (vd.View.HasFocus && vd.ViewType != ViewDataType.Splitter)
        //    //    {
        //    //        return vd.ViewType;
        //    //    }
        //    //    if (vd.GetFirstChild()?.View != null && vd.GetFirstChild().View.HasFocus)
        //    //    {
        //    //        return GetFocusedViewType(vd.GetFirstChild());
        //    //    }
        //    //    else if (vd.GetSecondChild()?.View != null && vd.GetSecondChild().View.HasFocus)
        //    //    {
        //    //        return GetFocusedViewType(vd.GetSecondChild());
        //    //    }
        //        return ViewDataType.List;
        //    //}

        //    //return GetFocusedViewType(MyViewDataFactory.RootViewData);
        //}

        internal void TsGlobalZoom_ZoomPercentChanged(object sender, EventArgs e)
        {
            //if (MyFocusedClocksViewProvider.FocusedClocksView == null)
            //{
            //    return;
            //}

            //// TODO: implement and use ITimersView.HasFocus property here and in other ZoomToolStrip evt handlers.
            //ViewDataType t = GetFocusedViewType();

            //if (t == ViewDataType.DataGrid)
            //{
            //    MyDataFile.Settings.SetValue("GlobalDataGridZoomPercent", (decimal)tsGlobalZoom.tstb.Value);
            //}
            //else if (t == ViewDataType.List)
            //{
            //    MyDataFile.Settings.SetValue("GlobalZoomPercent", (decimal)tsGlobalZoom.tstb.Value);
            //}
            //else if (t == ViewDataType.GroupList)
            //{
            //    MyDataFile.Settings.SetValue("GroupListZoomPercent", (decimal)tsGlobalZoom.tstb.Value);
            //}
            //else // ViewDataType.Splitter
            //{
            //    throw new NotImplementedException();
            //}
        }

        internal void TsGlobalZoom_ValuePropagationRequested(object sender, EventArgs e)
        {
        }

        internal void TsEdit_SettingsRequested(object sender, EventArgs e)
        {
            tsmiSettings.PerformClick();
        }

        internal List<ToolStrip> ToolStrips = new List<ToolStrip>();

        private void CmsNotifyIcon_AppExitRequested(object sender, EventArgs e)
        {
            Close();
        }

        internal NotifyIconContextMenu cmsNotifyIcon;

        private void TsDataGridZoom_SettingUpdateRequested(object sender, EventArgs e)
        {
        }

        private void TsListViewZoom_SettingUpdateRequested(object sender, EventArgs e)
        {
        }

        private void TsDataGridZoom_ZoomPercentChanged(object sender, EventArgs e)
        {
            MyDataFile.Settings.SetValue("GlobalDataGridZoomPercent", (decimal)tsDataGridZoom.tstb.Value);
        }

        private void TsListViewZoom_ZoomPercentChanged(object sender, EventArgs e)
        {
            MyDataFile.Settings.SetValue("GlobalZoomPercent", (decimal)tsListViewZoom.tstb.Value);
        }

        private void TsDataGridZoom_ValuePropagationRequested(object sender, EventArgs e)
        {
        }

        private void TsListViewZoom_ValuePropagationRequested(object sender, EventArgs e)
        {
        }

        private void MyDataFile_FileLoaded(object sender, EventArgs e)
        {
            ApplySettingsFromDataFile();
        }

        private void MyDataFile_FileClosed(object sender, FileClosedEventArgs e)
        {
            if (e.OtherFileWillBeOpened || MainFormIsBeingClosed)
            {
            }
            else
            {
                // until I find how to not create 2 ClockListView-s, I do this check below:
                if (MyDataFile.ClockVMCollection.Model.Ms.Count > 0)
                {
                    return;
                }
                MyDataFile.ClockVMCollection.Model.IsUnsavedLocked = true;
                MyDataFile.ClockVMCollection.Model.AddClock(new TimerData(MyDataFile, MyDataFile.MultiAudioPlayer));
                MyDataFile.ClockVMCollection.Model.IsUnsavedLocked = false;
            }
            ApplySettingsFromDataFile();
        }

        private void MyDataFile_IsDirtyChanged(object sender, EventArgs e)
        {
            OnFirstOpeningOrChange();
            UpdateFormTitle();
            UpdateTsmiNewEnabled();
        }

        protected void OnFirstOpeningOrChange()
        {
            if (Properties.Settings.Default.FirstStart)
            {
                Utils.SetStartProgramWithWindows(true);
            }
            lblFirstStart.Visible = false;
            Properties.Settings.Default.FirstStart = false;
            Properties.Settings.Default.Save();
        }

        private void MyDataFile_FilePathChanged(object sender, EventArgs e)
        {
            if (MainFormIsBeingClosed)
            {

            }
            else
            {
                OnFirstOpeningOrChange();
                UpdateFormTitle();
                UpdateTsmiNewEnabled();
                UpdateTsmiOpenContainingFolderEnabled();

                tsmiReloadOpenFileFromFileSystem.Enabled =
                    MyDataFile.FilePath != "";

                if (MyDataFile.FilePath != "")
                {
                    recentFilesMenuStrip1.InsertInRecentFilesList(MyDataFile.FilePath);
                }

                Properties.Settings.Default.LastOpenFile = MyDataFile.FilePath;
                Properties.Settings.Default.Save();
            }
        }

        private void UpdateTsmiOpenContainingFolderEnabled()
        {
            tsmiOpenContainingFolder.Enabled =
                MyDataFile.FilePath == "" ? false : true;
        }

        // TODO: [BIGGEST] save as setting the zoom levels of all advanced views.

        /// <summary>
        /// Methods called from this method use Suspend/ResumeLayoutRecursively
        /// for optimization.
        /// </summary>
        /// <param name="MyDataFile"></param>
        internal void ApplySettingsFromDataFile(DataFile MyDataFile = null)
        {
            if (MainFormIsBeingClosed)
            {
                return;
            }

            if (MyDataFile == null)
            {
                MyDataFile = this.MyDataFile;
            }

            ApplyWithoutSetting = true;

            // ...

            MyDataFile.Settings.IsUnsavedLocked = true;

            // ...

            // imi va aparea ca fisierul nu e salvat, deci aplica santinelele de mai sus inainte si dupa DataFile.LoadFromString
            
            PropagateBeepsDataTable(MyDataFile.GetBeepsDataTable());

            MyDataFile.ClockVMCollection.Model.IsUnsavedLocked = true;
            MyDataFile.ClockVMCollection.Model.IsUnsavedLocked = false;

            MyDataFile.Settings.IsUnsavedLocked = false;

            ApplyWithoutSetting = false;

            InitializeLastFocusedTimersView();
        }

        //private void ApplyScrollPercentsToControls()
        //{
        //}

        internal void ApplyEasyViewsSizesFromSettings(DataFile df)
        {
            MyClocksViewProvider.HandleSplitterMoved = false;
            
            Utils.SuspendLayoutRecursively(this);

            if (tsmiViewGroupListView.Checked)
            {
                //ApplyEasyViewGroupListWidthPercent(df);
                //ApplyEasyViewClockListViewSizePercent(df, true);
            }
            else
            {
                //ApplyEasyViewClockListViewSizePercent(df, false);
            }

            Utils.ResumeLayoutRecursively(this, true);

            MyClocksViewProvider.HandleSplitterMoved = true;
        }

        //private void ApplyEasyViewClockListViewSizePercent(DataFile df, bool inner)
        //{
        //    var percent = (float)df.Settings.GetValue("EasyViewClockListViewSizePercent");
        //    if (inner)
        //    {
        //        SplitterView rightPane = MyViewDataFactory.RootViewData.GetSecondChild()?.View as SplitterView;
        //        if (rightPane != null) // right pane is split
        //        {
        //            int w = (int)((float)rightPane.Width / 100F * percent);
        //            int h = (int)((float)rightPane.Height / 100F * percent);

        //            int ns = rightPane.Orientation == Orientation.Vertical ?
        //                w : h;

        //            SetSplitterDistance(rightPane, ns);
        //        }
        //    }
        //    else
        //    {
        //        SetSplitterDistance(MyClocksViewProvider.RootSplitterView, (int)(
        //            (MyClocksViewProvider.RootSplitterView.Orientation == Orientation.Horizontal ? MyClocksViewProvider.RootSplitterView.Height : MyClocksViewProvider.RootSplitterView.Width) /
        //            100F * percent));
        //    }
        //}

        //private void ApplyEasyViewGroupListWidthPercent(DataFile df)
        //{
        //    //var percent = (float)df.Settings.GetValue("EasyViewGroupListWidthPercent");
        //    //SetSplitterDistance(MyClocksViewProvider.RootSplitterView,
        //    //    (int)((float)MyClocksViewProvider.RootSplitterView.Width / 100F * percent));
        //}

        //internal static void SetSplitterDistance(SplitterView s, int d)
        //{
        //    if (d < s.Panel1MinSize)
        //    {
        //        d = s.Panel1MinSize;
        //    }
        //    else if (d > (s.Orientation == Orientation.Horizontal ?
        //        s.Height : s.Width) - s.Panel2MinSize)
        //    {
        //        d = (s.Orientation == Orientation.Horizontal ? s.Height : s.Width) - s.Panel2MinSize;
        //    }

        //    s.SplitterDistance = d;
        //}

        internal static void SetVerticalScrollPercentToControl(ClockFlowLayoutPanel clv, double percent)
        {
            //System.Windows.Controls.ScrollViewer sv = clv.MyScrollViewer;
            //double d = sv.ExtentHeight / 100D * percent;
            //if (double.IsNaN(d))
            //{
            //    return;
            //}
            //sv.ScrollToVerticalOffset(d);
        }

        internal static void SetVerticalScrollPercentToControl(ClockGroupListView tglv, double percent)
        {
            System.Windows.Controls.ScrollViewer sv = tglv.MyScrollViewer;
            double d = sv.ExtentHeight / 100D * percent;
            if (double.IsNaN(d))
            {
                return;
            }
            sv.ScrollToVerticalOffset(d);
        }

        internal static void SetVerticalScrollPercentToControl(ClockDataGrid c, double percent)
        {
            c.MyDataGrid.ApplyTemplate();
            DataGridScrollExtensions.ScrollInfo si = c.MyDataGrid.GetScrollInfo();
            c.MyDataGrid.SetScrollPosition(new DataGridScrollExtensions.ScrollInfo()
            {
                VerticalMaximum = si.VerticalMaximum,
                VerticalPosition = si.VerticalMaximum * percent / 100d
            });
        }

        internal static double GetVerticalScrollPercentFromControl(ClockGroupListView c)
        {
            return (double)c.MyScrollViewer.VerticalOffset * 100D /
                (double)c.MyScrollViewer.ExtentHeight;
        }

        internal static double GetVerticalScrollPercentFromControl(ClockFlowLayoutPanel c)
        {
            return (double)c.
                MyScrollViewer.VerticalOffset * 100D /
                    (double)c.
                MyScrollViewer.ExtentHeight;
        }

        internal static double GetVerticalScrollPercentFromControl(ClockDataGrid c)
        {
            c.MyDataGrid.ApplyTemplate();
            DataGridScrollExtensions.ScrollInfo si = c.MyDataGrid.GetScrollInfo();
            return si.VerticalPosition / si.VerticalMaximum * 100D;
        }

        internal void PropagateBeepsDataTable(string v)
        {
            foreach (ClockM cd in MyDataFile.ClockVMCollection.Model.Ms)
            {
                if (cd is TimerData td && td.Running)
                {
                    td.UpdateBeepTimers();
                }
            }
        }

        internal void ApplyFilter(Filter f = null)
        {
            if (f == null)
            {
                f = new Filter(MyDataFile.ClockVMCollection.Model, "")
                {
                    ShowActive = true,
                    ShowAlarms = true,
                    ShowInactive = true,
                    ShowTimers = true
                };
            }

            ClockGroupListView tglv = MyClocksViewProvider.GetExistingOrNewClockGroupListView();

            // TODO: use a Filter.Includes(Filter) method (if a filter displays all clocks inside the parameter Filter).
            foreach (FilterDisplay fd in tglv.MyFilters)
            {
                Filter ff = f.Clone();
                ff.SearchString = "";

                if (fd.MyFilter == ff)
                {
                    fd.IsSelected = true;
                }
                else
                {
                    fd.IsSelected = false;
                }
            }

            tglv.MySearchTextBox.Text = f.SearchString;

            MyDataFile.ClockVMCollection.Model.AppliedFilter = f;
        }

        private void PropagateGroupListZoomPercent()
        {
            //MyClocksViewProvider.GetExistingOrNewClockGroupListView().PropagateGlobalZoomFactor(MyClocksViewProvider.EasyClockGroupListViewZoomPercent / 100M);
            
            tsGroupListZoom.SetZoomPercentAsSystemIfValid(MyClocksViewProvider.EasyClockGroupListViewZoomPercent);
        }

        internal double LastTaskBarProgressPercent = -1;

        internal void SetTaskBarProgressPercent(double percent)
        {
            if (IsDisposed)
            {
                return;
            }

            LastTaskBarProgressPercent = percent;

            if (percent < 0 || percent == 0)
            {
                TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.NoProgress);
            }
            else
            {
                TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Normal);
                TaskbarProgress.SetValue(Handle, percent, 100D);
            }
        }

        //internal void DoMouseWheel(MouseEventArgs e)
        //{
        //    base.OnMouseWheel(e);
        //}

        // TODO: [VISUAL w/ another TODO] probably I need to subclass all controls capable of receiving mouse wheel events... and in their handlers call the form's wheel event handler.
        internal void MainForm_MouseWheel(object sender, MouseEventArgs e)
        {
            // OLD INCOMPLETE CODE:
            //if ((ModifierKeys & Keys.Control) ==
            //    Keys.Control)
            //{
            //    if (ZoomPercentIsValid(e.Delta * 10))
            //    {
            //        GlobalZoomFactor += e.Delta * 0.1F;
            //        PropagateGlobalZoomFactor();
            //        MyDataFile.SetValue("GlobalZoomPercent", GlobalZoomPercent); // also for "GlobalDataGridZoomPercent"
            //    }
            //}
        }
        
        internal void DragDropHandler(object sender, DragEventArgs e)
        {
            if (MyFocusedClocksViewProvider.FocusedClocksView is ClockFlowLayoutPanel tlv)
            {
                //tlv.HandleDargDrop(sender, e);
            }
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            // TODO: is this LOC necessary? On forced exit, the icon remains in the system tray,
            // the icon is not disposed well in this hndler. And on form close, it is disposed in the FormClosing handler well.
            notifyIcon1.Dispose();
        }

        internal bool MainFormIsBeingClosed = false;
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MainFormIsBeingClosed = true;
            
            Utils.SuspendLayoutRecursively(this);

            bool closed = CloseFile();

            MainFormIsBeingClosed = false;

            if (closed)
            {
                notifyIcon1.Dispose();
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
                Utils.ResumeLayoutRecursively(this, true);
            }
        }

        // TODO: optionally save full screen as setting in DataFile... future: dual-screen.
        private void cbFullScreen_CheckedChanged(object sender, EventArgs e)
        {
            // TODO: [MOFT] pastreaza WindowState anterior
            // intrarii in FullScreen.
            FormBorderStyle = cbFullScreen.Checked ?
                FormBorderStyle.None :
                FormBorderStyle.Sizable;
            WindowState = cbFullScreen.Checked ?
                FormWindowState.Maximized :
                FormWindowState.Normal;
            MyCustomStatusBar.ShowTimeDate = cbFullScreen.Checked;
            SizeGripStyle = cbFullScreen.Checked ?
                SizeGripStyle.Hide :
                SizeGripStyle.Show;

            if (MyCustomStatusBar.ShowTimeDate)
            {
                tableLayoutPanel1.RowStyles[2].Height = 50;

                MyCustomStatusBar.SysTimer.Enabled = cbFullScreen.Checked;
                MyCustomStatusBar.SysTimer.ForceUpdate();
            }
            else
            {
                tableLayoutPanel1.RowStyles[2].Height = 30;
            }
        }

        internal void PropagateGlobalZoomFactor()
        {
            MyClocksViewProvider.GetExistingOrNewClockListView().ZoomFactor =MyClocksViewProvider.EasyClockListViewZoomPercent / 100M;
            
            tsListViewZoom.SetZoomPercentAsSystemIfValid(MyClocksViewProvider.EasyClockListViewZoomPercent);
        }

        internal void PropagateDataGridGlobalZoomFactor()
        {
            MyClocksViewProvider.GetExistingOrNewClockDataGridView().ZoomFactor = MyClocksViewProvider.EasyClockDataGridViewZoomPercent / 100M;
            
            tsDataGridZoom.SetZoomPercentAsSystemIfValid(MyClocksViewProvider.EasyClockDataGridViewZoomPercent);
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            Activate();
        }

        internal void PropagateRoundedCornersSetting()
        {
        //    foreach (IClocksView v in MyClocksViews)
        //    {
        //        if (v is ClockFlowLayoutPanel clv)
        //        {
        //            clv.RoundedCorners = (bool)MyDataFile.GetValue("RoundedCorners");
        //        }
        //    }
        }

        /// <summary>
        /// When calling this, if the `RingingDuration` setting
        /// is zero, 1ms is used instead. The default sound is
        /// still hearable for a little when 1ms is used.
        /// </summary>
        internal void PropagateNewRingingDuration()
        {
            var ts = (TimeSpan)(MyDataFile.GetValue("RingingDuration"));

            // TODO: use a new Interval property directly inside
            // TimerData, not through t2, property which will
            // also accept 0. Also in another place where
            // td.t2.Interval is used. Not very important 
            // since the user
            // has the permanent or temporary Mute option.
            if (ts == TimeSpan.Zero)
            {
                ts = TimeSpan.FromMilliseconds(1);
            }

            // update effect of settings that changed
            foreach (ClockM td in MyDataFile.ClockVMCollection.Model.Ms)
            {
                td.t2.Interval = (int)(ts.TotalMilliseconds);
            }
        }

        internal bool IsLoadingSettingsFromFile = true;

        internal void cbDarkMode_CheckedChanged(object sender, EventArgs e)
        {
            if (ApplyWithoutSetting)
            {

            }
            else
            {
                MyDataFile.SetValue("DarkMode", cbDarkMode.Checked);
            }
        }

        internal void AdaptSubmenusToDarkTheme(
            ToolStripItem item)
        {
            if (!(item is ToolStripMenuItem tsmi))
            {
                return;
            }

            var d = (bool)MyDataFile.GetValue("DarkMode");

            foreach (object item2 in tsmi.DropDownItems)
            {
                var tsi = item2 as ToolStripItem;

                tsi.BackColor =
                    (d ? Utils.MyDarkGray : Color.White);
                tsi.ForeColor =
                    (d ? Color.White : Utils.MyDarkGray);

                AdaptSubmenusToDarkTheme(tsi);
            }
        }

        // TODO: [VISUAL] save toolbars visibility, position in file.

        /// <summary>
        /// d is true if dark mode is enabled.
        /// </summary>
        /// <param name="d"></param>
        private void AdaptMenuStripToDarkTheme(bool d)
        {
            menuStrip1.BackColor =
                (d ? Utils.MyDarkGray : Color.White);
            menuStrip1.Renderer = d ? DarkModeMenuStripRenderer : new ToolStripProfessionalRenderer();

            toolStripContainer1.LeftToolStripPanel.Renderer =
                toolStripContainer1.RightToolStripPanel.Renderer =
                toolStripContainer1.TopToolStripPanel.Renderer =
                toolStripContainer1.BottomToolStripPanel.Renderer =
                    menuStrip1.Renderer;

            foreach (ToolStrip ts in ToolStrips)
            {
                ts.Renderer = menuStrip1.Renderer;
            }


            foreach (ToolStripMenuItem item in menuStrip1.Items)
            {
                item.BackColor =
                    (d ? Utils.MyDarkGray : Color.White);
                item.ForeColor =
                    (d ? Color.White : Utils.MyDarkGray);

                AdaptSubmenusToDarkTheme(item);
            }
        }

        protected HelpWindow OpenHelpWindow = null;
        private void tsmiViewHelp_Click(object sender, EventArgs e)
        {
            // TODO: [VISUAL] make about & settings forms non-modal
            if (OpenHelpWindow == null)
            {
                OpenHelpWindow = new HelpWindow();
                ElementHost.EnableModelessKeyboardInterop(OpenHelpWindow);
            }
            OpenHelpWindow.Show();
            OpenHelpWindow.Activate();
        }

        internal void ForceUpdateOfDarkThemeSetting(bool d)
        {
            BackColor = d ? Color.Black :
                SystemColors.Control;
            ForeColor = d ? Color.Black :
                SystemColors.ControlText;

            cbFullScreen.ForeColor = d ? Color.LightGray : Color.Black;
            cbDarkMode.ForeColor = d ? Color.LightGray : Color.Black;

            MyCustomStatusBar.DarkTheme = d;

            //lblTimeDate.ForeColor = d ? Color.White : Color.Black;
            //lbl2.ForeColor = lblTimeDate.ForeColor;

            AdaptMenuStripToDarkTheme(d);

            //btnAbout.FlatStyle = d ? FlatStyle.Flat :
            //    FlatStyle.Standard;

            //tsbZoomIn.FlatStyle = btnAbout.FlatStyle;
            //tsbZoomOut.FlatStyle = btnAbout.FlatStyle;

            //btnAbout.FlatAppearance.BorderColor = Utils.MyDarkDarkGray;
            //tsbZoomIn.FlatAppearance.BorderColor = Utils.MyDarkDarkGray;
            //tsbZoomOut.FlatAppearance.BorderColor = Utils.MyDarkDarkGray;

            (elementHost1.Child as ViewsGrid).MyDataGrid.DarkTheme = d;

            //foreach (IClocksView sp in MyClocksViews)
            //{
            //    //if (sp is ClockListView tlv)
            //    //{
            //    //    // NOTE: useless recursion here when changing the dark theme checkbox checked state (see the end of the crt method)
            //    //    tlv.DarkMode = d;
            //    //    continue;
            //    //}

            //    if (sp is TimerGroupListView tglv)
            //    {
            //        tglv.DarkMode = d;
            //        continue;
            //    }
            //}

            //MySettingsForm.audioFileChooser1.DarkMode = d;
            foreach (ClockM td in MyDataFile.ClockVMCollection.Model.Ms)
            {
                foreach (object v in td.MyTimerViews)
                {
                //    if (v is ClockControl tc)
                //    {
                //        tc.DarkMode = d;
                //    }
                }
            }

            if (IsLoadingSettingsFromFile && ApplyWithoutSetting)
            {
                cbDarkMode.Checked = d;
            }
        }

        /// <summary>
        /// Returns whether save operation succeeded.
        /// </summary>
        /// <param name="fn"></param>
        /// <returns></returns>
        internal bool SaveAs(string fn = "")
        {
            if (fn != "")
            {
                MyDataFile.Save();
                return true;
            }

            // TODO: memoize in Utils.cs, use ~GetSaveFileChooser
            saveFileDialog1.FileName = $"TimedSilver {DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss")}.xml";
            saveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                MyDataFile.SaveAs(saveFileDialog1.FileName);
                return true;
            }

            return false;
        }

        private void tsmiSelectiveOpen_Click(object sender, EventArgs e)
        {
            var imw = new ImportWindow(this);
            ElementHost.EnableModelessKeyboardInterop(imw);
            imw.Show();
        }
        
        internal bool ApplyWithoutSetting = false;
        private void tsmiKeepOnTopAlways_Click(object sender, EventArgs e)
        {
            if (tsmiKeepOnTopAlways.Checked)
            {
                if (!ApplyWithoutSetting)
                    MyDataFile.SetValue("MainFormAlwaysOnTop", true);
            }
            else
            {
                if (!ApplyWithoutSetting)
                    MyDataFile.SetValue("MainFormAlwaysOnTop", false);
            }
        }

        internal void SetKeepOnTopAlways(bool b)
        {
            if (b)
            {
                tsmiKeepOnTopUntilNextRestart.Checked = false;
            }

            TopMost = b;

            tsmiKeepOnTopAlways.Checked = b;
        }

        private void tsmiKeepOnTopUntilNextRestart_Click(object sender, EventArgs e)
        {
            if (tsmiKeepOnTopUntilNextRestart.Checked)
            {
                tsmiKeepOnTopAlways.Checked = false;

                TopMost = true;
            }
            else
            {
                TopMost = false;
            }
        }

        private void tsmiSaveAs_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        private void tsmiOpenFile_Click(object sender, EventArgs e)
        {
            Utils.SuspendLayoutRecursively(this);

            if (MyDataFile.FilePath != "" || MyDataFile.IsUnsaved)
            {
                MyDataFile.CloseAndOpenOtherFile();
            }
            else
            {
                MyDataFile.LoadFromFile();
            }
            
            Utils.ResumeLayoutRecursively(this, true);
        }

        private void tsmiSave_Click(object sender, EventArgs e)
        {
            Save();
        }

        public bool Save()
        {
            if (MyDataFile.FilePath.Length == 0)
            {
                return SaveAs();
            }
            else
            {
                return SaveAs(MyDataFile.FilePath);
            }
        }

        internal bool IsFileBeingClosed = false;

        internal bool CloseFile(bool withConfirmationRequest = true)
        {
            IsFileBeingClosed = true;
            if (!withConfirmationRequest)
            {
                MyDataFile.Close();
                IsFileBeingClosed = false;
                return true;
            }

            if (MyDataFile.IsUnsaved)
            {
                DialogResult r = MessageBox.Show(this, "There are unsaved changes. Do you want to save the changes before closing?", "Confirmation request", MessageBoxButtons.YesNoCancel);
                if (r == DialogResult.No)
                {
                    MyDataFile.Close();
                    IsFileBeingClosed = false;
                    return true;
                }
                else if (r == DialogResult.Yes)
                {
                    if (Save())
                    {
                        MyDataFile.Close();
                        IsFileBeingClosed = false;
                        return true;
                    }
                }
                IsFileBeingClosed = false;
                return false;
                //MyDataFile.Close();
            }
            else
            {
                MyDataFile.Close();
                IsFileBeingClosed = false;
                return true;
            }
        }

        private void tsmiClose_Click(object sender, EventArgs e)
        {
            CloseFile();
        }

        private void tsmiExit_Click(object sender, EventArgs e)
        {
            MainFormIsBeingClosed = true;
            if (CloseFile())
            {
                MainFormIsBeingClosed = false;
                Close();
                return;
            }
            MainFormIsBeingClosed = false;
        }

        private void tsmiRemoveAllTimers_Click(object sender, EventArgs e)
        {
            int c = MyDataFile.ClockVMCollection.Model.Ms.Count;
            if (c > 0)
            {
                if (MessageBox.Show(this, $"Are you sure you want to delete all {c} existing clocks?", "Confirmation request", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    MyDataFile.ClockVMCollection.Model.RemoveAllClocks();
                    MyCustomStatusBar.PostMessage($"All {c} existing clocks have been deleted.",
                        LogCategory.Information);
                }
            }
            else
            {
                MyCustomStatusBar.PostMessage("There aren't any clocks to delete.",
                    LogCategory.Information);
            }
        }

        internal void ApplyAndSetEasyView(EasyViewType v)
        {
            MyDataFile.SetValue("ViewType", v.ToString());
        }

        private void tsmiViewDataGridView_Click(object sender, EventArgs e)
        {
            if (!tsmiDataGridView.Checked)
            {
                tsmiDataGridView.Checked = !tsmiDataGridView.Checked;
                return;
            }

            ApplyAndSetEasyView(EasyViewType.DataGrid);

            tsmiListView.Checked = tsmiHorizontalSplitView.Checked =
                tsmiVerticalSplitView.Checked = false;
        }

        private void tsmiViewListView_Click(object sender, EventArgs e)
        {
            if (!tsmiListView.Checked)
            {
                tsmiListView.Checked = !tsmiListView.Checked;
                return;
            }

            ApplyAndSetEasyView(EasyViewType.List);

            tsmiDataGridView.Checked = tsmiHorizontalSplitView.Checked =
                tsmiVerticalSplitView.Checked = false;
        }

        private void tsmiHorizontalSplitView_Click(object sender, EventArgs e)
        {
            if (!tsmiHorizontalSplitView.Checked)
            {
                tsmiHorizontalSplitView.Checked = !tsmiHorizontalSplitView.Checked;
                return;
            }

            ApplyAndSetEasyView(EasyViewType.HorizontalSplit);

            tsmiDataGridView.Checked = tsmiListView.Checked =
                tsmiVerticalSplitView.Checked = false;
        }

        private void tsmiVerticalSplitView_Click(object sender, EventArgs e)
        {
            if (!tsmiVerticalSplitView.Checked)
            {
                tsmiVerticalSplitView.Checked = !tsmiVerticalSplitView.Checked;
                return;
            }

            ApplyAndSetEasyView(EasyViewType.VerticalSplit);

            tsmiDataGridView.Checked = tsmiListView.Checked =
                tsmiHorizontalSplitView.Checked = false;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // when this condition is false, the ForceUpdateOfDarkThemeSetting method
            // requests a member of a variable that is null.
            if (OnLoadCreateBasicViewDataWithView)
            {
                //MyViewDataFactory.RootViewData = new ViewData(MyDataFile)
                //{
                //    ViewType = ViewDataType.Splitter
                //};

                //UpdateRootSplitterViewControl();

                //ClockFlowLayoutPanel clv = MyClocksViewProvider.GetExistingOrNewClockListView();

                //RootViewData.SetFirstChild(new ViewData(MyDataFile)
                //{
                //    ViewType = ViewDataType.List,
                //    View = clv
                //});
                //clv.MyViewData = RootViewData.GetFirstChild();
                
                //RootViewData.RecreateThisView();
                //MyViewDataFactory.SetRootViewDataViewPointer(MyClocksViewProvider);

                //RootSplitterViewPointer.Value = SplitterView.BuildFromViewData(this, RootViewData) as SplitterView;

                //SplitterView.SubscribeViewData(RootViewData, RootViewData.View);

                //UpdateRootSplitterViewControl();

                //MyClocksViewProvider.SubscribeSplitterMovedHandlers();

                ForceUpdateOfDarkThemeSetting(cbDarkMode.Checked);
            }


            MyDataFile.Settings.IsUnsavedLocked = true;
            MyDataFile.ClockVMCollection.Model.IsUnsavedLocked = true;

            foreach (KeyValuePair<string, SettingDataM> k in MyDataFile.Settings)
            {
                ApplySetting(k.Value);
            }

            MyDataFile.Settings.IsUnsavedLocked = false;
            MyDataFile.ClockVMCollection.Model.IsUnsavedLocked = false;

            bool cliOpenLast = Program.DefaultProgram != null ?
                Program.DefaultProgram.MyArgv.Contains("--last") : false;
            bool cliOpenNew = Program.DefaultProgram != null ?
                Program.DefaultProgram.MyArgv.Contains("--new") : false;

            bool hasCliFlag = cliOpenLast || cliOpenNew;

            if (hasCliFlag)
            {
                // respect the flag more than the setting
                if (cliOpenLast)
                {
                    LoadLastOpenFile();
                }
                else // if cliOpenNew
                {
                    LoadNewEmptyFile();
                }
            }
            else
            {
                if (Properties.Settings.Default.LastOpenFile != "" &&
                    Properties.Settings.Default.AutoOpenLastFile == "Yes")
                {
                    LoadLastOpenFile();
                }
                else if (Properties.Settings.Default.AutoOpenLastFile == "Ask" &&
                         Properties.Settings.Default.LastOpenFile != "")
                {
                    DialogResult dr = MessageBox.Show(this, $"Do you wish to open the last opened Timed Silver file, {Utils.BaseFileNameInPath(Properties.Settings.Default.LastOpenFile)}? (Press No to open a new empty file, or Cancel to close the program.)\n\nFull path: {Properties.Settings.Default.LastOpenFile}", "Confirmation request",
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                    if (dr == DialogResult.Yes)
                    {
                        LoadLastOpenFile();
                    }
                    else if (dr == DialogResult.No)
                    {
                        LoadNewEmptyFile();
                    }
                    else // Cancel
                    {
                        Close();
                        return;
                    }
                }
                else // "No"
                {
                    LoadNewEmptyFile();
                }
            }

            notifyIcon1.DoubleClick += NotifyIcon1_DoubleClick;
            notifyIcon1.MouseClick += NotifyIcon1_MouseClick;
            cmsNotifyIcon = new NotifyIconContextMenu(MyDataFile, notifyIcon1); 
            cmsNotifyIcon.AppExitRequested += CmsNotifyIcon_AppExitRequested;

            MyCustomStatusBar.PostMessage("Program successfully started.", LogCategory.Information);

            IsBeingLoaded = false;
        }

        private void NotifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                cmsNotifyIcon.Show();
            }
        }

        private void NotifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            Activate();
        }

        internal void LoadNewEmptyFile()
        {
            MyDataFile.Settings.IsUnsavedLocked = true;
            MyDataFile.ClockVMCollection.Model.IsUnsavedLocked = true;
            
            MyDataFile.ClockVMCollection.Model.AddClock(new TimerData(MyDataFile, MyDataFile.MultiAudioPlayer));

            MyDataFile.Settings.IsUnsavedLocked = false;
            MyDataFile.ClockVMCollection.Model.IsUnsavedLocked = false;

            ApplySettingsFromDataFile();
        }

        internal void LoadLastOpenFile()
        {
            MyDataFile.LoadFromFile(Properties.Settings.Default.LastOpenFile);
        }

        internal void InitializeLastFocusedTimersView()
        {
            //LastFocusedTimersView = RootViewData?.GetFirstChild().View as IClocksView;

            //tsGlobalZoom.TimersView = LastFocusedTimersView;
        }
        
        //internal void UpdateRootSplitterViewControl(ViewData vd = null)
        //{
        //    //const string key = "RootSplitterView";
        //    //SplitterView sv;
        //    //if (vd != null)
        //    //{
        //        //sv = vd.View as SplitterView;
        //        //tableLayoutPanel1.Controls.RemoveByKey(key);
        //        //sv.Name = key;
        //        //tableLayoutPanel1.Controls.Add(sv, 0, 1);
        //        //RootSplitterViewPointer.Value = sv;
        //    //}   
        //    //else
        //    //{
        //        //sv = RootSplitterViewPointer.Value as SplitterView;
        //        //if (sv == null)
        //        //{
        //            // not implemented, not understood situation
        //        //    return;
        //        //}
        //    //    tableLayoutPanel1.Controls.RemoveByKey(key);
        //    //    sv.Name = key;
        //    //    tableLayoutPanel1.Controls.Add(sv, 0, 1);
        //    //}
        //}

        private void tsmiSettings_Click(object sender, EventArgs e)
        {
            MySettingsForm.ShowDialog(this);
        }

        internal void tsmiFocusListView_Click(object sender, EventArgs e)
        {
            //if (MyClocksViewProvider.GetExistingOrNewClockListView() != null)
            //{
            //    MyClocksViewProvider.GetExistingOrNewClockListView().HasFocus = true;
            //}
        }

        private void tsmiFocusDataGridView_Click(object sender, EventArgs e)
        {
            //if (!Utils.IsNullOrDisposed(MyClocksViewProvider.GetExistingOrNewClockDataGridView()))
            //{
            //    MyClocksViewProvider.GetExistingOrNewClockDataGridView().HasFocus = true;
            //}
        }

        private void tsmiListViewZoomToolbar_Click(object sender, EventArgs e)
        {
            tsListViewZoom.Visible = tsmiListViewZoomToolbar.Checked;
        }

        private void tsmiDataGridZoomToolbar_Click(object sender, EventArgs e)
        {
            tsDataGridZoom.Visible = tsmiDataGridZoomToolbar.Checked;
        }

        // TODO: move this method in Filter class.
        internal string GetFirstGroupNameInFilter(Filter f)
        {
            return f.GroupIndices.Count == 0 ?
                "" : MyDataFile.ClockVMCollection.Model.Groups.Ms[
                        f.GroupIndices[0] - 1
                    ].Name;
        }

        internal void TsEdit_NewTimerRequested(object sender, EventArgs e)
        {
            var td = new TimerData(MyDataFile, MyDataFile.MultiAudioPlayer);

            td.GroupName = GetFirstGroupNameInFilter(MyDataFile.ClockVMCollection.Model.AppliedFilter);
            td.Checkable = GetIsCheckable();

            MyDataFile.ClockVMCollection.Model.AddClock(td);
        }

        private bool GetIsCheckable()
        {
            return viewsGrid1.MyFlowView.MultiSelectClocks;
        }

        internal void TsEdit_NewAlarmRequested(object sender, EventArgs e)
        {
            var td = new AlarmData(MyDataFile, MyDataFile.MultiAudioPlayer);

            td.GroupName = GetFirstGroupNameInFilter(MyDataFile.ClockVMCollection.Model.AppliedFilter);
            td.Checkable = GetIsCheckable();

            MyDataFile.ClockVMCollection.Model.AddClock(td);
        }

        private void tscbAutomaticSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tscbAutomaticSort.SelectedIndex)
            {
                case 0: // no automatic sort
                    MyDataFile.ClockVMCollection.Model.AutosortMode = AutosortMode.None;
                    break;

                case 1: // by closest ringing moment
                    MyDataFile.ClockVMCollection.Model.AutosortMode = AutosortMode.ClosestRingingMoment;
                    // code to apply selected sort once (autosort is done inside MyDataFile):
                    MyDataFile.ClockVMCollection.Model.DoSortByClosestRingingMoment();
                    break;

                case 2: // alphabetically
                    MyDataFile.ClockVMCollection.Model.AutosortMode = AutosortMode.Alphabetically;
                    // code to apply selected sort once (autosort is done inside MyDataFile):
                    MyDataFile.ClockVMCollection.Model.DoSortAlphabetically();
                    break;
            }
        }

        internal void ApplyAutosortSetting()
        {
            switch (MyDataFile.ClockVMCollection.Model.AutosortMode)
            {
                case AutosortMode.None:
                    tscbAutomaticSort.SelectedIndex = 0;
                    break;

                case AutosortMode.ClosestRingingMoment:
                    tscbAutomaticSort.SelectedIndex = 1;
                    MyDataFile.ClockVMCollection.Model.DoSortByClosestRingingMoment();
                    break;

                case AutosortMode.Alphabetically:
                    tscbAutomaticSort.SelectedIndex = 2;
                    MyDataFile.ClockVMCollection.Model.DoSortAlphabetically();
                    break;
            }
        }

        private void tsmiAbout_Click(object sender, EventArgs e)
        {
            // TODO: [VISUAL]  non-modal
            var w = new AboutWindow();
            ElementHost.EnableModelessKeyboardInterop(w);
            w.ShowDialog();
        }

        private void tsmiRemoveSplit_Click(object sender, EventArgs e)
        {
            // TODO: [BIGGEST] repair this type of foreachs.
            // Status	Code	File	Line	Column	Project
            // foreach (ISplitterPanel sp in splitterView1) E:\Lucru\cs - timed - silver\cs - timeout\MainForm.cs 1355    43  cs - timed - silver

            //LastFocusedTimersView.MyViewData.RemoveSplit();
        }

        internal Control GetFocusedSplitterPanel()
        {
            //Control lastWithFocus = null;
            //foreach (ISplitterPanel sp in RootSplitterView)
            //{
            //    var c = sp as Control;
            //    if (sp.HasFocus)
            //    {
            //        lastWithFocus = c;
            //    }
            //}
            //// NOTE: teh followin comment is idea for the GetFocusedTImersView.
            ////if (lastWithFocus is SplitterView)
            ////{
            ////    lastWithFocus = (lastWithFocus as SplitterView).MyViewData.GetViews()[0] as Control;
            ////}
            //return lastWithFocus == null ? (Control)RootSplitterView.FirstControl :
            //    lastWithFocus;
            return null;
        }

        internal void DoSplit(Orientation o)
        {
            //var tsv = LastFocusedTimersView as Control;
            //if (tsv != null)
            //{
            //    ViewData vd = (tsv as ISplitterPanel).MyViewData;
            //    //vd.View = LastFocusedTimersView;

            //    //vd.View = vd.PreviousView;
            //    //MyDataFile.MainForm.RootViewData.GetFirstChild().View = vd.View;
            //    vd.Split(o);

            //    RootViewData.FlattenOneLevel();
            //}
        }

        private void tsmiSplitViewHorizontally_Click(object sender, EventArgs e)
        {
            DoSplit(Orientation.Horizontal);
        }

        private void tsmiSplitViewVertically_Click(object sender, EventArgs e)
        {
            DoSplit(Orientation.Vertical);
        }
        
        [Obsolete]
        internal decimal GroupListZoomPercent
        {
            get
            {
                return MyClocksViewProvider.EasyClockGroupListViewZoomPercent;
            }
            set
            {
                MyClocksViewProvider.EasyClockGroupListViewZoomPercent = value;
            }
        }

        #region FormShown property implementation

        internal bool FormShown { get; set; } = false;

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            FormShown = true;
        }

        #endregion

        // TODO: [BIGGEST] remake following few methods
        internal void ReplaceFocusedViewWith(ISplitterPanel c)
        {
            //if (LastFocusedTimersView is Control tsv)
            //{
            //    // NOTE: trying the advanced views mode makes sometimes empty panels which when focused, 
            //    // trigger an ex. here because Parent does not exist (the 1st Parent below, but it is focused since GetFocusedTimersViews is called above).
            //    SplitterView sp = SplitterView.GetContainerSplitterView(tsv);

            //    if (sp != null)
            //    {
            //        if (sp.Panel1.ContainsFocus)
            //        {
            //            sp.SetFirstControl(c);
            //        }
            //        else if (sp.Panel2.ContainsFocus)
            //        {
            //            sp.SetSecondControl(c);
            //        }
            //    }
            //}
        }

        private void tsmiReplaceWithListView_Click(object sender, EventArgs e)
        {
            //ReplaceFocusedViewWith(MyClocksViewProvider.CreateNewTimerListView());
        }

        private void tsmiReplaceWithDataGrid_Click(object sender, EventArgs e)
        {
            // TODO: replace with working WPF:
            //ReplaceFocusedViewWith(MyClocksViewProvider.CreateNewTimerDataGridView());
        }

        private void tsmiReplaceWithGroupList_Click(object sender, EventArgs e)
        {
            //ReplaceFocusedViewWith(MyClocksViewProvider.CreateNewGroupListView());
        }

        internal void SetAdvancedViewsEnabled(bool e)
        {
            tsmiSplitViewHorizontally.Visible = tsmiSplitViewVertically.Visible =
                tsmiRemoveSplit.Visible = tsmiReplaceViewWith.Visible = tss6.Visible =
                tsmiFocusedSplitViewOrientation.Visible = e;

            tsmiFocusListView.Visible = tsmiFocusDataGridView.Visible = tsmiVerticalSplitView.Visible =
                tsmiHorizontalSplitView.Visible = tsmiListView.Visible = tsmiDataGridView.Visible =
                tsmiToolbars.Visible = tss4.Visible = tss5.Visible = tsmiViewGroupListView.Visible =
                    tss3.Visible =
                    !e;

            if (e)
            {
                tsmiViewGroupListView.Checked = false;
            }

            tsmiUseAdvancedViews.Checked = e;
        }

        private void tsmiUseAdvancedViews_Click(object sender, EventArgs e)
        {
            if (tsmiUseAdvancedViews.Checked)
            {
                if (MessageBox.Show(this, "Are you sure you want to switch to advanced views with groups? This is unstable and disables zoom. You may need to restart the program.", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    SetAdvancedViewsEnabled(tsmiUseAdvancedViews.Checked);
                }
                else
                {
                    SetAdvancedViewsEnabled(!tsmiUseAdvancedViews.Checked);
                }
            }
            else
            {
                SetAdvancedViewsEnabled(tsmiUseAdvancedViews.Checked);
            }
        }

        internal void tsmiViewGroupListView_Click(object sender, EventArgs e)
        {
            MyClocksViewProvider.ToggleGroupListView();
        }

        internal void SetOrientationOfFocusedSplitterView(Orientation o)
        {
            var tsv = LastFocusedTimersView as Control;
            if (tsv != null)
            {
                //var sp = SplitterView.GetContainerSplitterView(tsv);

                //if (sp != null)
                //{
                //    sp.MyViewData.Orientation = o;
                //}
            }
        }

        // TODO: [BIGGEST] when opening the Orientation menu in View, show current orientation checked. When LastFocusedTimersView changes (there is an On... handler) update the
        // checked menu item.
        private void tsmiHorizontalOrientation_Click(object sender, EventArgs e)
        {
            //    if (!tsmiHorizontalOrientation.Checked)
            //    {
            //        tsmiHorizontalOrientation.Checked = true;
            //    }
            //    if (tsmiVerticalOrientation.Checked)
            //    {
            //        tsmiVerticalOrientation.Checked = false;
            //    }

            SetOrientationOfFocusedSplitterView(Orientation.Horizontal);
        }

        private void tsmiVerticalOrientation_Click(object sender, EventArgs e)
        {
            //if (tsmiHorizontalOrientation.Checked)
            //{
            //    tsmiHorizontalOrientation.Checked = false;
            //}
            //if (!tsmiVerticalOrientation.Checked)
            //{
            //    tsmiVerticalOrientation.Checked = true;
            //}

            SetOrientationOfFocusedSplitterView(Orientation.Vertical);
        }

        private void tsmiOpenContainingFolder_Click(object sender, EventArgs e)
        {
            Utils.SelectFileInItsFolderInExplorer(MyDataFile.FilePath, this);
        }

        #region WinAPI usage
        // To make a ToolStripContainer immediately clickable without clicking on the form first
        // BUG: simulates double click on content inside ContentPanel. If I replace
        // "is ToolStripContainer" with just "ToolStrip", it does not work at all..
        //[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        //public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        //private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        //private const int MOUSEEVENTF_LEFTUP = 0x04;
        //private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        //private const int MOUSEEVENTF_RIGHTUP = 0x10;

        //private const int WM_PARENTNOTIFY = 0x210;
        //private const int WM_LBUTTONDOWN = 0x201;

        //protected override void WndProc(ref Message m)
        //{
        //    if (m.Msg == WM_PARENTNOTIFY)
        //    {
        //        if (m.WParam.ToInt32() == WM_LBUTTONDOWN && ActiveForm != this)
        //        {
        //            Point p = PointToClient(Cursor.Position);
        //            if (GetChildAtPoint(p) is ToolStripContainer)
        //                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)p.X, (uint)p.Y, 0, 0);
        //        }
        //    }
        //    base.WndProc(ref m);
        //}

        const int WM_SYSCOMMAND = 0x0112;
        const int SC_MAXIMIZE = 0xF030;
        const int SC_MINIMIZE = 0xF020;
        const int SC_RESTORE = 0xF120;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_SYSCOMMAND)
            {
                if (m.WParam == (IntPtr)SC_MAXIMIZE ||
                    m.WParam == (IntPtr)SC_MINIMIZE ||
                    m.WParam == (IntPtr)SC_RESTORE)
                {
                    // the window has been maximized
                    this.OnResizeEnd(EventArgs.Empty);
                }
            }
        }
        #endregion

        private void tsmiGroupListZoom_Click(object sender, EventArgs e)
        {
            tsGroupListZoom.Visible = tsmiGroupListZoom.Checked;
        }

        private void tsmiGlobalZoom_Click(object sender, EventArgs e)
        {
            tsGlobalZoom.Visible = tsmiGlobalZoom.Checked;
        }

        bool changeByUser_MultipleSelection = true;
        private void tsmiMultipleSelection_CheckedChanged(object sender, EventArgs e)
        {
            if (!changeByUser_MultipleSelection)
            {
                return;
            }

            bool chk = tsmiMultipleSelection.Checked;

            bool changed = true;
            //foreach (IClocksView v in MyClocksViews)
            //{
            //    if (v is ClockFlowLayoutPanel clv && clv.Visible)
            //    {
            //        changed = true;
            //        break;
            //    }
            //}

            if (changed)
            {
                viewsGrid1.MyFlowView.MultiSelectClocks = !viewsGrid1.MyFlowView.MultiSelectClocks;
                //foreach (IClocksView v in MyClocksViews)
                //{
                //    //v.MultiSelectClocks = chk;
                //}
            }
            else // if (!changed)
            {
                changeByUser_MultipleSelection = false;
                tsmiMultipleSelection.Checked = false;
                changeByUser_MultipleSelection = true;

                MessageBox.Show(this, "Multiple selection with common menu is available only in list view. Please see the View menu.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void tsmiFile_DropDownOpening(object sender, EventArgs e)
        {
            tsmiRestoreToPreviousDay.Enabled = MyDataFile.MyDailyBackupSystem.BackupExists;
        }

        private void tsmiRestoreToPreviousDay_Click(object sender, EventArgs e)
        {
            if (MyDataFile.MyDailyBackupSystem.BackupExists)
            {
                MyDataFile.MyDailyBackupSystem.RestoreBackup();
            }
        }

        private void tsmiFocusGroupListView_Click(object sender, EventArgs e)
        {
            //if (!Utils.IsNullOrDisposed(MyClocksViewProvider.GetExistingOrNewClockGroupListView()))
            //{
            //    MyClocksViewProvider.GetExistingOrNewClockGroupListView().HasFocus = true;
            //}
        }

        private void TsmiClearOpenFile_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Are you sure you want to clear (reset) the open file?", "Confirmation request", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                MyDataFile.ResetAll();
            }
        }

        private void TsmiReloadOpenFileFromFileSystem_Click(object sender, EventArgs e)
        {
            MyDataFile.ReloadOpenFileFromFileSystem();
        }

        private void TsmiStatusBar_Click(object sender, EventArgs e)
        {
            MyDataFile.Settings.SetValue("ShowGlobalStatusBar", tsmiStatusBar.Checked);
        }
    }
}
