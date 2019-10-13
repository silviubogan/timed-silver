using System;
using System.Collections.Generic;
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
    public partial class LicenseWindow : Window
    {
        public LicenseWindow(string filePath)
        {
            InitializeComponent();

            var r = new TextRange(MyReader.Document.ContentStart, MyReader.Document.ContentEnd);

            using (Stream str = ExtractResource(filePath))
            {
                r.Load(str, DataFormats.Rtf);
            }
        }

        private Stream ExtractResource(string filename)
        {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            return a.GetManifestResourceStream(filename);
        }
    }
}
