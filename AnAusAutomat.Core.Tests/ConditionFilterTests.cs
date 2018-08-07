using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Sensor;
using AnAusAutomat.Core.Conditions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AnAusAutomat.Core.Tests
{
    public class ConditionFilterTests
    {
        private Dictionary<Socket, PowerStatus> _physicalStates = new Dictionary<Socket, PowerStatus>()
        {
            { new Socket(1, "Bulb"), PowerStatus.Off },
            { new Socket(2, "External HDD"), PowerStatus.On }
        };

        private Socket _socketOne = new Socket(1, "Bulb");
        private Socket _socketTwo = new Socket(2, "External HDD");

        [Fact]
        public void FilterBySensor_NoConditions()
        {
            var stateStore = getStateStore(_socketOne, _socketTwo, new List<ConditionMode>());

            var filter = new ConditionFilter(stateStore, new List<Condition>());
            var result = filter.FilterBySensor(_socketOne, "SensorOne");
            Assert.Empty(result);
        }

        [Fact]
        public void FilterBySensor()
        {
            var stateStore = getStateStore(_socketOne, _socketTwo, new List<ConditionMode>());
            var conditions = getConditions(_socketOne, "SensorOne.PowerOn");

            var filter = new ConditionFilter(stateStore, conditions);
            var result = filter.FilterBySensor(_socketOne, "SensorOne");
            Assert.Single(result);
            Assert.Equal(conditions.First(), result.First());
        }

        [Fact]
        public void FilterByRelatedSocket_NoConditions()
        {
            var stateStore = getStateStore(_socketOne, _socketTwo, new List<ConditionMode>());
            var conditions = getConditions(_socketOne, "Socket(2).IsOff AND SensorOne.PowerOn");

            var filter = new ConditionFilter(stateStore, conditions);
            var result = filter.FilterByRelatedSocket(_socketOne);
            Assert.Empty(result);
        }

        [Fact]
        public void FilterByRelatedSocket()
        {
            var stateStore = getStateStore(_socketOne, _socketTwo, new List<ConditionMode>());
            var conditions = getConditions(_socketOne, "Socket(2).IsOn AND SensorFour.PowerOn");

            var filter = new ConditionFilter(stateStore, conditions);
            var result = filter.FilterByRelatedSocket(_socketTwo);
            //Assert.Single(result);
            //Assert.Contains(conditions.First(), result);

            // TODO: gars­tiges Mocking Zeug...
        }

        private IStateStore getStateStore(Socket socketOne, Socket socketTwo, IEnumerable<ConditionMode> modes)
        {
            var stateStore = new Mock<IStateStore>();
            stateStore.Setup(x => x.GetModes()).Returns(modes);
            stateStore.Setup(x => x.GetPhysicalStates()).Returns(_physicalStates);
            stateStore.Setup(x => x.GetSensorStates(socketOne)).Returns(
                new Dictionary<string, PowerStatus>()
                {
                    { "SensorOne", PowerStatus.Off },
                    { "SensorTwo", PowerStatus.On },
                    { "SensorThree", PowerStatus.On },
                    { "SensorFour", PowerStatus.Undefined },
                });
            stateStore.Setup(x => x.GetSensorStates(socketTwo)).Returns(
                new Dictionary<string, PowerStatus>()
                {
                    { "SensorOne", PowerStatus.Undefined },
                    { "SensorTwo", PowerStatus.Undefined },
                    { "SensorThree", PowerStatus.Off },
                    { "SensorFour", PowerStatus.On },
                });

            return stateStore.Object;
        }

        private IEnumerable<Condition> getConditions(Socket socket, string conditionText)
        {
            var a = new Mock<IConditionChecker>();
            a.Setup(x => x.IsTrue(_physicalStates,
                new Dictionary<string, PowerStatus>()
                {
                    { "SensorOne", PowerStatus.Off },
                    { "SensorTwo", PowerStatus.On },
                    { "SensorThree", PowerStatus.On },
                    { "SensorFour", PowerStatus.Undefined },
                })).Returns(true);

            return new List<Condition>()
            {
                new Condition(
                    new ConditionSettings(conditionText, PowerStatus.On, ConditionType.Regular, "", socket),
                    a.Object)
            };
        }
    }
}
