﻿using AnAusAutomat.Contracts.Sensor;
using AnAusAutomat.Toolbox.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnAusAutomat.Sensors.Keylogger.Internals
{
    public class KeyloggerSettingsParser
    {
        private KeyloggerSettings _defaultSettings;

        public KeyloggerSettingsParser()
        {
            _defaultSettings = KeyloggerSettings.GetDefault();
        }

        public KeyloggerSettings Parse(IEnumerable<SensorParameter> parameters)
        {
            return new KeyloggerSettings(
                offDelay: parseOffDelay(parameters));
        }

        private TimeSpan parseOffDelay(IEnumerable<SensorParameter> parameters)
        {
            int count = parameters.Count(x => x.Name == "OffDelaySeconds");

            if (count == 0)
            {
                Logger.Warning("OffDelaySeconds is not defined. Using default value.");
            }
            else if (count > 1)
            {
                Logger.Warning("OffDelaySeconds is more than once defined. Using default value.");
            }
            else
            {
                string valueAsString = parameters.FirstOrDefault(x => x.Name == "OffDelaySeconds").Value;
                bool successful = uint.TryParse(valueAsString, out uint result);

                if (successful)
                {
                    return TimeSpan.FromSeconds(result);
                }
                else
                {
                    Logger.Warning("OffDelaySeconds is defined, but the value is not valid. Using default value.");
                }
            }

            return _defaultSettings.OffDelay;
        }
    }
}
