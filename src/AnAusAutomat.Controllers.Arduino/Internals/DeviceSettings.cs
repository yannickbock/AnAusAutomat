using AnAusAutomat.Contracts.Controller;
using System.Collections.Generic;

namespace AnAusAutomat.Controllers.Arduino.Internals
{
    public class DeviceSettings : IDevice
    {
        public DeviceSettings(string name, IEnumerable<Pin> pins)
        {
            Name = name;
            Pins = pins;
        }

        public string Name { get; private set; }

        public IEnumerable<Pin> Pins { get; private set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
