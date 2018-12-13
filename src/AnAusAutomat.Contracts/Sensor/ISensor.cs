using AnAusAutomat.Contracts.Sensor.Events;
using System;

namespace AnAusAutomat.Contracts.Sensor
{
    public interface ISensor
    {
        void Start();

        void Stop();

        event EventHandler<StatusChangedEventArgs> StatusChanged;
    }
}
