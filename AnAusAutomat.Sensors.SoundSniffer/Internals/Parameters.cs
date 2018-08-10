namespace AnAusAutomat.Sensors.SoundSniffer.Internals
{
    public class Parameters
    {
        public Parameters(uint offDelaySeconds, uint minimumSignalSeconds)
        {
            OffDelaySeconds = offDelaySeconds;
            MinimumSignalSeconds = minimumSignalSeconds;
        }

        public uint OffDelaySeconds { get; private set; }

        public uint MinimumSignalSeconds { get; private set; }
    }
}
