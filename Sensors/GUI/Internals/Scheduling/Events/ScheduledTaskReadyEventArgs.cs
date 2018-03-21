using System;

namespace GUI.Internals.Scheduling.Events
{
    internal class ScheduledTaskReadyEventArgs : EventArgs
    {
        internal ScheduledTaskReadyEventArgs(ScheduledTask task)
        {
            Task = task;
        }

        internal ScheduledTask Task { get; private set; }
    }
}
