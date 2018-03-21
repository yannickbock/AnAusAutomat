using AnAusAutomat.Contracts.Sensor.Metadata;
using System;

namespace AnAusAutomat.Contracts.Sensor.Events
{
    public class ModeChangedEventArgs : EventArgs
    {
        public ModeChangedEventArgs(string message, ISensorMetadata triggeredBy, string mode)
        {
            Message = message;
            TriggeredBy = triggeredBy;
            Mode = mode;
            TimeStamp = DateTime.Now;
        }

        public string Message { get; private set; }

        public ISensorMetadata TriggeredBy { get; private set; }

        public string Mode { get; private set; }

        public DateTime TimeStamp { get; private set; }
    }
}
