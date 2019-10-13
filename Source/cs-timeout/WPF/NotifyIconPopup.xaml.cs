using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for NotifyIconPopup.xaml
    /// </summary>
    public partial class NotifyIconPopup : Window
    {
        public static readonly DependencyProperty ClocksProperty =
            DependencyProperty.Register("Clocks", typeof(ClockVMCollection),
                typeof(NotifyIconPopup), new FrameworkPropertyMetadata(null));
        public ClockVMCollection Clocks
        {
            get
            {
                return (ClockVMCollection)GetValue(ClocksProperty);
            }
            set
            {
                SetValue(ClocksProperty, value);
            }
        }

        public NotifyIconPopup()
        {
            InitializeComponent();

            DataContext = this;
        }

        public event EventHandler AppExitRequested;

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            AppExitRequested?.Invoke(this, e);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            Clocks = (Application.Current.MainWindow as MainWindow).VM.ClockVMCollection;
        }

        private void Popup_Loaded(object sender, RoutedEventArgs e)
        {
            Clocks = (Application.Current.MainWindow as MainWindow).VM.ClockVMCollection;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
