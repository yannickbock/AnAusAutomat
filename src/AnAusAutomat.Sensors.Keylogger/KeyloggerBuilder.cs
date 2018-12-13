using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Sensor;
using AnAusAutomat.Sensors.Keylogger.Internals;
using System;
using System.Collections.Generic;

namespace AnAusAutomat.Sensors.Keylogger
{
    public class KeyloggerBuilder : ISensorBuilder
    {
        private User32 _user32;
        private KeyloggerStateStore _stateStore;

        public KeyloggerBuilder()
        {
            _user32 = new User32();
            _stateStore = new KeyloggerStateStore();
        }

        public void AddMode(ConditionMode mode)
        {
        }

        public void AddParameter(SensorParameter parameter)
        {
        }

        public void AddSocket(Socket socket, IEnumerable<SensorParameter> parameters)
        {
            var parser = new KeyloggerSettingsParser();
            var settings = parser.ParseSocketSettings(parameters);

            _stateStore.SetSettings(socket, settings);
        }

        public ISensor Build()
        {
            return new Keylogger(_stateStore, _user32);
        }
    }
}
