using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Sensor;
using AnAusAutomat.Contracts.Sensor.Attributes;
using AnAusAutomat.Contracts.Sensor.Events;
using AnAusAutomat.Contracts.Sensor.Features;
using AnAusAutomat.Sensors.SoundDetector.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Timers;

namespace AnAusAutomat.Sensors.SoundDetector
{
    [Parameter("MinimumSignalSeconds", typeof(uint), "3")]
    [Parameter("OffDelaySeconds", typeof(uint), "300")]
    [Description("Fires turn on event after MinimumSignalSeconds of music without a break." +
        "Fires turn off event after OffDelaySeconds without music.")]
    public class SoundDetector : ISensor, ISendStatusChangesIn
    {
        private ISoundSettingsProvider _soundSettings;
        private Timer _timer;
        private IEnumerable<Cache> _cache;
        private Dictionary<Socket, DateTime> _lastStatusChangesInEventsFired;

        public event EventHandler<StatusChangedEventArgs> StatusChanged;
        public event EventHandler<StatusChangesInEventArgs> StatusChangesIn;

        public void Initialize(SensorSettings settings)
        {
            _soundSettings = new SoundSettingsProvider();
            _timer = new Timer(250);
            _timer.Elapsed += _timer_Elapsed;

            _cache = settings.Sockets.Select(x => new Cache(x, parseParameters(x.Parameters))).ToList();
            _lastStatusChangesInEventsFired = new Dictionary<Socket, DateTime>();
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            bool isPlaying = isAudioPlaying();

            foreach (var cache in _cache)
            {
                bool turnSocketOn = cache.CurrentSignalSeconds > cache.Parameters.MinimumSignalSeconds;
                bool turnSocketOff = (DateTime.Now - cache.LastSignal).TotalSeconds >= cache.Parameters.OffDelaySeconds;

                if (turnSocketOff && cache.Status != PowerStatus.Off)
                {
                    turnSocketOffAndFireStatusChangedEvent(cache);
                }
                else if (turnSocketOn && cache.Status != PowerStatus.On)
                {
                    turnSocketOnAndFireStatusChangedEvent(cache);
                }

                if (isPlaying)
                {
                    cache.CurrentSignalSeconds += ((Timer)sender).Interval / 1000;
                    cache.LastSignal = DateTime.Now;
                }
                else
                {
                    cache.CurrentSignalSeconds = 0;
                }
            }
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private void turnSocketOnAndFireStatusChangedEvent(Cache cache)
        {
            cache.Status = PowerStatus.On;
            var args = convertToStatusChangedEventArgs(cache);
            StatusChanged?.Invoke(this, args);
        }

        private void turnSocketOffAndFireStatusChangedEvent(Cache cache)
        {
            cache.Status = PowerStatus.Off;
            var args = convertToStatusChangedEventArgs(cache);
            StatusChanged?.Invoke(this, args);
        }

        private StatusChangedEventArgs convertToStatusChangedEventArgs(Cache cache)
        {
            var args = new StatusChangedEventArgs(
                message: "",
                condition: "",
                socket: cache.Socket,
                status: cache.Status);

            return args;
        }

        private Parameters parseParameters(IEnumerable<SensorParameter> parameters)
        {
            uint offDelaySeconds = 300;
            uint minimumSignalSeconds = 3;

            if (parameters.Count() > 0)
            {
                bool offDelaySecondsDefined = parameters.Count(x => x.Name == "OffDelaySeconds") == 1;
                bool minimumSignalSecondsDefined = parameters.Count(x => x.Name == "MinimumSignalSeconds") == 1;

                if (offDelaySecondsDefined)
                {
                    string offDelaySecondsAsString = parameters.FirstOrDefault(x => x.Name == "OffDelaySeconds").Value;
                    offDelaySeconds = uint.Parse(offDelaySecondsAsString);
                }

                if (minimumSignalSecondsDefined)
                {
                    string minimumSignalSecondsAsString = parameters.FirstOrDefault(x => x.Name == "MinimumSignalSeconds").Value;
                    minimumSignalSeconds = uint.Parse(minimumSignalSecondsAsString);
                }
            }

            return new Parameters(offDelaySeconds, minimumSignalSeconds);
        }

        private bool isAudioPlaying()
        {
            bool isMuted = _soundSettings.IsMuted;
            bool volumeIsZero = _soundSettings.SystemVolume == 0;
            bool isPlaying = _soundSettings.PeakValue > 0;

            return !isMuted && !volumeIsZero && isPlaying;
        }
    }
}