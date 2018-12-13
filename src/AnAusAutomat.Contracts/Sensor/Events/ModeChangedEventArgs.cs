using System;

namespace AnAusAutomat.Contracts.Sensor.Events
{
    public class ModeChangedEventArgs : EventArgs
    {
        public ModeChangedEventArgs(string message, ConditionMode mode)
        {
            Message = message;
            Mode = mode;
            TimeStamp = DateTime.Now;
        }

        public string Message { get; private set; }

        public ConditionMode Mode { get; private set; }

        public DateTime TimeStamp { get; private set; }
    }
}
