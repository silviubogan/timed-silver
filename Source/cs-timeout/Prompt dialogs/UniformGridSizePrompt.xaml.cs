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
    /// Interaction logic for UniformGridSizePrompt.xaml
    /// </summary>
    public partial class UniformGridSizePrompt : Window
    {
        public UniformGridSizePrompt()
        {
            InitializeComponent();
        }

        internal int RowCount { get; set; } = 0;
        internal int ColumnCount { get; set; } = 0;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RowCount = MyRowsIntegerUpDown.Value.Value;
            ColumnCount = MyColumnsIntegerUpDown.Value.Value;

            DialogResult = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //DialogResult = null;
        }
    }
}
