using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace cs_timed_silver
{
    public class ClockDataToUserClockTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return value == null ? null :
                (value.GetType().Name == "AlarmData" ? "Alarm" : "Timer");
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}
