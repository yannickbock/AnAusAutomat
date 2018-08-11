namespace AnAusAutomat.Sensors.SoundSniffer.Internals
{
    public interface ISystemAudio
    {
        bool IsMuted { get; }
        double PeakValue { get; }
        double SystemVolume { get; }
    }
}