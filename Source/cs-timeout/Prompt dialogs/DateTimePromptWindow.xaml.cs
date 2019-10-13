using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for TextPromptWindow.xaml
    /// </summary>
    public partial class DateTimePromptWindow : Window
    {
        public DateTimePromptWindow()
        {
            InitializeComponent();

        }

        internal DateTime UserDateTime { get; set; } = DateTime.Now;

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            UserDateTime = (DateTime)MyDateTimePicker.Value;

            DialogResult = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MyDateTimePicker.Value = UserDateTime;
            MyDateTimePicker.Focus();
            MyDateTimePicker.SelectAll();
        }
    }
}
