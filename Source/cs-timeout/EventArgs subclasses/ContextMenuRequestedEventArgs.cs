using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_timed_silver
{
    internal class ContextMenuRequestedEventArgs : EventArgs
    {
        internal Point Position { get; set; }

        public ContextMenuRequestedEventArgs()
        {
            Position = Point.Empty;
        }

        public ContextMenuRequestedEventArgs(Point p)
        {
            Position = p;
        }
    }
}
