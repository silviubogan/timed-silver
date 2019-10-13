using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_timed_silver
{
    public class TimeOutWindowVM : BindableBase
    {
        public TimeOutWindowM Model;

        public TimeOutWindowVM(TimeOutWindowM model)
        {
            Model = model;
        }

        public void ShowWindow()
        {
            Model.MyClockM.ShowTimeOutForm();
        }
    }
}
