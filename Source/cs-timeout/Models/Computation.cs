using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_timed_silver
{
    public abstract class Computation<T>
    {
        public abstract object Compute(T arg);
    }
}
