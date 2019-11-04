using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace cs_timed_silver
{
    public class ClockGroupVMCollection : BindableBase
    {
        public ClockGroupMCollection MyModel;
        public ObservableCollection<ClockGroupVM> VMs { get; set; }

        public ClockMCollection MyClockDataCollection;

        public ClockGroupVMCollection(ClockMCollection cdc)
        {
            MyClockDataCollection = cdc;
            MyModel = new ClockGroupMCollection(MyClockDataCollection);
            MyModel.Ms.CollectionChanged += MyModel_CollectionChanged;

            VMs = new ObservableCollection<ClockGroupVM>();
            VMs.CollectionChanged += VMs_CollectionChanged;
        }

        private void VMs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (SynchDisabled1)
            {
                return;
            }
            SynchDisabled1 = true;

            Utils.SynchronizeCollectionChange(VMs, e, MyModel.Ms,
                null,
                null,
                (ClockGroupVM vm, ClockGroupM m) =>
                {
                    return vm.Name == m.Name;
                },
                (ClockGroupVM vm) =>
                {
                    return vm.M;
                });

            //Utils.SynchronizeCollectionChange(Model.Ms, e, VMs,
            //    (ClockVM vm) =>
            //    {
            //    },
            //    (ClockM m) =>
            //    {
            //        VMForM(m).ClockTypeChanged -= ClockVMCollection_ClockTypeChanged;
            //    },
            //    (ClockM m, ClockVM vm) =>
            //    {
            //        return vm.Model == m;
            //    },
            //    (ClockM m) =>
            //    {
            //        int idx = Model.Ms.IndexOf(x => ReferenceEquals(x, m)); // ClockVM subclasses ClockM
            //        if (0 <= idx && idx <= Model.Ms.Count - 1)
            //        {
            //            MToVM[m] = new ClockVM(m, this);
            //            MToVM[m].ClockTypeChanged += ClockVMCollection_ClockTypeChanged;
            //            //VMs.Insert(idx, MToVM[m]);
            //            return MToVM[m];
            //        }
            //        return null;

            //        //// indicele 1
            //        //int idx = Model.Ms.IndexOf(x => ReferenceEquals(x, m)); // ClockVM subclasses ClockM
            //        //if (!(idx >= 0 && idx <= Model.Ms.Count - 1))
            //        //{
            //        //    // indicele 2
            //        //    if (MToVM.ContainsKey(m))
            //        //    {
            //        //        idx = MToVM.Keys.ToList().IndexOf(m);

            //        //        MToVM[m] = new ClockVM(m, this);
            //        //        MToVM[m].ClockTypeChanged += ClockVMCollection_ClockTypeChanged;
            //        //        //VMs.Insert(idx, MToVM[m]);
            //        //        return MToVM[m];
            //        //    }
            //        //    return null;
            //        //}
            //        //return null;
            //    });

            //switch (e.Action)
            //{
            //    case NotifyCollectionChangedAction.Add:
            //    //    foreach (object item in e.NewItems)
            //    //    {
            //    //        var mItem = (ClockGroupVM)item;
            //    //        int idx = MyModel.Ms.IndexOf(mItem);

            //    //        // the operation could have been either Insert or Add
            //    //        if (0 <= idx && idx <= VMs.Count - 1)
            //    //        {
            //    //            MyModel.Ms.Insert(idx, mItem);
            //    //        }
            //    //        else
            //    //        {
            //    //            MyModel.Add(mItem);
            //    //        }
            //    //    }
            //        break;

            //    case NotifyCollectionChangedAction.Remove:
            //        //foreach (object item in e.OldItems)
            //        //{
            //        //    var vmItem = item as ClockGroupVM;

            //        //    // find VM objects that wrap the relevant model object and remove them
            //        //    IEnumerable<ClockGroupM> query;
            //        //    while ((query = from vm in MyModel.Ms
            //        //                    where vm.Name == vmItem.Name
            //        //                    select vm).Count() > 0)
            //        //    {
            //        //        ClockGroupM mItem = query.First();
            //        //        int index = MyModel.Ms.IndexOf(vmItem);
            //        //        MyModel.Remove(vmItem.Name);
            //        //    }
            //        //}
            //        break;

            //    case NotifyCollectionChangedAction.Reset:
            //        MyModel.Clear();
            //        break;

            //    case NotifyCollectionChangedAction.Move:
            //        // TODO: handle multiple items
            //        MyModel.Ms.Move(e.OldStartingIndex, e.NewStartingIndex);
            //        break;

            //    case NotifyCollectionChangedAction.Replace:
            //        // TODO: handle multiple items
            //        MyModel.Ms[e.OldStartingIndex] = (ClockGroupM)e.NewItems[0];
            //        break;

            //    default:
            //        throw new NotImplementedException();
            //}

            SynchDisabled1 = false;
        }

        public bool SynchDisabled2 { get; set; } = false;

        private void MyModel_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (SynchDisabled1)
            {

            }
            else
            {
                SynchDisabled1 = true;

                Utils.SynchronizeCollectionChange(MyModel.Ms, e, VMs,
                    afterAddition: null,
                    prepareDeletion: null,
                    equalsWithinTargetTo: (ClockGroupM m, ClockGroupVM vm) =>
                    {
                        return vm.Name == m.Name;
                    },
                    toTarget: (ClockGroupM m) =>
                    {
                        foreach (ClockGroupVM vm in VMs)
                        {
                            if (object.ReferenceEquals(vm.M, m))
                            {
                                return vm;
                            }
                        }

                        return new ClockGroupVM(m);
                    });

                SynchDisabled1 = false;
            }


            if (SynchDisabled2)
            {

            }
            else
            {
                SynchDisabled2 = true;

                Utils.SynchronizeCollectionChange(MyModel.Ms, e, MyModel.Clocks.Filters.Ms,
                    afterAddition: null,
                    prepareDeletion: null,
                    equalsWithinTargetTo: (ClockGroupM m, FilterM f) =>
                    {
                        if (f.GroupNames.Count == 0)
                        {
                            return false;
                        }
                        return m.Name == f.GroupNames[0];
                    },
                    toTarget: (ClockGroupM m) =>
                    {
                        foreach (FilterM f in MyModel.Clocks.Filters.Ms)
                        {
                            if (f.GroupNames.Count < 1)
                            {
                                continue;
                            }
                            if (f.GroupNames[0] == m.Name)
                            {
                                return f;
                            }
                        }

                        return new FilterM(MyModel.Clocks, $"{MyModel.Clocks.Groups.IndexOf(m.Name) + 1}");
                    },
                    startingIndexInSource: 0,
                    startingIndexInTarget: 5);

                SynchDisabled2 = false;
            }
            //switch (e.Action)
            //{
            //    case NotifyCollectionChangedAction.Add:
            //        foreach (object item in e.NewItems)
            //        {
            //            var vmItem = new ClockGroupVM((ClockGroupM)item);

            //            // the operation could have been either Insert or Add
            //            if (0 <= e.NewStartingIndex &&
            //                e.NewStartingIndex <= MyModel.Ms.Count)
            //            {
            //                VMs.Insert(e.NewStartingIndex, vmItem);
            //            }
            //            else // e.NewStartingIndex < 0, so addition
            //            {
            //                VMs.Insert(MyModel.Ms.Count - 1, vmItem);
            //            }
            //        }
            //        break;

            //    case NotifyCollectionChangedAction.Remove:
            //        foreach (object item in e.OldItems)
            //        {
            //            // find VM objects that wrap the relevant model object and remove them
            //            IEnumerable<ClockGroupVM> query;
            //            while ((query = from vm in VMs
            //                            where vm.Name == ((ClockGroupM)item).Name
            //                            select vm).Count() > 0)
            //            {
            //                ClockGroupVM vmItem = query.First();
            //                int index = VMs.IndexOf(vmItem);
            //                VMs.Remove(vmItem);
            //            }
            //        }
            //        break;

            //    case NotifyCollectionChangedAction.Reset:
            //        for (int i = VMs.Count - 2; i >= 0; --i)
            //        {
            //            VMs.RemoveAt(i);
            //        }
            //        break;

            //    case NotifyCollectionChangedAction.Move:
            //        // TODO: handle multiple items
            //        VMs.Move(e.OldStartingIndex, e.NewStartingIndex);
            //        break;

            //    case NotifyCollectionChangedAction.Replace:
            //        // TODO: handle multiple items
            //        VMs[e.OldStartingIndex] = new ClockGroupVM((ClockGroupM)e.NewItems[0]);
            //        break;

            //    default:
            //        throw new NotImplementedException();
            //}
        }

        protected bool SynchDisabled1 = false;
    }
}
