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
    /// Interaction logic for SmallClockUserControl.xaml
    /// </summary>
    public partial class SmallClockUserControl : UserControl
    {
        public SmallClockUserControl()
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
        }

        public static readonly DependencyProperty ClockTagProperty = DependencyProperty.Register("ClockTag", typeof(string),
            typeof(SmallClockUserControl), new PropertyMetadata(""));
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
             typeof(SmallClockUserControl), new PropertyMetadata(""));
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

        public static readonly DependencyProperty RoundedCornersProperty =
            DependencyProperty.Register("RoundedCorners", typeof(bool), typeof(SmallClockUserControl),
                new PropertyMetadata(true));

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


        public static readonly DependencyProperty DarkThemeProperty =
            DependencyProperty.Register("DarkTheme", typeof(bool), typeof(SmallClockUserControl),
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
                //o.MySecondGrid.Style = (Style)o.FindResource("darkTheme");
                //o.MyTextBox.Style = (Style)o.FindResource("darkTextBoxTheme");
                //o.MyBorder.Style = (Style)o.FindResource("darkBorderTheme");
            }
            else
            {
                //o.MySecondGrid.Style = (Style)o.FindResource("lightTheme");
                //o.MyTextBox.Style = (Style)o.FindResource("lightTextBoxTheme");
                //o.MyBorder.Style = (Style)o.FindResource("lightBorderTheme");
            }
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    var dc = DataContext as ClockVM;
        //    Point pt = PointToScreen(new Point(ActualWidth, 0));

        //    ViewsGrid vg;
        //    if (dc.Model.MyDataFile.MainForm != null)
        //    {
        //        vg = dc.Model.MyDataFile.MainForm.viewsGrid1;
        //    }
        //    else
        //    {
        //        vg = dc.Model.MyDataFile.MainWindow.MyViewsGrid;
        //    }
        //    vg.MyFlowView.ShowContextMenu(
        //            new HashSet<ClockVM>() { dc },
        //            new System.Drawing.Point((int)Math.Round(pt.X), (int)Math.Round(pt.Y)));
        //}

        public override string ToString()
        {
            return $"SmallClockUserControl \"{(DataContext as ClockVM).Tag}\"";
        }

        private void MyClockUC_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            (Application.Current.MainWindow as MainWindow).FocusClockVM(DataContext as ClockVM);
        }
    }
}
