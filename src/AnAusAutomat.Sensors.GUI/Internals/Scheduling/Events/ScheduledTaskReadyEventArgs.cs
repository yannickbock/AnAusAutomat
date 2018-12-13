using System;

namespace AnAusAutomat.Sensors.GUI.Internals.Scheduling.Events
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
