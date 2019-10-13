using System;

namespace cs_timed_silver
{
    internal class Pointer
    {
        internal bool HasValue
        {
            get
            {
                return Value != null;
            }
        }

        internal event EventHandler ValueChanged;

        internal object _Value = null;
        internal object Value
        {
            get
            {
                return _Value;
            }
            set
            {
                if (_Value != value)
                {
                    _Value = value;
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}
