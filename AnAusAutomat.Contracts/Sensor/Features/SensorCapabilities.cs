using System;

namespace AnAusAutomat.Contracts.Sensor.Features
{
    [Flags]
    public enum SensorCapabilities
    {
        None = 0,

        ReceiveModeChanged = 1,
        SendModeChanged = 2,

        ReceiveStatusChangesIn = 4,
        SendStatusChangesIn = 8,

        ReceiveStatusChanged = 16,

        SendExit = 32,
        ReceiveExit = 64
    }
}
