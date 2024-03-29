﻿using System;
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
            tsmiUndock.Icon = new Image()
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Oxygen-Icons.org-Oxygen-Actions-window-new.ico", UriKind.RelativeOrAbsolute)),
                Width = 16
            };

            // 
            // tsmiResetTo
            // 
            tsmiResetTo.Header = "Reset t_o";
            //tsmiResetTo.ContextMenuOpening += tsmiResetTo_DropDownOpening;
            tsmiResetTo.PreviewMouseDown += tsmiResetTo_Click;
            tsmiResetTo.Icon = new Image()
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Resources/path1092.png", UriKind.RelativeOrAbsolute)),
                Width = 16
            };

            // 
            // tsmiGroupName
            // 
            tsmiGroupName.Header = "_Group...";
            //tsmiGroupName.ContextMenuOpening += tsmiGroupName_DropDownOpening;
            tsmiGroupName.Icon = new Image()
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Resources/folder.ico", UriKind.RelativeOrAbsolute)),
                Width = 16
            };

            // 
            // tsmiSelectColor
            // 
            tsmiSelectColor.Header = "Sele_ct color...";
            tsmiSelectColor.Icon = new Image()
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Oxygen-Icons.org-Oxygen-Actions-format-stroke-color.ico", UriKind.RelativeOrAbsolute)),
                Width = 16
            };

            // 
            // tsmiRemoveCustomColor
            // 
            tsmiRemoveCustomColor.Header = "Dele_te user color";
            tsmiRemoveCustomColor.Icon = new Image()
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Resources/remove_custom_color.ico", UriKind.RelativeOrAbsolute)),
                Width = 16
            };

            // 
            // tsmiSelectIcon
            // 
            tsmiSelectIcon.Header = "Se_lect icon...";
            tsmiSelectIcon.Click += tsmiSelectIcon_Click;
            tsmiSelectIcon.Icon = new Image()
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Oxygen-Icons.org-Oxygen-Actions-document-edit.ico", UriKind.RelativeOrAbsolute)),
                Width = 16
            };

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
            tsmiDelete.Icon = new Image()
            {
                Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/Oxygen-Icons.org-Oxygen-Actions-edit-delete.ico", UriKind.RelativeOrAbsolute)),
                Width = 16
            };

            // 
            // tsmiRename
            // 
            tsmiRename.Header = "_Rename...";
            tsmiRename.Click += tsmiRename_Click;
            tsmiRename.Icon = new Image()
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Resources/set_current_value.ico", UriKind.RelativeOrAbsolute)),
                Width = 16
            };

            tsmiSetCurrentValue.Header = "Set curre_nt value...";
            tsmiSetCurrentValue.Click += TsmiSetCurrentValue_Click;
            tsmiSetCurrentValue.Icon = new Image()
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Oxygen-Icons.org-Oxygen-Actions-edit-rename.ico", UriKind.RelativeOrAbsolute)),
                Width = 16
            };

            tsmiSetRandomColors.Header = "Set random _colors";
            tsmiSetRandomColors.Click += TsmiSetRandomColors_Click;
            tsmiSetRandomColors.Icon = new Image()
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Resources/path1129.png", UriKind.RelativeOrAbsolute)),
                Width = 16
            };

            tsmiConvertToOtherType.Header = "T_oggle Types";
            tsmiConvertToOtherType.ToolTip =
                "From Alarm to Timer, and from Timer to Alarm";
            tsmiConvertToOtherType.Click += TsmiConvertToOtherType_Click;

            tsmiSaveIconAs.Header = "Save _icon as...";
            tsmiSaveIconAs.Icon = new Image()
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Oxygen-Icons.org-Oxygen-Actions-document-save-as.ico", UriKind.RelativeOrAbsolute)),
                Width = 16
            };
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
            tsmiSetToSunrise.Icon = new Image()
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Resources/sunrise.png", UriKind.RelativeOrAbsolute)),
                Width = 16
            };
            tsmiSetToSunrise.Click += TsmiSetToSunrise_Click;

            tsmiSetToSunset = new MenuItem();
            tsmiSetToSunset.Header = "Set to Sunset";
            tsmiSetToSunset.Icon = new Image()
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Resources/sunset.png", UriKind.RelativeOrAbsolute)),
                Width = 16
            };
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
                            }), 91.0);

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

            if (MyClocks.Count > 0)
            {
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

                w.TryStart(false, TimeSpan.FromMilliseconds(10000));
            }
        }

        private void UpdateTimeOutBackgroundItems()
        {
            if (MyClock == null)
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

        private void TsmiChooseTimeOutBackgroundImageFile_Click(object sender, EventArgs e)
        {
            if (MyClock == null)
            {
                return;
            }

            System.Windows.Forms.OpenFileDialog d = Utils.GetImageChooser();
            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (ClockVM vm in MyClocks)
                {
                    vm.TimeOutBackgroundImageRelativePath = d.FileName;
                }
            }
        }

        private void TsmiRemoveTimeOutBackgroundImageFile_Click(object sender, EventArgs e)
        {
            if (MyClock == null)
            {
                return;
            }

            foreach (ClockVM vm in MyClocks)
            {
                vm.TimeOutBackgroundImageRelativePath = null;
            }
        }

        private void TsmiOpenTimeOutBackgroundImageFile_Click(object sender, EventArgs e)
        {
            if (MyClock == null)
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
            if (MyClocks.Count == 0)
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

        internal static Image iconStop = new Image()
        {
            Source = new BitmapImage(
                new Uri("pack://application:,,,/Resources/Oxygen-Icons.org-Oxygen-Actions-media-playback-pause.ico", UriKind.RelativeOrAbsolute)),
            Width = 16
        };
        internal static Image iconStart = new Image()
        {
            Source = new BitmapImage(
                new Uri("pack://application:,,,/Resources/Oxygen-Icons.org-Oxygen-Actions-media-playback-start.ico", UriKind.RelativeOrAbsolute)),
            Width = 16
        };

        public void UpdateContents()
        {
            if (MyClocks.Count != 0)
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
                else
                {
                    var ad = MyClock.Model as AlarmData;
                    tsmiActivateDezactivate.Header = ad.Enabled ? "_Deactivate" : "_Activate";
                    tsmiActivateDezactivate.Icon = ad.Enabled ? iconStop : iconStart;
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
                    new Image()
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/Resources/timers filter (clepsidra 4).ico", UriKind.RelativeOrAbsolute)),
                        Width = 16
                    } :
                    new Image()
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/Resources/alarms filter (alarm clock 5).ico", UriKind.RelativeOrAbsolute)),
                        Width = 16
                    };

                tsmiConvertToOtherType.Header =
                    MyClocks.First().ClockType == ClockVM.ClockTypes.Alarm ?
                    "C_onvert to Timer" : "C_onvert to Alarm";
            }

            UpdateTimeOutBackgroundItems();

            UpdateGroupsSubmenuContents();
            UpdateResetSubmenuContents();
        }

        internal static ContextMenu MyEmptyContextMenu;

        // TODO: make a special method to create the empty context menu and remove the static c-tor
        static ClockContextMenuStrip()
        {
            var cms = new ContextMenu();
            MenuItem item = new MenuItem()
            {
                Header = "No selected clocks."
            };
            cms.Items.Add(item);

            item.FontStyle = System.Windows.FontStyles.Italic;
            item.Foreground = System.Windows.SystemColors.GrayTextBrush;

            MyEmptyContextMenu = cms;
        }

        private void TsmiSetCurrentValue_Click(object sender, EventArgs e)
        {
            bool allTimers = true, allAlarms = true;
            foreach (ClockVM c in MyClocks)
            {
                if (c.Model is TimerData)
                {
                    allAlarms = false;
                }
                else
                {
                    allTimers = false;
                }
            }

            if (allTimers)
            {
                var d = new TimeSpanPromptWindow();
                d.Owner = MainWindow;
                d.UserTimeSpan = (TimeSpan)MyClock.CurrentValue;

                if ((bool)d.ShowDialog())
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

                if ((bool)d.ShowDialog())
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
            if (MyClocks.Count == 0)
            {
                return;
            }

            var p = new RichTextPromptWindow();

            // TODO: get the correct MainWindow, not the first one (I will implement multiple Document windows):
            p.Owner = System.Windows.Application.Current.MainWindow;
            p.UserString = XamlWriter.Save(MyClock.Tag);

            if ((bool)p.ShowDialog())
            {
                var hs = new HashSet<ClockVM>(MyClocks);
                foreach (ClockVM c in hs)
                {
                    c.Tag = XamlReader.Parse(p.UserString) as FlowDocument;
                }
            }
        }

        private void tsmiDelete_Click(object sender, EventArgs e)
        {
            if (MyClocks.Count == 0)
            {
                return;
            }

            if (MessageBox.Show(MainWindow, $"Are you sure you want to delete the {MyClocks.Count} selected clocks?", "Confirmation request", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                var hs = new HashSet<ClockVM>(MyClocks);
                foreach (ClockVM c in hs)
                {
                    c.Model.Delete();
                }
            }
        }

        //private void tsmiGroupName_DropDownOpening(object sender, EventArgs e)
        //{
        //    UpdateGroupsSubmenuContents();
        //}

        public void UpdateGroupsSubmenuContents()
        {
            if (MyClock == null)
            {
                return;
            }

            tsmiGroupName.Items.Clear();

            if (MyClock.Model.MyDataFile.ClockVMCollection.Model.Groups.Ms.Count == 0)
            {
                var tsmiNoGroups = new MenuItem()
                {
                    Header = "No groups available"
                };
                tsmiNoGroups.FontStyle = System.Windows.FontStyles.Italic;
                tsmiNoGroups.IsEnabled = false;
                tsmiGroupName.Items.Add(tsmiNoGroups);
            }

            foreach (ClockGroupM g in MainWindow.VM.ClockVMCollection.Model.Groups.Ms)
            {
                var tsmi = new MenuItem()
                {
                    Header = g.Name
                };

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

            var tsmiNew = new MenuItem()
            {
                Header = "Add to new group..."
            };
            tsmiNew.FontWeight = System.Windows.FontWeights.Bold;
            tsmiNew.Icon = new Image()
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Oxygen-Icons.org-Oxygen-Actions-list-add.ico", UriKind.RelativeOrAbsolute)),
                Width = 16
            };
            tsmiNew.Click += TsmiNew_Click;
            tsmiGroupName.Items.Add(tsmiNew);
        }

        private void TsmiNew_Click(object sender, EventArgs e)
        {
            if (MyClocks.Count == 0)
            {
                return;
            }

            string g;
            g = ClockGroupListView.ShowAddNewGroupPrompt(MainWindow.VM);
            if (!string.IsNullOrEmpty(g))
            {
                foreach (ClockVM c in MyClocks)
                {
                    c.GroupName = g;
                }
            }
        }

        private void tsmiResetTo_Click(object sender, EventArgs e)
        {
            if (MyClocks.Count == 0)
            {
                return;
            }

            foreach (ClockVM c in MyClocks)
            {
                if (c.Model is TimerData td)
                {
                    td.Stop();
                }
                c.CurrentValue = c.ResetToValue;
            }

            //Hide();
        }

        //private void tsmiResetTo_DropDownOpening(object sender, EventArgs e)
        //{
        //    UpdateGroupsSubmenuContents();
        //}

        public void UpdateResetSubmenuContents()
        {
            if (MyClock == null)
            {
                return;
            }

            tsmiResetTo.Items.Clear();

            var tsmiUpdateToCurrent = new MenuItem()
            {
                Header = "Update to current"
            };
            tsmiUpdateToCurrent.Click += TsmiUpdateToCurrent_Click;

            MenuItem tsmiUpdateToCustom = null;
            // TODO: optimize:
            if (
                ((from c in MyClocks
                  where c.Model is TimerData
                  select c).Count() == MyClocks.Count()) ||
                ((from c in MyClocks
                  where c.Model is AlarmData
                  select c).Count() == MyClocks.Count())
            )
            {
                tsmiUpdateToCustom = new MenuItem()
                {
                    Header = "Update to custom..."
                };
                tsmiUpdateToCustom.Click += TsmiUpdateToCustom_Click;
            }
            else
            {
                // TODO: [VISUAL] maybe just italic grayed out instead of removed from the menu?
            }

            var tsmiLockResetToValue = new MenuItem()
            {
                Header = "Lock reset-to value"
            };
            tsmiLockResetToValue.IsCheckable = true;
            tsmiLockResetToValue.Icon = new Image()
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Oxygen-Icons.org-Oxygen-Actions-system-lock-screen.ico", UriKind.RelativeOrAbsolute)),
                Width = 16
            };
            tsmiLockResetToValue.IsChecked = MyClock.ResetToValueLocked;
            tsmiLockResetToValue.Click += TsmiLockResetToValue_Click;

            tsmiResetTo.Items.Add(tsmiUpdateToCurrent);
            if (tsmiUpdateToCustom != null)
            {
                tsmiResetTo.Items.Add(tsmiUpdateToCustom);
            }
            tsmiResetTo.Items.Add(tsmiLockResetToValue);
        }

        private void TsmiLockResetToValue_Click(object sender, EventArgs e)
        {
            var tsmi = (MenuItem)sender;
            foreach (ClockVM c in MyClocks)
            {
                c.ResetToValueLocked = tsmi.IsChecked;
            }
        }

        private void TsmiUpdateToCustom_Click(object sender, EventArgs e)
        {
            if (MyClocks.Count == 0)
            {
                return;
            }

            bool allTimers = true, allAlarms = true;
            foreach (ClockVM c in MyClocks)
            {
                if (c.Model is TimerData)
                {
                    allAlarms = false;
                }
                else
                {
                    allTimers = false;
                }
            }

            if (allTimers)
            {
                var tspd = new TimeSpanPromptWindow();
                tspd.Owner = MainWindow;
                if ((bool)tspd.ShowDialog())
                {
                    foreach (ClockVM c in MyClocks)
                    {
                        c.ResetToValue = tspd.UserTimeSpan;
                    }
                }
            }
            else if (allAlarms)
            {
                var dtp = new DateTimePromptWindow();
                dtp.Owner = MainWindow;
                if ((bool)dtp.ShowDialog())
                {
                    foreach (ClockVM c in MyClocks)
                    {
                        c.ResetToValue = dtp.UserDateTime;
                    }
                }
            }
        }

        private void TsmiUpdateToCurrent_Click(object sender, EventArgs e)
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
                System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(cw);
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

        private void tsmiSelectIcon_Click(object sender, EventArgs e)
        {
            if (MyClocks.Count == 0)
            {
                return;
            }

            System.Windows.Forms.OpenFileDialog fd = Utils.GetImageChooser();
            // TODO: pass MainWindow to ShowDialog somehow:
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // 1. resize image in memory
                float maxHeight = 200;
                float maxWidth = 200;

                var r = new Rectangle(0,
                    0,
                    (int)Math.Round(maxWidth),
                    (int)Math.Round(maxHeight)
                );
                Bitmap bmp = Utils.ResizeToFitBoundingBox(
                    new Bitmap(fd.FileName),
                    r);

                foreach (ClockVM c in MyClocks)
                {
                    // TODO: is this sentinel variable needed & correctly used?
                    c.Model.ChangeFromClockControlView = true;
                    c.Icon = bmp;
                    c.Model.ChangeFromClockControlView = false;
                }
            }
        }

        private void tsmiRemoveIcon_Click(object sender, EventArgs e)
        {
            foreach (ClockVM c in MyClocks)
            {
                // TODO: is this sentinel variable needed & correctly used?
                c.Model.ChangeFromClockControlView = true;
                c.Icon = null;
                c.Model.ChangeFromClockControlView = false;
            }
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

        private void TsmiChangeColor_Click(object sender, EventArgs e)
        {
            if (MyClocks.Count > 0)
            {
                var cd = Utils.GetColorChooser(MyClock.UserBackColor);

                // TODO: pass MainWindow to ShowDialog somehow:
                if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    foreach (ClockVM c in MyClocks)
                    {
                        c.UserBackColor = cd.Color;
                    }
                }
            }
        }

        internal bool DontFireTsmiClick = false;

        private void Tsmi_Click(object sender, EventArgs e)
        {
            if (DontFireTsmiClick)
            {
                return;
            }

            var tsmi = sender as MenuItem;
            if (tsmi.IsChecked)
            {
                var hs = new HashSet<ClockVM>(MyClocks);
                foreach (ClockVM c in hs)
                {
                    c.GroupName = "";
                }

                DontFireTsmiClick = true;
                tsmi.IsChecked = false;
                DontFireTsmiClick = false;
            }
            else
            {
                var hs = new HashSet<ClockVM>(MyClocks);
                foreach (ClockVM c in hs)
                {
                    c.GroupName = tsmi.Header.ToString(); // TODO: what about the '_' char?
                }

                DontFireTsmiClick = true;
                tsmi.IsChecked = true;
                DontFireTsmiClick = false;
            }
        }
    }
}
