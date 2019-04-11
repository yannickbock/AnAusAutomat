using System;

namespace AnAusAutomat.Sensors.GUI.HotKeys
{
    [Flags]
    public enum KeyModifiers
    {
        Alt = 1,
        Control = 2,
        Shift = 4,
        Windows = 8
    }
}
