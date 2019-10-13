using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_timed_silver
{
    public class FilterMCollection : BindableBase
    {
        public ObservableCollection<FilterM> Ms;

        internal ClockMCollection MyClocks;

        public FilterMCollection(ClockMCollection c)
        {
            MyClocks = c;

            Ms = new ObservableCollection<FilterM>();
            Ms.CollectionChanged += Ms_CollectionChanged;
        }

        private OcPropertyChangedListener<FilterM> l1;
        private OcPropertyChangedListener<ClockGroupM> l2;

        public void Initialize()
        {
            l1 = OcPropertyChangedListener.Create(MyClocks.Filters.Ms);
            l1.PropertyChanged += l1_PropertyChanged;

            l2 = OcPropertyChangedListener.Create(MyClocks.Groups.Ms);
            l2.PropertyChanged += l2_PropertyChanged;
        }

        private void l2_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var g = sender as ClockGroupM;

            var x = (PropertyChangedExtendedEventArgs)e;

            if (e.PropertyName == "Name")
            {
                string oldGroupName = (string)x.OldValue;
                string newGroupName = (string)x.NewValue;

                foreach (FilterM f in MyClocks.Filters.Ms)
                {
                    if (SyncClockGroupMToFilterM(oldGroupName, g, f))
                    {
                        return;
                    }
                }
            }
            else
            {
                string oldGroupName = g.Name;
                string newGroupName = g.Name;

                foreach (FilterM f in MyClocks.Filters.Ms)
                {
                    if (SyncClockGroupMToFilterM(oldGroupName, g, f))
                    {
                        return;
                    }
                }
            }
        }

        private bool SyncClockGroupMToFilterM(string oldGName, ClockGroupM g, FilterM f)
        {
            if (!f.IsBaseFilter && g.Name == f.GroupNames.FirstOrDefault())
            {
                f.DisplayString = g.Name;
                f.IsSelected = g.IsSelected;
                f.IsBaseFilter = false;

                f.MyConstantImageSource = g.Icon;
                f.MyEmptyImageSource = MyClocks.FiltersVM.MyEmptyFolderIcon;
                f.MyNonEmptyImageSource = MyClocks.FiltersVM.MyNonEmptyFolderIcon;

                f.GroupNames.Clear();
                f.GroupNames.Add(g.Name);

                f.UpdateItemCount();
            }
            else
            {
                if (!f.GroupNames.Contains(oldGName))
                {
                    return false;
                }

                var ng = new ObservableCollection<string>(f.GroupNames);

                ng.Remove(oldGName);
                ng.Add(g.Name);

                f.GroupNames = ng;
                f.DisplayString = g.Name;
            }

            return true;
        }

        private void l1_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var f = sender as FilterM;

            var x = (PropertyChangedExtendedEventArgs)e;

            string ov = f.GroupNames.FirstOrDefault();

            if (x.PropertyName == "GroupNames")
            {
                ov = ((ObservableCollection<string>)x.OldValue).FirstOrDefault();
            }


            if (f.GroupNames.Count == 0 || f.GroupNames.Count > 1)
            {
                return;
            }

            foreach (ClockGroupM g in MyClocks.Groups.Ms)
            {
                if (g.Name == ov)
                {
                    g.Name = f.GroupNames.FirstOrDefault();
                    g.Icon = f.MyConstantImageSource;
                    g.IsSelected = f.IsSelected;
                    break;
                }
            }
        }

        private void Ms_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

            if (MyClocks.GroupsVM.SynchDisabled2)
            {

            }
            else
            {
                MyClocks.GroupsVM.SynchDisabled2 = true;

                Utils.SynchronizeCollectionChange(Ms, e, MyClocks.Groups.Ms,
                    afterAddition: null,
                    prepareDeletion: null,
                    equalsWithinTargetTo: (FilterM f, ClockGroupM g) =>
                    {
                        if (f.GroupNames.Count == 0)
                        {
                            return false;
                        }
                        return g.Name == f.DisplayString;
                    },
                    toTarget: (FilterM m) =>
                    {
                        if (m.GroupNames.Count == 0)
                        {
                            return null;
                        }

                        foreach (ClockGroupM g in MyClocks.Groups.Ms)
                        {
                            if (m.GroupNames.Count < 1)
                            {
                                continue;
                            }
                            if (m.GroupNames[0] == g.Name)
                            {
                                return g;
                            }
                        }

                        return new ClockGroupM()
                        {
                            Name = m.DisplayString,
                            Icon = m.MyImageSource,
                            IsSelected = m.IsSelected
                        };
                    },
                    startingIndexInSource: 5,
                    startingIndexInTarget: 0);

                MyClocks.GroupsVM.SynchDisabled2 = false;
            }
        }
    }
}
