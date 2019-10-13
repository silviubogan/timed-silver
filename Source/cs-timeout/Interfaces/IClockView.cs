using System;

namespace cs_timed_silver
{
    public interface IClockView : IDisposable
    {
        ClockM MyClockData { get; set; }

        IClocksView GetClocksView();

        void FocusView();

        void SetIDText(string s);

        bool ShowClockCheckBox { get; set; }
    }
}