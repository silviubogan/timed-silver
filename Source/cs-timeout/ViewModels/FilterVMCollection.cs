using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace cs_timed_silver
{
    public class FilterVMCollection : BindableBase
    {
        public ObservableCollection<FilterVM> VMs { get; set; }

        internal FilterMCollection MyModel;

        internal ClockMCollection MyClocks;

        public FilterVMCollection(ClockMCollection c)
        {
            MyClocks = c;

            MyModel = new FilterMCollection(MyClocks);
        }

        public void Init()
        {
            MyClocks.Filters.Initialize();

            MyModel.Ms.CollectionChanged += MyModel_Ms_CollectionChanged;

            VMs = new ObservableCollection<FilterVM>();
            VMs.CollectionChanged += VMs_CollectionChanged;

            MyClocks.GroupsVM.VMs.CollectionChanged += GroupsVMs_CollectionChanged;

            var v1 = new FilterM(MyClocks.MyDataFile.ClockVMCollection.Model, "")
            {
                ShowActive = true,
                ShowInactive = true,
                ShowTimers = true,
                ShowAlarms = true,
                DisplayString = "All",
                MyConstantImageSource = new BitmapImage(
                    new Uri("/Resources/show-all-icon.png", UriKind.Relative)),
                IsBaseFilter = true
            };

            MyModel.Ms.Add(v1);

            MyModel.Ms.Add(new FilterM(MyClocks.MyDataFile.ClockVMCollection.Model, "")
            {
                ShowActive = true,
                ShowInactive = false,
                ShowTimers = true,
                ShowAlarms = true,
                DisplayString = "Active",
                MyConstantImageSource = new BitmapImage(
                    new Uri("/Resources/on filter.ico", UriKind.Relative)),
                IsBaseFilter = true
            });

            MyModel.Ms.Add(new FilterM(MyClocks.MyDataFile.ClockVMCollection.Model, "")
            {
                ShowActive = false,
                ShowInactive = true,
                ShowTimers = true,
                ShowAlarms = true,
                DisplayString = "Inactive",
                MyConstantImageSource = new BitmapImage(
                    new Uri("/Resources/off filter.ico", UriKind.Relative)),
                IsBaseFilter = true
            });

            MyModel.Ms.Add(new FilterM(MyClocks.MyDataFile.ClockVMCollection.Model, "")
            {
                ShowActive = true,
                ShowInactive = true,
                ShowTimers = true,
                ShowAlarms = false,
                DisplayString = "Timers",
                MyConstantImageSource = new BitmapImage(
                    new Uri("/Resources/timers filter (clepsidra 4).ico", UriKind.Relative)),
                IsBaseFilter = true
            });

            MyModel.Ms.Add(new FilterM(MyClocks.MyDataFile.ClockVMCollection.Model, "")
            {
                ShowActive = true,
                ShowInactive = true,
                ShowTimers = false,
                ShowAlarms = true,
                DisplayString = "Alarms",
                MyConstantImageSource = (DrawingImage)App.Current.MainWindow.FindResource("alarmClockDrawingImage"),
                IsBaseFilter = true
            });
        }

        private void GroupsVMs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //if (SynchDisabled2)
            //{
            //    return;
            //}

            //SynchDisabled2 = true;

            //Utils.SynchronizeCollectionChange(MyClocks.GroupsVM.VMs, e, MyModel.Ms,
            //    afterAddition: null,
            //    prepareDeletion: null,
            //    equalsWithinTargetTo: new Func<ClockGroupVM, FilterM, bool>
            //        (ClockGroupVMEqualsWithinTargetToFilterM),
            //    toTarget: new Func<ClockGroupVM, FilterM>(ClockGroupVMToFilterM),
            //    startingIndexInSource: 0,
            //    startingIndexInTarget: 5);

            //SynchDisabled2 = false;
        }

        public static bool ClockGroupVMEqualsWithinTargetToFilterM(ClockGroupVM vm, FilterM m)
        {
            return m.GroupNames.Count > 0 && m.ShowsGroup(vm.Name);
        }

        public FilterM ClockGroupVMToFilterM(ClockGroupVM vm)
        {
            var f = new FilterM(MyClocks.MyDataFile.ClockVMCollection.Model,
                $"{MyClocks.GroupsVM.VMs.IndexOf(vm) + 1}")
            {
                ShowActive = true,
                ShowInactive = true,
                ShowAlarms = true,
                ShowTimers = true
            };

            return f;
        }

        private void VMs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (SynchDisabled3)
            {
                return;
            }

            SynchDisabled3 = true;

            Utils.SynchronizeCollectionChange(VMs, e, MyModel.Ms,
                afterAddition: null,
                prepareDeletion: null,
                equalsWithinTargetTo: new Func<FilterVM, FilterM, bool>((FilterVM vm, FilterM m) =>
                {
                    return vm.M == m;
                }),
                toTarget: new Func<FilterVM, FilterM>((FilterVM vm) =>
                {
                    return vm.M;
                }));

            SynchDisabled3 = false;
        }

        public bool /*SynchDisabled2 = false,*/
            SynchDisabled3 = false;

        public ImageSource MyEmptyFolderIcon { get; set; } =
            new BitmapImage(new Uri("/Resources/pictograma folder 2.png",
        UriKind.Relative));
        public ImageSource MyNonEmptyFolderIcon { get; set; } =
            new BitmapImage(new Uri("/Resources/pictograma folder cu continut.png",
                UriKind.Relative));

        private void MyModel_Ms_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!SynchDisabled3)
            {
                SynchDisabled3 = true;

                Utils.SynchronizeCollectionChange(MyModel.Ms, e, VMs,
                    afterAddition: null,
                    prepareDeletion: null,
                    equalsWithinTargetTo: new Func<FilterM, FilterVM, bool>((FilterM m, FilterVM vm) =>
                    {
                        return vm.M == m;
                    }),
                    toTarget: new Func<FilterM, FilterVM>((FilterM m) =>
                    {
                        foreach (FilterVM vm in VMs)
                        {
                            if (vm.M == m) { return vm; }
                        }

                        var rv = new FilterVM(m)
                        {
                            DisplayString = m.DisplayString,
                            MyConstantImageSource = m.MyConstantImageSource,
                            MyEmptyImageSource = m.MyEmptyImageSource,
                            MyNonEmptyImageSource = m.MyNonEmptyImageSource,
                            Items = m.Items
                        };

                        if (!m.IsBaseFilter)
                        {
                            rv.MyEmptyImageSource = MyEmptyFolderIcon;
                            rv.MyNonEmptyImageSource = MyNonEmptyFolderIcon;
                            rv.DisplayString = m.GroupNames.FirstOrDefault();
                        }

                        return rv;
                    }));

                SynchDisabled3 = false;
            }

        //    if (!SynchDisabled2)
        //    {
        //        SynchDisabled2 = true;

        //        Utils.SynchronizeCollectionChange(MyModel.Ms, e, MyClocks.GroupsVM.VMs,
        //            afterAddition: null,
        //            prepareDeletion: null,
        //            equalsWithinTargetTo: new Func<FilterM, ClockGroupVM, bool>((FilterM fm, ClockGroupVM gm) =>
        //            {
        //                return fm.SearchString == gm.Name;

        //            }),
        //            toTarget: new Func<FilterM, ClockGroupVM>((FilterM m) =>
        //            {
        //                if (m.GroupNames.Count != 1)
        //                {
        //                    return null;
        //                }

        //                //return new FilterVM()
        //                //{
        //                // //DisplayString = m.
        //                // MyFilter = m
        //                //};
        //                var gm = new ClockGroupVM(new ClockGroupM()
        //                {
        //                    Name = m.GroupNames[0],
        //                    IsSelected = m.IsSelected,
        //                    Icon = m.MyImageSource
        //                });

        //                //var f = new FilterM(MyClocks.MyDataFile.ClockVMCollection.Model,
        //                //    $"{MyModel.Ms.IndexOf(m) + 1}")
        //                //{
        //                //    ShowActive = true,
        //                //    ShowInactive = true,
        //                //    ShowAlarms = true,
        //                //    ShowTimers = true
        //                //};

        //                return gm;
        //            }),
        //            startingIndexInSource: 5,
        //            startingIndexInTarget: 0);

        //        SynchDisabled2 = false;
        //    }
        }
    }
}
