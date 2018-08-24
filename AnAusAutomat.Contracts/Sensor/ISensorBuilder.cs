using System.Collections.Generic;

namespace AnAusAutomat.Contracts.Sensor
{
    public interface ISensorBuilder
    {
        void AddParameter(SensorParameter parameter);

        void AddSocket(Socket socket, IEnumerable<SensorParameter> parameters);

        void AddMode(ConditionMode mode);

        ISensor Build();
    }
}
