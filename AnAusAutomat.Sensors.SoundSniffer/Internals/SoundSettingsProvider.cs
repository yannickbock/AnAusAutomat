using CSCore.CoreAudioAPI;
using System;

namespace AnAusAutomat.Sensors.SoundSniffer.Internals
{
    public class SoundSettingsProvider : ISoundSettingsProvider
    {
        private AudioEndpointVolume _volume;
        private AudioMeterInformation _meter;

        public SoundSettingsProvider()
        {
            var device = new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
            _volume = AudioEndpointVolume.FromDevice(device);
            _meter = AudioMeterInformation.FromDevice(device);
        }

        public bool IsMuted
        {
            get
            {
                return _volume.GetMute();
            }
        }

        public double SystemVolume
        {
            get
            {
                return Math.Round(_volume.GetMasterVolumeLevelScalar(), 3);
            }
        }

        public double PeakValue
        {
            get
            {
                return Math.Round(_meter.PeakValue, 3);
            }
        }
    }
}
