using System;

namespace cs_timed_silver
{
    internal class PathValidationEventArgs : EventArgs
    {
        internal bool Valid { get; set; } = false;
        internal string Path { get; set; } = "";

        public PathValidationEventArgs(bool v = false, string p = "")
        {
            Valid = v;
            Path = p;
        }
    }
}
