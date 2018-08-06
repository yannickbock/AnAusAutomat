using AnAusAutomat.Contracts.Sensor.Events;
using System;
using System.Collections.Generic;

namespace AnAusAutomat.Contracts.Sensor.Features
{
    public interface IReceiveModeChanged
    {
        /// <summary>
        /// Called before ISensor.Initialize()
        /// </summary>
        /// <param name="modes"></param>
        void InitializeModes(IEnumerable<ConditionMode> modes);

        /// <summary>
        /// Is called if the mode has changed by an other sensor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnModeHasChanged(object sender, ModeChangedEventArgs e);
    }
}
