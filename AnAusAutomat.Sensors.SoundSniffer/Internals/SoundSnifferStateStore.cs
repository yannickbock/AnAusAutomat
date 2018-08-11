using AnAusAutomat.Contracts;
using System;
using System.Collections.Generic;

namespace AnAusAutomat.Sensors.SoundSniffer.Internals
{
    public class SoundSnifferStateStore
    {
        private Dictionary<Socket, PowerStatus> _states;
        private Dictionary<Socket, SoundSnifferSettings> _settings;
        private DateTime _lastSignal;
        private TimeSpan _signalDuration;

        public SoundSnifferStateStore()
        {
            _states = new Dictionary<Socket, PowerStatus>();
            _settings = new Dictionary<Socket, SoundSnifferSettings>();
            _lastSignal = DateTime.MinValue;
        }

        public SoundSnifferSettings GetSettings(Socket socket)
        {
            return _settings.ContainsKey(socket) ? _settings[socket] : SoundSnifferSettings.GetDefault();
        }

        public void SetSettings(Socket socket, SoundSnifferSettings settings)
        {
            _settings[socket] = settings;
        }

        public PowerStatus GetStatus(Socket socket)
        {
            return _states.ContainsKey(socket) ? _states[socket] : PowerStatus.Undefined;
        }

        public void SetStatus(Socket socket, PowerStatus status)
        {
            _states[socket] = status;
        }

        public DateTime GetLastSignal()
        {
            return _lastSignal;
        }

        public void SetLastSignal(DateTime value)
        {
            _lastSignal = value;
        }

        public TimeSpan GetSignalDuration()
        {
            return _signalDuration;
        }

        public void SetSignalDuration(TimeSpan value)
        {
            _signalDuration = value;
        }
    }
}
