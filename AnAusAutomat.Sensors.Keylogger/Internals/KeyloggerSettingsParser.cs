using AnAusAutomat.Contracts.Sensor;
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
            return parseTimeSpanValue(parameters, "OffDelaySeconds", _defaultSettings.OffDelay);
        }

        private TimeSpan parseTimeSpanValue(IEnumerable<SensorParameter> parameters, string name, TimeSpan defaultValue)
        {
            int count = parameters.Count(x => x.Name == name);

            if (count == 0)
            {
                Logger.Warning(string.Format("{0} is not defined. Using default value.", name));
            }
            else if (count > 1)
            {
                Logger.Warning(string.Format("{0} is more than once defined. Using default value.", name));
            }
            else
            {
                string valueAsString = parameters.FirstOrDefault(x => x.Name == name).Value;
                bool successful = uint.TryParse(valueAsString, out uint result);

                if (successful)
                {
                    return TimeSpan.FromSeconds(result);
                }
                else
                {
                    Logger.Warning(string.Format("{0} is defined, but {1} is not a valid value. Using default value.", name, valueAsString));
                }
            }

            return defaultValue;
        }
    }
}
