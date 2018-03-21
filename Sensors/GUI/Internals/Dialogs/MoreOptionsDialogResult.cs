using GUI.Internals.Scheduling;

namespace GUI.Internals.Dialogs
{
    internal class MoreOptionsDialogResult
    {
        internal MoreOptionsDialogResult(bool canceled, ScheduledTask scheduledTask)
        {
            Canceled = canceled;
            ScheduledTask = scheduledTask;
        }

        internal bool Canceled { get; private set; }

        internal ScheduledTask ScheduledTask { get; private set; }
    }
}
