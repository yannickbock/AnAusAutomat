﻿using AnAusAutomat.Contracts.Sensor.Events;
using System;

namespace AnAusAutomat.Contracts.Sensor.Features
{
    public interface ISendStatusChangesIn
    {
        /// <summary>
        /// Need to be called if the the sensor will change his status in a specific time.
        /// </summary>
        event EventHandler<StatusChangesInEventArgs> StatusChangesIn;
    }
}
