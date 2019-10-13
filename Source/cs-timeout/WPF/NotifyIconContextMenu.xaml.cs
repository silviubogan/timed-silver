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
    /// Interaction logic for NotifyIconContextMenu.xaml
    /// </summary>
    public partial class NotifyIconContextMenu : ContextMenu
    {
        internal DataFile MyDataFile;

        public NotifyIconContextMenu()
        {
            InitializeComponent();
        }

        public NotifyIconContextMenu(DataFile df, System.Windows.Forms.NotifyIcon notifyIcon)
        {
            InitializeComponent();

            MyDataFile = df;

            df.ClockVMCollection.Model.ClockAdded += Model_ClockAdded;
            df.ClockVMCollection.Model.ClocksAdded += Model_ClocksAdded;
            df.ClockVMCollection.Model.ClockMoved += Model_ClockMoved;
            df.ClockVMCollection.Model.ClockRemoved += Model_ClockRemoved;
            df.ClockVMCollection.Model.TimerStopped += Model_TimerStopped;
            df.ClockVMCollection.Model.TimerStoppedByUser += Model_TimerStoppedByUser;
            df.ClockVMCollection.Model.TimerStartedByUser += Model_TimerStartedByUser;


            RefreshInactiveTimersSubmenu();
            FirstLoadOfMenu();
        }

        private void Model_TimerStartedByUser(object sender, ClockEventArgs e)
        {
            //var td = e.Clock as TimerData;
            //if (td == null)
            //{
            //    return;
            //}

            //ClockVM vm = MyDataFile.ClockVMCollection.VMForM(td);

            //ClockMenuItem cmi = ExistingClockMenuItemInInactiveMenu(vm);
            //if (cmi != null)
            //{
            //    MyInactiveMenuItem.Items.Remove(cmi);
            //    Items.Insert(0, cmi); // TODO: [VISUAL] insert at correct position according to the order in the MainForm
            //}

            UpdateSeparatorsVisibility();
        }

        private void UpdateSeparatorsVisibility()
        {
            //MyFirstSeparator.Visibility = Items.Count > 4 ?
                //Visibility.Visible : Visibility.Collapsed;
            //DoUpdate(MyInactiveMenuItem);
        }

        internal void DoUpdate(MenuItem tsmi)
        {
            if (tsmi.Items.Count > 0)
            {
                tsmi.Visibility = Visibility.Visible;
            }
            else
            {
                tsmi.Visibility = Visibility.Collapsed;
            }

            //MySecondSeparator.Visibility = Visibility.Visible;
            // tsmi.Visible; - Visible not changed, tried using Invoke, DoEvents.
        }

        //private ClockMenuItem ExistingClockMenuItemInInactiveMenu(ClockVM td)
        //{
            //for (int i = 0; i < MyInactiveMenuItem.Items.Count; ++i)
            //{
            //    if (MyInactiveMenuItem.Items[i] is ClockMenuItem tmi)
            //    {
            //        if (ReferenceEquals(td, tmi.DataContext))
            //        {
            //            return tmi;
            //        }
            //    }
            //}
            //return null;
        //}

        private ClockMenuItem ExistingClockMenuItemInActiveMenu(ClockVM td)
        {
            for (int i = 0; i < Items.Count; ++i)
            {
                if (Items[i] is ClockMenuItem tmi)
                {
                    if (ReferenceEquals(td, tmi.DataContext))
                    {
                        return tmi;
                    }
                }
            }
            return null;
        }

        private void Model_TimerStoppedByUser(object sender, ClockEventArgs e)
        {
            HandleClockRemoval(e.Clock);
        }

        private void Model_TimerStopped(object sender, ClockEventArgs e)
        {
            HandleClockRemoval(e.Clock);
        }

        private void Model_ClockRemoved(object sender, ClockEventArgs e)
        {
            var td = e.Clock as TimerData;
            if (td == null)
            {
                return;
            }

            ClockVM vm = MyDataFile.ClockVMCollection.VMForM(td);

            bool active = true;
            ClockMenuItem tmi = ExistingClockMenuItemInActiveMenu(vm);
            if (tmi == null)
            {
                active = false;
                //tmi = ExistingClockMenuItemInInactiveMenu(vm);
            }
            if (tmi != null)
            {
                if (active)
                {
                    Items.Remove(tmi);
                }
                else
                {
                    //MyInactiveMenuItem.Items.Remove(tmi);
                }
            }

            UpdateSeparatorsVisibility();
        }

        private void Model_ClockMoved(object sender, ClockMovedEventArgs e)
        {
            var td = e.Clock as TimerData;
            if (td == null)
            {
                return;
            }

            ClockVM vm = MyDataFile.ClockVMCollection.VMForM(td);

            bool active = true;
            ClockMenuItem tmi = ExistingClockMenuItemInActiveMenu(vm);
            //if (tmi == null)
            //{
            //    active = false;
            //    tmi = ExistingClockMenuItemInInactiveMenu(vm);
            //}
            if (tmi != null)
            {
                // TODO: What if e.NewIndex does not care about FilteredOut property of ClockData?

                //if (active)
                //{
                    Items.Remove(tmi);

                    int idx = Math.Min(e.NewIndex, Items.Count);

                    Items.Insert(idx, tmi);
                //}
                //else
                //{
                //    MyInactiveMenuItem.Items.Remove(tmi);

                //    int idx = Math.Min(e.NewIndex, MyInactiveMenuItem.Items.Count);

                //    MyInactiveMenuItem.Items.Insert(idx, tmi);
                //}
            }
        }

        private void Model_ClocksAdded(object sender, ClocksEventArgs e)
        {
            foreach (ClockM m in e.Clocks)
            {
                ClockVM vm = MyDataFile.ClockVMCollection.VMForM(m);

                if (m is TimerData tdd)
                {
                    var tmi = new ClockMenuItem();
                    tmi.DataContext = vm;
                    //if (tdd.Running)
                    //{
                        AddTimerMenuItemToMainMenu(tmi);
                    //}
                    //else
                    //{
                    //    MyInactiveMenuItem.Items.Insert(0, tmi);
                    //}
                }
            }

            UpdateSeparatorsVisibility();
        }

        internal void FirstLoadOfMenu()
        {
            foreach (ClockVM td in MyDataFile.ClockVMCollection.VMs)
            {
                if (td.ClockType == ClockVM.ClockTypes.Timer/* && td.IsActive*/)
                {
                    AddTimerMenuItemToMainMenu(new ClockMenuItem()
                    {
                        DataContext = td
                    });
                }
            }

            UpdateSeparatorsVisibility();

            //RefreshInactiveTimersSubmenu();
        }

        internal void RefreshInactiveTimersSubmenu()
        {
            //MyInactiveMenuItem.Items.Clear();
            //foreach (ClockVM td in MyDataFile.ClockVMCollection.VMs)
            //{
            //    if (td.Model is TimerData tdd && !tdd.Running)
            //    {
            //        if (!AlreadyExistsInMenus(td))
            //        {
            //            MyInactiveMenuItem.Items.Add(new ClockMenuItem()
            //            {
            //                DataContext = td
            //            });
            //        }
            //    }
            //}

            ////if (tsmi.GetCurrentParent() == null)
            ////{
            //DoUpdate(MyInactiveMenuItem);
            ////}
            ////else
            ////{
            ////    tsmi.GetCurrentParent().Invoke(new Action(() =>
            ////    {
            ////        DoUpdate(tsmi);
            ////    }));
            ////}
        }

        private void Model_ClockAdded(object sender, ClockEventArgs e)
        {
            if (e.Clock is TimerData tdd)
            {
                ClockVM vm = MyDataFile.ClockVMCollection.VMForM(tdd);

                var tmi = new ClockMenuItem();
                tmi.DataContext = vm;
                if (tdd.Running)
                {
                    AddTimerMenuItemToMainMenu(tmi);
                }
                else
                {
                    AddTimerMenuItemToMainMenu(tmi);
                    //MyInactiveMenuItem.Items.Insert(0, tmi);
                }
            }

            UpdateSeparatorsVisibility();
        }

        internal void AddTimerMenuItemToMainMenu(ClockMenuItem tmi)
        {
            if (!AlreadyExistsInMenus((ClockVM)tmi.DataContext))
            {
                Items.Insert(Items.Count - 1, tmi);
            }
        }

        internal bool AlreadyExistsInMenus(ClockVM cd)
        {
            if (ExistingClockMenuItemInActiveMenu(cd) != null)
            {
                return true;
            }

            //if (ExistingClockMenuItemInInactiveMenu(cd) != null)
            //{
            //    return true;
            //}

            return false;
        }

        public void Show()
        {
            Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
            //HorizontalOffset = SystemParameters.PrimaryScreenWidth - ActualWidth;
            //VerticalOffset = SystemParameters.PrimaryScreenHeight - ActualHeight;
            IsOpen = true;
        }

        internal event EventHandler AppExitRequested;

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            AppExitRequested?.Invoke(this, EventArgs.Empty);
        }

        internal void HandleClockRemoval(ClockM c)
        {
            var td = c as TimerData;
            if (td == null)
            {
                return;
            }

            ClockVM vm = MyDataFile.ClockVMCollection.VMForM(td);

            ClockMenuItem tmi = ExistingClockMenuItemInActiveMenu(vm);
            if (tmi != null)
            {
                //tmi.UpdateImage();
                //ToolStrip p = tmi.Owner;
                //Items.Remove(tmi);
                //MyInactiveMenuItem.Items.Insert(0, tmi);
            }

            UpdateSeparatorsVisibility();
        }

        //private void TsmiInactive_DropDownOpening(object sender, EventArgs e)
        //{
        //    var menuItem = sender as ToolStripMenuItem;
        //    if (menuItem.HasDropDownItems == false)
        //    {
        //        return; // not a drop down item
        //    }

        //    // Current bounds of the current monitor
        //    Rectangle Bounds = tsmiInactive.GetCurrentParent().Bounds;
        //    Screen CurrentScreen = Screen.FromPoint(Bounds.Location);

        //    // Look how big our children are:
        //    int MaxWidth = 0;
        //    foreach (ToolStripMenuItem subitem in menuItem.DropDownItems)
        //    {
        //        MaxWidth = Math.Max(subitem.Width, MaxWidth);
        //    }
        //    MaxWidth += 10; // Add a little wiggle room

        //    int FarRight = Bounds.Right + MaxWidth;

        //    int CurrentMonitorRight = CurrentScreen.Bounds.Right;

        //    if (FarRight > CurrentMonitorRight)
        //    {
        //        menuItem.DropDownDirection = ToolStripDropDownDirection.Left;
        //    }
        //    else
        //    {
        //        menuItem.DropDownDirection = ToolStripDropDownDirection.Right;
        //    }
        //}
    }
}
