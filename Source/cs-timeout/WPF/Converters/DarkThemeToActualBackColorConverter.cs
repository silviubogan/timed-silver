using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace cs_timed_silver
{
    public class DarkThemeToActualBackColorConverter : IValueConverter
    {
        // TODO: set this in XAML:
        public Control MyClockUserControl { get; set; } = null;

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            Control uc = MyClockUserControl;
            if (uc == null)
            {
                throw new NotImplementedException();
            }

            if (value == DependencyProperty.UnsetValue)
            {
                return (SolidColorBrush)uc.FindResource("LightThemeNormalBackBrush");
            }

            var dark = (bool)value;

            return (SolidColorBrush)uc.FindResource("DarkThemeNormalBackBrush");
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
