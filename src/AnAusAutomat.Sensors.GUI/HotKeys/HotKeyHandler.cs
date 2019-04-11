using AnAusAutomat.Contracts;
using AnAusAutomat.Toolbox.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnAusAutomat.Sensors.GUI.HotKeys
{
    public class HotKeyHandler : IHotKeyHandler
    {
        private IHotKeyNotifier _hotKeyNotifier;
        private HotKeySettings _settings;
        private List<int> _ids;

        private Socket _socket;
        private PowerStatus? _status;
        private DateTime _lastKeyPressed = DateTime.MinValue;

        public event EventHandler<HotKeyPressedEventArgs> HotKeyPressed;

        public HotKeyHandler(IHotKeyNotifier hotKeyNotifier, HotKeySettings settings)
        {
            _hotKeyNotifier = hotKeyNotifier;
            _settings = settings;
            _hotKeyNotifier.HotKeyPressed += hotKeyPressed;
        }

        private void hotKeyPressed(object sender, HotKeyEventArgs e)
        {
            var hotKey = new HotKey(e.Modifiers, e.Key);
            bool timedOut = DateTime.Now - _lastKeyPressed > TimeSpan.FromSeconds(10);

            if (timedOut)
            {
                _status = null;
                _socket = null;
            }

            if (_settings.PowerOn.Equals(hotKey))
            {
                _status = PowerStatus.On;
                Logger.Debug(string.Format("PowerOn HotKey {0} + {1} pressed.", hotKey.Modifier, hotKey.Key));
            }
            else if (_settings.PowerOff.Equals(hotKey))
            {
                _status = PowerStatus.Off;
                Logger.Debug(string.Format("PowerOff HotKey {0} + {1} pressed.", hotKey.Modifier, hotKey.Key));
            }
            else if (_settings.Undefined.Equals(hotKey))
            {
                _status = PowerStatus.Undefined;
                Logger.Debug(string.Format("Undefined HotKey {0} + {1} pressed.", hotKey.Modifier, hotKey.Key));
            }

            if (_settings.Sockets.ContainsValue(hotKey))
            {
                _socket = _settings.Sockets.FirstOrDefault(x => x.Value.Equals(hotKey)).Key;
                Logger.Debug(string.Format("Socket HotKey {0} + {1} ({2}) pressed.", hotKey.Modifier, hotKey.Key, _socket));
            }

            bool statusAndSocketDefined = _status != null && _socket != null;
            if (statusAndSocketDefined)
            {
                HotKeyPressed?.Invoke(this, new HotKeyPressedEventArgs(_socket, (PowerStatus)_status));
                _lastKeyPressed = DateTime.MinValue;
            }

            _lastKeyPressed = DateTime.Now;
        }

        public void Start()
        {
            _ids = new List<int>();

            if (_settings.PowerOn != null)
            {
                _ids.Add(_hotKeyNotifier.RegisterHotKey(_settings.PowerOn.Modifier, _settings.PowerOn.Key));
            }
            if (_settings.PowerOff != null)
            {
                _ids.Add(_hotKeyNotifier.RegisterHotKey(_settings.PowerOff.Modifier, _settings.PowerOff.Key));
            }
            if (_settings.Undefined != null)
            {
                _ids.Add(_hotKeyNotifier.RegisterHotKey(_settings.Undefined.Modifier, _settings.Undefined.Key));
            }

            _ids.AddRange(_settings.Sockets.Where(x => x.Value != null)
                .Select(x =>
                {
                    return _hotKeyNotifier.RegisterHotKey(x.Value.Modifier, x.Value.Key);
                }));
        }

        public void Stop()
        {
            foreach (int id in _ids)
            {
                _hotKeyNotifier.UnregisterHotKey(id);
            }
            _ids = new List<int>();
        }
    }
}
