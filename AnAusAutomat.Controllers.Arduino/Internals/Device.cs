using System.Collections.Generic;

namespace AnAusAutomat.Controllers.Arduino.Internals
{
    public class Device
    {
        public Device(string name, IEnumerable<Pin> pins)
        {
            Name = name;
            Pins = pins;
        }

        public string Name { get; private set; }

        public IEnumerable<Pin> Pins { get; private set; }

        public override string ToString()
        {
            return string.Format("Device [ Name: {0} ]", Name);
        }
    }
}
