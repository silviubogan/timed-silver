using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class ClockUserControl : UserControl
    {
        public ClockUserControl()
        {
            var conv = new UserBackColorToActualBackColor();
            conv.MyClockUserControl = this;
            this.Resources.Add("UserToActualColor", conv);

            InitializeComponent();

            DependencyPropertyDescriptor prop = DependencyPropertyDescriptor.FromProperty(Image.SourceProperty, typeof(Image));
            prop.AddValueChanged(this.MyImage, OnImageSourceChanged);
        }

        internal void OnImageSourceChanged(object sender, EventArgs e)
        {
            var o = sender as Image;
            UpdateImageVisibility();
        }

        public static readonly DependencyProperty ClockTagProperty = DependencyProperty.Register("ClockTag", typeof(string),
            typeof(ClockUserControl), new PropertyMetadata(""));
        public string ClockTag
        {
            get
            {
                return (string)GetValue(ClockTagProperty);
            }
            set
            {
                SetValue(ClockTagProperty, value);
            }
        }

        public static readonly DependencyProperty ClockIDProperty =
            DependencyProperty.Register("ClockID", typeof(string),
             typeof(ClockUserControl), new PropertyMetadata(""));
        public string ClockID
        {
            get
            {
                return (string)GetValue(ClockIDProperty);
            }
            set
            {
                SetValue(ClockIDProperty, value);
            }
        }

        public static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register("ZoomFactor", typeof(double),
                typeof(ClockUserControl), new PropertyMetadata(1d,
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
            ClockUserControl c = d as ClockUserControl;
            c.MyScaleTransform.ScaleX = (double)e.NewValue;
            c.MyScaleTransform.ScaleY = (double)e.NewValue;
        }

        public static readonly DependencyProperty RoundedCornersProperty =
            DependencyProperty.Register("RoundedCorners", typeof(bool), typeof(ClockUserControl),
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
            var o = d as ClockUserControl;

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


        public static readonly DependencyProperty DarkThemeProperty =
            DependencyProperty.Register("DarkTheme", typeof(bool), typeof(ClockUserControl),
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
            var o = d as ClockUserControl;

            if ((bool)e.NewValue)
            {
                o.MySecondGrid.Style = (Style)o.FindResource("darkTheme");
                o.MyTextBox.Style = (Style)o.FindResource("darkTextBoxTheme");
                o.MyBorder.Style = (Style)o.FindResource("darkBorderTheme");
            }
            else
            {
                o.MySecondGrid.Style = (Style)o.FindResource("lightTheme");
                o.MyTextBox.Style = (Style)o.FindResource("lightTextBoxTheme");
                o.MyBorder.Style = (Style)o.FindResource("lightBorderTheme");
            }
        }

        public static readonly DependencyProperty ClockStyleProperty =
            DependencyProperty.Register("ClockStyle", typeof(ClockM.ClockStyles),
                typeof(ClockUserControl), new PropertyMetadata(ClockM.ClockStyles.ShowIconAndID, OnClockStyleChanged));

        private static void OnClockStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var o = d as ClockUserControl;

            //if (o.MyImage.Source == null)
            //{
            //    o.MyThirdGrid.RowDefinitions[0].Height =
            //        new GridLength(0);
            //    o.MyThirdGrid.RowDefinitions[1].Height =
            //        new GridLength(1, GridUnitType.Star);
            //}
            //else
            //{
            //    o.MyThirdGrid.RowDefinitions[0].Height =
            //        new GridLength(1, GridUnitType.Star);
            //    o.MyThirdGrid.RowDefinitions[1].Height =
            //        new GridLength(1, GridUnitType.Star);
            //}
        }

        public ClockM.ClockStyles ClockStyle
        {
            get
            {
                return (ClockM.ClockStyles)GetValue(ClockStyleProperty);
            }
            set
            {
                SetValue(ClockStyleProperty, value);
            }
        }

        public static readonly DependencyProperty CheckableProperty =
            DependencyProperty.Register("Checkable", typeof(bool), typeof(ClockUserControl), new FrameworkPropertyMetadata(false, OnCheckableChanged));

        public bool Checkable
        {
            get
            {
                return (bool)GetValue(CheckableProperty);
            }
            set
            {
                SetValue(CheckableProperty, value);
            }
        }
        private static void OnCheckableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var o = d as ClockUserControl;

            (o.DataContext as ClockVM).Checkable = (bool)e.NewValue;
        }

        void UpdateImageVisibility()
        {
            if (MyImage.Source == null)
            {
                MyThirdGrid.RowDefinitions[0].Height =
                    new GridLength(0);
                MyThirdGrid.RowDefinitions[1].Height =
                    new GridLength(1, GridUnitType.Star);
            }
            else
            {
                MyThirdGrid.RowDefinitions[0].Height =
                    new GridLength(1, GridUnitType.Star);
                MyThirdGrid.RowDefinitions[1].Height =
                    new GridLength(1, GridUnitType.Star);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // TODO: fix bug: sometimes (probably always) DataContext is a ClockM,
            // not a ClockVM, and this method crashes because of this.

            var dc = DataContext as ClockVM;
            Point pt = PointToScreen(new Point(ActualWidth, 0));

            ViewsGrid vg = dc.Model.MyDataFile.MainWindow.MyViewsGrid;
            vg.MyFlowView.ShowContextMenu(
                    new HashSet<ClockVM>() { dc },
                    new System.Drawing.Point((int)Math.Round(pt.X), (int)Math.Round(pt.Y)));
        }

        public override string ToString()
        {
            return $"ClockUserControl \"{(DataContext as ClockVM).Tag}\"";
        }

        private void MyTextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            //e.Effects = DragDropEffects.Move;
            //e.Handled = true;
        }

        private void MyTextBox_DragOver(object sender, DragEventArgs e)
        {
        }

        private void MyTextBox_PreviewDragEnter(object sender, DragEventArgs e)
        {
        }

        private void MyTextBox_DragEnter(object sender, DragEventArgs e)
        {
        }
    }
}
