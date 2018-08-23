using AnAusAutomat.Contracts.Controller;
using System.Collections.Generic;

namespace AnAusAutomat.Controllers.Arduino.Internals
{
    public class ControllerSettings : IDevice
    {
        public ControllerSettings(string deviceName, IEnumerable<Pin> pins)
        {
            Name = deviceName;
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
