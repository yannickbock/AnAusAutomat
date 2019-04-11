using AnAusAutomat.Contracts.Sensor;
using AnAusAutomat.Contracts.Sensor.Events;
using AnAusAutomat.Contracts.Sensor.Features;
using AnAusAutomat.Sensors.GUI.Dialogs;
using AnAusAutomat.Sensors.GUI.HotKeys;
using AnAusAutomat.Sensors.GUI.Scheduling;
using AnAusAutomat.Sensors.GUI.TrayIcon;
using System;
using System.Windows.Forms;

namespace AnAusAutomat.Sensors.GUI
{
    public class GUI : ISensor, IReceiveStatusChanged, IReceiveStatusForecast, IReceiveModeChanged, ISendModeChanged, ISendExit
    {
        public event EventHandler<StatusChangedEventArgs> StatusChanged;
        public event EventHandler<ModeChangedEventArgs> ModeChanged;
        public event EventHandler<ApplicationExitEventArgs> ApplicationExit;

        private ITranslation _translation;
        private IScheduler _scheduler;
        private ITrayIconMenu _trayIcon;
        private IHotKeyHandler _hotKeyHandler;

        public GUI(ITranslation translation, IScheduler scheduler, ITrayIconMenu trayIcon, IHotKeyHandler hotKeyHandler)
        {
            _translation = translation;
            _scheduler = scheduler;
            _trayIcon = trayIcon;
            _hotKeyHandler = hotKeyHandler;

            _scheduler.ScheduledTaskReady += _scheduler_ScheduledTaskReady;

            _trayIcon.ModeOnClick += _trayIcon_ModeOnClick;
            _trayIcon.ExitOnClick += _trayIcon_ExitOnClick;
            _trayIcon.StatusOnClick += _trayIcon_StatusOnClick;
            _trayIcon.MoreOptionsOnClick += _trayIcon_MoreOptionsOnClick;

            _hotKeyHandler.HotKeyPressed += _hotKeyHandler_HotKeyPressed;
        }

        private void _hotKeyHandler_HotKeyPressed(object sender, HotKeyPressedEventArgs e)
        {
            StatusChanged?.Invoke(this, new StatusChangedEventArgs("HotKey.", "", e.Socket, e.Status));
        }

        private void _scheduler_ScheduledTaskReady(object sender, ScheduledTaskReadyEventArgs e)
        {
            StatusChanged?.Invoke(this, new StatusChangedEventArgs("Scheduled task.", "", e.Task.Socket, e.Task.Status));
        }

        private void _trayIcon_MoreOptionsOnClick(object sender, MoreOptionsOnClickEventArgs e)
        {
            var dialog = new MoreOptionsDialog(_translation);
            var result = dialog.ShowDialog(e.Socket);

            if (!result.Canceled)
            {
                _scheduler.Add(new ScheduledTask(
                    DateTime.Now + result.TimeSpan,
                    result.Status,
                    result.Socket));
            }
        }

        private void _trayIcon_StatusOnClick(object sender, StatusOnClickEventArgs e)
        {
            StatusChanged?.Invoke(this, new StatusChangedEventArgs("TrayIcon.", "", e.Socket, e.Status));
        }

        private void _trayIcon_ExitOnClick(object sender, ExitOnClickEventArgs e)
        {
            var result = MessageBox.Show(_translation.GetMessageBoxExitText(), "AnAusAutomat", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                ApplicationExit?.Invoke(this, new ApplicationExitEventArgs(""));
            }
        }

        private void _trayIcon_ModeOnClick(object sender, ModeOnClickEventArgs e)
        {
            ModeChanged?.Invoke(this, new ModeChangedEventArgs("", e.Mode));
        }

        public void OnModeHasChanged(object sender, ModeChangedEventArgs e)
        {
            _trayIcon.SetModeState(e.Mode);
        }

        public void OnPhysicalStatusHasChanged(object sender, StatusChangedEventArgs e)
        {
            _trayIcon.SetPhysicalStatus(e.Socket, e.Status);
            _trayIcon.ShowPhysicalStatusBalloonTip(e.Socket, e.Status, e.TimeStamp, sender.GetType().Name, e.Condition);
        }

        public void OnSensorStatusHasChanged(object sender, StatusChangedEventArgs e)
        {
            //_trayIcon.OnSensorStatusHasChanged(sender, e);
        }

        public void OnStatusForecast(object sender, StatusForecastEventArgs e)
        {
            _trayIcon.ShowStatusForecastBalloonTip(e.Socket, e.Status, e.CountDown, sender.GetType().Name);
        }

        public void Start()
        {
            _trayIcon.Show();
            _scheduler.Start();
            _hotKeyHandler.Start();
        }

        public void Stop()
        {
            _trayIcon.Hide();
            _scheduler.Stop();
            _hotKeyHandler.Stop();
        }
    }
}
