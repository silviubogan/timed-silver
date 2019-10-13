using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace cs_timed_silver
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public static RoutedUICommand NavigateCommand { get; set; } =
            new RoutedUICommand("", "NavigateCommand", typeof(AboutWindow));

        static AboutWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AboutWindow),
                new FrameworkPropertyMetadata(typeof(AboutWindow)));
        }

        public AboutWindow()
        {
            InitializeComponent();

            DataContext = this;

            Title = $"About - v{Assembly.GetAssembly(typeof(MainWindow)).GetName().Version}";
        }

        internal Button BtnLicenseOxygen;
        internal Button BtnLicenseNAudio;
        internal Button BtnLicenseExtendedWPFToolkit;
        internal Button BtnBack;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            BtnLicenseOxygen = (Button)GetTemplateChild("BtnLicenseOxygen");
            BtnLicenseNAudio = (Button)GetTemplateChild("BtnLicenseNAudio");
            BtnLicenseExtendedWPFToolkit = (Button)GetTemplateChild("BtnLicenseExtendedWPFToolkit");
            BtnBack = (Button)GetTemplateChild("BtnBack");

            BtnLicenseOxygen.Click += BtnLicenseOxygen_Click;
            BtnLicenseNAudio.Click += BtnLicenseNAudio_Click;
            BtnLicenseExtendedWPFToolkit.Click += BtnLicenseExtendedWPFToolkit_Click;
            BtnBack.Click += Button_Click;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            //System.Diagnostics.Process.Start(
            //    new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri));
            //e.Handled = true;
        }

        private void BtnLicenseOxygen_Click(object sender, RoutedEventArgs e)
        {
            var w = new LicenseWindow("cs_timed_silver.Resources.oxygen-icons-license.rtf");
            w.Owner = this;
            w.ShowDialog();
        }

        private void BtnLicenseNAudio_Click(object sender, RoutedEventArgs e)
        {
            var w = new LicenseWindow("cs_timed_silver.Resources.naudio-license.rtf");
            w.Owner = this;
            w.ShowDialog();
        }

        private void BtnLicenseExtendedWPFToolkit_Click(object sender, RoutedEventArgs e)
        {
            var w = new LicenseWindow("cs_timed_silver.Resources.xceed-wpftoolkit-license.rtf");
            w.Owner = this;
            w.ShowDialog();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(
                new System.Diagnostics.ProcessStartInfo((e.Parameter as Uri).AbsoluteUri));
        }
    }
}
