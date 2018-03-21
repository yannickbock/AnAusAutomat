using System;
using System.Collections.Generic;

namespace TimeClock.Internals
{
    internal class Parameters
    {
        internal Parameters(IEnumerable<DateTime> powerOn, IEnumerable<DateTime> powerOff)
        {
            PowerOn = powerOn;
            PowerOff = powerOff;
        }

        internal IEnumerable<DateTime> PowerOn { get; private set; }

        internal IEnumerable<DateTime> PowerOff { get; private set; }
    }
}
