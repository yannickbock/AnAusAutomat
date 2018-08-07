using AnAusAutomat.Contracts.Controller;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AnAusAutomat.Core.Plugins
{
    public class ControllerLoader
    {
        private string _directoryPath;

        public ControllerLoader(string directoryPath)
        {
            _directoryPath = directoryPath;
        }

        public IEnumerable<IController> Load()
        {
            Log.Debug(string.Format("Searching controllers in directory {0} ...", _directoryPath));

            var files = getFiles();
            var controllers = new List<IController>();
            foreach (string file in files)
            {
                try
                {
                    var types = Assembly.LoadFrom(file).GetTypes();
                    int count = types.Count(x => typeof(IControllerFactory).IsAssignableFrom(x));

                    Log.Debug(string.Format("Searching for controller in {0}", file));

                    if (count > 1)
                    {
                        Log.Warning(string.Format("More then one controller found. Just loading the first.", file));
                    }

                    if (count >= 1)
                    {
                        var controllerFactoryType = types.FirstOrDefault(x => typeof(IControllerFactory).IsAssignableFrom(x));
                        var factory = Activator.CreateInstance(controllerFactoryType) as IControllerFactory;

                        Log.Information(string.Format("Found {0} controller", controllerFactoryType.Name.Replace("Controller", "").Replace("Factory", ""), file));
                        controllers.AddRange(factory.Create());
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, string.Format("Error while searching for controller in {0}", file));
                }
            }

            return controllers;
        }

        private IEnumerable<string> getFiles()
        {
            var directories = Directory.GetDirectories(_directoryPath, "*", SearchOption.TopDirectoryOnly);
            return directories.SelectMany(x => Directory.GetFiles(x, "*.dll", SearchOption.TopDirectoryOnly));
        }
    }
}
