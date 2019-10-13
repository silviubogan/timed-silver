using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cs_timed_silver
{
    public interface IMultipleViewTypesForm
    {
        EasyViewType SelectedViewType { get; set; }

        void ForceUpdateViewType(EasyViewType value);
    }
}
