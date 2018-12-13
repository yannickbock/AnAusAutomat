using System.Collections.Generic;

namespace AnAusAutomat.Contracts.Sensor
{
    public class SocketWithSensorParameters : Socket
    {
        public SocketWithSensorParameters(int id, string name, IEnumerable<SensorParameter> parameters) : base(id, name)
        {
            Parameters = parameters;
        }

        public IEnumerable<SensorParameter> Parameters { get; private set; }
    }
}
