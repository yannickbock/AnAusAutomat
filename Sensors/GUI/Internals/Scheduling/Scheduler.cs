using GUI.Internals.Scheduling.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace GUI.Internals.Scheduling
{
    internal class Scheduler
    {
        private Timer _timer;
        private List<ScheduledTask> _scheduledTasks;

        public event EventHandler<ScheduledTaskReadyEventArgs> ScheduledTaskReady;

        internal Scheduler()
        {
            _timer = new Timer(1000);
            _timer.Elapsed += _timer_Elapsed;
            _scheduledTasks = new List<ScheduledTask>();
        }

        internal void Add(ScheduledTask task)
        {
            _scheduledTasks.Add(task);
        }

        internal void Remove(ScheduledTask task)
        {
            if (_scheduledTasks.Contains(task))
            {
                _scheduledTasks.Remove(task);
            }
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var tasksToExecute = _scheduledTasks.Where(x => x.ExecuteAt > DateTime.Now);

            foreach (var task in tasksToExecute)
            {
                ScheduledTaskReady?.Invoke(this, new ScheduledTaskReadyEventArgs(task));
                _scheduledTasks.Remove(task);
            }
        }

        internal void Start()
        {
            _timer.Start();
        }

        internal void Stop()
        {
            _timer.Stop();
        }
    }
}
