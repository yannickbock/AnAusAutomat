using AnAusAutomat.Contracts.Sensor;
using AnAusAutomat.Contracts.Sensor.Events;
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
            bool hasSendModeChangedSupport = sensor as ISendModeChanged != null;
            bool hasReceiveModeChangedSupport = sensor as IReceiveModeChanged != null;

            if (hasSendModeChangedSupport || hasReceiveModeChangedSupport)
            {
                // Call InitializeModes() only once.
                if (hasSendModeChangedSupport)
                {
                    ((ISendModeChanged)sensor).InitializeModes(_modes, _currentMode);
                    ((ISendModeChanged)sensor).ModeChanged += sensor_ModeChanged;
                }
                else if (hasReceiveModeChangedSupport)
                {
                    ((IReceiveModeChanged)sensor).InitializeModes(_modes, _currentMode);
                }
            }

            if (sensor as ISendStatusChangesIn != null)
            {
                ((ISendStatusChangesIn)sensor).StatusChangesIn += sensor_StatusChangesIn;
            }
            if (sensor as ISendExit != null)
            {
                ((ISendExit)sensor).ApplicationExit += sensor_ApplicationExit;
            }
        }

        private void sensor_ApplicationExit(object sender, ApplicationExitEventArgs e)
        {
            ApplicationExit?.Invoke(sender, e);

            var sensorsWithReceiveApplicationExitSupport = _sensors.Where(x => x as IReceiveExit != null).Select(x => (IReceiveExit)x).ToList();
            foreach (var sensor in sensorsWithReceiveApplicationExitSupport)
            {
                bool sensorIsSender = sender.GetType().Name == sensor.GetType().Name;
                if (!sensorIsSender)
                {
                    sensor.OnApplicationExit(sender, e);
                }
            }
        }

        private void sensor_StatusChangesIn(object sender, StatusChangesInEventArgs e)
        {
            var sensorsWithReceiveStatusChangesInSupport = _sensors.Where(x => x as IReceiveStatusChangesIn != null).Select(x => (IReceiveStatusChangesIn)x).ToList();
            foreach (var sensor in sensorsWithReceiveStatusChangesInSupport)
            {
                bool sensorIsSender = sender.GetType().Name == sensor.GetType().Name;
                if (!sensorIsSender)
                {
                    sensor.OnStatusChangesIn(sender, e);
                }
            }
        }

        private void sensor_ModeChanged(object sender, ModeChangedEventArgs e)
        {
            _currentMode = e.Mode;

            var sensorsWithReceiveModeChangedSupport = _sensors.Where(x => x as IReceiveModeChanged != null).Select(x => (IReceiveModeChanged)x).ToList();
            foreach (var sensor in sensorsWithReceiveModeChangedSupport)
            {
                bool sensorIsSender = sender.GetType().Name == sensor.GetType().Name;
                if (!sensorIsSender)
                {
                    sensor.OnModeHasChanged(sender, e);
                }
            }
        }

        private void sensor_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            StatusChanged?.Invoke(sender, e);

            var sensorsWithReceiveStatusChangedSupport = _sensors.Where(x => x as IReceiveStatusChanged != null).Select(x => (IReceiveStatusChanged)x).ToList();
            foreach (var sensor in sensorsWithReceiveStatusChangedSupport)
            {
                bool sensorIsSender = sender.GetType().Name == sensor.GetType().Name;
                if (!sensorIsSender)
                {
                    sensor.OnSensorStatusHasChanged(sender, e);
                }
            }
        }

        public void OnPhysicalStatusHasChanged(object sender, StatusChangedEventArgs e)
        {
            var sensorsWithReceiveStatusChangedSupport = _sensors.Where(x => x as IReceiveStatusChanged != null).Select(x => (IReceiveStatusChanged)x).ToList();
            foreach (var sensor in sensorsWithReceiveStatusChangedSupport)
            {
                sensor.OnPhysicalStatusHasChanged(sender, e);
            }
        }

        public void Start()
        {
            foreach (var sensor in _sensors)
            {
                string sensorName = sensor.GetType().Name;
                Log.Information(string.Format("Starting sensor {0} ...", sensorName));
                sensor.Start();
            }
        }

        public void Stop()
        {
            foreach (var sensor in _sensors)
            {
                string sensorName = sensor.GetType().Name;

                Log.Information(string.Format("Stopping sensor {0} ...", sensorName));
                sensor.Stop();
            }
        }
    }
}
