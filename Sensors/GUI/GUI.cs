using AnAusAutomat.Contracts.Sensor;
using AnAusAutomat.Contracts.Sensor.Events;
using AnAusAutomat.Contracts.Sensor.Extensions;
using AnAusAutomat.Contracts.Sensor.Features;
using AnAusAutomat.Contracts.Sensor.Metadata;
using GUI.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI
{
    [Export(typeof(ISensor))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [SensorMetadata(Name = "GUI", Description = "", Parameters = "StartMinimized: bool {true}")]
    public class GUI : ISensor, IReceiveStatusChanged, IReceiveStatusChangesIn, IReceiveModeChanged, ISendModeChanged, ISendExit
    {
        public event EventHandler<StatusChangedEventArgs> StatusChanged;
        public event EventHandler<ModeChangedEventArgs> ModeChanged;
        public event EventHandler<ApplicationExitEventArgs> ApplicationExit;

        private Settings _settings;
        private IEnumerable<string> _modes;
        private string _currentMode;
        private TrayIcon _trayIcon;

        public void InitializeModes(IEnumerable<string> modes, string currentMode)
        {
            _modes = modes;
            _currentMode = currentMode;
        }

        public void Initialize(SensorSettings settings)
        {
            _settings = parseSettings(settings.Parameters);
            _trayIcon = new TrayIcon(this.GetMetadata(), settings, _modes, _currentMode);
            _trayIcon.StatusChanged += _trayIcon_StatusChanged;
            _trayIcon.ModeChanged += _trayIcon_ModeChanged;
            _trayIcon.ApplicationExit += _trayIcon_ApplicationExit;
        }

        private void _trayIcon_ApplicationExit(object sender, ApplicationExitEventArgs e)
        {
            ApplicationExit?.Invoke(sender, e);
        }

        private void _trayIcon_ModeChanged(object sender, ModeChangedEventArgs e)
        {
            ModeChanged?.Invoke(sender, e);
        }

        private void _trayIcon_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            StatusChanged?.Invoke(sender, e);
        }

        public void OnModeHasChanged(object sender, ModeChangedEventArgs e)
        {
            _trayIcon.OnModeHasChanged(sender, e);
        }

        public void OnPhysicalStatusHasChanged(object sender, StatusChangedEventArgs e)
        {
            _trayIcon.OnPhysicalStatusHasChanged(sender, e);
        }

        public void OnSensorStatusHasChanged(object sender, StatusChangedEventArgs e)
        {
            _trayIcon.OnSensorStatusHasChanged(sender, e);
        }

        public void OnStatusChangesIn(object sender, StatusChangesInEventArgs e)
        {
            _trayIcon.OnStatusChangesIn(sender, e);
        }

        public void Start()
        {
            if (!_settings.StartMinimized)
            {
                MessageBox.Show("StartMinimized = false");
            }

            _trayIcon.Show();
        }

        public void Stop()
        {
            _trayIcon.Hide();
        }

        private Settings parseSettings(IEnumerable<SensorParameter> parameters)
        {
            bool startMinimized = true;

            if (parameters.Count() > 0)
            {
                bool startMinimizedDefined = parameters.Count(x => x.Name == "StartMinimized") == 1;

                if (startMinimizedDefined)
                {
                    string startMinimizedAsString = parameters.FirstOrDefault(x => x.Name == "StartMinimized").Value.ToLower();
                    startMinimized = new string[] { "true", "yes", "1" }.Contains(startMinimizedAsString) ;
                }
            }

            return new Settings(startMinimized);
        }
    }
}
