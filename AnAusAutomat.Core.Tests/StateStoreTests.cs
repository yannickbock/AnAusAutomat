using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Sensor;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AnAusAutomat.Core.Tests
{
    public class StateStoreTests
    {
        [Fact]
        public void GetPhysicalState_DefaultValue()
        {
            var store = new StateStore();
            var result = store.GetPhysicalStates();

            Assert.Empty(result);
        }

        [Fact]
        public void GetPhysicalState_SetAndGet()
        {
            var socket = new Socket(1, "Klaus-Dieter");

            var store = new StateStore();
            store.SetPhysicalState(socket, PowerStatus.On);
            var result = store.GetPhysicalStates();

            Assert.Single(result);
            Assert.True(result.ContainsKey(socket));
            Assert.Equal(PowerStatus.On, result[socket]);
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
            store.SetModes(new List<ConditionMode>()
            {
                new ConditionMode("Video", false),
                new ConditionMode("Audio", true),
                new ConditionMode("Work", true)
            });
            var result = store.GetModes();

            Assert.Equal(3, result.Count());
            Assert.Contains(new ConditionMode("Video", false), result);
            Assert.Contains(new ConditionMode("Audio", true), result);
            Assert.Contains(new ConditionMode("Work", true), result);
        }

        [Fact]
        public void SetModeState()
        {
            var store = new StateStore();
            store.SetModes(new List<ConditionMode>()
            {
                new ConditionMode("Video", false),
                new ConditionMode("Audio", true),
                new ConditionMode("Work", true)
            });

            store.SetModeState(new ConditionMode("Audio", false));

            var result = store.GetModes();
            Assert.Equal(3, result.Count());
            Assert.Contains(new ConditionMode("Video", false), result);
            Assert.Contains(new ConditionMode("Audio", false), result);
            Assert.Contains(new ConditionMode("Work", true), result);
        }
    }
}
