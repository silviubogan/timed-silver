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
    public class DrawingColorToSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (!(value is System.Drawing.Color))
            {
                return DependencyProperty.UnsetValue;
            }
            var val = (System.Drawing.Color)value;
            return new System.Windows.Media.SolidColorBrush(new System.Windows.Media.Color()
            {
                A = val.A,
                R = val.R,
                G = val.G,
                B = val.B
            });
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();

            ////bool isColor = true;
            //if (!(value is System.Windows.Media.Color)) // Here handle Media.SolidColorBrush
            //{
            //    //isColor = false;
            //    return DependencyProperty.UnsetValue;
            //}
            ////if (!(value is System.Windows.Media.SolidColorBrush))
            ////{
            ////    return DependencyProperty.UnsetValue;
            ////}
            ////if (isColor)
            ////{
            //    var val = (System.Windows.Media.Color)value;
            //    return System.Drawing.Color.FromArgb(val.A, val.R, val.G, val.B);
            ////}
            ////else // solid brush
            ////{
            ////    var val = (System.Windows.Media.SolidColorBrush)value;
            ////    return System.Drawing.Color.FromArgb(val.Color.A, val.Color.R,
            ////        val.Color.G, val.Color.B);
            ////}
        }
    }
}
