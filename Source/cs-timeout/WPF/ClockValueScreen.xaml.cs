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
    /// Interaction logic for ClockValueDisplay.xaml
    /// </summary>
    public partial class ClockValueScreen : UserControl
    {
        public ClockValueScreen()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty CurrentValueProperty =
            DependencyProperty.Register("CurrentValue", typeof(object),
                typeof(ClockValueScreen),
                new UIPropertyMetadata(null, OnCurrentValueChanged));

        private static void OnCurrentValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //var o = d as ClockValueScreen;
            //o.ApplyTemplate();
            //var child = Utils.GetVisualChild<wpf_timespanpicker.TimeSpanPicker>(o);
            //if (child != null) // TODO: child is null, it is not in the visual tree
            //{
            //    child.TimeSpan = (TimeSpan)e.NewValue;
            //}
        }

        public object CurrentValue
        {
            get
            {
                return GetValue(CurrentValueProperty);
            }
            set
            {
                SetValue(CurrentValueProperty, value);
            }
        }

        internal wpf_timespanpicker.TimeSpanPicker MyTimeSpanPicker = null;

        private void MyTimeSpanPicker_Loaded(object sender, RoutedEventArgs e)
        {
            //MyTimeSpanPicker = (wpf_timespanpicker.TimeSpanPicker)sender;
            //MyTimeSpanPicker.TimeSpanValueChanged += MyTimeSpanPicker_TimeSpanValueChanged;

            //// TimeSpanValueChanged = "MyTimeSpanPicker_TimeSpanValueChanged"
            //var c = sender as ContentControl;

            //wpf_timespanpicker.TimeSpanPicker tsp = Utils.GetVisualChild<wpf_timespanpicker.TimeSpanPicker>(c);

            //tsp.TimeSpanValueChanged += MyTimeSpanPicker_TimeSpanValueChanged;
        }

        internal Xceed.Wpf.Toolkit.DateTimePicker MyDateTimePicker;
        private void DateTimePicker_Loaded(object sender, RoutedEventArgs e)
        {
            //MyDateTimePicker = (Xceed.Wpf.Toolkit.DateTimePicker)sender;
            //MyDateTimePicker.ValueChanged += DateTimePicker_ValueChanged;
        }
    }
}
