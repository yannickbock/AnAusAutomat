using System.Collections.Generic;

namespace AnAusAutomat.Contracts.Controller
{
    public class Device
    {
        public Device(int id, string name, string type, IEnumerable<SocketWithPins> sockets)
        {
            ID = id;
            Name = name;
            Type = type;
            Sockets = sockets;
        }

        public int ID { get; private set; }

        public string Name { get; private set; }

        public string Type { get; private set; }

        public IEnumerable<SocketWithPins> Sockets { get; private set; }

        public override string ToString()
        {
            return string.Format("Device [ ID: {0} | Name: {1} | Type: {2} ]", ID, Name, Type);
        }
    }
}
