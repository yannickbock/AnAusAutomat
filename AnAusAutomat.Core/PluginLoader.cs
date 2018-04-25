using AnAusAutomat.Contracts.Controller;
using AnAusAutomat.Contracts.Sensor;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AnAusAutomat.Core
{
    public class PluginLoader
    {
        public IEnumerable<ISensor> LoadSensors(string directoryPath)
        {
            Log.Information(string.Format("Loading sensors in directory \"{0}\" ...", directoryPath));

            var directories = Directory.GetDirectories(directoryPath, "*", SearchOption.AllDirectories);
            var files = directories.SelectMany(x => Directory.GetFiles(x, "*.dll", SearchOption.TopDirectoryOnly));

            var sensors = new List<ISensor>();
            var sensorQualifiedName = typeof(ISensor).AssemblyQualifiedName;
            foreach (string file in files)
            {
                try
                {
                    var types = Assembly.LoadFrom(file).GetTypes();
                    var sensorType = types.FirstOrDefault(x => typeof(ISensor).IsAssignableFrom(x));

                    var sensor = Activator.CreateInstance(sensorType) as ISensor;
                    sensors.Add(sensor);
                }
                catch (Exception e)
                {

                }
            }

            return sensors;
        }

        public IEnumerable<IController> LoadControllers(string directoryPath)
        {
            Log.Information(string.Format("Loading controllers in directory \"{0}\" ...", directoryPath));

            var directories = Directory.GetDirectories(directoryPath, "*", SearchOption.AllDirectories);
            var files = directories.SelectMany(x => Directory.GetFiles(x, "*.dll", SearchOption.TopDirectoryOnly));

            var controllers = new List<IController>();
            var controllerFactoryQualifiedName = typeof(IControllerFactory).AssemblyQualifiedName;
            foreach (string file in files)
            {
                try
                {
                    var types = Assembly.LoadFrom(file).GetTypes();
                    var controllerFactoryType = types.FirstOrDefault(x => typeof(IControllerFactory).IsAssignableFrom(x));

                    var factory = Activator.CreateInstance(controllerFactoryType) as IControllerFactory;
                    controllers.AddRange(factory.Create());
                }
                catch (Exception e)
                {

                }
            }

            return controllers;
        }
    }
}
