using System;
using System.Collections.Generic;

namespace AnAusAutomat.Controllers.Proprietary.Internals
{
    public class ProprietaryDevice : IEquatable<ProprietaryDevice>
    {
        public ProprietaryDevice(string name, IEnumerable<int> socketIDs, string serialPort)
        {
            Name = name;
            SerialPort = serialPort;
            SocketIDs = socketIDs;
        }

        public string Name { get; private set; }

        public IEnumerable<int> SocketIDs { get; private set; }

        public string SerialPort { get; private set; }

        public bool Equals(ProprietaryDevice other)
        {
            return Name == other.Name && SerialPort == other.SerialPort;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() + SerialPort.GetHashCode();
        }
    }
}
