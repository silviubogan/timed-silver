using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_timed_silver
{
    public class Beep : BindableBase
    {
        protected int _MsBeforeRinging = -1;
        public int MsBeforeRinging {
            get { return _MsBeforeRinging; }
            set { SetProperty(ref _MsBeforeRinging, value); }
        }

        protected int _BeepDuration = -1;
        public int BeepDuration
        {
            get { return _BeepDuration; }
            set { SetProperty(ref _BeepDuration, value); }
        }

        protected int _BeepFrequency = -1;
        public int BeepFrequency
        {
            get { return _BeepFrequency; }
            set { SetProperty(ref _BeepFrequency, value); }
        }

        public Beep(int a = -1, int b = -1, int c = -1)
        {
            MsBeforeRinging = a;
            BeepDuration = b;
            BeepFrequency = c;
        }
    }
}
