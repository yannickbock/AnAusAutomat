using AnAusAutomat.Contracts.Sensor.Metadata;
using System;

namespace AnAusAutomat.Contracts.Sensor.Events
{
    public class ApplicationExitEventArgs : EventArgs
    {
        public ApplicationExitEventArgs(string message, ISensorMetadata triggeredBy)
        {
            Message = message;
            TriggeredBy = triggeredBy;
            TimeStamp = DateTime.Now;
        }

        public string Message { get; private set; }

        public ISensorMetadata TriggeredBy { get; private set; }

        public DateTime TimeStamp { get; private set; }
    }
}
