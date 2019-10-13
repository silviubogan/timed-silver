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
    public class FilterClocksByTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ClockVM))
            {
                throw new NotImplementedException();
            }

            var cvm = value as ClockVM;

            if (parameter.ToString() == "Alarm")
            {
                return cvm.ClockType == ClockVM.ClockTypes.Alarm ? Visibility.Visible : Visibility.Collapsed;
            }
            else if (parameter.ToString() == "Timer")
            {
                return cvm.ClockType == ClockVM.ClockTypes.Timer ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
