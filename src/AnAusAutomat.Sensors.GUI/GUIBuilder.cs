using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Sensor;
using AnAusAutomat.Sensors.GUI.HotKeys;
using AnAusAutomat.Sensors.GUI.Scheduling;
using AnAusAutomat.Sensors.GUI.TrayIcon;
using System.Collections.Generic;

namespace AnAusAutomat.Sensors.GUI
{
    public class GUIBuilder : ISensorBuilder
    {
        private List<ConditionMode> _modes;
        private List<SensorParameter> _parameters;
        private Dictionary<Socket, IEnumerable<SensorParameter>> _sockets;

        public GUIBuilder()
        {
            _modes = new List<ConditionMode>();
            _parameters = new List<SensorParameter>();
            _sockets = new Dictionary<Socket, IEnumerable<SensorParameter>>();
        }

        public void AddMode(ConditionMode mode)
        {
            _modes.Add(mode);
        }

        public void AddParameter(SensorParameter parameter)
        {
            _parameters.Add(parameter);
        }

        public void AddSocket(Socket socket, IEnumerable<SensorParameter> parameters)
        {
            _sockets.Add(socket, parameters);
        }

        public ISensor Build()
        {
            var parser = new SettingsParser(_parameters, _sockets);
            var settings = parser.Parse();

            var translation = new Translation();
            var scheduler = new Scheduler();

            var builder = new TrayIconMenuBuilder(translation);
            foreach (var socket in _sockets)
            {
                builder.AddSocketStrip(socket.Key);
            }
            builder.AddSeparatorStrip();
            builder.AddModeStrip(_modes);
            builder.AddSeparatorStrip();
            builder.AddExitStrip();
            var trayIcon = builder.Build();

            var hotKeyHandler = new HotKeyHandler(new HotKeyNotifier(), settings.HotKeys);

            return new GUI(translation, scheduler, trayIcon, hotKeyHandler);
        }
    }
}
