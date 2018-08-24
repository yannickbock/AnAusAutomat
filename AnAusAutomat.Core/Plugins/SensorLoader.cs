using AnAusAutomat.Contracts.Sensor;
using AnAusAutomat.Toolbox.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AnAusAutomat.Core.Plugins
{
    public class SensorLoader
    {
        private string _directoryPath;

        public SensorLoader(string directoryPath)
        {
            _directoryPath = directoryPath;
        }

        public IEnumerable<ISensorBuilder> Load(IEnumerable<string> sensorNames)
        {
            Logger.Information(string.Format("Searching sensors in directory {0} ...", _directoryPath));

            var files = getFiles();
            var sensors = new List<ISensorBuilder>();
            foreach (string file in files)
            {
                try
                {
                    var types = Assembly.LoadFrom(file).GetTypes();
                    var filterResult = types.Where(x => typeof(ISensorBuilder).IsAssignableFrom(x));
                    bool sensorFoundButNotConfigured = filterResult.Any(x => !sensorNames.Contains(x.Name.Replace("Builder", "")));
                    int count = filterResult.Count(x => sensorNames.Contains(x.Name.Replace("Builder", "")));

                    Logger.Debug(string.Format("Searching for sensor in {0}", file));

                    if (sensorFoundButNotConfigured)
                    {
                        Logger.Warning("Sensor found but not configured. Skipping.");
                    }

                    if (count > 1)
                    {
                        Logger.Warning(string.Format("More then one sensor found. Just loading the first."));
                    }

                    if (count >= 1)
                    {
                        var sensorType = types.FirstOrDefault(x => typeof(ISensorBuilder).IsAssignableFrom(x));
                        var sensor = Activator.CreateInstance(sensorType) as ISensorBuilder;

                        Logger.Information(string.Format("Found {0} sensor.", sensorType.Name.Replace("Sensor", "")));
                        sensors.Add(sensor);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e, string.Format("Error while searching for sensor in {0}", file));
                }
            }

            return sensors;
        }

        private IEnumerable<string> getFiles()
        {
            var directories = Directory.GetDirectories(_directoryPath, "*", SearchOption.TopDirectoryOnly);
            return directories.SelectMany(x => Directory.GetFiles(x, "*.dll", SearchOption.TopDirectoryOnly));
        }

        private void logUnconfiguredSensors(IEnumerable<Type> sensors, IEnumerable<string> sensorNames)
        {
            var s = sensors.Where(x => !sensorNames.Contains(x.Name));
            if (s.Any())
            {
                var result = s.Select(x => x.Name);

                if (result.Count() == 1)
                {
                    Logger.Warning(string.Format("Found unconfigured sensor: {0}", result.FirstOrDefault()));
                }
            }
        }
    }
}
