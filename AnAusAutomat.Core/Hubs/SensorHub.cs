using AnAusAutomat.Contracts.Sensor;
using AnAusAutomat.Contracts.Sensor.Events;
using AnAusAutomat.Contracts.Sensor.Extensions;
using AnAusAutomat.Contracts.Sensor.Features;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnAusAutomat.Core.Hubs
{
    // chain of responsibility pattern?
    public class SensorHub
    {
        // outsource all the event handling to an eventbroker?
        private IEnumerable<ISensor> _sensors;
        private IEnumerable<string> _modes;
        private string _currentMode;

        public SensorHub(IEnumerable<ISensor> sensors, IEnumerable<string> modes, string defaultMode)
        {
            _sensors = sensors;
            _modes = modes;
            _currentMode = defaultMode;
        }

        public event EventHandler<StatusChangedEventArgs> StatusChanged;
        public event EventHandler<ApplicationExitEventArgs> ApplicationExit;

        public void Initialize(IEnumerable<SensorSettings> settings)
        {
            // Do Not Use AsParallel! Thread Problems ...
            foreach (var sensor in _sensors)
            {
                string sensorName = sensor.GetType().Name;

                var s = settings.FirstOrDefault(x => x.SensorName == sensorName);

                if (s != null)
                {
                    sensor.StatusChanged += sensor_StatusChanged;

                    initializeFeatures(sensor);
                    sensor.Initialize(s);
                }
            }
        }

        private void initializeFeatures(ISensor sensor)
        {
            var capabilities = sensor.GetCapabilities();

            if (capabilities.HasFlag(SensorCapabilities.ReceiveModeChanged) || capabilities.HasFlag(SensorCapabilities.SendModeChanged))
            {
                var s = sensor as ISendModeChanged; // or as IReceiveModeChanged
                s.InitializeModes(_modes, _currentMode);

                if (capabilities.HasFlag(SensorCapabilities.SendModeChanged))
                {
                    s.ModeChanged += sensor_ModeChanged;
                }
            }

            if (capabilities.HasFlag(SensorCapabilities.SendStatusChangesIn))
            {
                var s = sensor as ISendStatusChangesIn;
                s.StatusChangesIn += sensor_StatusChangesIn;
            }
            if (capabilities.HasFlag(SensorCapabilities.SendExit))
            {
                var s = sensor as ISendExit;
                s.ApplicationExit += sensor_ApplicationExit; ;
            }
        }

        private void sensor_ApplicationExit(object sender, ApplicationExitEventArgs e)
        {
            ApplicationExit?.Invoke(sender, e);

            var sensorsWithReceiveApplicationExitSupport = _sensors.Where(x => x.GetCapabilities().HasFlag(SensorCapabilities.ReceiveExit)).ToList();
            foreach (var sensor in sensorsWithReceiveApplicationExitSupport)
            {
                if (sender.GetType().Name != sensor.GetType().Name)
                {
                    var s = sensor as IReceiveExit;
                    s.OnApplicationExit(sender, e);
                }
            }
        }

        private void sensor_StatusChangesIn(object sender, StatusChangesInEventArgs e)
        {
            var sensorsWithReceiveStatusChangesInSupport = _sensors.Where(x => x.GetCapabilities().HasFlag(SensorCapabilities.ReceiveStatusChangesIn)).ToList();
            foreach (var sensor in sensorsWithReceiveStatusChangesInSupport)
            {
                if (sender.GetType().Name != sensor.GetType().Name)
                {
                    var s = sensor as IReceiveStatusChangesIn;
                    s.OnStatusChangesIn(sender, e);
                }
            }
        }

        private void sensor_ModeChanged(object sender, ModeChangedEventArgs e)
        {
            _currentMode = e.Mode;

            var sensorsWithReceiveModeChangedSupport = _sensors.Where(x => x.GetCapabilities().HasFlag(SensorCapabilities.ReceiveModeChanged)).ToList();
            foreach (var sensor in sensorsWithReceiveModeChangedSupport)
            {
                if (sender.GetType().Name != sensor.GetType().Name)
                {
                    var s = sensor as IReceiveModeChanged;
                    s.OnModeHasChanged(sender, e);
                }
            }
        }

        private void sensor_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            StatusChanged?.Invoke(sender, e);

            var sensorsWithReceiveStatusChangedSupport = _sensors.Where(x => x.GetCapabilities().HasFlag(SensorCapabilities.ReceiveStatusChanged)).ToList();
            foreach (var sensor in sensorsWithReceiveStatusChangedSupport)
            {
                if (sender.GetType().Name != sensor.GetType().Name)
                {
                    var s = sensor as IReceiveStatusChanged;
                    s.OnSensorStatusHasChanged(sender, e);
                }
            }
        }

        public void OnPhysicalStatusHasChanged(object sender, StatusChangedEventArgs e)
        {
            var sensorsWithReceiveStatusChangedSupport = _sensors.Where(x => x.GetCapabilities().HasFlag(SensorCapabilities.ReceiveStatusChanged)).ToList();

            foreach (var sensor in sensorsWithReceiveStatusChangedSupport)
            {
                var s = sensor as IReceiveStatusChanged;
                s.OnPhysicalStatusHasChanged(sender, e);
            }
        }

        public void Start()
        {
            // Do Not Use AsParallel! Thread Problems ...
            foreach (var sensor in _sensors)
            {
                string sensorName = sensor.GetType().Name;
                Log.Information(string.Format("Starting sensor {0} ...", sensorName));
                sensor.Start();
            }
        }

        public void Stop()
        {
            // Do Not Use AsParallel! Thread Problems ...
            foreach (var sensor in _sensors)
            {
                string sensorName = sensor.GetType().Name;

                Log.Information(string.Format("Stopping sensor {0} ...", sensorName));
                sensor.Stop();
            }
        }
    }
}
