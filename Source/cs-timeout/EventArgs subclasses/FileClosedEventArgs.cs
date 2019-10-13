using System;

namespace cs_timed_silver
{
    public class FileClosedEventArgs : EventArgs
    {
        public bool OtherFileWillBeOpened { get; set; } = false;

        public FileClosedEventArgs(bool o = false)
        {
            OtherFileWillBeOpened = o;
        }
    }
}
