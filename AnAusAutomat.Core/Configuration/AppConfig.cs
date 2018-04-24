using AnAusAutomat.Contracts.Sensor;
using AnAusAutomat.Core.Conditions;
using System.Collections.Generic;

namespace AnAusAutomat.Core.Configuration
{
    public class AppConfig
    {
        public AppConfig(IEnumerable<SensorSettings> sensors, IEnumerable<Condition> conditions, IEnumerable<string> modes, string defaultMode)
        {
            Sensors = sensors;
            Conditions = conditions;
            Modes = modes;
            DefaultMode = defaultMode;
        }

        public IEnumerable<SensorSettings> Sensors { get; private set; }

        public IEnumerable<Condition> Conditions { get; private set; }

        public IEnumerable<string> Modes { get; private set; }

        public string DefaultMode { get; private set; }
    }
}