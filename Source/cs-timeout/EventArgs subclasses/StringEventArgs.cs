using System;

namespace cs_timed_silver
{
    public class StringEventArgs : EventArgs
    {
        public string StringValue { get; set; }

        public StringEventArgs(string s = null)
        {
            StringValue = s;
        }
    }
}
