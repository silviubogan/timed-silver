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
using System.Windows.Forms;

namespace cs_timed_silver
{
    internal class ClockContextMenuStrip : ContextMenuStrip
    {
        private ToolStripMenuItem tsmiSelectColor;
        private ToolStripMenuItem tsmiRemoveCustomColor;
        private ToolStripMenuItem tsmiGroupName;
        private ToolStripMenuItem tsmiResetTo;
        private ToolStripMenuItem tsmiSelectIcon;
        private ToolStripMenuItem tsmiSelectSuggestedColor;
        private ToolStripSeparator tss1, tss2, tss3, tss4, tss5;
        private ToolStripMenuItem tsmiRemoveIcon;
        //private ToolStripMenuItem tsmiAlwaysShowID;
        private ToolStripMenuItem tsmiUndock;
        internal ToolStripMenuItem tsmiDelete, tsmiRename,
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

                return MyClocks.First();
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
            ShowCheckMargin = false;
            ShowImageMargin = true;


            tsmiUndock = new ToolStripMenuItem();

            tss1 = new ToolStripSeparator();

            tsmiResetTo = new ToolStripMenuItem();
            tsmiGroupName = new ToolStripMenuItem();

            tss2 = new ToolStripSeparator();

            tsmiSelectColor = new ToolStripMenuItem();
            tsmiRemoveCustomColor = new ToolStripMenuItem();

            tss3 = new ToolStripSeparator();

            tsmiSelectIcon = new ToolStripMenuItem();
            tsmiRemoveIcon = new ToolStripMenuItem();
            //tsmiAlwaysShowID = new ToolStripMenuItem();

            tss4 = new ToolStripSeparator();

            tsmiDelete = new ToolStripMenuItem();
            tsmiRename = new ToolStripMenuItem();
            tsmiActivateDezactivate = new ToolStripMenuItem();
            tsmiSetCurrentValue = new ToolStripMenuItem();
            tsmiSetRandomColors = new ToolStripMenuItem();
            tsmiConvertToOtherType = new ToolStripMenuItem();

            tss5 = new ToolStripSeparator();

            tsmiAdvanced = new ToolStripMenuItem();
            tsmiSaveIconAs = new ToolStripMenuItem();
            tsmiSelectSuggestedColor = new ToolStripMenuItem();

            Items.AddRange(new ToolStripItem[] {
                tsmiUndock,

                tss1,

                tsmiResetTo,
                tsmiGroupName,

                tss2,

                tsmiSelectColor,
                tsmiRemoveCustomColor,

                tss3,

                tsmiSelectIcon,
                tsmiRemoveIcon,
                //tsmiAlwaysShowID,

                tss4,

                tsmiActivateDezactivate,
                tsmiSetCurrentValue,
                tsmiDelete,
                tsmiRename,
                tsmiSetRandomColors,
                tsmiConvertToOtherType,

                tss5,

                tsmiAdvanced
            });

            ImageList = new ImageList();
            ImageList.ColorDepth = ColorDepth.Depth32Bit;
            ImageList.TransparentColor = Color.Empty;
            ImageList.ImageSize = new Size(128, 128);
            ImageList.Images.Add(Utils.IconResourceVersionBySize(Properties.Resources.Oxygen_Icons_org_Oxygen_Actions_window_new, 16));
            ImageList.Images.Add(Utils.ResizeToFitBoundingBox(Properties.Resources.reset_icon, new Rectangle(Point.Empty, new Size(32, 32))));

            // 
            // tsmiUndock
            // 
            tsmiUndock.Text = "&Undock...";
            tsmiUndock.Click += new EventHandler(tsmiUndock_Click);
            tsmiUndock.ImageTransparentColor = Color.Empty;
            tsmiUndock.ImageIndex = 0;

            // 
            // tsmiResetTo
            // 
            tsmiResetTo.Text = "Reset t&o";
            tsmiResetTo.DropDownOpening += new EventHandler(tsmiResetTo_DropDownOpening);
            tsmiResetTo.Click += new EventHandler(tsmiResetTo_Click);
            tsmiResetTo.ImageTransparentColor = Color.Empty;
            tsmiResetTo.ImageIndex = 1;

            // 
            // tsmiGroupName
            // 
            tsmiGroupName.Name = "tsmiGroupName";
            tsmiGroupName.Text = "&Group...";
            tsmiGroupName.DropDownOpening += new EventHandler(tsmiGroupName_DropDownOpening);
            tsmiGroupName.Image = Utils.IconResourceVersionBySize(Properties.Resources.folder_menuitem, 32);

            // 
            // tsmiSelectColor
            // 
            tsmiSelectColor.Name = "tsmiSelectColor";
            tsmiSelectColor.Text = "Sele&ct color...";
            tsmiSelectColor.Image = Utils.IconResourceVersionBySize(Properties.Resources.Oxygen_Icons_org_Oxygen_Actions_format_stroke_color, 16);

            // 
            // tsmiRemoveCustomColor
            // 
            tsmiRemoveCustomColor.Name = "tsmiRemoveCustomColor";
            tsmiRemoveCustomColor.Text = "Dele&te user color";
            tsmiRemoveCustomColor.Image = Utils.IconResourceVersionBySize(Properties.Resources.remove_custom_color, 16);

            // 
            // tsmiSelectIcon
            // 
            tsmiSelectIcon.Name = "tsmiSelectIcon";
            tsmiSelectIcon.Text = "Se&lect icon...";
            tsmiSelectIcon.Click += new EventHandler(tsmiSelectIcon_Click);
            tsmiSelectIcon.Image = Utils.IconResourceVersionBySize(Properties.Resources.Oxygen_Icons_org_Oxygen_Actions_document_edit, 16);

            // 
            // tsmiRemoveIcon
            // 
            tsmiRemoveIcon.Name = "tsmiRemoveIcon";
            tsmiRemoveIcon.Text = "R&emove icon";
            tsmiRemoveIcon.Click += new EventHandler(tsmiRemoveIcon_Click);

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
            tsmiDelete.Text = "&Delete...";
            tsmiDelete.Click += tsmiDelete_Click;
            tsmiDelete.Image = Utils.IconResourceVersionBySize(Properties.Resources.Oxygen_Icons_org_Oxygen_Actions_edit_delete, 16);

            // 
            // tsmiRename
            // 
            tsmiRename.Text = "&Rename...";
            tsmiRename.Click += tsmiRename_Click;
            tsmiRename.Image = Utils.IconResourceVersionBySize(Properties.Resources.Oxygen_Icons_org_Oxygen_Actions_edit_rename, 16);

            tsmiSetCurrentValue.Text = "Set curre&nt value...";
            tsmiSetCurrentValue.Click += TsmiSetCurrentValue_Click;
            tsmiSetCurrentValue.Image = Utils.IconResourceVersionBySize(Properties.Resources.set_current_value, 16);

            tsmiSetRandomColors.Text = "Set random &colors";
            tsmiSetRandomColors.Click += TsmiSetRandomColors_Click;
            tsmiSetRandomColors.Image = Utils.ResizeToFitBoundingBox(Properties.Resources.random, new Rectangle(Point.Empty, new Size(16, 16)));

            tsmiConvertToOtherType.Text = "T&oggle Types";
            tsmiConvertToOtherType.ToolTipText =
                "From Alarm to Timer, and from Timer to Alarm";
            tsmiConvertToOtherType.Click += TsmiConvertToOtherType_Click;

            tsmiSaveIconAs.Text = "Save &icon as...";
            tsmiSaveIconAs.Image = Utils.IconResourceVersionBySize(Properties.Resources.Oxygen_Icons_org_Oxygen_Actions_document_save_as, 16);
            tsmiSaveIconAs.Click += TsmiSaveIconAs_Click;

            tsmiSelectSuggestedColor.Text = "Select suggested color";
            tsmiSelectSuggestedColor.Click += TsmiSelectSuggestedColor_Click;

            tsmiSelectColor.Click += TsmiChangeColor_Click;
            tsmiRemoveCustomColor.Click += TsmiRemoveCustomColor_Click;
            tsmiActivateDezactivate.Click += TsmiActivateDezactivate_Click;

            tsmiOpenTimeOutBackgroundImageFile = new ToolStripMenuItem();
            tsmiOpenTimeOutBackgroundImageFile.Click += TsmiOpenTimeOutBackgroundImageFile_Click;

            tsmiRemoveTimeOutBackgroundImageFile = new ToolStripMenuItem();
            tsmiRemoveTimeOutBackgroundImageFile.Text = "Remove Time-out Background Image File";
            tsmiRemoveTimeOutBackgroundImageFile.Click += TsmiRemoveTimeOutBackgroundImageFile_Click;

            tsmiChooseTimeOutBackgroundImageFile = new ToolStripMenuItem();
            tsmiChooseTimeOutBackgroundImageFile.Text = "Choose a Time-out Background Image File...";
            tsmiChooseTimeOutBackgroundImageFile.Click += TsmiChooseTimeOutBackgroundImageFile_Click;

            tsmiSetToSunrise = new ToolStripMenuItem();
            tsmiSetToSunrise.Text = "Set to Sunrise";
            tsmiSetToSunrise.Click += TsmiSetToSunrise_Click;

            tsmiSetToSunset = new ToolStripMenuItem();
            tsmiSetToSunset.Text = "Set to Sunset";
            tsmiSetToSunset.Click += TsmiSetToSunset_Click;

            tsmiAdvanced.Text = "Advanced...";
            tsmiAdvanced.DropDownItems.Add(tsmiSaveIconAs);
            tsmiAdvanced.DropDownItems.Add(tsmiSelectSuggestedColor);
            tsmiAdvanced.DropDownItems.Add(new ToolStripSeparator());
            tsmiAdvanced.DropDownItems.Add(tsmiOpenTimeOutBackgroundImageFile);
            tsmiAdvanced.DropDownItems.Add(tsmiRemoveTimeOutBackgroundImageFile);
            tsmiAdvanced.DropDownItems.Add(tsmiChooseTimeOutBackgroundImageFile);
            tsmiAdvanced.DropDownItems.Add(new ToolStripSeparator());
            tsmiAdvanced.DropDownItems.Add(tsmiSetToSunrise);
            tsmiAdvanced.DropDownItems.Add(tsmiSetToSunset);
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
                tsmiOpenTimeOutBackgroundImageFile.Text = Path.GetFileName(MyClock.TimeOutBackgroundImageRelativePath);
                tsmiOpenTimeOutBackgroundImageFile.ToolTipText = MyClock.TimeOutBackgroundImageRelativePath;
                tsmiOpenTimeOutBackgroundImageFile.Font = new Font(tsmiOpenTimeOutBackgroundImageFile.Font, FontStyle.Regular);
                tsmiOpenTimeOutBackgroundImageFile.ForeColor = SystemColors.ControlText;
                tsmiOpenTimeOutBackgroundImageFile.Enabled = true;

                tsmiRemoveTimeOutBackgroundImageFile.Enabled = true;
            }
            else
            {
                tsmiOpenTimeOutBackgroundImageFile.Text = "No Specific Time-out Background";
                tsmiOpenTimeOutBackgroundImageFile.ToolTipText = "";
                tsmiOpenTimeOutBackgroundImageFile.Font = new Font(tsmiOpenTimeOutBackgroundImageFile.Font, FontStyle.Italic);
                tsmiOpenTimeOutBackgroundImageFile.ForeColor = SystemColors.GrayText;
                tsmiOpenTimeOutBackgroundImageFile.Enabled = false;

                tsmiRemoveTimeOutBackgroundImageFile.Enabled = false;
            }
        }

        private void TsmiChooseTimeOutBackgroundImageFile_Click(object sender, EventArgs e)
        {
            if (MyClock == null)
            {
                return;
            }

            OpenFileDialog d = Utils.GetImageChooser();
            if (d.ShowDialog() == DialogResult.OK)
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
                SaveFileDialog fd = Utils.GetImageSaveChooser();

                if (fd.ShowDialog(this) == DialogResult.OK &&
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

        internal static Image iconStop = Utils.IconResourceVersionBySize(Properties.Resources.Oxygen_Icons_org_Oxygen_Actions_media_playback_pause, 16);
        internal static Image iconStart = Utils.IconResourceVersionBySize(Properties.Resources.Oxygen_Icons_org_Oxygen_Actions_media_playback_start, 16);

        protected override void OnOpening(CancelEventArgs e)
        {
            base.OnOpening(e);

            if (MyClocks.Count != 0)
            {
                MainWindow = MyClock.Model.MyDataFile.MainWindow;
            }

            if (MyClocks.Count >= 1)
            {
                tss4.Visible = true;
                tsmiDelete.Visible = true;
                tsmiRename.Visible = true;
                tsmiActivateDezactivate.Visible = true;
                tsmiSetCurrentValue.Visible = true;

                if (MyClock.Model is TimerData td)
                {
                    tsmiActivateDezactivate.Text = td.Running ? "&Stop" : "&Start";
                    tsmiActivateDezactivate.Image = td.Running ? iconStop : iconStart;
                }
                else
                {
                    var ad = MyClock.Model as AlarmData;
                    tsmiActivateDezactivate.Text = ad.Enabled ? "&Deactivate" : "&Activate";
                    tsmiActivateDezactivate.Image = ad.Enabled ? iconStop : iconStart;
                }

                Bitmap ico = GetFirstClockIcon();

                tsmiSaveIconAs.Enabled = ico != null;
            }
            else
            {
                tss4.Visible = false;
                tsmiDelete.Visible = false;
                tsmiRename.Visible = false;
                tsmiActivateDezactivate.Visible = false;
                tsmiSetCurrentValue.Visible = false;
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
                tsmiResetTo.Text = $"Reset to...";
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

                tsmiResetTo.Text = $"Reset to {rtv}";


                tsmiConvertToOtherType.Image =
                    Utils.IconResourceVersionBySize(
                        (MyClocks.First().ClockType == ClockVM.ClockTypes.Alarm ?
                        Properties.Resources.timers_filter__clepsidra_4_ :
                        Properties.Resources.alarms_filter__alarm_clock_5_),
                    32);
                tsmiConvertToOtherType.Text =
                    MyClocks.First().ClockType == ClockVM.ClockTypes.Alarm ?
                    "C&onvert to Timer" : "C&onvert to Alarm";
            }

            UpdateTimeOutBackgroundItems();
        }

        internal static ContextMenuStrip MyEmptyContextMenu;

        static ClockContextMenuStrip()
        {
            var cms = new ContextMenuStrip();
            ToolStripItem item = cms.Items.Add("No selected clocks.");
            item.Font = new Font(item.Font, FontStyle.Italic);
            item.ForeColor = SystemColors.GrayText;

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
                MessageBox.Show(this, "Cannot set current value because there are clocks of different types selected.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            var p = new TextPromptWindow();

            // TODO: get the correct MainWindow, not the first one (I will implement multiple Document windows):
            p.Owner = System.Windows.Application.Current.MainWindow;
            p.UserString = MyClock.Tag;

            if ((bool)p.ShowDialog())
            {
                var hs = new HashSet<ClockVM>(MyClocks);
                foreach (ClockVM c in hs)
                {
                    c.Tag = p.UserString;
                }
            }
        }

        private void tsmiDelete_Click(object sender, EventArgs e)
        {
            if (MyClocks.Count == 0)
            {
                return;
            }

            if (MessageBox.Show(this, $"Are you sure you want to delete the {MyClocks.Count} selected clocks?", "Confirmation request", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                var hs = new HashSet<ClockVM>(MyClocks);
                foreach (ClockVM c in hs)
                {
                    c.Model.Delete();
                }
            }
        }

        private void tsmiGroupName_DropDownOpening(object sender, EventArgs e)
        {
            tsmiGroupName.DropDownItems.Clear();

            if (MyClock.Model.MyDataFile.ClockVMCollection.Model.Groups.Ms.Count == 0)
            {
                var tsmiNoGroups = new ToolStripMenuItem("No groups available");
                tsmiNoGroups.Font = new Font(Font, FontStyle.Italic);
                tsmiNoGroups.Enabled = false;
                tsmiGroupName.DropDownItems.Add(tsmiNoGroups);
            }

            foreach (ClockGroupM g in MainWindow.VM.ClockVMCollection.Model.Groups.Ms)
            {
                var tsmi = new ToolStripMenuItem(g.Name);

                bool chk = true;
                foreach (ClockVM c in MyClocks)
                {
                    if (c.GroupName != g.Name)
                    {
                        chk = false;
                        break;
                    }
                }
                tsmi.Checked = chk;

                tsmiGroupName.DropDownItems.Add(tsmi);
                tsmi.Click += Tsmi_Click;
            }

            var tsmiNew = new ToolStripMenuItem("Add to new group...");
            tsmiNew.Font = new Font(Font, FontStyle.Bold);
            tsmiNew.Image = Utils.IconResourceVersionBySize(Properties.Resources.Oxygen_Icons_org_Oxygen_Actions_list_add, 16);
            tsmiNew.Click += TsmiNew_Click;
            tsmiGroupName.DropDownItems.Add(tsmiNew);
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

            Hide();
        }

        private void tsmiResetTo_DropDownOpening(object sender, EventArgs e)
        {
            tsmiResetTo.DropDownItems.Clear();

            var tsmiUpdateToCurrent = new ToolStripMenuItem("Update to current");
            tsmiUpdateToCurrent.Click += TsmiUpdateToCurrent_Click;

            ToolStripMenuItem tsmiUpdateToCustom = null;
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
                tsmiUpdateToCustom = new ToolStripMenuItem("Update to custom...");
                tsmiUpdateToCustom.Click += TsmiUpdateToCustom_Click;
            }
            else
            {
                // TODO: [VISUAL] maybe just italic grayed out instead of removed from the menu?
            }

            var tsmiLockResetToValue = new ToolStripMenuItem("Lock reset-to value");
            tsmiLockResetToValue.CheckOnClick = true;
            tsmiLockResetToValue.Image = Utils.IconResourceVersionBySize(Properties.Resources.Oxygen_Icons_org_Oxygen_Actions_system_lock_screen, 16);
            tsmiLockResetToValue.Checked = MyClock.ResetToValueLocked;
            tsmiLockResetToValue.Click += TsmiLockResetToValue_Click;

            tsmiResetTo.DropDownItems.Add(tsmiUpdateToCurrent);
            if (tsmiUpdateToCustom != null)
            {
                tsmiResetTo.DropDownItems.Add(tsmiUpdateToCustom);
            }
            tsmiResetTo.DropDownItems.Add(tsmiLockResetToValue);
        }

        private void TsmiLockResetToValue_Click(object sender, EventArgs e)
        {
            var tsmi = (ToolStripMenuItem)sender;
            foreach (ClockVM c in MyClocks)
            {
                c.ResetToValueLocked = tsmi.Checked;
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
                cw.MyClockData = c.Model;
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

            OpenFileDialog fd = Utils.GetImageChooser();
            if (fd.ShowDialog(this) == DialogResult.OK)
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

                if (cd.ShowDialog(FindForm()) == DialogResult.OK)
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

            var tsmi = sender as ToolStripMenuItem;
            if (tsmi.Checked)
            {
                var hs = new HashSet<ClockVM>(MyClocks);
                foreach (ClockVM c in hs)
                {
                    c.GroupName = "";
                }

                DontFireTsmiClick = true;
                tsmi.Checked = false;
                DontFireTsmiClick = false;
            }
            else
            {
                var hs = new HashSet<ClockVM>(MyClocks);
                foreach (ClockVM c in hs)
                {
                    c.GroupName = tsmi.Text;
                }

                DontFireTsmiClick = true;
                tsmi.Checked = true;
                DontFireTsmiClick = false;
            }
        }
    }
}
