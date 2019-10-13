using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_timed_silver
{
    public class LogMessageM : BindableBase
    {
        internal string _Text = null;
        public string Text
        {
            get { return _Text; }
            set { SetProperty(ref _Text, value); }
        }

        internal LogCategory _Category = LogCategory.None;
        public LogCategory Category
        {
            get { return _Category; }
            set { SetProperty(ref _Category, value); }
        }

        internal DateTime _DateTime;
        public DateTime DateTime
        {
            get { return _DateTime; }
            set { SetProperty(ref _DateTime, value); }
        }
    }
}
