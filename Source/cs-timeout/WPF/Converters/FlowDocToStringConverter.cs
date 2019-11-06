using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;

namespace cs_timed_silver
{
    public class FlowDocToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is FlowDocument) ||
                value is null)
            {
                return DependencyProperty.UnsetValue;
            }

            var fd = value as FlowDocument;
            var p = fd.Blocks?.FirstBlock as Paragraph;
            if (p != null)
            {
                return XamlWriter.Save(new Section(p));
            }
            var s = fd.Blocks?.FirstBlock as Section;
            if (s != null)
            {
                return XamlWriter.Save(s);
            }
            var l = fd.Blocks?.FirstBlock as List;
            if (l != null)
            {
                return XamlWriter.Save(new Section(l));
            }
            return XamlWriter.Save(new Section());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fd = new FlowDocument();
            var s = XamlReader.Parse(value as string) as Section;
            fd.Blocks.Add(s);
            return fd;
        }
    }
}
