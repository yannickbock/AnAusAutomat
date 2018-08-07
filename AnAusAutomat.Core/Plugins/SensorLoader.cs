using AnAusAutomat.Contracts.Sensor;
using Serilog;
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

        public IEnumerable<ISensor> Load()
        {
            Log.Information(string.Format("Searching sensors in directory {0} ...", _directoryPath));

            var files = getFiles();
            var sensors = new List<ISensor>();
            foreach (string file in files)
            {
                try
                {
                    var types = Assembly.LoadFrom(file).GetTypes();
                    int count = types.Count(x => typeof(ISensor).IsAssignableFrom(x));

                    Log.Debug(string.Format("Searching for sensor in {0}", file));

                    if (count > 1)
                    {
                        Log.Warning(string.Format("More then one sensor found. Just loading the first."));
                    }

                    if (count >= 1)
                    {
                        var sensorType = types.FirstOrDefault(x => typeof(ISensor).IsAssignableFrom(x));
                        var sensor = Activator.CreateInstance(sensorType) as ISensor;

                        Log.Information(string.Format("Found {0} sensor.", sensorType.Name.Replace("Sensor", "")));
                        sensors.Add(sensor);
                    }
                    
                }
                catch (Exception e)
                {
                    Log.Error(e, string.Format("Error while searching for sensor in {0}", file));
                }
            }

            return sensors;
        }

        private IEnumerable<string> getFiles()
        {
            var directories = Directory.GetDirectories(_directoryPath, "*", SearchOption.TopDirectoryOnly);
            return directories.SelectMany(x => Directory.GetFiles(x, "*.dll", SearchOption.TopDirectoryOnly));
        }
    }
}
