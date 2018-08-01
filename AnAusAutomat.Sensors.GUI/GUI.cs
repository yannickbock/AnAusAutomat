using AnAusAutomat.Contracts.Sensor;
using AnAusAutomat.Contracts.Sensor.Attributes;
using AnAusAutomat.Contracts.Sensor.Events;
using AnAusAutomat.Contracts.Sensor.Features;
using AnAusAutomat.Sensors.GUI.Internals;
using AnAusAutomat.Sensors.GUI.Internals.Dialogs;
using AnAusAutomat.Sensors.GUI.Internals.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace AnAusAutomat.Sensors.GUI
{
    [Parameter("StartMinimized", typeof(string), "false")]
    [Description("...")]
    public class GUI : ISensor, IReceiveStatusChanged, IReceiveStatusChangesIn, IReceiveModeChanged, ISendModeChanged, ISendExit
    {
        public event EventHandler<StatusChangedEventArgs> StatusChanged;
        public event EventHandler<ModeChangedEventArgs> ModeChanged;
        public event EventHandler<ApplicationExitEventArgs> ApplicationExit;

        private Settings _settings;
        private IEnumerable<string> _modes;
        private string _currentMode;
        private TrayIcon _trayIcon;
        private Translation _translation;

        public void InitializeModes(IEnumerable<string> modes, string currentMode)
        {
            _modes = modes;
            _currentMode = currentMode;
        }

        public void Initialize(SensorSettings settings)
        {
            _translation = new Translation();

            var parser = new SettingsParser();
            _settings = parser.Parse(settings.Parameters);

            var builder = new TrayIconBuilder(new Translation());
            foreach (var socket in settings.Sockets)
            {
                builder.AddSocketStrip(socket);
            }
            builder.AddSeparatorStrip();
            builder.AddModeStrip(_modes, _currentMode);
            builder.AddSeparatorStrip();
            builder.AddExitStrip();

            _trayIcon = builder.Build();
            _trayIcon.ModeOnClick += _trayIcon_ModeOnClick;
            _trayIcon.ExitOnClick += _trayIcon_ExitOnClick;
            _trayIcon.StatusOnClick += _trayIcon_StatusOnClick;
            _trayIcon.MoreOptionsOnClick += _trayIcon_MoreOptionsOnClick;
        }

        private void _trayIcon_MoreOptionsOnClick(object sender, MoreOptionsOnClickEventArgs e)
        {
            var dialog = new MoreOptionsDialog(_translation);
            var result = dialog.ShowDialog(e.Socket);
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
            _trayIcon.SetCurrentMode(e.Mode);
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

        public void OnStatusChangesIn(object sender, StatusChangesInEventArgs e)
        {
            //_trayIcon.OnStatusChangesIn(sender, e);
        }

        public void Start()
        {
            _trayIcon.Show();
        }

        public void Stop()
        {
            _trayIcon.Hide();
        }
    }
}
