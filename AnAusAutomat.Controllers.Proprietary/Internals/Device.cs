using System.Collections.Generic;

namespace AnAusAutomat.Controllers.Proprietary.Internals
{
    public class Device
    {
        public Device(string name, IEnumerable<int> sockets)
        {
            Name = name;
            Sockets = sockets;
        }

        public string Name { get; private set; }

        public IEnumerable<int> Sockets { get; private set; }
    }
}
