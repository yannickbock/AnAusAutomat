using AnAusAutomat.Contracts.Sensor.Events;
using System;
using System.Collections.Generic;

namespace AnAusAutomat.Contracts.Sensor.Features
{
    public interface ISendModeChanged
    {
        /// <summary>
        /// Called before Sensor.Initialize()
        /// </summary>
        /// <param name="modes"></param>
        void InitializeModes(IEnumerable<ConditionMode> modes);

        /// <summary>
        /// Need to be call if the sensor changed the mode.
        /// </summary>
        event EventHandler<ModeChangedEventArgs> ModeChanged;
    }
}
