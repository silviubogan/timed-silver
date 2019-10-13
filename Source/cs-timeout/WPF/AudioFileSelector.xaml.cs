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
using Xceed.Wpf.Toolkit;

namespace cs_timed_silver
{
    /// <summary>
    /// Interaction logic for AudioFileSelector.xaml
    /// </summary>
    public partial class AudioFileSelector : ContentControl
    {
        static AudioFileSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AudioFileSelector),
                new FrameworkPropertyMetadata(typeof(AudioFileSelector)));
        }



        public RecentAudioFilesContextMenu RecentAudioFilesContextMenu
        {
            get { return (RecentAudioFilesContextMenu)GetValue(RecentAudioFilesContextMenuProperty); }
            set { SetValue(RecentAudioFilesContextMenuProperty, value); }
        }
        public static readonly DependencyProperty RecentAudioFilesContextMenuProperty =
            DependencyProperty.Register("RecentAudioFilesContextMenu", typeof(RecentAudioFilesContextMenu), typeof(AudioFileSelector), new PropertyMetadata(null));



        // TODO: [VISUAL] notify icon context menu should optionally show more information.
        public AudioFileSelector()
        {
            InitializeComponent();

            MainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
        }

        internal MainWindow MainWindow = null;
        internal NAudioPlayer AudioPlayer = null;

        private void MyDataFile_FilePathChanged(object sender, EventArgs e)
        {
            Initialize();
        }

        internal void Initialize()
        {
            LoadAudioPlayer();
            MainWindow.MySettingsWindow.MyControl.LoadAudioFilePathFromSetting();
            if (AudioPlayer != null)
            {
                AudioPlayer.AudioFilePath = FilePath;
                MainWindow.VM.MultiAudioPlayer.AudioFilePath = AudioPlayer.AudioFilePath;
            }
        }

        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register("FilePath", typeof(string), typeof(AudioFileSelector),
                new FrameworkPropertyMetadata("", OnFilePathChanged));
        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }
        private static void OnFilePathChanged(object d, DependencyPropertyChangedEventArgs e)
        {
            var o = d as AudioFileSelector;

            o.PreviousPath = (string)e.OldValue;

            o.ApplyTemplate();

            if (o.AudioPlayer != null)
            {
                o.AudioPlayer.AudioFilePath = (string)e.NewValue;
            }
            o.FilePathChanged?.Invoke(o, new FilePathEventArgs()
            {
                NewFilePath = (string)e.NewValue
            });
        }

        internal string _PreviousPath = "";
        internal string PreviousPath
        {
            get
            {
                return _PreviousPath;
            }
            set
            {
                if (_PreviousPath != value)
                {
                    _PreviousPath = value;
                }
            }
        }

        internal event EventHandler<FilePathEventArgs> FilePathChanged;

        private void LoadAudioPlayer()
        {
            if (MainWindow.VM == null)
            {
                return;
            }
            AudioPlayer = new NAudioPlayer(MainWindow.VM);
        }

        // TODO: [VISUAL] List view - double click on ListBox item and it
        // shows as a new window (?), or in the right as a sidebar.
        private void UpdateLabel()
        {
            string path = FilePath;

            BtnReset.Visibility = path != "" ? Visibility.Visible : Visibility.Collapsed;
            //Application.DoEvents(); // next access to btnReset below will result in the correct expected value: [update: this still doesnt work, maybe use VisibleChanged event?..... some `await` syntax? maybe not (the await syntax).]
            LblAudio.Text = path != "" ? // btnReset.Visible ?
                path : "Default sound";
        }

        internal Button BtnReset;
        internal Button BtnPlayPause;
        internal TextBlock LblAudio;
        internal Image ImgPlayPause;
        internal DropOverlay MyDropOverlay;
        internal SplitButton BtnSetAudio;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetStartStopButtonStartIcon();
            if (MainWindow == null || MainWindow.VM == null)
            {
                return;
            }
            MainWindow.VM.FilePathChanged += MyDataFile_FilePathChanged;
            Initialize();
        }

        internal void SetStartStopButtonStartIcon()
        {
            if (ImgPlayPause == null)
            {
                return;
            }

            // TODO: [VISUAL] tooltip adapted to button state (normal tooltip with normal font, "Click NOW to [...]" with bold above the normal text, in the same tooltip. Needed: rich text tooltip.
            ImgPlayPause.Source = new BitmapImage(new Uri("/Resources/Oxygen-Icons.org-Oxygen-Actions-media-playback-start.ico", UriKind.Relative));
        }

        internal void SetStartStopButtonPauseIcon()
        {
            // TODO: [VISUAL] tooltip adapted to button state
            ImgPlayPause.Source = new BitmapImage(new Uri("/Resources/Oxygen-Icons.org-Oxygen-Actions-media-playback-pause.ico", UriKind.Relative));
        }

        internal bool SetAudioPath(string path)
        {
            if (MainWindow == null)
            {
                throw new ArgumentNullException("MainWindow", "Expected exception #1.");
            }

            //// daca noua cale ar fi aceeasi
            //if (path == PreviousPath)
            //{
            //    FilePath = path;
            //    return true;
            //}

            // si fa calea curenta cea noua
            //if (!MainForm.ApplyWithoutSetting)
            //{
            FilePath = path;
            //}

            // if the audio player is loaded
            if (AudioPlayer != null)
            {
                // if the audio is playing
                if (AudioPlayer.IsPlaying)
                {
                    // stop and update icon
                    AudioPlayer.StopSound();
                    SetStartStopButtonStartIcon();
                }
                // if the selected audio path is invalid file
                if (!AudioPlayer.Test())
                {
                    ShowErrorNotification();

                    // if setting should be applied, not set
                    //if (MainForm.ApplyWithoutSetting)
                    //{
                    //    // set previous path to current
                    //    //PreviousPath = path;
                    //}
                    //else
                    //{
                    //    // set current path to previous path, without changing the previous path
                    FilePath = PreviousPath;
                    //}

                    UpdateLabel();
                    return false;
                }
            }

            // update the label to show the current audio file path
            UpdateLabel();
            return true;
        }

        internal void ShowErrorNotification()
        {
            MainWindow.MyStatusBar.PostMessage("Error when loading the selected file. Please choose a correct audio file.", LogCategory.Error);
        }

        private void BtnSetAudio_Click(object sender, RoutedEventArgs e)
        {
            // ask the user for an audio file
            System.Windows.Forms.OpenFileDialog ofd = Utils.GetAudioChooserDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SetAudioPathAndAddItToRecentsMenu(ofd.FileName);
            }
        }

        internal void SetAudioPathAndAddItToRecentsMenu(string fp)
        {
            // try to set it as the selected audio file
            if (SetAudioPath(fp))
            {
                // if success, put it in the recent files menu
                RecentAudioFilesContextMenu.InsertInRecentFilesList(fp);
            }
        }

        private void BtnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            // if sound is playing stop sound and change icon according to this
            if (AudioPlayer.IsPlaying)
            {
                AudioPlayer.StopSound();
                SetStartStopButtonStartIcon();
            }
            else
            {
                // if sound's not playing, test if it can not play
                if (!AudioPlayer.Test())
                {
                    // TODO: [VISUAL] put the error file path in the notification. [ better a status bar expandable to a Log panel ].
                    ShowErrorNotification();

                    // set selected sound to default sound
                    SetAudioPath("");
                    return;
                }

                // change icon & play the sound
                SetStartStopButtonPauseIcon();
                AudioPlayer.PlaySound();
            }
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            ResetAudioPath();
        }

        internal void ResetAudioPath()
        {
            // set sound to default sound
            SetAudioPath("");

            // if the sound is playing, stop and change icon to start icon
            if (AudioPlayer.IsPlaying)
            {
                SetStartStopButtonStartIcon();
                AudioPlayer.StopSound();
            }
        }

        private void BtnSetAudio_Opened(object sender, RoutedEventArgs e)
        {
            RecentAudioFilesContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
            RecentAudioFilesContextMenu.IsOpen = true;
            //e.Handled = true; ?
        }

        protected override void OnTemplateChanged(ControlTemplate oldTemplate, ControlTemplate newTemplate)
        {
            base.OnTemplateChanged(oldTemplate, newTemplate);

            ApplyTemplate();

            if (oldTemplate != null)
            {
                BtnSetAudio.Click -= BtnSetAudio_Click;
                BtnSetAudio.Opened -= BtnSetAudio_Opened;

                BtnPlayPause.Click -= BtnPlayPause_Click;

                BtnReset.Click -= BtnReset_Click;
            }

            if (newTemplate != null)
            {
                BtnReset = (Button)GetTemplateChild("BtnReset");
                BtnPlayPause = (Button)GetTemplateChild("BtnPlayPause");
                LblAudio = (TextBlock)GetTemplateChild("LblAudio");
                ImgPlayPause = (Image)GetTemplateChild("ImgPlayPause");
                MyDropOverlay = (DropOverlay)GetTemplateChild("MyDropOverlay");
                BtnSetAudio = (SplitButton)GetTemplateChild("BtnSetAudio");

                BtnSetAudio.Click += BtnSetAudio_Click;
                BtnSetAudio.Opened += BtnSetAudio_Opened;

                BtnPlayPause.Click += BtnPlayPause_Click;

                BtnReset.Click += BtnReset_Click;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();


        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            ApplyTemplate();
        }

        private void UserControl_PreviewDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Link;

                var paths = e.Data.
                    GetData(DataFormats.FileDrop) as string[];

                string path = paths[0];

                bool knownExtension = false;
                foreach (string ext in Utils.SupportedAudioFileExtensions())
                {
                    if (path.EndsWith($".{ext}"))
                    {
                        knownExtension = true;
                        break;
                    }
                }

                if (!knownExtension)
                {
                    MyDropOverlay.ShowWarning = true;
                }
                else
                {
                    MyDropOverlay.ShowWarning = false;
                }

                DraggingOver = true;
            }
            //else
            //{
            //    //drgevent.Effect = DragDropEffects.None;
            //    //DraggingOver = false; // TODO: [VISUAL] show anyway a message that the dragged format is not supported.
            //}
        }

        private void UserControl_PreviewDragLeave(object sender, DragEventArgs e)
        {
            DraggingOver = false;
        }

        private void UserControl_PreviewDrop(object sender, DragEventArgs e)
        {
            if ((e.Effects & DragDropEffects.Link) == DragDropEffects.Link)
            {
                var paths = e.Data.
                    GetData(DataFormats.FileDrop) as string[];
                SetAudioPathAndAddItToRecentsMenu(paths[0]);
            }
            DraggingOver = false;
        }

        internal bool _DraggingOver = false;
        internal bool DraggingOver
        {
            get
            {
                return _DraggingOver;
            }
            set
            {
                if (_DraggingOver != value)
                {
                    _DraggingOver = value;

                    UpdateDropIndicator();
                }
            }
        }

        internal void UpdateDropIndicator()
        {
            if (DraggingOver)
            {
                MyDropOverlay.FadeIn();
            }
            else
            {
                MyDropOverlay.FadeOut();
            }
        }
    }
}
