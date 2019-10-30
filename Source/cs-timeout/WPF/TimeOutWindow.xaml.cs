using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace cs_timed_silver
{
    /// <summary>
    /// Interaction logic for TimeOutWindow.xaml
    /// </summary>
    public partial class TimeOutWindow : Window
    {
        public TimeOutWindow()
        {
            InitializeComponent();
        }

        internal Timer t;
        internal int ElapsedSeconds = 0;

        internal BitmapImage MuteImage = new BitmapImage(new Uri("/Resources/Oxygen-Icons.org-Oxygen-Status-audio-volume-muted.ico", UriKind.Relative));
        internal BitmapImage UnmuteImage = new BitmapImage(new Uri("/Resources/Oxygen-Icons.org-Oxygen-Status-audio-volume-high.ico", UriKind.Relative));

        public static readonly DependencyProperty ClockProperty =
            DependencyProperty.Register("Clock", typeof(ClockM), typeof(TimeOutWindow), new FrameworkPropertyMetadata(null));
        public ClockM Clock
        {
            get
            {
                return (ClockM)GetValue(ClockProperty);
            }
            set
            {
                SetValue(ClockProperty, value);
            }
        }

        internal bool HasFadeInAnimation = true;

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            // TODO: use DispatcherTimer instead:
            t = new Timer()
            {
                Interval = 1000
            };
            t.Tick += T_Tick;
        }

        private void ApplyMuteSetting(bool setting)
        {
            // TODO: use bindings:

            // do unmute
            if (setting)
            {
                BtnMuteImage.Source = UnmuteImage;
                BtnMuteAccessText.Text = "_Unmute";
            }
            else // do mute
            {
                BtnMuteImage.Source = MuteImage;
                BtnMuteAccessText.Text = "M_ute";
            }
        }

        private void T_Tick(object sender, EventArgs e)
        {
            ElapsedSeconds++;
            TimeSpan ts = TimeSpan.FromSeconds(ElapsedSeconds);
            TbElapsed.Text = $"Ringed for {ts.ToString()}";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HasFadeInAnimation = (int)Clock.MyDataFile.Settings.GetValue("TimeOutFormFadeInDuration") > 0;

            ImageSource bmp = Clock.TimeOutBackgroundImageSource;

            // there is no specific background image set for the Clock, so use the general background from the settings
            if (bmp == null)
            {
                string p = (string)Clock.MyDataFile.Settings.GetValue("TimeOutFormBackgroundImage");
                if (p != "")
                {
                    try
                    {
                        var b = new ImageBrush(new BitmapImage(new Uri(p, UriKind.RelativeOrAbsolute)));
                        b.TileMode = TileMode.None;
                        b.Stretch = Stretch.Uniform;
                        MyRootGrid.Background = b;
                    }
                    catch (Exception ex) // file not found, for example
                    {
                        // log somewhere
                    }
                }
            }
            else
            {
                try
                {
                    var b = new ImageBrush(bmp);
                    b.TileMode = TileMode.None;
                    b.Stretch = Stretch.Uniform;
                    MyRootGrid.Background = b;
                }
                catch (Exception ex) // file not found, for example
                {
                    // log somewhere
                }
            }

            if (Clock is TimerData td)
            {
                MyTimeSpanPicker.Value = TimeSpan.FromMinutes(10);

                MyDateTimePicker.Visibility = Visibility.Collapsed;
                MyTimeSpanPicker.Visibility = Visibility.Visible;
            }
            else if (Clock is AlarmData ad)
            {
                MyDateTimePicker.Value = ad.CurrentDateTime;

                LblFirst.Text = "Alarm";

                BtnRestartAndCloseAccessText.Text = "_Reenable for tomorrow and close";
                BtnBackAccessText.Text = "_Disable and close";
                BtnResetAndCloseAccessText.Text = "R_eset and close";

                MyDateTimePicker.Visibility = Visibility.Visible;
                MyTimeSpanPicker.Visibility = Visibility.Collapsed;
            }

            bool setting = Clock.MyDataFile.MultiAudioPlayer.Mute;
            ApplyMuteSetting(setting);

            if (HasFadeInAnimation)
            {
                Opacity = 0;
                var a = new DoubleAnimation(0, 1,
                    new Duration(TimeSpan.FromMilliseconds((int)Clock.MyDataFile.Settings.GetValue("TimeOutFormFadeInDuration"))));
                BeginAnimation(OpacityProperty, a);
            }

            if (Clock is TimerData)
            {
                MyClepsidraIcon.Visibility = Visibility.Visible;

                var c = new ColorAnimation(Colors.Black, Colors.Yellow, TimeSpan.FromSeconds(0.4));
                c.AutoReverse = true;
                c.RepeatBehavior = RepeatBehavior.Forever;
                (MyClepsidraIcon.MyThing.Fill as SolidColorBrush).BeginAnimation(SolidColorBrush.ColorProperty, c);
            }
            else // AlarmData
            {
                MyAlarmClockIcon.Visibility = Visibility.Visible;

                var r = new DoubleAnimation(-15, 15, TimeSpan.FromSeconds(0.4));
                r.AutoReverse = true;
                r.RepeatBehavior = RepeatBehavior.Forever;
                MyAlarmClockIcon.MyRotateTransform.BeginAnimation(RotateTransform.AngleProperty, r);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void BtnRestartAndClose_Click(object sender, RoutedEventArgs e)
        {
            if (Clock is TimerData td)
            {
                Clock.Reset();
                td.StartOrStop();
            }
            else if (Clock is AlarmData ad)
            {
                // for AlarmData-s the reset should be manual or do this:
                // add 1 day to the current date time
                ad.CurrentDateTime = ad.CurrentDateTime.AddDays(1);

                ad.EnableOrDisable();
            }
            Hide();
        }

        private void BtnResetAndClose_Click(object sender, RoutedEventArgs e)
        {
            Clock.Reset();
            // there is no sense in reenabling here because the reset-to value has different uses
            Hide();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            t.Dispose();
        }

        //internal bool IsBeingClosed { get; set; } = false;
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //IsBeingClosed = true;
            e.Cancel = true;
            Hide();
        }

        private void BtnMute_Click(object sender, RoutedEventArgs e)
        {
            bool setting = Clock.MyDataFile.MultiAudioPlayer.Mute;

            // do unmute
            if (setting)
            {
                Clock.MyDataFile.MultiAudioPlayer.Mute = false;
            }
            else // do mute
            {
                Clock.MyDataFile.MultiAudioPlayer.Mute = true;
            }

            ApplyMuteSetting(!setting);
        }

        private void BtnSnooze_Click(object sender, RoutedEventArgs e)
        {
            if (Clock is TimerData td)
            {
                TimeSpan ts = MyTimeSpanPicker.Value;
                td.CurrentTimeSpan += TimeSpan.FromSeconds((int)(ts.TotalSeconds));
                td.StartOrStop();
            }
            else if (Clock is AlarmData ad)
            {
                if (!MyDateTimePicker.Value.HasValue)
                {
                    throw new NotImplementedException();
                }
                DateTime dt = MyDateTimePicker.Value.Value;
                ad.CurrentDateTime = dt;
                ad.EnableOrDisable();
            }
            Hide();
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsVisible)
            {
                if (HasFadeInAnimation)
                {
                    BeginAnimation(OpacityProperty, null);
                }

                t.Stop();

                Clock.MyDataFile.MultiAudioPlayer.Mute = (bool)Clock.MyDataFile.Settings.GetValue("AlwaysMute");

                // TODO: call Unloaded handler's code here too

                return;
            }

            rtbTag.Background = Brushes.Transparent;
            rtbTag.IsReadOnly = true;

            rtbTag.Document = Clock.Tag.Clone();
            rtbTag.Document.TextAlignment = TextAlignment.Center;
            rtbTag.Document.Foreground = Brushes.Blue;

            foreach (Block b in rtbTag.Document.Blocks)
            {
                b.FontSize = b.FontSize * 3;
            }

            t.Start();

            if (HasFadeInAnimation)
            {
                BeginAnimation(OpacityProperty, null);
            }
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
