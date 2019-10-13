using System;
using System.Drawing;
using System.Windows.Forms;

namespace cs_timed_silver
{
    public partial class ImageFileChooser : UserControl
    {
        internal MainForm _MainForm = null;
        internal MainForm MainForm
        {
            get
            {
                return _MainForm;
            }
            set
            {
                if (_MainForm != value)
                {
                    _MainForm = value;
                    OnMainFormChanged();
                }
            }
        }

        private void OnMainFormChanged()
        {
            if (MainForm != null)
            {
                recentFilesMenuStrip1.MainForm = MainForm;
                MainForm.MyDataFile.FilePathChanged += MyDataFile_FilePathChanged;
            }
        }

        private void MyDataFile_FilePathChanged(object sender, EventArgs e)
        {
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

        internal OverlayControl MyOverlay;

        private void UpdateDropIndicator()
        {
            if (DraggingOver)
            {
                MyOverlay.Background = new Bitmap(Width, Height);
                DrawToBitmap(MyOverlay.Background, ClientRectangle);

                Rectangle visibleRect = Utils.GetControlVisibleRectangle(this);
                var cropRect = new Rectangle(
                    PointToClient(visibleRect.Location),
                    visibleRect.Size);

                MyOverlay.Background = Utils.CropImage(MyOverlay.Background, cropRect);
                MyOverlay.BringToFront();
                MyOverlay.Visible = true;
            }
            else
            {
                MyOverlay.Visible = false;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (MyOverlay != null)
            {
                MyOverlay.Size = Size;
            }
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            base.OnDragEnter(drgevent);

            if (drgevent.Data.GetDataPresent(DataFormats.FileDrop))
            {
                drgevent.Effect = DragDropEffects.Link;
                
                var paths = drgevent.Data.
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
                    MyOverlay.DropHereText = "Unknown file extension";
                    MyOverlay.MyImage = SystemIcons.Warning.ToBitmap();
                }
                else
                {
                    MyOverlay.ResetDropHereText();
                    MyOverlay.ResetDropHereImage();
                }
                DraggingOver = true;
            }
            else
            {
                drgevent.Effect = DragDropEffects.None;
                DraggingOver = false;
            }
        }

        protected override void OnDragLeave(EventArgs e)
        {
            base.OnDragLeave(e);

            DraggingOver = false;
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);

            if (drgevent.Effect == DragDropEffects.Link)
            {
                var paths = drgevent.Data.
                    GetData(DataFormats.FileDrop) as string[];
                SetImagePathAndAddItToRecentsMenu(paths[0]);
            }
            DraggingOver = false;

            MyOverlay.ResetDropHereText();
            MyOverlay.ResetDropHereImage();
        }

        internal string _FilePath = "";
        internal string FilePath
        {
            get
            {
                return _FilePath;
            }
            set
            {
                if (_FilePath != value)
                {
                    PreviousPath = _FilePath;
                    _FilePath = value;

                    if (value == "")
                    {
                        pictureBox1.Visible = false;

                        UpdateLabel();
                    }
                    else
                    {
                        try
                        {
                            pictureBox1.ImageLocation = value;
                            pictureBox1.Visible = true;

                            UpdateLabel();

                            FilePathChanged?.Invoke(this, new FilePathEventArgs()
                            {
                                NewFilePath = value
                            });
                        }
                        catch (Exception /*ex*/)
                        {
                            pictureBox1.Visible = false;
                            _FilePath = "";

                            ShowErrorNotification();

                            UpdateLabel();
                        }
                    }
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

        #region Copied from AudioFileChooser class, and commented out.

        //protected bool _DarkMode = false;
        //public bool DarkMode
        //{
        //    get
        //    {
        //        return _DarkMode;
        //    }
        //    set
        //    {
        //        //if (!value && value != _DarkMode) // "!value &&" disables dark theme (AudioFileChooser is used only in SettingsForm)
        //        //{
        //        //    _DarkMode = value;

        //        //    ForceUpdateDarkModeEnabled(value);
        //        //}
        //    }
        //}

        //private void ForceUpdateDarkModeEnabled(bool dark)
        //{
        //    lblPath.ForeBrush = dark ?
        //        new SolidBrush(Color.White) :
        //        new SolidBrush(Utils.MyDarkDarkGray);

        //    btnPlayPause.FlatStyle = dark ? FlatStyle.Flat :
        //        FlatStyle.Standard;
        //    btnPlayPause.BackColor = dark ? Utils.MyDarkGray :
        //        SystemColors.ButtonFace;
        //    btnPlayPause.FlatAppearance.BorderColor =
        //        Utils.MyDarkDarkGray;

        //    btnSetImageFile.FlatStyle = FlatStyle.Flat;
        //    btnSetImageFile.BackColor = Color.LightGray;
        //    btnSetImageFile.FlatAppearance.BorderColor =
        //        Utils.MyDarkDarkGray;

        //    btnRecentFilesMenu.FlatStyle = FlatStyle.Flat;
        //    btnRecentFilesMenu.BackColor = Color.LightGray;
        //    btnRecentFilesMenu.FlatAppearance.BorderColor =
        //        Utils.MyDarkDarkGray;

        //    btnReset.FlatStyle = dark ? FlatStyle.Flat :
        //        FlatStyle.Standard;
        //    btnReset.BackColor = dark ? Utils.MyDarkGray :
        //        SystemColors.ButtonFace;
        //    btnReset.FlatAppearance.BorderColor =
        //        Utils.MyDarkDarkGray;

        //    btnPlayPause.ForeColor = btnReset.ForeColor =
        //        dark ? Color.LightGray : Color.Black;
        //    btnRecentFilesMenu.ForeColor =
        //        btnSetImageFile.ForeColor = Color.Black;
        //}

        #endregion

        internal RecentFilesContextMenu recentFilesMenuStrip1;


        public ImageFileChooser()
        {
            InitializeComponent();

            MainWindow = System.Windows.Application.Current.MainWindow as MainWindow;

            if (MainWindow != null)
            {
                MainWindow.VM.FilePathChanged += MyDataFile_FilePathChanged;
            }

            btnRecentFilesMenu.Resize += BtnRecentFilesMenu_Resize;

            pictureBox1.Cursor = Cursors.Hand;
            pictureBox1.LoadCompleted += PictureBox1_LoadCompleted;
            pictureBox1.Click += PictureBox1_Click;

            recentFilesMenuStrip1 =
                new RecentFilesContextMenu("RecentImageFilePaths");
            recentFilesMenuStrip1.PathValidationRequested += RecentFilesMenuStrip1_PathValidationRequested;
            recentFilesMenuStrip1.EmptyLabel = "No recent image files.";

            toolTip1.SetToolTip(btnSetImageFile, "Click this button to open a dialog in which you can select an image file to use for all clocks.");
            toolTip1.SetToolTip(btnRecentFilesMenu, "Click this button to open a list of the most recent 10 used image files, where you can click one to use it for all clocks.");
            toolTip1.SetToolTip(lblPath, "This is the currently selected audio file (or the default \"No image\").");
            toolTip1.SetToolTip(pictureBox1, "Click here to open the selected image file.");
            toolTip1.SetToolTip(btnReset, "Click this button to reset to \"No image\".");
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            if (MainForm == null)
            {
                Utils.OpenUrlInDefaultApp(FilePath);
                return;
            }

            Utils.OpenUrlInDefaultApp(FilePath, MainForm);
        }

        private void PictureBox1_LoadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            //if (e.Error != null)
            //{
            //    FilePath = "";
            //    ShowErrorNotification();
            //}
        }

        private void RecentFilesMenuStrip1_PathValidationRequested(object sender, PathValidationEventArgs e)
        {
            e.Valid = SetImagePath(e.Path);
        }

        public ImageFileChooser(MainForm mf) : this()
        {
            MainForm = mf;
        }

        // TODO: [VISUAL] List view - double click on ListBox item and it
        // shows as a new window (?), or in the right as a sidebar.
        private void UpdateLabel()
        {
            string path = FilePath;

            btnReset.Visible = path != "";
            Application.DoEvents(); // next access to btnReset below will result in the correct expected value: [update: this still doesnt work, maybe use VisibleChanged event?..... some `await` syntax? maybe not (the await syntax).]
            lblPath.Text = path != "" ? // btnReset.Visible ?
                path : "No image.";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            LoadOverlay();
        }

        internal void LoadOverlay()
        {
            MyOverlay = new OverlayControl();
            MyOverlay.Visible = false;
            Controls.Add(MyOverlay);
            MyOverlay.Location = Point.Empty;
            MyOverlay.Size = Size;
        }

        private void BtnRecentFilesMenu_Resize(object sender, EventArgs e)
        {
            btnRecentFilesMenuBottomRightPoint = new Point(btnRecentFilesMenu.Size);
        }

        public MainWindow MainWindow { get; set; } = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns>true, if the loading of the image succeeded.</returns>
        internal bool SetImagePath(string path)
        {
            if (MainForm == null && MainWindow == null)
            {
                throw new ArgumentNullException("MainForm", "Expected exception #1.");
            }
            
            FilePath = path;
            
            return true;
        }

        internal void ShowErrorNotification()
        {
            if (MainForm != null)
            {
                MainForm.MyCustomStatusBar.PostMessage("Error when loading the selected file. Please choose a correct image file.", LogCategory.Error);
            }
            else
            {
                MainWindow.MyStatusBar.PostMessage("Error when loading the selected file. Please choose a correct image file.", LogCategory.Error);
            }
        }

        private void btnSetImageFile_Click(object sender, EventArgs e)
        {
            // ask the user for an audio file
            OpenFileDialog ofd = Utils.GetImageChooser();
            if (ofd.ShowDialog() == DialogResult.OK)
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
                recentFilesMenuStrip1.InsertInRecentFilesList(fp);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            // set image to default image (no image)
            SetImagePath("");
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        internal Point btnRecentFilesMenuBottomRightPoint = Point.Empty;

        private void btnRecentFilesMenu_Click(object sender, EventArgs e)
        {
            recentFilesMenuStrip1.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
            recentFilesMenuStrip1.IsOpen = true;
        }

        private void ImageFileChooser_Load(object sender, EventArgs e)
        {
            btnReset.Size = pictureBox1.Size = new Size(Height, Height);
            btnReset.BackgroundImage = Utils.IconResourceVersionBySize(Properties.Resources.Oxygen_Icons_org_Oxygen_Actions_edit_delete, 256);
        }
    }
}
