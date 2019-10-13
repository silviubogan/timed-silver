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

        public ImportWindowVM VM { get; set; }

        public DataFile MyDataFile, ImportedDataFile;

        private void BtnChooseXMLFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog fd = Utils.GetDataFileOpener();

            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                VM.SelectedFilePath = fd.FileName;
            }
        }

        internal Button BtnChooseXMLFile, BtnResetChosenFile,
            BtnCheckAllSettings, BtnUncheckAllSettings,
            BtnCheckAllClocks, BtnUncheckAllClocks,
            BtnCancel, BtnImportAll, BtnImportSelected;
        internal CheckBox CbDeleteExistingClocks;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            BtnChooseXMLFile = (Button)GetTemplateChild("BtnChooseXMLFile");
            BtnResetChosenFile = (Button)GetTemplateChild("BtnResetChosenFile");
            BtnCheckAllSettings = (Button)GetTemplateChild("BtnCheckAllSettings");
            BtnUncheckAllSettings = (Button)GetTemplateChild("BtnUncheckAllSettings");
            CbDeleteExistingClocks = (CheckBox)GetTemplateChild("CbDeleteExistingClocks");
            BtnCheckAllClocks = (Button)GetTemplateChild("BtnCheckAllClocks");
            BtnUncheckAllClocks = (Button)GetTemplateChild("BtnUncheckAllClocks");
            BtnCancel = (Button)GetTemplateChild("BtnCancel");
            BtnImportAll = (Button)GetTemplateChild("BtnImportAll");
            BtnImportSelected = (Button)GetTemplateChild("BtnImportSelected");

            BtnCancel.Click += BtnCancel_Click;
            BtnImportAll.Click += BtnImportAll_Click;
            BtnImportSelected.Click += BtnImportSelected_Click;
            
            BtnCheckAllClocks.Click += BtnCheckAllClocks_Click;
            BtnUncheckAllClocks.Click += BtnUncheckAllClocks_Click;

            BtnCheckAllSettings.Click += BtnCheckAllSettings_Click;
            BtnUncheckAllSettings.Click += BtnUncheckAllSettings_Click;

            BtnChooseXMLFile.Click += BtnChooseXMLFile_Click;
            BtnResetChosenFile.Click += BtnResetChosenFile_Click;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="justSelected">Whether to import only the items that are selected. If false, everything is imported.</param>
        internal void DoImport(bool justSelected = false)
        {
            foreach (SettingDataVM vm in VM.SettingVMs)
            {
                if (justSelected)
                {
                    if (vm.IsChecked)
                    {
                        MyDataFile.Settings.SetValue(vm.M.Name, vm.M.Value);
                    }
                }
                else
                {
                    MyDataFile.Settings.SetValue(vm.M.Name, vm.M.Value);
                }
            }

            if ((bool)CbDeleteExistingClocks.IsChecked)
            {
                MyDataFile.ClockVMCollection.VMs.Clear();
            }

            foreach (ClockVM vm in VM.ClockVMs.VMs)
            {
                if (justSelected)
                {
                    if (vm.Checked)
                    {
                        MyDataFile.ClockVMCollection.VMs.Add(vm);
                    }
                }
                else
                {
                    MyDataFile.ClockVMCollection.VMs.Add(vm);
                }
            }

            if (justSelected)
            {
                MessageBox.Show(this, "\"Import selected\" operation done. Press OK to close the import window.", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            else
            {
                MessageBox.Show(this, "\"Import all\" operation done. Press OK to close the import window.", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
        }

        private void BtnImportAll_Click(object sender, RoutedEventArgs e)
        {
            DoImport();
        }

        private void BtnImportSelected_Click(object sender, RoutedEventArgs e)
        {
            DoImport(true);
        }

        private void BtnResetChosenFile_Click(object sender, RoutedEventArgs e)
        {
            VM.Reset(ImportedDataFile);
        }

        private void BtnCheckAllSettings_Click(object sender, RoutedEventArgs e)
        {
            foreach (SettingDataVM vm in VM.SettingVMs)
            {
                vm.IsChecked = true;
            }
        }

        private void BtnUncheckAllSettings_Click(object sender, RoutedEventArgs e)
        {
            foreach (SettingDataVM vm in VM.SettingVMs)
            {
                vm.IsChecked = false;
            }
        }

        private void BtnCheckAllClocks_Click(object sender, RoutedEventArgs e)
        {
            foreach (ClockVM vm in VM.ClockVMs.VMs)
            {
                // multiple layers of VM types or a VM type specially made for showing in the ImportWindow.xaml
                vm.Checkable = true;
                vm.Checked = true;
            }
        }

        private void BtnUncheckAllClocks_Click(object sender, RoutedEventArgs e)
        {
            foreach (ClockVM vm in VM.ClockVMs.VMs)
            {
                vm.Checkable = false;
                vm.Checked = false;
            }
        }

        internal MainWindow MyMainWindow = null;

        public ImportWindow()
        {
            MyMainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            MyDataFile = MyMainWindow.VM;

            ImportedDataFile = new DataFile();

            Initialize();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        private void Initialize()
        {
            InitializeComponent();


            VM = new ImportWindowVM();

            VM.Reset(ImportedDataFile);

            DataContext = VM;
        }
    }
}
