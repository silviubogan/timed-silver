using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace cs_timed_silver
{
    public static class CustomCommands
    {
        public static readonly RoutedUICommand OpenContainingFolder = new RoutedUICommand
        (
            "_Open Containing Folder...",
            "OpenContainingFolder",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.O, ModifierKeys.Control | ModifierKeys.Shift)
            }
        );

        public static readonly RoutedUICommand RestoreToPreviousDay = new RoutedUICommand
        (
            "Restore to _Previous Day...",
            "RestoreToPreviousDay",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
            }
        );

        public static readonly RoutedUICommand ClearOpenFile = new RoutedUICommand
        (
            "_Clear Open File...",
            "ClearOpenFile",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
            }
        );

        public static readonly RoutedUICommand ReloadOpenFileFromFileSystem = new RoutedUICommand
        (
            "Reload _Open File from File System...",
            "ReloadOpenFileFromFileSystem",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
            }
        );

        public static readonly RoutedUICommand Import = new RoutedUICommand
        (
            "_Import...",
            "_Import",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
            }
        );

        public static readonly RoutedUICommand Exit = new RoutedUICommand
        (
            "E_xit",
            "Exit",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.Q, ModifierKeys.Control)
            }
        );

        public static readonly RoutedUICommand RemoveAllClocks = new RoutedUICommand
        (
            "_Remove All Clocks...",
            "RemoveAllClocks",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
            }
        );

        public static readonly RoutedUICommand MultipleSelectionInListView = new RoutedUICommand
        (
            "_Multiple Selection in List View",
            "MultipleSelectionInListView",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
            }
        );

        public static readonly RoutedUICommand ListView = new RoutedUICommand
        (
            "_List View",
            "ListView",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
            }
        );

        public static readonly RoutedUICommand DataGridView = new RoutedUICommand
        (
            "_Data Grid View",
            "DataGridView",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
            }
        );

        public static readonly RoutedUICommand HorizontalSplitView = new RoutedUICommand
        (
            "_Horizontal Split View",
            "HorizontalSplitView",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
            }
        );

        public static readonly RoutedUICommand VerticalSplitView = new RoutedUICommand
        (
            "_Vertical Split View",
            "VerticalSplitView",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
            }
        );

        public static readonly RoutedUICommand ShowGroupListView = new RoutedUICommand
        (
            "Show _Group List View",
            "ShowGroupListView",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
            }
        );

        public static readonly RoutedUICommand FocusListView = new RoutedUICommand
        (
            "_Focus List View",
            "FocusListView",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
            }
        );

        public static readonly RoutedUICommand FocusDataGridView = new RoutedUICommand
        (
            "F_ocus Data Grid View",
            "FocusDataGridView",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
            }
        );

        public static readonly RoutedUICommand FocusGroupListView = new RoutedUICommand
        (
            "Fo_cus Group List View",
            "FocusGroupListView",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
            }
        );

        public static readonly RoutedUICommand ShowToolBarZoomListView = new RoutedUICommand
        (
            "L_ist View Zoom",
            "ShowToolBarZoomListView",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
            }
        );

        public static readonly RoutedUICommand ShowToolBarZoomDataGridView = new RoutedUICommand
        (
            "D_ata Grid View Zoom",
            "ShowToolBarZoomDataGridView",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
            }
        );

        public static readonly RoutedUICommand ShowToolBarZoomGroupListView = new RoutedUICommand
        (
            "_Group List View Zoom",
            "ShowToolBarZoomGroupListView",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
            }
        );

        public static readonly RoutedUICommand ShowToolBarZoomGlobal = new RoutedUICommand
        (
            "Global _Zoom",
            "ShowToolBarZoomGlobal",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
            }
        );

        public static readonly RoutedUICommand ShowStatusBar = new RoutedUICommand
        (
            "Status _Bar",
            "ShowStatusBar",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
            }
        );

        public static readonly RoutedUICommand KeepOnTopAlways = new RoutedUICommand
        (
            "_Always",
            "KeepOnTopAlways",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
            }
        );

        public static readonly RoutedUICommand KeepOnTopUntilNextRestart = new RoutedUICommand
        (
            "_Until Next Restart",
            "KeepOnTopUntilNextRestart",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
            }
        );

        public static readonly RoutedUICommand KeepOnTopFor1Min = new RoutedUICommand
        (
            "_For 1 minute",
            "KeepOnTopFor1Min",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
            }
        );

        public static readonly RoutedUICommand KeepOnTopFor5Min = new RoutedUICommand
        (
            "_For 5 minutes",
            "KeepOnTopFor5Min",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
            }
        );

        public static readonly RoutedUICommand KeepOnTopFor15Min = new RoutedUICommand
        (
            "F_or 15 minutes",
            "KeepOnTopFor15Min",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
            }
        );

        public static readonly RoutedUICommand KeepOnTopFor1Hour = new RoutedUICommand
        (
            "Fo_r 1 hour",
            "KeepOnTopFor1Hour",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
            }
        );

        public static readonly RoutedUICommand About = new RoutedUICommand
        (
            "_About...",
            "About",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
            }
        );

        public static readonly RoutedUICommand StopAllTimers = new RoutedUICommand
        (
            "_Stop All Timers...",
            "StopAllTimers",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
            }
        );
    }
}
