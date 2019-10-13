using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace cs_timed_silver
{
    public class ImportWindowVM : BindableBase
    {
        protected MainWindow MyMainWindow;

        public ImportWindowVM()
        {
            MyMainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
        }

        private string _SelectedFilePath = "";
        public string SelectedFilePath
        {
            get { return _SelectedFilePath; }
            set { SetProperty(ref _SelectedFilePath, value, OnSelectedFilePathChanged); }
        }

        private string _DisplayFilePath = "";
        public string DisplayFilePath
        {
            get { return _DisplayFilePath; }
            set { SetProperty(ref _DisplayFilePath, value); }
        }

        private ObservableCollection<SettingDataVM> _SettingVMs =
            new ObservableCollection<SettingDataVM>();
        public ObservableCollection<SettingDataVM> SettingVMs
        {
            get { return _SettingVMs; }
            set { SetProperty(ref _SettingVMs, value); }
        }

        private ClockVMCollection _ClockVMs;
        public ClockVMCollection ClockVMs
        {
            get { return _ClockVMs; }
            set { SetProperty(ref _ClockVMs, value); }
        }

        private void OnSelectedFilePathChanged()
        {
            DisplayFilePath =
                SelectedFilePath != "" ? SelectedFilePath : "No file selected.";

            if (SelectedFilePath != "")
            {
                DataFile idf;
                idf = new DataFile();

                if (!idf.LoadFromFileWPF(SelectedFilePath))
                {
                    // TODO: show the exception message, not this:
                    MessageBox.Show("Error when loading the selected file path. Please try opening it with the main window for a better description of the error.");
                    SelectedFilePath = "";
                    return;
                }

                SettingVMs.Clear();
                foreach (KeyValuePair<string, SettingDataM> p in idf.Settings)
                {
                    var s = new SettingDataVM(p.Value);
                    s.IsChecked = true;
                    SettingVMs.Add(s);
                }

                ClockVMs.VMs.Clear();
                foreach (ClockVM vm in idf.ClockVMCollection.VMs)
                {
                    ClockVMs.VMs.Add(vm);
                }
            }
            else
            {
                SettingVMs.Clear();
                ClockVMs.VMs.Clear();
            }
        }

        internal void Reset(DataFile importDataFile)
        {
            SelectedFilePath = "";
            ClockVMs = new ClockVMCollection(importDataFile);
            SettingVMs = new ObservableCollection<SettingDataVM>();
        }
    }
}
