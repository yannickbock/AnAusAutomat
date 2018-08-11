using AnAusAutomat.Contracts;
using System;
using System.Collections.Generic;

namespace AnAusAutomat.Sensors.SoundSniffer.Internals
{
    public class SoundSnifferStateStore
    {
        private Dictionary<Socket, PowerStatus> _states;
        private Dictionary<Socket, SoundSnifferSettings> _settings;

        public SoundSnifferStateStore()
        {
            _states = new Dictionary<Socket, PowerStatus>();
            _settings = new Dictionary<Socket, SoundSnifferSettings>();
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
    }
}
