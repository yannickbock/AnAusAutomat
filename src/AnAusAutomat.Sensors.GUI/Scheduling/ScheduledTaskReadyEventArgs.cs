using System;

namespace AnAusAutomat.Sensors.GUI.Scheduling
{
    public class ScheduledTaskReadyEventArgs : EventArgs
    {
        public ScheduledTaskReadyEventArgs(ScheduledTask task)
        {
            Task = task;
        }

        public ScheduledTask Task { get; private set; }
    }
}
