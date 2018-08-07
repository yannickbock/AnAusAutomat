using AnAusAutomat.Contracts;
using System.Collections.Generic;

namespace AnAusAutomat.Core.Conditions
{
    public interface IConditionChecker
    {
        bool IsTrue(Dictionary<Socket, PowerStatus> physicalStates, Dictionary<string, PowerStatus> sensorStates);
    }
}
