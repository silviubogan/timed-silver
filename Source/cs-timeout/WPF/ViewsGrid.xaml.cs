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
    /// Interaction logic for ViewsGrid.xaml
    /// </summary>
    public partial class ViewsGrid : UserControl
    {
        public ViewsGrid()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty DataFileProperty =
            DependencyProperty.Register("DataFile", typeof(DataFile), typeof(ViewsGrid), new FrameworkPropertyMetadata(null, OnDataFileChanged));
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

        private static void OnDataFileChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var o = d as ViewsGrid;
            if (e.NewValue != null)
            {

            }
        }

        internal void SetKeyboardFocus(IInputElement ie)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new Action(() =>
                {
                    Keyboard.Focus(ie);
                }));
        }

        internal void ApplyViewType(string vt)
        {
            ApplyGroupListVisibility((bool)
                (Application.Current.MainWindow as MainWindow).VM.Settings.GetValue("ShowGroupListView"));

            switch (vt)
            {
                case "List":
                    MyFlowViewAnchorable.IsSelected = true;
                    MyFlowViewAnchorable.IsActive = true;
                    MyDockingManager.ActiveContent = MyFlowViewAnchorable;
                    SetKeyboardFocus(MyFlowView);
                    break;

                case "DataGrid":
                    MyDataGridAnchorable.IsSelected = true;
                    MyDataGridAnchorable.IsActive = true;
                    MyDockingManager.ActiveContent = MyFlowViewAnchorable;
                    SetKeyboardFocus(MyDataGrid);
                    break;

                case "HorizontalSplit":
                   // throw new NotImplementedException();
                    break;

                case "VerticalSplit":
                   // throw new NotImplementedException();
                    //MyFlowViewAnchorable.Float();
                    //MyFlowViewAnchorable.IsVisible = false;
                    //MyFlowViewAnchorable.IsHidden = false;
                    //MyDataGridAnchorable.is();
                    //MyFlowViewAnchorable.AddToLayout(MyDockingManager,
                    //    Xceed.Wpf.AvalonDock.Layout.AnchorableShowStrategy.Top);
                    //MyDataGridAnchorable.AddToLayout(MyDockingManager,
                    //    Xceed.Wpf.AvalonDock.Layout.AnchorableShowStrategy.Bottom);
                    break;
            }
        }

        internal void ApplyGroupListVisibility(bool value)
        {
            MyGroupListAnchorable.IsSelected = value;
            MyGroupListAnchorable.IsActive = value;
            Keyboard.Focus(MyClockGroupListView);
        }

        public class ZoomableControlEventArgs : EventArgs
        {
            public IZoomableControl ZoomableControl;

            public ZoomableControlEventArgs(IZoomableControl c)
            {
                ZoomableControl = c;
            }
        }

        public event EventHandler<ZoomableControlEventArgs> FocusedZoomableControlChanged;

        private void MyClockGroupListView_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // handled in another handler for each of the zoomable subcontrols of MyClockGroupListView
        }

        private void MyFlowView_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                FocusedZoomableControlChanged?.Invoke(this, new ZoomableControlEventArgs(MyFlowView));
            }
        }

        private void MyDataGrid_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                FocusedZoomableControlChanged?.Invoke(this, new ZoomableControlEventArgs(MyDataGrid));
            }
        }

        private void MyViewsGrid_Loaded(object sender, RoutedEventArgs e)
        {

        }

        internal event ScrollChangedEventHandler ScrollChanged;

        private void MyClockGroupListView_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollChanged?.Invoke(sender, e);
        }

        private void MyFlowView_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollChanged?.Invoke(sender, e);
        }

        private void MyDataGrid_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollChanged?.Invoke(sender, e);
        }

        private void MyClockGroupListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (DataFile == null)
            {
                return;
            }

            if (e.NewSize.Width == 0 || ActualWidth == 0)
            {
                return;
            }

            DataFile.SetValueWithoutApply("EasyViewGroupListWidthPercent",
                e.NewSize.Width / ActualWidth * 100d); // works
        }

        //private void MyFlowView_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    if (DataFile == null)
        //    {
        //        return;
        //    }

        //    switch (DataFile.Settings.GetValue("ViewType").ToString())
        //    {
        //        case "HorizontalSplit":
        //            if (ActualWidth == 0 || e.NewSize.Width == 0)
        //            {
        //                return;
        //            }
        //            DataFile.SetValueWithoutApply("EasyViewClockListViewSizePercent", e.NewSize.Width / ActualWidth * 100d);
        //            break;

        //        case "VerticalSplit":
        //            if (ClocksViewsGrid.ActualHeight == 0 || e.NewSize.Height == 0)
        //            {
        //                return;
        //            }
        //            DataFile.SetValueWithoutApply("EasyViewClockListViewSizePercent", e.NewSize.Height / ClocksViewsGrid.ActualHeight * 100d);
        //            break;

        //        case "List":
        //        case "DataGrid":
        //            DataFile.SetValueWithoutApply("EasyViewClockListViewSizePercent", 100d);
        //            break;
        //    }
        //}

        private void MyDataGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // here nothing to do, all jobs handled in MyFlowView_SizeChanged
        }

        private void MyClockGroupListView_FocusedZoomableSubcontrolChanged(object sender, ZoomableControlEventArgs e)
        {
            FocusedZoomableControlChanged?.Invoke(this, e);
        }
    }
}
