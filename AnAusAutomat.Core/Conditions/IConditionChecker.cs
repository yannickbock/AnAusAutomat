using AnAusAutomat.Contracts.Sensor;
using System.Collections.Generic;

namespace AnAusAutomat.Core.Conditions
{
    public interface IConditionChecker
    {
        bool IsTrue(PowerStatus physicalStatus, Dictionary<string, PowerStatus> sensorStates);
    }
}
