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

        private TimeSpan parseMinimumSignalDuration(IEnumerable<SensorParameter> parameters)
        {
            int count = parameters.Count(x => x.Name == "MinimumSignalSeconds");

            if (count == 0)
            {
                Logger.Warning("MinimumSignalSeconds is not defined. Using default value.");
            }
            else if (count > 1)
            {
                Logger.Warning("MinimumSignalSeconds is more than once defined. Using default value.");
            }
            else
            {
                string valueAsString = parameters.FirstOrDefault(x => x.Name == "MinimumSignalSeconds").Value;
                bool successful = uint.TryParse(valueAsString, out uint result);

                if (successful)
                {
                    return TimeSpan.FromSeconds(result);
                }
                else
                {
                    Logger.Warning("MinimumSignalSeconds is defined, but the value is not valid. Using default value.");
                }
            }

            return _defaultSettings.MinimumSignalDuration;
        }
    }
}
