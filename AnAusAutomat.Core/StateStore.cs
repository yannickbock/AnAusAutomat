using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Sensor;
using System.Collections.Generic;

namespace AnAusAutomat.Core
{
    public class StateStore : IStateStore
    {
        private Dictionary<Socket, PowerStatus> _physicalStates;
        private Dictionary<Socket, Dictionary<string, PowerStatus>> _sensorStates;
        private string _currentMode;

        public StateStore()
        {
            _physicalStates = new Dictionary<Socket, PowerStatus>();
            _sensorStates = new Dictionary<Socket, Dictionary<string, PowerStatus>>();
            _currentMode = string.Empty;
        }

        public PowerStatus GetPhysicalState(Socket socket)
        {
            return _physicalStates.ContainsKey(socket) ? _physicalStates[socket] : PowerStatus.Undefined;
        }

        public void SetPhysicalState(Socket socket, PowerStatus status)
        {
            _physicalStates[socket] = status;
        }

        public Dictionary<string, PowerStatus> GetSensorStates(Socket socket)
        {
            if (!_sensorStates.ContainsKey(socket))
            {
                return new Dictionary<string, PowerStatus>();
            }

            return _sensorStates[socket];
        }

        public void SetSensorState(Socket socket, string sensorName, PowerStatus status)
        {
            if (!_sensorStates.ContainsKey(socket))
            {
                _sensorStates[socket] = new Dictionary<string, PowerStatus>();
            }

            _sensorStates[socket][sensorName] = status;
        }

        public string GetCurrentMode()
        {
            return _currentMode;
        }

        public void SetCurrentMode(string mode)
        {
            _currentMode = mode;
        }
    }
}
