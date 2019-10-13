using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Threading;

namespace cs_timed_silver
{
    // TODO: [VISUAL] customizable TimerControl / table row context menu and big buttons too.
    public class MultiAudioPlayer : NAudioPlayer
    {
        internal DispatcherTimer MyTimer;
        internal Dictionary<ClockM, int> Times;

        public MultiAudioPlayer(DataFile df) : base(df)
        {
            Times = new Dictionary<ClockM, int>();

            MyTimer = new DispatcherTimer(DispatcherPriority.Send)
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            MyTimer.Tick += MyTimer_Tick;
        }

        private void MyTimer_Tick(object sender, EventArgs e)
        {
            foreach (KeyValuePair<ClockM, int> p in Times)
            {
                Times[p.Key]--;
                if (Times[p.Key] == 0)
                {
                    Times.Remove(p.Key);
                }
            }
            StopSoundIfNeeded();
        }

        internal void RemoveClockMAndStopSoundIfNeeded(ClockM tc)
        {
            Times.Remove(tc);
            StopSoundIfNeeded();
        }

        internal void StopSoundIfNeeded()
        {
            if (Times.Keys.Count == 0)
            {
                StopSound();
            }
        }

        internal void AddClockData(ClockM clockData)
        {
            Times[clockData] = (int)(
                ((TimeSpan)MyDataFile.Settings.GetValue("RingingDuration")).TotalSeconds
            );
        }

        internal bool HasClockData(ClockM cd)
        {
            return Times.ContainsKey(cd);
        }
    }
}
