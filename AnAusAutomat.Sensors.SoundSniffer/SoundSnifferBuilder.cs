using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Sensor;
using AnAusAutomat.Sensors.SoundSniffer.Internals;
using System.Collections.Generic;

namespace AnAusAutomat.Sensors.SoundSniffer
{
    public class SoundSnifferBuilder : ISensorBuilder
    {
        private SoundSnifferStateStore _stateStore;

        public SoundSnifferBuilder()
        {
            _stateStore = new SoundSnifferStateStore();
        }

        public void AddMode(ConditionMode mode)
        {
        }

        public void AddParameter(SensorParameter parameter)
        {
        }

        public void AddSocket(Socket socket, IEnumerable<SensorParameter> parameters)
        {
            var parser = new SoundSnifferSettingsParser();
            var settings = parser.ParseSocketSettings(parameters);

            _stateStore.SetSettings(socket, settings);
        }

        public ISensor Build()
        {
            return new SoundSniffer(new WindowsAudio(), _stateStore);
        }
    }
}
