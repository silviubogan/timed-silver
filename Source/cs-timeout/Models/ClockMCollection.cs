using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Markup;

namespace cs_timed_silver
{
    public class ClockMCollection : BindableBase,
        IUnsavedStatusCapable, IEquatable<ClockMCollection>
    {
        public ObservableCollection<ClockM> Ms { get; set; }

        internal bool _IsUnsaved = false;
        public bool IsUnsaved
        {
            get
            {
                return _IsUnsaved;
            }
            set
            {
                if (!IsUnsavedLocked && _IsUnsaved != value)
                {
                    _IsUnsaved = value;
                    OnIsUnsavedChanged();
                }
            }
        }
        public bool IsUnsavedLocked { get; set; } = false;

        public event EventHandler
            RemoveAllClocksDone, BeforeRemoveAllClocks,
            BeforeAppliedFilterChange, AfterAppliedFilterChange;

        public event EventHandler IsUnsavedChanged;

        public event NotifyCollectionChangedEventHandler GroupsChanged
        {
            add
            {
                Groups.Ms.CollectionChanged += value;
            }
            remove
            {
                Groups.Ms.CollectionChanged -= value;
            }
        }

        public event EventHandler<ClockEventArgs> ClockRemoved, ClockAdded,
            ClockFilteredOut, ClockFilteredIn,
            // just for TimerData-s:
            TimerStartedByUser, TimerStopped,
            TimerStoppedByUser;

        public event EventHandler<ClocksEventArgs> ClocksAdded;

        internal void CheckAll()
        {
            foreach (ClockM cd in Ms)
            {
                if (!cd.FilteredOut)
                {
                    cd.Checked = true;
                }
            }
        }

        internal void UncheckAll()
        {
            foreach (ClockM cd in Ms)
            {
                if (!cd.FilteredOut)
                {
                    cd.Checked = false;
                }
            }
        }

        public event PropertyChangedEventHandler ClockPropertyChanged;
        public event EventHandler<ClockMovedEventArgs> ClockMoved;

        /// <summary>
        /// The DoubleEventArgs is a percent (-1 means no value).
        /// </summary>
        public event EventHandler<DoubleEventArgs> GlobalProgressChanged;

        internal void OnIsUnsavedChanged()
        {
            if (!IsUnsaved)
            {
                IsUnsavedLocked = true;
                foreach (ClockM td in Ms)
                {
                    td.IsUnsaved = false;
                }
                IsUnsavedLocked = false;
            }

            IsUnsavedChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Also adds the group of the ClockData to the Groups collection.
        /// </summary>
        /// <param name="td"></param>
        internal void AddClock(ClockM td)
        {
            if (Contains(td))
            {
                return;
            }

            AddClockUnchecked(td);

            ++VisibleCount;

            IsUnsaved = true;

            OnClockAdded(new ClockEventArgs()
            {
                Clock = td
            });

            DoAllNeededSorts();
        }

        internal void DoAllNeededSorts()
        {
            // TODO: optimization:do not sort the entire collections,
            // just move the new clock at the good position:
            DoSortAlphabetically();
            DoSortByClosestRingingMoment();
        }

        private void OnClockAdded(ClockEventArgs e)
        {
            SubscribeToClock(e.Clock);

            // before sorting...
            //CollectionChanged?.Invoke(this,
            //    new NotifyCollectionChangedEventArgs(
            //        NotifyCollectionChangedAction.Add, e.Clock));

            // this sorts sometimes...
            ClockAdded?.Invoke(this, e);
        }

        private void OnClocksAdded(ClocksEventArgs e)
        {
            foreach (ClockM cd in e.Clocks)
            {
                SubscribeToClock(cd);

                // before sorting...
                //CollectionChanged?.Invoke(this,
                //    new NotifyCollectionChangedEventArgs(
                //        NotifyCollectionChangedAction.Add, cd));
            }

            // this sorts sometimes...
            ClocksAdded?.Invoke(this, e);
        }

        internal bool Contains(ClockM cd)
        {
            return Utils.ContainsReference(Ms, cd);
        }


        /// <summary>
        /// NOTE: BUG: this does not get called for existin unmodified files (if I press the Add timer btn, everything starts to work) it is subscribed to the IsDirtyChanged evt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Td_IsDirtyChanged(object sender, EventArgs e)
        {
            IsUnsaved = true;
        }

        public ClockGroupMCollection Groups { get; set; }

        public FilterMCollection Filters { get; set; }
        public FilterVMCollection FiltersVM { get; set; }

        internal FilterM _PreviousFilter = null;
        internal FilterM PreviousFilter
        {
            get
            {
                return _PreviousFilter;
            }
            set
            {
                if (_PreviousFilter != value)
                {
                    _PreviousFilter = value;
                }
            }
        }

        internal FilterM _AppliedFilter = null;
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
                    PreviousFilter = _AppliedFilter;
                    _AppliedFilter = value;
                    OnAppliedFilterChanged();
                    IsUnsaved = true;
                }
            }
        }

        internal void Insert(int newIndex, ClockM cd)
        {
            AddClock(cd);
            Ms.Move(Ms.Count - 1, newIndex);
        }

        /// <summary>
        /// Requires f (the filter) to not be null.
        /// </summary>
        /// <param name="f"></param>
        /// <param name="td"></param>
        /// <returns></returns>
        internal static bool ClockSatisfiesFilter(FilterM f, ClockM td)
        {
            bool r = false;

            if (!f.ShowActive || !f.ShowInactive)
            {
                if (f.ShowActive)
                {
                    if (td is AlarmData ad && !ad.Enabled)
                    {
                        r = true;
                    }
                    else if (td is TimerData tdd && !tdd.Running)
                    {
                        r = true;
                    }
                    else
                    {
                        //r |= false;
                    }
                }

                if (f.ShowInactive)
                {
                    if (td is AlarmData ad && ad.Enabled)
                    {
                        r = true;
                    }
                    else if (td is TimerData tdd && tdd.Running)
                    {
                        r = true;
                    }
                    else
                    {
                        //r |= false;
                    }
                }
            }
            if (!f.ShowTimers || !f.ShowAlarms)
            {
                if (f.ShowTimers && td is AlarmData)
                {
                    r = true;
                }
                else
                {
                    //r |= false;
                }

                if (f.ShowAlarms && td is TimerData)
                {
                    r = true;
                }
                else
                {
                    //r |= false;
                }
            }

            if (f.ShowsGroup(td.GroupName))/* ||
                    td.GroupName == ""*/
            {
                //r = r == false;
            }
            else
            {
                r = true;
            }

            if (!td.Tag.ToPlainText().ToLower().Contains(f.SearchString.ToLower()))
            {
                r = true;
            }

            return !r;
        }

        private void OnAppliedFilterChanged()
        {
            BeforeAppliedFilterChange?.Invoke(this, EventArgs.Empty);

            foreach (ClockM td in Ms)
            {
                ApplyFilterToClock(td);
            }

            MyDataFile.Settings.SetValue("Filter", AppliedFilter.ToString());

            RefreshIDs();

            AfterAppliedFilterChange?.Invoke(this, EventArgs.Empty);
        }

        internal IEnumerable<ClockM> OneTimeFilter(FilterM f)
        {
            foreach (ClockM cd in Ms)
            {
                if (ClockSatisfiesFilter(f, cd))
                {
                    yield return cd;
                }
            }
        }

        public DataFile MyDataFile { get; set; }

        public ClockGroupVMCollection GroupsVM { get; set; }

        internal ClockMCollection(DataFile df)
        {
            MyDataFile = df;

            Ms = new ObservableCollection<ClockM>();
            Ms.CollectionChanged += Ms_CollectionChanged;

            GroupsVM = new ClockGroupVMCollection(this);
            Groups = GroupsVM.MyModel;

            FiltersVM = new FilterVMCollection(this);
            Filters = FiltersVM.MyModel;

            IsUnsavedLocked = true;
            // the AlarmsTimer Tick handler necessitates an applied filter
            // (refactor: move the default filter as an AcceptAllFilter static
            // member in the Filter class)
            AppliedFilter = new FilterM(this)
            {
                ShowActive = true,
                ShowInactive = true,
                ShowAlarms = true,
                ShowTimers = true
            };
            IsUnsavedLocked = false;

            ClockRemoved += TimerDataCollection_ClockRemoved;

            TimerStartedByUser += TimerDataCollection_TimerStartedByUser;
            TimerStopped += TimerDataCollection_TimerStopped;
            TimerStoppedByUser += TimerDataCollection_TimerStoppedByUser;
        }

        public void Init()
        {
            FiltersVM.Init();
        }

        private void Ms_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //if (e.Action == NotifyCollectionChangedAction.Move)
            //{
            //    OnClockMoved(new ClockMovedEventArgs()
            //    {
            //        Clock = e.OldItems[0] as ClockM,
            //        OldIndex = e.OldStartingIndex,
            //        NewIndex = e.NewStartingIndex
            //    });
            //}

            // move these somewhere better (after synchronizing the Ms with VMs or VMs with Ms):
            RefreshIDs();
            IsUnsaved = true;
        }

        /// <summary>
        /// Also creates the group of the ClockData if it does not exist.
        /// </summary>
        /// <param name="td"></param>
        internal void AddClockUnchecked(ClockM td)
        {
            if (!string.IsNullOrEmpty(td.GroupName))
            {
                if (!Groups.Contains(td.GroupName))
                {
                    Groups.Add(td.GroupName);
                }
            }

            Ms.Add(td);
        }

        private void TimerDataCollection_TimerStoppedByUser(object sender, ClockEventArgs e)
        {
            ReportGlobalProgress();
        }

        private void TimerDataCollection_TimerStopped(object sender, ClockEventArgs e)
        {
            ReportGlobalProgress();

            ApplyFilterToClock(e.Clock);

            DoSortByClosestRingingMoment();
        }

        private void TimerDataCollection_TimerStartedByUser(object sender, ClockEventArgs e)
        {
            ReportGlobalProgress();
        }

        private void TimerDataCollection_ClockRemoved(object sender, ClockEventArgs e)
        {
            ReportGlobalProgress();
        }

        internal void ReportGlobalProgress()
        {

            // NOTE: arithmetic average
            //TimeSpan resetToSum = TimeSpan.Zero;
            //TimeSpan remainingSum = TimeSpan.Zero;
            //foreach (TimerData td in TimersData)
            //{
            //    if (td.t.Enabled)
            //    {
            //        resetToSum += td.ResetToValue;
            //        remainingSum = td.RemainingTimeSpan;
            //    }
            //}

            // NOTE: soonest ringing
            TimeSpan resetToSum = TimeSpan.MaxValue;
            TimeSpan remainingSum = TimeSpan.MaxValue;
            foreach (ClockM td in Ms)
            {
                if (td is TimerData tdata && tdata.Running)
                {
                    if (remainingSum > tdata.CurrentTimeSpan)
                    {
                        remainingSum = tdata.CurrentTimeSpan;
                        resetToSum = (TimeSpan)tdata.ResetToValue;
                    }
                }
            }
            
            if (resetToSum == TimeSpan.MaxValue)
            {
                GlobalProgressChanged?.Invoke(this, new DoubleEventArgs()
                {
                    Value = -1D
                });
            }
            else if (remainingSum == TimeSpan.Zero)
            {
                GlobalProgressChanged?.Invoke(this, new DoubleEventArgs()
                {
                    Value = 0D
                });
            }
            else
            {
                GlobalProgressChanged?.Invoke(this, new DoubleEventArgs()
                {
                    Value = remainingSum.TotalMilliseconds / resetToSum.TotalMilliseconds * 100F
                });
            }

            /* NOTE: for arithmetic average tehnique:
            if (resetToSum == TimeSpan.Zero)
            {
                // remove progress from task bar
                MyDataFile.MainForm.SetTaskBarProgressPercent(-1);
            }
            else if (remainingSum == TimeSpan.Zero)
            {
                MyDataFile.MainForm.SetTaskBarProgressPercent(0);
            }
            else
            {
                // set progress to percent: remainingSum/resetToSum * 100
                MyDataFile.MainForm.SetTaskBarProgressPercent(remainingSum.TotalMilliseconds / resetToSum.TotalMilliseconds * 100F);
            }*/
        }

        internal void RemoveClock(ClockM td)
        {
            int index = td.GetIndex();

            int removedCount = Ms.Remove(x => ReferenceEquals(x, td));

            if (removedCount > 0)
            {
                IsUnsaved = true;

                --VisibleCount;
            }

            OnClockRemoved(new ClockEventArgs()
            {
                Clock = td,
                Index = index
            });
        }

        private void OnClockRemoved(ClockEventArgs e)
        {
            UnsubscribeFromClock(e.Clock);

            ClockRemoved?.Invoke(this, e);

            //CollectionChanged?.Invoke(this,
            //    new NotifyCollectionChangedEventArgs(
            //        NotifyCollectionChangedAction.Remove, e.Clock, e.Index));
        }

        internal void RemoveAllClocksUnchecked()
        {
            Ms.Clear();
        }

        internal void RemoveAllClocks()
        {
            OnBeforeRemoveAllClocks(EventArgs.Empty);

            if (Ms.Count > 0)
            {
                Ms.Clear();

                VisibleCount = 0;

                IsUnsaved = true;
            }

            OnRemoveAllClocksDone(EventArgs.Empty);

            //CollectionChanged?.Invoke(this,
            //    new NotifyCollectionChangedEventArgs(
            //        NotifyCollectionChangedAction.Reset));
        }

        protected void OnRemoveAllClocksDone(EventArgs e)
        {
            RemoveAllClocksDone?.Invoke(this, e);
        }

        protected void OnBeforeRemoveAllClocks(EventArgs e)
        {
            BeforeRemoveAllClocks?.Invoke(this, e);
        }

        internal void RefreshIDs()
        {
            int i = 0;
            foreach (ClockM td in Ms)
            {
                td.ID = i + 1;
                if (!td.FilteredOut)
                {
                    ++i;
                }
            }
        }

        private void OnClockMoved(ClockMovedEventArgs e)
        {
            ClockMoved?.Invoke(this, e);

            //CollectionChanged?.Invoke(this,
            //    new NotifyCollectionChangedEventArgs(
            //        NotifyCollectionChangedAction.Move, e.Clock, e.NewIndex, e.OldIndex));
        }

        internal void SubscribeToClock(ClockM td)
        {
            if (td is TimerData tdata)
            {
                tdata.TimerStartedByUser += Td_TimerStartedByUser;
                tdata.TimerStoppedByUser += Td_TimerStoppedByUser;
                tdata.TimerStopped += Tdata_TimerStopped;
            }

            if (td is AlarmData adata)
            {
                adata.EnabledChanged += Adata_EnabledChanged;
            }

            td.IsUnsavedChanged += Td_IsDirtyChanged; // TODO: [NEXT] BUG interesting: open a file with at least 2 timers (1? did not test), the subscription is done but when typing in the text box, the * does not appear in the title bar. More closely, Td_IsDirtyChanged above is not called. Also see OnTimerAdded above.
            td.FilteredOutChanged += Td_FilteredOutChanged;
            td.PropertyChanged += Td_PropertyChanged;
        }

        private void Tdata_TimerStopped(object sender, ClockEventArgs e)
        {
            TimerStopped?.Invoke(this, e);
        }

        private void Adata_EnabledChanged(object sender, ClockEventArgs e)
        {
            ApplyFilterToClock(e.Clock);

            DoSortByClosestRingingMoment();
        }

        private void Td_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ClockM cd = sender as ClockM;

            ClockPropertyChanged?.Invoke(sender, e);

            ReportGlobalProgress();

            if (e.PropertyName == "FilteredOut")
            {
                if (cd.FilteredOut)
                {
                    --VisibleCount;
                }
                else
                {
                    ++VisibleCount;
                }
            }

            ApplyFilterToClock(cd);

            if (e.PropertyName == "Tag")
            {
                DoSortAlphabetically();
            }
        }

        private void Td_FilteredOutChanged(object sender, ClockEventArgs e)
        {
            if (e.Clock.FilteredOut)
            {
                ClockFilteredOut?.Invoke(this, e);
            }
            else
            {
                ClockFilteredIn?.Invoke(this, e);
            }
        }

        private void Td_TimerStoppedByUser(object sender, ClockEventArgs e)
        {
            ApplyFilterToClock(e.Clock);

            DoSortByClosestRingingMoment();

            TimerStoppedByUser?.Invoke(this, e);
        }

        internal void UnsubscribeFromClock(ClockM td)
        {
            if (td is TimerData tdata)
            {
                tdata.TimerStartedByUser -= Td_TimerStartedByUser;
                tdata.TimerStoppedByUser -= Td_TimerStoppedByUser;
                tdata.TimerStopped -= Tdata_TimerStopped;
            }

            if (td is AlarmData adata)
            {
                adata.EnabledChanged -= Adata_EnabledChanged;
            }

            td.IsUnsavedChanged       -= Td_IsDirtyChanged;
            td.FilteredOutChanged     -= Td_FilteredOutChanged;
            td.PropertyChanged        -= Td_PropertyChanged;
        }

        /// <summary>
        /// Only sorts if the user activated sort by closest ringing moment.
        /// </summary>
        internal void DoSortByClosestRingingMoment()
        {
            if (AutosortMode == AutosortMode.ClosestRingingMoment)
            {
                SortByClosestRingingMoment();
            }
        }

        /// <summary>
        /// Requires a Filter applied to this ClockDataCollection.
        /// </summary>
        /// <param name="cd"></param>
        internal void ApplyFilterToClock(ClockM cd)
        {
            if (AppliedFilter == null)
            {
                throw new ArgumentNullException();
            }
            cd.FilteredOut = !ClockSatisfiesFilter(AppliedFilter, cd);
        }

        private void Td_TimerStartedByUser(object sender, ClockEventArgs e)
        {
            DoSortByClosestRingingMoment();

            ApplyFilterToClock(e.Clock);

            TimerStartedByUser?.Invoke(this, e);
        }

        /// <summary>
        /// Bubble sort (through swaps).
        /// </summary>
        /// <param name="comparison"></param>
        internal void SortTheList(Comparison<ClockM> comparison)
        {
            bool sorted = false;
            for (int i = 0; i < Ms.Count - 1 && !sorted; ++i)
            {
                sorted = true;
                for (int j = 0; j < Ms.Count - i - 1; ++j)
                {
                    int cmp = comparison(Ms[j], Ms[j + 1]);
                    if (cmp > 0)
                    {
                        sorted = false;
                        Ms.Move(j, j + 1);
                    }
                }
            }
        }

        /// <summary>
        /// NOTE: executed when a timer is started, or stopped, by user or automatically.
        /// Handlers set in DataFile class.
        /// </summary>
        internal void SortByClosestRingingMoment()
        {
            // (1) in model:
            Comparison<ClockM> comparison = delegate (ClockM td1, ClockM td2)
            {
                int retVal;

                var tdata1 = td1 as TimerData;
                var tdata2 = td2 as TimerData;
                if (tdata1 != null &&
                    tdata2 != null)
                {
                    if (tdata1.Running && tdata2.Running)
                    {
                        retVal = tdata1.CurrentTimeSpan.CompareTo(tdata2.CurrentTimeSpan);
                    }
                    else if (tdata1.Running)
                    {
                        retVal = -1;
                    }
                    else if (tdata2.Running)
                    {
                        retVal = 1;
                    }
                    else
                    {
                        retVal = 0;
                    }
                }
                // if one of td1 and td2 is not a timer but an alarm
                else if (tdata1 != null)
                {
                    var ad = td2 as AlarmData;
                    if (ad.Enabled && tdata1.Running)
                    {
                        retVal = TimeSpan.Compare(tdata1.CurrentTimeSpan,
                            ad.CurrentDateTime - DateTime.Now);
                    }
                    // if ad (the second) is activated, then they should be reversed with this 1
                    else if (ad.Enabled) // !tdata1.Running
                    {
                        retVal = 1;
                    }
                    else if (tdata1.Running) // !ad.Enabled
                    {
                        retVal = -1;
                    }
                    else
                    {
                        retVal = 0;
                    }
                }
                else if (tdata2 != null)
                {
                    var ad = td1 as AlarmData;
                    if (ad.Enabled && tdata2.Running)
                    {
                        retVal = TimeSpan.Compare(ad.CurrentDateTime - DateTime.Now,
                            tdata2.CurrentTimeSpan);
                    }
                    else if (ad.Enabled) // !tdata1.Running
                    {
                        retVal = -1;
                    }
                    else if (tdata2.Running) // !ad.Enabled
                    {
                        retVal = 1;
                    }
                    else
                    {
                        retVal = 0;
                    }
                }
                else // both are alarms
                {
                    var ad1 = td1 as AlarmData;
                    var ad2 = td2 as AlarmData;

                    if (ad1.Enabled && ad2.Enabled)
                    {
                        retVal = DateTime.Compare(
                            ad1.CurrentDateTime,
                            ad2.CurrentDateTime);
                    }
                    else if (ad1.Enabled)
                    {
                        retVal = -1;
                    }
                    else if (ad2.Enabled)
                    {
                        retVal = 1;
                    }
                    else
                    {
                        retVal = 0;
                    }
                }

                return retVal;
            };
            SortTheList(comparison);

            // TODO: mark is dirty only if smth moved
            IsUnsaved = true;
        }

        /// <summary>
        /// TODO: should be executed when a timer is added, removed or its tag is edited.
        /// Handlers should be set in DataFile class, like in the other sort mode.
        /// </summary>
        internal void SortAlphabetically()
        {
            // (1) in model:
            Comparison<ClockM> comparison = delegate (ClockM td1, ClockM td2)
            {
                return td1.Tag.ToPlainText().CompareTo(td2.Tag.ToPlainText());
            };
            SortTheList(comparison);

            // TODO: mark is dirty only if smth moved
            IsUnsaved = true;
        }
        
        internal AutosortMode AutosortMode
        {
            get
            {
                string s = MyDataFile.Settings.GetValue("AutosortMode") as string;

                return Utils.StrToEnum<AutosortMode>(s);
            }
            set
            {
                MyDataFile.Settings.SetValue("AutosortMode", value.ToString());
            }
        }

        internal int _VisibleCount = 0;
        public int VisibleCount
        {
            get
            {
                return _VisibleCount;
            }
            set
            {
                if (_VisibleCount != value)
                {
                    _VisibleCount = value;
                    RaisePropertyChanged("VisibleCount");
                }
            }
        }

        /// <summary>
        /// Only sorts if the user activated alphabetical sort.
        /// </summary>
        internal void DoSortAlphabetically()
        {
            if (AutosortMode == AutosortMode.Alphabetically)
            {
                SortAlphabetically();
            }
        }

        public bool Equals(ClockMCollection other)
        {
            if (other == null)
            {
                return false;
            }

            return Groups == other.Groups &&
                Ms.Count == other.Ms.Count &&
                Ms.SequenceEqual(other.Ms);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var cdc = obj as ClockMCollection;
            if (cdc == null)
                return false;
            else
                return Equals(cdc);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 19 + Groups.GetHashCode();
                foreach (ClockM cd in Ms)
                {
                    hash = hash * 31 + cd.GetHashCode();
                }
                return hash;
            }
        }

        public static bool operator ==(ClockMCollection cdc1, ClockMCollection cdc2)
        {
            if (((object)cdc1) == null || ((object)cdc2) == null)
                return object.Equals(cdc1, cdc2);

            return cdc1.Equals(cdc2);
        }

        public static bool operator !=(ClockMCollection cdc1, ClockMCollection cdc2)
        {
            return !(cdc1 == cdc2);
        }

        internal void AddClocks(params ClockM[] clocks)
        {
            foreach (ClockM cd in clocks)
            {
                if (Contains(cd))
                {
                    return;
                }

                AddClockUnchecked(cd);

                ++VisibleCount;

                IsUnsaved = true;
            }
            OnClocksAdded(new ClocksEventArgs()
            {
                Clocks = clocks
            });
            DoAllNeededSorts();
        }

        public int Count
        {
            get
            {
                return Ms.Count;
            }
        }

        public int IndexOf(ClockM cd)
        {
            return Ms.IndexOfReference(cd);
        }
    }
}
