using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
    /// Interaction logic for BeepsDataGrid.xaml
    /// </summary>
    public partial class BeepsDataGrid : DataGrid
    {
        internal BeepCollection Beeps;

        public BeepsDataGrid()
        {
            InitializeComponent();

            Beeps = new BeepCollection();

            ItemsSource = Beeps;
        }

        private void MiDeleteSelectedRows_Click(object sender, RoutedEventArgs e)
        {
            for (int i = SelectedItems.Count - 1; i >= 0; --i)
            {
                Beeps.Remove(SelectedItems[i] as Beep);
            }
        }

        private void DataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            var b = e.NewItem as Beep;
            b.BeepDuration = 1000;
            b.BeepFrequency = 100;
            b.MsBeforeRinging = 1000;
        }
    }

    public class BeepValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value,
            System.Globalization.CultureInfo cultureInfo)
        {
            // TODO: coerce the invalid values to valid values when possible
            Beep b = (value as BindingGroup).Items[0] as Beep;
            if (b.MsBeforeRinging <= 0)
            {
                return new ValidationResult(false, "Ms Before Ringing must be >= 1.");
            }
            else if (b.BeepDuration <= 0)
            {
                return new ValidationResult(false, "Beep Duration must be >= 1.");
            }
            else if (b.BeepFrequency < 37)
            {
                return new ValidationResult(false, "Beep Frequency must be >= 37.");
            }
            else if (b.BeepFrequency > 32767)
            {
                return new ValidationResult(false, "Beep Frequency must be <= 32767.");
            }
            else
            {
                return ValidationResult.ValidResult;
            }
        }
    }
}
