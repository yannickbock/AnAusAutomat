using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Sensor;
using System.Collections.Generic;

namespace AnAusAutomat.Sensors.GUI
{
    public class GUIBuilder : ISensorBuilder
    {
        private List<ConditionMode> _modes;
        private List<SensorParameter> _parameters;
        private Dictionary<Socket, IEnumerable<SensorParameter>> _sockets;

        public GUIBuilder()
        {
            _modes = new List<ConditionMode>();
            _parameters = new List<SensorParameter>();
            _sockets = new Dictionary<Socket, IEnumerable<SensorParameter>>();
        }

        public void AddMode(ConditionMode mode)
        {
            _modes.Add(mode);
        }

        public void AddParameter(SensorParameter parameter)
        {
            _parameters.Add(parameter);
        }

        public void AddSocket(Socket socket, IEnumerable<SensorParameter> parameters)
        {
            _sockets.Add(socket, parameters);
        }

        public ISensor Build()
        {
            return new GUI(_sockets.Keys, _modes);
        }
    }
}
