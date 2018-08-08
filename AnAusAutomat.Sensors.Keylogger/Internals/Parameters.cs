namespace AnAusAutomat.Sensors.Keylogger.Internals
{
    public class Parameters
    {
        public Parameters(uint offDelaySeconds)
        {
            OffDelaySeconds = offDelaySeconds;
        }

        public uint OffDelaySeconds { get; private set; }
    }
}
