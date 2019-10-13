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
    public class UserBackColorToActualBackColor : IMultiValueConverter
    {
        // TODO: set this in XAML:
        public Control MyClockUserControl = null;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Control uc = MyClockUserControl;
            if (uc == null)
            {
                throw new NotImplementedException();
            }

            if (values.Contains(DependencyProperty.UnsetValue))
            {
                return (SolidColorBrush)uc.FindResource("LightThemeNormalBackBrush");
            }

            var userbg = (System.Drawing.Color)values[0];
            var active = (bool)values[1];
            var dark = (bool)values[2];

            if (userbg.A == 0)
            {
                if (dark)
                {
                    if (active)
                    {
                        return (SolidColorBrush)uc.FindResource("DarkThemeActiveBackBrush");
                    }
                    else
                    {
                        return (SolidColorBrush)uc.FindResource("DarkThemeNormalBackBrush");
                    }
                }
                else
                {
                    if (active)
                    {
                        return (SolidColorBrush)uc.FindResource("LightThemeActiveBackBrush");
                    }
                    else
                    {
                        return (SolidColorBrush)uc.FindResource("LightThemeNormalBackBrush");
                    }
                }
            }

            return new SolidColorBrush(new System.Windows.Media.Color()
            {
                A = userbg.A,
                R = userbg.R,
                G = userbg.G,
                B = userbg.B,
            });
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
