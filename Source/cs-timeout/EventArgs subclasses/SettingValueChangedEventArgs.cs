using System;

namespace cs_timed_silver
{
    public class SettingValueChangedEventArgs : EventArgs
    {
        public bool IsInitialization { get; set; }

        public SettingValueChangedEventArgs(bool init = false)
        {
            IsInitialization = init;
        }
    }
}
