using AnAusAutomat.Contracts;
using System;

namespace AnAusAutomat.Sensors.GUI.Scheduling
{
    public class ScheduledTask
    {
        public ScheduledTask(DateTime executeAt, PowerStatus status, Socket socket)
        {
            ExecuteAt = executeAt;
            Status = status;
            Socket = socket;
        }

        public DateTime ExecuteAt { get; private set; }

        public PowerStatus Status { get; private set; }

        public Socket Socket { get; private set; }
    }
}
