using AnAusAutomat.Contracts.Sensor;
using AnAusAutomat.Contracts.Sensor.Events;
using AnAusAutomat.Core.Conditions;
using AnAusAutomat.Core.Configuration;
using AnAusAutomat.Core.Hubs;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        public App(AppConfig appConfig)
        {
            _appConfig = appConfig;

            // Bootstrapper ?
        }

        private void _sensorHub_ApplicationExit(object sender, ApplicationExitEventArgs e)
        {
            Stop();
        }

        private void _sensorHub_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            string triggeredBy = sender.GetType().Name;
            _stateStore.SetSensorState(e.Socket, triggeredBy, e.Status);

            var condition = _conditionFilter.Filter(e.Socket, triggeredBy).FirstOrDefault();
            if (condition != null)
            {
                switch (condition.ResultingStatus)
                {
                    case PowerStatus.On:
                        _stateStore.SetPhysicalState(e.Socket, PowerStatus.On);
                        _controllerHub.TurnOn(e.Socket);
                        _sensorHub.OnPhysicalStatusHasChanged(sender, new StatusChangedEventArgs(e.Message, condition.Text, e.Socket, PowerStatus.On));
                        break;
                    case PowerStatus.Off:
                        _stateStore.SetPhysicalState(e.Socket, PowerStatus.Off);
                        _controllerHub.TurnOff(e.Socket);
                        _sensorHub.OnPhysicalStatusHasChanged(sender, new StatusChangedEventArgs(e.Message, condition.Text, e.Socket, PowerStatus.Off));
                        break;
                }
            }
        }

        public void Initialize()
        {
            string rootDirectoryPath = getRootDirectoryPath();
            string sensorsDirectoryPath = rootDirectoryPath + "\\Sensors";
            string controllersDirectoryPath = rootDirectoryPath + "\\Controllers";

            var pluginLoader = new PluginLoader();
            var sensors = pluginLoader.LoadSensors(sensorsDirectoryPath);
            var controllers = pluginLoader.LoadControllers(controllersDirectoryPath);

            _sensorHub = new SensorHub(sensors, _appConfig.Modes, _appConfig.DefaultMode);
            _sensorHub.StatusChanged += _sensorHub_StatusChanged;
            _sensorHub.ApplicationExit += _sensorHub_ApplicationExit;

            _controllerHub = new ControllerHub(controllers);

            _stateStore = new StateStore();
            var conditions = compile(_appConfig.Conditions.Where(x => x.Type == ConditionType.Regular));
            _conditionFilter = new ConditionFilter(_stateStore, conditions);

            _controllerHub.Connect();
            _sensorHub.Initialize(_appConfig.Sensors);
        }

        private IEnumerable<Condition> compile(IEnumerable<ConditionSettings> conditionSettings)
        {
            var compiler = new ConditionCompiler();

            var conditions = conditionSettings
                .Select(x => compiler.Compile(x))
                .Where(x => x != null)
                .ToList();

            return conditions;
        }

        private string getRootDirectoryPath()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);

            return Path.GetDirectoryName(path);
        }

        public void Start()
        {
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
                switch (status.ResultingStatus)
                {
                    case PowerStatus.On:
                        _stateStore.SetPhysicalState(status.Socket, PowerStatus.On);
                        _controllerHub.TurnOn(status.Socket);
                        _sensorHub.OnPhysicalStatusHasChanged(this, new StatusChangedEventArgs("", status.Type.ToString(), status.Socket, PowerStatus.On));
                        break;
                    case PowerStatus.Off:
                        _stateStore.SetPhysicalState(status.Socket, PowerStatus.Off);
                        _controllerHub.TurnOff(status.Socket);
                        _sensorHub.OnPhysicalStatusHasChanged(this, new StatusChangedEventArgs("", status.Type.ToString(), status.Socket, PowerStatus.Off));
                        break;
                }
            }
        }
    }
}
