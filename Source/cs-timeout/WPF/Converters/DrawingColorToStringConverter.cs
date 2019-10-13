using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace cs_timed_silver
{
    public class DrawingColorToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is System.Drawing.Color))
            {
                return DependencyProperty.UnsetValue;
            }
            return Utils.ColorToString((System.Drawing.Color)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Utils.StringToColor(value as string);
        }
    }
}
