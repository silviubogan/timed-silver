using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_timed_silver
{
    internal class FilePathEventArgs : EventArgs
    {
        internal string NewFilePath { get; set; } = "";

        public FilePathEventArgs(string f = "")
        {
            NewFilePath = f;
        }
    }
}
