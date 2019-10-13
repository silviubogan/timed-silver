using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace wpf_timespanpicker
{
    internal static class Utils
    {
        internal static Size MeasureString(string candidate, FontFamily fontFamily, FontStyle fontStyle,
            FontWeight fontWeight, FontStretch fontStretch,
            double fontSize)
        {
            var formattedText = new FormattedText(
                candidate,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(
                    fontFamily, fontStyle,
                    fontWeight, fontStretch),
                fontSize,
                Brushes.Black,
                new NumberSubstitution(),
                TextFormattingMode.Ideal);

            return new Size(formattedText.Width, formattedText.Height);
        }
    }
}
