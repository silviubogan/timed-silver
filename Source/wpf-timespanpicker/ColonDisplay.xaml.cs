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
using System.Windows.Threading;

namespace wpf_timespanpicker
{
    /// <summary>
    /// Interaction logic for ColonDisplay.xaml
    /// </summary>
    public partial class ColonDisplay : UserControl
    {
        public ColonDisplay()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty PulseProperty =
            DependencyProperty.Register("Pulse", typeof(bool), typeof(ColonDisplay),
                new FrameworkPropertyMetadata(false));

        public bool Pulse
        {
            get
            {
                return (bool)GetValue(PulseProperty);
            }
            set
            {
                SetValue(PulseProperty, value);
            }
        }
    }
}
