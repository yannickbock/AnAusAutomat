using AnAusAutomat.Contracts.Sensor;
using System.Collections.Generic;
using System.Linq;

namespace AnAusAutomat.Sensors.GUI.Internals
{
    public class SettingsParser
    {
        public Settings Parse(IEnumerable<SensorParameter> parameters)
        {
            bool startMinimized = true;

            if (parameters.Any())
            {
                bool startMinimizedDefined = parameters.Any(x => x.Name == "StartMinimized");

                if (startMinimizedDefined)
                {
                    string startMinimizedAsString = parameters.FirstOrDefault(x => x.Name == "StartMinimized").Value.ToLower();
                    startMinimized = new string[] { "true", "yes", "1" }.Contains(startMinimizedAsString);
                }
            }

            return new Settings(startMinimized);
        }
    }
}
