using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Sensor;
using AnAusAutomat.Contracts.Sensor.Attributes;
using AnAusAutomat.Contracts.Sensor.Events;
using AnAusAutomat.Contracts.Sensor.Features;
using AnAusAutomat.Sensors.SoundSniffer.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Timers;

namespace AnAusAutomat.Sensors.SoundSniffer
{
    public class SoundSniffer : ISensor, ISendStatusForecast
    {
        private ISystemAudio _systemAudio;
        private SoundSnifferStateStore _stateStore;

        private Timer _timer;
        private Dictionary<Socket, DateTime> _lastStatusForecastEventsFired;

        public event EventHandler<StatusChangedEventArgs> StatusChanged;
        public event EventHandler<StatusForecastEventArgs> StatusForecast;

        public SoundSniffer(ISystemAudio systemAudio, SoundSnifferStateStore stateStore)
        {
            _systemAudio = systemAudio;
            _stateStore = stateStore;
            _timer = new Timer(250);
            _timer.Elapsed += _timer_Elapsed;
            _lastStatusForecastEventsFired = new Dictionary<Socket, DateTime>();
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var sockets = _stateStore.GetSockets();
            bool isPlaying = isAudioPlaying();
            var lastSignal = _stateStore.GetLastSignal();
            var signalDuration = _stateStore.GetSignalDuration();

            foreach (var socket in sockets)
            {
                var settings = _stateStore.GetSettings(socket);
                var status = _stateStore.GetStatus(socket);

                var signalIdle = DateTime.Now - lastSignal;
                bool turnSocketOn = signalDuration > settings.MinimumSignalDuration && status != PowerStatus.On;
                bool turnSocketOff = signalIdle >= settings.OffDelay && status != PowerStatus.Off;

                if (turnSocketOn)
                {
                    _stateStore.SetStatus(socket, PowerStatus.On);
                    StatusChanged?.Invoke(this, new StatusChangedEventArgs("", "", socket, PowerStatus.On));
                }
                else if (turnSocketOff)
                {
                    _stateStore.SetStatus(socket, PowerStatus.Off);
                    StatusChanged?.Invoke(this, new StatusChangedEventArgs("", "", socket, PowerStatus.Off));
                }

                fireStatusForecastEvent(socket, signalIdle, settings.OffDelay);
            }

            if (isPlaying)
            {
                _stateStore.SetSignalDuration(signalDuration + TimeSpan.FromSeconds(((Timer)sender).Interval / 1000));
                _stateStore.SetLastSignal(DateTime.Now);
            }
            else
            {
                _stateStore.SetSignalDuration(TimeSpan.FromSeconds(0));
            }
        }

        private void fireStatusForecastEvent(Socket socket, TimeSpan signalIdle, TimeSpan offDelay)
        {
            var fireAt = new int[] { 240, 180, 120, 60, 30, 15 };

            if (signalIdle.TotalSeconds > offDelay.TotalSeconds / 2)
            {
                var countdown = offDelay - signalIdle;
                int countdownInSeconds = (int)Math.Ceiling(countdown.TotalSeconds);
                bool fireEvent = fireAt.Contains(countdownInSeconds);
                var lastEventFiredAt = _lastStatusForecastEventsFired.ContainsKey(socket) ? _lastStatusForecastEventsFired[socket] : DateTime.MinValue;
                bool eventAlreadyFired = (DateTime.Now - lastEventFiredAt) <= TimeSpan.FromSeconds(1.5);

                if (fireEvent && !eventAlreadyFired)
                {
                    _lastStatusForecastEventsFired[socket] = DateTime.Now;

                    StatusForecast?.Invoke(this, new StatusForecastEventArgs("", countdown, socket, PowerStatus.Off));
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

        private bool isAudioPlaying()
        {
            bool isMuted = _systemAudio.IsMuted;
            bool volumeIsZero = _systemAudio.SystemVolume == 0;
            bool isPlaying = _systemAudio.PeakValue > 0;

            return !isMuted && !volumeIsZero && isPlaying;
        }
    }
}
