using AnAusAutomat.Contracts;
using System;

namespace AnAusAutomat.Sensors.GUI.Dialogs
{
    public class MoreOptionsDialogResult
    {
        public MoreOptionsDialogResult(Socket socket, PowerStatus status, TimeSpan timeSpan, bool canceled)
        {
            Socket = socket;
            Status = status;
            TimeSpan = timeSpan;
            Canceled = canceled;
        }

        public Socket Socket { get; private set; }

        public PowerStatus Status { get; private set; }

        public TimeSpan TimeSpan { get; private set; }

        public bool Canceled { get; private set; }
    }
}
