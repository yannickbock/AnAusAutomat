using System;

namespace AnAusAutomat.Sensors.SoundSniffer.Internals
{
    public class SoundSnifferSettings
    {
        public SoundSnifferSettings(TimeSpan offDelay, TimeSpan minimumSignalDuration)
        {
            OffDelay = offDelay;
            MinimumSignalDuration = minimumSignalDuration;
        }

        public TimeSpan OffDelay { get; private set; }

        public TimeSpan MinimumSignalDuration { get; private set; }

        public override int GetHashCode()
        {
            return (OffDelay.GetHashCode() + MinimumSignalDuration.GetHashCode()) / 2;
        }

        public override bool Equals(object obj)
        {
            return GetHashCode() == obj.GetHashCode();
        }

        public static SoundSnifferSettings GetDefault()
        {
            return new SoundSnifferSettings(
                offDelay: TimeSpan.FromSeconds(300),
                minimumSignalDuration: TimeSpan.FromSeconds(3));
        }
    }
}
