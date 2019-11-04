using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Threading;
using DragDropEffects = System.Windows.DragDropEffects;
using TabControl = System.Windows.Controls.TabControl;

namespace cs_timed_silver
{
    public class ClockVMCollection : BindableBase,  GongSolutions.Wpf.DragDrop.IDropTarget
    {
        public ClockMCollection Model { get; set; }

        public ObservableCollection<ClockVM> SelectedClocks { get; set; }

        internal DispatcherTimer AlarmsTimer;

        internal DataFile MyDataFile;

        //public ObservableCollectionSynchronizer<ClockM> MySync;

        protected bool SynchDisabled = false;

        private ObservableCollection<ClockVM> _VMs;
        public ObservableCollection<ClockVM> VMs
        {
            get { return _VMs; }
            set { SetProperty(ref _VMs, value); }
        }

        public ClockVMCollection(DataFile df)
        {
            MyDataFile = df;

            SelectedClocks = new ObservableCollection<ClockVM>();

            AlarmsTimer = new DispatcherTimer(DispatcherPriority.Send)
            {
                Interval = TimeSpan.FromSeconds(0.5)
            };
            AlarmsTimer.Tick += AlarmsTimer_Tick;
        }

        public void Init()
        {
            Model = new ClockMCollection(MyDataFile);

            Model.Init();

            Model.IsUnsavedChanged += TimerCollection_IsDirtyChanged;
            Model.ClockPropertyChanged += Model_ClockPropertyChanged;

            Model.ClockAdded += Model_ClockAdded;
            Model.ClocksAdded += Model_ClocksAdded;
            Model.BeforeRemoveAllClocks += Model_BeforeRemoveAllClocks;

            // TODO: remove these:
            Model.TimerStartedByUser += Model_TimerStartedByUser;
            Model.TimerStopped += Model_TimerStopped;
            Model.TimerStoppedByUser += Model_TimerStoppedByUser;


            VMs = new ObservableCollection<ClockVM>();
            VMs.CollectionChanged += VMs_CollectionChanged;
            Model.Ms.CollectionChanged += Ms_CollectionChanged;

            // TODO: keep the latest value of SelectedCount
            // and only trigger the property changed event
            // when the value changes (not on identical
            // consecutive values)
            SelectedClocks.CollectionChanged +=
                SelectedClocks_CollectionChanged;

            //MySync = new ObservableCollectionSynchronizer<ClockM>(Model.Ms);
            //MySync.Synchronize(Model.Ms, new Func<ClockM, ClockVM>((ClockM m) =>
            //{
            //    return 
            //}));

            AlarmsTimer.Start();
        }

        private void Model_BeforeRemoveAllClocks(object sender, EventArgs e)
        {
            UpdateClosestRingingMomentClockBasedOnChangedClock(null);
        }

        private void Model_TimerStoppedByUser(object sender, ClockEventArgs e)
        {
            //UpdateClosestRingingMomentClockBasedOnChangedClock(e.Clock);
        }

        private void Model_TimerStopped(object sender, ClockEventArgs e)
        {
            //UpdateClosestRingingMomentClockBasedOnChangedClock(e.Clock);
        }

        private void Model_TimerStartedByUser(object sender, ClockEventArgs e)
        {
            //UpdateClosestRingingMomentClockBasedOnChangedClock(e.Clock);
        }

        private void Model_ClocksAdded(object sender, ClocksEventArgs e)
        {
            foreach (ClockM c in e.Clocks)
            {
                UpdateClosestRingingMomentClockBasedOnChangedClock(c);
            }
        }

        private void UpdateClosestRingingMomentClockBasedOnChangedClock(ClockM c)
        {
            if (c == null)
            {
                ClosestRingingMomentClockVM = null;
                return;
            }

            var vm = VMForM(c);

            // `vm == null` is when changing (toggling) the same clock's type twice
            if (vm != null && vm.IsActive && !c.IsBeingDeleted)
            {
                if (ClosestRingingMomentClockVM == null)
                {
                    ClosestRingingMomentClockVM = vm;
                }
                else
                {
                    ClosestRingingMomentClockVM = ClockMin(vm, ClosestRingingMomentClockVM);
                }
            }
            else
            {
                if (ClosestRingingMomentClockVM == null)
                {
                    FullUpdateOfClosestRingingMomentClock();
                }
                else if (ClosestRingingMomentClockVM != null &&
                            (!ClosestRingingMomentClockVM.IsActive ||
                             c.IsBeingDeleted))
                {
                    FullUpdateOfClosestRingingMomentClock();
                }
            }
        }

        internal void FullUpdateOfClosestRingingMomentClock()
        {
            ClockVM cvm = null;
            for (int i = 0; i < Model.Count; ++i)
            {
                ClockVM vm = VMForM(Model.Ms[i]);
                if (vm == null)
                {
                    // TODO: this case should not exist (verify:
                    // change timer to alarm in menu of a
                    // ClockUserControl.xaml) after a file is loaded.
                    continue;
                }
                if (vm.IsActive)
                {
                    cvm = ClockMin(cvm, vm);
                }
            }
            ClosestRingingMomentClockVM = cvm;
        }

        private void Model_ClockAdded(object sender, ClockEventArgs e)
        {
            UpdateClosestRingingMomentClockBasedOnChangedClock(e.Clock);
        }

        internal ClockVM ClockMin(ClockVM a, ClockVM b)
        {
            if (a == null) return b;
            if (b == null) return a;

            if (a.ClockType == b.ClockType &&
                a.ClockType == ClockVM.ClockTypes.Timer)
            {
                if (a.CurrentTimeSpan < b.CurrentTimeSpan)
                {
                    return a;
                }
                return b;
            }

            if (a.ClockType == ClockVM.ClockTypes.Timer &&
                b.ClockType == ClockVM.ClockTypes.Alarm)
            {
                if (DateTime.Now + a.CurrentTimeSpan < b.CurrentDateTime)
                {
                    return a;
                }
                if (b.CurrentDateTime < DateTime.Now)
                {
                    return a;
                }
                return b;
            }

            if (b.ClockType == ClockVM.ClockTypes.Alarm &&
                a.ClockType == ClockVM.ClockTypes.Timer)
            {
                if (DateTime.Now + b.CurrentTimeSpan < a.CurrentDateTime)
                {
                    return b;
                }
                if (a.CurrentDateTime < DateTime.Now)
                {
                    return b;
                }
                return a;
            }

            return a;
        }

        private void Model_ClockPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var cd = sender as ClockM;

            if (e.PropertyName == "Checked" || e.PropertyName == "FilteredOut")
            {
                if (cd.Checked && !cd.FilteredOut)
                {
                    SelectedClocks.Add(VMForM(cd));
                }
                else // if (!cd.Checked || cd.FilteredOut)
                {
                    SelectedClocks.Remove(VMForM(cd));
                }
            }
            else if (
                (e.PropertyName == "CurrentValue" ||
                 e.PropertyName == "CurrentDateTime" ||
                 e.PropertyName == "CurrentTimeSpan" ||
                 e.PropertyName == "IsActive"))
            {
                UpdateClosestRingingMomentClockBasedOnChangedClock(cd);
            }
        }

        private void AlarmsTimer_Tick(object sender, EventArgs e)
        {
            RingPassedAlarms();
            UpdateRemainingTimeStatusString(ClosestRingingMomentClockVM);
        }

        internal void RingPassedAlarms()
        {
            if (VMs == null)
            {
                return;
            }

            for (int i = VMs.Count - 1; i >= 0; --i)
            {
                ClockVM cd = VMs[i];

                if (cd.Enabled && cd.ClockType == ClockVM.ClockTypes.Alarm)
                {
                    var ad = cd.Model as AlarmData;
                    if (ad.CurrentDateTime <= DateTime.Now)
                    {
                        ad.Enabled = false;
                        ad.ShowTimeOutForm();
                    }
                }
            }
        }

        internal void TimerCollection_IsDirtyChanged(object sender, EventArgs e)
        {
            MyDataFile.UpdateGlobalIsUnsaved();
        }

        internal Dictionary<ClockM, ClockVM> MToVM =
            new Dictionary<ClockM, ClockVM>(EqualityComparer<ClockM>.Default);
        internal ClockVM VMForM(ClockM td)
        {
            if (MToVM.ContainsKey(td))
            {
                return MToVM[td];
            }
            return null;
        }

        private void VMs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (SynchDisabled)
            {
                return;
            }
            SynchDisabled = true;

            Utils.SynchronizeCollectionChange(VMs, e, Model.Ms,
                (ClockM m) =>
                {
                    if (m.Checked && !m.FilteredOut)
                    {
                        SelectedClocks.Add(VMForM(m));
                    }
                },
                null,
                (ClockVM vm, ClockM m) =>
                {
                    return object.ReferenceEquals(vm.Model, m);
                },
                (ClockVM vm) =>
                {
                    return vm.Model;
                });

            SynchDisabled = false;
        }
        
        // TODO: set MToVM also in synching coded reacting to VM collection change.

        private void Ms_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (SynchDisabled)
            {
                return;
            }
            SynchDisabled = true;

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ClockM m in e.OldItems)
                {
                    UpdateClosestRingingMomentClockBasedOnChangedClock(m);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (ClockVM vm in VMs)
                {
                    vm.ClockTypeChanged -= ClockVMCollection_ClockTypeChanged;
                }
                MToVM.Clear();
                SelectedClocks.Clear();
            }

            Utils.SynchronizeCollectionChange(Model.Ms, e, VMs,
                (ClockVM vm) =>
                {
                },
                (ClockM m) =>
                {
                    VMForM(m).ClockTypeChanged -= ClockVMCollection_ClockTypeChanged;
                },
                (ClockM m, ClockVM vm) =>
                {
                    return ReferenceEquals(vm.Model, m);
                },
                (ClockM m) =>
                {
                    int idx = Model.Ms.IndexOf(x => ReferenceEquals(x, m)); // ClockVM subclasses ClockM
                    if (0 <= idx && idx <= Model.Ms.Count - 1)
                    {
                        MToVM[m] = new ClockVM(m, this);
                        MToVM[m].ClockTypeChanged += ClockVMCollection_ClockTypeChanged;
                        //VMs.Insert(idx, MToVM[m]);
                        return MToVM[m];
                    }
                    return null;

                    //// indicele 1
                    //int idx = Model.Ms.IndexOf(x => ReferenceEquals(x, m)); // ClockVM subclasses ClockM
                    //if (!(idx >= 0 && idx <= Model.Ms.Count - 1))
                    //{
                    //    // indicele 2
                    //    if (MToVM.ContainsKey(m))
                    //    {
                    //        idx = MToVM.Keys.ToList().IndexOf(m);

                    //        MToVM[m] = new ClockVM(m, this);
                    //        MToVM[m].ClockTypeChanged += ClockVMCollection_ClockTypeChanged;
                    //        //VMs.Insert(idx, MToVM[m]);
                    //        return MToVM[m];
                    //    }
                    //    return null;
                    //}
                    //return null;
                });

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ClockM m in e.OldItems)
                {
                    SelectedClocks.RemoveReference(x => object.ReferenceEquals(VMForM(m), x));
                }
            }

            //switch (e.Action)
            //{
            //    case NotifyCollectionChangedAction.Add:
                    //foreach (object item in e.NewItems)
                    //{
                    //    var mItem = (ClockM)item;
                    //    int idx = Model.Ms.IndexOf(x => x == mItem);

                    //    MToVM[mItem] = new ClockVM(mItem, this);

                    //    MToVM[mItem].ClockTypeChanged += ClockVMCollection_ClockTypeChanged;

                    //    // the operation could have been either Insert or Add
                    //    if (0 <= idx && idx <= Model.Ms.Count - 1)
                    //    {
                    //        VMs.Insert(idx, MToVM[mItem]);
                    //    }
                    //    else
                    //    {
                    //        VMs.Add(MToVM[mItem]);

                    //        if (mItem.Checked && !mItem.FilteredOut)
                    //        {
                    //            SelectedClocks.Add(MToVM[mItem]);
                    //        }
                    //    }
                    //}
                    //break;

            //    case NotifyCollectionChangedAction.Remove:
            //        foreach (object item in e.OldItems)
            //        {
            //            var mItem = item as ClockM;

            //            UpdateClosestRingingMomentClockBasedOnChangedClock(mItem);

            //            // find VM objects that wrap the relevant model object and remove them
            //            IEnumerable<ClockVM> query;
            //            while ((query = from vm in VMs
            //                            where ReferenceEquals(vm.Model, mItem)
            //                            select vm).Count() > 0)
            //            {
            //                ClockVM m = query.First();

            //                m.ClockTypeChanged -= ClockVMCollection_ClockTypeChanged;


            //                int index = VMs.IndexOf(m);
            //                VMs.Remove(x => ReferenceEquals(x, m));

            //                MToVM.Remove(mItem);
            //            }

            //            SelectedClocks.Remove(VMForM(mItem));
            //        }
            //        break;

            //    case NotifyCollectionChangedAction.Reset:
            //        foreach (ClockVM vm in VMs)
            //        {
            //            vm.ClockTypeChanged -= ClockVMCollection_ClockTypeChanged;
            //        }
            //        MToVM.Clear();
            //        VMs.Clear();
            //        SelectedClocks.Clear();
            //        break;

            //    case NotifyCollectionChangedAction.Move:
            //        // TODO: handle multiple items
            //        VMs.Move(e.OldStartingIndex, e.NewStartingIndex);
            //        break;

            //    case NotifyCollectionChangedAction.Replace:
            //        // TODO: handle multiple items
            //        var cm = (ClockM)e.NewItems[0];
            //        var cvm = new ClockVM(cm, this);
            //        VMs[e.OldStartingIndex] = cvm;
            //        MToVM[cm] = cvm;
            //        break;

            //    default:
            //        throw new NotImplementedException();
            //}

            SynchDisabled = false;
        }

        private void ClockVMCollection_ClockTypeChanged(object sender, ClockVM.ClockTypeChangedEventArgs e)
        {
            UpdateClosestRingingMomentClockBasedOnChangedClock(e.NewClockM);
        }

        internal void SubscribeToClock(ClockVM cd)
        {
            if (cd.Model is TimerData td)
            {
                td.TimerStopped += Td_TimerStopped;
            }
            cd.ClockTypeChanged += Cd_ClockTypeChanged;
        }

        private void Cd_ClockTypeChanged(object sender, ClockVM.ClockTypeChangedEventArgs e)
        {
            // these remain because there is a handler which is 
            // un/subscribed here that has to do with the view
            // (VM should not access the view and modifying the view
            // just through data-binding, triggers and other WPF stuff
            // but timers are a case when the view must be updated
            // through classic ways)

            MToVM[e.NewClockM] = e.ClockVM;

            UnsubscribeFromClock(e.ClockVM);
            Model.UnsubscribeFromClock(e.OldClockM);
            Model.SubscribeToClock(e.NewClockM);
            SubscribeToClock(e.ClockVM); // like VMForM(e.NewClockM)

            MToVM.Remove(e.OldClockM);


            Model.ApplyFilterToClock(e.NewClockM);
        }

        internal void UnsubscribeFromClock(ClockVM cd)
        {
            if (cd.Model is TimerData td)
            {
                td.TimerStopped -= Td_TimerStopped;
            }
            cd.ClockTypeChanged -= Cd_ClockTypeChanged;
        }

        private void Td_TimerStopped(object sender, ClockEventArgs e)
        {
            new TimeOutWindowVM(
                new TimeOutWindowM(e.Clock)).ShowWindow();
        }

        private void SelectedClocks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("SelectedCount");
        }

        public int SelectedCount
        {
            get
            {
                return SelectedClocks.Count;
            }
        }


        internal ClockVM PreviousClosestRingingMomentClockVM = null;
        internal ClockVM _ClosestRingingMomentClockVM = null;
        public ClockVM ClosestRingingMomentClockVM
        {
            get
            {
                return _ClosestRingingMomentClockVM;
            }
            set
            {
                if (_ClosestRingingMomentClockVM != value)
                {
                    PreviousClosestRingingMomentClockVM = _ClosestRingingMomentClockVM;
                    _ClosestRingingMomentClockVM = value;
                    OnClosestRingingMomentClockVMChanged();
                }
            }
        }

        private void OnClosestRingingMomentClockVMChanged()
        {
            if (PreviousClosestRingingMomentClockVM != null)
            {
                PreviousClosestRingingMomentClockVM.PropertyChanged -= ClosestRingingMomentClockVM_PropertyChanged;
            }
            if (ClosestRingingMomentClockVM != null)
            {
                ClosestRingingMomentClockVM.PropertyChanged += ClosestRingingMomentClockVM_PropertyChanged;
            }
            else
            {

            }
            UpdateRemainingTimeStatusString(ClosestRingingMomentClockVM);
        }

        internal string _RemainingTimeStatusString = "";
        public string RemainingTimeStatusString
        {
            get
            {
                return _RemainingTimeStatusString;
            }
            set
            {
                if (value != _RemainingTimeStatusString)
                {
                    _RemainingTimeStatusString = value;
                    RaisePropertyChanged();
                }
            }
        }

        private void ClosestRingingMomentClockVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentTimeSpan" ||
                e.PropertyName == "CurrentDateTime" ||
                e.PropertyName == "CurrentValue" ||
                e.PropertyName == "IsActive")
            {
                UpdateRemainingTimeStatusString(sender);
            }
        }

        private void UpdateRemainingTimeStatusString(object sender)
        {
            if (sender == null)
            {
                RemainingTimeStatusString = "";
                return;
            }

            var s = sender as ClockVM;

            if (!s.IsActive)
            {
                RemainingTimeStatusString = "";
                return;
            }

            if (s.ClockType == ClockVM.ClockTypes.Alarm)
            {
                if (s.CurrentDateTime < DateTime.Now)
                {
                    RemainingTimeStatusString = "";
                }
                else
                {
                    RemainingTimeStatusString = (s.CurrentDateTime - DateTime.Now).ToString() + " remaining";
                }
            }
            else // Timer
            {
                if (s.CurrentTimeSpan == TimeSpan.Zero)
                {
                    RemainingTimeStatusString = "";
                }
                else
                {
                    RemainingTimeStatusString = s.CurrentTimeSpan.ToString() + " remaining";
                }
            }
        }

        void GongSolutions.Wpf.DragDrop.IDropTarget.DragOver(IDropInfo dropInfo)
        {
            var sourceItem = dropInfo.Data as ClockVM;
            var targetItem = dropInfo.TargetItem as ClockVM;

            if (sourceItem != null /*&& targetItem != null*/) // targetItem is null when the user drops at the end of the list
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = System.Windows.DragDropEffects.Move;
            }
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


        public static IEnumerable ExtractData(object data)
        {
            if (data is IEnumerable && !(data is string))
            {
                return (IEnumerable)data;
            }

            return Enumerable.Repeat(data, 1);
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
        /// From Gang WPF Drag Drop.
        /// </summary>
        /// <param name="dropInfo"></param>
        void GongSolutions.Wpf.DragDrop.IDropTarget.Drop(IDropInfo dropInfo)
        {
            if (dropInfo == null || dropInfo.DragInfo == null)
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

            var destinationList = dropInfo.TargetCollection.TryGetList();
            var data = ExtractData(dropInfo.Data).OfType<object>().ToList();

            var copyData = ShouldCopyData(dropInfo);
            if (!copyData)
            {
                var sourceList = dropInfo.DragInfo.SourceCollection.TryGetList();
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

            // My implementation here (not good):
            //HandleDrop(dropInfo);
        }

        private void HandleDrop(IDropInfo dropInfo)
        {
            if (dropInfo.TargetItem == dropInfo.Data)
            {
                return;
            }

            //dropInfo.DragInfo.SourceItems;
            //dropInfo.TargetCollection;
            //dropInfo.InsertIndex;

            foreach (ClockVM vm in dropInfo.DragInfo.SourceItems)
            {
                (dropInfo.DragInfo.SourceCollection as ObservableCollection<ClockVM>).Remove(vm);
            }

            var c = dropInfo.TargetCollection as ObservableCollection<ClockVM>;

            int i = dropInfo.InsertIndex;// Math.Max(0, dropInfo.InsertIndex - 1);

            //if (dropInfo.InsertPosition== RelativeInsertPosition.AfterTargetItem)
            //{
            //    i = dropInfo.InsertIndex + 1;
            //}

            foreach (ClockVM vm in dropInfo.DragInfo.SourceItems)
            {
                if (i >= c.Count)
                {
                    c.Add(vm);
                }
                else
                {
                    c.Insert(i, vm);
                }
                i++;
            }

            (dropInfo.DragInfo.VisualSource as System.Windows.Controls.Control).Dispatcher.BeginInvoke(new Action(() =>
            {
                Model.RefreshIDs();
                Model.IsUnsaved = true;
            }), DispatcherPriority.ApplicationIdle);
        }
    }
}
