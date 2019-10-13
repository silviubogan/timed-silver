using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;

namespace cs_timed_silver
{
    /// <summary>
    /// Interaction logic for ClockGroupListView.xaml
    /// </summary>
    public partial class ClockGroupListView : ContentControl, IClocksView, IZoomableControl,
        GongSolutions.Wpf.DragDrop.IDropTarget, IDragSource
    {
        internal bool IsDragDropIndexValid(int i)
        {
            return i > 4;
        }

        void GongSolutions.Wpf.DragDrop.IDropTarget.DragOver(IDropInfo dropInfo)
        {
            var sourceItem = dropInfo.Data as FilterVM;
            var targetItem = dropInfo.TargetItem as FilterVM;

            if (sourceItem != null
                /*&& targetItem != null*/ && // targetItem is null when the user drops at the end of the list
                IsDragDropIndexValid(dropInfo.DragInfo.SourceIndex)) // &&
                                                                     //IsDragDropIndexValid(dropInfo.InsertIndex)) // commented this because it makes dropping to
                                                                     // the empty space of the ListView interdicted, with interdiction cursor while dragging...
                                                                     // TODO: make it work someday...
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = System.Windows.DragDropEffects.Move;
            }
            else
            {
                dropInfo.Effects = System.Windows.DragDropEffects.None;
            }
        }

        public static IEnumerable ExtractData(object data)
        {
            if (data is IEnumerable && !(data is string))
            {
                return (IEnumerable)data;
            }

            return Enumerable.Repeat(data, 1);
        }

        /// <summary>
        /// Clears the current selected items and selects the given items.
        /// </summary>
        /// <param name="dropInfo">The drop information.</param>
        /// <param name="items">The items which should be select.</param>
        /// <param name="applyTemplate">if set to <c>true</c> then for all items the ApplyTemplate will be invoked.</param>
        /// <param name="focusVisualTarget">if set to <c>true</c> the visual target will be focused.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="dropInfo" /> is <see langword="null" /></exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="dropInfo" /> is <see langword="null" /></exception>
        public static void SelectDroppedItems(IDropInfo dropInfo, IEnumerable items, bool applyTemplate = true, bool focusVisualTarget = true)
        {
            if (dropInfo == null) throw new ArgumentNullException(nameof(dropInfo));
            if (items == null) throw new ArgumentNullException(nameof(items));
            var itemsControl = dropInfo.VisualTarget as ItemsControl;
            if (itemsControl != null)
            {
                var tvItem = dropInfo.VisualTargetItem as TreeViewItem;
                var tvItemIsExpanded = tvItem != null && tvItem.HasHeader && tvItem.HasItems && tvItem.IsExpanded;

                var itemsParent = tvItemIsExpanded ? tvItem : (dropInfo.VisualTargetItem != null ? ItemsControl.ItemsControlFromItemContainer(dropInfo.VisualTargetItem) : itemsControl);
                itemsParent = itemsParent ?? itemsControl;

                itemsParent.ClearSelectedItems();

                foreach (var obj in items)
                {
                    if (applyTemplate)
                    {
                        // call ApplyTemplate for TabItem in TabControl to avoid this error:
                        //
                        // System.Windows.Data Error: 4 : Cannot find source for binding with reference
                        var container = itemsParent.ItemContainerGenerator.ContainerFromItem(obj) as FrameworkElement;
                        container?.ApplyTemplate();
                    }
                    itemsParent.SetItemSelected(obj, true);
                }

                if (focusVisualTarget)
                {
                    itemsControl.Focus();
                }
            }
        }

        /// <summary>
        /// Determines whether the data of the drag drop action should be copied otherwise moved.
        /// </summary>
        /// <param name="dropInfo">The DropInfo with a valid DragInfo.</param>
        public static bool ShouldCopyData(IDropInfo dropInfo)
        {
            // default should always the move action/effect
            if (dropInfo == null || dropInfo.DragInfo == null)
            {
                return false;
            }
            var copyData = ((dropInfo.DragInfo.DragDropCopyKeyState != default(DragDropKeyStates)) && dropInfo.KeyStates.HasFlag(dropInfo.DragInfo.DragDropCopyKeyState))
                           || dropInfo.DragInfo.DragDropCopyKeyState.HasFlag(DragDropKeyStates.LeftMouseButton);
            copyData = copyData
                       //&& (dropInfo.DragInfo.VisualSource != dropInfo.VisualTarget)
                       && !(dropInfo.DragInfo.SourceItem is HeaderedContentControl)
                       && !(dropInfo.DragInfo.SourceItem is HeaderedItemsControl)
                       && !(dropInfo.DragInfo.SourceItem is ListBoxItem);
            return copyData;
        }

        /// <summary>
        /// Gets the enumerable as list.
        /// If enumerable is an ICollectionView then it returns the SourceCollection as list.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>Returns a list.</returns>
        public static IList TryGetList(IEnumerable enumerable)
        {
            if (enumerable is ICollectionView)
            {
                return ((ICollectionView)enumerable).SourceCollection as IList;
            }
            else
            {
                var list = enumerable as IList;
                return list ?? (enumerable != null ? enumerable.OfType<object>().ToList() : null);
            }
        }

        void GongSolutions.Wpf.DragDrop.IDropTarget.Drop(IDropInfo dropInfo)
        {
            if (dropInfo == null || dropInfo.DragInfo == null)
            {
                return;
            }

            bool sourceIndexValid = IsDragDropIndexValid(dropInfo.DragInfo.SourceIndex);
            bool targetIndexValid = IsDragDropIndexValid(dropInfo.InsertIndex);

            if (!sourceIndexValid ||
                !targetIndexValid)
            {
                return;
            }

            var insertIndex = dropInfo.UnfilteredInsertIndex;

            var itemsControl = dropInfo.VisualTarget as ItemsControl;
            if (itemsControl != null)
            {
                var editableItems = itemsControl.Items as IEditableCollectionView;
                if (editableItems != null)
                {
                    var newItemPlaceholderPosition = editableItems.NewItemPlaceholderPosition;
                    if (newItemPlaceholderPosition == NewItemPlaceholderPosition.AtBeginning && insertIndex == 0)
                    {
                        ++insertIndex;
                    }
                    else if (newItemPlaceholderPosition == NewItemPlaceholderPosition.AtEnd && insertIndex == itemsControl.Items.Count)
                    {
                        --insertIndex;
                    }
                }
            }

            var destinationList = TryGetList(dropInfo.TargetCollection);
            var data = ExtractData(dropInfo.Data).OfType<object>().ToList();

            var copyData = ShouldCopyData(dropInfo);
            if (!copyData)
            {
                var sourceList = TryGetList(dropInfo.DragInfo.SourceCollection);
                if (sourceList != null)
                {
                    foreach (var o in data)
                    {
                        var index = sourceList.IndexOf(o);
                        if (index != -1)
                        {
                            sourceList.RemoveAt(index);
                            // so, is the source list the destination list too ?
                            if (destinationList != null && Equals(sourceList, destinationList) && index < insertIndex)
                            {
                                --insertIndex;
                            }
                        }
                    }
                }
            }

            if (destinationList != null)
            {
                var objects2Insert = new List<object>();

                // check for cloning
                var cloneData = dropInfo.Effects.HasFlag(DragDropEffects.Copy) || dropInfo.Effects.HasFlag(DragDropEffects.Link);
                foreach (var o in data)
                {
                    var obj2Insert = o;
                    if (cloneData)
                    {
                        var cloneable = o as ICloneable;
                        if (cloneable != null)
                        {
                            obj2Insert = cloneable.Clone();
                        }
                    }

                    objects2Insert.Add(obj2Insert);
                    destinationList.Insert(insertIndex++, obj2Insert);
                }

                var selectDroppedItems = itemsControl is TabControl || (itemsControl != null && GongSolutions.Wpf.DragDrop.DragDrop.GetSelectDroppedItems(itemsControl));
                if (selectDroppedItems)
                {
                    SelectDroppedItems(dropInfo, objects2Insert);
                }
            }

            // my custom implementation (not good):
            //Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    ReloadGroups();
            //}), System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            //HandleDrop(dropInfo);
        }

        //private void HandleDrop(IDropInfo dropInfo)
        //{
        //    if (dropInfo.TargetItem == dropInfo.Data)
        //    {
        //        return;
        //    }

        //    //dropInfo.DragInfo.SourceItems;
        //    //dropInfo.TargetCollection;
        //    //dropInfo.InsertIndex;

        //    foreach (FilterVM vm in dropInfo.DragInfo.SourceItems)
        //    {
        //        (dropInfo.DragInfo.SourceCollection as ObservableCollection<FilterVM>).Remove(vm);
        //    }

        //    var c = dropInfo.TargetCollection as ObservableCollection<FilterVM>;

        //    int i = dropInfo.InsertIndex;

        //    //if (dropInfo.InsertPosition== RelativeInsertPosition.AfterTargetItem)
        //    //{
        //    //    i = dropInfo.InsertIndex + 1;
        //    //}

        //    foreach (FilterVM vm in dropInfo.DragInfo.SourceItems)
        //    {
        //        //if (i >= c.Count)
        //        //{
        //        //    c.Add(vm);
        //        //}
        //        //else
        //        //{
        //        c.Insert(Math.Max(0, i - 1), vm);
        //        //}
        //        i++;
        //    }
        //}

        public static readonly DependencyProperty MyDataFileProperty =
            DependencyProperty.Register("MyDataFile", typeof(DataFile), typeof(ClockGroupListView),
                new FrameworkPropertyMetadata(null, OnMyDataFileChanged));
        public DataFile MyDataFile
        {
            get
            {
                return (DataFile)GetValue(MyDataFileProperty);
            }
            set
            {
                SetValue(MyDataFileProperty, value);
            }
        }

        private static void OnMyDataFileChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var o = d as ClockGroupListView;

            o.ApplyTemplate();

            o.MyFilters = ((DataFile)e.NewValue).ClockVMCollection.Model.FiltersVM.VMs;

            //o.ReloadGroups();

            o.MyDataFile.ClockVMCollection.Model.Ms.CollectionChanged += o.ClockMCollection_CollectionChanged;
            o.MyDataFile.ClockVMCollection.Model.ClockPropertyChanged += o.ClockMCollection_ClockPropertyChanged;

            //o.MyDataFile.ClockVMCollection.Model.GroupsVM.VMs.CollectionChanged += o.VMs_CollectionChanged;
            o.MyDataFile.ClockVMCollection.Model.GroupsChanged += o.Model_GroupsChanged;

            o.MyListView.ItemsSource = o.MyFilters;
        }

        //private void VMs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    ReloadGroups();
        //}

        internal ListView MyListView;
        internal Grid MyGrid;
        internal ZoomableStackPanel MyToolBarContainer;
        internal ComboBox MyViewComboBox;
        internal ComboBoxItem MyUniformGridComboBoxItem;
        internal Button AddGroupBtn, DeleteGroupBtn, MyResetButton, MySearchButton;
        internal TextBox MySearchTextBox;
        internal ZoomableScrollViewer MyScrollViewer;
        internal ToolBar MyFirstToolBar;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            MyListView = (ListView)GetTemplateChild("MyListView");
            MyGrid = (Grid)GetTemplateChild("MyGrid");
            MyToolBarContainer = (ZoomableStackPanel)GetTemplateChild("MyToolBarContainer");
            MyViewComboBox = (ComboBox)GetTemplateChild("MyViewComboBox");
            MyUniformGridComboBoxItem = (ComboBoxItem)GetTemplateChild("MyUniformGridComboBoxItem");
            AddGroupBtn = (Button)GetTemplateChild("AddGroupBtn");
            DeleteGroupBtn = (Button)GetTemplateChild("DeleteGroupBtn");
            MySearchTextBox = (TextBox)GetTemplateChild("MySearchTextBox");
            MyResetButton = (Button)GetTemplateChild("MyResetButton");
            MySearchButton = (Button)GetTemplateChild("MySearchButton");
            MyScrollViewer = (ZoomableScrollViewer)GetTemplateChild("MyScrollViewer");
            MyFirstToolBar = (ToolBar)GetTemplateChild("MyFirstToolBar");

            MyListView.MouseDoubleClick += MyListView_MouseDoubleClick;

            MyToolBarContainer.IsKeyboardFocusWithinChanged += MyToolBarContainer_IsKeyboardFocusWithinChanged;
            MyToolBarContainer.MouseDown += MyToolBarContainer_MouseDown;

            MyViewComboBox.SelectionChanged += MyViewComboBox_SelectionChanged;
            MyViewComboBox.DropDownClosed += MyViewComboBox_DropDownClosed;

            AddGroupBtn.Click += AddGroup_Click;
            DeleteGroupBtn.Click += DeleteGroup_Click;

            MySearchTextBox.KeyDown += MySearchTextBox_KeyDown;
            MySearchTextBox.KeyUp += MySearchTextBox_KeyUp;
            MySearchTextBox.LostKeyboardFocus += MySearchTextBox_LostKeyboardFocus;

            MyResetButton.Click += MyResetButton_Click;

            MySearchButton.Click += MySearchButton_Click;

            MyScrollViewer.Drop += MyScrollViewer_Drop;
            MyScrollViewer.Loaded += MyScrollViewer_Loaded;
            MyScrollViewer.IsKeyboardFocusWithinChanged += MyScrollViewer_IsKeyboardFocusWithinChanged;
            MyScrollViewer.MouseDown += MyScrollViewer_MouseDown;
            MyScrollViewer.AddHandler(ScrollViewer.ScrollChangedEvent,
                new ScrollChangedEventHandler(MyScrollViewer_ScrollChanged));
        }

        private void Model_GroupsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ReloadGroups();
        }

        private void ClockMCollection_ClockPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateItemCounts();
        }

        private void ClockMCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateItemCounts();
        }

        internal ObservableCollection<FilterVM> MyFilters;

        static ClockGroupListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ClockGroupListView),
                new FrameworkPropertyMetadata(typeof(ClockGroupListView)));
        }

        public ClockGroupListView()
        {
            InitializeComponent();

            FilterActivated += ClockGroupListView_FilterActivated;

            DataContext = this;
        }

        private void ClockGroupListView_FilterActivated(object sender, FilterEventArgs e)
        {
            AppliedFilter = e.Filter;
        }

        internal void ReloadGroups()
        {
            foreach (FilterVM vm in MyFilters)
            {
                vm.UpdateItemCount();
                vm.SynchronizeFromModel();
            }

            //for (int i = MyFilters.Count - 1; i >= 5; --i)
            //{
            //    MyFilters.RemoveAt(i);
            //}

            //if (MyDataFile == null)
            //{
            //    return;
            //}

            //// here MyFilters wouldbe correctly updated,  but until this point gr.Ms/VMs is not the same with the VMs.

            //ObservableCollection<ClockGroupVM> gr = MyDataFile.ClockVMCollection.Model.GroupsVM.VMs;
            //int ig = 0;
            //foreach (ClockGroupVM g in gr)
            //{
            //    var fd = new FilterVM()
            //    {
            //        DisplayString = g.Name,
            //        MyFilter = new FilterM(MyDataFile.ClockVMCollection.Model, $"{ig + 1}")
            //        {
            //            ShowActive = true,
            //            ShowInactive = true,
            //            ShowAlarms = true,
            //            ShowTimers = true
            //        }
            //    };
            //    if (g.Icon == null)
            //    {
            //        fd.MyConstantImageSource = null;
            //        fd.MyEmptyImageSource = MyEmptyFolderIcon;
            //        fd.MyNonEmptyImageSource = MyNonEmptyFolderIcon;
            //    }
            //    else
            //    {
            //        fd.MyConstantImageSource = Utils.GetBitmapImageFromBitmap(g.Icon);
            //        fd.MyEmptyImageSource = null;
            //        fd.MyNonEmptyImageSource = null;

            //    }
            //    MyFilters.Add(fd);
            //    ++ig;
            //}

            //UpdateItemCounts();
        }

        internal void UpdateItemCounts()
        {
            foreach (FilterVM fd in MyFilters)
            {
                fd.UpdateItemCount();
            }
        }

        public override string ToString()
        {
            return "WPF ClockGroupListView";
        }

        public static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register("ZoomFactor", typeof(decimal), typeof(ClockGroupListView), new FrameworkPropertyMetadata(1M, OnZoomFactorChanged));

        private static void OnZoomFactorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

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

        internal event EventHandler<FilterEventArgs> FilterActivated;

        private void MyListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //ListViewItem it = Utils.GetVisualParent<ListViewItem>(e.OriginalSource as DependencyObject);
            if (MyListView.SelectedItem != null)
            {
                // supports multiple filters at once through the context menu
                FilterActivated?.Invoke(this, new FilterEventArgs()
                {
                    Filter = (MyListView.SelectedItem as FilterVM).M
                });
            }
        }

        private void RenameThisGroup_Click(object sender, RoutedEventArgs e)
        {
            var pd = new TextPromptWindow();
            pd.Owner = System.Windows.Application.Current.MainWindow;

            var fd = MyListView.SelectedItems[0] as FilterVM;
            pd.UserString = fd.DisplayString;

            if ((bool)pd.ShowDialog())
            {
                MyDataFile.ClockVMCollection.Model.Groups.Rename(
                    fd.DisplayString,
                    pd.UserString);
                ReloadGroups();
            }
        }

        private void ClearGroup_Click(object sender, RoutedEventArgs e)
        {
            foreach (FilterVM fd in MyListView.SelectedItems)
            {
                MyDataFile.ClockVMCollection.Model.Groups.ClearGroup(
                        fd.DisplayString);

                fd.Items = 0;
            }
        }

        private void SetIcon_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = Utils.GetImageChooser();

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var ico = new System.Drawing.Bitmap(ofd.FileName);

                for (int i = MyListView.SelectedItems.Count - 1; i >= 0; --i)
                {
                    var fd = MyListView.SelectedItems[i] as FilterVM;

                    ClockGroupMCollection x = MyDataFile.ClockVMCollection.Model.Groups;

                    // DisplayString equivalent to Name for filters which have a context
                    // menu:
                    x.Ms[x.Ms.IndexOf(cgm => cgm.Name == fd.DisplayString)].Icon =
                        Utils.GetBitmapImageFromBitmap(ico);

                    ReloadGroups();
                }

                ico.Dispose();
            }
        }

        private void ContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            bool showCms = false;
            foreach (FilterVM fd in MyListView.SelectedItems)
            {
                if (fd.M.GroupNames.Count != 0)
                {
                    showCms = true;
                    break;
                }
            }

            if (!showCms)
            {
                e.Handled = true;
            }
        }

        private void DeleteGroup_Click(object sender, RoutedEventArgs e)
        {
            var arr = new FilterVM[MyListView.SelectedItems.Count];
            MyListView.SelectedItems.CopyTo(arr, 0);

            for (int i = arr.Length - 1; i >= 0; --i)
            {
                var fd = arr[i];
                if (MyFilters.IndexOf(fd) >= 5)
                {
                    string s = fd.DisplayString;
                    MyDataFile.ClockVMCollection.Model.Groups.Remove(s);
                }
            }
        }

        private void RemoveIcon_Click(object sender, RoutedEventArgs e)
        {
            for (int i = MyListView.SelectedItems.Count - 1; i >= 0; --i)
            {
                var fd = MyListView.SelectedItems[i] as FilterVM;

                ClockGroupMCollection x = MyDataFile.ClockVMCollection.Model.Groups;

                // DisplayString equivalent to Name for filters which have a context
                // menu:
                x.Ms[x.IndexOf(fd.DisplayString)].Icon = null;

                ReloadGroups();
            }
        }

        private void AddGroup_Click(object sender, RoutedEventArgs e)
        {
            ShowAddNewGroupPrompt(MyDataFile);
            ReloadGroups();
        }

        internal static string ShowAddNewGroupPrompt(DataFile df)
        {
            var p = new TextPromptWindow();
            p.Owner = System.Windows.Application.Current.MainWindow;

            bool r;
            r = (bool)p.ShowDialog();

            if (r)
            {
                if (p.UserString != "")
                {
                    df.ClockVMCollection.Model.Groups.Add(p.UserString);
                    return p.UserString;
                }
            }
            return null;
        }

        internal Rect dragBoxFromMouseDown;
        internal List<FilterVM> DraggedFilterDisplays = new List<FilterVM>();
        private void ListViewItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //if (e.LeftButton == MouseButtonState.Pressed)
            //{
            //    var mySender = sender as FrameworkElement;
            //    var myData = mySender.DataContext as FilterDisplay;

            //    DraggedFilterDisplays.Clear();
            //    bool thisAdded = false;
            //    for (int i = 5; i < MyFilters.Count; ++i)
            //    {
            //        FilterDisplay fd = MyFilters[i];
            //        if (fd.IsSelected)
            //        {
            //            DraggedFilterDisplays.Add(fd);
            //            if (ReferenceEquals(fd, myData))
            //            {
            //                thisAdded = true;
            //            }
            //        }
            //    }
            //    if (!thisAdded && (from fd in MyFilters
            //                       where MyFilters.IndexOf(fd) >= 5
            //                       where fd == myData
            //                       select fd).Count() == 1)
            //    {
            //        DraggedFilterDisplays.Add(myData);
            //    }

            //    if (DraggedFilterDisplays.Count > 0 &&
            //        mySender is ListViewItem)
            //    {
            //        Size dragSize = new Size(
            //            SystemParameters.MinimumHorizontalDragDistance,
            //            SystemParameters.MinimumVerticalDragDistance);
            //        System.Drawing.Point pt = Utils.GetMousePositionWindowsForms();
            //        var pt2 = new System.Windows.Point(pt.X, pt.Y);

            //        dragBoxFromMouseDown = new Rect(
            //            new Point(
            //                pt2.X - (dragSize.Width / 2),
            //                pt2.Y - (dragSize.Height / 2)),
            //            dragSize);
            //    }
            //    else
            //    {
            //        dragBoxFromMouseDown = Rect.Empty;
            //        DraggedFilterDisplays.Clear();
            //    }
            //}
        }

        private void ListViewItem_MouseMove(object sender, MouseEventArgs e)
        {
            //if (e.LeftButton == MouseButtonState.Pressed)
            //{
            //    System.Drawing.Point p = Utils.GetMousePositionWindowsForms();
            //    if (dragBoxFromMouseDown != Rect.Empty &&
            //        !dragBoxFromMouseDown.Contains(new System.Windows.Point(p.X, p.Y)))
            //    {
            //        var mySender = sender as FrameworkElement;
            //        var myData = mySender.DataContext as FilterDisplay;

            //        var data = new DataObject();
            //        data.SetData(typeof(List<FilterDisplay>),
            //            DraggedFilterDisplays);
            //        DragDrop.DoDragDrop(sender as DependencyObject,
            //            data, DragDropEffects.Move);

            //        e.Handled = true;
            //    }
            //}
        }

        private void ListViewItem_Drop(object sender, DragEventArgs e)
        {
            var mySender = sender as FrameworkElement;
            //if (e.Data.GetDataPresent(typeof(List<FilterDisplay>)))
            //{
            //    var fds = e.Data.GetData(typeof(List<FilterDisplay>)) as List<FilterDisplay>;
            //    if (fds != null)
            //    {
            //        if (!(e.OriginalSource is ScrollViewer))
            //        {
            //            int newIndex = Math.Max(5, MyListView.Items.IndexOf(
            //                (e.OriginalSource as FrameworkElement).DataContext));
            //            var list = MyListView.ItemsSource as ObservableCollection<FilterDisplay>;

            //            foreach (int idx in from fd in fds
            //                                select list.IndexOf(fd))
            //            {
            //                list.Move(idx, newIndex);
            //            }
            //        }
            //    }
            //}
            ///*else*/ if (e.Data.GetDataPresent(typeof(List<ClockData>)))
            //{
            //    var cds = e.Data.GetData(typeof(List<ClockData>)) as List<ClockData>;
            //    if (cds != null)
            //    {
            //        if (!(e.OriginalSource is ScrollViewer))
            //        {
            //            var fd = (e.OriginalSource as FrameworkElement).
            //                DataContext as FilterDisplay;

            //            int i = MyListView.Items.IndexOf(fd);



            //            foreach (ClockData cd in cds)
            //            {
            //                if (i <= 4)
            //                {
            //                    cd.GroupName = "";
            //                }
            //                else
            //                {
            //                    cd.GroupName = fd.DisplayString;
            //                }
            //            }
            //        }
            //    }
            //}
        }

        private void MyScrollViewer_Drop(object sender, DragEventArgs e)
        {
            //var fds = e.Data.GetData(typeof(List<FilterDisplay>)) as List<FilterDisplay>;
            //var mySender = sender as FrameworkElement;
            //if (e.OriginalSource is ScrollViewer && fds != null)
            //{
            //    var list = MyListView.ItemsSource as ObservableCollection<FilterDisplay>;
            //    foreach (FilterDisplay fd in fds)
            //    {
            //        list.Move(list.IndexOf(fd), list.Count() - 1);
            //    }
            //}
        }

        private void ApplyFilters_Click(object sender, RoutedEventArgs e)
        {
            if (MyListView.SelectedItems.Count > 0)
            {
                bool more = false;

                FilterM f = (MyListView.SelectedItems[0] as FilterVM).M;
                for (int i = 1; i < MyListView.SelectedItems.Count; ++i)
                {
                    f = FilterM.Combine(f,
                            (MyListView.SelectedItems[i] as FilterVM).M);
                    more = true;
                }

                if (!more)
                {
                    f = f.Clone();
                }
                f.SearchString = MySearchTextBox.Text;
                //MyTimerGroupListView.AppliedFilter = f;
            }
        }

        private void MySearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter ||
                e.Key == Key.Return)
            {
                MySearchButton.IsDefault = true;
                DoSearch();
                e.Handled = true;
            }
        }

        public FilterM _AppliedFilter = null;
        public FilterM AppliedFilter
        {
            get
            {
                return _AppliedFilter;
            }
            set
            {
                if (_AppliedFilter != value)
                {
                    _AppliedFilter = value;
                    OnAppliedFilterChanged();
                }
            }
        }

        private void OnAppliedFilterChanged()
        {
            //foreach (ITimersView tsv in TargetViews)
            //{
            //    if (tsv as TimerGroupListView == null)
            //    {
            MyDataFile.ClockVMCollection.Model.AppliedFilter = AppliedFilter;
            //    }
            //}
        }

        internal void DoSearch()
        {
            if (AppliedFilter != null)
            {
                FilterM f = AppliedFilter.Clone();
                f.SearchString = MySearchTextBox.Text;
                AppliedFilter = f;
            }
            else
            {
                var f = new FilterM(MyDataFile.ClockVMCollection.Model);
                f.SearchString = MySearchTextBox.Text;
                AppliedFilter = f;
            }
        }

        private void MyResetButton_Click(object sender, RoutedEventArgs e)
        {
            MySearchTextBox.Clear();
            DoSearch();
        }

        private void MySearchButton_Click(object sender, RoutedEventArgs e)
        {
            DoSearch();
        }

        private void ListViewItem_DragOver(object sender, DragEventArgs e)
        {
            //if (sender is ListViewItem lvi &&
            //    lvi.DataContext is FilterVM)
            //{
            //    MyListView.SelectedItem = lvi.DataContext;
            //    e.Handled = true;
            //}
        }

        private void MyScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            //MyScrollViewer.Tag = new ListViewDragDropManager(MyListView);
        }

        private void MyViewComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MyDataFile == null)
            {
                return;
            }

            // selected item's Content can be null before the
            // XAML is completely loaded (SelectionChanged is
            // triggered then too)
            if (!MyViewComboBox.IsLoaded)
            {
                return;
            }

            var item = MyViewComboBox.SelectedItem as ComboBoxItem;
            //string s = (e.AddedItems[0] as ComboBoxItem).Content.ToString();
            string s = item.Content.ToString();

            switch (s)
            {
                case "Large icons":
                    MyListView.View = null;
                    MyListView.ItemTemplate = (DataTemplate)FindResource("LargeIconsTemplate");
                    MyListView.ItemsPanel = (ItemsPanelTemplate)FindResource("LargeIconsPanelTemplate");

                    MyDataFile.Settings.SetValue("GroupListListViewViewType", "Large icons");
                    break;

                case "Stack":
                    MyListView.View = null;
                    MyListView.ItemTemplate = (DataTemplate)FindResource("StackTemplate");
                    MyListView.ItemsPanel = (ItemsPanelTemplate)FindResource("StackPanelTemplate");

                    MyDataFile.Settings.SetValue("GroupListListViewViewType", "Stack");
                    break;

                case "Grid":
                    MyListView.ItemTemplate = (DataTemplate)FindResource("StackTemplate");
                    MyListView.ItemsPanel = (ItemsPanelTemplate)FindResource("StackPanelTemplate");
                    MyListView.View = (GridView)FindResource("MyGridView");

                    MyDataFile.Settings.SetValue("GroupListListViewViewType", "Grid");
                    break;

                case "Uniform grid":
                    //var p = new UniformGridSizePrompt();
                    //p.Owner = Application.Current.MainWindow;
                    //if ((bool)p.ShowDialog())
                    //{
                    //    MyListView.View = null;
                    //    MyListView.ItemTemplate = (DataTemplate)FindResource("LargeIconsWithViewboxTemplate");
                    //    MyListView.ItemsPanel = (ItemsPanelTemplate)FindResource("UniformGridLargeIconsPanelTemplate");

                    //    rc = p.RowCount;
                    //    cc = p.ColumnCount;

                    //    MyDataFile.Settings.SetValue("GroupListListViewViewType", "Uniform grid");
                    //}
                    break;
            }
        }

        internal int rc, cc;

        private void MySearchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter ||
                e.Key == Key.Return)
            {
                MySearchButton.IsDefault = false;
                e.Handled = true;
            }
        }

        private void MySearchTextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            MySearchButton.IsDefault = false;
        }

        public event ScrollChangedEventHandler ScrollChanged;

        private void MyScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollChanged?.Invoke(sender, e);
        }

        public event EventHandler<ViewsGrid.ZoomableControlEventArgs> FocusedZoomableSubcontrolChanged;

        private void MyScrollViewer_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (MyScrollViewer.IsKeyboardFocusWithin)
            {
                FocusedZoomableSubcontrolChanged?.Invoke(sender, new ViewsGrid.ZoomableControlEventArgs(MyScrollViewer));
            }
        }

        private void MyToolBarContainer_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (MyToolBarContainer.IsKeyboardFocusWithin)
            {
                FocusedZoomableSubcontrolChanged?.Invoke(sender, new ViewsGrid.ZoomableControlEventArgs(MyToolBarContainer));
            }
        }

        private void MyToolBarContainer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.Focus(MyToolBarContainer);
        }

        private void ContentControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void MyScrollViewer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.Focus(MyScrollViewer);
        }

        private void MyViewComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (MyUniformGridComboBoxItem.IsSelected)
            {
                var p = new UniformGridSizePrompt();
                p.Owner = Application.Current.MainWindow;
                if ((bool)p.ShowDialog())
                {
                    MyListView.View = null;
                    MyListView.ItemTemplate = (DataTemplate)FindResource("LargeIconsWithViewboxTemplate");
                    MyListView.ItemsPanel = (ItemsPanelTemplate)FindResource("UniformGridLargeIconsPanelTemplate");

                    if (MyLastUniformGrid == null)
                    {
                        rc = p.RowCount;
                        cc = p.ColumnCount;
                    }
                    else
                    {
                        MyLastUniformGrid.Rows = p.RowCount;
                        MyLastUniformGrid.Columns = p.ColumnCount;
                    }

                    MyDataFile.Settings.SetValue("GroupListListViewViewType", "Uniform grid");
                }
            }
            else
            {
                MyLastUniformGrid = null;
            }
        }

        internal UniformGrid MyLastUniformGrid = null;

        private void MyUniformGrid_Loaded(object sender, RoutedEventArgs e)
        {
            MyLastUniformGrid = sender as UniformGrid;
            MyLastUniformGrid.Rows = rc;
            MyLastUniformGrid.Columns = cc;
        }

        void IDragSource.StartDrag(IDragInfo dragInfo)
        {
            var items = TypeUtilities.CreateDynamicallyTypedList(dragInfo.SourceItems).Cast<object>().ToList();
            if (items.Count > 1)
            {
                dragInfo.Data = items;
            }
            else
            {
                // special case: if the single item is an enumerable then we can not drop it as single item
                var singleItem = items.FirstOrDefault();
                if (singleItem is IEnumerable && !(singleItem is string))
                {
                    dragInfo.Data = items;
                }
                else
                {
                    dragInfo.Data = singleItem;
                }
            }

            dragInfo.Effects = dragInfo.Data != null ? DragDropEffects.Copy | DragDropEffects.Move : DragDropEffects.None;
        }

        bool IDragSource.CanStartDrag(IDragInfo dragInfo)
        {
            if (dragInfo.SourceIndex > 4)
            {
                return true;
            }
            return false;
        }

        void IDragSource.Dropped(IDropInfo dropInfo)
        {
            //throw new NotImplementedException();
        }

        void IDragSource.DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo)
        {
            //if (operationResult != DragDropEffects.None)
            //{
               // ReloadGroups();
                //Model.IsUnsaved = true; // Is this needed?
            //}
        }

        void IDragSource.DragCancelled()
        {
        }

        bool IDragSource.TryCatchOccurredException(Exception exception)
        {
            return false;
        }
    }
}
