using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace cs_timed_silver
{
    public class ClockVM : BindableBase
    {
        public ClockVMCollection Collection;

        public ClockM Model;

        public bool Enabled
        {
            get
            {
                return Model.Enabled;
            }
            set
            {
                if (value == Model.Enabled)
                {
                    return;
                }
                Model.Enabled = value;
                RaisePropertyChanged();
                RaisePropertyChanged("IsActive");
            }
        }

        private double _ProgressPercent = double.NaN;
        public double ProgressPercent
        {
            get { return _ProgressPercent; }
            set { SetProperty(ref _ProgressPercent, value); }
        }

        public bool IsActive
        {
            get
            {
                return Model.IsActive;
            }
        }

        public bool FilteredOut
        {
            get
            {
                return Model.FilteredOut;
            }
            set
            {
                if (value == Model.FilteredOut)
                {
                    return;
                }
                Model.FilteredOut = value;
                RaisePropertyChanged();
            }
        }

        public int ID
        {
            get
            {
                return Model.ID;
            }
            set
            {
                if (value == Model.ID)
                {
                    return;
                }
                Model.ID = value;
                RaisePropertyChanged();
            }
        }

        public string TimeOutBackgroundImageRelativePath
        {
            get
            {
                return Model.TimeOutBackgroundImageRelativePath;
            }
            set
            {
                if (value == Model.TimeOutBackgroundImageRelativePath)
                {
                    return;
                }
                Model.TimeOutBackgroundImageRelativePath = value;
                RaisePropertyChanged();
                RaisePropertyChanged("TimeOutBackgroundImageSource");
            }
        }

        public ImageSource TimeOutBackgroundImageSource
        {
            get
            {
                return Model.TimeOutBackgroundImageSource;
            }
        }

        public System.Drawing.Bitmap Icon
        {
            get
            {
                return Model.Icon;
            }
            set
            {
                if (value == Model.Icon)
                {
                    return;
                }
                Model.Icon = value;
                RaisePropertyChanged();
                RaisePropertyChanged("IconSource");
            }
        }

        public ImageSource IconSource
        {
            get
            {
                return Model.IconSource;
            }
        }

        public string GroupName
        {
            get
            {
                return Model.GroupName;
            }
            set
            {
                if (value == Model.GroupName)
                {
                    return;
                }
                Model.GroupName = value;
                RaisePropertyChanged();
            }
        }

        public string Tag
        {
            get
            {
                return Model.Tag;
            }
            set
            {
                if (value == Model.Tag)
                {
                    return;
                }
                Model.Tag = value;
                RaisePropertyChanged();
            }
        }

        public object CurrentValue
        {
            get
            {
                return Model.CurrentValue;
            }
            set
            {
                if (value == Model.CurrentValue)
                {
                    return;
                }
                Model.CurrentValue = value;
                RaisePropertyChanged();
                if (value is TimeSpan)
                {
                    RaisePropertyChanged("CurrentTimeSpan");
                }
                else
                {
                    RaisePropertyChanged("CurrentDateTime");
                }
            }
        }

        public TimeSpan CurrentTimeSpan
        {
            get
            {
                if (Model is AlarmData)
                {
                    return TimeSpan.Zero;
                    //throw new InvalidOperationException();
                }

                return (Model as TimerData).CurrentTimeSpan;
            }
            set
            {
                if (Model is AlarmData)
                {
                    return;
                    //throw new InvalidOperationException();
                }

                var t = Model as TimerData;
                if (value == t.CurrentTimeSpan)
                {
                    return;
                }
                t.CurrentTimeSpan = value;
                RaisePropertyChanged();
                RaisePropertyChanged("CurrentValue");
            }
        }

        public DateTime CurrentDateTime
        {
            get
            {
                if (Model is TimerData)
                {
                    return DateTime.Now;
                    //throw new InvalidOperationException();
                }

                return (Model as AlarmData).CurrentDateTime;
            }
            set
            {
                if (Model is TimerData)
                {
                    //throw new InvalidOperationException();
                    return;
                }

                var a = Model as AlarmData;
                if (value == a.CurrentDateTime)
                {
                    return;
                }
                a.CurrentDateTime = value;
                RaisePropertyChanged();
                RaisePropertyChanged("CurrentValue");
            }
        }

        public System.Drawing.Color UserBackColor
        {
            get
            {
                return Model.UserBackColor;
            }
            set
            {
                if (value == Model.UserBackColor)
                {
                    return;
                }
                Model.UserBackColor = value;
                RaisePropertyChanged();
            }
        }

        public bool IsUnsaved
        {
            get
            {
                return Model.IsUnsaved;
            }
            set
            {
                if (value == Model.IsUnsaved)
                {
                    return;
                }
                Model.IsUnsaved = value;
                RaisePropertyChanged();
            }
        }

        public bool ResetToValueLocked
        {
            get
            {
                return Model.ResetToValueLocked;
            }
            set
            {
                if (value == Model.ResetToValueLocked)
                {
                    return;
                }
                Model.ResetToValueLocked = value;
                RaisePropertyChanged();
            }
        }

        public object ResetToValue
        {
            get
            {
                return Model.ResetToValue;
            }
            set
            {
                if (value == Model.ResetToValue)
                {
                    return;
                }
                Model.ResetToValue = value;
                RaisePropertyChanged();
            }
        }

        public bool Checked
        {
            get
            {
                return Model.Checked;
            }
            set
            {
                if (value == Model.Checked)
                {
                    return;
                }
                Model.Checked = value;
                RaisePropertyChanged();
            }
        }

        public bool Checkable
        {
            get
            {
                return Model.Checkable;
            }
            set
            {
                if (value == Model.Checkable)
                {
                    return;
                }
                Model.Checkable = value;
                RaisePropertyChanged();
            }
        }

        public class ClockTypeChangedEventArgs : EventArgs
        {
            public ClockM OldClockM = null;
            public ClockM NewClockM = null;
            public ClockVM ClockVM = null;
        }

        public event EventHandler<ClockTypeChangedEventArgs> ClockTypeChanged;

        public enum ClockTypes
        {
            Alarm,
            Timer
        }

        internal void ChangeTypeOfClock(ClockM cd, Type newType)
        {
            Model.PropertyChanged -= Model_PropertyChanged;

            if (newType == typeof(TimerData))
            {
                if (cd.IsActive)
                {
                    cd.ActivateOrDeactivate();
                }

                var tdata = new TimerData(Model.MyDataFile, Model.MyDataFile.MultiAudioPlayer);

                tdata.GroupName = cd.GroupName;
                tdata.Icon = cd.Icon;
                tdata.Style = cd.Style;
                tdata.UserBackColor = cd.UserBackColor;
                tdata.Tag = cd.Tag;
                tdata.IsUnsaved = true;
                tdata.ID = cd.ID;
                tdata.Checkable = cd.Checkable;
                tdata.Checked = cd.Checked;

                Model = tdata;
            }
            else // AlarmData
            {
                if (cd.IsActive)
                {
                    cd.ActivateOrDeactivate();
                }

                var adata = new AlarmData(Model.MyDataFile, Model.MyDataFile.MultiAudioPlayer);

                adata.GroupName = cd.GroupName;
                adata.Icon = cd.Icon;
                adata.Style = cd.Style;
                adata.UserBackColor = cd.UserBackColor;
                adata.Tag = cd.Tag;
                adata.IsUnsaved = true;
                adata.ID = cd.ID;
                adata.Checkable = cd.Checkable;
                adata.Checked = cd.Checked;

                Model = adata;
            }

            Model.PropertyChanged += Model_PropertyChanged;

            RaisePropertyChanged("CurrentValue");
            RaisePropertyChanged("ResetToValue");
            RaisePropertyChanged("IsActive");
            RaisePropertyChanged("IsUnsaved");

            ClockTypeChanged?.Invoke(this, new ClockTypeChangedEventArgs()
            {
                ClockVM = this,
                NewClockM = Model,
                OldClockM = cd
            });

            IsUnsaved = true;
        }

        public ClockTypes ClockType
        {
            get
            {
                return Model is TimerData ? ClockTypes.Timer : ClockTypes.Alarm;
            }
            set
            {
                bool change = Model is TimerData && value == ClockTypes.Alarm ||
                              Model is AlarmData && value == ClockTypes.Timer;

                if (!change)
                {
                    return;
                }

                if (value == ClockTypes.Timer)
                {
                    ChangeTypeOfClock(Model, typeof(TimerData));
                }
                else
                {
                    ChangeTypeOfClock(Model, typeof(AlarmData));
                }

                RaisePropertyChanged();
            }
        }

        public int TotalSeconds
        {
            get
            {
                if (!(Model is TimerData))
                {
                    return -1;
                }
                return (Model as TimerData).GetSeconds();
            }
        }

        public ClockVM(ClockM model, ClockVMCollection coll)
        {
            Collection = coll;
            Model = model;

            Model.PropertyChanged += Model_PropertyChanged;

            Collection.SubscribeToClock(this);

            MyToggleCommand = new RelayCommand(OnToggle);
            MyDeleteCommand = new RelayCommand(OnDelete);
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
            if (e.PropertyName == "Running")
            {
                RaisePropertyChanged("IsActive");
            }

            if (ClockType == ClockTypes.Timer && ResetToValue != null && IsActive)
            {
                ProgressPercent = TotalSeconds /
                    ((TimeSpan)ResetToValue).TotalSeconds * 100d;
            }
            else
            {
                ProgressPercent = 0d;
            }
        }

        public RelayCommand MyToggleCommand { get; set; } = null;
        public void OnToggle()
        {
            Model.ActivateOrDeactivate();
        }

        public RelayCommand MyDeleteCommand { get; set; } = null;
        public void OnDelete()
        {
            Model.Delete();
        }

        public override string ToString()
        {
            return $"ClockVM: {Tag} | {CurrentValue} | {DateTime.Now}";
        }

        ~ClockVM() // TODO: this is never called because of strong event handler references,
            // so use Dispose pattern and manual Dispose call.
            // study weak events
        {
            Collection.UnsubscribeFromClock(this);
        }
    }
}
