using System;
using System.Windows.Forms;
using System.Windows.Threading;

namespace cs_timed_silver
{
    /// <summary>
    /// Minute-by-minute timer.
    /// </summary>
    class SystemClockTimer
    {
        internal DispatcherTimer DateTimeTimer = new DispatcherTimer(DispatcherPriority.Send)
        {
            Interval = TimeSpan.FromMinutes(1)
        };
        internal DispatcherTimer InitialTimer = new DispatcherTimer(DispatcherPriority.Send);

        internal bool _Enabled = false;
        public bool Enabled
        {
            get
            {
                return _Enabled;
            }
            set
            {
                if (_Enabled != value)
                {
                    if (value)
                    {
                        DateTime dt = DateTime.Now;
                        InitialTimer.Interval = TimeSpan.FromMilliseconds(Math.Max(60 * 1000 -
                            (dt.Millisecond + dt.Second * 1000), 1));
                        InitialTimer.Start();
                    }
                    else
                    {
                        InitialTimer.Stop();
                        DateTimeTimer.Stop();
                    }

                    _Enabled = value;
                }
            }
        }

        public event EventHandler Tick;

        internal SystemClockTimer()
        {
            DateTimeTimer.Tick += DateTimeTimer_Tick;
            InitialTimer.Tick += InitialTimer_Tick;
        }

        private void InitialTimer_Tick(object sender, EventArgs e)
        {
            InitialTimer.Stop();

            ForceUpdate();

            DateTimeTimer.Start();
        }

        private void DateTimeTimer_Tick(object sender, EventArgs e)
        {
            ForceUpdate();
        }

        internal void ForceUpdate()
        {
            Tick?.Invoke(this, EventArgs.Empty);
        }
    }
}
