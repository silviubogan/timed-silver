using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Interaction logic for ClockDataGrid.xaml
    /// </summary>
    public partial class ClockDataGrid : UserControl, IClocksView, IZoomableControl
    {
        public ObservableCollection<ClockType> ClockTypes { get; set; }

        public ClockDataGrid()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ImportModeProperty =
            DependencyProperty.Register("ImportMode", typeof(bool),
                typeof(ClockDataGrid), new PropertyMetadata(false, OnImportModeChanged));

        private static void OnImportModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var o = d as ClockDataGrid;
            o.ImportModeChangedHandler();
        }

        private void ImportModeChangedHandler()
        {
            if (!IsReallyLoaded)
            {
                return;
            }

            if (ImportMode)
            {
                foreach (DataGridColumn c in MyDataGrid.Columns)
                {
                    if (c.Header.ToString() == "")
                    {
                        c.Visibility = Visibility.Collapsed;
                        break;
                    }
                }
            }
            else
            {
                MyFillColumnsItem = new MenuItem()
                {
                    Header = "_Autoresize columns to fill the table width"
                };
                MyFillColumnsItem.Icon = Properties.Resources.fill_columns_icon;
                MyFillColumnsItem.Checked += MyFillColumnsItem_Checked;
                MyFillColumnsItem.Unchecked += MyFillColumnsItem_Unchecked;
                MyFillColumnsItem.IsCheckable = true;
                MyFillColumnsItem.IsChecked = false;

                MyToolStripSeparator = new Separator();
            }
        }

        private void MyFillColumnsItem_Unchecked(object sender, RoutedEventArgs e)
        {
            // resize columns to fill table width
            Cms_ResizeColumnsToFill(sender, e);
        }

        internal void MyFillColumnsItem_Checked(object sender, EventArgs e)
        {
            // resize columns to fill table width
            Cms_ResizeColumnsToFill(sender, e);
        }

        internal void Cms_ResizeColumnsToFill(object sender, EventArgs e)
        {
            if (Clocks != null)
            {
                Clocks.MyDataFile.Settings.SetValue("AutoresizeTableColumns", MyFillColumnsItem.IsChecked);
            }
        }

        public bool ImportMode
        {
            get
            {
                return (bool)GetValue(ImportModeProperty);
            }
            set
            {
                SetValue(ImportModeProperty, value);
            }
        }

        public static readonly DependencyProperty DarkThemeProperty =
            DependencyProperty.Register("DarkTheme", typeof(bool),
                typeof(ClockDataGrid), new PropertyMetadata(false, OnDarkThemeChanged));

        private static void OnDarkThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var o = d as ClockDataGrid;
            if ((bool)e.NewValue)
            {
                //o.MyDataGrid.Style = (Style)o.FindResource("MyDarkThemeStyle");
            }
            else
            {
                //o.MyDataGrid.Style = (Style)o.FindResource("MyLightThemeStyle");
            }
        }

        public bool DarkTheme
        {
            get
            {
                return (bool)GetValue(DarkThemeProperty);
            }
            set
            {
                SetValue(DarkThemeProperty, value);
            }
        }

        public static readonly DependencyProperty ClocksProperty =
            DependencyProperty.Register("Clocks", typeof(ClockVMCollection),
                typeof(ClockDataGrid), new PropertyMetadata(null, OnClocksChanged));
        public ClockVMCollection Clocks
        {
            get
            {
                return (ClockVMCollection)GetValue(ClocksProperty);
            }
            set
            {
                SetValue(ClocksProperty, value);
            }
        }

        public static readonly DependencyProperty AutoresizeTableColumnsProperty =
            DependencyProperty.Register("AutoresizeTableColumns", typeof(bool), typeof(ClockDataGrid), new FrameworkPropertyMetadata(false,
            OnAutoresizeTableColumnsChanged));
        public bool AutoresizeTableColumns
        {
            get
            {
                return (bool)GetValue(AutoresizeTableColumnsProperty);
            }
            set
            {
                SetValue(AutoresizeTableColumnsProperty, value);
            }
        }

        private static void OnAutoresizeTableColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ClockDataGrid).HandleAutoresizeTableColumnsChanged();
        }

        private void HandleAutoresizeTableColumnsChanged()
        {
            if (!IsReallyLoaded)
            {
                return;
            }

            foreach (DataGridColumn column in MyDataGrid.Columns)
            {
                if (column.Header == null) // in design mode
                {
                    continue;
                }

                if (column.Header.ToString() == "Tag")
                {
                    if (AutoresizeTableColumns)
                    {
                        column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                    }
                    else
                    {
                        column.Width = new DataGridLength(1, DataGridLengthUnitType.Auto);
                    }
                }
                else
                {
                }
            }

            if (MyFillColumnsItem != null) // not in design mode
            {
                MyFillColumnsItem.IsChecked = AutoresizeTableColumns;
            }
        }


        private static void OnClocksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var o = d as ClockDataGrid;

            if (e.NewValue != null)
            {
                o.MyClockContextMenuStrip = new ClockContextMenuStrip(); // TODO: is this necessary? check for null and only then do this.

                var hs = new HashSet<ClockVM>(o.Clocks.VMs);
                o.MyClockContextMenuStrip.MyClocks = hs;

                o.MyClockContextMenuStrip.UpdateContents();
            }
        }

        /// <summary>
        /// 1. adjust the selection like on right click on an item
        /// 1.5. make all the selected items be Checked
        /// 1.6. make all the unselected items be !Checked
        /// 2. show the context menu for the Checked items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyDataGrid_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Utils.WPFDoRightMouseButtonSelect(MyDataGrid, e);

            ShowContextMenu(Utils.GetMousePositionWindowsForms(), System.Windows.Forms.ToolStripDropDownDirection.BelowRight);
        }

        internal Separator MyToolStripSeparator;
        internal MenuItem MyFillColumnsItem;
        internal ClockContextMenuStrip MyClockContextMenuStrip;
        internal void ShowContextMenu(System.Drawing.Point screenPosition,
            System.Windows.Forms.ToolStripDropDownDirection dir = System.Windows.Forms.ToolStripDropDownDirection.BelowRight)
        {
            var cc = new HashSet<ClockVM>();
            foreach (ClockVM cd in Clocks.VMs)
            {
                if (cd.Checked)
                {
                    cc.Add(cd);
                }
            }
            if (cc.Count == 0) // only takes place in multiselect mode (header)
            {
                var cms = new ContextMenu();
                cms.Items.Add(MyFillColumnsItem);
                //cms.Show(screenPosition, dir);
                cms.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                cms.IsOpen = true;
                return;
            }

            if (ImportMode)
            {
                return;
            }

            MyClockContextMenuStrip.MyClocks = cc;
            MyClockContextMenuStrip.UpdateContents();
            MyClockContextMenuStrip.Items.Remove(MyToolStripSeparator);
            MyClockContextMenuStrip.Items.Remove(MyFillColumnsItem);
            MyClockContextMenuStrip.Items.Add(MyToolStripSeparator);
            MyClockContextMenuStrip.Items.Add(MyFillColumnsItem);
            MyClockContextMenuStrip.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
            //MyClockContextMenuStrip.Show(screenPosition, dir);
            MyClockContextMenuStrip.IsOpen = true;
        }

        private void MyDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            foreach (DataGridCellInfo info in e.RemovedCells)
            {
                if (!info.IsValid)
                {
                    continue;
                }

                (info.Item as ClockVM).Checked = false;
            }

            foreach (DataGridCellInfo info in e.AddedCells)
            {
                if (!info.IsValid)
                {
                    continue;
                }

                (info.Item as ClockVM).Checked = true;
            }

            // TODO: in future: autofocus new row
            // if wanted, using Select method, but
            // take into account the multiselect
            // feature.
        }

        private void ComboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var cb = sender as ComboBox;

            if ((e.Key == Key.Return ||
                e.Key == Key.Enter) &&
                cb.Text != "")
            {
                bool duplicate = false;
                foreach (ClockGroupVM vm in Clocks.Model.GroupsVM.VMs)
                {
                    if (vm.Name == cb.Text)
                    {
                        cb.SelectedItem = vm;
                        duplicate = true;
                        break;
                    }
                }

                if (duplicate)
                {
                    return;
                }

                // create a ClockGroupM and corresponding ClockGroupVM
                // (ClockGroupVM inherits from ClockGroupM)
                var cvm = new ClockGroupVM(new ClockGroupM()
                {
                    Name = cb.Text
                });
                Clocks.Model.GroupsVM.VMs.Insert(0, cvm);
                cb.SelectedItem = cvm;
            }
        }

        internal bool HandlingSelectionChange = false;
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HandlingSelectionChange)
            {
                return;
            }
            HandlingSelectionChange = true;

            var cb = sender as ComboBox;

            //if (cb.SelectedItem is the VM with Style != Normal)

            ClockGroupVM foundVM = null;
            foreach (ClockGroupVM vm in Clocks.Model.GroupsVM.VMs)
            {
                if (vm.FontStyle != FontStyles.Normal &&
                    ReferenceEquals(cb.SelectedItem, vm))
                {
                    foundVM = vm;
                    break;
                }
            }

            if (foundVM != null)
            {
                cb.Text = "";
                cb.SelectedValue = null;
                e.Handled = true;
            }

            HandlingSelectionChange = false;
        }

        private void ColorPicker_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space ||
                e.Key == Key.Enter ||
                e.Key == Key.Return)
            {
                (sender as Xceed.Wpf.Toolkit.ColorPicker).IsOpen = true;
                e.Handled = true;
            }
        }

        public static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register("ZoomFactor", typeof(decimal), typeof(ClockDataGrid),
                new PropertyMetadata(1M, OnZoomFactorChanged));

        public decimal ZoomFactor
        {
            get
            {
                return (decimal)GetValue(ZoomFactorProperty);
            }
            set
            {
                SetValue(ZoomFactorProperty, value);
            }
        }

        private static void OnZoomFactorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var o = d as ClockDataGrid;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        internal bool IsReallyLoaded = false;

        private void MyClockDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            IsReallyLoaded = true;

            ClockTypes = new ObservableCollection<ClockType>();
            ClockTypes.Add(new ClockType()
            {
                Name = "Timer",
                ImagePath = "/Resources/timers filter (clepsidra 4).ico"
            });
            ClockTypes.Add(new ClockType()
            {
                Name = "Alarm",
                ImagePath = "/Resources/alarms filter (alarm clock 5).ico"
            });

            DataContext = this;

            ImportModeChangedHandler();

            HandleAutoresizeTableColumnsChanged();

            ScrollChanged += MyScrollViewer_ScrollChanged;
        }

        public event ScrollChangedEventHandler ScrollChanged;

        private void MyScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollChanged?.Invoke(sender, e);
        }

        private void MyUserControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) ||
                Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Delta < 0)
                {
                    ZoomFactor -= 0.1M;
                }
                else
                {
                    ZoomFactor += 0.1M;
                }

                e.Handled = true;
            }
        }
    }

    // TODO: move to its own file:
    public class ClockType : BindableBase
    {
        internal string _Name = "";
        public string Name
        {
            get { return _Name; }
            set { SetProperty(ref _Name, value); }
        }

        internal string _ImagePath = "";
        public string ImagePath
        {
            get { return _ImagePath; }
            set { SetProperty(ref _ImagePath, value); }
        }
    }
}
