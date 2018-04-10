using AnAusAutomat.Contracts.Controller;
using AnAusAutomat.Contracts.Sensor;
using AnAusAutomat.Core.Conditions;
using System.Collections.Generic;

namespace AnAusAutomat.Core.Configuration
{
    public abstract class AppConfig
    {
        public IEnumerable<SensorSettings> Sensors { get; protected set; }

        public IEnumerable<Condition> Conditions { get; protected set; }

        public IEnumerable<string> Modes { get; protected set; }

        public string DefaultMode { get; protected set; }
    }
}