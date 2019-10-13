using System;

namespace cs_timed_silver
{
    internal interface IDateTimeProvider
    {
        DateTime CurrentDateTime { get; }
        DateTime TodayDateTime { get; }
    }
}