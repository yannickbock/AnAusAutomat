using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Sensor;
using AnAusAutomat.Core.Conditions;
using System.Collections.Generic;

namespace AnAusAutomat.Core.Configuration
{
    public class AppConfig
    {
        public AppConfig(IEnumerable<SensorSettings> sensors, IEnumerable<ConditionSettings> conditions, IEnumerable<ConditionMode> modes)
        {
            Sensors = sensors;
            Conditions = conditions;
            Modes = modes;
        }

        public IEnumerable<SensorSettings> Sensors { get; private set; }

        public IEnumerable<ConditionSettings> Conditions { get; private set; }

        public IEnumerable<ConditionMode> Modes { get; private set; }
    }
}