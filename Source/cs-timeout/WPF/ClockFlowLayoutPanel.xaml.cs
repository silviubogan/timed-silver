using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace cs_timed_silver
{
    /// <summary>
    /// Interaction logic for ClockFlowLayoutPanel.xaml
    /// </summary>
    public partial class ClockFlowLayoutPanel : UserControl, IClocksView, IZoomableControl
    {
        public static readonly DependencyProperty MyClocksProperty =
            DependencyProperty.Register("MyClocks", typeof(ClockVMCollection),
                typeof(ClockFlowLayoutPanel), new FrameworkPropertyMetadata(null));
        public ClockVMCollection MyClocks
        {
            get
            {
                return (ClockVMCollection)GetValue(MyClocksProperty);
            }
            set
            {
                SetValue(MyClocksProperty, value);
            }
        }


        public ClockVMCollection MyDesignTimeClocks;

        public ClockFlowLayoutPanel()
        {
            InitializeComponent();

            //MyClocks.Add(new TimerData(df, map)
            //{
            //    Tag = "1"
            //});

            //MyClocks.Add(new AlarmData(df, map)
            //{
            //    Tag = "2"
            //});

            //MyClocks.Add(new ClockData());

            //MyDesignTimeClocks = new ClockDataCollection(df);
            //MyDesignTimeClocks.Add(new AlarmData(df, map)
            //{
            //    Tag = "2"
            //});
        }

        public static readonly DependencyProperty DataFileProperty =
            DependencyProperty.Register("DataFile", typeof(DataFile), typeof(ClockFlowLayoutPanel),
                new FrameworkPropertyMetadata(null, OnDataFileChanged));

        private static void OnDataFileChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
            }
        }

        public DataFile DataFile
        {
            get
            {
                return (DataFile)GetValue(DataFileProperty);
            }
            set
            {
                SetValue(DataFileProperty, value);
            }
        }

        public static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register("ZoomFactor", typeof(decimal), typeof(ClockFlowLayoutPanel),
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
            var o = d as ClockFlowLayoutPanel;
        }

        public static readonly DependencyProperty RoundedCornersProperty =
            DependencyProperty.Register("RoundedCorners", typeof(bool), typeof(ClockFlowLayoutPanel),
                new PropertyMetadata(true, OnRoundedCornersChanged));

        public bool RoundedCorners
        {
            get
            {
                return (bool)GetValue(RoundedCornersProperty);
            }
            set
            {
                SetValue(RoundedCornersProperty, value);
            }
        }

        private static void OnRoundedCornersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var o = d as ClockFlowLayoutPanel;

            if ((bool)e.NewValue)
            {

            }
        }

        public static readonly DependencyProperty MultiSelectClocksProperty =
            DependencyProperty.Register("MultiSelectClocks", typeof(bool), typeof(ClockFlowLayoutPanel),
                new PropertyMetadata(false, OnMultiSelectClocksChanged));

        public bool MultiSelectClocks
        {
            get
            {
                return (bool)GetValue(MultiSelectClocksProperty);
            }
            set
            {
                SetValue(MultiSelectClocksProperty, value);
            }
        }

        private static void OnMultiSelectClocksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var o = d as ClockFlowLayoutPanel;
            var v = (bool)e.NewValue;

            if (v)
            {
                o.MySelectionHeader.ShowAnimated();
            }
            else
            {
                o.MySelectionHeader.HideAnimated();
            }

            foreach (ClockUserControl cc in Utils.GetVisualChildCollection<ClockUserControl>(o))
            {
                cc.Checkable = v;
            }
        }

        public static readonly DependencyProperty DarkThemeProperty =
            DependencyProperty.Register("DarkTheme", typeof(bool), typeof(ClockFlowLayoutPanel),
                new PropertyMetadata(false, OnDarkThemeChanged));
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
        private static void OnDarkThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var o = d as ClockFlowLayoutPanel;

            if ((bool)e.NewValue)
            {
                //o.MyUserControl.Style = (Style)o.FindResource("darkTheme");
                o.Background = Brushes.Black;
            }
            else
            {
                //o.MyUserControl.Style = (Style)o.FindResource("lightTheme");
                o.Background = Brushes.White;
            }
        }

        private void ClockUserControl_Drop(object sender, DragEventArgs e)
        {
            //MyDragTimer.Stop();
        }

        private void MyScrollViewer_Drop(object sender, DragEventArgs e)
        {
            //MyDragTimer.Stop();
        }

        private void ClockUserControl_MouseMove(object sender, MouseEventArgs e)
        {
        }

        private void ClockUserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //dragBoxFromMouseDown = Rect.Empty;
            //DraggedClocks.Clear();
        }

        bool firstLoad = true;

        private void MyUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (firstLoad)
            {
                firstLoad = false;

                //if (ReferenceEquals(DataFile, null)) // design time
                //{
                //    return;
                //}

                if (object.ReferenceEquals(DataFile, null))
                {
                    return;
                }

                MyClocks = DataFile.ClockVMCollection;

                //if (ReferenceEquals(MyClocks, null)) // design time
                //{
                //    return;
                //}

                DataContext = MyClocks;


                MyClockContextMenuStrip = new ClockContextMenuStrip();

                //if (ReferenceEquals(DataFile.Settings, null)) // design time
                //{
                //    return;
                //}

                if (object.ReferenceEquals(DataFile.Settings, null))
                {
                    return;
                }

                DarkTheme = (bool)DataFile.Settings.GetValue("DarkMode");

                // Otherwise, in ListViewDragDropManager, when I try to access the parent ScrollViewer of the MyItemsControl,
                // the MyItemsControl is not yet in the visual tree (has no parent yet) and so drag & drop does not work well.
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    //MyScrollViewer.Tag = new ListViewDragDropManager(MyItemsControl);

                    MySelectionHeader.MyClockListView = this;
                }), DispatcherPriority.Loaded);



                //MyItemsControl.ItemContainerGenerator.
            }
        }

        private void UserControl_DragLeave(object sender, DragEventArgs e)
        {
            //MyDragTimer.Stop();
        }

        private void MyScrollViewer_DragOver(object sender, DragEventArgs e)
        {
            //MyDragTimer.Start();
        }

        private void ClockUserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        internal ClockContextMenuStrip MyClockContextMenuStrip = null;
        internal void ShowContextMenu(HashSet<ClockVM> cc, System.Drawing.Point screenPosition,
            System.Windows.Forms.ToolStripDropDownDirection dir = System.Windows.Forms.ToolStripDropDownDirection.BelowRight)
        {
            if (cc.Count == 0) // only takes place in multiselect mode (header)
            {
                ClockContextMenuStrip.MyEmptyContextMenu.Show(screenPosition, dir);
                return;
            }

            MyClockContextMenuStrip.MyClocks = cc;
            MyClockContextMenuStrip.Show(screenPosition, dir);
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

        private void MyUserControl_Initialized(object sender, EventArgs e)
        {

        }
    }
}
