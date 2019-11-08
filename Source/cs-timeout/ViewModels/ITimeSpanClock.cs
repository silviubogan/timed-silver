using System;

namespace cs_timed_silver
{
    public interface ITimeSpanClock
    {
        int GetSeconds();

        TimeSpan CurrentTimeSpan { get; set; }
    }
}