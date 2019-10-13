using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace cs_timed_silver
{
    internal interface IClocksViewProvider
    {
        Pointer RootSplitterViewPointer { get; set; }

        bool HandleSplitterMoved { get; set; }

        bool EasyGroupListViewVisible { get; set; }

        decimal EasyClockListViewZoomPercent { get; set; }
        decimal EasyClockDataGridViewZoomPercent { get; set; }
        decimal EasyClockGroupListViewZoomPercent { get; set; }

        ClockFlowLayoutPanel GetExistingOrNewClockListView();
        ClockDataGrid GetExistingOrNewClockDataGridView();
        ClockGroupListView GetExistingOrNewClockGroupListView();

        ClockFlowLayoutPanel CreateNewTimerListView();
        ClockDataGrid CreateNewTimerDataGridView();
        ClockGroupListView CreateNewGroupListView();

        //List<IClocksView> GetAllExistingClocksViews(ViewDataFactory factory);

        void SubscribeSplitterMovedHandlers();
        void UnsubscribeSplitterMovedHandlers();

        void ForceUpdateViewType_List(bool showGlv);
        void ForceUpdateViewType_DataGrid(bool showGlv);
        void ForceUpdateViewType_Split(Orientation value, bool showGlv);

        void ToggleGroupListView();
        void HideGroupList(bool isSplit);
        void ShowGroupList_Split();
        void ShowGroupList_ListOrDataGrid();

        void cdgv_Scroll(object sender, System.Windows.Controls.ScrollChangedEventArgs e);
        void clv_Scroll(object sender, System.Windows.Controls.ScrollChangedEventArgs e);
        void cglv_Scroll(object sender, System.Windows.Controls.ScrollChangedEventArgs e);

        void Sv_SplitterMoved(object sender, SplitterEventArgs e);
    }
}