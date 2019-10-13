using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using System.Windows.Shapes;
using System.Threading.Tasks;

namespace cs_timed_silver
{
    #region ListViewDragDropManager

    /// <summary>
    /// Manages the dragging and dropping of ListViewItems in a ListView.
    /// The ItemType type parameter indicates the type of the objects in
    /// the ListView's items source.  The ListView's ItemsSource must be 
    /// set to an instance of ClockDataCollection of ItemType, or an 
    /// Exception will be thrown.
    /// </summary>
    /// <typeparam name="ClockData">The type of the ListView's items.</typeparam>
    public class ListViewDragDropManager
    {
        #region Data

        bool canInitiateDrag;
        DragAdorner dragAdorner;
        double dragAdornerOpacity;
        int indexToSelect;
        object itemUnderDragCursor;
        Point ptMouseDown;

        #endregion // Data


        #region Constructors

        /// <summary>
        /// Initializes a new instance of ListViewDragManager.
        /// </summary>
        public ListViewDragDropManager()
        {
            canInitiateDrag = false;
            dragAdornerOpacity = 1;
            indexToSelect = -1;
            showDragAdorner = true;
        }

        /// <summary>
        /// Initializes a new instance of ListViewDragManager.
        /// </summary>
        /// <param name="itemsControl"></param>
        public ListViewDragDropManager(ItemsControl itemsControl)
            : this()
        {
            MyItemsControl = itemsControl;

            if (MyItemsControl.ItemsSource is ObservableCollection<ClockVM>)
            {
                DraggedClocks = new List<ClockVM>();
                DraggedFilterDisplays = null;
            }
            else
            {
                DraggedClocks = null;
                DraggedFilterDisplays = new List<FilterDisplay>();
            }
        }

        /// <summary>
        /// Initializes a new instance of ListViewDragManager.
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="dragAdornerOpacity"></param>
        public ListViewDragDropManager(ItemsControl listView, double dragAdornerOpacity)
            : this(listView)
        {
            DragAdornerOpacity = dragAdornerOpacity;
        }

        /// <summary>
        /// Initializes a new instance of ListViewDragManager.
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="showDragAdorner"></param>
        public ListViewDragDropManager(ItemsControl listView, bool showDragAdorner)
            : this(listView)
        {
            ShowDragAdorner = showDragAdorner;
        }

        #endregion // Constructors

        #region Public Interface

        #region DragAdornerOpacity

        /// <summary>
        /// Gets/sets the opacity of the drag adorner.  This property has no
        /// effect if ShowDragAdorner is false. The default value is 0.7
        /// </summary>
        public double DragAdornerOpacity
        {
            get
            {
                return dragAdornerOpacity;
            }
            set
            {
                if (IsDragInProgress)
                {
                    throw new InvalidOperationException("Cannot set the DragAdornerOpacity property during a drag operation.");
                }

                if (value < 0.0 || value > 1.0)
                {
                    throw new ArgumentOutOfRangeException("DragAdornerOpacity", value, "Must be between 0 and 1.");
                }

                dragAdornerOpacity = value;
            }
        }

        #endregion // DragAdornerOpacity

        #region IsDragInProgress

        protected bool isDragInProgress;
        /// <summary>
        /// Returns true if there is currently a drag operation being managed.
        /// </summary>
        public bool IsDragInProgress
        {
            get
            {
                return isDragInProgress;
            }
            private set
            {
                isDragInProgress = value;
            }
        }

        #endregion // IsDragInProgress

        public ScrollViewer MyScrollViewer
        {
            get
            {
                if (MyItemsControl.TryFindVisualParentElement(out ScrollViewer r))
                {
                    return r;
                }
                return null;
            }
        }

        #region ListView

        ItemsControl _MyItemsControl;
        /// <summary>
        /// Gets/sets the ListView whose dragging is managed.  This property
        /// can be set to null, to prevent drag management from occuring.  If
        /// the ListView's AllowDrop property is false, it will be set to true.
        /// </summary>
        public ItemsControl MyItemsControl
        {
            get
            {
                return _MyItemsControl;
            }
            set
            {
                if (IsDragInProgress)
                {
                    throw new InvalidOperationException("Cannot set the ListView property during a drag operation.");
                }

                if (_MyItemsControl != null)
                {
                    #region Unhook Events

                    _MyItemsControl.PreviewMouseLeftButtonDown -=
                        listView_PreviewMouseLeftButtonDown;
                    _MyItemsControl.PreviewMouseMove -= listView_PreviewMouseMove;
                    if (MyScrollViewer == null)
                    {
                        _MyItemsControl.PreviewDragOver -= listView_PreviewDragOver;
                        _MyItemsControl.Drop -= listView_Drop;
                        _MyItemsControl.DragLeave -= listView_DragLeave;
                        _MyItemsControl.DragEnter -= listView_DragEnter;
                    }
                    else
                    {
                        MyScrollViewer.PreviewDragOver -= listView_PreviewDragOver;
                        MyScrollViewer.DragOver -= MyScrollViewer_DragOver;
                        MyScrollViewer.Drop -= listView_Drop;
                        MyScrollViewer.DragLeave -= listView_DragLeave;
                        MyScrollViewer.DragEnter -= listView_DragEnter;
                    }

                    #endregion // Unhook Events
                }

                _MyItemsControl = value;

                if (_MyItemsControl != null)
                {
                    if (!_MyItemsControl.AllowDrop)
                    {
                        _MyItemsControl.AllowDrop = true;
                    }

                    #region Hook Events

                    _MyItemsControl.PreviewMouseLeftButtonDown +=
                        listView_PreviewMouseLeftButtonDown;
                    _MyItemsControl.PreviewMouseMove += listView_PreviewMouseMove;
                    if (MyScrollViewer == null)
                    {
                        _MyItemsControl.PreviewDragOver += listView_PreviewDragOver;
                        _MyItemsControl.Drop += listView_Drop;
                        _MyItemsControl.DragLeave += listView_DragLeave;
                        _MyItemsControl.DragEnter += listView_DragEnter;
                    }
                    else
                    {
                        MyScrollViewer.PreviewDragOver += listView_PreviewDragOver;
                        MyScrollViewer.DragOver += MyScrollViewer_DragOver;
                        MyScrollViewer.Drop += listView_Drop;
                        MyScrollViewer.DragLeave += listView_DragLeave;
                        MyScrollViewer.DragEnter += listView_DragEnter;
                    }

                    #endregion // Hook Events
                }
            }
        }

        private void MyScrollViewer_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
            //HandleEventDragOver(e);
        }

        #endregion // ListView

        #region ProcessDrop [event]

        /// <summary>
        /// Raised when a drop occurs.  By default the dropped item will be moved
        /// to the target index.  Handle this event if relocating the dropped item
        /// requires custom behavior.  Note, if this event is handled the default
        /// item dropping logic will not occur.
        /// </summary>
        public event EventHandler<ProcessDropEventArgs<ClockVM>> ProcessDrop;

        #endregion // ProcessDrop [event]

        #region ShowDragAdorner

        protected bool showDragAdorner;
        /// <summary>
        /// Gets/sets whether a visual representation of the ListViewItem being dragged
        /// follows the mouse cursor during a drag operation.  The default value is true.
        /// </summary>
        public bool ShowDragAdorner
        {
            get
            {
                return showDragAdorner;
            }
            set
            {
                if (IsDragInProgress)
                {
                    throw new InvalidOperationException(
                        "Cannot set the ShowDragAdorner property during a drag operation.");
                }

                showDragAdorner = value;
            }
        }

        #endregion // ShowDragAdorner

        #endregion // Public Interface

        #region Event Handling Methods

        #region listView_PreviewMouseLeftButtonDown

        void listView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsMouseOverScrollbar)
            {
                // 4/13/2007 - Set the flag to false when cursor is over scrollbar.
                canInitiateDrag = false;
                return;
            }

            int index = IndexUnderDragCursor;
            canInitiateDrag = index > -1;

            if (canInitiateDrag)
            {
                // Remember the location and index of the ClockUserControl the user clicked on for later.
                ptMouseDown = e.GetPosition(MyScrollViewer);//MouseUtilities.GetMousePosition(MyScrollViewer);
                indexToSelect = index;
            }
            else
            {
                ptMouseDown = new Point(-10000, -10000);
                indexToSelect = -1;
            }
        }

        #endregion // listView_PreviewMouseLeftButtonDown

        #region listView_PreviewMouseMove

        internal List<ClockVM> DraggedClocks;
        internal List<FilterDisplay> DraggedFilterDisplays;

        void listView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!CanStartDragOperation)
            {
                return;
            }

            // dragging clocks
            if (MyItemsControl.ItemsSource is ObservableCollection<ClockVM>)
            {
                bool dragFromDragHandler = Utils.HasAsParent<DragCheckHandleUserControl>(e.OriginalSource as DependencyObject);
                bool clickedOnClockUserControl = Utils.GetVisualParent<ClockUserControl>(e.OriginalSource as DependencyObject) != null;

                if (!dragFromDragHandler && clickedOnClockUserControl)
                {
                    return;
                }
            }
            // dragging filters
            else
            {
                //if (!Utils.HasAsParent<DragCheckHandleUserControl>(e.OriginalSource as DependencyObject) &&
                //    Utils.GetVisualParent<ClockUserControl>(e.OriginalSource as DependencyObject) != null)
                //{
                //    return;
                //}
            }

            //// Select the item the user clicked on.
            //if( this.listView.SelectedIndex != this.indexToSelect )
            //	this.listView.SelectedIndex = this.indexToSelect;

            //// If the item at the selected index is null, there's nothing
            //// we can do, so just return;
            //if( this.listView.SelectedItem == null )
            //	return;


            UIElement itemToDrag;

            Point pos = MouseUtilities.GetMousePosition(MyItemsControl);




            int visibleCount;
            ObservableCollection<ClockVM> cc = null;
            ObservableCollection<FilterDisplay> fc = null;
            if (MyItemsControl.ItemsSource is ObservableCollection<ClockVM> cdc)
            {
                cc = cdc;
                visibleCount = cc.Count(x => !x.FilteredOut);
            }
            else if (MyItemsControl.ItemsSource is ObservableCollection<FilterDisplay> ocfd)
            {
                fc = ocfd;
                visibleCount = fc.Count;
            }
            else
            {
                throw new Exception(
                    "An ItemsControl managed by ItemsControlDragDropManager must have its ItemsSource set to an ObservableCollection<ClockVM> || ObservableCollection<FilterDisplay>.");
            }

            if (cc != null)
            {
                itemToDrag = MyItemsControl.GetContainerAtPoint<ClockUserControl>(pos);

                if (itemToDrag == null)
                {
                    return;
                }



                var cd = (ClockVM)MyItemsControl.GetObjectAtPoint<ClockUserControl>(pos);


                DraggedClocks.Clear();
                bool thisAdded = false;
                var clocksPanel = Utils.
                    GetVisualParent<ClockFlowLayoutPanel>(MyScrollViewer) as
                        ClockFlowLayoutPanel;
                if (clocksPanel.MultiSelectClocks)
                {
                    foreach (ClockVM cd2 in (MyItemsControl.ItemsSource as ObservableCollection<ClockVM>))
                    {
                        if (cd2.Checked)
                        {
                            DraggedClocks.Add(cd2);
                            if (ReferenceEquals(cd2, cd))
                            {
                                thisAdded = true;
                            }
                        }
                    }
                }
                // check for null because of debugging (or because dragging from empty space?)
                if (!thisAdded && cd != null)
                {
                    DraggedClocks.Add(cd);
                    cd.Checked = true;
                }

                if (DraggedClocks.Count == 0)
                {
                    return;
                }
            }
            else
            {
                itemToDrag = MyItemsControl.GetContainerAtPoint<ListViewItem>(pos);

                if (itemToDrag == null)
                    return;



                var cd = (FilterDisplay)MyItemsControl.GetObjectAtPoint<ListViewItem>(pos);


                var source = MyItemsControl.ItemsSource as ObservableCollection<FilterDisplay>;

                DraggedFilterDisplays.Clear();
                bool thisAdded = false;
                foreach (FilterDisplay cd2 in source)
                {
                    if (cd2.IsSelected && source.IndexOf(cd2) >= 5)
                    {
                        DraggedFilterDisplays.Add(cd2);
                        if (ReferenceEquals(cd2, cd))
                        {
                            thisAdded = true;
                        }
                    }
                }


                if (!thisAdded && source.IndexOf(cd) >= 5)
                {
                    DraggedFilterDisplays.Add(cd);
                    cd.IsSelected = true;
                }

                if (DraggedFilterDisplays.Count == 0)
                {
                    return;
                }
            }
            

            AdornerLayer adornerLayer = ShowDragAdornerResolved ?
                InitializeAdornerLayer(itemToDrag) : null;

            InitializeDragOperation(itemToDrag);
            PerformDragOperation(new Action(() =>
            {
                FinishDragOperation(itemToDrag, adornerLayer);
            }));
        }

        #endregion // listView_PreviewMouseMove

        #region listView_PreviewDragOver

        internal bool _HandleDragOver = true;
        public bool HandleDragOver
        {
            get
            {
                return _HandleDragOver;
            }
            set
            {
                if (_HandleDragOver != value)
                {
                    _HandleDragOver = value;

                }
            }
        }

        void listView_PreviewDragOver(object sender, DragEventArgs e)
        {
            HandleEventDragOver(e);
        }

        internal void HandleEventDragOver(DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;

            //if (!DragDropService.GetIsScrollingWhileDragging(MyScrollViewer))
            //{
            //    // TODO: uncomment this, and make the initial value to not enter this branch initially
            //    DragDropService.SetIsScrollingWhileDragging(MyScrollViewer, true);
            //    return;
            //}

            // Update the item which is known to be currently under the drag cursor.
            int index = IndexUnderDragCursor;
            ItemUnderDragCursor = index < 0 ?
                null :
                MyItemsControl.Items[index];

            // ignore HandleDragOver because UpdateDragAdornerLocation
            // should be called also between scroll timer ticks:
            //if (ShowDragAdornerResolved /*&& HandleDragOver*/
            //                            /*&& !DragDropService.GetIsScrollingWhileDragging(MyScrollViewer)*/)
            //{
                UpdateDragAdornerLocation(DragDropService.InstancesData[MyScrollViewer].LinesDelta, e);
            //}
        }

        #endregion // listView_DragOver

        #region listView_DragLeave

        void listView_DragLeave(object sender, DragEventArgs e)
        {
            if (!IsMouseOver(MyScrollViewer))
            {
                if (ItemUnderDragCursor != null)
                {
                    ItemUnderDragCursor = null;
                }

                if (dragAdorner != null)
                {
                    dragAdorner.Visibility = Visibility.Collapsed;
                }
            }
        }

        #endregion // listView_DragLeave

        #region listView_DragEnter

        void listView_DragEnter(object sender, DragEventArgs e)
        {
            if (dragAdorner != null &&
                dragAdorner.Visibility != Visibility.Visible)
            {
                e.Effects = DragDropEffects.All;
                //DragDropService.SetIsScrollingWhileDragging(MyScrollViewer, false);

                // Update the location of the adorner
                // and then show it.
                UpdateDragAdornerLocation(0, e);

                dragAdorner.Visibility = Visibility.Visible;
            }
        }

        #endregion // listView_DragEnter

        #region listView_Drop

        private void listView_Drop(object sender, DragEventArgs e)
        {
            HandleDrop(sender, e);
        }

        internal async Task HandleDrop(object sender, DragEventArgs e)
        {
            if (ItemUnderDragCursor != null)
            {
                ItemUnderDragCursor = null;
            }

            e.Effects = DragDropEffects.None;

            int visibleCount;
            int newIndex;
            ObservableCollection<ClockVM> cc = null;
            ObservableCollection<FilterDisplay> fc = null;
            object data;
            if (MyItemsControl.ItemsSource is ObservableCollection<ClockVM> cdc)
            {
                cc = cdc;
                visibleCount = cc.Count(x => !x.FilteredOut);

                if (!e.Data.GetDataPresent(typeof(List<ClockVM>)))
                {
                    return;
                }

                // Get the data object which was dropped.
                data = e.Data.GetData(typeof(List<ClockVM>)) as List<ClockVM>;
                if (data == null)
                {
                    return;
                }

                newIndex = IndexUnderDragCursor < 0 ?
                    visibleCount - 1 :
                    IndexUnderDragCursor;
            }
            else if (MyItemsControl.ItemsSource is ObservableCollection<FilterDisplay> ocfd)
            {
                fc = ocfd;
                visibleCount = fc.Count;

                bool fdAvailable = e.Data.GetDataPresent(typeof(List<FilterDisplay>));
                bool cdAvailable = e.Data.GetDataPresent(typeof(List<ClockVM>));

                if (!fdAvailable && !cdAvailable)
                {
                    return;
                }

                if (fdAvailable)
                {
                    // Get the data object which was dropped.
                    data = e.Data.GetData(typeof(List<FilterDisplay>)) as List<FilterDisplay>;
                    if (data == null)
                    {
                        return;
                    }

                    newIndex = IndexUnderDragCursor < 0 ?
                        visibleCount - 1 :
                        Math.Max(5, IndexUnderDragCursor);
                }
                else if (cdAvailable)
                {
                    // Get the data object which was dropped.
                    data = e.Data.GetData(typeof(List<ClockVM>)) as List<ClockVM>;
                    if (data == null)
                    {
                        return;
                    }

                    //newIndex = IndexUnderDragCursor < 0 ?
                    //    visibleCount - 1 :
                    //    Math.Max(5, IndexUnderDragCursor);

                    var l = data as List<ClockVM>;

                    foreach (ClockVM cd in l)
                    {
                        if (IndexUnderDragCursor >= 5)
                        {
                            cd.GroupName = ocfd[IndexUnderDragCursor].DisplayString;
                        }
                        else
                        {
                            cd.GroupName = "";
                        }
                    }

                    // TODO: perfect the algorithm, use the algorithm from ClockDataCollection
                    // (move it in Utils):
                    //foreach (int i in oldsList)
                    //{
                    //    fc.Move(i, newIndex);
                    //}

                    e.Effects = DragDropEffects.Move;
                    return;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                throw new Exception(
                    "An ItemsControl managed by ItemsControlDragDropManager must have its ItemsSource set to an ClockDataCollection || ObservableCollection<FilterDisplay>.");
            }

            //if (newIndex < 0)
            //{
            //    // The drag started somewhere else, and our ItemsControl is empty
            //    // so make the new item the first in the list.
            //    if (itemsSource.Count == 0)
            //    {
            //        newIndex = 0;
            //    }

            //    // The drag started somewhere else, but our ItemsControl has items
            //    // so make the new item the last in the list.
            //    else if (oldIndex < 0)
            //    {
            //        newIndex = itemsSource.Count - 1;
            //    }

            //    // The user is trying to drop an item from our ListView into
            //    // our ListView, but the mouse is not over an item, so don't
            //    // let them drop it.
            //    // Update: let the user drop it at the end of the ItemsControl.
            //    else
            //    {
            //        newIndex = itemsSource.Count - 1;
            //    }
            //}

            // Dropping an item back onto itself is not considered an actual 'drop'.
            //if (oldIndex == newIndex)
            //    return;

            if (ProcessDrop != null)
            {
                // Let the client code process the drop.
                //var args = new ProcessDropEventArgs<ClockData>(itemsSource, data, oldIndex, newIndex, e.AllowedEffects);
                //ProcessDrop(this, args);
                //e.Effects = args.Effects;
            }
            else
            {
                if (cc != null)
                {
                    var l = data as List<ClockVM>;

                    IEnumerable<int> oldIndices = from cd in l
                                                  select cc.IndexOf(cd);
                    List<int> oldsList = oldIndices.ToList();

                    // Move the dragged data object from it's original index to the
                    // new index (according to where the mouse cursor is).  If it was
                    // not previously in the ListBox, then insert the item.
                    //if ( oldIndex > -1 )
                    await cc.MoveClocksFromIndicesToIndex(
                        oldsList, newIndex, cc.Count((ClockVM x) => !x.FilteredOut));
                    //else
                    //foreach (ClockData cd in data as List<ClockData>)
                    //{
                    //    itemsSource.Insert(newIndex, cd);
                    //}
                }
                else
                {
                    var l = data as List<FilterDisplay>;

                    IEnumerable<int> oldIndices = from cd in l
                                                  select fc.IndexOf(cd);
                    List<int> oldsList = oldIndices.ToList();

                    // Move the dragged data object from it's original index to the
                    // new index (according to where the mouse cursor is).  If it was
                    // not previously in the ListBox, then insert the item.
                    //if ( oldIndex > -1 )


                    // TODO: perfect the algorithm, use the algorithm from ClockDataCollection
                    // (move it in Utils):
                    foreach (int i in oldsList)
                    {
                        fc.Move(i, newIndex);
                    }


                    //else
                    //foreach (ClockData cd in data as List<ClockData>)
                    //{
                    //    itemsSource.Insert(newIndex, cd);
                    //}
                }


                // Set the Effects property so that the call to DoDragDrop will return 'Move'.
                e.Effects = DragDropEffects.Move;
            }
        }

        #endregion // listView_Drop

        #endregion // Event Handling Methods

        #region Private Helpers

        #region CanStartDragOperation

        bool CanStartDragOperation
        {
            get
            {
                if (Mouse.LeftButton != MouseButtonState.Pressed)
                    return false;

                if (!this.canInitiateDrag)
                    return false;

                if (this.indexToSelect == -1)
                    return false;

                if (!this.HasCursorLeftDragThreshold)
                    return false;

                return true;
            }
        }

        #endregion // CanStartDragOperation

        #region FinishDragOperation

        void FinishDragOperation(UIElement draggedItem, AdornerLayer adornerLayer)
        {
            if (DraggedClocks != null)
            {
                foreach (ClockVM cd in DraggedClocks)
                {
                    ClockUserControl uc = Utils.GetVisualChild<ClockUserControl>(MyItemsControl.ItemContainerGenerator.ContainerFromItem(cd));
                    // Let the ClockUserControl know that it is not being dragged anymore.
                    ListViewItemDragState.SetIsBeingDragged(uc, false);
                }
                DraggedClocks.Clear();
            }
            else
            {
                foreach (FilterDisplay fd in DraggedFilterDisplays)
                {
                    var uc = (ListViewItem)MyItemsControl.ItemContainerGenerator.ContainerFromItem(fd);
                    // Let the ClockUserControl know that it is not being dragged anymore.
                    ListViewItemDragState.SetIsBeingDragged(uc, false);
                }
                DraggedFilterDisplays.Clear();
            }

            IsDragInProgress = false;

            if (ItemUnderDragCursor != null)
            {
                ItemUnderDragCursor = null;
            }

            // Remove the drag adorner from the adorner layer.
            if (adornerLayer != null)
            {
                adornerLayer.Remove(dragAdorner);
                dragAdorner = null;
            }

            //var mw = Application.Current.MainWindow as MainWindow;

            //mw.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
            //{
            //    mw.VM.ClockVMCollection.Model.RefreshIDs();
            //}));
        }

        #endregion // FinishDragOperation

        #region GetListViewItem

        UIElement GetListViewItem(int index)
        {
            if (MyItemsControl.ItemContainerGenerator.Status !=
                GeneratorStatus.ContainersGenerated)
            {
                return null;
            }

            if (MyItemsControl.ItemsSource is ObservableCollection<FilterDisplay>)
            {
                return (ListViewItem)MyItemsControl.ItemContainerGenerator.ContainerFromIndex(index);
            }

            return Utils.GetVisualChild<ClockUserControl>(MyItemsControl.ItemContainerGenerator.ContainerFromIndex(index));
        }

        UIElement GetListViewItem(object dataItem)
        {
            if (MyItemsControl.ItemContainerGenerator.Status !=
                GeneratorStatus.ContainersGenerated)
            {
                return null;
            }

            if (MyItemsControl.ItemsSource is ObservableCollection<FilterDisplay>)
            {
                return (ListViewItem)MyItemsControl.ItemContainerGenerator.ContainerFromItem(dataItem);
            }

            return Utils.GetVisualChild<ClockUserControl>(MyItemsControl.ItemContainerGenerator.ContainerFromItem(dataItem));
        }

        #endregion // GetListViewItem

        #region HasCursorLeftDragThreshold

        bool HasCursorLeftDragThreshold
        {
            get
            {
                if (indexToSelect < 0)
                {
                    return false;
                }

                UIElement item = GetListViewItem(indexToSelect);
                Rect bounds = VisualTreeHelper.GetDescendantBounds(item);
                Point ptInItem = MyItemsControl.TranslatePoint(ptMouseDown, item);

                // In case the cursor is at the very top or bottom of the ListViewItem
                // we want to make the vertical threshold very small so that dragging
                // over an adjacent item does not select it.
                double topOffset = Math.Abs(ptInItem.Y);
                double btmOffset = Math.Abs(bounds.Height - ptInItem.Y);
                double vertOffset = Math.Min(topOffset, btmOffset);

                double width = SystemParameters.MinimumHorizontalDragDistance * 2;
                double height = Math.Min(
                    SystemParameters.MinimumVerticalDragDistance, vertOffset) * 2;
                var szThreshold = new Size(width, height);

                var rect = new Rect(ptMouseDown, szThreshold);
                rect.Offset(szThreshold.Width / -2, szThreshold.Height / -2);
                Point ptInListView = MouseUtilities.GetMousePosition(MyItemsControl);
                return !rect.Contains(ptInListView);
            }
        }

        #endregion // HasCursorLeftDragThreshold

        #region IndexUnderDragCursor

        /// <summary>
        /// Returns the index of the ListViewItem underneath the
        /// drag cursor, or -1 if the cursor is not over an item.
        /// </summary>
        protected int IndexUnderDragCursor
        {
            get
            {
                int index = -1;
                for (int i = 0; i < MyItemsControl.Items.Count; ++i)
                {
                    UIElement item = GetListViewItem(i);
                    if (IsMouseOver(item))
                    {
                        index = i;
                        break;
                    }
                }
                return index;
            }
        }

        #endregion // IndexUnderDragCursor

        #region InitializeAdornerLayer

        AdornerLayer InitializeAdornerLayer(UIElement itemToDrag)
        {
            // TODO: custom drag adorner for multiselection dragging.

            double zoom = itemToDrag is ClockUserControl ?
                ((ClockUserControl)itemToDrag).ZoomFactor : 1d;

            VisualBrush brush;


            int draggedCount;
            if (DraggedClocks != null)
            {
                draggedCount = DraggedClocks.Count;
            }
            else
            {
                draggedCount = DraggedFilterDisplays.Count;
            }

            if (draggedCount == 1)
            {
                // Create a brush which will paint the ListViewItem onto
                // a visual in the adorner layer.
                brush = new VisualBrush(itemToDrag);
            }
            else
            {
                var details = new List<string>();

                if (DraggedClocks != null)
                {
                    foreach (ClockVM cd in DraggedClocks)
                    {
                        details.Add(cd.Tag);
                    }
                }
                else
                {
                    foreach (FilterDisplay fd in DraggedFilterDisplays)
                    {
                        details.Add(fd.DisplayString);
                    }
                }

                var ccuToDrag = itemToDrag as ClockUserControl;

                // TODO: memoize:
                brush = new VisualBrush(new ClockUserControlPlaceholder()
                {
                    Text = draggedCount.ToString(),
                    Details = details//,
                    //DarkTheme = ccuToDrag.DarkTheme,
                    //RoundedCorners = ccuToDrag.RoundedCorners,
                    //ZoomFactor = ccuToDrag.ZoomFactor
                });
            }


            AdornerLayer layer = AdornerLayer.GetAdornerLayer(MyScrollViewer);
            //if (dragAdorner != null)
            //{
            //    layer.Remove(dragAdorner);
            //}

            // Create an element which displays the source item while it is dragged.
            dragAdorner = new DragAdorner(MyScrollViewer,
                new Size(
                    itemToDrag.RenderSize.Width * zoom,
                    itemToDrag.RenderSize.Height * zoom),
                brush);

            // Set the drag adorner's opacity.
            dragAdorner.Opacity = DragAdornerOpacity;

            layer.Add(dragAdorner);

            // Save the location of the cursor when the left mouse button was pressed.
            //ptMouseDown = MouseUtilities.GetMousePosition(MyItemsControl);

            return layer;
        }

        #endregion // InitializeAdornerLayer

        #region InitializeDragOperation

        void InitializeDragOperation(UIElement itemToDrag)
        {
            // Set some flags used during the drag operation.
            IsDragInProgress = true;
            canInitiateDrag = false;

            if (DraggedClocks != null)
            {
                foreach (ClockVM cd in DraggedClocks)
                {
                    ClockUserControl uc = Utils.GetVisualChild<ClockUserControl>(MyItemsControl.ItemContainerGenerator.ContainerFromItem(cd));

                    // Let the ListViewItem know that it is being dragged.
                    ListViewItemDragState.SetIsBeingDragged(uc, true);
                }
            }
            else
            {
                foreach (FilterDisplay fd in DraggedFilterDisplays)
                {
                    var uc = (ListViewItem)MyItemsControl.ItemContainerGenerator.ContainerFromItem(fd);

                    // Let the ListViewItem know that it is being dragged.
                    ListViewItemDragState.SetIsBeingDragged(uc, true);
                }
            }
        }

        #endregion // InitializeDragOperation

        #region IsMouseOver

        bool IsMouseOver(Visual target)
        {
            if (target == null)
            {
                return false;
            }

            // We need to use MouseUtilities to figure out the cursor
            // coordinates because, during a drag-drop operation, the WPF
            // mechanisms for getting the coordinates behave strangely.

            Rect bounds = VisualTreeHelper.GetDescendantBounds(target);
            Point mousePos = MouseUtilities.GetMousePosition(target);
            return bounds.Contains(mousePos);
        }

        #endregion // IsMouseOver

        #region IsMouseOverScrollbar

        /// <summary>
        /// Returns true if the mouse cursor is over a scrollbar in the ListView.
        /// </summary>
        bool IsMouseOverScrollbar
        {
            get
            {
                Point ptMouse = MouseUtilities.GetMousePosition(MyItemsControl);
                HitTestResult res = VisualTreeHelper.HitTest(MyItemsControl, ptMouse);
                if (res == null)
                {
                    return false;
                }

                DependencyObject depObj = res.VisualHit;
                while (depObj != null)
                {
                    if (depObj is ScrollBar)
                    {
                        return true;
                    }

                    // VisualTreeHelper works with objects of type Visual or Visual3D.
                    // If the current object is not derived from Visual or Visual3D,
                    // then use the LogicalTreeHelper to find the parent element.
                    if (depObj is Visual ||
                        depObj is System.Windows.Media.Media3D.Visual3D)
                    {
                        depObj = VisualTreeHelper.GetParent(depObj);
                    }
                    else
                    {
                        depObj = LogicalTreeHelper.GetParent(depObj);
                    }
                }

                return false;
            }
        }

        #endregion // IsMouseOverScrollbar

        #region ItemUnderDragCursor

        object ItemUnderDragCursor
        {
            get
            {
                return itemUnderDragCursor;
            }
            set
            {
                if (itemUnderDragCursor == value)
                {
                    return;
                }

                // The first pass handles the previous item under the cursor.
                // The second pass handles the new one.
                for (int i = 0; i < 2; ++i)
                {
                    if (i == 1)
                        itemUnderDragCursor = value;

                    if (itemUnderDragCursor != null)
                    {
                        UIElement listViewItem =
                            GetListViewItem(itemUnderDragCursor);
                        if (listViewItem != null)
                            ListViewItemDragState.SetIsUnderDragCursor(
                                listViewItem, i == 1);
                    }
                }
            }
        }

        #endregion // ItemUnderDragCursor

        #region PerformDragOperation

        void PerformDragOperation(Action finish)
        {
            MyScrollViewer.Dispatcher.BeginInvoke(new Action(() =>
            {
                StartDragDrop();
                finish();
            }));
        }

        internal void StartDragDrop()
        {
            Point p = MouseUtilities.GetMousePosition(MyItemsControl);

            DragDropEffects allowedEffects = DragDropEffects.Copy |
                DragDropEffects.Move |
                DragDropEffects.Link;

            DataObject obj;
            if (DraggedClocks != null)
            {
                obj = new DataObject(
                    typeof(List<ClockVM>).FullName,
                    DraggedClocks
                );
            }
            else
            {
                obj = new DataObject(
                    typeof(List<FilterDisplay>).FullName,
                    DraggedFilterDisplays
                );
            }

            //MyScrollViewer.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            //{
            try
            {
                if (DragDrop.DoDragDrop(
                    MyScrollViewer,
                    obj,
                    allowedEffects) != DragDropEffects.None)
                {
                    if (DraggedClocks != null)
                    {
                        ClockUserControl c =
                            MyItemsControl.GetContainerAtPoint<ClockUserControl>(p);
                        // FIXME: why can it be null?
                        if (c != null)
                        {
                            c.BringIntoView();
                            Keyboard.Focus(c);
                        }
                    }
                    else
                    {
                        ListViewItem c =
                            MyItemsControl.GetContainerAtPoint<ListViewItem>(p);
                        // FIXME: (as above) why can it be null?
                        if (c != null)
                        {
                            c.BringIntoView();
                            Keyboard.Focus(c);
                        }
                    }

                    // The item was dropped into a new location,
                    // so make it the new selected item.
                    //this.listView.SelectedItem = selectedItem;
                }
            }
            catch (System.Runtime.InteropServices.COMException)
            {

            }
            catch (System.Runtime.Serialization.SerializationException)
            {

            }
            //}));
        }

        #endregion // PerformDragOperation

        #region ShowDragAdornerResolved

        bool ShowDragAdornerResolved
        {
            get
            {
                return ShowDragAdorner && DragAdornerOpacity > 0.0;
            }
        }

        #endregion // ShowDragAdornerResolved

        #region UpdateDragAdornerLocation

        public UIElement itemBeingDragged = null;

        public void UpdateDragAdornerLocation(int lines = 0, DragEventArgs e = null)
        {
            if (dragAdorner != null)
            {
                itemBeingDragged = GetListViewItem(indexToSelect);

                // cursor position relative to the scroll viewer:
                Point ptCursor;
                if (e != null)
                {
                    ptCursor = MouseUtilities.GetMousePosition(MyScrollViewer);
                }
                else
                {
                    ptCursor = MouseUtilities.GetMousePosition(MyScrollViewer);
                    //return;
                }

                Point itemLoc = itemBeingDragged.TransformToAncestor(MyScrollViewer).Transform(new Point(0, 0));
                //Point itemLoc = itemBeingDragged.TranslatePoint(new Point(0, 0), MyScrollViewer);
                //Point absLoc = itemBeingDragged.TranslatePoint(new Point(0, 0), MyItemsControl);

                //ScrollBar verticalScrollBar = MyScrollViewer.Template.FindName("PART_VerticalScrollBar", MyScrollViewer) as ScrollBar;

                double left = itemLoc.X + ptCursor.X - ptMouseDown.X;
                // TODO: use ptMouseDown but relative to another element to make the dragging more smoothly:
                double top = itemLoc.Y + ptCursor.Y - ptMouseDown.Y;/*- MyScrollViewer.TranslatePoint(ptMouseDown, itemBeingDragged).Y*/;

                dragAdorner.SetOffsets(left, top);
            }
        }

        #endregion // UpdateDragAdornerLocation

        #endregion // Private Helpers
    }

    #endregion // ListViewDragDropManager

    #region ListViewItemDragState

    /// <summary>
    /// Exposes attached properties used in conjunction with the ListViewDragDropManager class.
    /// Those properties can be used to allow triggers to modify the appearance of ListViewItems
    /// in a ListView during a drag-drop operation.
    /// </summary>
    public static class ListViewItemDragState
    {
        #region IsBeingDragged

        /// <summary>
        /// Identifies the ListViewItemDragState's IsBeingDragged attached property.  
        /// This field is read-only.
        /// </summary>
        public static readonly DependencyProperty IsBeingDraggedProperty =
            DependencyProperty.RegisterAttached(
                "IsBeingDragged",
                typeof(bool),
                typeof(ListViewItemDragState),
                new UIPropertyMetadata(false));

        /// <summary>
        /// Returns true if the specified ListViewItem is being dragged, else false.
        /// </summary>
        /// <param name="item">The ListViewItem to check.</param>
        public static bool GetIsBeingDragged(UIElement item)
        {
            return (bool)item.GetValue(IsBeingDraggedProperty);
        }

        /// <summary>
        /// Sets the IsBeingDragged attached property for the specified ListViewItem.
        /// </summary>
        /// <param name="item">The ListViewItem to set the property on.</param>
        /// <param name="value">Pass true if the element is being dragged, else false.</param>
        internal static void SetIsBeingDragged(UIElement item, bool value)
        {
            item.SetValue(IsBeingDraggedProperty, value);
        }

        #endregion // IsBeingDragged

        #region IsUnderDragCursor

        /// <summary>
        /// Identifies the ListViewItemDragState's IsUnderDragCursor attached property.  
        /// This field is read-only.
        /// </summary>
        public static readonly DependencyProperty IsUnderDragCursorProperty =
            DependencyProperty.RegisterAttached(
                "IsUnderDragCursor",
                typeof(bool),
                typeof(ListViewItemDragState),
                new UIPropertyMetadata(false));

        /// <summary>
        /// Returns true if the specified ListViewItem is currently underneath the cursor 
        /// during a drag-drop operation, else false.
        /// </summary>
        /// <param name="item">The ListViewItem to check.</param>
        public static bool GetIsUnderDragCursor(UIElement item)
        {
            return (bool)item.GetValue(IsUnderDragCursorProperty);
        }

        /// <summary>
        /// Sets the IsUnderDragCursor attached property for the specified ListViewItem.
        /// </summary>
        /// <param name="item">The ListViewItem to set the property on.</param>
        /// <param name="value">Pass true if the element is underneath the drag cursor, else false.</param>
        internal static void SetIsUnderDragCursor(UIElement item, bool value)
        {
            item.SetValue(IsUnderDragCursorProperty, value);
        }

        #endregion // IsUnderDragCursor
    }

    #endregion // ListViewItemDragState

    #region ProcessDropEventArgs

    /// <summary>
    /// Event arguments used by the ListViewDragDropManager.ProcessDrop event.
    /// </summary>
    /// <typeparam name="ItemType">The type of data object being dropped.</typeparam>
    public class ProcessDropEventArgs<ItemType> :
        EventArgs where ItemType : class
    {
        #region Data

        ClockVMCollection itemsSource;
        List<ItemType> dataItem;
        int oldIndex;
        int newIndex;
        DragDropEffects allowedEffects = DragDropEffects.None;
        DragDropEffects effects = DragDropEffects.None;

        #endregion // Data

        #region Constructor

        internal ProcessDropEventArgs(
            ClockVMCollection itemsSource,
            List<ItemType> dataItem,
            int oldIndex,
            int newIndex,
            DragDropEffects allowedEffects)
        {
            this.itemsSource = itemsSource;
            this.dataItem = dataItem;
            this.oldIndex = oldIndex;
            this.newIndex = newIndex;
            this.allowedEffects = allowedEffects;
        }

        #endregion // Constructor

        #region Public Properties

        /// <summary>
        /// The items source of the ListView where the drop occurred.
        /// </summary>
        public ClockVMCollection ItemsSource
        {
            get { return itemsSource; }
        }

        /// <summary>
        /// The data object which was dropped.
        /// </summary>
        public List<ItemType> DataItem
        {
            get { return dataItem; }
        }

        /// <summary>
        /// The current index of the data item being dropped, in the ItemsSource collection.
        /// </summary>
        public int OldIndex
        {
            get { return oldIndex; }
        }

        /// <summary>
        /// The target index of the data item being dropped, in the ItemsSource collection.
        /// </summary>
        public int NewIndex
        {
            get { return newIndex; }
        }

        /// <summary>
        /// The drag drop effects allowed to be performed.
        /// </summary>
        public DragDropEffects AllowedEffects
        {
            get { return allowedEffects; }
        }

        /// <summary>
        /// The drag drop effect(s) performed on the dropped item.
        /// </summary>
        public DragDropEffects Effects
        {
            get { return effects; }
            set { effects = value; }
        }

        #endregion // Public Properties
    }

    #endregion // ProcessDropEventArgs
}