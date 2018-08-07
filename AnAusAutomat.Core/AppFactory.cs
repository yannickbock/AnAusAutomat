using AnAusAutomat.Core.Configuration;
using AnAusAutomat.Core.Hubs;
using AnAusAutomat.Core.Plugins;
using System;
using System.IO;
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

            var sensorLoader = new SensorLoader(sensorsDirectoryPath);
            var controllerLoader = new ControllerLoader(controllersDirectoryPath);
            var sensors = sensorLoader.Load();
            var controllers = controllerLoader.Load();

            var stateStore = new StateStore();
            stateStore.SetModes(appConfig.Modes);

            var sensorHub = new SensorHub(stateStore, sensors);
            var controllerHub = new ControllerHub(controllers);

            return new App(stateStore, sensorHub, controllerHub, appConfig);
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
