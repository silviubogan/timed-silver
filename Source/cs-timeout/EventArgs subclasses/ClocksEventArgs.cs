namespace cs_timed_silver
{
    public class ClocksEventArgs
    {
        public ClockM[] Clocks { get; set; }

        public ClocksEventArgs()
        {
            Clocks = System.Array.Empty<ClockM>();
        }

        public ClocksEventArgs(ClockM[] cs)
        {
            Clocks = cs;
        }
    }
}