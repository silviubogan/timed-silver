using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace cs_timed_silver
{
    public class ClockGroupVM : BindableBase
    {
        public ClockGroupM M { get; private set; } = null;

        private string _DisplayString = "";
        public string DisplayString
        {
            get { return _DisplayString; }
            set { SetProperty(ref _DisplayString, value); }
        }

        private Brush _Foreground = Brushes.Black;
        public Brush Foreground
        {
            get { return _Foreground; }
            set { SetProperty(ref _Foreground, value); }
        }

        internal FontStyle _FontStyle = FontStyles.Normal;
        public FontStyle FontStyle
        {
            get { return _FontStyle; }
            set { SetProperty(ref _FontStyle, value); }
        }

        private ImageSource _Icon = null;
        public ImageSource Icon
        {
            get { return _Icon; }
            set { SetProperty(ref _Icon, value); }
        }

        private string _Name = "";
        public string Name
        {
            get { return _Name; }
            set { SetProperty(ref _Name, value); }
        }

        private bool _IsSelected = false;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set { SetProperty(ref _IsSelected, value); }
        }

        public ClockGroupVM(ClockGroupM m) : base()
        {
            M = m;
            M.PropertyChanged += M_PropertyChanged;

            SynchronizeFromModel();
            PropertyChanged += ClockGroupVM_PropertyChanged;
        }

        private void ClockGroupVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SynchronizeToModel();
        }

        private void M_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SynchronizeFromModel();
        }

        private void SynchronizeFromModel()
        {
            Icon = M.Icon;
            Name = M.Name;
            DisplayString = M.Name;
            IsSelected = M.IsSelected;
        }

        private void SynchronizeToModel()
        {
            M.Icon = Icon;
            M.Name = Name;
            M.IsSelected = IsSelected;
        }

        public override bool Equals(object obj)
        {
            var o = obj as ClockGroupVM;

            if (ReferenceEquals(o, null))
            {
                return false;
            }

            return base.Equals(obj) &&
                DisplayString == o.DisplayString &&
                FontStyle == o.FontStyle &&
                Foreground == o.Foreground;
        }

        public static bool operator ==(ClockGroupVM m1,
            ClockGroupVM m2)
        {
            if (ReferenceEquals(m1, null))
            {
                return ReferenceEquals(m2, null);
            }
            return m1.Equals(m2);
        }

        public static bool operator !=(ClockGroupVM m1,
            ClockGroupVM m2)
        {
            return !(m1 == m2);
        }

        public override string ToString()
        {
            return DisplayString;
        }
    }
}
