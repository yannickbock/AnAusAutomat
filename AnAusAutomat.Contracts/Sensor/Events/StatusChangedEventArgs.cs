using AnAusAutomat.Contracts.Sensor.Metadata;
using System;

namespace AnAusAutomat.Contracts.Sensor.Events
{
    public class StatusChangedEventArgs : EventArgs
    {
        public StatusChangedEventArgs(string message, string condition, ISensorMetadata triggeredBy, Socket socket, PowerStatus status)
        {
            Message = message;
            Condition = condition;
            TriggeredBy = triggeredBy;
            Socket = socket;
            Status = status;
            TimeStamp = DateTime.Now;
        }

        public string Message { get; private set; }

        public string Condition { get; private set; }

        public ISensorMetadata TriggeredBy { get; private set; }

        public DateTime TimeStamp { get; private set; }

        public Socket Socket { get; private set; }

        public PowerStatus Status { get; private set; }
    }
}
