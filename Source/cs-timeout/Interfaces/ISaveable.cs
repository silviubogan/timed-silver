using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_timed_silver
{
    public interface ISaveable
    {
        bool Save();

        bool SaveIfOnDisk();
    }
}
