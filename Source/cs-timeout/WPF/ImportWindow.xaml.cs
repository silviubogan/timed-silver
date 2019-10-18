using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for ImportWindow.xaml
    /// </summary>
    public partial class ImportWindow : Window
    {
        static ImportWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImportWindow),
                new FrameworkPropertyMetadata(typeof(ImportWindow)));
        }

        public ImportWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            (GetTemplateChild("PART_ImportControl") as ImportControl).MyImportWindow = this;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
