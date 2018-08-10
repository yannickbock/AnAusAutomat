using AnAusAutomat.Contracts.Sensor;
using AnAusAutomat.Contracts.Sensor.Events;
using AnAusAutomat.Contracts.Sensor.Features;
using AnAusAutomat.Toolbox.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnAusAutomat.Core.Hubs
{
    public class SensorHub
    {
        private IStateStore _stateStore;
        private IEnumerable<ISensor> _sensors;

        public SensorHub(IStateStore stateStore, IEnumerable<ISensor> sensors)
        {
            _stateStore = stateStore;
            _sensors = sensors;
        }

        public event EventHandler<StatusChangedEventArgs> StatusChanged;
        public event EventHandler<ApplicationExitEventArgs> ApplicationExit;

        public void Initialize(IEnumerable<SensorSettings> settings)
        {
            foreach (var sensor in _sensors)
            {
                string sensorName = sensor.GetType().Name;
                Logger.Information(string.Format("Initializing sensor {0} ...", sensorName));

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
                var modes = _stateStore.GetModes();

                // Call InitializeModes() only once.
                if (hasSendModeChangedSupport)
                {
                    ((ISendModeChanged)sensor).InitializeModes(modes);
                    ((ISendModeChanged)sensor).ModeChanged += sensor_ModeChanged;
                }
                else if (hasReceiveModeChangedSupport)
                {
                    ((IReceiveModeChanged)sensor).InitializeModes(modes);
                }
            }

            if (sensor as ISendStatusForecast != null)
            {
                ((ISendStatusForecast)sensor).StatusForecast += sensor_StatusForecast;
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

        private void sensor_StatusForecast(object sender, StatusForecastEventArgs e)
        {
            var sensorsWithReceiveStatusForecastSupport = _sensors.Where(x => x as IReceiveStatusForecast != null).Select(x => (IReceiveStatusForecast)x).ToList();
            foreach (var sensor in sensorsWithReceiveStatusForecastSupport)
            {
                bool sensorIsSender = sender.GetType().Name == sensor.GetType().Name;
                if (!sensorIsSender)
                {
                    sensor.OnStatusForecast(sender, e);
                }
            }
        }

        private void sensor_ModeChanged(object sender, ModeChangedEventArgs e)
        {
            _stateStore.SetModeState(e.Mode);

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
                Logger.Information(string.Format("Starting sensor {0} ...", sensorName));
                sensor.Start();
            }
        }

        public void Stop()
        {
            foreach (var sensor in _sensors)
            {
                string sensorName = sensor.GetType().Name;
                Logger.Information(string.Format("Stopping sensor {0} ...", sensorName));
                sensor.Stop();
            }
        }
    }
}
