using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls.Primitives;

namespace cs_timed_silver
{
    public enum ScrollMode
    {
        Vertical,
        Horizontal
    }

    // TODO: move the relevant code below to ScrollDataGrid class.
    public static class DataGridScrollExtensions
    {
        /// <summary>
        /// Scroll to the start of the ScrollBar.
        /// <param name="mode"></param>
        public static void ScrollToStart(this DataGrid grid, ScrollMode mode)
        {
            switch (mode)
            {
                case ScrollMode.Vertical:
                    grid.ScrollToPercent(ScrollMode.Vertical, 0);
                    break;
                case ScrollMode.Horizontal:
                    grid.ScrollToPercent(ScrollMode.Horizontal, 0);
                    break;
            }
        }

        /// <summary>
        /// Scroll to the end of the ScrollBar.
        /// </summary>
        /// <param name="mode"></param>
        public static void ScrollToEnd(this DataGrid grid, ScrollMode mode)
        {
            switch (mode)
            {
                case ScrollMode.Vertical:
                    grid.ScrollToPercent(ScrollMode.Vertical, 100);
                    break;
                case ScrollMode.Horizontal:
                    grid.ScrollToPercent(ScrollMode.Horizontal, 100);
                    break;
            }
        }

        /// <summary>
        /// Scroll to a percentage of the scrollbar (50% = half).
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="percent"></param>
        public static void ScrollToPercent(this DataGrid grid, ScrollMode mode, double percent)
        {
            // Fix the percentage.
            if (percent < 0)
                percent = 0;
            else if (percent > 100)
                percent = 100;

            // Get the scroll provider.
            var scrollProvider = GetScrollProvider(grid);

            // TODO: Use the ScrollViewer from the template.
            throw new NotImplementedException();

            // Scroll.
            //switch (mode)
            //{
            //    case ScrollMode.Vertical:
            //        scrollProvider.SetScrollPercent(System.Windows.Automation.ScrollPatternIdentifiers.NoScroll, percent);
            //        break;
            //    case ScrollMode.Horizontal:
            //        scrollProvider.SetScrollPercent(percent, System.Windows.Automation.ScrollPatternIdentifiers.NoScroll);
            //        break;
            //}
        }

        /// <summary>
        /// Get the current position of the scrollbar.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static double GetScrollPosition(this DataGrid grid, ScrollMode mode)
        {
            var scrollBar = grid.GetScrollbar(mode);
            return scrollBar.Value;
        }

        /// <summary>
        /// Get the maximum position of a scrollbar.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static double GetScrollMaximum(this DataGrid grid, ScrollMode mode)
        {
            var scrollBar = grid.GetScrollbar(mode);
            return scrollBar.Maximum;
        }

        /// <summary>
        /// Scroll to a position of the scrollbar.
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="position"></param>
        public static void Scroll(this DataGrid grid, ScrollMode mode, double position)
        {
            // Get the scrollbar and convert the position to percent.
            var scrollBar = grid.GetScrollbar(mode);
            double positionPct = ((position / scrollBar.Maximum) * 100);

            // Scroll to a specfic percentage of the scrollbar.
            grid.ScrollToPercent(mode, positionPct);
        }

        /// <summary>
        /// Get a scroll provider for the grid.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static IScrollProvider GetScrollProvider(DataGrid grid)
        {
            var p = FrameworkElementAutomationPeer.FromElement(grid) ?? FrameworkElementAutomationPeer.CreatePeerForElement(grid);
            return p.GetPattern(PatternInterface.Scroll) as IScrollProvider;
        }

        /// <summary>
        /// Get one of the grid's scrollbars.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static ScrollBar GetScrollbar(this DataGrid grid, ScrollMode mode)
        {
            if (mode == ScrollMode.Vertical)
                return grid.GetScrollbar("VerticalScrollbar");
            else
                return grid.GetScrollbar("HorizontalScrollbar");
        }
        
        /// <summary>
        /// Find the scrollbar for our datagrid.
        /// </summary>
        /// <param name="dep"></param>
        /// <returns></returns>
        private static ScrollBar GetScrollbar(this DependencyObject dep, string name)
        {
            var o = dep as ScrollDataGrid;
            if (name == "VerticalScrollbar")
            {
                return o.MyVerticalScrollBar;
                //return o.GetTemplateChild("VerticalScrollbar") as ScrollBar;
            }
            else
            {
                return o.MyHorizontalScrollBar;
                //return o.GetTemplateChild("HorizontalScrollbar") as ScrollBar;
            }
            //for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dep); i++)
            //{
            //    var child = VisualTreeHelper.GetChild(dep, i);
            //    if (child != null && child is ScrollBar && ((ScrollBar)child).Name == name)
            //        return child as ScrollBar;
            //    else
            //    {
            //        ScrollBar sub = GetScrollbar(child, name);
            //        if (sub != null)
            //            return sub;
            //    }
            //}
            return null;
        }

        public class ScrollInfo
        {
            //public double HorizontalPosition;
            //public double HorizontalMaximum;
            public double VerticalPosition;
            public double VerticalMaximum;
        }

        public static ScrollInfo GetScrollInfo(this DataGrid grid)
        {
            ScrollInfo oInfo = new ScrollInfo();

            //ScrollBar sbHorizontal = grid.GetScrollbar(ScrollMode.Horizontal);
            //oInfo.HorizontalMaximum = sbHorizontal.Maximum;
            //oInfo.HorizontalPosition = sbHorizontal.Value;

            ScrollBar sbVertical = grid.GetScrollbar(ScrollMode.Vertical);
            oInfo.VerticalMaximum = sbVertical.Maximum;
            oInfo.VerticalPosition = sbVertical.Value;

            return oInfo;
        }

        public static void SetScrollPosition(this DataGrid grid, ScrollInfo info)
        {
            //if (info.HorizontalPosition > 0)
            //{
            //    ScrollBar sbHorizontal = grid.GetScrollbar(ScrollMode.Horizontal);
            //    sbHorizontal.Maximum = info.HorizontalMaximum;
            //    grid.Scroll(ScrollMode.Horizontal, info.HorizontalPosition);
            //}

            if (info.VerticalPosition > 0)
            {
                ScrollBar sbVertical = grid.GetScrollbar(ScrollMode.Vertical);
                sbVertical.Maximum = info.VerticalMaximum;
                grid.Scroll(ScrollMode.Vertical, info.VerticalPosition);
            }
        }
    }
}
