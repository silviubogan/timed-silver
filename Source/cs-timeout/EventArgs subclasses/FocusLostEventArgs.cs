using System;
using System.Windows.Forms;

namespace cs_timed_silver
{
    internal class FocusLostEventArgs : EventArgs
    {
        internal Control Control { get; set; } = null;

        public FocusLostEventArgs(Control c = null)
        {
            Control = c;
        }
    }
}
