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
        private int _MsBeforeRinging = -1;
        public int MsBeforeRinging {
            get { return _MsBeforeRinging; }
            set { SetProperty(ref _MsBeforeRinging, value); }
        }

        private int _BeepDuration = -1;
        public int BeepDuration
        {
            get { return _BeepDuration; }
            set { SetProperty(ref _BeepDuration, value); }
        }

        private int _BeepFrequency = -1;
        public int BeepFrequency
        {
            get { return _BeepFrequency; }
            set { SetProperty(ref _BeepFrequency, value); }
        }

        /// <summary>
        /// Needed for usage in DataGrid so that the new-row row is visible and editable.
        /// </summary>
        public Beep()
        {
        }

        public Beep(int a = -1, int b = -1, int c = -1) : this()
        {
            MsBeforeRinging = a;
            BeepDuration = b;
            BeepFrequency = c;
        }
    }
}
