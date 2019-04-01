using System;

namespace AnAusAutomat.Sensors.GUI.Internals.HotKeys
{
    [Flags]
    public enum KeyModifiers
    {
        Alt = 1,
        Control = 2,
        Shift = 4,
        Windows = 8,
        NoRepeat = 0x4000
    }
}
