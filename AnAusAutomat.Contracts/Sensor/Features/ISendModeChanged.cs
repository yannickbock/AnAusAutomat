using AnAusAutomat.Contracts.Sensor.Events;
using System;

namespace AnAusAutomat.Contracts.Sensor.Features
{
    public interface ISendModeChanged
    {
        /// <summary>
        /// Need to be call if the sensor changed the mode.
        /// </summary>
        event EventHandler<ModeChangedEventArgs> ModeChanged;
    }
}
