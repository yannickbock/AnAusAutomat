using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Sensor;
using System;

namespace AnAusAutomat.Sensors.GUI.Internals.Events
{
    public class StatusOnClickEventArgs : EventArgs
    {
        public StatusOnClickEventArgs(Socket socket, PowerStatus status)
        {
            Socket = socket;
            Status = status;
        }

        public Socket Socket { get; private set; }

        public PowerStatus Status { get; private set; }
    }
}
