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
    public class Resolver
    {
        public IEnumerable<ISensor> LoadSensors()
        {
            Log.Information(string.Format("Loading sensors in directory \"{0}\" ...", SensorsDirectory));

            var directories = Directory.GetDirectories(SensorsDirectory, "*", SearchOption.AllDirectories);
            var files = directories.SelectMany(x => Directory.GetFiles(x, "*.dll", SearchOption.TopDirectoryOnly));

            var sensors = new List<ISensor>();
            var sensorQualifiedName = typeof(ISensor).AssemblyQualifiedName;
            foreach (string file in files)
            {
                try
                {
                    var types = Assembly.LoadFrom(file).GetTypes();
                    var sensorType = types.FirstOrDefault(x => x.GetInterfaces().FirstOrDefault(y => y.AssemblyQualifiedName == sensorQualifiedName) != null);

                    //var isaf = controllerFactoryType.IsAssignableFrom(typeof(IControllerFactory)); // false, why?

                    var sensor = Activator.CreateInstance(sensorType) as ISensor;
                    sensors.Add(sensor);
                }
                catch (Exception e)
                {

                }
            }

            return sensors;
        }

        public IEnumerable<IController> LoadControllers()
        {
            Log.Information(string.Format("Loading controllers in directory \"{0}\" ...", ControllersDirectory));

            var directories = Directory.GetDirectories(ControllersDirectory, "*", SearchOption.AllDirectories);
            var files = directories.SelectMany(x => Directory.GetFiles(x, "*.dll", SearchOption.TopDirectoryOnly));

            var controllers = new List<IController>();
            var controllerFactoryQualifiedName = typeof(IControllerFactory).AssemblyQualifiedName;
            foreach (string file in files)
            {
                try
                {
                    var types = Assembly.LoadFrom(file).GetTypes();
                    var controllerFactoryType = types.FirstOrDefault(x => x.GetInterfaces().FirstOrDefault(y => y.AssemblyQualifiedName == controllerFactoryQualifiedName) != null);

                    //var isaf = controllerFactoryType.IsAssignableFrom(typeof(IControllerFactory)); // false, why?

                    var factory = Activator.CreateInstance(controllerFactoryType) as IControllerFactory;
                    controllers.AddRange(factory.Create());
                }
                catch (Exception e)
                {

                }
            }

            return controllers;
        }

        public static string BaseDirectory
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);

                return Path.GetDirectoryName(path);
            }
        }

        public static string SensorsDirectory
        {
            get
            {
                return BaseDirectory + "\\Sensors";
            }
        }

        public static string ControllersDirectory
        {
            get
            {
                return BaseDirectory + "\\Controllers";
            }
        }
    }
}
