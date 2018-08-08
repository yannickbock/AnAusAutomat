using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Sensor;
using System;

namespace AnAusAutomat.Sensors.Keylogger.Internals
{
    public class Cache
    {
        public Cache(Socket socket, Parameters parameters)
        {
            Socket = socket;
            Parameters = parameters;
            Status = PowerStatus.Undefined;
            CurrentSignalSeconds = 0;
            LastSignal = DateTime.MinValue;
        }

        public Socket Socket { get; set; }

        public Parameters Parameters { get; set; }

        public PowerStatus Status { get; set; }

        public double CurrentSignalSeconds { get; set; }

        public DateTime LastSignal { get; set; }
    }
}
