using System;
using System.Collections.Generic;

namespace AnAusAutomat.Contracts.Sensor
{
    [Obsolete]
    public class SensorSettings
    {
        public SensorSettings(string sensorName, IEnumerable<SensorParameter> parameters, IEnumerable<SocketWithSensorParameters> sockets)
        {
            SensorName = sensorName;
            Parameters = parameters;
            Sockets = sockets;
        }

        public string SensorName { get; private set; }

        public IEnumerable<SensorParameter> Parameters { get; private set; }

        public IEnumerable<SocketWithSensorParameters> Sockets { get; private set; }
    }
}
