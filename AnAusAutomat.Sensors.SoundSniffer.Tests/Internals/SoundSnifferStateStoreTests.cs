using AnAusAutomat.Contracts;
using AnAusAutomat.Sensors.SoundSniffer.Internals;
using System;
using Xunit;

namespace AnAusAutomat.Sensors.SoundSniffer.Tests.Internals
{
    public class SoundSnifferStateStoreTests
    {
        [Fact]
        public void GetSettings_DefaultValue()
        {
            var socket = new Socket(1, "Test");
            var defaultSettings = SoundSocketSnifferSettings.GetDefault();
            var stateStore = new SoundSnifferStateStore();

            Assert.Equal(defaultSettings, stateStore.GetSettings(socket));
        }

        [Fact]
        public void GetSettings_SetAndGet()
        {
            var socket = new Socket(1, "Test");
            var settings = new SoundSocketSnifferSettings(TimeSpan.FromSeconds(50), TimeSpan.FromSeconds(7));
            var stateStore = new SoundSnifferStateStore();
            stateStore.SetSettings(socket, settings);

            Assert.Equal(settings, stateStore.GetSettings(socket));
        }

        [Fact]
        public void GetStatus_DefaultValue()
        {
            var socket = new Socket(1, "Test");
            var stateStore = new SoundSnifferStateStore();

            Assert.Equal(PowerStatus.Undefined, stateStore.GetStatus(socket));
        }

        [Fact]
        public void GetStatus_SetAndGet()
        {
            var socket = new Socket(1, "Test");
            var stateStore = new SoundSnifferStateStore();
            stateStore.SetStatus(socket, PowerStatus.On);

            Assert.Equal(PowerStatus.On, stateStore.GetStatus(socket));
        }

        [Fact]
        public void GetLastSignal_DefaultValue()
        {
            var stateStore = new SoundSnifferStateStore();

            Assert.Equal(DateTime.MinValue, stateStore.GetLastSignal());
        }

        [Fact]
        public void GetLastSignal_SetAndGet()
        {
            var lastSignal = new DateTime(2018, 1, 1, 15, 15, 30);
            var stateStore = new SoundSnifferStateStore();
            stateStore.SetLastSignal(lastSignal);

            Assert.Equal(lastSignal, stateStore.GetLastSignal());
        }

        [Fact]
        public void GetSignalDuration_DefaultValue()
        {
            var stateStore = new SoundSnifferStateStore();

            Assert.Equal(TimeSpan.FromSeconds(0), stateStore.GetSignalDuration());
        }

        [Fact]
        public void GetSignalDuration_SetAndGet()
        {
            var signalDuration = TimeSpan.FromMilliseconds(3500);
            var stateStore = new SoundSnifferStateStore();
            stateStore.SetSignalDuration(signalDuration);

            Assert.Equal(signalDuration, stateStore.GetSignalDuration());
        }
    }
}
