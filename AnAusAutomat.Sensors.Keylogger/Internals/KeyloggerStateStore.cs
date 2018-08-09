using AnAusAutomat.Contracts;
using System.Collections.Generic;

namespace AnAusAutomat.Sensors.Keylogger.Internals
{
    public class KeyloggerStateStore
    {
        private Dictionary<Socket, PowerStatus> _states;
        private Dictionary<Socket, KeyloggerSettings> _settings;

        public KeyloggerStateStore()
        {
            _states = new Dictionary<Socket, PowerStatus>();
            _settings = new Dictionary<Socket, KeyloggerSettings>();
        }

        public KeyloggerSettings GetSettings(Socket socket)
        {
            return _settings.ContainsKey(socket) ? _settings[socket] : KeyloggerSettings.GetDefault();
        }

        public void SetSettings(Socket socket, KeyloggerSettings settings)
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
