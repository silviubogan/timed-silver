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
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : Control
    {
        public SettingsControl()
        {
            InitializeComponent();

            Loaded += SettingsControl_Loaded;
        }

        private void SettingsControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (MyItemsControl.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                HandleContainersGenerated();

            }
            else
            {
                MyItemsControl.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
            }
        }

        static SettingsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SettingsControl),
                new FrameworkPropertyMetadata(typeof(SettingsControl)));
        }

        public SettingsWindow MySettingsWindow { get; set; } = null;

        public ItemsControl MyItemsControl;
        public ComboBox CbAutoOpenLastFile;
        public CheckBox CbStartProgramWithWindows;
        public BeepsDataGrid DgBeeps;
        public TextBlock TbBeepsPlayerBtnText;
        public Image ImgBeepsPlayer;
        public Button PartBtnSave, PartBtnCancel, PartBtnResetToDefaults,
            PartBtnPlayStopBeeps;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.ApplyTemplateRecursively();

            MyItemsControl = (ItemsControl)GetTemplateChild("PART_MyItemsControl");
            CbAutoOpenLastFile = (ComboBox)GetTemplateChild("PART_CbAutoOpenLastFile");
            DgBeeps = (BeepsDataGrid)GetTemplateChild("PART_DgBeeps");
            TbBeepsPlayerBtnText = (TextBlock)GetTemplateChild("PART_TbBeepsPlayerBtnText");
            ImgBeepsPlayer = (Image)GetTemplateChild("PART_ImgBeepsPlayer");
            CbStartProgramWithWindows = (CheckBox)GetTemplateChild("PART_CbStartProgramWithWindows");

            PartBtnCancel = (Button)GetTemplateChild("PART_BtnCancel");
            PartBtnResetToDefaults = (Button)GetTemplateChild("PART_BtnResetToDefaults");
            PartBtnSave = (Button)GetTemplateChild("PART_BtnSave");
            PartBtnPlayStopBeeps = (Button)GetTemplateChild("PART_BtnPlayStopBeeps");

            PartBtnCancel.Click += BtnCancel_Click;
            PartBtnResetToDefaults.Click += BtnResetToDefaults_Click;
            PartBtnSave.Click += BtnSave_Click;
            PartBtnPlayStopBeeps.Click += BtnPlayStopBeeps_Click;


            MyRecentAudioFilesContextMenu.PathValidationRequested += RecentFilesMenuStrip1_PathValidationRequested;
            MyRecentImageFilesContextMenu.PathValidationRequested += RecentFilesMenuStrip2_PathValidationRequested;
        }

        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (MyItemsControl.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                MyItemsControl.ItemContainerGenerator.StatusChanged -= ItemContainerGenerator_StatusChanged;
                HandleContainersGenerated();
            }
        }

        internal DataFile MyDataFile
        {
            get
            {
                return MySettingsWindow.MyDataFile;
            }
        }

        internal void HandleContainersGenerated()
        {
            MyAudioFileSelector = null;
            MyImageFileSelector = null;

            this.ApplyTemplateRecursively();

            for (int i = 1; i >= 0; --i)
            {
                var cvg = MyItemsControl.ItemContainerGenerator.ContainerFromItem(MyItemsControl.ItemContainerGenerator.Items[i]);

                cvg.ApplyTemplateRecursively();

                MyAudioFileSelector = MyAudioFileSelector ?? cvg.FindVisualChildren<AudioFileSelector>().FirstOrDefault();
                MyImageFileSelector = MyImageFileSelector ?? cvg.FindVisualChildren<ImageFileSelector>().FirstOrDefault();
            }

            //foreach (SettingDataVM vm in MyDataFile.SettingsVMs)
            //{
            //    if (vm is AudioFileSettingDataVM)
            //    {
            //        ContentPresenter cp = null;



            //        //var cp = (ContentPresenter)((MyItemsControl.ItemContainerGenerator.Items[0] as ItemContainerGenerator).ContainerFromItem(vm));

            //        //if (cp == null)
            //        //{
            //        //    cp = (ContentPresenter)((MyItemsControl.ItemContainerGenerator.Items[1] as ItemContainerGenerator).ContainerFromItem(vm));

            //        //    //cp = (ContentPresenter)MyItemsControl.ItemContainerGenerator.item.ContainerFromItem(vm);

            //        //    if (cp == null)
            //        //    {
            //        //        continue;
            //        //    }
            //        //}

            //        cp.ApplyTemplateRecursively();
            //        var afs = (AudioFileSelector)GetRootControlFromContentPresenter(cp);
            //        if (afs != null)
            //        {
            //            MyAudioFileSelector = afs;
            //            break;
            //        }
            //    }
            //}

            //foreach (SettingDataVM vm in MyDataFile.SettingsVMs)
            //{
            //    if (vm is ImageFileSettingDataVM)
            //    {
            //        var cp = (ContentPresenter)MyItemsControl.ItemContainerGenerator.ContainerFromItem(vm);
            //        if (cp == null)
            //        {
            //            continue;
            //        }

            //        cp.ApplyTemplateRecursively();
            //        var ifs = (ImageFileSelector)GetRootControlFromContentPresenter(cp);
            //        if (ifs != null)
            //        {
            //            MyImageFileSelector = ifs;
            //            break;
            //        }
            //    }
            //}
        }

        internal Control GetRootControlFromContentPresenter(ContentPresenter container)
        {
            container.ApplyTemplate();
            return (Control)VisualTreeHelper.GetChild(container, 0);
        }

        internal AudioFileSelector MyAudioFileSelector { get; set; } = null;

        internal ImageFileSelector MyImageFileSelector { get; set; } = null;

        internal RecentAudioFilesContextMenu MyRecentAudioFilesContextMenu
        {
            get
            {
                return (RecentAudioFilesContextMenu)FindResource("MyRecentAudioFilesContextMenu");

                //MyControl.ApplyTemplate();
                //return (RecentAudioFilesContextMenu)MyControl.
                //    Template.FindName("MyRecentAudioFilesContextMenu", MyControl);

                //MyAudioFileSelector?.ApplyTemplate();
                //return MyAudioFileSelector?.RecentAudioFilesContextMenu;
            }
        }

        internal RecentFilesContextMenu MyRecentImageFilesContextMenu
        {
            get
            {
                return (RecentFilesContextMenu)FindResource("MyRecentImageFilesContextMenu");

                //MyControl.ApplyTemplate();
                //return (RecentFilesContextMenu)MyControl.
                //    Template.FindName("MyRecentImageFilesContextMenu", MyControl);

                //MyImageFileSelector?.ApplyTemplate();
                //return MyImageFileSelector?.RecentImageFilesContextMenu;
            }
        }


        private void RecentFilesMenuStrip1_PathValidationRequested(
            object sender, PathValidationEventArgs e)
        {
            e.Valid = MyAudioFileSelector.SetAudioPath(e.Path);
        }

        private void RecentFilesMenuStrip2_PathValidationRequested(object sender, PathValidationEventArgs e)
        {
            e.Valid = MyImageFileSelector.SetImagePath(e.Path);
        }

        internal string LastAudioFilePath = "";

        private void AudioFileChooser1_FilePathChanged(object sender, FilePathEventArgs e)
        {
            LastAudioFilePath = e.NewFilePath;
        }

        internal void LoadAudioFilePathFromSetting()
        {
            //MyAudioFileSelector.SetAudioPath((string)MainWindow.VM.Settings.GetValue("AudioFilePath"));
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            LoadAudioFilePathFromSetting();

            MyImageFileSelector.SetImagePath(
                (string)MyDataFile.Settings.GetValue("TimeOutFormBackgroundImage"));

            DgBeeps.Beeps.Clear();

            StopBeeps();

            MySettingsWindow.DialogResult = false;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // TODO: use a SetValues method that accepts a Dictionary of values

            DataFile VM;
            if (MySettingsWindow.MainWindow != null)
            {
                VM = MyDataFile;
            }
            else
            {
                throw new NotImplementedException();
            }

            SaveEditedValuesToDb();

            Properties.Settings.Default.AutoOpenLastFile = (CbAutoOpenLastFile.SelectedItem as ComboBoxItem).Content as string;
            Properties.Settings.Default.Save();

            MyDataFile.Beeps.Clear();
            foreach (Beep b in DgBeeps.Beeps)
            {
                MyDataFile.Beeps.Add(b);
            }
            DgBeeps.Beeps.Clear();

            StopBeeps();

            Utils.SetStartProgramWithWindows((bool)CbStartProgramWithWindows.IsChecked);

            MySettingsWindow.DialogResult = true;
        }

        internal void LoadValuesToBeEditedFromDb()
        {
            // load edited values from db
            foreach (SettingDataVM vm in MyItemsControl.ItemsSource)
            {
                vm.EditedValue = vm.M.Value;
            }
        }

        private void SaveEditedValuesToDb()
        {
            // save editors' values to db
            foreach (SettingDataVM vm in MyItemsControl.ItemsSource)
            {
                vm.M.Value = vm.EditedValue;
            }
        }

        private void LoadDefaultValuesToBeEditedFromDb()
        {
            // load edited values from default values in db
            foreach (SettingDataVM vm in MyItemsControl.ItemsSource)
            {
                if (vm.M.DefaultValueComputation == null)
                {
                    vm.EditedValue = vm.M.DefaultValue;
                }
                else
                {
                    vm.EditedValue = vm.M.DefaultValueComputation.Compute(vm.M);
                }
            }
        }

        // TODO: new class BeepsPlayer with code below:

        internal BitmapImage PlayIcon = new BitmapImage(
            new Uri("/Resources/Oxygen-Icons.org-Oxygen-Actions-media-playback-start.ico",
            UriKind.Relative));
        internal BitmapImage StopIcon = new BitmapImage(
            new Uri("/Resources/Oxygen-Icons.org-Oxygen-Actions-media-playback-stop.ico",
            UriKind.Relative));

        internal BeepTimerCollection CurrentlyPlayingBeeps = null;

        private void BtnPlayStopBeeps_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentlyPlayingBeeps != null/* && CurrentlyPlayingBeeps.IsPlaying*/)
            {
                StopBeeps();
                return;
            }

            CurrentlyPlayingBeeps = DgBeeps.Beeps.CreateBeepTimerCollection(null, true);
            CurrentlyPlayingBeeps.DoneRinging += CurrentlyPlayingBeeps_DoneRinging;
            TbBeepsPlayerBtnText.Text = "Stop beeps";
            ImgBeepsPlayer.Source = StopIcon;
        }

        private void StopBeeps()
        {
            if (CurrentlyPlayingBeeps == null)
            {
                return;
            }

            CurrentlyPlayingBeeps.Delete();
            CurrentlyPlayingBeeps = null;
            TbBeepsPlayerBtnText.Text = "Play beeps";
            ImgBeepsPlayer.Source = PlayIcon;
        }

        private void CurrentlyPlayingBeeps_DoneRinging(object sender, EventArgs e)
        {
            CurrentlyPlayingBeeps = null;
            TbBeepsPlayerBtnText.Text = "Play beeps";
            ImgBeepsPlayer.Source = PlayIcon;
        }

        private void BtnResetToDefaults_Click(object sender, RoutedEventArgs e)
        {
            MyAudioFileSelector.AudioPlayer.StopSound();
            MyAudioFileSelector.SetAudioPath("");

            LoadDefaultValuesToBeEditedFromDb();

            CbAutoOpenLastFile.Text = "Yes";
            CbStartProgramWithWindows.IsChecked = false;

            DgBeeps.Beeps.Clear();
        }
    }
}
