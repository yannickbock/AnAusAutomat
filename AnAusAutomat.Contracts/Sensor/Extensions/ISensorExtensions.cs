﻿using AnAusAutomat.Contracts.Sensor.Features;
using AnAusAutomat.Contracts.Sensor.Metadata;
using System.Linq;

namespace AnAusAutomat.Contracts.Sensor.Extensions
{
    public static class ISensorExtensions
    {
        public static ISensorMetadata GetMetadata(this ISensor sensor)
        {
            var attribute = sensor.GetType().GetCustomAttributes(typeof(SensorMetadataAttribute), true).FirstOrDefault() as ISensorMetadata;

            return attribute;
        }

        public static SensorCapabilities GetCapabilities(this ISensor sensor)
        {
            var capabilities = SensorCapabilities.None;

            if (typeof(IReceiveModeChanged).IsInstanceOfType(sensor))
            {
                capabilities |= SensorCapabilities.ReceiveModeChanged;
            }
            if (typeof(ISendModeChanged).IsInstanceOfType(sensor))
            {
                capabilities |= SensorCapabilities.SendModeChanged;
            }
            if (typeof(IReceiveStatusChanged).IsInstanceOfType(sensor))
            {
                capabilities |= SensorCapabilities.ReceiveStatusChanged;
            }
            if (typeof(IReceiveStatusChangesIn).IsInstanceOfType(sensor))
            {
                capabilities |= SensorCapabilities.ReceiveStatusChangesIn;
            }
            if (typeof(ISendStatusChangesIn).IsInstanceOfType(sensor))
            {
                capabilities |= SensorCapabilities.SendStatusChangesIn;
            }
            if (typeof(ISendExit).IsInstanceOfType(sensor))
            {
                capabilities |= SensorCapabilities.SendExit;
            }
            if (typeof(IReceiveExit).IsInstanceOfType(sensor))
            {
                capabilities |= SensorCapabilities.ReceiveExit;
            }

            bool hasAnyCapability = capabilities > 0;
            if (hasAnyCapability)
            {
                // remove None
                capabilities &= ~SensorCapabilities.None;
            }

            return capabilities;
        }
    }
}
