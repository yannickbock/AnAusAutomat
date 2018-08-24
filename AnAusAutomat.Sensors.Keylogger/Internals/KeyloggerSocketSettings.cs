using System;

namespace AnAusAutomat.Sensors.Keylogger.Internals
{
    public class KeyloggerSocketSettings
    {
        public KeyloggerSocketSettings(TimeSpan offDelay)
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

        public static KeyloggerSocketSettings GetDefault()
        {
            return new KeyloggerSocketSettings(TimeSpan.FromSeconds(300));
        }
    }
}
