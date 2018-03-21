using System.Collections.Generic;

namespace AnAusAutomat.Contracts.Controller
{
    public class SocketWithPins : Socket
    {
        public SocketWithPins(int id, string name, IEnumerable<Pin> pins) : base(id, name)
        {
            Pins = pins;
        }

        public IEnumerable<Pin> Pins { get; private set; }
    }
}
