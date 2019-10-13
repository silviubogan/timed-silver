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
using System.Windows.Threading;

namespace cs_timed_silver
{
    /// <summary>
    /// Interaction logic for ClockUserControl.xaml
    /// </summary>
    public partial class ClockUserControlPlaceholder : UserControl
    {
        public ClockUserControlPlaceholder()
        {
            InitializeComponent();
        }

        #region ZoomFactorProperty

        public static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register("ZoomFactor", typeof(double),
                typeof(ClockUserControlPlaceholder), new PropertyMetadata(1d,
                OnZoomFactorChanged));
        public double ZoomFactor
        {
            get
            {
                return (double)GetValue(ZoomFactorProperty);
            }
            set
            {
                SetValue(ZoomFactorProperty, value);
            }
        }

        private static void OnZoomFactorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as ClockUserControlPlaceholder;
            c.MyScaleTransform.ScaleX = (double)e.NewValue;
            c.MyScaleTransform.ScaleY = (double)e.NewValue;
        }

        #endregion

        #region RoundedCornersProperty

        public static readonly DependencyProperty RoundedCornersProperty =
            DependencyProperty.Register("RoundedCorners", typeof(bool), typeof(ClockUserControlPlaceholder),
                new PropertyMetadata(true, OnRoundedCornersChanged));

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

        private static void OnRoundedCornersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var o = d as ClockUserControlPlaceholder;

            if ((bool)e.NewValue)
            {
                o.MyRectangleGeometry.RadiusX = o.MyRectangleGeometry.RadiusY = 10d;
                o.MyBorder.CornerRadius = new CornerRadius(10d);
            }
            else
            {
                o.MyRectangleGeometry.RadiusX = o.MyRectangleGeometry.RadiusY = 0d;
                o.MyBorder.CornerRadius = new CornerRadius(0d);
            }
        }

        #endregion

        #region DarkThemeProperty

        public static readonly DependencyProperty DarkThemeProperty =
            DependencyProperty.Register("DarkTheme", typeof(bool), typeof(ClockUserControlPlaceholder),
            new PropertyMetadata(false, OnDarkThemeChanged));
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
        private static void OnDarkThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var o = d as ClockUserControlPlaceholder;

            if ((bool)e.NewValue)
            {
                o.MySecondGrid.Style = (Style)o.FindResource("darkTheme");
                //o.MyTextBox.Style = (Style)o.FindResource("darkTextBoxTheme");
                o.MyBorder.Style = (Style)o.FindResource("darkBorderTheme");
            }
            else
            {
                o.MySecondGrid.Style = (Style)o.FindResource("lightTheme");
                //o.MyTextBox.Style = (Style)o.FindResource("lightTextBoxTheme");
                o.MyBorder.Style = (Style)o.FindResource("lightBorderTheme");
            }
        }

        #endregion

        #region TextProperty

        public static readonly DependencyProperty TextProperty =
           DependencyProperty.Register("Text", typeof(string), typeof(ClockUserControlPlaceholder),
           new PropertyMetadata("", OnTextChanged));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var o = d as ClockUserControlPlaceholder;
        }

        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        #endregion

        #region DetailsProperty

        public static readonly DependencyProperty DetailsProperty =
           DependencyProperty.Register("Details", typeof(List<string>), typeof(ClockUserControlPlaceholder),
           new PropertyMetadata(new List<string>(), OnDetailsChanged));

        private static void OnDetailsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var o = d as ClockUserControlPlaceholder;
        }

        public List<string> Details
        {
            get
            {
                return (List<string>)GetValue(DetailsProperty);
            }
            set
            {
                SetValue(DetailsProperty, value);
            }
        }

        #endregion

        public override string ToString()
        {
            return $"ClockUserControlPlaceholder";
        }
    }
}
