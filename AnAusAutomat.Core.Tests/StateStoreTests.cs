using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Sensor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AnAusAutomat.Core.Tests
{
    public class StateStoreTests
    {
        [Fact]
        public void GetPhysicalState_DefaultValue()
        {
            var store = new StateStore();
            var result = store.GetPhysicalState(new Socket(1, "Klaus-Dieter"));

            Assert.Equal(PowerStatus.Undefined, result);
        }

        [Fact]
        public void GetPhysicalState_SetAndGet()
        {
            var socket = new Socket(1, "Klaus-Dieter");

            var store = new StateStore();
            store.SetPhysicalState(socket, PowerStatus.On);
            var result = store.GetPhysicalState(socket);

            Assert.Equal(PowerStatus.On, result);
        }

        [Fact]
        public void GetSensorStates_DefaultValue()
        {
            var socket = new Socket(1, "Klaus-Dieter");

            var store = new StateStore();
            var result = store.GetSensorStates(socket);

            Assert.Equal(new Dictionary<string, PowerStatus>(), result);
        }

        [Fact]
        public void GetSensorStates_SetAndGet()
        {
            var socket = new Socket(1, "Klaus-Dieter");

            var store = new StateStore();
            store.SetSensorState(socket, "SoundDetector", PowerStatus.Off);
            var result = store.GetSensorStates(socket);

            Assert.Single(result);
            Assert.True(result.ContainsKey("SoundDetector"));
            Assert.Equal(PowerStatus.Off, result["SoundDetector"]);
        }

        [Fact]
        public void GetModes_DefaultValue()
        {
            var store = new StateStore();
            var result = store.GetModes();

            Assert.Empty(result);
        }

        [Fact]
        public void GetModes_SetAndGet()
        {
            var store = new StateStore();
            store.SetModes(new List<string>()
            {
                "Video",
                "Audio",
                "Work"
            });
            var result = store.GetModes();

            Assert.Equal(3, result.Count());
            Assert.Contains("Video", result);
            Assert.Contains("Audio", result);
            Assert.Contains("Work", result);
        }





        [Fact]
        public void GetCurrentMode_DefaultValue()
        {
            var store = new StateStore();
            string result = store.GetCurrentMode();

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetCurrentMode_SetAndGet()
        {
            var store = new StateStore();
            store.SetCurrentMode("Dings");
            string result = store.GetCurrentMode();

            Assert.Equal("Dings", result);
        }
    }
}
