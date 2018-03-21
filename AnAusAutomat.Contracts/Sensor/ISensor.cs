using AnAusAutomat.Contracts.Sensor.Events;
using System;

namespace AnAusAutomat.Contracts.Sensor
{
    public interface ISensor
    {
        void Initialize(SensorSettings settings);

        void Start();

        void Stop();

        event EventHandler<StatusChangedEventArgs> StatusChanged;
    }
}
