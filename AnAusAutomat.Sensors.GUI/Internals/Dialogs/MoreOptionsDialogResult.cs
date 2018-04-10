using AnAusAutomat.Sensors.GUI.Internals.Scheduling;

namespace AnAusAutomat.Sensors.GUI.Internals.Dialogs
{
    public class MoreOptionsDialogResult
    {
        public MoreOptionsDialogResult(bool canceled, ScheduledTask scheduledTask)
        {
            Canceled = canceled;
            ScheduledTask = scheduledTask;
        }

        public bool Canceled { get; private set; }

        public ScheduledTask ScheduledTask { get; private set; }
    }
}
