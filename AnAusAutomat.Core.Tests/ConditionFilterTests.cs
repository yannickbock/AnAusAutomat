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
        [Fact]
        public void Filter_NoConditions()
        {
            var socketOne = new Socket(1, "Bulb");
            var socketTwo = new Socket(2, "External HDD");
            var stateStore = getStateStore(socketOne, socketTwo, new List<ConditionMode>());

            var filter = new ConditionFilter(stateStore, new List<Condition>());
            var result = filter.Filter(socketOne, "SensorOne");
            Assert.Empty(result);
        }

        [Fact]
        public void Filter()
        {
            var socketOne = new Socket(1, "Bulb");
            var socketTwo = new Socket(2, "External HDD");
            var stateStore = getStateStore(socketOne, socketTwo, new List<ConditionMode>());
            var conditions = getConditions(socketOne);

            var filter = new ConditionFilter(stateStore, conditions);
            var result = filter.Filter(socketOne, "SensorOne");
            Assert.Single(result);
            Assert.Equal(conditions.First(), result.First());
        }

        private IStateStore getStateStore(Socket socketOne, Socket socketTwo, IEnumerable<ConditionMode> modes)
        {
            var stateStore = new Mock<IStateStore>();
            stateStore.Setup(x => x.GetModes()).Returns(modes);
            stateStore.Setup(x => x.GetPhysicalState(socketOne)).Returns(PowerStatus.Off);
            stateStore.Setup(x => x.GetPhysicalState(socketTwo)).Returns(PowerStatus.On);
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

        private IEnumerable<Condition> getConditions(Socket socket)
        {
            var a = new Mock<IConditionChecker>();
            a.Setup(x => x.IsTrue(PowerStatus.Off, new Dictionary<string, PowerStatus>()
                {
                    { "SensorOne", PowerStatus.Off },
                    { "SensorTwo", PowerStatus.On },
                    { "SensorThree", PowerStatus.On },
                    { "SensorFour", PowerStatus.Undefined },
                })).Returns(true);

            return new List<Condition>()
            {
                new Condition(
                    new ConditionSettings("SensorOne.PowerOn", PowerStatus.On, ConditionType.Regular, "", socket),
                    a.Object)
            };
        }
    }
}
