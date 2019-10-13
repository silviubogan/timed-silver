using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_timed_silver
{
    public class TimeOutWindowM
    {
        public ClockM MyClockM { get; set; }

        public TimeOutWindowM(ClockM model)
        {
            MyClockM = model;
        }
    }
}
