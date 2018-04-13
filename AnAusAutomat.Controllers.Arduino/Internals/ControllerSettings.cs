using System.Collections.Generic;

namespace AnAusAutomat.Controllers.Arduino.Internals
{
    public class ControllerSettings
    {
        public ControllerSettings(string deviceName, IEnumerable<Pin> pins)
        {
            DeviceName = deviceName;
            Pins = pins;
        }

        public string DeviceName { get; private set; }

        public IEnumerable<Pin> Pins { get; private set; }

        public override string ToString()
        {
            return DeviceName;
        }
    }
}
