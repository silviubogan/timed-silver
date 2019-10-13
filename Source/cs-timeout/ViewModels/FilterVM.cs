using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;

namespace cs_timed_silver
{
    public class FilterVM : BindableBase
    {
        public FilterM M { get; private set; } = null;

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

        internal string _DisplayString = null;
        public string DisplayString
        {
            get { return _DisplayString; }
            set { SetProperty(ref _DisplayString, value); }
        }

        internal ImageSource _MyConstantImageSource = null;
        public ImageSource MyConstantImageSource
        {
            get { return _MyConstantImageSource; }
            set
            {
                SetProperty(ref _MyConstantImageSource, value, new Action(() =>
                {
                    RaisePropertyChanged("MyImageSource");
                }));
            }
        }

        internal ImageSource _MyEmptyImageSource = null;
        public ImageSource MyEmptyImageSource
        {
            get { return _MyEmptyImageSource; }
            set
            {
                SetProperty(ref _MyEmptyImageSource, value, new Action(() =>
                {
                    RaisePropertyChanged("MyImageSource");
                }));
            }
        }

        internal ImageSource _MyNonEmptyImageSource = null;
        public ImageSource MyNonEmptyImageSource
        {
            get { return _MyNonEmptyImageSource; }
            set
            {
                SetProperty(ref _MyNonEmptyImageSource, value, new Action(() =>
                {
                    RaisePropertyChanged("MyImageSource");
                }));
            }
        }

        public ImageSource MyImageSource
        {
            get
            {
                return M.MyImageSource;
            }
        }

        internal bool _IsSelected = false;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set { SetProperty(ref _IsSelected, value); }
        }

        internal int _Items = 0;
        public int Items
        {
            get { return _Items; }
            set
            {
                SetProperty(ref _Items, value, new Action(() =>
                {
                    RaisePropertyChanged("HasContent");
                    RaisePropertyChanged("MyImageSource");
                }));
            }
        }

        public bool HasContent
        {
            get
            {
                return M.HasContent;
            }
        }

        internal void UpdateItemCount()
        {
            M.UpdateItemCount();
        }

        public FilterVM(FilterM m)
        {
            M = m;
            M.PropertyChanged += M_PropertyChanged;

            SynchronizeFromModel();

            PropertyChanged += FilterVM_PropertyChanged;
        }

        private void M_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SynchronizeFromModel();
        }

        private void FilterVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SynchronizeToModel();
        }

        public void SynchronizeFromModel()
        {
            DisplayString = M.DisplayString;
            GroupNames = M.GroupNames;
            IsBaseFilter = M.IsBaseFilter;
            IsSelected = M.IsSelected;
            Items = M.Items;
            MyConstantImageSource = M.MyConstantImageSource;
            MyEmptyImageSource = M.MyEmptyImageSource;
            MyNonEmptyImageSource = M.MyNonEmptyImageSource;
        }

        private void SynchronizeToModel()
        {
            M.GroupNames = M.GroupNames;

            M.IsBaseFilter = IsBaseFilter;

            M.DisplayString = DisplayString;

            //if (!string.IsNullOrEmpty(DisplayString))
            //{
            //    M.GroupNames.Add(DisplayString);
            //}

            M.IsSelected = IsSelected;
            M.Items = Items;
            M.MyConstantImageSource = MyConstantImageSource;
            M.MyEmptyImageSource = MyEmptyImageSource;
            M.MyNonEmptyImageSource = MyNonEmptyImageSource;

            //UpdateItemCount();
        }
    }
}