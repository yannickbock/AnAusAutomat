using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Sensor;
using System;

namespace AnAusAutomat.Sensors.GUI.Internals.Scheduling
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
