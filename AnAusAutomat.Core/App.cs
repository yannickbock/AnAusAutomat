using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Sensor.Events;
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
        private AppConfig _appConfig;
        private SensorHub _sensorHub;
        private ControllerHub _controllerHub;
        private ConditionFilter _conditionFilter;
        private IStateStore _stateStore;

        public App(IStateStore stateStore, SensorHub sensorHub, ControllerHub controllerHub, AppConfig appConfig)
        {
            _stateStore = stateStore;
            _sensorHub = sensorHub;
            _controllerHub = controllerHub;
            _appConfig = appConfig;

            _sensorHub.StatusChanged += _sensorHub_StatusChanged;
            _sensorHub.ApplicationExit += _sensorHub_ApplicationExit;
        }

        private void _sensorHub_ApplicationExit(object sender, ApplicationExitEventArgs e)
        {
            Stop();
        }

        private void _sensorHub_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            string triggeredBy = sender.GetType().Name;
            _stateStore.SetSensorState(e.Socket, triggeredBy, e.Status);

            var condition = _conditionFilter.FilterBySensor(e.Socket, triggeredBy).FirstOrDefault();
            if (condition != null)
            {
                turnOnOrOff(
                    socket: e.Socket,
                    status: condition.ResultingStatus,
                    condition: condition.Text,
                    message: e.Message,
                    sender: sender);

                var relatedConditions = _conditionFilter.FilterByRelatedSocket(e.Socket);
                if (relatedConditions.Any())
                {
                    foreach (var x in relatedConditions)
                    {
                        turnOnOrOff(
                            x.Socket,
                            x.ResultingStatus,
                            x.Text,
                            e.Message,
                            sender);
                    }
                }
            }
        }

        public void Initialize()
        {
            var conditionCompiler = new ConditionCompiler();
            var conditions = conditionCompiler.Compile(_appConfig.Conditions.Where(x => x.Type == ConditionType.Regular));
            _conditionFilter = new ConditionFilter(_stateStore, conditions);

            _sensorHub.Initialize(_appConfig.Sensors);
        }

        public void Start()
        {
            _controllerHub.Connect();
            _sensorHub.Start();

            var startupStates = _appConfig.Conditions.Where(x => x.Type == ConditionType.Startup);
            applyStartupOrShutdownStates(startupStates);
        }

        public void Stop()
        {
            var shutdownStates = _appConfig.Conditions.Where(x => x.Type == ConditionType.Shutdown);
            applyStartupOrShutdownStates(shutdownStates);

            _sensorHub.Stop();
            Thread.Sleep(500);
            _controllerHub.Disconnect();

            Log.Information("Good bye! ;)");
            Thread.Sleep(1000);
            Environment.Exit(0);
        }

        private void applyStartupOrShutdownStates(IEnumerable<ConditionSettings> startupOrShutdownConditions)
        {
            foreach (var status in startupOrShutdownConditions)
            {
                turnOnOrOff(
                    socket: status.Socket,
                    status: status.ResultingStatus,
                    condition: status.Type.ToString(),
                    message: "",
                    sender: this);
            }
        }

        private void turnOnOrOff(Socket socket, PowerStatus status, string condition, string message, object sender)
        {
            if (status != PowerStatus.Undefined)
            {
                _stateStore.SetPhysicalState(socket, status);
                _sensorHub.OnPhysicalStatusHasChanged(sender, new StatusChangedEventArgs(message, condition, socket, status));
                switch (status)
                {
                    case PowerStatus.On:
                        _controllerHub.TurnOn(socket);
                        break;
                    case PowerStatus.Off:
                        _controllerHub.TurnOff(socket);
                        break;
                }
            }
        }
    }
}
