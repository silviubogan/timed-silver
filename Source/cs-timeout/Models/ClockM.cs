using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;

namespace cs_timed_silver
{
    public abstract class ClockM : BindableBase, IDisposable, IUnsavedStatusCapable, IEquatable<ClockM>
    {
        public event EventHandler<ClockEventArgs> FilteredOutChanged;
        public event EventHandler IsUnsavedChanged, CurrentValueChanged,
            GroupNameChanged;
        public event EventHandler Deleted;

        internal bool _Enabled = true;
        public virtual bool Enabled
        {
            get
            {
                return _Enabled;
            }
            set
            {
                if (_Enabled != value)
                {
                    _Enabled = value;
                    if (!SupressPropertyChangedEvents)
                    {
                        RaisePropertyChanged();
                        RaisePropertyChanged("IsActive");
                    }
                }
            }
        }
        
        internal System.Windows.Threading.DispatcherTimer t2;

        public bool IsUnsavedLocked { get; set; } = false;

        internal bool _FilteredOut = false;
        public bool FilteredOut
        {
            get
            {
                return _FilteredOut;
            }
            set
            {
                if (_FilteredOut != value)
                {
                    _FilteredOut = value;
                    if (_FilteredOut)
                    {
                        Checked = false;
                    }
                    if (!SupressPropertyChangedEvents)
                    {
                        OnPropertyChanged();
                    }
                    FilteredOutChanged?.Invoke(this, new ClockEventArgs()
                    {
                        Clock = this
                    });
                }
            }
        }

        internal int _ID = 0;
        public int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                if (_ID != value)
                {
                    _ID = value;
                    if (!SupressPropertyChangedEvents)
                    {
                        OnPropertyChanged();
                    }
                }
            }
        }

        internal void Reset()
        {
            CurrentValue = ResetToValue;
        }
        
        internal Bitmap _Icon = null;
        public Bitmap Icon
        {
            get
            {
                return _Icon;
            }
            set
            {
                if (_Icon != value)
                {
                    _Icon = value;
                    IsUnsaved = true;
                    if (!SupressPropertyChangedEvents)
                    {
                        OnPropertyChanged();
                        OnPropertyChanged("IconSource");
                    }
                }
            }
        }

        public ImageSource IconSource
        {
            get
            {
                return Utils.GetBitmapImageFromBitmap(Icon);
            }
        }

        protected string _TimeOutBackgroundImageRelativePath = null;
        /// <summary>
        /// Relative to the assembly.
        /// Use: Uri.MakeRelativeUri(Uri) : what, appended to `this`, gives the argument.
        /// </summary>
        public string TimeOutBackgroundImageRelativePath
        {
            get { return _TimeOutBackgroundImageRelativePath; }
            set {
                SetProperty(ref _TimeOutBackgroundImageRelativePath, value, new Action(() =>
                {
                    RaisePropertyChanged(nameof(TimeOutBackgroundImageSource));
                }));
            }
        }

        public ImageSource TimeOutBackgroundImageSource
        {
            get
            {
                if (string.IsNullOrEmpty(TimeOutBackgroundImageRelativePath))
                {
                    return null;
                }

                try
                {
                    return new BitmapImage(new Uri(TimeOutBackgroundImageRelativePath, UriKind.Absolute));
                }
                catch
                {
                    return null;
                }
            }
        }

        public enum ClockStyles
        {
            ShowIcon,
            ShowID,
            ShowIconAndID
        }

        internal ClockStyles _Style = ClockStyles.ShowID;
        public ClockStyles Style
        {
            get
            {
                return _Style;
            }
            set
            {
                if (_Style != value)
                {
                    _Style = value;
                    IsUnsaved = true;
                    if (!SupressPropertyChangedEvents)
                    {
                        OnPropertyChanged();
                    }
                }
            }
        }

        internal string _GroupName = "";
        public string GroupName
        {
            get
            {
                return _GroupName;
            }
            set
            {
                if (_GroupName != value)
                {
                    _GroupName = value;
                    IsUnsaved = true;
                    if (!SupressPropertyChangedEvents)
                    {
                        OnPropertyChanged();
                    }
                    OnGroupNameChanged();
                }
            }
        }

        /// <summary>
        /// Only use this method when `this` clock is
        /// disabled.
        /// </summary>
        internal void ShowTimeOutForm()
        {
            string pt = Tag.ToPlainText();

            if ((bool)MyDataFile.Settings.GetValue("EnableRingingNotification"))
            {
                MyDataFile.MainWindow.MyNotifyIcon.ShowBalloonTip(5000, "Time out!",
                    pt.Length == 0 ? "-" : pt,
                    ToolTipIcon.Info);
            }

            MyDataFile.MainWindow.MyTimeOutFormsManager.ShowForm(this);

            string t = GetType() == typeof(AlarmData) ? "Alarm" : "Timer";
            MyDataFile.MainWindow.MyStatusBar.PostMessage(
                $"{t} #{ID} <{pt}> started ringing.",
                LogCategory.Information);
        }

        protected virtual void OnGroupNameChanged()
        {
            GroupNameChanged?.Invoke(this, EventArgs.Empty);
        }

        internal MultiAudioPlayer MultiAudioPlayer = null;

        internal bool ChangeFromClockControlView { get; set; } = false;

        internal bool SupressPropertyChangedEvents { get; set; } = false;

        internal FlowDocument _Tag;
        public FlowDocument Tag
        {
            get
            {
                return _Tag;
            }
            set
            {
                if (value != _Tag &&
                    XamlWriter.Save(value) != XamlWriter.Save(_Tag))
                {
                    _Tag = value.Clone();

                    IsUnsaved = true;
                    if (!SupressPropertyChangedEvents)
                    {
                        OnPropertyChanged();
                    }
                }
            }
        }

        internal object _CurrentValue = null;
        public object CurrentValue
        {
            get
            {
                return _CurrentValue;
            }
            set
            {
                if (value != _CurrentValue)
                {
                    _CurrentValue = value;
                    IsUnsaved = true;
                    if (!SupressPropertyChangedEvents)
                    {
                        OnPropertyChanged();
                        if (this is TimerData)
                        {
                            OnPropertyChanged("CurrentTimeSpan");
                        }
                        else
                        {
                            OnPropertyChanged("CurrentDateTime");
                        }
                    }
                    OnCurrentValueChanged(EventArgs.Empty);
                }
            }
        }

        protected virtual void OnCurrentValueChanged(EventArgs e)
        {
            CurrentValueChanged?.Invoke(this, e);
        }

        internal Color _UserBackColor = Color.Empty;
        public Color UserBackColor
        {
            get
            {
                return _UserBackColor;
            }
            set
            {
                if (!Utils.ColorsAreTheSame(_UserBackColor, value))
                {
                    _UserBackColor = value;
                    IsUnsaved = true;
                    if (!SupressPropertyChangedEvents)
                    {
                        RaisePropertyChanged(nameof(BlackWhiteSuggestedForeground));
                        OnPropertyChanged();
                    }
                }
            }
        }

        public System.Drawing.Color BlackWhiteSuggestedForeground
        {
            get
            {
                if (UserBackColor.GetBrightness() < 0.5)
                {
                    return System.Drawing.Color.White;
                }
                return System.Drawing.Color.Black;
            }
        }

        internal bool _IsUnsaved = false;
        public bool IsUnsaved
        {
            get
            {
                return _IsUnsaved;
            }
            set
            {
                if (!IsUnsavedLocked && value != _IsUnsaved)
                {
                    _IsUnsaved = value;
                    if (!SupressPropertyChangedEvents)
                    {
                        OnPropertyChanged();
                    }
                    OnIsUnsavedChanged(EventArgs.Empty);
                }
            }
        }

        internal bool _ResetToValueLocked = false;
        public bool ResetToValueLocked
        {
            get
            {
                return _ResetToValueLocked;
            }
            set
            {
                if (_ResetToValueLocked != value)
                {
                    _ResetToValueLocked = value;
                    IsUnsaved = true;
                    if (!SupressPropertyChangedEvents)
                    {
                        OnPropertyChanged();
                    }
                    OnResetToValueLockedChanged();
                }
            }
        }

        protected virtual void OnResetToValueLockedChanged()
        {
        }

        internal object _ResetToValue = null;
        public object ResetToValue
        {
            get
            {
                return _ResetToValue;
            }
            set
            {
                if (_ResetToValue != value)
                {
                    _ResetToValue = value;
                    IsUnsaved = true;
                    if (!SupressPropertyChangedEvents)
                    {
                        OnPropertyChanged();
                    }
                    OnResetToValueChanged();
                }
            }
        }

        internal bool _Checked = false;
        /// <summary>
        /// This property is not saved to file, it is part of the session data.
        /// </summary>
        public bool Checked
        {
            get
            {
                return _Checked;
            }
            set
            {
                if (_Checked != value)
                {
                    _Checked = value;
                    if (!SupressPropertyChangedEvents)
                    {
                        OnPropertyChanged();
                    }
                }
            }
        }

        internal bool _Checkable = false;
        /// <summary>
        /// This property is not saved to file, it is part of the session data.
        /// </summary>
        public bool Checkable
        {
            get
            {
                return _Checkable;
            }
            set
            {
                if (_Checkable != value)
                {
                    _Checkable = value;
                    if (!SupressPropertyChangedEvents)
                    {
                        OnPropertyChanged();
                    }
                }
            }
        }

        protected virtual void OnResetToValueChanged()
        {

        }

        protected virtual void OnIsUnsavedChanged(EventArgs e)
        {
            IsUnsavedChanged?.Invoke(this, e);
        }

        [Obsolete]
        internal List<IClockView> MyTimerViews;

        internal DataFile MyDataFile = null;

        internal ClockM(DataFile df, MultiAudioPlayer map)
        {
            _Tag = new FlowDocument();

            MyTimerViews = new List<IClockView>();

            MultiAudioPlayer = map;
            MyDataFile = df;

            // ringing timer
            t2 = new System.Windows.Threading.DispatcherTimer(System.Windows.Threading.DispatcherPriority.Send);
            t2.Tick += T2_Tick;

            MyToggleCommand = new RelayCommand(OnToggle);
            MyDeleteCommand = new RelayCommand(OnDelete);
        }

        private void T2_Tick(object sender, EventArgs e)
        {
            MultiAudioPlayer.RemoveClockMAndStopSoundIfNeeded(this);
            t2.Stop();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                MyDataFile.MainWindow.MyTimeOutFormsManager.CloseForm(this);

                t2.Stop();

                MultiAudioPlayer.RemoveClockMAndStopSoundIfNeeded(this);
            }

            disposed = true;
        }

        public bool IsBeingDeleted { get; protected set; } = false;

        internal virtual void Delete()
        {
            IsBeingDeleted = true;

            MyDataFile.ClockVMCollection.Model.RemoveClock(this);

            Deleted?.Invoke(this, EventArgs.Empty);

            Dispose();
        }
        
        internal int GetIndex()
        {
            return MyDataFile.ClockVMCollection.Model.IndexOf(this);
        }

        public bool Equals(ClockM other)
        {
            // TODO: there still are some fields not compared below:

            bool e = GetType() == other.GetType();
            if (!e) return false;

            // here the type is the same
            if (CurrentValue != null &&
                other.CurrentValue != null)
            {
                if (GetType() == typeof(TimerData))
                {
                    e &= (TimeSpan)CurrentValue == (TimeSpan)other.CurrentValue;
                }
                else
                {
                    e &= (DateTime)CurrentValue == (DateTime)other.CurrentValue;
                }
            }
            else if (CurrentValue != null ||
                     other.CurrentValue != null)
            {
                return false;
            }
            if (!e) return false;

            if (ResetToValue != null &&
                other.ResetToValue != null)
            {
                if (GetType() == typeof(TimerData))
                {
                    e &= (TimeSpan)ResetToValue == (TimeSpan)other.ResetToValue;
                }
                else
                {
                    e &= (DateTime)ResetToValue == (DateTime)other.ResetToValue;
                }
            }
            else if (ResetToValue != null ||
                     other.ResetToValue != null)
            {
                return false;
            }
            if (!e) return false;

            return e &&
                Enabled == other.Enabled &&
                FilteredOut == other.FilteredOut &&
                GroupName == other.GroupName &&
                Icon == other.Icon &&
                IsUnsaved == other.IsUnsaved &&
                IsUnsavedLocked == other.IsUnsavedLocked &&
                ResetToValueLocked == other.ResetToValueLocked &&
                Style == other.Style &&
                XamlWriter.Save(Tag) == XamlWriter.Save(other.Tag) &&
                Utils.ColorsAreTheSame(UserBackColor, other.UserBackColor);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var cd = obj as ClockM;
            if (cd == null)
                return false;
            else
                return Equals(cd);
        }

        // TODO: bug here, when uncommented, GetHashCode returns wrong values.
        // TEST THIS: create a new AlarmData in a ClockControl, start it, and it does not stop ringing. Try the 3 buttons on the TimeOutForm.
        //public override int GetHashCode()
        //{
        //    // TODO: there still are some fields not inserted in the hash below:

        //    unchecked
        //    {
        //        int hash = 19 + GetType().GetHashCode();
                
        //        if (GetType() == typeof(TimerData))
        //        {
        //            if (CurrentValue != null)
        //            {
        //                hash = hash * 31 + ((TimeSpan)CurrentValue).GetHashCode();
        //            }
        //            else
        //            {
        //                hash = hash * 31 + 1;
        //            }

        //            if (ResetToValue != null)
        //            {
        //                hash = hash * 31 + ((TimeSpan)ResetToValue).GetHashCode();
        //            }
        //            else
        //            {
        //                hash = hash * 31 + 1;
        //            }
        //        }
        //        else
        //        {
        //            if (CurrentValue != null)
        //            {
        //                hash = hash * 31 + ((DateTime)CurrentValue).GetHashCode();
        //            }
        //            else
        //            {
        //                hash = hash * 31 + 1;
        //            }

        //            if (ResetToValue != null)
        //            {
        //                hash = hash * 31 + ((DateTime)ResetToValue).GetHashCode();
        //            }
        //            else
        //            {
        //                hash = hash * 31 + 1;
        //            }
        //        }

        //        if (Icon != null)
        //        {
        //            hash = hash * 31 + Icon.GetHashCode();
        //        }
        //        else
        //        {
        //            hash = hash * 31 + 1;
        //        }

        //        return ((((((((hash * 31 +
        //            Enabled.GetHashCode()) * 31 +
        //            FilteredOut.GetHashCode()) * 31 +
        //            GroupName.GetHashCode()) * 31 +
        //            IsDirty.GetHashCode()) * 31 +
        //            IsDirtyLocked.GetHashCode()) * 31 +
        //            ResetToValueLocked.GetHashCode()) * 31 +
        //            Style.GetHashCode()) * 31 +
        //            Tag.GetHashCode()) * 31 +
        //            UserBackColor.GetHashCode();
        //    }
        //}

        public static bool operator ==(ClockM cd1, ClockM cd2)
        {
            if (((object)cd1) == null || ((object)cd2) == null)
                return object.Equals(cd1, cd2);

            return cd1.Equals(cd2);
        }

        public static bool operator !=(ClockM cdc1, ClockM cdc2)
        {
            return !(cdc1 == cdc2);
        }

        internal void ActivateOrDeactivate()
        {
            if (this is TimerData td)
            {
                td.StartOrStop();
            }
            else if (this is AlarmData ad)
            {
                ad.EnableOrDisable();
            }
        }

        public override string ToString()
        {
            return $"ClockM {Tag.ToPlainText()} | {CurrentValue} | {DateTime.Now}";
        }

        public bool IsActive
        {
            get
            {
                return (this is TimerData td && td.Running) ||
                       (this is AlarmData && Enabled);
            }
        }

        public RelayCommand MyToggleCommand { get; set; } = null;
        public void OnToggle()
        {
            ActivateOrDeactivate();
        }

        public RelayCommand MyDeleteCommand { get; set; } = null;
        public void OnDelete()
        {
            Delete();
        }
    }
}
