namespace AnAusAutomat.Sensors.SoundSniffer.Internals
{
    public interface ISoundSettingsProvider
    {
        bool IsMuted { get; }
        double PeakValue { get; }
        double SystemVolume { get; }
    }
}