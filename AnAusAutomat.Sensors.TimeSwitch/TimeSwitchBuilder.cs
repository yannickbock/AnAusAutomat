using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Sensor;
using System.Collections.Generic;
using System.Linq;

namespace AnAusAutomat.Sensors.TimeSwitch
{
    public class TimeSwitchBuilder : ISensorBuilder
    {
        private List<SensorParameter> _parameters;
        private Dictionary<Socket, IEnumerable<SensorParameter>> _sockets;

        public TimeSwitchBuilder()
        {
            _parameters = new List<SensorParameter>();
            _sockets = new Dictionary<Socket, IEnumerable<SensorParameter>>();
        }

        public void AddMode(ConditionMode mode)
        {
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
            var temp = _sockets.Select(x => new SocketWithSensorParameters(x.Key.ID, x.Key.Name, x.Value)).ToList();
            var settings = new SensorSettings("TimeSwitch", _parameters, temp);

            var timeSwitch = new TimeSwitch();
            timeSwitch.Initialize(settings);
            return timeSwitch;
        }
    }
}
