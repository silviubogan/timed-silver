using System.ComponentModel;

namespace cs_timed_silver
{
    public class PropertyChangedClockEventArgs : PropertyChangedEventArgs
    {
        internal ClockM Clock { get; set; } = null;

        internal PropertyChangedClockEventArgs(string propertyName)
            : base(propertyName)
        {
            
        }

        internal PropertyChangedClockEventArgs(string propertyName, ClockM c)
            : base(propertyName)
        {
            Clock = c;
        }
    }
}
