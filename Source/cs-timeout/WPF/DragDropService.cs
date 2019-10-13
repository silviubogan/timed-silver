using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace cs_timed_silver
{
    public class DragDropService
    {
        public enum ScrollingState
        {
            Up,
            Down,
            None
        }

        public class InstanceData
        {
            public ScrollingState ScrollingState;
            public DispatcherTimer DraggingTimer;
            public bool ProcessingATick = false;
            public int LinesDelta;
            public bool HandleScrollChanged;
        }

        internal static void BeginScrollingWhileDragging(ScrollViewer s)
        {
            SetIsScrollingWhileDragging(s, true);
            InstancesData[s].DraggingTimer.Start();
        }

        internal static void EndScrollingWhileDragging(ScrollViewer s)
        {
            InstancesData[s].DraggingTimer.Stop();
            SetIsScrollingWhileDragging(s, false);
        }

        internal static Dictionary<ScrollViewer, InstanceData> InstancesData =
            new Dictionary<ScrollViewer, InstanceData>();

        private static void OnScrollWhileDraggingChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var s = d as ScrollViewer;
            if (s == null)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                // register
                InstancesData[s] = new InstanceData()
                {
                    ScrollingState = ScrollingState.None,
                    DraggingTimer = new DispatcherTimer(DispatcherPriority.Input)
                    {
                        Interval = TimeSpan.FromMilliseconds(200),
                        Tag = s
                    },
                    LinesDelta = 0,
                    HandleScrollChanged = false
                };

                //s.DragEnter += S_DragEnter;
                //s.DragLeave += S_DragLeave;
                s.PreviewDragOver += S_DragOver;
                s.Drop += S_Drop;
                s.ScrollChanged += S_ScrollChanged;

                InstancesData[s].DraggingTimer.Tick += MyDragTimer_Tick;

                var dm = s.Tag as ListViewDragDropManager;
                if (dm != null)
                {
                    dm.HandleDragOver = true;
                }
            }
            else
            {
                // unregister
                if (InstancesData.ContainsKey(s))
                {
                    EndScrollingWhileDragging(s);
                    InstancesData.Remove(s);
                }

                var dm = s.Tag as ListViewDragDropManager;
                if (dm != null)
                {
                    dm.HandleDragOver = true;
                }
            }
        }

        internal bool HandleScrollChanged = false;

        private static void S_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var s = sender as ScrollViewer;
            var dm = s.Tag as ListViewDragDropManager;

            if (!InstancesData[s].HandleScrollChanged)
            {
                return;
            }

            if (dm != null) dm.HandleDragOver = true;

            //EndScrollingWhileDragging(s);

            //InstancesData[s].LinesDelta = 0;

            InstanceData id = InstancesData[s];

            if (dm != null)
            {
                dm.UpdateDragAdornerLocation(id.LinesDelta);
            }

            var verticalScrollBar = s.Template.FindName("PART_VerticalScrollBar", s) as ScrollBar;

            if (dm != null && dm.itemBeingDragged != null)
            {
                double y = dm.itemBeingDragged.
               TranslatePoint(new Point(0, 0), dm.MyItemsControl).Y;

                bool scrolledDown = e.VerticalChange > 0d;
                bool scrolledUp = !scrolledDown;
                bool canScrollDown = y + verticalScrollBar.SmallChange <
                    s.ExtentHeight;
                //s.VerticalOffset + verticalScrollBar.SmallChange >=
                //s.ExtentHeight - s.ViewportHeight;
                bool canScrollUp = y - verticalScrollBar.SmallChange > 0d;//s.VerticalOffset - verticalScrollBar.SmallChange <=
                                                                          //0d;

                if ((scrolledDown && !canScrollDown) ||
                    (scrolledUp && !canScrollUp))
                {
                    EndScrollingWhileDragging(s);
                    id.LinesDelta = 0;
                    if (dm != null)
                    {
                        dm.HandleDragOver = true;
                    }
                }
            }
        }

        //private static void S_DragEnter(object sender, DragEventArgs e)
        //{
        //    var s = sender as ScrollViewer;

        //    //if (e.Source != s)
        //    //{
        //    //    return;
        //    //}

        //    var dm = s.Tag as ListViewDragDropManager;
        //    dm.HandleDragOver = false;


        //    InstancesData[s].LinesDelta = 0;

        //    InstancesData[s].DraggingTimer.Start();
        //}

        private static void S_Drop(object sender, DragEventArgs e)
        {
            var s = sender as ScrollViewer;

            //if (e.Source != s)
            //{
            //    return;
            //}

            var dm = s.Tag as ListViewDragDropManager;
            dm.HandleDragOver = true;
            EndScrollingWhileDragging(s);
        }

        private static void S_DragOver(object sender, DragEventArgs e)
        {
            var s = sender as ScrollViewer;

            //if (e.Source != s)
            //{
            //    return;
            //}

            var dm = s.Tag as ListViewDragDropManager;
            dm.HandleDragOver = false;
            BeginScrollingWhileDragging(s);
        }

        //private static void S_DragLeave(object sender, DragEventArgs e)
        //{
        //    var s = sender as ScrollViewer;

        //    //if (e.Source != s)
        //    //{
        //    //    return;
        //    //}

        //    var dm = s.Tag as ListViewDragDropManager;
        //    dm.HandleDragOver = true;

        //    InstancesData[s].DraggingTimer.Stop();
        //}

        #region ScrollWhileDraggingProperty

        public static readonly DependencyProperty ScrollWhileDraggingProperty = DependencyProperty.RegisterAttached(
            "ScrollWhileDragging",
            typeof(bool),
            typeof(DragDropService),
            new FrameworkPropertyMetadata(false,
                FrameworkPropertyMetadataOptions.AffectsRender, OnScrollWhileDraggingChanged)
            );

        public static void SetScrollWhileDragging(UIElement element, bool value)
        {
            element.SetValue(ScrollWhileDraggingProperty, value);
        }
        public static bool GetScrollWhileDragging(UIElement element)
        {
            return (bool)element.GetValue(ScrollWhileDraggingProperty);
        }

        #endregion

        #region IsScrolllingWhileDraggingProperty

        public static readonly DependencyProperty IsScrolllingWhileDraggingProperty = DependencyProperty.RegisterAttached(
            "IsScrolllingWhileDragging",
            typeof(bool),
            typeof(DragDropService),
            new FrameworkPropertyMetadata(false));

        public static void SetIsScrollingWhileDragging(UIElement element, bool value)
        {
            element.SetValue(IsScrolllingWhileDraggingProperty, value);
        }

        public static bool GetIsScrollingWhileDragging(UIElement element)
        {
            return (bool)element.GetValue(IsScrolllingWhileDraggingProperty);
        }

        #endregion

        internal static ScrollViewer ScrollViewerForTimer(DispatcherTimer d)
        {
            foreach (KeyValuePair<ScrollViewer, InstanceData> id in InstancesData)
            {
                if (id.Value.DraggingTimer == d)
                {
                    return id.Key;
                }
            }
            return null;
        }

        internal static ScrollingState GetScrollingStateForScrollViewer(ScrollViewer s)
        {
            double scrollZoneHeight = s.ActualHeight / 7;
            Point point = MouseUtilities.GetMousePosition(s);

            if (point.Y < scrollZoneHeight)
            {
                return ScrollingState.Up;
            }
            else if (point.Y > s.ActualHeight - scrollZoneHeight)
            {
                return ScrollingState.Down;
            }
            else
            {
                return ScrollingState.None;
            }
        }

        private static void MyDragTimer_Tick(object sender, EventArgs e)
        {
            ScrollViewer s = ScrollViewerForTimer(sender as DispatcherTimer);
            var dm = s.Tag as ListViewDragDropManager;
            InstanceData id = InstancesData[s];
            //dm.HandleDragOver = false;
            //return;

            if (id.ProcessingATick)
            {
                return;
            }
            id.ProcessingATick = true;

            //bool starting = id.ScrollingState == ScrollingState.None;
            //if (starting)
            //{
            //    dm.HandleDragOver = false;
            //}

            id.ScrollingState = GetScrollingStateForScrollViewer(s);

            if (id.ScrollingState == ScrollingState.Up)
            {
                //if (starting)
                //{
                //    id.LinesDelta = 0;
                //}

                //double offset = s.VerticalOffset;

                if (s.VerticalOffset > 0d)
                {
                    id.LinesDelta++;
                    id.HandleScrollChanged = true;
                    s.LineUp();
                    id.HandleScrollChanged = false;
                }
                else
                {
                    StopTimer(s, dm, id);
                }

                //double offset2 = s.VerticalOffset;

                //if (offset != offset2)
                //{
                //    id.LinesDelta++;
                //}
            }
            else if (id.ScrollingState == ScrollingState.Down)
            {
                //if (starting)
                //{
                //    id.LinesDelta = 0;
                //}

                //double offset = s.VerticalOffset;

                if (s.VerticalOffset < s.ExtentHeight - s.ViewportHeight)
                {
                    id.LinesDelta--;
                    id.HandleScrollChanged = true;
                    s.LineDown();
                    id.HandleScrollChanged = false;
                }
                else
                {
                    StopTimer(s, dm, id);
                }

                //double offset2 = s.VerticalOffset;

                //if (offset != offset2)
                //{
                //    id.LinesDelta--;
                //}
            }
            else
            {
                StopTimer(s, dm, id);
            }

            // TODO: do a link to the Drag Manager class better than the Tag

            id.ProcessingATick = false;
        }

        private static void StopTimer(ScrollViewer s,
            ListViewDragDropManager dm, InstanceData id)
        {
            EndScrollingWhileDragging(s);
            dm.HandleDragOver = true;
            id.LinesDelta = 0;
            id.ScrollingState = GetScrollingStateForScrollViewer(s);
        }
    }
}
