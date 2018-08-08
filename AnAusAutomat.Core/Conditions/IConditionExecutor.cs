using AnAusAutomat.Contracts;
using System.Collections.Generic;

namespace AnAusAutomat.Core.Conditions
{
    public interface IConditionExecutor
    {
        bool IsTrue(Dictionary<Socket, PowerStatus> physicalStates, Dictionary<string, PowerStatus> sensorStates);
    }
}
