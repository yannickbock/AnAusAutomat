namespace UserInputDetector.Internals
{
    internal class Parameters
    {
        internal Parameters(uint offDelaySeconds)
        {
            OffDelaySeconds = offDelaySeconds;
        }

        internal uint OffDelaySeconds { get; private set; }
    }
}
