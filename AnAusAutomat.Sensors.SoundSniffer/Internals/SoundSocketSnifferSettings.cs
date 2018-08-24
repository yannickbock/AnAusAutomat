using System;

namespace AnAusAutomat.Sensors.SoundSniffer.Internals
{
    public class SoundSocketSnifferSettings
    {
        public SoundSocketSnifferSettings(TimeSpan offDelay, TimeSpan minimumSignalDuration)
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

        public static SoundSocketSnifferSettings GetDefault()
        {
            return new SoundSocketSnifferSettings(
                offDelay: TimeSpan.FromSeconds(300),
                minimumSignalDuration: TimeSpan.FromSeconds(3));
        }
    }
}
