using AnAusAutomat.Contracts.Sensor;
using AnAusAutomat.Core.Conditions;
using System.Collections.Generic;
using Xunit;

namespace AnAusAutomat.Core.Tests.Conditions
{
    public class ConditionCompilerTests
    {
        [Fact]
        public void Compile()
        {
            var condition = compile("Socket.IsOn AND InputDeviceObserver.PowerOff");
            Assert.True(condition.IsTrue(
                physicalStatus: PowerStatus.On,
                sensorStates: new Dictionary<string, PowerStatus>()
                {
                    { "InputDeviceObserver", PowerStatus.Off }
                }));
            Assert.False(condition.IsTrue(
                physicalStatus: PowerStatus.Off,
                sensorStates: new Dictionary<string, PowerStatus>()
                {
                    { "InputDeviceObserver", PowerStatus.Off }
                }));
            Assert.False(condition.IsTrue(
                physicalStatus: PowerStatus.On,
                sensorStates: new Dictionary<string, PowerStatus>()
                {
                    { "InputDeviceObserver", PowerStatus.On }
                }));

            // ---
            condition = compile("!Socket.IsOn AND SoundDetector.PowerOff AND GUI.Undefined");
            Assert.True(condition.IsTrue(
                physicalStatus: PowerStatus.Off,
                sensorStates: new Dictionary<string, PowerStatus>()
                {
                    { "SoundDetector", PowerStatus.Off },
                    { "GUI", PowerStatus.Undefined }
                }));
        }

        [Fact]
        public void Compile_Corrupted()
        {
            var condition = compile("Socket.IsOn == InputDeviceObserver.PowerOff");
            Assert.Null(condition);
        }

        private IConditionChecker compile(string conditionText)
        {
            var compiler = new ConditionCompiler();
            return compiler.Compile(conditionText);
        }
    }
}
