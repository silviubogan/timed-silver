using System;
using System.Windows.Threading;

namespace cs_timed_silver
{
    internal class BeepTimerCollection
    {
        internal DispatcherTimer[] BeepTimers = null;
        internal int[] BeepDurations;
        internal int[] BeepMsBeforeRinging;
        internal int[] BeepFrequecies;

        internal void TickHandler(object sender, EventArgs e)
        {
            var t = sender as DispatcherTimer;
            int index = IndexOf(t);

            if (index == -1)
            {
                throw new IndexOutOfRangeException();
            }

            t.Stop();


            // tangent beeps are sometimes not played when using Task.Run
            // (also not played when using the class Thread; see https://stackoverflow.com/a/2751716/258462):
            //Task.Run(new Action(() =>
            //{
            Console.Beep(
                BeepFrequecies[index],
                BeepDurations[index]);
            //}));

            CheckIfDone();
        }

        public event EventHandler DoneRinging;

        internal void CheckIfDone()
        {
            if (BeepTimers == null)
            {
                DoneRinging?.Invoke(this, EventArgs.Empty);
                return;
            }

            bool isTimerRunning = false;
            foreach (DispatcherTimer t in BeepTimers)
            {
                if (t.IsEnabled)
                {
                    isTimerRunning = true;
                    break;
                }
            }
            if (!isTimerRunning)
            {
                DoneRinging?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// To be called only when BeepTimers != null.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        internal int IndexOf(DispatcherTimer t)
        {
            int index = -1;
            for (int i = 0; i < BeepTimers.Length; ++i)
            {
                if (BeepTimers[i] == t)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        internal void Delete()
        {
            if (BeepTimers != null)
            {
                foreach (DispatcherTimer tmr in BeepTimers)
                {
                    tmr.Stop();
                }

                BeepTimers = null;
                BeepDurations = null;
                BeepMsBeforeRinging = null;
                BeepFrequecies = null;
            }

            CheckIfDone();
        }
    }
}
