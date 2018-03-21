using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Sensor;
using System;

namespace GUI.Internals.Scheduling
{
    internal class ScheduledTask
    {
        internal ScheduledTask(DateTime executeAt, PowerStatus status, Socket socket)
        {
            ExecuteAt = executeAt;
            Status = status;
            Socket = socket;
        }

        internal DateTime ExecuteAt { get; private set; }

        internal PowerStatus Status { get; private set; }

        internal Socket Socket { get; private set; }
    }
}
