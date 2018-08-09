using System;

namespace AnAusAutomat.Sensors.Keylogger.Internals
{
    public class KeyloggerSettings
    {
        public KeyloggerSettings(TimeSpan offDelay)
        {
            OffDelay = offDelay;
        }

        public TimeSpan OffDelay { get; private set; }

        public override int GetHashCode()
        {
            return OffDelay.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return GetHashCode() == obj.GetHashCode();
        }

        public static KeyloggerSettings GetDefault()
        {
            return new KeyloggerSettings(TimeSpan.FromSeconds(300));
        }
    }
}
