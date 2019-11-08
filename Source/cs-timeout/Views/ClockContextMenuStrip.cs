using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using Image = System.Windows.Controls.Image;

namespace cs_timed_silver
{
    internal class ClockContextMenuStrip : ContextMenu
    {
        private MenuItem tsmiSelectColor;
        private MenuItem tsmiRemoveCustomColor;
        private MenuItem tsmiGroupName;
        private MenuItem tsmiResetTo;
        private MenuItem tsmiSelectIcon;
        private MenuItem tsmiSelectSuggestedColor;
        private Separator tss1, tss2, tss3, tss4, tss5;
        private MenuItem tsmiRemoveIcon;
        //private ToolStripMenuItem tsmiAlwaysShowID;
        private MenuItem tsmiUndock;
        internal MenuItem tsmiDelete, tsmiRename,
            tsmiActivateDezactivate, tsmiSetCurrentValue,
            tsmiSetRandomColors, tsmiConvertToOtherType,
            tsmiAdvanced, tsmiSaveIconAs, tsmiOpenTimeOutBackgroundImageFile,
            tsmiRemoveTimeOutBackgroundImageFile,
            tsmiChooseTimeOutBackgroundImageFile,
            tsmiSetToSunrise, tsmiSetToSunset;

        internal HashSet<ClockVM> _MyClocks = null;
        internal HashSet<ClockVM> MyClocks
        {
            get
            {
                return _MyClocks;
            }
            set
            {
                if (_MyClocks != value)
                {
                    _MyClocks = value;
                }
            }
        }

        internal ClockVM MyClock
        {
            get
            {
                if (MyClocks == null)
                {
                    return null;
                }

                return MyClocks.FirstOrDefault();
            }
        }

        internal MainWindow MainWindow = null;

        /// <summary>
        /// After c-tor call, MyClocks property should be assigned.
        /// </summary>
        internal ClockContextMenuStrip() : base()
        {
            MainWindow = System.Windows.Application.Current.MainWindow as MainWindow;

            Initialize();
        }

        private void Initialize()
        {
            //ImageScalingSize = new Size(28, 28);
            //ShowCheckMargin = false;
            //ShowImageMargin = true;


            tsmiUndock = new MenuItem();

            tss1 = new Separator();

            tsmiResetTo = new MenuItem();
            tsmiGroupName = new MenuItem();

            tss2 = new Separator();

            tsmiSelectColor = new MenuItem();
            tsmiRemoveCustomColor = new MenuItem();

            tss3 = new Separator();

            tsmiSelectIcon = new MenuItem();
            tsmiRemoveIcon = new MenuItem();
            //tsmiAlwaysShowID = new ToolStripMenuItem();

            tss4 = new Separator();

            tsmiDelete = new MenuItem();
            tsmiRename = new MenuItem();
            tsmiActivateDezactivate = new MenuItem();
            tsmiSetCurrentValue = new MenuItem();
            tsmiSetRandomColors = new MenuItem();
            tsmiConvertToOtherType = new MenuItem();

            tss5 = new Separator();

            tsmiAdvanced = new MenuItem();
            tsmiSaveIconAs = new MenuItem();
            tsmiSelectSuggestedColor = new MenuItem();

            Items.Add(tsmiUndock);

            Items.Add(tss1);

            Items.Add(tsmiResetTo);
            Items.Add(tsmiGroupName);

            Items.Add(tss2);

            Items.Add(tsmiSelectColor);
            Items.Add(tsmiRemoveCustomColor);

            Items.Add(tss3);

            Items.Add(tsmiSelectIcon);
            Items.Add(tsmiRemoveIcon);

            Items.Add(tss4);

            Items.Add(tsmiActivateDezactivate);
            Items.Add(tsmiSetCurrentValue);
            Items.Add(tsmiDelete);
            Items.Add(tsmiRename);
            Items.Add(tsmiSetRandomColors);
            Items.Add(tsmiConvertToOtherType);

            Items.Add(tss5);

            Items.Add(tsmiAdvanced);



            // 
            // tsmiUndock
            // 
            tsmiUndock.Header = "_Undock...";
            tsmiUndock.Click += tsmiUndock_Click;
            tsmiUndock.Icon = CreateIcon("Oxygen-Icons.org-Oxygen-Actions-window-new.ico");

            // 
            // tsmiResetTo
            // 
            tsmiResetTo.Header = "Reset t_o";
            //tsmiResetTo.ContextMenuOpening += tsmiResetTo_DropDownOpening;
            tsmiResetTo.PreviewMouseDown += tsmiResetTo_Click;
            tsmiResetTo.Icon = CreateIcon("path1092.png");

            // 
            // tsmiGroupName
            // 
            tsmiGroupName.Header = "_Group...";
            //tsmiGroupName.ContextMenuOpening += tsmiGroupName_DropDownOpening;
            tsmiGroupName.Icon = CreateIcon("folder.ico");

            // 
            // tsmiSelectColor
            // 
            tsmiSelectColor.Header = "Sele_ct color...";
            tsmiSelectColor.Icon = CreateIcon("Oxygen-Icons.org-Oxygen-Actions-format-stroke-color.ico");

            // 
            // tsmiRemoveCustomColor
            // 
            tsmiRemoveCustomColor.Header = "Dele_te user color";
            tsmiRemoveCustomColor.Icon = CreateIcon("remove_custom_color.ico");

            // 
            // tsmiSelectIcon
            // 
            tsmiSelectIcon.Header = "Se_lect icon...";
            tsmiSelectIcon.Click += tsmiSelectIcon_Click;
            tsmiSelectIcon.Icon = CreateIcon("Oxygen-Icons.org-Oxygen-Actions-document-edit.ico");

            // 
            // tsmiRemoveIcon
            // 
            tsmiRemoveIcon.Header = "R_emove icon";
            tsmiRemoveIcon.Click += tsmiRemoveIcon_Click;

            // 
            // tsmiAlwaysShowID
            // 
            //tsmiAlwaysShowID.CheckOnClick = true;
            //tsmiAlwaysShowID.CheckState = CheckState.Checked;
            //tsmiAlwaysShowID.Name = "tsmiAlwaysShowID";
            //tsmiAlwaysShowID.Text = "&Always show ID";
            //tsmiAlwaysShowID.Click += new EventHandler(tsmiAlwaysShowID_Click);
            //tsmiAlwaysShowID.Image = Utils.IconResourceVersionBySize(Properties.Resources.Iconsmind_Outline_ID_2, 16);

            // 
            // tsmiDelete
            // 
            tsmiDelete.Header = "_Delete...";
            tsmiDelete.Click += tsmiDelete_Click;
            tsmiDelete.Icon = CreateIcon("Oxygen-Icons.org-Oxygen-Actions-edit-delete.ico");

            // 
            // tsmiRename
            // 
            tsmiRename.Header = "_Rename...";
            tsmiRename.Click += tsmiRename_Click;
            tsmiRename.Icon = CreateIcon("set_current_value.ico");

            tsmiSetCurrentValue.Header = "Set curre_nt value...";
            tsmiSetCurrentValue.Click += TsmiSetCurrentValue_Click;
            tsmiSetCurrentValue.Icon = CreateIcon("Oxygen-Icons.org-Oxygen-Actions-edit-rename.ico");

            tsmiSetRandomColors.Header = "Set random _colors";
            tsmiSetRandomColors.Click += TsmiSetRandomColors_Click;
            tsmiSetRandomColors.Icon = CreateIcon("path1129.png");

            tsmiConvertToOtherType.Header = "T_oggle Types";
            tsmiConvertToOtherType.ToolTip =
                "From Alarm to Timer, and from Timer to Alarm";
            tsmiConvertToOtherType.Click += TsmiConvertToOtherType_Click;

            tsmiSaveIconAs.Header = "Save _icon as...";
            tsmiSaveIconAs.Icon = CreateIcon("Oxygen-Icons.org-Oxygen-Actions-document-save-as.ico");
            tsmiSaveIconAs.Click += TsmiSaveIconAs_Click;

            tsmiSelectSuggestedColor.Header = "Select suggested color";
            tsmiSelectSuggestedColor.Click += TsmiSelectSuggestedColor_Click;

            tsmiSelectColor.Click += TsmiChangeColor_Click;
            tsmiRemoveCustomColor.Click += TsmiRemoveCustomColor_Click;
            tsmiActivateDezactivate.Click += TsmiActivateDezactivate_Click;

            tsmiOpenTimeOutBackgroundImageFile = new MenuItem();
            tsmiOpenTimeOutBackgroundImageFile.Click += TsmiOpenTimeOutBackgroundImageFile_Click;

            tsmiRemoveTimeOutBackgroundImageFile = new MenuItem();
            tsmiRemoveTimeOutBackgroundImageFile.Header = "Remove Time-out Background Image File";
            tsmiRemoveTimeOutBackgroundImageFile.Click += TsmiRemoveTimeOutBackgroundImageFile_Click;

            tsmiChooseTimeOutBackgroundImageFile = new MenuItem();
            tsmiChooseTimeOutBackgroundImageFile.Header = "Choose a Time-out Background Image File...";
            tsmiChooseTimeOutBackgroundImageFile.Click += TsmiChooseTimeOutBackgroundImageFile_Click;

            tsmiSetToSunrise = new MenuItem();
            tsmiSetToSunrise.Header = "Set to Sunrise";
            tsmiSetToSunrise.Icon = CreateIcon("sunrise.png");

            tsmiSetToSunrise.Click += TsmiSetToSunrise_Click;

            tsmiSetToSunset = new MenuItem();
            tsmiSetToSunset.Header = "Set to Sunset";
            tsmiSetToSunset.Icon = CreateIcon("sunset.png");
            tsmiSetToSunset.Click += TsmiSetToSunset_Click;

            tsmiAdvanced.Header = "Advanced...";
            tsmiAdvanced.Items.Add(tsmiSaveIconAs);
            tsmiAdvanced.Items.Add(tsmiSelectSuggestedColor);
            tsmiAdvanced.Items.Add(new Separator());
            tsmiAdvanced.Items.Add(tsmiOpenTimeOutBackgroundImageFile);
            tsmiAdvanced.Items.Add(tsmiRemoveTimeOutBackgroundImageFile);
            tsmiAdvanced.Items.Add(tsmiChooseTimeOutBackgroundImageFile);
            tsmiAdvanced.Items.Add(new Separator());
            tsmiAdvanced.Items.Add(tsmiSetToSunrise);
            tsmiAdvanced.Items.Add(tsmiSetToSunset);
        }

        private void TsmiSetToSunset_Click(object sender, EventArgs e)
        {
            GetLocationThenApply(false);
        }

        public void ApplySunriseSunset(bool sunrise, double Lat, double Long)
        {
            if (Lat < 0 || Long < 0)
            {
                return;
            }

            foreach (ClockVM c in MyClocks)
            {
                if (c.ClockType == ClockVM.ClockTypes.Alarm)
                {
                    DateTime sunriseOrSunset =
                        Sun.Calculate(c.CurrentDateTime, Lat, Long, sunrise,
                            new Func<DateTime, DateTime>((DateTime dt) =>
                            {
                                return dt.ToLocalTime();
                            }), 91.0d);

                    c.CurrentDateTime = sunriseOrSunset;
                }
            }
        }

        private void TsmiSetToSunrise_Click(object sender, EventArgs e)
        {
            GetLocationThenApply(true);
        }

        private void GetLocationThenApply(bool sunrise)
        {
            double Lat = -1, Long = -1;

            if (!ThereIsAtLeastAClock())
            {
                return;
            }

            var w = new GeoCoordinateWatcher();

            w.PositionChanged += (object s2, GeoPositionChangedEventArgs<GeoCoordinate> e2) =>
            {
                GeoCoordinate coord = e2.Position.Location;
                if (coord.IsUnknown)
                {
                    return;
                }
                Lat = coord.Latitude;
                Long = coord.Longitude;

                ApplySunriseSunset(sunrise, Lat, Long);

                w.Dispose();
            };

            w.TryStart(false, TimeSpan.FromSeconds(10));
        }

        private void UpdateTimeOutBackgroundItems()
        {
            if (!ThereIsAtLeastAClock())
            {
                return;
            }

            if (!string.IsNullOrEmpty(MyClock.TimeOutBackgroundImageRelativePath))
            {
                var ui = new StackPanel()
                {
                    Orientation = Orientation.Vertical,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };
                ui.Children.Add(
                    new TextBlock(
                        new Run(
                            Path.GetFileName(MyClock.TimeOutBackgroundImageRelativePath)))
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(5)
                    });
                ui.Children.Add(new Image()
                {
                    Source = new BitmapImage(
                        new Uri(MyClock.TimeOutBackgroundImageRelativePath,
                            UriKind.RelativeOrAbsolute)),
                    MaxHeight = 200,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(5, 0, 5, 5)
                });

                // TODO: what if the file name has _ in it? it is shown as accelerator, and the _ is removed from the actual string shown in the UI
                tsmiOpenTimeOutBackgroundImageFile.Header = ui;
                tsmiOpenTimeOutBackgroundImageFile.ToolTip = MyClock.TimeOutBackgroundImageRelativePath;
                tsmiOpenTimeOutBackgroundImageFile.FontStyle = System.Windows.FontStyles.Normal;
                tsmiOpenTimeOutBackgroundImageFile.Foreground = System.Windows.SystemColors.ControlTextBrush;
                tsmiOpenTimeOutBackgroundImageFile.IsEnabled = true;

                tsmiRemoveTimeOutBackgroundImageFile.IsEnabled = true;
            }
            else
            {
                tsmiOpenTimeOutBackgroundImageFile.Header = "No Specific Time-out Background";
                tsmiOpenTimeOutBackgroundImageFile.ToolTip = null;
                tsmiOpenTimeOutBackgroundImageFile.FontStyle = System.Windows.FontStyles.Italic;
                tsmiOpenTimeOutBackgroundImageFile.Foreground = System.Windows.SystemColors.GrayTextBrush;
                tsmiOpenTimeOutBackgroundImageFile.IsEnabled = false;

                tsmiRemoveTimeOutBackgroundImageFile.IsEnabled = false;
            }
        }

        private void SetTimeOutBackGroundImageRelativePath(string s)
        {
            foreach (ClockVM vm in MyClocks)
            {
                vm.TimeOutBackgroundImageRelativePath = s;
            }
        }

        private void TsmiChooseTimeOutBackgroundImageFile_Click(object sender, EventArgs e)
        {
            if (!ThereIsAtLeastAClock())
            {
                return;
            }

            System.Windows.Forms.OpenFileDialog d = Utils.GetImageChooser();
            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SetTimeOutBackGroundImageRelativePath(d.FileName);
            }
        }

        private void TsmiRemoveTimeOutBackgroundImageFile_Click(object sender, EventArgs e)
        {
            if (!ThereIsAtLeastAClock())
            {
                return;
            }

            SetTimeOutBackGroundImageRelativePath(null);
        }

        private void TsmiOpenTimeOutBackgroundImageFile_Click(object sender, EventArgs e)
        {
            if (!ThereIsAtLeastAClock())
            {
                return;
            }

            Utils.OpenUrlInDefaultApp(MyClock.TimeOutBackgroundImageRelativePath);
        }

        private void TsmiSelectSuggestedColor_Click(object sender, EventArgs e)
        {
            foreach (ClockVM cm in MyClocks)
            {
                if (cm.Icon != null)
                {
                    PictureAnalysis.GetMostUsedColor(cm.Icon);
                    cm.UserBackColor = PictureAnalysis.MostUsedColor;
                }
            }
        }

        private void TsmiConvertToOtherType_Click(object sender, EventArgs e)
        {
            foreach (ClockVM cm in MyClocks)
            {
                if (cm.ClockType == ClockVM.ClockTypes.Stopwatch)
                {
                    throw new NotImplementedException();
                }

                cm.ClockType =
                    cm.ClockType == ClockVM.ClockTypes.Timer ?
                        ClockVM.ClockTypes.Alarm : ClockVM.ClockTypes.Timer;
            }
        }

        internal Bitmap GetFirstClockIcon()
        {
            Bitmap ico = null;
            foreach (ClockVM cd in MyClocks)
            {
                if (cd.Icon != null)
                {
                    ico = cd.Icon;
                    break;
                }
            }
            return ico;
        }

        private void TsmiSaveIconAs_Click(object sender, EventArgs e)
        {
            Bitmap ico = GetFirstClockIcon();

            if (ico != null)
            {
                System.Windows.Forms.SaveFileDialog fd = Utils.GetImageSaveChooser();

                // TODO: pass MainWindow somehow to ShowDialog:
                if (fd.ShowDialog(/*MainWindow*/) == System.Windows.Forms.DialogResult.OK &&
                    fd.FileName != "")
                {
                    var fs = (FileStream)fd.OpenFile();

                    // TODO: use a method for this switch-case, method inside a new subclass of SaveFileDialog
                    // corresponding to Utils.GetImageSaveChooser.
                    switch (fd.FilterIndex)
                    {
                        case 1:
                            ico.Save(fs, ImageFormat.Jpeg);
                            break;

                        case 2:
                            ico.Save(fs, ImageFormat.Bmp);
                            break;

                        case 3:
                            ico.Save(fs, ImageFormat.Gif);
                            break;

                        case 4:
                            ico.Save(fs, ImageFormat.Emf);
                            break;

                        case 5:
                            // Made transparent on load of DataFile:
                            //ico.MakeTransparent(Color.Empty);
                            using (Icon ico2 = Utils.IconFromImage(ico))
                            {
                                ico2.Save(fs);
                            }
                            break;

                        case 6:
                            ico.Save(fs, ImageFormat.Png);
                            break;

                        case 7:
                            ico.Save(fs, ImageFormat.Tiff);
                            break;
                    }

                    fs.Close();
                }
            }
        }

        private void TsmiSetRandomColors_Click(object sender, EventArgs e)
        {
            if (!ThereIsAtLeastAClock())
            {
                return;
            }

            var randomGen = new Random();
            var names = (KnownColor[])Enum.GetValues(typeof(KnownColor));

            foreach (ClockVM c in MyClocks)
            {
                KnownColor randomColorName = names[randomGen.Next(names.Length)];
                Color randomColor = Color.FromKnownColor(randomColorName);
                c.UserBackColor = randomColor;
            }
        }

        internal static Image iconStop = CreateIcon("Oxygen-Icons.org-Oxygen-Actions-media-playback-pause.ico");
        internal static Image iconStart = CreateIcon("Oxygen-Icons.org-Oxygen-Actions-media-playback-start.ico");

        public void UpdateContents()
        {
            if (ThereIsAtLeastAClock())
            {
                MainWindow = MyClock.Model.MyDataFile.MainWindow;
            }

            if (MyClocks.Count >= 1)
            {
                tss4.Visibility = System.Windows.Visibility.Visible;
                tsmiDelete.Visibility = System.Windows.Visibility.Visible;
                tsmiRename.Visibility = System.Windows.Visibility.Visible;
                tsmiActivateDezactivate.Visibility = System.Windows.Visibility.Visible;
                tsmiSetCurrentValue.Visibility = System.Windows.Visibility.Visible;

                if (MyClock.Model is TimerData td)
                {
                    tsmiActivateDezactivate.Header = td.Running ? "_Stop" : "_Start";
                    tsmiActivateDezactivate.Icon = td.Running ? iconStop : iconStart;
                }
                else if (MyClock.Model is AlarmData ad)
                {
                    tsmiActivateDezactivate.Header = ad.Enabled ? "_Deactivate" : "_Activate";
                    tsmiActivateDezactivate.Icon = ad.Enabled ? iconStop : iconStart;
                }
                else if (MyClock.Model is StopwatchData sd)
                {
                    tsmiActivateDezactivate.Header = sd.Running ? "_Stop" : "_Start";
                    tsmiActivateDezactivate.Icon = sd.Running ? iconStop : iconStart;
                }
                else
                {
                    throw new NotImplementedException();
                }

                Bitmap ico = GetFirstClockIcon();

                tsmiSaveIconAs.IsEnabled = ico != null;
            }
            else
            {
                tss4.Visibility = System.Windows.Visibility.Collapsed;
                tsmiDelete.Visibility = System.Windows.Visibility.Collapsed;
                tsmiRename.Visibility = System.Windows.Visibility.Collapsed;
                tsmiActivateDezactivate.Visibility = System.Windows.Visibility.Collapsed;
                tsmiSetCurrentValue.Visibility = System.Windows.Visibility.Collapsed;
            }

            //bool AlwaysShowID = true;
            //foreach (ClockM c in MyClocks)
            //{
            //    if (c.Style == ClockM.ClockStyles.ShowIcon)
            //    {
            //        AlwaysShowID = false;
            //        break;
            //    }
            //}
            //tsmiAlwaysShowID.Checked = AlwaysShowID;

            if (MyClocks.Count == 0)
            {
                tsmiResetTo.Header = $"Reset to...";
            }
            else
            {
                // TODO: handle below stopwatches
                object rtv = MyClock.ResetToValue;
                foreach (ClockVM c in MyClocks)
                {
                    if (c.ResetToValue != rtv)
                    {
                        rtv = 0;
                    }
                }

                tsmiResetTo.Header = $"Reset to {rtv}";


                tsmiConvertToOtherType.Icon =
                    MyClocks.First().ClockType == ClockVM.ClockTypes.Alarm ?
                    CreateIcon("timers filter (clepsidra 4).ico") :
                    CreateIcon("alarms filter (alarm clock 5).ico");

                // TODO: a new submenu: Convert to... (with 2 options, 3 excluding the current type)
                tsmiConvertToOtherType.Header =
                    MyClock.ClockType == ClockVM.ClockTypes.Alarm ?
                    "C_onvert to Timer" : "C_onvert to Alarm";
            }

            UpdateTimeOutBackgroundItems();

            UpdateGroupsSubmenuContents();
            UpdateResetSubmenuContents();
        }

        internal static ContextMenu MyEmptyContextMenu = CreateEmptyContextMenu();

        private static ContextMenu CreateEmptyContextMenu()
        {
            var cms = new ContextMenu();
            MenuItem item = CreateMenuItem("No selected clocks.");
            cms.Items.Add(item);

            item.FontStyle = System.Windows.FontStyles.Italic;
            item.Foreground = System.Windows.SystemColors.GrayTextBrush;

            return cms;
        }

        private void ClockTypeScan(out bool allTimers, out bool allAlarms, out bool allStopwatches)
        {
            allTimers = true;
            allAlarms = true;
            allStopwatches = true;

            foreach (ClockVM c in MyClocks)
            {
                if (c.Model is TimerData)
                {
                    allAlarms = false;
                }
                else if (c.Model is AlarmData)
                {
                    allTimers = false;
                }
                else if (c.Model is StopwatchData)
                {
                    allStopwatches = false;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        private void TsmiSetCurrentValue_Click(object sender, EventArgs e)
        {
            ClockTypeScan(out bool allTimers, out bool allAlarms, out bool allStopwatches);

            if (allTimers)
            {
                var d = new TimeSpanPromptWindow();
                d.Owner = MainWindow;
                d.UserTimeSpan = (TimeSpan)MyClock.CurrentValue;

                if (d.ShowDialog() == true)
                {
                    foreach (ClockVM c in MyClocks)
                    {
                        c.CurrentValue = d.UserTimeSpan;
                    }
                }
            }
            else if (allAlarms)
            {
                var d = new DateTimePromptWindow();
                d.Owner = MainWindow;
                d.UserDateTime = (DateTime)MyClock.CurrentValue;

                if (d.ShowDialog() == true)
                {
                    foreach (ClockVM c in MyClocks)
                    {
                        c.CurrentValue = d.UserDateTime;
                    }
                }
            }
            else
            {
                MessageBox.Show(MainWindow, "Cannot set current value because there are clocks of different types selected.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void TsmiActivateDezactivate_Click(object sender, EventArgs e)
        {
            // See the comment below.
            var h = new HashSet<ClockVM>(MyClocks);

            // The following foreach changes the collection (h).
            foreach (ClockVM c in h)
            {
                c.Model.ActivateOrDeactivate();
            }
        }

        private void tsmiRename_Click(object sender, EventArgs e)
        {
            if (!ThereIsAtLeastAClock())
            {
                return;
            }

            var p = new RichTextPromptWindow();

            // TODO: get the correct MainWindow, not the first one (I will implement multiple Document windows):
            p.Owner = System.Windows.Application.Current.MainWindow;
            p.UserString = XamlWriter.Save(MyClock.Tag);

            if (p.ShowDialog() == true)
            {
                var hs = new HashSet<ClockVM>(MyClocks);
                foreach (ClockVM c in hs)
                {
                    c.Tag = (FlowDocument)XamlReader.Parse(p.UserString);
                }
            }
        }

        private void DeleteClocks()
        {
            var hs = new HashSet<ClockVM>(MyClocks);
            foreach (ClockVM c in hs)
            {
                c.Model.Delete();
            }
        }

        private void tsmiDelete_Click(object sender, EventArgs e)
        {
            if (!ThereIsAtLeastAClock())
            {
                return;
            }

            if (MessageBox.Show(MainWindow, $"Are you sure you want to delete the {MyClocks.Count} selected clocks?", "Confirmation request", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                DeleteClocks();
            }
        }

        //private void tsmiGroupName_DropDownOpening(object sender, EventArgs e)
        //{
        //    UpdateGroupsSubmenuContents();
        //}

        public void UpdateGroupsSubmenuContents()
        {
            if (!ThereIsAtLeastAClock())
            {
                return;
            }

            tsmiGroupName.Items.Clear();

            if (MyClock.Model.MyDataFile.ClockVMCollection.Model.Groups.Ms.Count == 0)
            {
                MenuItem tsmiNoGroups = CreateMenuItem("No groups available");
                tsmiNoGroups.FontStyle = System.Windows.FontStyles.Italic;
                tsmiNoGroups.IsEnabled = false;
                tsmiGroupName.Items.Add(tsmiNoGroups);
            }

            foreach (ClockGroupM g in MainWindow.VM.ClockVMCollection.Model.Groups.Ms)
            {
                MenuItem tsmi = CreateMenuItem(g.Name);

                bool chk = true;
                foreach (ClockVM c in MyClocks)
                {
                    if (c.GroupName != g.Name)
                    {
                        chk = false;
                        break;
                    }
                }
                tsmi.IsChecked = chk;

                if (g.Icon != null)
                {
                    tsmi.Icon = new Image()
                    {
                        Source = g.Icon
                    };
                }

                tsmiGroupName.Items.Add(tsmi);
                tsmi.Click += Tsmi_Click;
            }

            MenuItem tsmiNew = CreateMenuItem("Add to new group...",
                CreateIcon("Oxygen-Icons.org-Oxygen-Actions-list-add.ico"));
            tsmiNew.FontWeight = FontWeights.Bold;
            tsmiNew.Click += TsmiNew_Click;

            tsmiGroupName.Items.Add(tsmiNew);
        }

        private void TsmiNew_Click(object sender, EventArgs e)
        {
            if (!ThereIsAtLeastAClock())
            {
                return;
            }

            string g = ClockGroupListView.ShowAddNewGroupPrompt(MainWindow.VM);
            if (!string.IsNullOrEmpty(g))
            {
                SetGroupName("");
            }
        }

        private void StopAndResetAllTimeSpanClocks()
        {
            foreach (ClockVM c in MyClocks)
            {
                if (c.Model is TimerData td)
                {
                    td.Stop();
                }
                else if (c.Model is StopwatchData sd)
                {
                    sd.Stop();
                }
                c.CurrentValue = c.ResetToValue;
            }
        }

        private void tsmiResetTo_Click(object sender, EventArgs e)
        {
            if (!ThereIsAtLeastAClock())
            {
                return;
            }

            StopAndResetAllTimeSpanClocks();
        }

        //private void tsmiResetTo_DropDownOpening(object sender, EventArgs e)
        //{
        //    UpdateGroupsSubmenuContents();
        //}

        private bool AllClocksAreTimers()
        {
            // TODO: optimize:
            return ((from c in MyClocks
                     where c.Model is TimerData
                     select c).Count() == MyClocks.Count());
        }

        private bool AllClocksAreAlarms()
        {
            // TODO: optimize:
            return ((from c in MyClocks
                     where c.Model is AlarmData
                     select c).Count() == MyClocks.Count());
        }

        private static MenuItem CreateMenuItem(string str, Image icon = null)
        {
            var mi = new MenuItem()
            {
                Header = str
            };
            if (icon != null)
            {
                mi.Icon = icon;
            }
            return mi;
        }

        private static Image CreateIcon(string name)
        {
            return new Image()
            {
                Source = new BitmapImage(new Uri($"pack://application:,,,/Resources/{name}", UriKind.RelativeOrAbsolute)),
                Width = 16
            };
        }

        private bool ThereIsAtLeastAClock()
        {
            return MyClock != null;
        }

        public void UpdateResetSubmenuContents()
        {
            if (!ThereIsAtLeastAClock())
            {
                return;
            }

            tsmiResetTo.Items.Clear();

            var tsmiUpdateToCurrent = CreateMenuItem("Update to current");
            tsmiUpdateToCurrent.Click += TsmiUpdateToCurrent_Click;

            MenuItem tsmiUpdateToCustom = null;
            if (AllClocksAreTimers() || AllClocksAreAlarms())
            {
                tsmiUpdateToCustom = CreateMenuItem("Update to custom...");
                tsmiUpdateToCustom.Click += TsmiUpdateToCustom_Click;
            }
            else
            {
                // TODO: [VISUAL] maybe just italic grayed out instead of removed from the menu?
            }

            var tsmiLockResetToValue = CreateMenuItem("Lock reset-to value",
                CreateIcon("Oxygen-Icons.org-Oxygen-Actions-system-lock-screen.ico"));
            tsmiLockResetToValue.IsCheckable = true;
            tsmiLockResetToValue.IsChecked = MyClock.ResetToValueLocked;
            tsmiLockResetToValue.Click += TsmiLockResetToValue_Click;

            tsmiResetTo.Items.Add(tsmiUpdateToCurrent);
            if (tsmiUpdateToCustom != null)
            {
                tsmiResetTo.Items.Add(tsmiUpdateToCustom);
            }
            tsmiResetTo.Items.Add(tsmiLockResetToValue);
        }

        private void SetResetToValueLocked(bool locked)
        {
            foreach (ClockVM c in MyClocks)
            {
                c.ResetToValueLocked = locked;
            }
        }

        private void TsmiLockResetToValue_Click(object sender, EventArgs e)
        {
            var tsmi = (MenuItem)sender;
            SetResetToValueLocked(tsmi.IsChecked);
        }

        private void SetResetToValue(TimeSpan v)
        {
            foreach (ClockVM c in MyClocks)
            {
                c.ResetToValue = v;
            }
        }

        private void SetResetToValue(DateTime v)
        {
            foreach (ClockVM c in MyClocks)
            {
                c.ResetToValue = v;
            }
        }

        private void TsmiUpdateToCustom_Click(object sender, EventArgs e)
        {
            if (!ThereIsAtLeastAClock())
            {
                return;
            }

            ClockTypeScan(out bool allTimers, out bool allAlarms, out bool allStopwatches);

            if (allTimers || allStopwatches)
            {
                var tspd = new TimeSpanPromptWindow();
                tspd.Owner = MainWindow;
                if ((bool)tspd.ShowDialog())
                {
                    SetResetToValue(tspd.UserTimeSpan);
                }
            }
            else if (allAlarms)
            {
                var dtp = new DateTimePromptWindow();
                dtp.Owner = MainWindow;
                if ((bool)dtp.ShowDialog())
                {
                    SetResetToValue(dtp.UserDateTime);
                }
            }
        }

        private void TsmiUpdateToCurrent_Click(object sender, EventArgs e)
        {
            SetResetToValueToCurrentValue();
        }

        private void SetResetToValueToCurrentValue()
        {
            foreach (ClockVM c in MyClocks)
            {
                c.ResetToValue = c.CurrentValue;
            }
        }

        private void tsmiUndock_Click(object sender, EventArgs e)
        {
            foreach (ClockVM c in MyClocks)
            {
                var cw = new ClockWindow()
                {
                    DarkTheme = (bool)((System.Windows.Application.Current.MainWindow as MainWindow).VM.Settings.GetValue("DarkMode"))
                };
                cw.MyClockData = c;
                cw.Show();
            }
        }

        //private void tsmiAlwaysShowID_Click(object sender, EventArgs e)
        //{
        //    foreach (ClockData c in MyClocks)
        //    {
        //        foreach (IClockView v in c.MyTimerViews)
        //        {
        //            if (v is ClockControl cc)
        //            {
        //                cc.AlwaysShowID = !tsmiAlwaysShowID.Checked;
        //                cc.UpdateTimerStyle();
        //            }
        //        }
        //    }
        //}

        private void SetIcon(Bitmap bmp)
        {
            foreach (ClockVM c in MyClocks)
            {
                // TODO: is this sentinel variable needed & correctly used?
                c.Model.ChangeFromClockControlView = true;
                c.Icon = bmp;
                c.Model.ChangeFromClockControlView = false;
            }
        }

        private void tsmiSelectIcon_Click(object sender, EventArgs e)
        {
            if (!ThereIsAtLeastAClock())
            {
                return;
            }

            System.Windows.Forms.OpenFileDialog fd = Utils.GetImageChooser();
            // TODO: pass MainWindow to ShowDialog somehow:
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var bmp = new Bitmap(fd.FileName);

                SetIcon(bmp);
            }
        }

        private void tsmiRemoveIcon_Click(object sender, EventArgs e)
        {
            SetIcon(null);
        }

        private void TsmiRemoveCustomColor_Click(object sender, EventArgs e)
        {
            foreach (ClockVM c in MyClocks)
            {
                c.Model.ChangeFromClockControlView = true;
                c.UserBackColor = Color.Empty;
                c.Model.ChangeFromClockControlView = false;
            }
        }

        private void SetUserBackColor(Color c)
        {
            foreach (ClockVM cvm in MyClocks)
            {
                cvm.UserBackColor = c;
            }
        }

        private void TsmiChangeColor_Click(object sender, EventArgs e)
        {
            if (!ThereIsAtLeastAClock())
            {
                return;
            }

            var cd = Utils.GetColorChooser(MyClock.UserBackColor);

            // TODO: pass MainWindow to ShowDialog somehow:
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SetUserBackColor(cd.Color);
            }
        }

        private void SetGroupName(string gn)
        {
            var hs = new HashSet<ClockVM>(MyClocks);
            foreach (ClockVM c in hs)
            {
                c.GroupName = gn;
            }
        }

        internal bool DontFireTsmiClick = false;

        private void Tsmi_Click(object sender, EventArgs e)
        {
            if (DontFireTsmiClick)
            {
                return;
            }

            var tsmi = (MenuItem)sender;
            if (tsmi.IsChecked)
            {
                SetGroupName("");

                DontFireTsmiClick = true;
                tsmi.IsChecked = false;
                DontFireTsmiClick = false;
            }
            else
            {
                SetGroupName(tsmi.Header.ToString()); // TODO: what about the '_' char?

                DontFireTsmiClick = true;
                tsmi.IsChecked = true;
                DontFireTsmiClick = false;
            }
        }
    }
}
