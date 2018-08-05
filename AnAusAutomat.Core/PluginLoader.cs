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
            foreach (string file in files)
            {
                try
                {
                    var types = Assembly.LoadFrom(file).GetTypes();
                    int count = types.Count(x => typeof(ISensor).IsAssignableFrom(x));

                    Log.Debug(string.Format("Searching for sensor in {0}", file));

                    if (count == 1)
                    {
                        var sensorType = types.FirstOrDefault(x => typeof(ISensor).IsAssignableFrom(x));
                        var sensor = Activator.CreateInstance(sensorType) as ISensor;
                        sensors.Add(sensor);
                    }
                    else if (count > 1)
                    {
                        Log.Warning(string.Format("More then one sensor found in {0}. Just loading the first.", file));
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, string.Format("Error while searching for sensor in {0}", file));
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
            foreach (string file in files)
            {
                try
                {
                    var types = Assembly.LoadFrom(file).GetTypes();
                    int count = types.Count(x => typeof(IControllerFactory).IsAssignableFrom(x));

                    Log.Debug(string.Format("Searching for controller in {0}", file));
                    if (count == 1)
                    {
                        var controllerFactoryType = types.FirstOrDefault(x => typeof(IControllerFactory).IsAssignableFrom(x));
                        var factory = Activator.CreateInstance(controllerFactoryType) as IControllerFactory;
                        controllers.AddRange(factory.Create());
                    }
                    else if (count > 1)
                    {
                        Log.Warning(string.Format("More then one controller found in {0}. Just loading the first.", file));
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, string.Format("Error while searching for controller in {0}", file));
                }
            }

            return controllers;
        }
    }
}
