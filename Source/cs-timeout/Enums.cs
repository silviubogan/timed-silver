using System;

namespace cs_timed_silver
{
    // TODO: [BIGGEST] d&d does not work perfectly well in List View.
    public enum EasyViewType
    {
        List,
        DataGrid,
        HorizontalSplit,
        VerticalSplit
    }

    [Obsolete("Used just in tests.")]
    public enum ViewDataType
    {
        List,
        DataGrid,
        GroupList,
        Splitter
    }

    public enum AutosortMode
    {
        None,
        ClosestRingingMoment,
        Alphabetically
    }

    public enum LogCategory
    {
        Information,
        Error,
        None
    }

    public enum AppTheme
    {
        Light,
        Dark
    }
}
