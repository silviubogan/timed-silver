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
    public class ClockToButtonIconPathConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (values[0] == DependencyProperty.UnsetValue ||
                values[1] == DependencyProperty.UnsetValue)
            {
                return new BitmapImage(
                    new Uri("/Resources/Oxygen-Icons.org-Oxygen-Actions-media-playback-start.ico",
                        UriKind.Relative));
            }
            var type = (ClockVM.ClockTypes)values[0];

            string path;

            if (type == ClockVM.ClockTypes.Timer)
            {
                var running = (bool)values[1];
                if (running)
                {
                    path = "/Resources/Oxygen-Icons.org-Oxygen-Actions-media-playback-pause.ico";
                }
                else
                {
                    path = "/Resources/Oxygen-Icons.org-Oxygen-Actions-media-playback-start.ico";
                }
            }
            else if (type == ClockVM.ClockTypes.Alarm)
            {
                var enabled = (bool)values[1];
                if (enabled)
                {
                    path = "/Resources/on filter.ico";
                }
                else
                {
                    path = "/Resources/off filter.ico";
                }
            }
            else if (type == ClockVM.ClockTypes.Stopwatch)
            {
                var running = (bool)values[1];
                if (running)
                {
                    path = "/Resources/Oxygen-Icons.org-Oxygen-Actions-media-playback-pause.ico";
                }
                else
                {
                    path = "/Resources/Oxygen-Icons.org-Oxygen-Actions-media-playback-start.ico";
                }
            }
            else
            {
                throw new NotImplementedException();
            }

            return new BitmapImage(new Uri(path, UriKind.Relative));
        }

        public object[] ConvertBack(object value, Type[] targetTypes,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
