using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace AnAusAutomat.Sensors.GUI.Scheduling
{
    public class Scheduler : IScheduler
    {
        private Timer _timer;
        private List<ScheduledTask> _scheduledTasks;

        public event EventHandler<ScheduledTaskReadyEventArgs> ScheduledTaskReady;

        public Scheduler()
        {
            _timer = new Timer(1000);
            _timer.Elapsed += _timer_Elapsed;
            _scheduledTasks = new List<ScheduledTask>();
        }

        public void Add(ScheduledTask task)
        {
            _scheduledTasks.Add(task);
        }

        public void Remove(ScheduledTask task)
        {
            if (_scheduledTasks.Contains(task))
            {
                _scheduledTasks.Remove(task);
            }
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var tasksToExecute = _scheduledTasks.Where(x => x.ExecuteAt < DateTime.Now).ToList();

            foreach (var task in tasksToExecute)
            {
                ScheduledTaskReady?.Invoke(this, new ScheduledTaskReadyEventArgs(task));
                _scheduledTasks.Remove(task);
            }
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }
}
