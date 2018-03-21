using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Sensor;
using System;

namespace UserInputDetector.Internals
{
    internal class Cache
    {
        internal Cache(Socket socket, Parameters parameters)
        {
            Socket = socket;
            Parameters = parameters;
            Status = PowerStatus.Undefined;
            CurrentSignalSeconds = 0;
            LastSignal = DateTime.MinValue;
        }

        internal Socket Socket { get; set; }

        internal Parameters Parameters { get; set; }

        internal PowerStatus Status { get; set; }

        internal double CurrentSignalSeconds { get; set; }

        internal DateTime LastSignal { get; set; }
    }
}
