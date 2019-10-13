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
    /// Interaction logic for ImageFileSelector.xaml
    /// </summary>
    public partial class ImageFileSelector : Control
    {
        static ImageFileSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageFileSelector),
                new FrameworkPropertyMetadata(typeof(ImageFileSelector)));
        }



        public RecentFilesContextMenu RecentImageFilesContextMenu
        {
            get { return (RecentFilesContextMenu)GetValue(RecentImageFilesContextMenuProperty); }
            set { SetValue(RecentImageFilesContextMenuProperty, value); }
        }
        public static readonly DependencyProperty RecentImageFilesContextMenuProperty =
            DependencyProperty.Register("RecentImageFilesContextMenu", typeof(RecentFilesContextMenu), typeof(ImageFileSelector), new PropertyMetadata(null));
            

        public ImageFileSelector()
        {
            InitializeComponent();

            MainWindow = System.Windows.Application.Current.MainWindow as MainWindow;

            //toolTip1.SetToolTip(btnSetImageFile, "Click this button to open a dialog in which you can select an image file to use for all clocks.");
            //toolTip1.SetToolTip(btnRecentFilesMenu, "Click this button to open a list of the most recent 10 used image files, where you can click one to use it for all clocks.");
            //toolTip1.SetToolTip(lblPath, "This is the currently selected audio file (or the default \"No image\").");
            //toolTip1.SetToolTip(pictureBox1, "Click here to open the selected image file.");
            //toolTip1.SetToolTip(btnReset, "Click this button to reset to \"No image\".");
        }

        internal SplitButton BtnSetImageFile;
        internal TextBlock LblPath;
        internal Button BtnOpenFile,
            BtnReset;
        internal DropOverlay MyDropOverlay;
        internal Image ImgSelectedImage;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            BtnSetImageFile = (SplitButton)GetTemplateChild("BtnSetImageFile");
            LblPath = (TextBlock)GetTemplateChild("LblPath");
            BtnOpenFile = (Button)GetTemplateChild("BtnOpenFile");
            BtnReset = (Button)GetTemplateChild("BtnReset");
            MyDropOverlay = (DropOverlay)GetTemplateChild("MyDropOverlay");
            ImgSelectedImage = (Image)GetTemplateChild("ImgSelectedImage");

            BtnOpenFile.Click += BtnOpenFile_Click;

            BtnReset.Click += BtnReset_Click;

            BtnSetImageFile.Click += BtnSetImageFile_Click;
            BtnSetImageFile.Opened += BtnSetImageFile_Opened;

            BtnOpenFile.Visibility = Visibility.Collapsed;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            // set image to default image (no image)
            SetImagePath("");
        }

        public static readonly DependencyProperty FilePathProperty =
           DependencyProperty.Register("FilePath", typeof(string), typeof(ImageFileSelector),
               new FrameworkPropertyMetadata("", OnFilePathChanged));
        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }
        private static void OnFilePathChanged(object d, DependencyPropertyChangedEventArgs e)
        {
            var o = d as ImageFileSelector;

            o.PreviousPath = (string)e.OldValue;

            var value = (string)e.NewValue;

            o.ApplyTemplate();

            if (string.IsNullOrWhiteSpace(value))
            {
                o.BtnOpenFile.Visibility = Visibility.Collapsed;

                o.UpdateLabel();
            }
            else
            {
                try
                {
                    o.ImgSelectedImage.Source = new BitmapImage(new Uri(value, UriKind.Absolute));

                    o.ImgSelectedImage.Visibility = Visibility.Visible;
                    o.BtnOpenFile.Visibility = Visibility.Visible;

                    o.UpdateLabel();

                    o.FilePathChanged?.Invoke(o, new FilePathEventArgs()
                    {
                        NewFilePath = value
                    });
                }
                catch (Exception /*ex*/)
                {
                    o.ImgSelectedImage.Visibility = Visibility.Collapsed;
                    o.BtnOpenFile.Visibility = Visibility.Collapsed;

                    o.FilePath = "";

                    o.ShowErrorNotification();

                    o.UpdateLabel();
                }
            }
        }

        internal event EventHandler<FilePathEventArgs> FilePathChanged;

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

        private void BtnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            // TODO: set owner to MainWindow:
            Utils.OpenUrlInDefaultApp(FilePath);
        }

        // TODO: [VISUAL] List view - double click on ListBox item and it
        // shows as a new window (?), or in the right as a sidebar.
        private void UpdateLabel()
        {
            string path = FilePath;

            BtnReset.Visibility = path != "" ? Visibility.Visible : Visibility.Collapsed;
            //Application.DoEvents(); // next access to btnReset below will result in the correct expected value: [update: this still doesnt work, maybe use VisibleChanged event?..... some `await` syntax? maybe not (the await syntax).]
            LblPath.Text = path != "" ? // btnReset.Visible ?
                path : "No image.";
        }

        public MainWindow MainWindow { get; set; } = null;

        internal bool SetImagePath(string path)
        {
            if (MainWindow == null)
            {
                throw new ArgumentNullException("MainWindow", "Expected exception #1.");
            }

            FilePath = path;

            return true;
        }

        internal void ShowErrorNotification()
        {
            MainWindow.MyStatusBar.PostMessage("Error when loading the selected file. Please choose a correct image file.", LogCategory.Error);
        }

        private void BtnSetImageFile_Click(object sender, RoutedEventArgs e)
        {
            // ask the user for an audio file
            System.Windows.Forms.OpenFileDialog ofd = Utils.GetImageChooser();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SetImagePathAndAddItToRecentsMenu(ofd.FileName);
            }
        }

        internal void SetImagePathAndAddItToRecentsMenu(string fp)
        {
            // try to set it as the selected image file
            if (SetImagePath(fp))
            {
                // if success, put it in the recent files menu
                RecentImageFilesContextMenu.InsertInRecentFilesList(fp);
            }
        }

        private void BtnSetImageFile_Opened(object sender, RoutedEventArgs e)
        {
            RecentImageFilesContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
            RecentImageFilesContextMenu.IsOpen = true;
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
                foreach (string ext in Utils.SupportedImageFileExtensions())
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
                SetImagePathAndAddItToRecentsMenu(paths[0]);
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
