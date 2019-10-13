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
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        internal DataFile MyDataFile { get; set; } = null;

        public SettingsWindow(DataFile dataFile) : this()
        {
            MyDataFile = dataFile;

            DataContext = MyDataFile;
        }

        public SettingsWindow()
        {
            InitializeComponent();
        }

        internal bool FirstLoad = true;

        private void SettingsWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
            {
                if (FirstLoad)
                {
                    this.ApplyTemplateRecursively();
                    MyControl.ApplyTemplateRecursively();

                    MainWindow = Owner as MainWindow;

                    Dispatcher.Invoke(new Action(() =>
                    {
                        if (FirstLoad)
                        {
                            //MyImageFileSelector.MainWindow = MainWindow;
                            //MyAudioFileSelector.MainWindow = MainWindow;

                            //MyAudioFileSelector.FilePathChanged += AudioFileChooser1_FilePathChanged;

                            if (MyControl.MyItemsControl.ItemsSource == null)
                            {
                                // ?
                            }
                            else
                            {
                                var v = (CollectionView)CollectionViewSource.GetDefaultView(MyControl.MyItemsControl.ItemsSource);
                                v.SortDescriptions.Add(new System.ComponentModel.SortDescription("Category", System.ComponentModel.ListSortDirection.Ascending));
                                v.GroupDescriptions.Add(new PropertyGroupDescription("Category")
                                {
                                });
                            }

                            FirstLoad = false;
                        }
                    }), System.Windows.Threading.DispatcherPriority.Loaded);
                }

                if (MyControl.MyItemsControl.ItemsSource == null)
                {
                    // ?
                }
                else
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        MyControl.LoadValuesToBeEditedFromDb();

                        MyControl.CbAutoOpenLastFile.Text = Properties.Settings.Default.AutoOpenLastFile;
                        MyControl.CbStartProgramWithWindows.IsChecked = Utils.ProgramStartsWithWindows();

                        MyControl.DgBeeps.Beeps.Clear();
                        foreach (Beep b in MainWindow.VM.Beeps)
                        {
                            MyControl.DgBeeps.Beeps.Add(b);
                        }
                    }), System.Windows.Threading.DispatcherPriority.Loaded);
                }
            }
            else
            {
                MyControl.ApplyTemplateRecursively();

                MyControl.MyAudioFileSelector.AudioPlayer.StopSound();
            }
        }

        internal MainWindow MainWindow = null;

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }
    }
}
