using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_timed_silver
{
    public class NamedCommandVM : BindableBase
    {
        internal string _Name = "Untitled Action";
        public string Name
        {
            get { return _Name; }
            set { SetProperty(ref _Name, value); }
        }

        internal RelayCommand _Command = null;
        public RelayCommand Command
        {
            get { return _Command; }
            set { SetProperty(ref _Command, value); }
        }

        public NamedCommandVM(string n, RelayCommand c)
        {
            Name = n;
            Command = c;
        }
    }
}
