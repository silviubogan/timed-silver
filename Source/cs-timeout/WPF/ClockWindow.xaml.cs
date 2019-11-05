using System;
using System.Collections.Generic;
using System.Linq;
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

namespace cs_timed_silver
{
    /// <summary>
    /// Interaction logic for ClockWindow.xaml
    /// </summary>
    public partial class ClockWindow : Window
    {
        public ClockWindow()
        {
            InitializeComponent();
        }

        internal ClockVM _MyClockData = null;
        public ClockVM MyClockData // TODO: make this a dependency property
        {
            get
            {
                return _MyClockData;
            }
            set
            {
                if (_MyClockData != value)
                {
                    _MyClockData = value;
                    OnMyClockDataChanged();
                }
            }
        }

        private void OnMyClockDataChanged()
        {
            DataContext = MyClockData;
            MyClockData.Model.Deleted += MyClockData_Deleted;
        }

        private void MyClockData_Deleted(object sender, EventArgs e)
        {
            Close();
        }

        public static readonly DependencyProperty DarkThemeProperty =
            DependencyProperty.Register("DarkTheme", typeof(bool), typeof(ClockWindow),
                new PropertyMetadata(false));
        public bool DarkTheme
        {
            get
            {
                return (bool)GetValue(DarkThemeProperty);
            }
            set
            {
                SetValue(DarkThemeProperty, value);
            }
        }

        public static readonly DependencyProperty RoundedCornersProperty =
            DependencyProperty.Register("RoundedCorners", typeof(bool), typeof(ClockWindow),
                new PropertyMetadata(false));
        public bool RoundedCorners
        {
            get
            {
                return (bool)GetValue(RoundedCornersProperty);
            }
            set
            {
                SetValue(RoundedCornersProperty, value);
            }
        }

        internal ScaleTransform MyViewboxScaleTransform;

        private void ZoomToolBar_ZoomApplicationRequested(object sender, EventArgs e)
        {
            if (MyViewboxScaleTransform == null)
            {
                return;
            }

            MyViewbox.Stretch = Stretch.None;
            //if (MyZoomToolBar.Value < 100)
            //{
            MyViewboxScaleTransform.ScaleX = MyViewboxScaleTransform.ScaleY = decimal.ToDouble(MyZoomToolBar.Value / 100);
            ///LayoutTransform = MyViewboxScaleTransform;
            //}
        }


        private void MyClockUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            MyViewboxScaleTransform = new ScaleTransform(1, 1);

            MyClockUserControl.LayoutTransform = MyViewboxScaleTransform;

            // TODO: Optional ZoomToolBar feature: zoom to fit button.
            //MyViewbox.Stretch = Stretch.Uniform;
        }

        private void MyZoomToolBar_ZoomToFitRequested(object sender, EventArgs e)
        {
            MyViewbox.Stretch = Stretch.Uniform;
        }
    }
}
