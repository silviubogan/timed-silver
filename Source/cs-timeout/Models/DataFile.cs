using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Windows.Shell;
using System.Windows.Threading;
using System.Xml;

namespace cs_timed_silver
{
    public class DataFile : INotifyPropertyChanged, IUnsavedStatusCapable, IEquatable<DataFile>
    {
        internal FileSystemWatcher MyFSWatcher { get; set; } = null;

        internal MultiAudioPlayer MultiAudioPlayer { get; set; }

        public SettingDataMCollection Settings { get; set; } = null;

        public ObservableCollection<SettingDataVM> SettingsVMs { get; set; } = null;

        public ClockVMCollection ClockVMCollection { get; set; }

        internal BeepCollection Beeps = new BeepCollection();

        internal DailyBackupSystem MyDailyBackupSystem = null;
        internal bool HasDailyBackupSystem
        {
            get
            {
                return MyDailyBackupSystem != null;
            }
        }

        public bool IsUnsavedLocked { get; set; } = false;

        internal DispatcherTimer AutoSaveTimer;

        internal string _FilePath = "";
        public string FilePath
        {
            get
            {
                return _FilePath;
            }
            set
            {
                if (value != _FilePath)
                {
                    _FilePath = value;
                    OnPropertyChanged();
                    OnFilePathChanged(EventArgs.Empty);
                }
            }
        }

        internal MainWindow MainWindow;



        [Obsolete]
        internal bool SettingsAreUnsaved
        {
            get
            {
                return Settings.IsUnsaved;
            }
            set
            {
                Settings.IsUnsaved = value;
            }
        }

        private bool _IsUnsaved = false;
        /// <summary>
        /// Global IsUnsaved flag (combining ClockCollecton & Settings & BeepsDataTable).
        /// </summary>
        public bool IsUnsaved
        {
            get
            {
                return _IsUnsaved;
            }
            set
            {
                if (value != _IsUnsaved && !IsUnsavedLocked)
                {
                    _IsUnsaved = value;
                    OnPropertyChanged();
                    OnIsUnsavedChanged(EventArgs.Empty);
                }
            }
        }

        [Obsolete("Indirection.")]
        public bool SettingsAreUnsavedLocked
        {
            get => Settings.IsUnsaved;
            set => Settings.IsUnsaved = value;
        }

        protected virtual void OnIsUnsavedChanged(EventArgs e)
        {
            if (AvailableToUpdateOrApplyGlobalIsUnsaved)
            {
                AvailableToUpdateOrApplyGlobalIsUnsaved = false;
                ClockVMCollection.Model.IsUnsaved = IsUnsaved;
                Settings.IsUnsaved = IsUnsaved;
                BeepsDataTableIsUnsaved = IsUnsaved;
                AvailableToUpdateOrApplyGlobalIsUnsaved = true;
            }

            IsUnsavedChanged?.Invoke(this, e);
        }

        /// <summary>
        /// A new seting must:
        /// - have a default value and a header;
        /// - be loaded in the settings form;
        /// - be changeable in the settings form;
        /// - be used in the code for the desired effect, which usually means only:
        /// - - update setting value and, maybe, in the same place,
        /// - - applied/propagated (LoadFromString, then ApplySettingsFromDataFile);
        /// - saved to file (GenerateAllDataXMLString);
        /// </summary>
        /// <param name="mf"></param>
        public DataFile(bool withBackup = false)
        {
            MainWindow = System.Windows.Application.Current.MainWindow as MainWindow;

            Initialize(withBackup);
        }

        private void Initialize(bool withBackup)
        {
            SetUpSettings();

            ClockVMCollection = new ClockVMCollection(this);
            ClockVMCollection.Init();
            ClockVMCollection.VMs.CollectionChanged += VMs_CollectionChanged;

            AutoSaveTimer = new DispatcherTimer(DispatcherPriority.Background)
            {
                IsEnabled = true
            };
            UpdateAutoSaveTimerInterval();
            AutoSaveTimer.Tick += AutoSaveTimer_Tick;

            if (withBackup)
            {
                MyDailyBackupSystem = new DailyBackupSystem(this, new DateTimeProvider());

                Settings["AutomaticBackup"].Changed += Settings_AutomaticBackup_Changed;
            }

            MultiAudioPlayer = new MultiAudioPlayer(this);
        }

        private void VMs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //if (ClockVMCollection.VMs.Count == 0)
            //{
            //    MainWindow.MyTaskbarItemInfo.ThumbnailClipMargin = new System.Windows.Thickness(0d);
            //}
            //else
            //{
            //    var el = MainWindow.MyViewsGrid.MyFlowView.MyItemsControl.
            //        ItemContainerGenerator.ContainerFromIndex(0) as System.Windows.FrameworkElement;

            //    if (el == null)
            //    {
            //        return;
            //    }

            //    System.Windows.Rect r = el.TransformToVisual(MainWindow)
            //                .TransformBounds(System.Windows.Controls.Primitives.LayoutInformation.GetLayoutSlot(el));

            //    MainWindow.MyTaskbarItemInfo.ThumbnailClipMargin = new System.Windows.Thickness(r.Left, r.Top,
            //        r.Right, r.Bottom);
            //}
        }

        private void SetUpSettings()
        {
            Settings = new SettingDataMCollection(this);
            Settings.IsUnsavedChanged += SettingCollection_IsUnsavedChanged;
            Settings.SettingValueChange += SettingsCollection_SettingValueChange;

            // TODO: set accelerators automatically when needed (when the SettingsForm will dynamically generate its UI).
            // TODO: more accelerators anyway!
            Settings.Add(new SettingDataM(typeof(bool), typeof(CheckBoxSettingDataVM), "AutosaveEnabled", false)
            {
                Header = "A_utosave",
                Priority = 16.5f,
                Category = "Autosave"
            });
            Settings.Add(new SettingDataM(typeof(TimeSpan), typeof(TimeSpanSettingDataVM), "AutosaveEvery", TimeSpan.FromSeconds(15),
                (object val) =>
                {
                    if (val == null)
                    {
                        return null;
                    }

                    var v = (TimeSpan)val;

                    if (v.TotalMilliseconds <= 0)
                    {
                        v = TimeSpan.FromSeconds(1);
                    }

                    return v;
                })
            {
                Header = "_Autosave every",
                Priority = 17f,
                Category = "Autosave"
            });
            Settings.Add(new SettingDataM(typeof(decimal), null, "GlobalZoomPercent", 100M)
            {
                Header = "List view zoom percent",
                Priority = 12f
            });
            Settings.Add(new SettingDataM(typeof(decimal), null, "GlobalDataGridZoomPercent", 100M)
            {
                Header = "Data grid zoom percent",
                Priority = 13f
            });
            Settings.Add(new SettingDataM(typeof(string), typeof(AudioFileSettingDataVM), "AudioFilePath", "")
            {
                Header = "Audio file path",
                Priority = 1f
            });
            Settings.Add(new SettingDataM(typeof(string), null, "RecentAudioFilePaths", "")
            {
                Header = "Recent audio file paths",
                Priority = 2f
            });
            Settings.Add(new SettingDataM(typeof(TimeSpan), typeof(TimeSpanSettingDataVM), "RingingDuration", TimeSpan.Parse("00:01:00"))
            {
                Header = "_Ring for",
                Priority = 5f
            });
            Settings.Add(new SettingDataM(typeof(bool), null, "DarkMode", false)
            {
                Header = "Dark theme enabled",
                Priority = 11f
            });
            Settings.Add(new SettingDataM(typeof(bool), null, "MainFormAlwaysOnTop", false)
            {
                Header = "Main form always on top",
                Priority = 6f
            });
            Settings.Add(new SettingDataM(typeof(bool), typeof(CheckBoxSettingDataVM), "RoundedCorners", true)
            {
                Header = "_Enable rounded corners",
                Priority = 4f
            });
            Settings.Add(new SettingDataM(typeof(string), null, "ViewType", "List")
            {
                Header = "View type",
                Priority = 16f
            });
            Settings.Add(new SettingDataM(typeof(bool), null, "ShowGroupListView", false) // TODO: new setting RememberGroupListViewShown settable in the settings form
            {
                Header = "Show group list view",
                Priority = 9f
            });
            Settings.Add(new SettingDataM(typeof(decimal), null, "GroupListZoomPercent", 100M)
            {
                Header = "Group list view zoom percent",
                Priority = 15f
            });
            Settings.Add(new SettingDataM(typeof(decimal), null, "GroupListToolBarsZoomPercent", 100M)
            {
                Header = "Group list view tool bars' zoom percent",
                Priority = 15f
            });
            Settings.Add(new SettingDataM(typeof(string), null, "AutosortMode", "None")
            {
                Header = "Autosort mode",
                Priority = 20f
            });
            Settings.Add(new SettingDataM(typeof(string), null, "Filter", "")
            {
                Header = "Filter",
                Priority = 21f
            });
            Settings.Add(new SettingDataM(typeof(bool), typeof(CheckBoxSettingDataVM), "ZoomGroupListToolStrip", false)
            {
                Header = "_Zoom group list tool strip",
                Priority = 1f
            });
            Settings.Add(new SettingDataM(typeof(bool), typeof(CheckBoxSettingDataVM), "AlwaysMute", false)
            {
                Header = "_Always mute (the notifications will still produce sounds)",
                Priority = 19f
            });
            Settings.Add(new SettingDataM(typeof(bool), typeof(CheckBoxSettingDataVM), "EnableRingingNotification", false)
            {
                Header = "_Enable ringing notification",
                Priority = 1f
            });
            Settings.Add(new SettingDataM(typeof(string), null, "RecentImageFilePaths", "")
            {
                Header = "Recent image file paths",
                Priority = 3f
            });
            Settings.Add(new SettingDataM(typeof(string), typeof(ImageFileSettingDataVM), "TimeOutFormBackgroundImage", "")
            {
                Header = "Time-out window background image",
                Priority = 1f
            });
            Settings.Add(new SettingDataM(typeof(int), typeof(IntegerSettingDataVM), "TimeOutFormFadeInDuration", 500)
            {
                Header = "Time-out window fade-in animation duration",
                Priority = 1f
            }); // in ms
            Settings.Add(new SettingDataM(typeof(string), null, "MainFormWindowState", "Normal")
            {
                Header = "Main window state",
                Priority = 7f
            });
            Settings.Add(new SettingDataM(typeof(bool), null, "AutoresizeTableColumns", false)
            {
                Header = "Autoresize columns to fill the table width",
                Priority = 27f
            });
            Settings.Add(new SettingDataM(typeof(bool), typeof(CheckBoxSettingDataVM), "AutomaticBackup", false)
            {
                Header = "Automatically create a backup before the first modification in each day in the folder of the opened file",
                Priority = 1f
            });
            Settings.Add(new SettingDataM(typeof(double), null, "EasyViewGroupListWidthPercent", 25D)
            {
                Header = "EasyViewGroupListWidthPercent",
                Priority = 22f
            });
            Settings.Add(new SettingDataM(typeof(double), null, "EasyViewClockListViewSizePercent", 50D)
            {
                Header = "EasyViewClockListViewSizePercent",
                Priority = 23f
            });
            Settings.Add(new SettingDataM(typeof(double), null, "EasyViewGroupListScrolledPercent", 0D)
            {
                Header = "EasyViewGroupListScrolledPercent",
                Priority = 24f
            });
            Settings.Add(new SettingDataM(typeof(double), null, "EasyViewClockListViewScrolledPercent", 0D)
            {
                Header = "EasyViewClockListViewScrolledPercent",
                Priority = 25f
            });
            Settings.Add(new SettingDataM(typeof(double), null, "EasyViewClockDataGridScrolledPercent", 0D)
            {
                Header = "EasyViewClockDataGridScrolledPercent",
                Priority = 26f
            });
            Settings.Add(new SettingDataM(typeof(Rectangle), null, "MainFormRectangle", new MainFormRectangleComputation())
            {
                Header = "MainFormRectangle",
                Priority = 8f,
                FirstChangeIsInitialization = true
            });
            Settings.Add(new SettingDataM(typeof(string), null, "GroupListListViewViewType", "Large icons")
            {
                Header = "Group list's list view view type",
                Priority = 14f
            });
            Settings.Add(new SettingDataM(typeof(bool), null,"ShowGlobalStatusBar", true)
            {
                Header = "Show global status bar",
                Priority = 14f
            });
            // TODO: apply this before applying the WindowState saved setting and if it is the case,
            // do not apply the WindowState setting anymore
            Settings.Add(new SettingDataM(typeof(bool), typeof(CheckBoxSettingDataVM), "MinimizeOnOpen", false)
            {
                Header = "Minimize on open",
                Priority = 14f
            });

            CreateSettingsVMs();
        }

        private void CreateSettingsVMs()
        {
            SettingsVMs = new ObservableCollection<SettingDataVM>();
            OnPropertyChanged(nameof(SettingsVMs));

            foreach (KeyValuePair<string, SettingDataM> p in Settings)
            {
                if (p.Value.VMType != null)
                {
                    SettingDataVM v;
                    if (p.Value.VMType == typeof(CheckBoxSettingDataVM))
                    {
                        v = new CheckBoxSettingDataVM(p.Value);
                    }
                    else if (p.Value.VMType == typeof(TimeSpanSettingDataVM))
                    {
                        v = new TimeSpanSettingDataVM(p.Value);
                    }
                    else if (p.Value.VMType == typeof(AudioFileSettingDataVM))
                    {
                        v = new AudioFileSettingDataVM(p.Value);
                    }
                    else if (p.Value.VMType == typeof(ImageFileSettingDataVM))
                    {
                        v = new ImageFileSettingDataVM(p.Value);
                    }
                    else if (p.Value.VMType == typeof(IntegerSettingDataVM))
                    {
                        v = new IntegerSettingDataVM(p.Value);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }

                    if (p.Key == "AutomaticBackup")
                    {
                        v.Actions.Add(new NamedCommandVM("_Delete the backup", new RelayCommand(() =>
                        {
                            if (MyDailyBackupSystem.BackupExists)
                            {
                                MyDailyBackupSystem.DeleteBackup();
                                System.Windows.MessageBox.Show("Backup deleted.", "Information", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                                return;
                            }
                            System.Windows.MessageBox.Show("There is no backup to delete.", "Information", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                        })));

                        v.Actions.Add(new NamedCommandVM("_Make backup now", new RelayCommand(() =>
                        {
                            // NOTE: better without deactivated buttons, better with message box
                            // because the file system is always in change, and the message box
                            // informs the user better than a tooltip

                            if (FilePath != "") // the file is on the disk
                            {
                                MyDailyBackupSystem.DoBackup();
                                System.Windows.MessageBox.Show(
                                    "Backup done.",
                                    "Information",
                                    System.Windows.MessageBoxButton.OK,
                                    System.Windows.MessageBoxImage.Information);
                                return;
                            }
                            System.Windows.MessageBox.Show(
                                "Backup not possible because the open file is not saved.",
                                "Information",
                                System.Windows.MessageBoxButton.OK,
                                System.Windows.MessageBoxImage.Information);
                        })));
                    }
                    SettingsVMs.Add(v);
                }
            }

            SettingDataVM vmAutosaveEnabled = null;

            foreach (SettingDataVM vm in SettingsVMs)
            {
                if (vm.M.Name == "AutosaveEnabled")
                {
                    vmAutosaveEnabled = vm;
                    break;
                }
            }

            SettingDataVM vmAutosaveEvery = null;

            foreach (SettingDataVM vm in SettingsVMs)
            {
                if (vm.M.Name == "AutosaveEvery")
                {
                    vmAutosaveEvery = vm;
                    break;
                }
            }

            vmAutosaveEvery.ParentSetting = vmAutosaveEnabled;
        }

        private void Settings_AutomaticBackup_Changed(object sender, EventArgs e)
        {
            if (FilePath == "" || IsBeingClosed)
            {
                return;
            }

            if ((bool)Settings.GetValue("AutomaticBackup"))
            {
                // force creation of copy .bak
                MyDailyBackupSystem.DoBackup(true);
            }
            else
            {
                // delete the .bak file if it exists
                MyDailyBackupSystem.DeleteBackup();
            }
        }

        private void SettingsCollection_SettingValueChange(object sender, SettingValueChangedEventArgs e)
        {
            string pn = (sender as SettingDataM).Name;

            if (pn == "AutosaveEvery" ||
                pn == "AutosaveEnabled")
            {
                UpdateAutoSaveTimerInterval();
            }
        }

        internal class DateTimeProvider : IDateTimeProvider
        {
            public DateTime CurrentDateTime => DateTime.Now;

            public DateTime TodayDateTime => DateTime.Today;
        }

        internal bool AvailableToUpdateOrApplyGlobalIsUnsaved = true;

        internal void SettingCollection_IsUnsavedChanged(object sender, EventArgs e)
        {
            SettingsAreDirtyChanged?.Invoke(this, e);

            UpdateGlobalIsUnsaved();
        }

        internal void UpdateGlobalIsUnsaved()
        {
            if (AvailableToUpdateOrApplyGlobalIsUnsaved)
            {
                AvailableToUpdateOrApplyGlobalIsUnsaved = false;
                IsUnsaved = Settings.IsUnsaved ||
                    ClockVMCollection.Model.IsUnsaved ||
                    BeepsDataTableIsUnsaved;
                AvailableToUpdateOrApplyGlobalIsUnsaved = true;
            }
        }

        internal void UpdateAutoSaveTimerInterval()
        {
            UpdateAutoSaveTimerInterval((bool)Settings.GetValue("AutosaveEnabled"),
                (int)((TimeSpan)Settings.GetValue("AutosaveEvery")).TotalMilliseconds);
        }

        internal void UpdateAutoSaveTimerInterval(bool enabled, int ms)
        {
            if (enabled)
            {
                AutoSaveTimer.Start();
            }
            else
            {
                AutoSaveTimer.Stop();
            }
            AutoSaveTimerInterval = ms;
        }

        internal bool AutoSaveTimerIsEnabled
        {
            get
            {
                return AutoSaveTimer.IsEnabled;
            }
            set
            {
                AutoSaveTimer.IsEnabled = value;
            }
        }

        internal int AutoSaveTimerInterval
        {
            get
            {
                return (int)AutoSaveTimer.Interval.TotalMilliseconds;
            }
            set
            {
                if (value == 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                AutoSaveTimer.Interval = TimeSpan.FromMilliseconds(value);
            }
        }

        private void AutoSaveTimer_Tick(object sender, EventArgs e)
        {
            UpdateAutoSaveTimerInterval();

            DoAutoSave(MainWindow);
        }

        internal void DoAutoSave(ISaveable saveable)
        {
            if ((bool)Settings.GetValue("AutosaveEnabled") == true)
            {
                saveable.SaveIfOnDisk();
            }
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        protected void OnPropertyChanged(
            [CallerMemberName]
            string propertyName = "")
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected void OnFilePathChanged(EventArgs e)
        {
            if (FilePath == "")
            {
                MyFSWatcher?.Dispose();
            }
            else
            {
                MyFSWatcher?.Dispose();
                MyFSWatcher = new FileSystemWatcher();
                MyFSWatcher.Path = Path.GetDirectoryName(Path.GetFullPath(FilePath));
                MyFSWatcher.NotifyFilter = NotifyFilters.LastWrite;
                MyFSWatcher.Filter = "*.xml";
                MyFSWatcher.Changed += MyFSWatcher_Changed;
                MyFSWatcher.IncludeSubdirectories = false;
                MyFSWatcher.EnableRaisingEvents = true;
            }

            FilePathChanged?.Invoke(this, e);
        }

        private bool IsProcessingFSChange = false; // used just in the below method
        private void MyFSWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath == FilePath &&
                e.ChangeType == WatcherChangeTypes.Changed &&
                !IsProcessingFSChange)
            {
                var wpfAction = new Action(() =>
                {
                    IsProcessingFSChange = true;
                    System.Windows.MessageBoxResult r = System.Windows.MessageBox.Show(MainWindow, $"The currently opened file ({e.FullPath}) has been changed by another program. Press Yes to load the changes into Timed Silver, No to save the changes from Timed Silver or Cancel to do nothing.", "Question", System.Windows.MessageBoxButton.YesNoCancel,
                    System.Windows.MessageBoxImage.Warning);
                    IsProcessingFSChange = false;

                    if (r == System.Windows.MessageBoxResult.Yes)
                    {
                        ReloadOpenFileFromFileSystem();
                    }
                    else if (r == System.Windows.MessageBoxResult.No)
                    {
                        Save();
                    }
                    else
                    {

                    }
                });
                MainWindow.Dispatcher.BeginInvoke(wpfAction);
            }
        }

        protected void OnFileLoaded(EventArgs e)
        {
            FileLoaded?.Invoke(this, e);
        }

        public event EventHandler FilePathChanged, IsUnsavedChanged,
            SettingsAreDirtyChanged,
            ResetAllDone, FileLoaded;
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<FileClosedEventArgs> FileClosed;

        internal string ErrorString { get; set; } = "";

        /// <summary>
        /// Gets or sets whether the program is currently loading things from the data file.
        /// </summary>
        internal bool LoadingFromXmlDocument { get; set; } = false;

        public bool LoadFromXmlDocument(XmlDocument doc)
        {
            LoadingFromXmlDocument = true;

            XmlElement root = doc.DocumentElement;


            ClockVMCollection.Model.IsUnsavedLocked = true;

            ClockVMCollection.Model.RemoveAllClocks();
            ClockVMCollection.Model.Groups.ClearWithoutChangingClocks();

            foreach (XmlElement el in doc.SelectNodes("/*/Groups/Group"))
            {
                LoadGroup(el);
            }

            XmlNodeList elBeeps = root.SelectNodes("/*/Beeps/Beeps/Beep");
            if (elBeeps != null)
            {
                Beeps.LoadFromXML(elBeeps);
            }

            var accumulator = new List<ClockM>();
            foreach (XmlElement el in root.SelectNodes("/*/Clocks/Clock"))
            {
                LoadClock(el, accumulator);
            }
            ClockVMCollection.Model.AddClocks(accumulator.ToArray());

            Settings.IsUnsavedLocked = true;

            Settings.ImportFromAttributes(doc);

            Settings.IsUnsavedLocked = false;

            ClockVMCollection.Model.IsUnsavedLocked = false;

            LoadingFromXmlDocument = false;

            return true;
        }

        public bool LoadFromString(string xml)
        {
            if (xml.Length == 0)
            {
                ErrorString = "The file could not be loaded because it is empty.";
                return false;
            }


            var doc = new XmlDocument();
            try
            {
                doc.LoadXml(xml);
            }
            catch (XmlException ex)
            {
                ErrorString = $"The file could not be loaded: {ex.Message}";
                return false;
            }

            LoadFromXmlDocument(doc);

            ErrorString = "";
            return true;
        }

        // TODO: make this method return bool and change the ErrorString
        // when it is the case.
        internal ClockM LoadClock(XmlElement el, List<ClockM> accumulator = null)
        {
            string type = el.GetAttribute("Type");

            ClockM cd;
            if (type == "Timer")
            {
                cd = InitializeTimer(el);
            }
            else // if (type == "Alarm")
            {
                cd = InitializeAlarm(el);
            }

            cd.IsUnsavedLocked = true;

            cd.Tag = XamlReader.Parse(el.GetAttribute("Tag")) as FlowDocument;

            string color = el.GetAttribute("UserBackColor");
            if (!string.IsNullOrEmpty(color))
            {
                cd.UserBackColor = Utils.StringToColor(color);
            }

            bool resetToLocked = el.GetAttribute("ResetToValueLocked") == "True";
            cd.ResetToValueLocked = resetToLocked;

            cd.GroupName = el.GetAttribute("GroupName");

            string s = el.GetAttribute("TimeOutBackgroundImageRelativePath");
            if (!string.IsNullOrEmpty(s))
            {
                cd.TimeOutBackgroundImageRelativePath = s;
            }

            if (el.GetAttribute("Icon") != "")
            {
                // TODO: use new Utils.Base64StringToTransparentBitmap().
                var bmp = new Bitmap(Utils.Base64StringToImage(
                    el.GetAttribute("Icon")));
                bmp.MakeTransparent(Color.Empty);
                cd.Icon = bmp;
            }
            else
            {
                // by default anyway:
                //td.Icon = null;
            }

            try
            {
                cd.Style = Utils.StrToEnum<ClockM.ClockStyles>(el.GetAttribute("Style"));
            }
            catch (Exception /*ex*/)
            {
                cd.Style = ClockM.ClockStyles.ShowID;
            }

            cd.IsUnsavedLocked = false;

            if (accumulator != null)
            {
                accumulator.Add(cd);
            }
            return cd;
        }

        internal ClockM InitializeAlarm(XmlElement el)
        {
            ClockM cd;
            DateTime dt;
            try
            {
                dt = DateTime.Parse(el.GetAttribute("CurrentValue"), CultureInfo.InvariantCulture);
            }
            catch (Exception /*ex*/)
            {
                dt = DateTime.Now;
            }

            DateTime resetTo;
            try
            {
                resetTo = DateTime.Parse(el.GetAttribute("ResetToValue"), CultureInfo.InvariantCulture);
            }
            catch (Exception /*ex*/)
            {
                resetTo = DateTime.Now;
            }

            cd = new AlarmData(this, MultiAudioPlayer);
            cd.IsUnsavedLocked = true;
            cd.CurrentValue = dt;
            cd.ResetToValue = resetTo;
            if (el.GetAttribute("Enabled") == "True")
            {
                cd.ActivateOrDeactivate();
            }
            cd.IsUnsavedLocked = false;

            return cd;
        }

        internal ClockM InitializeTimer(XmlElement el)
        {
            ClockM cd;
            TimeSpan ts;
            try
            {
                ts = TimeSpan.Parse(el.GetAttribute("CurrentValue"));
            }
            catch (Exception /*ex*/)
            {
                ts = TimeSpan.FromMinutes(0);
            }

            TimeSpan resetTo;
            try
            {
                resetTo = TimeSpan.Parse(el.GetAttribute("ResetToValue"));
            }
            catch (Exception /*ex*/)
            {
                resetTo = TimeSpan.FromMinutes(0);
            }

            cd = new TimerData(this, MultiAudioPlayer);
            cd.IsUnsavedLocked = true;
            cd.CurrentValue = ts;
            cd.ResetToValue = resetTo;
            return cd;
        }

        internal void LoadGroup(XmlElement el)
        {
            string g = el.GetAttribute("Name");

            Bitmap bmp = null;

            System.Windows.Media.ImageSource bmpi = null;

            if (el.GetAttribute("Icon") != "")
            {
                bmp = new Bitmap(Utils.Base64StringToImage(el.GetAttribute("Icon")));
                bmp.MakeTransparent(Color.Empty);
                bmpi = Utils.GetBitmapImageFromBitmap(bmp);
                bmp.Dispose();
            }

            ClockVMCollection.Model.Groups.Add(new ClockGroupM()
            {
                Name = g,
                Icon = bmpi
            });
        }

        internal bool SetWithoutApply { get; set; } = false;

        internal bool ValidateFile(string path,
            out XmlDocument doc,
            out string error)
        {
            try
            {
                var xd = new XmlDocument();
                xd.Load(path);
                doc = xd;
                error = "";
            }
            catch (Exception ex)
            {
                // I have many older files with UTF-8 signature conflicting with the XML declaration which says UTF-16.
                if (ex.Message == "There is no Unicode byte order mark. Cannot switch to Unicode.")
                {
                    var xd = new XmlDocument();
                    xd.LoadXml(File.ReadAllText(path));
                    foreach (XmlNode node in xd)
                    {
                        if (node.NodeType == XmlNodeType.XmlDeclaration)
                        {
                            xd.RemoveChild(node);
                        }
                    }
                    xd.Save(path);

                    return ValidateFile(path, out doc, out error);
                }
                error = ex.Message;
                doc = null;
                return false;
            }
            return true;
        }

        internal string LastUsedFolder { get; set; } = "";

        public bool LoadFromFileWPF(string filePath = "")
        {
            if (filePath == "")
            {
                Microsoft.Win32.OpenFileDialog fd =
                    Utils.GetDataFileOpenerWPF();

                if (LastUsedFolder == "")
                {
                    fd.InitialDirectory = Environment.GetFolderPath(
                        Environment.SpecialFolder.Personal);
                }
                else
                {
                    fd.InitialDirectory = LastUsedFolder;
                }

                fd.RestoreDirectory = true; // not used

                if ((bool)fd.ShowDialog(MainWindow))
                {
                    LastUsedFolder = Path.GetDirectoryName(Path.GetFullPath(fd.FileName));

                    Close();

                    string fp = fd.FileName;

                    string err;
                    XmlDocument xd;
                    if (ValidateFile(fp, out xd, out err))
                    {
                        Close(true);

                        // TODO:
                        //MainForm.MyJumpList.JumpItems.Add(new JumpPath()
                        //{
                        //    Path = fp
                        //});
                        //MainForm.MyJumpList.Apply();


                        LoadFromXmlDocument(xd);

                        OnFileLoaded(EventArgs.Empty);

                        FilePath = fp;
                        IsUnsaved = false;

                        return true;
                    }
                    else
                    {
                        System.Windows.MessageBox.Show(err);
                        Properties.Settings.Default.LastOpenFile = "";
                        Properties.Settings.Default.Save();

                        return false;
                    }
                }
            }
            else
            {
                string err;
                XmlDocument xd;
                if (ValidateFile(filePath, out xd, out err))
                {
                    Close(true);

                    // TODO:
                    //MainForm.MyJumpList.JumpItems.Add(new JumpPath()
                    //{
                    //    Path = filePath
                    //});
                    //MainForm.MyJumpList.Apply();


                    LoadFromXmlDocument(xd);

                    OnFileLoaded(EventArgs.Empty);

                    FilePath = filePath;
                    IsUnsaved = false;

                    return true;
                }
                else
                {
                    System.Windows.MessageBox.Show(err);
                    Properties.Settings.Default.LastOpenFile = "";
                    Properties.Settings.Default.Save();
                    return false;
                }
            }
            return false;
        }

        // TODO: used somewhere in the DailyBackupSystem & MainForm.
        //public void Reload()
        //{

        //}

        internal event EventHandler BeforeSave, AfterSave;

        public void Save()
        {
            if (FilePath.Length != 0)
            {
                if (MyFSWatcher != null)
                {
                    MyFSWatcher.EnableRaisingEvents = false;
                }

                BeforeSave?.Invoke(this, EventArgs.Empty);

                // simple Save
                File.WriteAllText(FilePath,
                    GenerateAllDataXMLString(), System.Text.Encoding.Unicode);

                IsUnsaved = false;

                AfterSave?.Invoke(this, EventArgs.Empty);

                if (MyFSWatcher != null)
                {
                    MyFSWatcher.EnableRaisingEvents = true;
                }
            }
        }

        public void SaveAs(string filePath)
        {
            BeforeSave?.Invoke(this, EventArgs.Empty);

            File.WriteAllText(filePath,
                GenerateAllDataXMLString(), System.Text.Encoding.Unicode);

            IsUnsaved = false;
            FilePath = filePath;

            AfterSave?.Invoke(this, EventArgs.Empty);
        }

        // TODO: make this a property.
        internal bool BeepsDataTableIsUnsaved = false;

        /// <summary>
        /// Marks IsDirty as true only if a setting is dirty,
        /// not because of timers.
        /// </summary>
        internal void ResetAll()
        {
            Settings.ResetAllValues();

            Beeps.Clear();

            ClockVMCollection.Model.IsUnsavedLocked = true;
            ClockVMCollection.Model.RemoveAllClocks();
            ClockVMCollection.Model.Groups.ClearWithoutChangingClocks();
            ClockVMCollection.Model.IsUnsavedLocked = false;

            ResetAllDone?.Invoke(this, EventArgs.Empty);
        }

        internal string GenerateAllDataXMLString()
        {
            var doc = new XmlDocument();
            XmlElement root = doc.CreateElement("TimedSilver");
            root.SetAttribute("version", Assembly.GetAssembly(typeof(MainWindow)).GetName().Version.ToString(2));
            doc.AppendChild(root);

            Settings.ExportAsAttributes(root);

            XmlElement elBeeps = doc.CreateElement("Beeps");
            elBeeps.InnerXml = Beeps.ToXMLString();
            root.AppendChild(elBeeps);

            XmlElement elGroups = doc.CreateElement("Groups");
            foreach (ClockGroupM g in ClockVMCollection.Model.Groups.Ms)
            {
                XmlElement elGrup = doc.CreateElement("Group");
                elGrup.SetAttribute("Name", g.Name);

                string icon = g.Icon == null ?
                    "" :
                    Utils.ImageToBase64((System.Windows.Media.Imaging.BitmapSource)g.Icon);

                elGrup.SetAttribute("Icon", icon);

                elGroups.AppendChild(elGrup);
            }
            root.AppendChild(elGroups);

            XmlElement elClocks = doc.CreateElement("Clocks");
            root.AppendChild(elClocks);

            foreach (ClockM td in ClockVMCollection.Model.Ms)
            {
                XmlElement elNode = doc.CreateElement("Clock");

                XmlAttribute attrType = doc.CreateAttribute("Type");
                attrType.Value = td is TimerData ? "Timer" : "Alarm";

                XmlAttribute attrEnabled = null;
                if (attrType.Value == "Alarm")
                {
                    attrEnabled = doc.CreateAttribute("Enabled");
                    attrEnabled.Value = td.Enabled.ToString();
                }

                XmlAttribute attrSeconds =
                    doc.CreateAttribute("CurrentValue");
                attrSeconds.Value = td.CurrentValue == null ?
                    "" :
                    (td is TimerData ?
                        td.CurrentValue.ToString() :
                        ((DateTime)td.CurrentValue).ToString
                            (CultureInfo.InvariantCulture)
                    );

                XmlAttribute attrTag = doc.CreateAttribute("Tag");
                attrTag.InnerText = XamlWriter.Save(td.Tag);

                XmlAttribute attrBackgroundPath = doc.CreateAttribute("TimeOutBackgroundImageRelativePath");
                attrBackgroundPath.Value = td.TimeOutBackgroundImageRelativePath ?? "";

                XmlAttribute attrResetToValue = doc.CreateAttribute("ResetToValue");
                attrResetToValue.Value = td.ResetToValue == null ?
                    "" :
                    (td is TimerData ?
                        td.ResetToValue.ToString() :
                        ((DateTime)td.ResetToValue).ToString
                            (CultureInfo.InvariantCulture)
                    );

                XmlAttribute attrResetToValueLocked = doc.CreateAttribute("ResetToValueLocked");
                attrResetToValueLocked.Value = td.ResetToValueLocked.ToString();

                XmlAttribute attrStyle = doc.CreateAttribute("Style");
                attrStyle.Value = td.Style.ToString();

                XmlAttribute attrIcon = doc.CreateAttribute("Icon");
                if (td.Icon == null)
                {
                    attrIcon.Value = "";
                }
                else
                {
                    attrIcon.Value = Utils.ImageToBase64String(td.Icon);
                }

                if (td.GroupName != "")
                {
                    XmlAttribute attrGroupName = doc.CreateAttribute("GroupName");
                    attrGroupName.Value = td.GroupName;
                    elNode.Attributes.Append(attrGroupName);
                }

                if (!Utils.ColorsAreTheSame(td.UserBackColor, Color.Empty))
                {
                    XmlAttribute attrUserBackColor =
                        doc.CreateAttribute("UserBackColor");
                    attrUserBackColor.Value =
                        Utils.ColorToString(td.UserBackColor);
                    elNode.Attributes.Append(attrUserBackColor);
                }

                elNode.Attributes.Append(attrType);
                elNode.Attributes.Append(attrSeconds);
                elNode.Attributes.Append(attrTag);
                elNode.Attributes.Append(attrResetToValue);
                elNode.Attributes.Append(attrResetToValueLocked);
                elNode.Attributes.Append(attrStyle);
                elNode.Attributes.Append(attrIcon);
                elNode.Attributes.Append(attrBackgroundPath);
                if (attrEnabled != null)
                {
                    elNode.Attributes.Append(attrEnabled);
                }

                elClocks.AppendChild(elNode);
            }
            
            return Utils.XmlDocumentToString(doc);
        }

        //internal bool FirstClose = true;

        internal bool IsBeingClosed = false;

        internal void Close(bool otherFileWillBeOpened = false)
        {
            MyFSWatcher?.Dispose();

            IsBeingClosed = true;

            if (!MainWindow.MainFormIsBeingClosed)// && 
                                                //    (!FirstClose && otherFileWillBeOpened)) // not working when the user changes the new file and presses New (the old Close) again without saving to disk
            {
                ResetAll();
                FilePath = "";
            }

            IsUnsaved = false;

            //if (FirstClose)
            //{
            //    FirstClose = false;
            //}

            IsBeingClosed = false;

            OnFileClosed(new FileClosedEventArgs()
            {
                OtherFileWillBeOpened = otherFileWillBeOpened
            });
        }

        private void OnFileClosed(FileClosedEventArgs e)
        {
            FileClosed?.Invoke(this, e);
        }

        internal void CloseAndOpenOtherFileWPF()
        {
            if (IsUnsaved)
            {
                MainWindow.IsFileBeingClosed = true;

                System.Windows.MessageBoxResult r = System.Windows.MessageBox.Show(MainWindow, "There are unsaved changes. Do you want to save the changes before closing?", "Confirmation request", System.Windows.MessageBoxButton.YesNoCancel);
                if (r == System.Windows.MessageBoxResult.No)
                {
                    InternalShowOpenFileDialogWPF();
                    MainWindow.IsFileBeingClosed = false;
                    return;
                }
                else if (r == System.Windows.MessageBoxResult.Yes)
                {
                    Save();
                    InternalShowOpenFileDialogWPF();
                    MainWindow.IsFileBeingClosed = false;
                    return;
                }
                else // Cancel
                {
                    return;
                }
            }
            else
            {
                InternalShowOpenFileDialogWPF();
                MainWindow.IsFileBeingClosed = false;
            }
        }

        /// <summary>
        /// Returns true if the file path is opened.
        /// (This method asks the user for a confirmation.)
        /// </summary>
        /// <param name="fp"></param>
        /// <returns></returns>
        internal bool CloseAndOpenOtherFileWPF(string fp)
        {
            if (IsUnsaved)
            {
                string err;
                XmlDocument xd;
                bool v = ValidateFile(fp, out xd, out err);

                if (v)
                {
                    System.Windows.MessageBoxResult r = System.Windows.MessageBox.Show(MainWindow, "There are unsaved changes. Do you want to save the changes before closing?", "Confirmation request", System.Windows.MessageBoxButton.YesNoCancel);
                    if (r == System.Windows.MessageBoxResult.No)
                    {
                        if (LoadFromFileWPF(fp))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (r == System.Windows.MessageBoxResult.Yes)
                    {
                        Save();
                        if (LoadFromFileWPF(fp))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else // Cancel
                    {
                        return false;
                    }
                }
                else // invalid file chosen
                {
                    LoadFromFileWPF(fp);
                    return false;
                }
            }
            else
            {
                if (LoadFromFileWPF(fp))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        internal bool InternalShowOpenFileDialogWPF()
        {
            Microsoft.Win32.OpenFileDialog fd = Utils.GetDataFileOpenerWPF();

            if (LastUsedFolder == "")
            {
                fd.InitialDirectory = Environment.GetFolderPath(
                    Environment.SpecialFolder.Personal);
            }
            else
            {
                fd.InitialDirectory = LastUsedFolder;
            }

            if ((bool)fd.ShowDialog(MainWindow))
            {
                string fp = fd.FileName;

                MainWindow.IsFileBeingClosed = false;

                if (LoadFromFileWPF(fp))
                {
                    MainWindow.ApplySettingsFromDataFile();

                    FilePath = fp;
                    IsUnsaved = false;

                    return true;
                }
                else
                {
                    MainWindow.IsFileBeingClosed = false;
                    return false;
                }
            }

            MainWindow.IsFileBeingClosed = false;
            return false;
        }

        public bool Equals(DataFile other)
        {
            if (Beeps.ToXMLString() != other.Beeps.ToXMLString())
            {
                return false;
            }
            if (ClockVMCollection.Model != other.ClockVMCollection.Model)
            {
                return false;
            }
            if (Settings != other.Settings)
            {
                return false;
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var df = obj as DataFile;
            if (df == null)
                return false;
            else
                return Equals(df);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = ClockVMCollection.Model.GetHashCode();
                
                hash ^= Settings.GetHashCode();

                hash ^= Beeps.GetHashCode();

                return hash;
            }
        }

        public static bool operator ==(DataFile df1, DataFile df2)
        {
            if (((object)df1) == null || ((object)df2) == null)
                return object.Equals(df1, df2);

            return df1.Equals(df2);
        }

        public static bool operator !=(DataFile df1, DataFile df2)
        {
            return !(df1 == df2);
        }

        [Obsolete]
        internal object GetValue(string settingKey)
        {
            return Settings.GetValue(settingKey);
        }

        [Obsolete]
        internal void SetValue(string settingKey, object val)
        {
            Settings.SetValue(settingKey, val);
        }

        internal void SetValueWithoutApply(string settingKey, object val)
        {
            bool stateBefore = SetWithoutApply;

            SetWithoutApply = true;
            Settings.SetValue(settingKey, val);

            SetWithoutApply = stateBefore;
        }

        internal void ReloadOpenFileFromFileSystem()
        {
            string fp = FilePath;

            if (MainWindow.CloseFile(false))
            {
                if (!LoadFromFileWPF(fp))
                {
                    System.Windows.MessageBox.Show("An error was received while loading the file.");
                }
            }
        }
    }
}
