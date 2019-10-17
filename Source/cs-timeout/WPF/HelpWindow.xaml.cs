using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
    /// Interaction logic for HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();

            var d = (FlowDocument)new Markdown.Xaml.TextToFlowDocumentConverter().Convert(File.ReadAllText("Resources\\Docs\\Help.md", Encoding.UTF8), null, null, CultureInfo.InvariantCulture);

            MyReader.Document = d;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void PerformGoToPage(object sender, ExecutedRoutedEventArgs e)
        {
            Utils.OpenUrlInDefaultApp((string)e.Parameter);
        }

        private void CanGoToPage(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}
