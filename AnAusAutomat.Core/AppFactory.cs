using AnAusAutomat.Contracts.Sensor;
using AnAusAutomat.Core.Configuration;
using AnAusAutomat.Core.Hubs;
using AnAusAutomat.Core.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AnAusAutomat.Core
{
    public static class AppFactory
    {
        public static App Create(AppConfig appConfig)
        {
            string rootDirectoryPath = getRootDirectoryPath();
            string sensorsDirectoryPath = rootDirectoryPath + "\\Sensors";
            string controllersDirectoryPath = rootDirectoryPath + "\\Controllers";

            var sensorNames = appConfig.Sensors.Select(x => x.SensorName);

            var sensorLoader = new SensorLoader(sensorsDirectoryPath);
            var controllerLoader = new ControllerLoader(controllersDirectoryPath);
            var sensorBuilders = sensorLoader.Load(sensorNames);
            var controllers = controllerLoader.Load();
            var sensors = buildSensors(sensorBuilders, appConfig);

            var stateStore = new StateStore();
            stateStore.SetModes(appConfig.Modes);

            var sensorHub = new SensorHub(stateStore, sensors);
            var controllerHub = new ControllerHub(controllers);

            return new App(stateStore, sensorHub, controllerHub, appConfig);
        }

        private static IEnumerable<ISensor> buildSensors(IEnumerable<ISensorBuilder> builders, AppConfig appConfig)
        {
            var list = new List<ISensor>();

            foreach (var sensorSettings in appConfig.Sensors)
            {
                var builder = builders.FirstOrDefault(x => x.GetType().Name.Replace("Builder", "") == sensorSettings.SensorName);

                foreach (var mode in appConfig.Modes)
                {
                    builder.AddMode(mode);
                }

                foreach (var socket in sensorSettings.Sockets)
                {
                    builder.AddSocket(socket, socket.Parameters);
                }

                foreach (var parameter in sensorSettings.Parameters)
                {
                    builder.AddParameter(parameter);
                }

                var sensor = builder.Build();
                list.Add(sensor);
            }

            return list;
        }

        private static string getRootDirectoryPath()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);

            return Path.GetDirectoryName(path);
        }
    }
}
