using AnAusAutomat.Contracts;
using System;

namespace AnAusAutomat.Sensors.GUI.TrayIcon
{
    public class MoreOptionsOnClickEventArgs : EventArgs
    {
        public MoreOptionsOnClickEventArgs(Socket socket)
        {
            Socket = socket;
        }

        public Socket Socket { get; private set; }
    }
}
