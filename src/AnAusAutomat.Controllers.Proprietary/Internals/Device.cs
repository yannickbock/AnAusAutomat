using AnAusAutomat.Contracts.Controller;
using System.Collections.Generic;

namespace AnAusAutomat.Controllers.Proprietary.Internals
{
    public class Device : IDevice
    {
        public Device(string name, IEnumerable<int> sockets)
        {
            Name = name;
            Sockets = sockets;
        }

        public string Name { get; private set; }

        public IEnumerable<int> Sockets { get; private set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
