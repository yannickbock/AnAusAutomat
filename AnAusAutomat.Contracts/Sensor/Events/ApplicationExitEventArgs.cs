using System;

namespace AnAusAutomat.Contracts.Sensor.Events
{
    public class ApplicationExitEventArgs : EventArgs
    {
        public ApplicationExitEventArgs(string message)
        {
            Message = message;
            TimeStamp = DateTime.Now;
        }

        public string Message { get; private set; }

        public DateTime TimeStamp { get; private set; }
    }
}
