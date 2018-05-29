namespace AnAusAutomat.Sensors.SoundDetector.Internals
{
    public interface ISoundSettingsProvider
    {
        bool IsMuted { get; }
        double PeakValue { get; }
        double SystemVolume { get; }
    }
}