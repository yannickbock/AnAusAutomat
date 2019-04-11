using System;

namespace AnAusAutomat.Sensors.GUI.Scheduling
{
    public interface IScheduler
    {
        event EventHandler<ScheduledTaskReadyEventArgs> ScheduledTaskReady;

        void Add(ScheduledTask task);
        void Remove(ScheduledTask task);
        void Start();
        void Stop();
    }
}