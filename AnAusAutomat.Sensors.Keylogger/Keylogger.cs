using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Sensor;
using AnAusAutomat.Contracts.Sensor.Attributes;
using AnAusAutomat.Contracts.Sensor.Events;
using AnAusAutomat.Contracts.Sensor.Features;
using AnAusAutomat.Sensors.Keylogger.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Timers;

namespace AnAusAutomat.Sensors.Keylogger
{
    [Parameter("OffDelaySeconds", typeof(uint), "300")]
    [Description("Fires turn on event if mouse or keyboard input action is detected. " +
        "Fires turn off event if time since last input has reached OffDelaySeconds.")]
    public class Keylogger : ISensor, ISendStatusForecast
    {
        private Timer _timer;
        private IEnumerable<Socket> _sockets;
        private Dictionary<Socket, DateTime> _lastStatusForecastEventsFired;
        private User32 _user32;
        private KeyloggerStateStore _stateStore;

        public event EventHandler<StatusChangedEventArgs> StatusChanged;
        public event EventHandler<StatusForecastEventArgs> StatusForecast;

        public Keylogger()
        {
            _user32 = new User32();
            _stateStore = new KeyloggerStateStore();
            _sockets = new List<Socket>();
            _timer = new Timer(250);
            _timer.Elapsed += _timer_Elapsed;
            _lastStatusForecastEventsFired = new Dictionary<Socket, DateTime>();
        }

        public void Initialize(SensorSettings settings)
        {
            var parser = new KeyloggerSettingsParser();
            _sockets = settings.Sockets;

            foreach (var socket in settings.Sockets)
            {
                var s = parser.Parse(socket.Parameters);
                _stateStore.SetSettings(socket, s);
            }
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var inputIdle = _user32.GetInputIdle();

            foreach (var socket in _sockets)
            {
                var settings = _stateStore.GetSettings(socket);
                var status = _stateStore.GetStatus(socket);

                bool turnSocketOn = inputIdle.TotalSeconds < 1 && status != PowerStatus.On;
                bool turnSocketOff = inputIdle >= settings.OffDelay && status != PowerStatus.Off;

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

                fireStatusForecastEvent(socket, settings.OffDelay, inputIdle);
            }
        }

        private void fireStatusForecastEvent(Socket socket, TimeSpan offDelay, TimeSpan inputIdle)
        {
            var fireAt = new int[] { 240, 180, 120, 60, 30, 15 };
            
            if (inputIdle.TotalSeconds > offDelay.TotalSeconds / 2)
            {
                var countdown = offDelay - inputIdle;
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
    }
}
