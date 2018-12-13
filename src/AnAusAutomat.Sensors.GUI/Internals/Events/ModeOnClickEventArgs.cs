using AnAusAutomat.Contracts;
using System;

namespace AnAusAutomat.Sensors.GUI.Internals.Events
{
    public class ModeOnClickEventArgs : EventArgs
    {
        public ModeOnClickEventArgs(ConditionMode mode)
        {
            Mode = mode;
        }

        public ConditionMode Mode { get; private set; }
    }
}
