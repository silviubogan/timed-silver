using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace cs_timed_silver
{
    /// <summary>
    /// Interaction logic for KeepOnTopMenuItem.xaml
    /// </summary>
    public partial class KeepOnTopMenuItem : MenuItem
    {
        public KeepOnTopMenuItem()
        {
            InitializeComponent();
        }

        internal void SetRemainingTimeSpan(TimeSpan ts)
        {
            ApplyTemplate();
            var c = Template.FindName("remainingTimeSpanText", this) as TextBlock;
            if (c == null)
            {
                return;
            }
            if (ts > TimeSpan.Zero)
            {
                if (ts > TimeSpan.FromDays(1))
                {
                    c.Text = "In Process";
                }
                else
                {
                    c.Text = ts.ToString();
                }
            }
            else
            {
                c.Text = "";
            }
        }
    }
}
