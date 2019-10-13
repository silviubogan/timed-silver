using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace cs_timed_silver
{
    /// <summary>
    /// Interaction logic for CustomStatusBar.xaml
    /// </summary>
    public partial class CustomStatusBar : UserControl
    {
        public LogWindow MyLogWindow;

        public ObservableCollection<LogMessageVM> Messages { get; set; }

        public static readonly DependencyProperty CurrentIconProperty =
            DependencyProperty.Register("CurrentIcon", typeof(ImageSource), typeof(CustomStatusBar), new PropertyMetadata(null));
        public ImageSource CurrentIcon
        {
            get
            {
                return (ImageSource)GetValue(CurrentIconProperty);
            }
            set
            {
                SetValue(CurrentIconProperty, value);
            }
        }

        public static readonly DependencyProperty CurrentSystemTimeProperty =
            DependencyProperty.Register("CurrentSystemTime", typeof(string), typeof(CustomStatusBar), new PropertyMetadata(""));
        public string CurrentSystemTime
        {
            get
            {
                return (string)GetValue(CurrentSystemTimeProperty);
            }
            set
            {
                SetValue(CurrentSystemTimeProperty, value);
            }
        }

        public static readonly DependencyProperty CurrentSystemDateProperty =
            DependencyProperty.Register("CurrentSystemDate", typeof(string), typeof(CustomStatusBar), new PropertyMetadata(""));
        public string CurrentSystemDate
        {
            get
            {
                return (string)GetValue(CurrentSystemDateProperty);
            }
            set
            {
                SetValue(CurrentSystemDateProperty, value);
            }
        }

        public static readonly DependencyProperty ShowTimeDateProperty =
            DependencyProperty.Register("ShowTimeDate", typeof(bool), typeof(CustomStatusBar), new PropertyMetadata(false));
        public bool ShowTimeDate
        {
            get
            {
                return (bool)GetValue(ShowTimeDateProperty);
            }
            set
            {
                SetValue(ShowTimeDateProperty, value);
            }
        }

        internal void ClearStatus()
        {
            MyMessageTextBlock.Text = "";
            CurrentIcon = null;
            MyTimeTextBlock.Text = "";

            Messages = new ObservableCollection<LogMessageVM>();
        }

        internal SystemClockTimer SysTimer;

        public CustomStatusBar()
        {
            InitializeComponent();
        }

        private void SysTimer_Tick(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;

            CurrentSystemTime = dt.ToShortTimeString();
            CurrentSystemDate = dt.ToShortDateString();
        }

        public void PostMessage(string msg, LogCategory c)
        {
            if (!MyLogWindow.LoggingEnabled)
            {
                return;
            }

            var l = new LogMessageVM()
            {
                Text = msg,
                Category = c,
                DateTime = DateTime.Now
            };
            Messages.Add(l);

            MyMessageTextBlock.Text = msg.
                Replace("\n", " ").
                Replace("\t", " ").
                Replace("\r", " ");
            CurrentIcon = l.Image;
            MyTimeTextBlock.Text = l.DateTime.ToLongTimeString();

            MyLogWindow.WriteToLog(l);
        }

        private void MyShowLogButton_Click(object sender, RoutedEventArgs e)
        {
            MyLogWindow.Show();
        }

        // required in XAML
        public static readonly DependencyProperty DataFileProperty =
            DependencyProperty.Register("DataFile", typeof(DataFile), typeof(CustomStatusBar),
                new FrameworkPropertyMetadata(null));
        public DataFile DataFile
        {
            get
            {
                return (DataFile)GetValue(DataFileProperty);
            }
            set
            {
                SetValue(DataFileProperty, value);
            }
        }

        private void MyCustomStatusBar_Initialized(object sender, EventArgs e)
        {
            Messages = new ObservableCollection<LogMessageVM>();

            SysTimer = new SystemClockTimer();
            SysTimer.Tick += SysTimer_Tick;

            DataContext = this;

            MyLogWindow = new LogWindow();
            MyLogWindow.Messages = Messages;

            MyLogWindow.OpenLogFile();
        }
    }
}
