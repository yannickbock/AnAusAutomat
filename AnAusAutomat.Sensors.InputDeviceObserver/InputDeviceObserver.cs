using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Sensor;
using AnAusAutomat.Contracts.Sensor.Attributes;
using AnAusAutomat.Contracts.Sensor.Events;
using AnAusAutomat.Contracts.Sensor.Features;
using AnAusAutomat.Sensors.InputDeviceObserver.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Timers;

namespace AnAusAutomat.Sensors.InputDeviceObserver
{
    [Parameter("OffDelaySeconds", typeof(uint), "300")]
    [Description("Fires turn on event if mouse or keyboard input action is detected. " +
        "Fires turn off event if time since last input has reached OffDelaySeconds.")]
    public class InputDeviceObserver : ISensor, ISendStatusForecast
    {
        private Timer _timer;
        private IEnumerable<Cache> _cache;
        private Dictionary<Socket, DateTime> _lastStatusForecastEventsFired;

        public event EventHandler<StatusChangedEventArgs> StatusChanged;
        public event EventHandler<StatusForecastEventArgs> StatusForecast;

        public void Initialize(SensorSettings settings)
        {
            _timer = new Timer(250);
            _timer.Elapsed += _timer_Elapsed;

            _cache = settings.Sockets.Select(x => new Cache(x, parseParameters(x.Parameters))).ToList();
            _lastStatusForecastEventsFired = new Dictionary<Socket, DateTime>();
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            uint inputIdleSeconds = User32Adapter.InputIdleSeconds;

            foreach (var cache in _cache)
            {
                bool turnSocketOn = inputIdleSeconds < 1;
                bool turnSocketOff = inputIdleSeconds >= cache.Parameters.OffDelaySeconds;

                if (turnSocketOn && cache.Status != PowerStatus.On)
                {
                    turnSocketOnAndFireStatusChangedEvent(cache);
                }
                else if (turnSocketOff && cache.Status != PowerStatus.Off)
                {
                    turnSocketOffAndFireStatusChangedEvent(cache);
                }

                fireStatusForecastEvent(cache, inputIdleSeconds);
            }
        }

        private void fireStatusForecastEvent(Cache cache, uint inputIdleSeconds)
        {
            double turnSocketOffCountDownInSeconds = cache.Parameters.OffDelaySeconds - inputIdleSeconds;

            bool isFiveMinuteStepAndMoreAsFiveMinutesRemain = turnSocketOffCountDownInSeconds % 300 == 0 && turnSocketOffCountDownInSeconds >= 300;

            bool fireTurnOffCountDownEvent = turnSocketOffCountDownInSeconds == 240 ||
                turnSocketOffCountDownInSeconds == 180 ||
                turnSocketOffCountDownInSeconds == 120 ||
                turnSocketOffCountDownInSeconds == 60 ||
                turnSocketOffCountDownInSeconds == 30 ||
                turnSocketOffCountDownInSeconds == 20 ||
                turnSocketOffCountDownInSeconds == 10 ||
                isFiveMinuteStepAndMoreAsFiveMinutesRemain;

            if (fireTurnOffCountDownEvent)
            {
                var args = new StatusForecastEventArgs(
                    message: "",
                    countDown: TimeSpan.FromSeconds(turnSocketOffCountDownInSeconds),
                    socket: cache.Socket,
                    status: PowerStatus.Off);

                var lastEventFiredAt = _lastStatusForecastEventsFired.ContainsKey(cache.Socket) ?
                    _lastStatusForecastEventsFired[cache.Socket] : DateTime.MinValue;
                bool currentEventAlreadyFired = (lastEventFiredAt - DateTime.Now) >= TimeSpan.FromSeconds(-1.5);
                if (!currentEventAlreadyFired)
                {
                    //StatusForecast?.Invoke(this, args);
                    _lastStatusForecastEventsFired[cache.Socket] = DateTime.Now;
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

            if (parameters.Count() > 0)
            {
                bool offDelaySecondsDefined = parameters.Count(x => x.Name == "OffDelaySeconds") == 1;

                if (offDelaySecondsDefined)
                {
                    string offDelaySecondsAsString = parameters.FirstOrDefault(x => x.Name == "OffDelaySeconds").Value;
                    offDelaySeconds = uint.Parse(offDelaySecondsAsString);
                }
            }

            return new Parameters(offDelaySeconds);
        }
    }
}

