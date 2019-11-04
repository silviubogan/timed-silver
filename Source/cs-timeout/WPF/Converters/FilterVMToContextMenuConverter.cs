using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace cs_timed_silver
{
    public class FilterVMToContextMenuConverter : IValueConverter
    {
        public ContextMenu GroupsOnlyContextMenu { get; set; }
        public ContextMenu FilterOnlyContextMenu { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null ||
                !(value is ListView))
            {
                throw new NotImplementedException();
            }

            var lv = value as ListView;

            bool groupsOnly = true;
            foreach (FilterVM fd in lv.SelectedItems)
            {
                if (fd.M.GroupNames.Count == 0) // if it is a filter without group information
                {
                    groupsOnly = false;
                    break;
                }
            }

            return groupsOnly ? GroupsOnlyContextMenu :
                FilterOnlyContextMenu;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
