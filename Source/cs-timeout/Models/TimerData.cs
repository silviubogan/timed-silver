using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Windows.Threading;

namespace cs_timed_silver
{
    internal class TimerData : ClockM
    {
        public event EventHandler<ClockEventArgs>
            TimerStopped, TimerStartedByUser, TimerStoppedByUser;
        public DispatcherTimer FormsTimer { get; set; } = null;

        internal TimerData(DataFile df, MultiAudioPlayer map) : base(df, map)
        {
            // second by second timer
            FormsTimer = new DispatcherTimer(DispatcherPriority.Send)
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            FormsTimer.Tick += T_Tick;

            IsUnsavedLocked = true;

            CurrentTimeSpan = TimeSpan.Zero;
            ResetToValue = TimeSpan.Zero;

            IsUnsavedLocked = false;
        }

        internal void SubstractSeconds(int s)
        {
            CurrentTimeSpan -= TimeSpan.FromSeconds(s);
        }

        internal int GetSeconds()
        {
            TimeSpan val = CurrentTimeSpan;
            return (int)(val.TotalSeconds);
        }

        internal BeepTimerCollection MyBeepTimers;

        internal void UpdateBeepTimers()
        {
            DestroyBeepTimers();

            MyBeepTimers = MyDataFile.Beeps.CreateBeepTimerCollection(this);
        }
        
        internal void T_Tick(object sender, EventArgs e)
        {
            HandleTick();
        }

        /// <summary>
        /// Handles one tick of the timer that repeats itself until
        /// the clock rings. It substracts a second from the current
        /// value and stops the timer if it's the case, and starts
        /// the ringing timer, and invokes the TimerStopped event.
        /// </summary>
        internal void HandleTick()
        {
            SubstractSeconds(1);

            if (GetSeconds() == 0)
            {
                Running = false;
                t2.Start();
                TimerStopped?.Invoke(this, new ClockEventArgs()
                {
                    Clock = this
                });
            }
        }

        internal bool StartOrStop()
        {
            if (GetSeconds() == 0)
            {
                return false;
            }

            if (Running)
            {
                DestroyBeepTimers();
            }
            else
            {
                UpdateBeepTimers();
            }

            if (Running)
            {
                Running = false;
                TimerStoppedByUser?.Invoke(this, new ClockEventArgs()
                {
                    Clock = this
                });
                return false;
            }
            else
            {
                if (!ResetToValueLocked)
                {
                    ResetToValue = CurrentValue;
                }
                Running = true;
                TimerStartedByUser?.Invoke(this, new ClockEventArgs()
                {
                    Clock = this
                });
                return true;
            }
        }

        internal void DestroyBeepTimers()
        {
            if (MyBeepTimers != null)
            {
                MyBeepTimers.Delete();
                MyBeepTimers = null;
            }
        }

        bool disposed = false;
        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                return;
            }
            
            if (disposing)
            {
                // dispose managed
                DestroyBeepTimers();

                Running = false;
            }

            // unmanaged
            disposed = true;
            
            base.Dispose(disposing);
        }

        internal void Stop()
        {
            if (Running)
            {
                StartOrStop();
            }
        }
        
        public TimeSpan CurrentTimeSpan
        {
            get
            {
                if (CurrentValue == null)
                {
                    CurrentValue = TimeSpan.Zero;
                }
                return (TimeSpan)CurrentValue;
            }
            set
            {
                if (CurrentValue == null ||
                    (CurrentValue != null && value != (TimeSpan)CurrentValue))
                {
                    CurrentValue = value;
                }
            }
        }

        internal bool _Running = false;
        public bool Running
        {
            get
            {
                return FormsTimer.IsEnabled;
            }
            set
            {
                if (_Running != value)
                {
                    FormsTimer.IsEnabled = value;
                    _Running = value;
                    OnPropertyChanged();
                    OnPropertyChanged("IsActive");
                }
            }
        }
    }
}
