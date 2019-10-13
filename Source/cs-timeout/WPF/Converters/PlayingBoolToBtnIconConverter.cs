using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace cs_timed_silver
{
    public class PlayingBoolToBtnIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is bool))
            {
                return DependencyProperty.UnsetValue;
            }

            if ((bool)value == true)
            {
                return new BitmapImage(
                    new Uri("/Resources/Oxygen-Icons.org-Oxygen-Actions-media-playback-pause.ico", UriKind.Relative));
            }
            else
            {
                return new BitmapImage(
                    new Uri("/Resources/Oxygen-Icons.org-Oxygen-Actions-media-playback-start.ico", UriKind.Relative));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
