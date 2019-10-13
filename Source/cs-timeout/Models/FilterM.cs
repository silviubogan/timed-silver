using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace cs_timed_silver
{
    public class FilterM : BindableBaseExtended
    {
        private bool _IsBaseFilter = false;
        public bool IsBaseFilter
        {
            get { return _IsBaseFilter; }
            set { SetProperty(ref _IsBaseFilter, value); }
        }

        private ObservableCollection<string> _GroupNames = null;
        public ObservableCollection<string> GroupNames
        {
            get { return _GroupNames; }
            set
            {
                SetProperty(ref _GroupNames, value);
            }
        }

        public string SearchString { get; set; }

        public bool ShowActive { get; set; } = true;
        public bool ShowInactive { get; set; } = true;
        public bool ShowTimers { get; set; } = true;
        public bool ShowAlarms { get; set; } = true;
        public ClockMCollection Clocks { get; set; }
        public ClockGroupMCollection Groups { get; set; }

        public bool Autocorrected { get; set; } = false;

        internal FilterM(ClockMCollection cdc, string s = "")
        {
            SearchString = "";
            GroupNames = new ObservableCollection<string>();
            Clocks = cdc;
            Groups = Clocks.Groups;

            List<string> l = s.Split(new char[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries).ToList();
            if (l.Contains("active"))
            {
                ShowActive = true;
                ShowInactive = false;
                l.RemoveAll(x => x == "active");
            }
            if (l.Contains("timers"))
            {
                ShowTimers = true;
                ShowAlarms = false;
                l.RemoveAll(x => x == "timers");
            }
            if (l.Contains("inactive"))
            {
                ShowActive = false;
                ShowInactive = true;
                l.RemoveAll(x => x == "inactive");
            }
            if (l.Contains("alarms"))
            {
                ShowTimers = false;
                ShowAlarms = true;
                l.RemoveAll(x => x == "alarms");
            }
            foreach (string str in l)
            {
                if (string.IsNullOrWhiteSpace(str))
                {
                    continue;
                }

                try
                {
                    int x = Convert.ToInt32(str);
                    if (1 <= x && x <= Groups.Ms.Count)
                    {
                        //GroupIndices.Add(x);
                        GroupNames.Add(Groups.Ms[x - 1].Name);
                    }
                    else
                    {
                        Autocorrected = true;
                    }
                }
                catch (FormatException /*ex*/)
                {
                    SearchString += str;
                    continue;
                }
            }

            SearchString = SearchString.Trim();

            DisplayString = GroupNames.FirstOrDefault();
        }

        public override string ToString()
        {
            string o = "";
            if (ShowActive && !ShowInactive)
            {
                o += "active ";
            }
            if (ShowInactive && !ShowActive)
            {
                o += "inactive ";
            }
            if (ShowTimers && !ShowAlarms)
            {
                o += "timers ";
            }
            if (ShowAlarms && !ShowTimers)
            {
                o += "alarms ";
            }
            foreach (string i in GroupNames)
            {
                o += $"{Groups.Ms.IndexOf(g => g.Name == i) + 1} ";
            }

            o += " " + SearchString;

            return o.Trim();
        }

        internal static FilterM Combine(FilterM f, FilterM f2)
        {
            FilterM c = f.Clone();

            c.ShowActive |= f2.ShowActive;
            c.ShowInactive |= f2.ShowInactive;
            c.ShowTimers |= f2.ShowTimers;
            c.ShowAlarms |= f2.ShowAlarms;

            c.GroupNames = new ObservableCollection<string>(
                Utils.AddEnumerablesDistinct(c.GroupNames, f2.GroupNames));

            string ss1 = c.SearchString;
            string ss2 = f2.SearchString;
            c.SearchString = ss1.Trim() + " " + ss2.Trim();

            return c;
        }

        internal FilterM Clone()
        {
            var f = new FilterM(Clocks, "");
            f.ShowActive = ShowActive;
            f.ShowInactive = ShowInactive;
            f.ShowTimers = ShowTimers;
            f.ShowAlarms = ShowAlarms;
            f.GroupNames = new ObservableCollection<string>(GroupNames);
            f.SearchString = SearchString;

            return f;
        }
        
        // TODO: in this class, Equals and ==, != operators etc. should
        // also compare the Groups collection of each CLock.
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            FilterM f1 = this, f2 = obj as FilterM;

            return f1.ShowActive == f2.ShowActive &&
                f1.ShowInactive == f2.ShowInactive &&
                f1.ShowTimers == f2.ShowTimers &&
                f1.ShowAlarms == f2.ShowAlarms &&
                f1.SearchString == f2.SearchString &&
                f1.GroupNames.Except(f2.GroupNames).Count() == 0 &&
                f2.GroupNames.Except(f1.GroupNames).Count() == 0;
        }
        
        public override int GetHashCode()
        {
            return ShowActive.GetHashCode() ^ ShowInactive.GetHashCode() ^
                ShowTimers.GetHashCode() ^ ShowAlarms.GetHashCode() ^
                GroupNames.GetHashCode() ^ SearchString.GetHashCode();
        }

        public static bool operator ==(FilterM f1, FilterM f2)
        {
            if (ReferenceEquals(f1, null))
            {
                return ReferenceEquals(f2, null);
            }

            return f1.Equals(f2);
        }

        public static bool operator !=(FilterM f1, FilterM f2)
        {
            return !(f1 == f2);
        }

        internal bool ShowsGroup(int gi)
        {
            int ngi = gi - 1;
            return GroupNames.Count == 0 ||
                ((ngi >= 0 && ngi < Groups.Ms.Count) && GroupNames.Contains(Groups.Ms[ngi].Name));
        }

        internal bool ShowsGroup(string g)
        {
            return GroupNames.Count == 0 || GroupNames.Contains(g);
            //return
            //    // if the group is "" then the clock is shown only when the filter does not ask for a specific group
            //    GroupIndices.Count == 0 || // && GroupIndices.Count == 0) ||
            //    // for each index in the filter, check if the respective group is the group of the checked clock
            //    GroupIndices.Any((x) =>
            //    {
            //        return Groups.Ms[x - 1].Name == g;
            //    });
        }

        private string _DisplayString = null;
        public string DisplayString
        {
            get { return _DisplayString; }
            set { SetProperty(ref _DisplayString, value); }
        }

        private ImageSource _MyConstantImageSource = null;
        public ImageSource MyConstantImageSource
        {
            get { return _MyConstantImageSource; }
            set
            {
                SetProperty(ref _MyConstantImageSource, value, new Action(() =>
                {
                    RaisePropertyChanged(null, null, nameof(MyImageSource));
                }));
            }
        }

        private ImageSource _MyEmptyImageSource = null;
        public ImageSource MyEmptyImageSource
        {
            get { return _MyEmptyImageSource; }
            set
            {
                SetProperty(ref _MyEmptyImageSource, value, new Action(() =>
                {
                    RaisePropertyChanged(null, null, nameof(MyImageSource));
                }));
            }
        }

        private ImageSource _MyNonEmptyImageSource = null;
        public ImageSource MyNonEmptyImageSource
        {
            get { return _MyNonEmptyImageSource; }
            set
            {
                SetProperty(ref _MyNonEmptyImageSource, value, new Action(() =>
                {
                    RaisePropertyChanged(null, null, nameof(MyImageSource));
                }));
            }
        }
        
        public ImageSource MyImageSource
        {
            get
            {
                if (MyConstantImageSource != null)
                {
                    return MyConstantImageSource;
                }
                return HasContent ? MyNonEmptyImageSource :
                    MyEmptyImageSource;
            }
        }

        private bool _IsSelected = false;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set { SetProperty(ref _IsSelected, value); }
        }

        private int _Items = 0;
        public int Items
        {
            get { return _Items; }
            set
            {
                SetProperty(ref _Items, value, new Action(() =>
                {
                    RaisePropertyChanged(null, null, nameof(HasContent));
                    RaisePropertyChanged(null, null, nameof(MyImageSource));
                }));
            }
        }

        public bool HasContent
        {
            get
            {
                return Items > 0;
            }
        }

        internal void UpdateItemCount()
        {
            int c = Clocks.OneTimeFilter(this).Count();
            Items = c;
        }
    }
}
