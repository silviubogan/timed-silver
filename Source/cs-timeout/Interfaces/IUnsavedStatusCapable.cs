using System;

namespace cs_timed_silver
{
    interface IUnsavedStatusCapable
    {
        bool IsUnsaved { get; set; }

        bool IsUnsavedLocked { get; set; }

        event EventHandler IsUnsavedChanged;
    }
}
