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
            var defaultSettings = SoundSnifferSettings.GetDefault();
            var stateStore = new SoundSnifferStateStore();

            Assert.Equal(defaultSettings, stateStore.GetSettings(socket));
        }

        [Fact]
        public void GetSettings_SetAndGet()
        {
            var socket = new Socket(1, "Test");
            var settings = new SoundSnifferSettings(TimeSpan.FromSeconds(50), TimeSpan.FromSeconds(7));
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
    }
}
