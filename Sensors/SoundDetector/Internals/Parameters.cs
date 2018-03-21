namespace SoundDetector.Internals
{
    internal class Parameters
    {
        internal Parameters(uint offDelaySeconds, uint minimumSignalSeconds)
        {
            OffDelaySeconds = offDelaySeconds;
            MinimumSignalSeconds = minimumSignalSeconds;
        }

        internal uint OffDelaySeconds { get; private set; }

        internal uint MinimumSignalSeconds { get; private set; }
    }
}
