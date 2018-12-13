using System;
using System.Collections.Generic;

namespace AnAusAutomat.Sensors.TimeSwitch.Internals
{
    public class Parameters
    {
        public Parameters(IEnumerable<DateTime> powerOn, IEnumerable<DateTime> powerOff)
        {
            PowerOn = powerOn;
            PowerOff = powerOff;
        }

        public IEnumerable<DateTime> PowerOn { get; private set; }

        public IEnumerable<DateTime> PowerOff { get; private set; }
    }
}
