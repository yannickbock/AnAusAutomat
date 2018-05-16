using System.Collections.Generic;
using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Sensor;

namespace AnAusAutomat.Core
{
    public interface IStateStore
    {
        string GetCurrentMode();
        IEnumerable<string> GetModes();
        PowerStatus GetPhysicalState(Socket socket);
        Dictionary<string, PowerStatus> GetSensorStates(Socket socket);
        void SetCurrentMode(string mode);
        void SetModes(IEnumerable<string> modes);
        void SetPhysicalState(Socket socket, PowerStatus status);
        void SetSensorState(Socket socket, string sensorName, PowerStatus status);
    }
}