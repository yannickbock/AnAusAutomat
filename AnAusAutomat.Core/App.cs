using AnAusAutomat.Contracts.Sensor;
using AnAusAutomat.Contracts.Sensor.Events;
using AnAusAutomat.Contracts.Sensor.Metadata;
using AnAusAutomat.Core.Conditions;
using AnAusAutomat.Core.Configuration;
using AnAusAutomat.Core.Hubs;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AnAusAutomat.Core
{
    public class App
    {
        private AppConfig _config;

        private SensorHub _sensorHub;
        private ControllerHub _controllerHub;
        private ConditionTester _conditionTester;

        public App(AppConfig appConfig)
        {
            _config = appConfig;

            _controllerHub = new ControllerHub(appConfig.Devices);
            _conditionTester = new ConditionTester(appConfig.Conditions);
            _sensorHub = new SensorHub("sensors", appConfig.Modes, appConfig.DefaultMode);
            _sensorHub.StatusChanged += _sensorHub_StatusChanged;
            _sensorHub.ApplicationExit += _sensorHub_ApplicationExit;
        }

        private void _sensorHub_ApplicationExit(object sender, ApplicationExitEventArgs e)
        {
            Stop();
        }

        private void _sensorHub_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            _conditionTester.UpdateSensorStatus(e.Socket, e.TriggeredBy.Name, e.Status);

            var condition = _conditionTester.CheckConditions(e.Socket, e.TriggeredBy.Name, _config.DefaultMode);
            if (condition != null)
            {
                switch (condition.ResultingStatus)
                {
                    case PowerStatus.On:
                        _conditionTester.UpdatePhysicalStatus(e.Socket, PowerStatus.On);
                        _controllerHub.TurnOn(e.Socket);
                        _sensorHub.OnPhysicalStatusHasChanged(sender, new StatusChangedEventArgs(e.Message, condition.Command, e.TriggeredBy, e.Socket, PowerStatus.On));
                        break;
                    case PowerStatus.Off:
                        _conditionTester.UpdatePhysicalStatus(e.Socket, PowerStatus.Off);
                        _controllerHub.TurnOff(e.Socket);
                        _sensorHub.OnPhysicalStatusHasChanged(sender, new StatusChangedEventArgs(e.Message, condition.Command, e.TriggeredBy, e.Socket, PowerStatus.Off));
                        break;
                }
            }
        }

        public void Initialize()
        {
            _controllerHub.Connect();
            _sensorHub.LoadSensors();
            _sensorHub.Initialize(_config.Sensors);
            _conditionTester.Compile();
        }

        public void Start()
        {
            _sensorHub.Start();

            var startupStates = _config.Conditions.Where(x => x.Type == ConditionType.Startup);
            applyStartupOrShutdownStates(startupStates);
        }

        public void Stop()
        {
            var shutdownStates = _config.Conditions.Where(x => x.Type == ConditionType.Shutdown);
            applyStartupOrShutdownStates(shutdownStates);

            _sensorHub.Stop();
            Thread.Sleep(500);
            _controllerHub.Disconnect();

            Log.Information("Good bye! ;)");
            Thread.Sleep(1000);
            Environment.Exit(0);
        }

        private void applyStartupOrShutdownStates(IEnumerable<Condition> startupOrShutdownConditions)
        {
            foreach (var status in startupOrShutdownConditions)
            {
                _conditionTester.UpdatePhysicalStatus(status.Socket, status.ResultingStatus);
                var triggeredBy = new SensorMetadata("AppCore", "", "");

                switch (status.ResultingStatus)
                {
                    case PowerStatus.On:
                        _conditionTester.UpdatePhysicalStatus(status.Socket, PowerStatus.On);
                        _controllerHub.TurnOn(status.Socket);
                        _sensorHub.OnPhysicalStatusHasChanged(this, new StatusChangedEventArgs("", status.Type.ToString(), triggeredBy, status.Socket, PowerStatus.On));
                        break;
                    case PowerStatus.Off:
                        _conditionTester.UpdatePhysicalStatus(status.Socket, PowerStatus.Off);
                        _controllerHub.TurnOff(status.Socket);
                        _sensorHub.OnPhysicalStatusHasChanged(this, new StatusChangedEventArgs("", status.Type.ToString(), triggeredBy, status.Socket, PowerStatus.Off));
                        break;
                }
            }
        }
    }
}
