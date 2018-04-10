using System;

namespace AnAusAutomat.Contracts.Sensor.Events
{
    public class StatusChangesInEventArgs : EventArgs
    {
        public StatusChangesInEventArgs(string message, TimeSpan countDown, Socket socket, PowerStatus status)
        {
            Message = message;
            CountDown = countDown;
            Socket = socket;
            Status = status;
            TimeStamp = DateTime.Now;
        }

        public string Message { get; private set; }

        public DateTime TimeStamp { get; private set; }

        public TimeSpan CountDown { get; private set; }

        public Socket Socket { get; private set; }

        public PowerStatus Status { get; private set; }
    }
}
