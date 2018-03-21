using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Sensor;
using System.Collections.Generic;
using System.Linq;

namespace AnAusAutomat.Core
{
    public class SocketStates
    {
        public SocketStates(Socket socket)
        {
            Socket = socket;
            PhysicalStatus = PowerStatus.Undefined;
            SensorStates = new Dictionary<string, PowerStatus>();
        }

        public Socket Socket { get; private set; }

        public PowerStatus PhysicalStatus { get; set; }

        /// <summary>
        /// SensorName, Status
        /// </summary>
        public Dictionary<string, PowerStatus> SensorStates { get; set; }
    }
}
