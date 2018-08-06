using System.Collections.Generic;
using AnAusAutomat.Contracts;

namespace AnAusAutomat.Core
{
    public interface IStateStore
    {
        IEnumerable<ConditionMode> GetModes();
        PowerStatus GetPhysicalState(Socket socket);
        Dictionary<string, PowerStatus> GetSensorStates(Socket socket);
        void SetModeState(ConditionMode mode);
        void SetModes(IEnumerable<ConditionMode> modes);
        void SetPhysicalState(Socket socket, PowerStatus status);
        void SetSensorState(Socket socket, string sensorName, PowerStatus status);
    }
}