using AnAusAutomat.Contracts;
using AnAusAutomat.Sensors.Keylogger.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AnAusAutomat.Sensors.Keylogger.Tests.Internals
{
    public class KeyloggerStateStoreTests
    {
        [Fact]
        public void GetSettings_DefaultValue()
        {
            var socket = new Socket(1, "Test");
            var defaultSettings = KeyloggerSocketSettings.GetDefault();
            var stateStore = new KeyloggerStateStore();

            Assert.Equal(defaultSettings, stateStore.GetSettings(socket));
        }

        [Fact]
        public void GetSettings_SetAndGet()
        {
            var socket = new Socket(1, "Test");
            var settings = new KeyloggerSocketSettings(TimeSpan.FromSeconds(50));
            var stateStore = new KeyloggerStateStore();
            stateStore.SetSettings(socket, settings);

            Assert.Equal(settings, stateStore.GetSettings(socket));
        }

        [Fact]
        public void GetStatus_DefaultValue()
        {
            var socket = new Socket(1, "Test");
            var stateStore = new KeyloggerStateStore();

            Assert.Equal(PowerStatus.Undefined, stateStore.GetStatus(socket));
        }

        [Fact]
        public void GetStatus_SetAndGet()
        {
            var socket = new Socket(1, "Test");
            var stateStore = new KeyloggerStateStore();
            stateStore.SetStatus(socket, PowerStatus.On);

            Assert.Equal(PowerStatus.On, stateStore.GetStatus(socket));
        }

        [Fact]
        public void GetSockets()
        {
            var socket = new Socket(1, "Test");
            var settings = new KeyloggerSocketSettings(TimeSpan.FromSeconds(50));
            var stateStore = new KeyloggerStateStore();
            stateStore.SetSettings(socket, settings);

            var sockets = stateStore.GetSockets();
            Assert.Single(sockets);
            Assert.Equal(socket, sockets.First());
        }
    }
}
