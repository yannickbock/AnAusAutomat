using AnAusAutomat.Contracts;
using System;

namespace AnAusAutomat.Sensors.GUI.HotKeys
{
    public class HotKeyPressedEventArgs : EventArgs
    {
        public HotKeyPressedEventArgs(Socket socket, PowerStatus status)
        {
            Socket = socket;
            Status = status;
        }

        public Socket Socket { get; private set; }

        public PowerStatus Status { get; private set; }
    }
}
