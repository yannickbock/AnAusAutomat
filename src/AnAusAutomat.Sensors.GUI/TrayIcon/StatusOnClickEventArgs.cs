using AnAusAutomat.Contracts;
using System;

namespace AnAusAutomat.Sensors.GUI.TrayIcon
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
