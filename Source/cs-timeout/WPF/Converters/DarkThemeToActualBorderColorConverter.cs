using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace cs_timed_silver
{
    public class DarkThemeToActualBorderColorConverter : IValueConverter
    {
        public SolidColorBrush DarkThemeBrush { get; set; }
        public SolidColorBrush LightThemeBrush { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == DependencyProperty.UnsetValue)
            {
                return LightThemeBrush;
            }

            if ((bool)value)
            {
                return DarkThemeBrush;
            }
            else
            {
                return LightThemeBrush;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
