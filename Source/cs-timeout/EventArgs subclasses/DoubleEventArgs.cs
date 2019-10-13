using System;

namespace cs_timed_silver
{
    public class DoubleEventArgs : EventArgs
    {
        public double Value { get; set; } = double.NaN;

        public DoubleEventArgs(double v = double.NaN)
        {
            Value = v;
        }
    }
}