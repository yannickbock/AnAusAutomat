using AnAusAutomat.Contracts.Sensor.Events;
using System;

namespace AnAusAutomat.Contracts.Sensor.Features
{
    public interface ISendExit
    {
        event EventHandler<ApplicationExitEventArgs> ApplicationExit;
    }
}
