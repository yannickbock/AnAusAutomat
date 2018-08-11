using AnAusAutomat.Contracts.Sensor;
using AnAusAutomat.Toolbox.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnAusAutomat.Sensors.SoundSniffer.Internals
{
    public class SoundSnifferSettingsParser
    {
        private SoundSnifferSettings _defaultSettings;

        public SoundSnifferSettingsParser()
        {
            _defaultSettings = SoundSnifferSettings.GetDefault();
        }

        public SoundSnifferSettings Parse(IEnumerable<SensorParameter> parameters)
        {
            return new SoundSnifferSettings(
                offDelay: parseOffDelay(parameters),
                minimumSignalDuration: parseMinimumSignalDuration(parameters));
        }

        private TimeSpan parseOffDelay(IEnumerable<SensorParameter> parameters)
        {
            return parseTimeSpanValue(parameters, "OffDelaySeconds", _defaultSettings.OffDelay);
        }

        private TimeSpan parseMinimumSignalDuration(IEnumerable<SensorParameter> parameters)
        {
            return parseTimeSpanValue(parameters, "MinimumSignalSeconds", _defaultSettings.MinimumSignalDuration);
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
