using System;

namespace cs_timed_silver
{
    public class ClockEventArgs : EventArgs
    {
        internal ClockM Clock { get; set; } = null;
        internal int Index { get; set; } = -1;

        public ClockEventArgs(ClockM c = null, int i = -1)
        {
            Clock = c;
            Index = i;
        }
    }
}
