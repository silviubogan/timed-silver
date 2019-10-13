using System;

namespace cs_timed_silver
{
    public class ClockMovedEventArgs : EventArgs
    {
        internal ClockM Clock { get; set; } = null;
        internal int OldIndex { get; set; } = -1;
        internal int NewIndex { get; set; } = -1;

        public ClockMovedEventArgs()
        {

        }

        public ClockMovedEventArgs(ClockM c = null, int o = -1, int n = -1)
        {
            Clock = c;
            OldIndex = o;
            NewIndex = n;
        }
    }
}
