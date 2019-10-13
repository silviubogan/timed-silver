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
    /// Interaction logic for RecentFilesContextMenu.xaml
    /// </summary>
    public partial class RecentFilesMenuItem : MenuItem
    {
        public RecentFilesMenuItem(string pn = "")
        {
            InitializeComponent();

            MainWindow = System.Windows.Application.Current.MainWindow as MainWindow;

            StoragePropertyName = pn;

            SubmenuOpened += RecentFilesMenuStrip_Opening;
        }

        public RecentFilesMenuItem()
        {
            InitializeComponent();

            MainWindow = System.Windows.Application.Current.MainWindow as MainWindow;

            SubmenuOpened += RecentFilesMenuStrip_Opening;
        }

        internal List<string> RecentPaths = new List<string>();

        internal string StoragePropertyName { get; set; } = "";

        internal MainWindow _MainWindow = null;
        internal MainWindow MainWindow
        {
            get
            {
                return _MainWindow;
            }
            set
            {
                if (_MainWindow != value)
                {
                    _MainWindow = value;
                    OnMainFormChanged();
                }
            }
        }

        public string EmptyLabel { get; set; } = "No recent files.";

        private void OnMainFormChanged()
        {
        }

        internal event EventHandler<StringEventArgs> SettingSaveRequested;

        internal void LoadRecentPathsFromString(string paths)
        {
            if (!MainWindow.ApplyWithoutSetting)
            {
                if (StoragePropertyName != "")
                {
                    MainWindow.VM.Settings.SetValue(StoragePropertyName, paths);
                }
                else
                {
                    SettingSaveRequested?.Invoke(this, new StringEventArgs()
                    {
                        StringValue = paths
                    });
                }
            }
            RecentPaths = paths.Split(new char[] { '|' },
                StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        private void RecentFilesMenuStrip_Opening(object sender, RoutedEventArgs e)
        {
            Items.Clear();

            int count = RecentPaths.Count;

            int i = 1;
            int alignment = count.ToString().Length;
            foreach (string p in RecentPaths)
            {
                string s = Utils.BaseFileNameInPath(p);
                var item = new MenuItem()
                {
                    Header = string.Format("{0," + alignment + "}. {1}", i, s),
                    Template = (ControlTemplate)FindResource("LeafMenuItemTemplate")
                };
                item.ToolTip = p;
                Items.Add(item);
                item.Click += Item_Click;

                ++i;
            }

            if (RecentPaths.Count > 0)
            {
                var item = new MenuItem()
                {
                    Header = "Clear list",
                    Foreground = Brushes.Blue,
                    Template = (ControlTemplate)FindResource("LeafMenuItemTemplate")
                };
                item.FontWeight = FontWeights.Bold;

                item.Icon = new Image()
                {
                    Source = new BitmapImage(new Uri("/Resources/Oxygen-Icons.org-Oxygen-Actions-edit-delete.ico", UriKind.Relative))
                };

                Items.Add(item);
                item.Click += Item_Click;
            }
            else
            {
                var item = new MenuItem()
                {
                    Header = EmptyLabel,
                    IsEnabled = false,
                    Template = (ControlTemplate)FindResource("LeafMenuItemTemplate")
                };
                item.FontStyle = FontStyles.Italic;
                Items.Add(item);
            }

            e.Handled = true;
        }

        internal event EventHandler<PathValidationEventArgs> PathValidationRequested;

        /// <summary>
        /// Requires PathValidationRequested to be handled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Item_Click(object sender, EventArgs e)
        {
            var item = (MenuItem)sender;
            string txt = item.Header as string;

            if (txt == "Clear list")
            {
                RecentPaths.Clear();
                SaveRecentFilesList();
                return;
            }

            var ea = new PathValidationEventArgs()
            {
                Path = item.ToolTip as string
            };
            PathValidationRequested?.Invoke(this, ea);
            if (!ea.Valid)
            {
                RemovePath(item.ToolTip as string);
                MessageBox.Show(Application.Current.MainWindow,
                    $"Invalid file path removed from list: {item.ToolTip}", "Error");
            }
            else
            {
                InsertInRecentFilesList(item.ToolTip as string);
            }
        }

        internal void RemovePath(string p)
        {
            RecentPaths.Remove(p);
        }

        internal void SaveRecentFilesList()
        {
            if (StoragePropertyName == "")
            {
                SettingSaveRequested?.Invoke(this, new StringEventArgs()
                {
                    StringValue = GetAsString()
                });
            }
            else
            {
                MainWindow.VM.Settings.SetValue(StoragePropertyName,
                    GetAsString());
            }
        }

        internal string GetAsString()
        {
            return string.Join("|", RecentPaths);
        }

        internal void InsertInRecentFilesList(string filePath)
        {
            RecentPaths.RemoveAll(s => s == filePath);
            RecentPaths.Insert(0, filePath);
            ShrinkList();
            SaveRecentFilesList();
        }

        private void ShrinkList()
        {
            if (RecentPaths.Count > 10)
            {
                RecentPaths = RecentPaths.Take(10).ToList();
            }
        }
    }
}
