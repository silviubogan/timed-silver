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
    public class UserBackColorToActualBorderColorConverter : IMultiValueConverter
    {
        public SolidColorBrush FocusedBrush { get; set; }
        public SolidColorBrush CheckedBrush { get; set; }
        public SolidColorBrush DarkThemeBrush { get; set; }
        public SolidColorBrush LightThemeBrush { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Contains(DependencyProperty.UnsetValue))
            {
                return LightThemeBrush;
            }

            var chked = (bool)values[0];
            var dark = (bool)values[1];
            var focused = (bool)values[2];

            if (chked)
            {
                return CheckedBrush;
            }

            if (focused)
            {
                return FocusedBrush;
            }

            if (dark)
            {
                return DarkThemeBrush;
            }

            return LightThemeBrush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
