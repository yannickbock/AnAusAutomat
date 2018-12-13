using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Sensor;
using AnAusAutomat.Contracts.Sensor.Events;
using AnAusAutomat.Contracts.Sensor.Features;
using AnAusAutomat.Sensors.GUI.Internals;
using AnAusAutomat.Sensors.GUI.Internals.Dialogs;
using AnAusAutomat.Sensors.GUI.Internals.Events;
using AnAusAutomat.Sensors.GUI.Internals.Scheduling;
using AnAusAutomat.Sensors.GUI.Internals.Scheduling.Events;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AnAusAutomat.Sensors.GUI
{
    public class GUI : ISensor, IReceiveStatusChanged, IReceiveStatusForecast, IReceiveModeChanged, ISendModeChanged, ISendExit
    {
        public event EventHandler<StatusChangedEventArgs> StatusChanged;
        public event EventHandler<ModeChangedEventArgs> ModeChanged;
        public event EventHandler<ApplicationExitEventArgs> ApplicationExit;

        private TrayIcon _trayIcon;
        private Translation _translation;
        private Scheduler _scheduler;

        public GUI(IEnumerable<Socket> sockets, IEnumerable<ConditionMode> modes)
        {
            _translation = new Translation();

            var builder = new TrayIconBuilder(_translation);
            foreach (var socket in sockets)
            {
                builder.AddSocketStrip(socket);
            }
            builder.AddSeparatorStrip();
            builder.AddModeStrip(modes);
            builder.AddSeparatorStrip();
            builder.AddExitStrip();

            _trayIcon = builder.Build();
            _trayIcon.ModeOnClick += _trayIcon_ModeOnClick;
            _trayIcon.ExitOnClick += _trayIcon_ExitOnClick;
            _trayIcon.StatusOnClick += _trayIcon_StatusOnClick;
            _trayIcon.MoreOptionsOnClick += _trayIcon_MoreOptionsOnClick;

            _scheduler = new Scheduler();
            _scheduler.ScheduledTaskReady += _scheduler_ScheduledTaskReady;
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
            StatusChanged?.Invoke(this, new StatusChangedEventArgs("", "", e.Socket, e.Status));
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
        }

        public void Stop()
        {
            _trayIcon.Hide();
            _scheduler.Stop();
        }
    }
}
