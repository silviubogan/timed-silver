using System;

namespace cs_timed_silver
{
    internal class AlarmData : ClockM
    {
        internal event EventHandler<ClockEventArgs> EnabledChanged;

        internal new bool _Enabled = false;
        public override bool Enabled
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
                    IsUnsaved = true;
                    if (!SupressPropertyChangedEvents)
                    {
                        RaisePropertyChanged();
                        RaisePropertyChanged("IsActive");
                    }
                    OnEnabledChanged();
                }
            }
        }

        internal DateTime CurrentDateTime
        {
            get
            {
                if (CurrentValue == null)
                {
                    CurrentValue = DateTime.Now;
                }
                return (DateTime)CurrentValue;
            }
            set
            {
                if (CurrentValue == null ||
                    (CurrentValue != null && value != (DateTime)CurrentValue))
                {
                    CurrentValue = value;
                }
            }
        }

        internal void EnableOrDisable()
        {
            Enabled = !Enabled;
        }

        protected virtual void OnEnabledChanged()
        {
            EnabledChanged?.Invoke(this, new ClockEventArgs()
            {
                Clock = this
            });
        }

        internal AlarmData(DataFile df, MultiAudioPlayer map) : base(df, map)
        {
            IsUnsavedLocked = true;

            CurrentDateTime = DateTime.Now;
            ResetToValue = DateTime.Now;

            IsUnsavedLocked = false;
        }
    }
}
