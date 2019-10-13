using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace cs_timed_silver
{
    public class ScrollDataGrid : DataGrid
    {
        /// <summary>
        /// The vertical scrollbar.
        /// </summary>
        internal ScrollBar MyVerticalScrollBar;

        /// <summary>
        /// The horizontal scrollbar.
        /// </summary>
        internal ScrollBar MyHorizontalScrollBar;

        /// <summary>
        /// Position of the vertical scrollbar we saved.
        /// </summary>
        private double savedVerticalScrollPosition;

        /// <summary>
        /// Position of the horizontal scrollbar we saved.
        /// </summary>
        private double savedHorizontalScrollPosition;

        public event ScrollChangedEventHandler ScrollChanged;
        
        /// <summary>
        /// Event for each vertical scroll.
        /// </summary>
        public event EventHandler<ScrollEventArgs> VerticalScroll;

        /// <summary>
        /// Event for each horizontal scroll.
        /// </summary>
        public event EventHandler<ScrollEventArgs> HorizontalScroll;

        /// <summary>
        /// Load the scrollbars after the template gets loaded.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.LoadScrollBars();
        }

        public ScrollViewer MyScrollViewer;

        /// <summary>
        /// Get both scrollbars.
        /// </summary>
        private void LoadScrollBars()
        {
            MyScrollViewer = this.Template.FindName("DG_ScrollViewer", this) as ScrollViewer;

            if (MyScrollViewer != null)
            {
                MyScrollViewer.ScrollChanged += MyScrollViewer_ScrollChanged;
                MyScrollViewer.CanContentScroll = false;
            }

            //var dg = ;

            //var l = new List<ScrollBar>();
            //ScrollBar vsb = Utils.FindChild<ScrollBar>(dg, "PART_VerticalScrollBar") as ScrollBar;

            //MyVerticalScrollBar = dg.Template.FindName("PART_VerticalScrollBar", dg) as ScrollBar;
            //if (MyVerticalScrollBar != null)
            //    MyVerticalScrollBar.Scroll += new ScrollEventHandler(OnVerticalScroll);
            //MyHorizontalScrollBar = this.GetTemplateChild("PART_HorizontalScrollBar") as ScrollBar;
            //if (MyHorizontalScrollBar != null)
            //    MyHorizontalScrollBar.Scroll += new ScrollEventHandler(OnHorizontalScroll);
        }

        private void MyScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollChanged?.Invoke(sender, e);
        }

        /// <summary>
        /// Notify that we are scrolling vertically.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnVerticalScroll(object sender, ScrollEventArgs e)
        {
            if (VerticalScroll != null)
                VerticalScroll(sender, e);
        }

        /// <summary>
        /// Notify that we are scrolling horizontally.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnHorizontalScroll(object sender, ScrollEventArgs e)
        {
            if (HorizontalScroll != null)
                HorizontalScroll(sender, e);
        }
        
        /// <summary>
        /// Save the current scroll position.
        /// </summary>
        /// <param name="mode"></param>
        public void SaveScrollPosition(ScrollMode mode)
        {
            switch (mode)
            {
                case ScrollMode.Vertical:
                    this.savedVerticalScrollPosition = MyVerticalScrollBar.Value;
                    break;
                case ScrollMode.Horizontal:
                    this.savedHorizontalScrollPosition = MyHorizontalScrollBar.Value;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Reload the scroll position that was saved before.
        /// </summary>
        /// <param name="mode"></param>
        public void ReloadScrollPosition(ScrollMode mode)
        {
            switch (mode)
            {
                case ScrollMode.Vertical:
                    this.Scroll(ScrollMode.Vertical, savedVerticalScrollPosition);
                    break;
                case ScrollMode.Horizontal:
                    this.Scroll(ScrollMode.Horizontal, savedHorizontalScrollPosition);
                    break;
            }
        }

        public DataGridScrollExtensions.ScrollInfo GetScrollInfo()
        {
            LoadScrollBars();
            DataGridScrollExtensions.ScrollInfo oInfo = new DataGridScrollExtensions.ScrollInfo();

            //ScrollBar sbHorizontal = grid.GetScrollbar(ScrollMode.Horizontal);
            //oInfo.HorizontalMaximum = sbHorizontal.Maximum;
            //oInfo.HorizontalPosition = sbHorizontal.Value;

            ScrollBar sbVertical = MyVerticalScrollBar;
            oInfo.VerticalMaximum = MyScrollViewer.ExtentHeight;
            oInfo.VerticalPosition = MyScrollViewer.VerticalOffset;

            return oInfo;
        }

        public void SetScrollPosition(
            DataGridScrollExtensions.ScrollInfo info)
        {
            LoadScrollBars();
            //if (info.HorizontalPosition > 0)
            //{
            //    ScrollBar sbHorizontal = grid.GetScrollbar(ScrollMode.Horizontal);
            //    sbHorizontal.Maximum = info.HorizontalMaximum;
            //    grid.Scroll(ScrollMode.Horizontal, info.HorizontalPosition);
            //}

            if (info.VerticalPosition > 0)
            {
                //ScrollBar sbVertical = MyVerticalScrollBar;
                //sbVertical.Maximum = ;
                //this.Scroll(ScrollMode.Vertical, );


                MyScrollViewer.ScrollToVerticalOffset(info.VerticalPosition);
            }
        }

        public ScrollDataGrid()
        {
            Loaded += ScrollDataGrid_Loaded;
        }

        private void ScrollDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            this.LoadScrollBars();
        }
    }
}
