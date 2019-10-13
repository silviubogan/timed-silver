using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;

namespace cs_timed_silver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ISaveable
    {
        internal JumpList MyJumpList;

        internal bool IsFileBeingClosed { get; set; } = false;
        internal bool IsLoadingSettingsFromFile { get; set; } = false;
        internal bool ApplyWithoutSetting { get; set; } = false;
        internal bool MainFormIsBeingClosed { get; set; } = false;
        internal bool IsBeingLoaded { get; set; } = true;

        internal TimeOutFormsManager MyTimeOutFormsManager;
        //internal NotifyIconContextMenu MyNotifyIconMenu;
        internal NotifyIconPopup MyNotifyIconPopup;

        internal SettingsWindow MySettingsWindow { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MyNotifyIconMenu_AppExitRequested(object sender, EventArgs e)
        {
            Close();
        }

        private void MyNotifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //if (e.Button == System.Windows.Forms.MouseButtons.Right)
            //{
                // For focusing inside the "popup" (a Window, actually)?:
                //System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(MyNotifyIconPopup);

                //MyNotifyIconPopup.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                MyNotifyIconPopup.Left = SystemParameters.PrimaryScreenWidth - MyNotifyIconPopup.Width;
                MyNotifyIconPopup.Top = SystemParameters.WorkArea.Height - MyNotifyIconPopup.Height;
                MyNotifyIconPopup.Show();
                MyNotifyIconPopup.Activate();
                //MyNotifyIconMenu.Show();
            //}
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainFormIsBeingClosed = true;

            bool closed = CloseFile();

            MainFormIsBeingClosed = false;

            if (closed)
            {
                // TODO: (comment from WinForms version) is this LOC necessary? On forced exit, the icon remains in the system tray,
                // the icon is not disposed well in this hndler. And on form close, it is disposed in the FormClosing handler well.
                MyNotifyIcon.Dispose();

                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
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

        internal void SetTaskBarProgressPercent(double percent)
        {
            //if (IsDisposed)
            //{
            //    return;
            //}

            if (percent < 0 || percent == 0)
            {
                MyTaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;
            }
            else
            {
                MyTaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
                MyTaskbarItemInfo.ProgressValue = percent / 100d;
            }
        }

        private void Settings_SettingValueChange(object sender, SettingValueChangedEventArgs e)
        {
            var sd = sender as SettingDataM;
            ApplySetting(sd);
        }

        /// <summary>
        /// When calling this, if the `RingingDuration` setting
        /// is zero, 1ms is used instead. The default sound is
        /// still hearable for a little when 1ms is used.
        /// </summary>
        internal void PropagateNewRingingDuration()
        {
            var ts = (TimeSpan)(VM.Settings.GetValue("RingingDuration"));

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
            foreach (ClockM td in VM.ClockVMCollection.Model.Ms)
            {
                td.t2.Interval = ts;
            }
        }

        internal void ApplySetting(SettingDataM sd)
        {
            switch (sd.Name)
            {
                case "ZoomGroupListToolStrip":
                    // TODO: what should be put here? Test.
                    break;

                case "AudioFilePath":
                    VM.MultiAudioPlayer.AudioFilePath = (string)sd.Value;//MySettingsForm.LastAudioFilePath;
                    break;

                case "RingingDuration":
                    PropagateNewRingingDuration();
                    break;

                case "RoundedCorners":
                    MyViewsGrid.MyFlowView.RoundedCorners = (bool)sd.Value;
                    break;

                case "AlwaysMute":
                    VM.MultiAudioPlayer.Mute = (bool)VM.Settings.GetValue("AlwaysMute");
                    break;

                case "RecentAudioFilePaths":
                    MySettingsWindow.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        MySettingsWindow.ApplyTemplate();
                        MySettingsWindow.MyControl.MyRecentAudioFilesContextMenu.
                            LoadRecentPathsFromString(VM.Settings.GetValue("RecentAudioFilePaths") as string);
                    }), System.Windows.Threading.DispatcherPriority.Loaded);
                    break;

                case "RecentImageFilePaths":
                    MySettingsWindow.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        MySettingsWindow.ApplyTemplate();
                        MySettingsWindow.MyControl.MyRecentImageFilesContextMenu.
                            LoadRecentPathsFromString(
                                VM.Settings.GetValue("RecentImageFilePaths") as string);
                    }), System.Windows.Threading.DispatcherPriority.Loaded);
                    break;

                case "MainFormAlwaysOnTop":
                    SetKeepOnTopAlways((bool)sd.Value ? KeepOnTopWindowMode.Always : KeepOnTopWindowMode.None);
                    break;

                case "MainFormWindowState":
                    WindowState s = Utils.StrToEnum<WindowState>((string)VM.Settings.GetValue("MainFormWindowState"));
                    WindowState = s;
                    break;

                case "MainFormRectangle":
                    object objFormR = VM.Settings.GetValue("MainFormRectangle");
                    if (objFormR != null)
                    {
                        var r = (System.Drawing.Rectangle)objFormR;
                        Left = r.Left;
                        Top = r.Top;
                        Width = r.Width;
                        Height = r.Height;
                    }
                    break;

                case "ShowGroupListView":
                    ToggleShowGroupListView(sd);
                    break;

                case "ViewType":
                    ApplyViewType();
                    break;

                case "DarkMode":
                    bool v = (bool)VM.Settings.GetValue("DarkMode");

                    if (IsLoadingSettingsFromFile && ApplyWithoutSetting)
                    {
                        CbDarkTheme.IsChecked = v;
                    }

                    MyViewsGrid.MyDataGrid.DarkTheme = v;
                    MyViewsGrid.MyFlowView.DarkTheme = v;

                    (App.Current as App).LoadTheme(v ? AppTheme.Dark : AppTheme.Light);
                    break;

                case "GlobalZoomPercent":
                    MyViewsGrid.MyFlowView.ZoomFactor =
                        (decimal)VM.Settings.GetValue("GlobalZoomPercent") / 100M;
                    break;

                case "GlobalDataGridZoomPercent":
                    MyViewsGrid.MyDataGrid.ZoomFactor =
                        (decimal)VM.Settings.GetValue("GlobalDataGridZoomPercent") / 100M;
                    break;

                case "GroupListListViewViewType":
                    ClockGroupListView eglv = MyViewsGrid.MyClockGroupListView;
                    string view = VM.Settings.GetValue("GroupListListViewViewType").ToString();
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
                    MyViewsGrid.MyClockGroupListView.ZoomFactor =
                        (decimal)VM.Settings.GetValue("GroupListZoomPercent") / 100M;
                    break;

                case "GroupListToolBarsZoomPercent":
                    MyViewsGrid.MyClockGroupListView.ZoomFactor =
                        (decimal)VM.Settings.GetValue("GroupListToolBarsZoomPercent") / 100M;
                    break;

                case "AutosortMode":
                    if (!string.IsNullOrEmpty((string)VM.Settings.GetValue("AutosortMode")))
                    {
                        VM.ClockVMCollection.Model.AutosortMode = Utils.StrToEnum<AutosortMode>(
                            (string)VM.Settings.GetValue("AutosortMode"));
                    }
                    ApplyAutosortSetting();
                    break;

                case "Filter":
                    ApplyFilter(new FilterM(
                        VM.ClockVMCollection.Model,
                        (string)VM.Settings.GetValue("Filter")));
                    break;

                case "EasyViewGroupListWidthPercent":
                case "EasyViewClockListViewSizePercent":
                    ApplyEasyViewsSizesFromSettings();
                    break;

                case "EasyViewGroupListScrolledPercent":
                case "EasyViewClockListViewScrolledPercent":
                case "EasyViewClockDataGridScrolledPercent":
                    ApplyScrollPercentsToControls();
                    break;

                case "ShowGlobalStatusBar":
                    if ((bool)VM.Settings.GetValue("ShowGlobalStatusBar"))
                    {
                        if ((bool)CbFullScreen.IsChecked)
                        {
                            MyStatusBar.Height = 50;
                        }
                        else
                        {
                            MyStatusBar.Height = 30;
                        }
                        MyStatusBar.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        MyStatusBar.Visibility = Visibility.Collapsed;
                    }
                    break;

                case "AutoresizeTableColumns":
                    MyViewsGrid.MyDataGrid.AutoresizeTableColumns =
                        (bool)VM.Settings.GetValue("AutoresizeTableColumns");
                    break;
            }
        }

        private void ApplyViewType()
        {
            var vt = (string)VM.Settings.GetValue("ViewType");
            switch (vt)
            {
                case "List":
                    CbListView.IsChecked = true;

                    CbDataGridView.IsChecked = false;
                    CbHorizontalSplitView.IsChecked = false;
                    CbVerticalSplitView.IsChecked = false;
                    break;

                case "DataGrid":
                    CbDataGridView.IsChecked = true;

                    CbListView.IsChecked = false;
                    CbHorizontalSplitView.IsChecked = false;
                    CbVerticalSplitView.IsChecked = false;
                    break;

                case "HorizontalSplit":
                    CbHorizontalSplitView.IsChecked = true;

                    CbListView.IsChecked = false;
                    CbDataGridView.IsChecked = false;
                    CbVerticalSplitView.IsChecked = false;
                    break;

                case "VerticalSplit":
                    CbVerticalSplitView.IsChecked = true;

                    CbListView.IsChecked = false;
                    CbDataGridView.IsChecked = false;
                    CbHorizontalSplitView.IsChecked = false;
                    break;
            }
            MyViewsGrid.ApplyViewType(vt);
            ApplyEasyViewsSizesFromSettings();
        }

        private void ToggleShowGroupListView(SettingDataM sd)
        {
            MyViewsGrid.ApplyGroupListVisibility((bool)sd.Value);
            CbShowGroupListView.IsChecked = (bool)sd.Value;

            ApplyEasyViewsSizesFromSettings();
            ApplyScrollPercentsToControls();
        }

        internal void ApplyEasyViewsSizesFromSettings()
        {
            ApplyEasyViewGroupListWidthPercent();
            ApplyEasyViewClockListViewSizePercent();
        }

        private void ApplyEasyViewClockListViewSizePercent()
        {
            //var percent = (double)VM.Settings.GetValue("EasyViewClockListViewSizePercent");

            //if (VM.Settings.GetValue("ViewType").ToString() == "HorizontalSplit")
            //{
            //    // Previous version:
            //    //percent = percent -
            //    //    MyViewsGrid.RootGrid.ColumnDefinitions[0].Width.Value;

            //    // Begin new version
            //    // (it has a small error that increases over many switches
            //    // from horizontal split to vertical split and back)
            //    double ensw = MyViewsGrid.MyFlowView.ActualWidth;
            //    //double ensw = MyViewsGrid.ClocksViewsGrid.ColumnDefinitions[0].Width.Value; // TODO: homework: express the right member of `ensw` here in function of the expression of ensw in uncommented line above
            //    double x = 100d * ensw *
            //        (MyViewsGrid.ClocksViewsGrid.ActualWidth - MyViewsGrid.ActualWidth) /
            //        (MyViewsGrid.ActualWidth * MyViewsGrid.ClocksViewsGrid.ActualWidth);
            //    percent = percent - x;
            //    // End new version

            //    // main right pane : 1*
            //    MyViewsGrid.RootGrid.ColumnDefinitions[2].Width =
            //        new GridLength(100d - MyViewsGrid.RootGrid.ColumnDefinitions[0].Width.Value,
            //            GridUnitType.Star);

            //    // flow view row : percent
            //    MyViewsGrid.ClocksViewsGrid.ColumnDefinitions[0].Width = new GridLength(percent, GridUnitType.Star);
            //    MyViewsGrid.ClocksViewsGrid.ColumnDefinitions[2].Width = new GridLength(100d - percent, GridUnitType.Star);
            //}
            //else if (VM.Settings.GetValue("ViewType").ToString() == "VerticalSplit")
            //{
            //    // main right pane : 1*
            //    MyViewsGrid.RootGrid.ColumnDefinitions[2].Width =
            //        new GridLength(100d - MyViewsGrid.RootGrid.ColumnDefinitions[0].Width.Value,
            //            GridUnitType.Star);

            //    // flow view row : percent
            //    MyViewsGrid.ClocksViewsGrid.RowDefinitions[0].Height = new GridLength(percent, GridUnitType.Star);
            //    MyViewsGrid.ClocksViewsGrid.RowDefinitions[2].Height = new GridLength(100d - percent, GridUnitType.Star);
            //}
            //else
            //{
            //    // main right pane : percent
            //    MyViewsGrid.RootGrid.ColumnDefinitions[2].Width = new GridLength(percent, GridUnitType.Star);
            //}
        }

        private void ApplyEasyViewGroupListWidthPercent()
        {
            //double percent;

            //if ((bool)VM.Settings.GetValue("ShowGroupListView"))
            //{
            //    percent = (double)VM.Settings.GetValue("EasyViewGroupListWidthPercent");

            //    // left pane: percent
            //    MyViewsGrid.MyGroupListColumn.Width =
            //        new GridLength(percent, GridUnitType.Star);

            //    // main grid splitter: 0
            //    MyViewsGrid.MyRootGridSplitterColumn.Width =
            //        new GridLength(1, GridUnitType.Auto);
            //}
            //else
            //{
            //    percent = 0d;

            //    // left pane: 0
            //    MyViewsGrid.MyGroupListColumn.Width =
            //        new GridLength(0d, GridUnitType.Star);

            //    // main grid splitter: 0
            //    MyViewsGrid.MyRootGridSplitterColumn.Width =
            //        new GridLength(0d, GridUnitType.Star);
            //}
        }

        internal KeepOnTopWindowMode CurrentKeepOnTopWindowMode = KeepOnTopWindowMode.None;

        public enum KeepOnTopWindowMode
        {
            Always,
            UntilNextRestart,
            For1Min,
            For5Min,
            For15Min,
            For1Hour,
            None
        }

        internal System.Windows.Threading.DispatcherTimer KeepOnTopTimer = new System.Windows.Threading.DispatcherTimer(System.Windows.Threading.DispatcherPriority.Normal)
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        internal TimeSpan RemainingKeepOnTop = TimeSpan.Zero;

        internal void SetKeepOnTopAlways(KeepOnTopWindowMode b)
        {
            CurrentKeepOnTopWindowMode = b;

            KeepOnTopTimer.Tick -= KeepOnTopTimer_Tick;
            KeepOnTopTimer.Tick += KeepOnTopTimer_Tick;

            Topmost = b != KeepOnTopWindowMode.None;

            MiKeepOnTop.MiKeepOnTopAlways.IsChecked = b == KeepOnTopWindowMode.Always;
            MiKeepOnTop.MiKeepOnTopUntilNextRestart.IsChecked = b == KeepOnTopWindowMode.UntilNextRestart;
            MiKeepOnTop.MiKeepOnTopFor5Min.IsChecked = b == KeepOnTopWindowMode.For5Min;
            MiKeepOnTop.MiKeepOnTopFor15Min.IsChecked = b == KeepOnTopWindowMode.For15Min;
            MiKeepOnTop.MiKeepOnTopFor1Hour.IsChecked = b == KeepOnTopWindowMode.For1Hour;
            MiKeepOnTop.MiKeepOnTopFor1Min.IsChecked = b == KeepOnTopWindowMode.For1Min;

            switch (b)
            {
                case KeepOnTopWindowMode.None:
                    KeepOnTopTimer.Stop();
                    MiKeepOnTop.SetRemainingTimeSpan(TimeSpan.Zero);
                    VM.Settings.SetValue("MainFormAlwaysOnTop", false);
                    break;

                case KeepOnTopWindowMode.Always:
                case KeepOnTopWindowMode.UntilNextRestart:
                    VM.Settings.SetValue("MainFormAlwaysOnTop", true);
                    RemainingKeepOnTop = TimeSpan.MaxValue;
                    KeepOnTopTimer.Start();
                    break;

                case KeepOnTopWindowMode.For1Min:
                    VM.Settings.SetValue("MainFormAlwaysOnTop", false);
                    RemainingKeepOnTop = TimeSpan.FromMinutes(1);
                    KeepOnTopTimer.Start();
                    break;

                case KeepOnTopWindowMode.For5Min:
                    VM.Settings.SetValue("MainFormAlwaysOnTop", false);
                    RemainingKeepOnTop = TimeSpan.FromMinutes(5);
                    KeepOnTopTimer.Start();
                    break;

                case KeepOnTopWindowMode.For15Min:
                    VM.Settings.SetValue("MainFormAlwaysOnTop", false);
                    RemainingKeepOnTop = TimeSpan.FromMinutes(15);
                    KeepOnTopTimer.Start();
                    break;

                case KeepOnTopWindowMode.For1Hour:
                    VM.Settings.SetValue("MainFormAlwaysOnTop", false);
                    RemainingKeepOnTop = TimeSpan.FromHours(1);
                    KeepOnTopTimer.Start();
                    break;
            }
        }

        private void KeepOnTopTimer_Tick(object sender, EventArgs e)
        {
            switch (CurrentKeepOnTopWindowMode)
            {
                case KeepOnTopWindowMode.None:
                    KeepOnTopTimer.Stop();
                    break;

                case KeepOnTopWindowMode.Always:
                    RemainingKeepOnTop -= TimeSpan.FromSeconds(1);
                    break;

                case KeepOnTopWindowMode.UntilNextRestart:
                    RemainingKeepOnTop -= TimeSpan.FromSeconds(1);
                    break;

                case KeepOnTopWindowMode.For1Min:
                    RemainingKeepOnTop -= TimeSpan.FromSeconds(1);
                    break;

                case KeepOnTopWindowMode.For5Min:
                    RemainingKeepOnTop -= TimeSpan.FromSeconds(1);
                    break;

                case KeepOnTopWindowMode.For15Min:
                    RemainingKeepOnTop -= TimeSpan.FromSeconds(1);
                    break;

                case KeepOnTopWindowMode.For1Hour:
                    RemainingKeepOnTop -= TimeSpan.FromSeconds(1);
                    break;
            }

            if (RemainingKeepOnTop <= TimeSpan.Zero)
            {
                SetKeepOnTopAlways(KeepOnTopWindowMode.None);
            }
            else
            {
                MiKeepOnTop.SetRemainingTimeSpan(RemainingKeepOnTop);
            }
        }

        private void VM_FileLoaded(object sender, EventArgs e)
        {
            ApplySettingsFromDataFile();
        }

        private void VM_FileClosed(object sender, FileClosedEventArgs e)
        {
            if (e.OtherFileWillBeOpened || MainFormIsBeingClosed)
            {
            }
            else
            {
                // until I find how to not create 2 ClockListView-s, I do this check below:
                if (VM.ClockVMCollection.Model.Ms.Count > 0)
                {
                    return;
                }
                VM.ClockVMCollection.Model.IsUnsavedLocked = true;
                VM.ClockVMCollection.Model.AddClock(new TimerData(VM, VM.MultiAudioPlayer));
                VM.ClockVMCollection.Model.IsUnsavedLocked = false;
            }

            ApplySettingsFromDataFile();
        }

        private void VM_IsUnsavedChanged(object sender, EventArgs e)
        {
            OnFirstOpeningOrChange();
            UpdateWindowTitle();
        }

        protected void OnFirstOpeningOrChange()
        {
            if (Properties.Settings.Default.FirstStart)
            {
                Utils.SetStartProgramWithWindows(true);
            }
            LblFirstStart.Visibility = Visibility.Collapsed;
            Properties.Settings.Default.FirstStart = false;
            Properties.Settings.Default.Save();
        }

        private void VM_FilePathChanged(object sender, EventArgs e)
        {
            MyStatusBar.PostMessage($"Open file path changed to: {VM.FilePath}", LogCategory.Information);

            if (MainFormIsBeingClosed)
            {

            }
            else
            {
                OnFirstOpeningOrChange();
                UpdateWindowTitle();

                if (VM.FilePath != "")
                {
                    MyRecentFilesContextMenu.InsertInRecentFilesList(VM.FilePath);
                }

                Properties.Settings.Default.LastOpenFile = VM.FilePath;
                Properties.Settings.Default.Save();
            }
        }

        internal void EnableMultipleSelection()
        {
            MiMultipleSelection.IsChecked = true;
        }

        public DataFile VM
        {
            get
            {
                return DataContext as DataFile;
            }
        }

        private void MyNotifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Show();
            if (WindowState == WindowState.Minimized)
            {
                WindowState = WindowState.Normal;
            }
            Activate();
        }

        public System.Windows.Forms.NotifyIcon MyNotifyIcon;
        public SaveFileDialog MySaveFileDialog;

        private void CanExecute_ApplicationCommands_New(object sender, CanExecuteRoutedEventArgs e)
        {
            if (VM.FilePath == "")
            {
                e.CanExecute = VM.IsUnsaved;
            }
            else
            {
                e.CanExecute = true;
            }
        }

        private void Executed_ApplicationCommands_New(object sender, ExecutedRoutedEventArgs e)
        {
            CloseFile();
        }

        private void CanExecute_ApplicationCommands_Open(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Executed_ApplicationCommands_Open(object sender, ExecutedRoutedEventArgs e)
        {
            if (VM.FilePath != "" || VM.IsUnsaved)
            {
                VM.CloseAndOpenOtherFileWPF();
            }
            else
            {
                VM.LoadFromFileWPF();
            }
        }

        private void CanExecute_OpenContainingFolder(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = VM.FilePath == "" ? false : true;
        }

        private void Executed_OpenContainingFolder(object sender, ExecutedRoutedEventArgs e)
        {
            Utils.SelectFileInItsFolderInExplorer(VM.FilePath);
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

        public void ForceUpdateViewType(EasyViewType value)
        {
            // Update the views data structure and Controls
            if (value == EasyViewType.HorizontalSplit ||
                value == EasyViewType.VerticalSplit)
            {
                //MyClocksViewProvider.ForceUpdateViewType_Split(
                //    value == EasyViewType.HorizontalSplit ?
                //        Orientation.Horizontal : Orientation.Vertical,
                //    tsmiViewGroupListView.Checked);
            }
            else if (value == EasyViewType.List)
            {
                //MyClocksViewProvider.ForceUpdateViewType_List(tsmiViewGroupListView.Checked);
            }
            else // if (value == EasyViewType.DataGrid)
            {
                //MyClocksViewProvider.ForceUpdateViewType_DataGrid(tsmiViewGroupListView.Checked);
            }


            // Update the menu items and tool bars to reflect the new views
            //tsmiDataGridView.Checked = value == EasyViewType.DataGrid;
            //tsmiHorizontalSplitView.Checked = value == EasyViewType.HorizontalSplit;
            //tsmiVerticalSplitView.Checked = value == EasyViewType.VerticalSplit;
            //tsmiListView.Checked = value == EasyViewType.List;

            //tsDataGridZoom.SetItemsEnabled(
            //    value == EasyViewType.DataGrid ||
            //    value == EasyViewType.HorizontalSplit ||
            //    value == EasyViewType.VerticalSplit);
            //tsListViewZoom.SetItemsEnabled(
            //    value == EasyViewType.List ||
            //    value == EasyViewType.HorizontalSplit ||
            //    value == EasyViewType.VerticalSplit);
            //tsGroupListZoom.SetItemsEnabled(tsmiViewGroupListView.Checked);


            // Apply zoom percents from settings to possible new views created above
            //if (value == EasyViewType.List)
            //{
            //    PropagateGlobalZoomFactor();
            //}
            //else if (value == EasyViewType.DataGrid)
            //{
            //    PropagateDataGridGlobalZoomFactor();
            //}
            //else if (value == EasyViewType.HorizontalSplit ||
            //         value == EasyViewType.VerticalSplit)
            //{
            //    PropagateGlobalZoomFactor();
            //    PropagateDataGridGlobalZoomFactor();
            //}

            //if (tsmiViewGroupListView.Checked)
            //{
            //    PropagateGroupListZoomPercent();
            //}

            //ApplyEasyViewsSizesFromSettings(MyDataFile);
        }

        internal void UpdateWindowTitle()
        {
            if (VM.FilePath == "")
            {
                if (VM.IsUnsaved)
                {
                    Title = $"Timed Silver - new file *";
                }
                else
                {
                    Title = $"Timed Silver - new file";
                }
            }
            else
            {
                string baseFileName = System.IO.Path.GetFileName(VM.FilePath);

                if (VM.IsUnsaved)
                {
                    Title = $"Timed Silver - {baseFileName} *";
                }
                else
                {
                    Title = $"Timed Silver - {baseFileName}";
                }
            }
        }

        public bool Save()
        {
            if (VM.FilePath.Length == 0)
            {
                return SaveAs();
            }
            else
            {
                return SaveAs(VM.FilePath);
            }
        }

        public bool SaveIfOnDisk()
        {
            if (VM.FilePath.Length == 0)
            {
                return false;
            }
            else
            {
                return SaveAs(VM.FilePath);
            }
        }

        internal bool CloseFile(bool withConfirmationRequest = true)
        {
            IsFileBeingClosed = true;
            if (!withConfirmationRequest)
            {
                VM.Close();
                IsFileBeingClosed = false;
                return true;
            }

            if (VM.IsUnsaved)
            {
                System.Windows.MessageBoxResult r = MessageBox.Show(this, "There are unsaved changes. Do you want to save the changes before closing?", "Confirmation request", System.Windows.MessageBoxButton.YesNoCancel);
                if (r == MessageBoxResult.No)
                {
                    VM.Close();
                    IsFileBeingClosed = false;
                    return true;
                }
                else if (r == MessageBoxResult.Yes)
                {
                    if (Save())
                    {
                        VM.Close();
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
                VM.Close();
                IsFileBeingClosed = false;
                return true;
            }
        }

        private void CanExecute_ReloadOpenFileFromFileSystem(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = VM.FilePath != "";
        }

        internal void ApplyAutosortSetting()
        {
            switch (VM.ClockVMCollection.Model.AutosortMode)
            {
                case AutosortMode.None:  // no automatic sort
                    MyViewsGrid.MyClockGroupListView.MyViewComboBox.SelectedIndex = 0;
                    break;

                case AutosortMode.ClosestRingingMoment:  // by closest ringing moment
                    MyViewsGrid.MyClockGroupListView.MyViewComboBox.SelectedIndex = 1;
                    // code to apply selected sort once (autosort is done inside MyDataFile):
                    VM.ClockVMCollection.Model.DoSortByClosestRingingMoment();
                    break;

                case AutosortMode.Alphabetically:  // alphabetically
                    MyViewsGrid.MyClockGroupListView.MyViewComboBox.SelectedIndex = 2;
                    // code to apply selected sort once (autosort is done inside MyDataFile):
                    VM.ClockVMCollection.Model.DoSortAlphabetically();
                    break;
            }
        }

        internal void ApplyFilter(FilterM f = null)
        {
            if (f == null)
            {
                f = new FilterM(VM.ClockVMCollection.Model, "")
                {
                    ShowActive = true,
                    ShowAlarms = true,
                    ShowInactive = true,
                    ShowTimers = true
                };
            }

            ClockGroupListView tglv = MyViewsGrid.MyClockGroupListView;

            if (tglv.MyFilters == null)
            {
                return;
            }

            // TODO: use a Filter.Includes(Filter) method (if a filter displays all clocks inside the parameter Filter).
            foreach (FilterVM fd in tglv.MyFilters)
            {
                FilterM ff = f.Clone();
                ff.SearchString = "";

                if (fd.M == ff)
                {
                    fd.IsSelected = true;
                }
                else
                {
                    fd.IsSelected = false;
                }
            }

            tglv.MySearchTextBox.Text = f.SearchString;

            VM.ClockVMCollection.Model.AppliedFilter = f;
        }

        internal bool RegisterScrollChanges { get; set; } = true;

        private void ApplyScrollPercentsToControls()
        {
            if (!RegisterScrollChanges)
            {
                return;
            }
            //RegisterScrollChanges = false;

            SetVerticalScrollPercentToControl(MyViewsGrid.MyClockGroupListView,
                (double)VM.Settings.GetValue("EasyViewGroupListScrolledPercent"));

            SetVerticalScrollPercentToControl(MyViewsGrid.MyFlowView,
                (double)VM.Settings.GetValue("EasyViewClockListViewScrolledPercent"));

            SetVerticalScrollPercentToControl(MyViewsGrid.MyDataGrid,
                (double)VM.Settings.GetValue("EasyViewClockDataGridScrolledPercent"));

            //RegisterScrollChanges = true;
        }

        private void Executed_ShowStatusBar(object sender, ExecutedRoutedEventArgs e)
        {
            if ((bool)e.Parameter)
            {
                VM.Settings.SetValue("ShowGlobalStatusBar", true);
            }
            else
            {
                VM.Settings.SetValue("ShowGlobalStatusBar", false);
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
                VM.CloseAndOpenOtherFileWPF(e.Path);
        }

        internal void PropagateBeepsDataTable(BeepCollection v)
        {
            foreach (ClockM cd in VM.ClockVMCollection.Model.Ms)
            {
                if (cd is TimerData td && td.Running)
                {
                    td.UpdateBeepTimers();
                }
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateMainFormRectangleSetting();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (!ApplyWithoutSetting && VM != null)
            {
                VM.Settings.SetValue("MainFormWindowState", WindowState.ToString());
            }
        }

        private void UpdateMainFormRectangleSetting()
        {
            if (!IsVisible)
            {
                VM.Settings.IsUnsavedLocked = true;
            }

            Rect r = RestoreBounds;

            System.Drawing.Rectangle rr = System.Drawing.Rectangle.FromLTRB(
                (int)Math.Round(r.Left),
                (int)Math.Round(r.Top),
                (int)Math.Round(r.Right),
                (int)Math.Round(r.Bottom));

            VM.Settings.SetValue("MainFormRectangle", rr);

            if (!IsVisible)
            {
                VM.Settings.IsUnsavedLocked = false;
            }
        }

        internal void AddNewTimer()
        {
            var td = new TimerData(VM, VM.MultiAudioPlayer);

            td.GroupName = GetFirstGroupNameInFilter(VM.ClockVMCollection.Model.AppliedFilter);
            td.Checkable = IsMultipleSelectionMenuItemCheckable;

            VM.ClockVMCollection.Model.AddClock(td);
        }

        internal void AddNewAlarm()
        {
            var td = new AlarmData(VM, VM.MultiAudioPlayer);

            td.GroupName = GetFirstGroupNameInFilter(VM.ClockVMCollection.Model.AppliedFilter);
            td.Checkable = IsMultipleSelectionMenuItemCheckable;

            VM.ClockVMCollection.Model.AddClock(td);
        }

        private void CanExecute_ApplicationCommands_Properties(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        internal bool SettingsWindowVisible = false;

        private void Executed_ApplicationCommands_Properties(object sender, ExecutedRoutedEventArgs e)
        {
            SettingsWindowVisible = true;
            MySettingsWindow.ShowDialog();
            SettingsWindowVisible = false;
        }

        internal void ApplySettingsFromDataFile(DataFile MyDataFile = null)
        {
            if (MainFormIsBeingClosed)
            {
                return;
            }

            if (MyDataFile == null)
            {
                MyDataFile = this.VM;
            }

            ApplyWithoutSetting = true;

            // ...

            MyDataFile.Settings.IsUnsavedLocked = true;

            // ...

            // imi va aparea ca fisierul nu e salvat, deci aplica santinelele de mai sus inainte si dupa DataFile.LoadFromString

            PropagateBeepsDataTable(MyDataFile.Beeps);

            MyDataFile.ClockVMCollection.Model.IsUnsavedLocked = true;
            MyDataFile.ClockVMCollection.Model.IsUnsavedLocked = false;

            MyDataFile.Settings.IsUnsavedLocked = false;

            ApplyWithoutSetting = false;

            // TODO:
            //InitializeLastFocusedTimersView();

            ApplySetting(VM.Settings["RingingDuration"]);
        }

        internal static void SetVerticalScrollPercentToControl(ClockFlowLayoutPanel clv, double percent)
        {
            System.Windows.Controls.ScrollViewer sv = clv.MyScrollViewer;

            //if (sv.ScrollableHeight == 0)
            //{
            //    return;
            //}

            double d = sv.ScrollableHeight / 100D * percent;
            if (double.IsNaN(d))
            {
                return;
            }
            sv.ScrollToVerticalOffset(d);
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

        private void CbFullScreen_Checked(object sender, RoutedEventArgs e)
        {
            WindowStyle = WindowStyle.None;
            WindowState = WindowState.Maximized;
            MyStatusBar.ShowTimeDate = true;
            ResizeMode = ResizeMode.NoResize;


            MyStatusBar.Height = 50;
            MyStatusBar.SysTimer.Enabled = true;
            MyStatusBar.SysTimer.ForceUpdate();
        }

        private void CbFullScreen_Unchecked(object sender, RoutedEventArgs e)
        {
            WindowStyle = WindowStyle.SingleBorderWindow;
            WindowState = WindowState.Normal;
            MyStatusBar.ShowTimeDate = false;
            ResizeMode = ResizeMode.CanResizeWithGrip;

            MyStatusBar.Height = 30;
        }

        internal void PropagateRoundedCornersSetting()
        {
            MyViewsGrid.MyFlowView.RoundedCorners =
                (bool)VM.GetValue("RoundedCorners");
        }

        private void CbDarkTheme_Checked(object sender, RoutedEventArgs e)
        {
            if (ApplyWithoutSetting)
            {

            }
            else
            {
                VM.SetValue("DarkMode", (bool)CbDarkTheme.IsChecked);
            }
        }

        private void CbDarkTheme_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ApplyWithoutSetting)
            {

            }
            else
            {
                VM.SetValue("DarkMode", (bool)CbDarkTheme.IsChecked);
            }
        }

        private void CanExecute_ApplicationCommands_Help(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        protected HelpWindow OpenHelpWindow = null;
        private void Executed_ApplicationCommands_Help(object sender, ExecutedRoutedEventArgs e)
        {
            // TODO: [VISUAL] make about & settings forms non-modal
            if (OpenHelpWindow == null)
            {
                OpenHelpWindow = new HelpWindow();
            }
            OpenHelpWindow.Show();
            OpenHelpWindow.Activate();
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
                VM.Save();
                return true;
            }

            // TODO: memoize in Utils.cs, use ~GetSaveFileChooser
            MySaveFileDialog.FileName = $"TimedSilver {DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss")}.xml";
            MySaveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            if ((bool)MySaveFileDialog.ShowDialog(this))
            {
                VM.SaveAs(MySaveFileDialog.FileName);
                return true;
            }

            return false;
        }

        private void CanExecute_Import(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Executed_Import(object sender, ExecutedRoutedEventArgs e)
        {
            var imw = new ImportWindow();
            imw.Show();
        }

        private void CanExecute_ApplicationCommands_SaveAs(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Executed_ApplicationCommands_SaveAs(object sender, ExecutedRoutedEventArgs e)
        {
            SaveAs();
        }

        private void CanExecute_ApplicationCommands_Save(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Executed_ApplicationCommands_Save(object sender, ExecutedRoutedEventArgs e)
        {
            Save();
        }

        private void CanExecute_Exit(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Executed_Exit(object sender, ExecutedRoutedEventArgs e)
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

        private void CanExecute_RemoveAllClocks(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Executed_RemoveAllClocks(object sender, ExecutedRoutedEventArgs e)
        {
            int c = VM.ClockVMCollection.Model.Ms.Count;
            if (c > 0)
            {
                if (MessageBox.Show(this, $"Are you sure you want to delete all {c} existing clocks?", "Confirmation request", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    VM.ClockVMCollection.Model.RemoveAllClocks();
                    MyStatusBar.PostMessage($"All {c} existing clocks have been deleted.",
                        LogCategory.Information);
                }
            }
            else
            {
                MyStatusBar.PostMessage("There aren't any clocks to delete.",
                    LogCategory.Information);
            }
        }

        internal void ApplyAndSetEasyView(EasyViewType v)
        {
            VM.Settings.SetValue("ViewType", v.ToString());
        }

        private void CanExecute_ListView(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Executed_ListView(object sender, ExecutedRoutedEventArgs e)
        {
            ApplyAndSetEasyView(EasyViewType.List);
        }

        private void CanExecute_DataGridView(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Executed_DataGridView(object sender, ExecutedRoutedEventArgs e)
        {
            ApplyAndSetEasyView(EasyViewType.DataGrid);
        }

        private void CanExecute_HorizontalSplitView(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Executed_HorizontalSplitView(object sender, ExecutedRoutedEventArgs e)
        {
            ApplyAndSetEasyView(EasyViewType.HorizontalSplit);
        }

        private void CanExecute_VerticalSplitView(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Executed_VerticalSplitView(object sender, ExecutedRoutedEventArgs e)
        {
            ApplyAndSetEasyView(EasyViewType.VerticalSplit);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MySettingsWindow.Owner = this;
            MySettingsWindow.MainWindow = this;

            MySettingsWindow.ApplyTemplateRecursively();


            VM.Settings.IsUnsavedLocked = true;
            VM.ClockVMCollection.Model.IsUnsavedLocked = true;

            foreach (KeyValuePair<string, SettingDataM> k in VM.Settings)
            {
                ApplySetting(k.Value);
            }

            VM.Settings.IsUnsavedLocked = false;
            VM.ClockVMCollection.Model.IsUnsavedLocked = false;

            // NOTE: do this before loading the data file:
            MyViewsGrid.FocusedZoomableControlChanged += MyViewsGrid_FocusedZoomableSubcontrolChanged;

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
                    MessageBoxResult dr = MessageBox.Show(this, $"Do you wish to open the last opened Timed Silver file, {Utils.BaseFileNameInPath(Properties.Settings.Default.LastOpenFile)}? (Press No to open a new empty file, or Cancel to close the program.)\n\nFull path: {Properties.Settings.Default.LastOpenFile}", "Confirmation request",
                        MessageBoxButton.YesNoCancel, MessageBoxImage.Information);
                    if (dr == MessageBoxResult.Yes)
                    {
                        LoadLastOpenFile();
                    }
                    else if (dr == MessageBoxResult.No)
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


            if (MySettingsWindow.MyControl.MyItemsControl.ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            {
                HandleContainersGenerated();
            }
            else
            {
                MySettingsWindow.MyControl.MyItemsControl.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
            }
        }

        private void HandleContainersGenerated()
        {


            MyStatusBar.PostMessage("Program successfully started.", LogCategory.Information);

            CmbAutomaticSort.SelectionChanged += CmbAutomaticSort_SelectionChanged;

            IsBeingLoaded = false;
        }

        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (MySettingsWindow.MyControl.MyItemsControl.ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            {
                MySettingsWindow.MyControl.MyItemsControl.ItemContainerGenerator.StatusChanged -= ItemContainerGenerator_StatusChanged;
                HandleContainersGenerated();
            }
        }

        private void MyViewsGrid_FocusedZoomableSubcontrolChanged(object sender, ViewsGrid.ZoomableControlEventArgs e)
        {
            TbZoomGlobal.LinkedZoomableControl = e.ZoomableControl as IZoomableControl;
        }

        internal void LoadNewEmptyFile()
        {
            VM.Settings.IsUnsavedLocked = true;
            VM.ClockVMCollection.Model.IsUnsavedLocked = true;

            VM.ClockVMCollection.Model.AddClock(new TimerData(VM, VM.MultiAudioPlayer));

            VM.Settings.IsUnsavedLocked = false;
            VM.ClockVMCollection.Model.IsUnsavedLocked = false;

            ApplySettingsFromDataFile();
        }

        internal void LoadLastOpenFile()
        {
            VM.LoadFromFileWPF(Properties.Settings.Default.LastOpenFile);
        }

        // TODO: move this method in Filter class.
        internal string GetFirstGroupNameInFilter(FilterM f)
        {
            return f.GroupNames.Count == 0 ?
                "" : f.GroupNames[0];
        }

        public bool IsMultipleSelectionMenuItemCheckable
        {
            get
            {
                return (bool)MiMultipleSelection.IsChecked;
            }
        }

        private void CmbAutomaticSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (CmbAutomaticSort.SelectedIndex)
            {
                case 0: // no automatic sort
                    VM.ClockVMCollection.Model.AutosortMode = AutosortMode.None;
                    break;

                case 1: // by closest ringing moment
                    VM.ClockVMCollection.Model.AutosortMode = AutosortMode.ClosestRingingMoment;
                    // code to apply selected sort once (autosort is done inside MyDataFile):
                    VM.ClockVMCollection.Model.DoSortByClosestRingingMoment();
                    break;

                case 2: // alphabetically
                    VM.ClockVMCollection.Model.AutosortMode = AutosortMode.Alphabetically;
                    // code to apply selected sort once (autosort is done inside MyDataFile):
                    VM.ClockVMCollection.Model.DoSortAlphabetically();
                    break;
            }
        }

        private void CanExecute_About(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Executed_About(object sender, ExecutedRoutedEventArgs e)
        {
            var w = new AboutWindow();
            w.Owner = this;
            w.ShowDialog();
        }

        private void CanExecute_MultipleSelectionInListView(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        bool changeByUser_MultipleSelection = true;
        private void Executed_MultipleSelectionInListView(object sender, ExecutedRoutedEventArgs e)
        {
            if (!changeByUser_MultipleSelection)
            {
                return;
            }

            bool chk = (bool)e.Parameter;

            bool changed = false;
            if (MyViewsGrid.MyFlowView.Visibility == Visibility.Visible)
            {
                changed = true;
            }

            if (changed)
            {
                MyViewsGrid.MyFlowView.MultiSelectClocks = chk;
                MiMultipleSelection.IsChecked = chk;
            }
            else
            {
                changeByUser_MultipleSelection = false;
                MiMultipleSelection.IsChecked = false;
                changeByUser_MultipleSelection = true;

                MessageBox.Show(this, "Multiple selection with common menu is available only in list view. Please see the View menu.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CanExecute_RestoreToPreviousDay(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = VM.MyDailyBackupSystem.BackupExists;
        }

        private void Executed_RestoreToPreviousDay(object sender, ExecutedRoutedEventArgs e)
        {
            if (VM.MyDailyBackupSystem.BackupExists)
            {
                VM.MyDailyBackupSystem.RestoreBackup();
            }
        }

        private void CanExecute_ClearOpenFile(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Executed_ClearOpenFile(object sender, ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show(this, "Are you sure you want to clear (reset) the open file?", "Confirmation request", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                VM.ResetAll();
            }
        }

        private void Executed_ReloadOpenFileFromFileSystem(object sender, ExecutedRoutedEventArgs e)
        {
            VM.ReloadOpenFileFromFileSystem();
        }

        private void CanExecute_ShowStatusBar(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CanExecute_ShowGroupListView(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Executed_ShowGroupListView(object sender, ExecutedRoutedEventArgs e)
        {
            VM.Settings.SetValue("ShowGroupListView", (bool)e.Parameter);
        }

        private void MyViewsGrid_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            //if (!RegisterScrollChanges)
            //{
            //    return;
            //}
            RegisterScrollChanges = false;

            double d = GetVerticalScrollPercentFromControl(MyViewsGrid.MyDataGrid);
            if (!double.IsNaN(d))
            {
                VM.SetValueWithoutApply("EasyViewClockDataGridScrolledPercent", d);
            }

            double d2 = GetVerticalScrollPercentFromControl(MyViewsGrid.MyClockGroupListView);
            if (!double.IsNaN(d2))
            {
                VM.SetValueWithoutApply("EasyViewGroupListScrolledPercent", d2);
            }

            double d3 = GetVerticalScrollPercentFromControl(MyViewsGrid.MyFlowView);
            if (!double.IsNaN(d3))
            {
                VM.SetValueWithoutApply("EasyViewClockListViewScrolledPercent", d3);
            }

            RegisterScrollChanges = true;
        }

        private void TbZoomList_ValueChanged(object sender, EventArgs e)
        {
            //VM.SetValueWithoutApply("GlobalZoomPercent", TbZoomList.Value);
        }

        private void TbZoomDataGrid_ValueChanged(object sender, EventArgs e)
        {
            //VM.SetValueWithoutApply("GlobalDataGridZoomPercent", TbZoomDataGrid.Value);
        }

        private void TbZoomGroupList_ValueChanged(object sender, EventArgs e)
        {
            //VM.SetValueWithoutApply("GroupListZoomPercent", TbZoomGroupList.Value);
        }

        private void TbZoomGlobal_ValueChanged(object sender, EventArgs e)
        {
            if (TbZoomGlobal.LinkedZoomableControl == MyViewsGrid.MyFlowView)
            {
                VM.SetValueWithoutApply("GlobalDataGridZoomPercent", TbZoomGlobal.Value);
                //TbZoomList.Value = TbZoomGlobal.Value;
            }
            else if (TbZoomGlobal.LinkedZoomableControl == MyViewsGrid.MyDataGrid)
            {
                VM.SetValueWithoutApply("GlobalZoomPercent", TbZoomGlobal.Value);
                //TbZoomDataGrid.Value = TbZoomGlobal.Value;
            }
            else if (TbZoomGlobal.LinkedZoomableControl == MyViewsGrid.MyClockGroupListView)
            {
            }
            else if (TbZoomGlobal.LinkedZoomableControl == null)
            {
                
            }
            else if (TbZoomGlobal.LinkedZoomableControl is ZoomableScrollViewer)
            {
                VM.SetValueWithoutApply("GroupListZoomPercent", TbZoomGlobal.Value);
                //TbZoomGroupList.Value = TbZoomGlobal.Value;
            }
            else if (TbZoomGlobal.LinkedZoomableControl is ZoomableStackPanel)
            { 

                VM.SetValueWithoutApply("GroupListToolBarsZoomPercent", TbZoomGlobal.Value);
                // A seprate zoom toolbar for the toolbars? No:
                //TbZoomGroupList.Value = TbZoomGlobal.Value;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void NewTimers_CustomSplitButton_CountRequested(object sender, IntEventArgs e)
        {
            for (int i = 0; i < e.Value; ++i)
            {
                AddNewTimer();
            }
        }

        private void NewAlarms_CustomSplitButton_CountRequested(object sender, IntEventArgs e)
        {
            for (int i = 0; i < e.Value; ++i)
            {
                AddNewAlarm();
            }
        }

        internal void FocusClockVM(ClockVM vm)
        {
            if (WindowState == WindowState.Minimized)
            {
                WindowState = WindowState.Normal;
            }
            Activate();
            var c2 = (FrameworkElement)MyViewsGrid.MyFlowView.MyItemsControl.ItemContainerGenerator.ContainerFromItem(vm) as ContentPresenter;
            var c = (ClockUserControl)c2.ContentTemplate.FindName("MyClockUserControl", c2);
            c.BringIntoView();

            Dispatcher.BeginInvoke(new Action(() =>
            {
                c.MyTextBox.Focus();
            }), System.Windows.Threading.DispatcherPriority.SystemIdle); // ApplicationIdle better?
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

        private void UpdateDropIndicator()
        {
            if (DraggingOver)
            {
                MyDropOverlay.FadeIn();
            }
            else
            {
                MyDropOverlay.FadeOut();
            }
        }

        private void MyWindow_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) &&
                !SettingsWindowVisible // TODO: do a Binding to the AllowDrop property from this flag by making it a dependency property
                /*!(e.Source is DropOverlay)*/)
            {
                e.Effects = DragDropEffects.Link;

                var paths = e.Data.
                    GetData(DataFormats.FileDrop) as string[];

                string path = paths[0];

                bool knownExtension = path.EndsWith(".xml");

                if (!knownExtension)
                {
                    MyDropOverlay.ShowWarning = true;
                }
                else
                {
                    MyDropOverlay.ShowWarning = false;
                }

                DraggingOver = true;

                e.Handled = true;
            }

            if (e.Data.GetDataPresent(DataFormats.FileDrop) &&
                SettingsWindowVisible)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void MyWindow_Drop(object sender, DragEventArgs e)
        {
            if (SettingsWindowVisible)
            {
                return;
            }

            string[] paths = null;

            if (e.Effects != DragDropEffects.None &&
                e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // TODO: use a separate thread (Task) to read the file from XML to DataFile.
                // TODO [VISUAL]: at least show a loading indicator while loading the file, using Application.DoEvents from time to time to show a small animation.
                paths = e.Data.
                    GetData(DataFormats.FileDrop) as string[];
            }

            if ((e.Effects == DragDropEffects.None ||
                !e.Data.GetDataPresent(DataFormats.FileDrop)) &&
                Utils.GetVisualParent<DropOverlay>(e.Source) == null)
            {
                // Not nececssarily to set e.Handled to true, because there are other
                // Drop event handlers deepder in the visual tree, such as
                // the ClockGroupListView or ClockFlowLayoutPanel's that must handle it.
                return;
            }

            DraggingOver = false;
            //MyDropOverlay.ShowWarning = false;

            // TODO: [VISUAL] do not accept folders, parse shortcut files, accept only XML files
            if (paths != null)
            {
                if (VM.FilePath != "" || VM.IsUnsaved)
                {
                    VM.CloseAndOpenOtherFileWPF(paths[0]);
                }
                else
                {
                    VM.LoadFromFileWPF(paths[0]);
                }

                e.Handled = true;
            }
        }

        private void MyWindow_DragLeave(object sender, DragEventArgs e)
        {
            if (SettingsWindowVisible)
            {
                return;
            }

            if (Utils.GetVisualParent<DropOverlay>(e.Source) == null)
            {
                e.Handled = true;
                return;
            }

            Point p = Mouse.GetPosition(this);
            if (p.X < 0 || p.Y < 0 ||
                p.X > ActualWidth || p.Y > ActualHeight)
            {
                DraggingOver = false;

                e.Handled = true;
            }
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = VM.ClockVMCollection.VMs.Count > 0;
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            bool changed = false;

            for (int i = VM.ClockVMCollection.VMs.Count - 1; i >= 0; --i)
            {
                ClockVM vm = VM.ClockVMCollection.VMs[i];
                if (vm.ClockType == ClockVM.ClockTypes.Timer)
                {
                    if (vm.IsActive)
                    {
                        vm.Model.ActivateOrDeactivate();
                        changed = true;
                    }
                }
            }

            if (!changed)
            {
                MessageBox.Show(this, "There wasn't any timer to stop.", "Information",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Executed_KeepOnTopAlways(object sender, ExecutedRoutedEventArgs e)
        {
            if (MiKeepOnTop.MiKeepOnTopAlways.IsChecked)
            {
                VM.Settings.SetValue("MainFormAlwaysOnTop", true);
            }
            else
            {
                VM.Settings.SetValue("MainFormAlwaysOnTop", false);
            }
        }

        private void Executed_KeepOnTopUntilNextRestart(object sender, ExecutedRoutedEventArgs e)
        {
            if (MiKeepOnTop.MiKeepOnTopUntilNextRestart.IsChecked)
            {
                SetKeepOnTopAlways(KeepOnTopWindowMode.UntilNextRestart);
            }
            else
            {
                SetKeepOnTopAlways(KeepOnTopWindowMode.None);
            }
        }

        private void Executed_KeepOnTopFor5Min(object sender, ExecutedRoutedEventArgs e)
        {
            if (MiKeepOnTop.MiKeepOnTopFor5Min.IsChecked)
            {
                SetKeepOnTopAlways(KeepOnTopWindowMode.For5Min);
            }
            else
            {
                SetKeepOnTopAlways(KeepOnTopWindowMode.None);
            }
        }

        private void Executed_KeepOnTopFor15Min(object sender, ExecutedRoutedEventArgs e)
        {
            if (MiKeepOnTop.MiKeepOnTopFor15Min.IsChecked)
            {
                SetKeepOnTopAlways(KeepOnTopWindowMode.For15Min);
            }
            else
            {
                SetKeepOnTopAlways(KeepOnTopWindowMode.None);
            }
        }

        private void Executed_KeepOnTopFor1Hour(object sender, ExecutedRoutedEventArgs e)
        {
            if (MiKeepOnTop.MiKeepOnTopFor1Hour.IsChecked)
            {
                SetKeepOnTopAlways(KeepOnTopWindowMode.For1Hour);
            }
            else
            {
                SetKeepOnTopAlways(KeepOnTopWindowMode.None);
            }
        }

        private void Executed_KeepOnTopFor1Min(object sender, ExecutedRoutedEventArgs e)
        {
            if (MiKeepOnTop.MiKeepOnTopFor1Min.IsChecked)
            {
                SetKeepOnTopAlways(KeepOnTopWindowMode.For1Min);
            }
            else
            {
                SetKeepOnTopAlways(KeepOnTopWindowMode.None);
            }
        }

        private void CanExecute_SelectAll(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Executed_SelectAll(object sender, ExecutedRoutedEventArgs e)
        {
            EnableMultipleSelection();
            VM.ClockVMCollection.Model.CheckAll();
        }

        private void MyWindow_Initialized(object sender, EventArgs e)
        {
            MySaveFileDialog = new SaveFileDialog();
            MySaveFileDialog.DefaultExt = "xml";
            MySaveFileDialog.FileName = "TimedSilver.xml";
            MySaveFileDialog.Filter = "XML files|*.xml|All files|*.*";

            MyNotifyIcon = new System.Windows.Forms.NotifyIcon();
            MyNotifyIcon.Icon = Properties.Resources._02_10_2018_final;
            MyNotifyIcon.MouseDoubleClick += MyNotifyIcon_MouseDoubleClick;
            MyNotifyIcon.Visible = true;

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

            MyTimeOutFormsManager = new TimeOutFormsManager();

            MyNotifyIcon.Text = $"Timed Silver v{Assembly.GetAssembly(typeof(MainWindow)).GetName().Version.ToString(4)}";


            DataContext = new DataFile(true);


            VM.FilePathChanged += VM_FilePathChanged;
            VM.IsUnsavedChanged += VM_IsUnsavedChanged;
            VM.FileClosed += VM_FileClosed;
            VM.FileLoaded += VM_FileLoaded;

            VM.Settings.SettingValueChange += Settings_SettingValueChange;

            VM.ClockVMCollection.Model.GlobalProgressChanged += ClockCollection_GlobalProgressChanged;

            MyRecentFilesContextMenu.PathValidationRequested += RecentFilesMenuStrip1_PathValidationRequested;
            MyRecentFilesContextMenu.SettingSaveRequested += RecentFilesMenuStrip1_SettingSaveRequested;

            MyRecentFilesContextMenu.LoadRecentPathsFromString(Properties.Settings.Default.RecentFilePaths);

            MySettingsWindow = new SettingsWindow(VM);

            if (Properties.Settings.Default.FirstStart)
            {
                LblFirstStart.Visibility = Visibility.Visible;
            }
            else
            {
                LblFirstStart.Visibility = Visibility.Collapsed;
            }
            Properties.Settings.Default.FirstStart = false;
            Properties.Settings.Default.Save();

            Closing += MainWindow_Closing;

            /* TODO:
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
             */

            //MyNotifyIconMenu = new NotifyIconContextMenu(VM, MyNotifyIcon);
            //MyNotifyIconMenu.AppExitRequested += MyNotifyIconMenu_AppExitRequested;

            MyNotifyIconPopup = new NotifyIconPopup();
            MyNotifyIconPopup.AppExitRequested += MyNotifyIconMenu_AppExitRequested;
            MyNotifyIcon.MouseClick += MyNotifyIcon_MouseClick;

            //TbZoomDataGrid.LinkedZoomableControl = MyViewsGrid.MyDataGrid;
            //TbZoomGroupList.LinkedZoomableControl = MyViewsGrid.MyClockGroupListView;
            //TbZoomList.LinkedZoomableControl = MyViewsGrid.MyFlowView;
        }
    }
}
