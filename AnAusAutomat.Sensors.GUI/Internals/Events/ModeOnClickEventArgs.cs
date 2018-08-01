using System;

namespace AnAusAutomat.Sensors.GUI.Internals.Events
{
    public class ModeOnClickEventArgs : EventArgs
    {
        public ModeOnClickEventArgs(string mode)
        {
            Mode = mode;
        }

        public string Mode { get; private set; }
    }
}
