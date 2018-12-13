using AnAusAutomat.Contracts;
using AnAusAutomat.Core.Conditions;
using System.Collections.Generic;
using System.Linq;
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
                physicalStates: new Dictionary<Socket, PowerStatus>() { { new Socket(1, ""), PowerStatus.On } },
                sensorStates: new Dictionary<string, PowerStatus>()
                {
                    { "InputDeviceObserver", PowerStatus.Off }
                }));
            Assert.False(condition.IsTrue(
                physicalStates: new Dictionary<Socket, PowerStatus>() { { new Socket(1, ""), PowerStatus.Off } },
                sensorStates: new Dictionary<string, PowerStatus>()
                {
                    { "InputDeviceObserver", PowerStatus.Off }
                }));
            Assert.False(condition.IsTrue(
                physicalStates: new Dictionary<Socket, PowerStatus>() { { new Socket(1, ""), PowerStatus.On } },
                sensorStates: new Dictionary<string, PowerStatus>()
                {
                    { "InputDeviceObserver", PowerStatus.On }
                }));

            // ---
            condition = compile("!Socket.IsOn AND SoundDetector.PowerOff AND GUI.Undefined");
            Assert.True(condition.IsTrue(
                physicalStates: new Dictionary<Socket, PowerStatus>() { { new Socket(1, ""), PowerStatus.Off } },
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

        private Condition compile(string conditionText)
        {
            var settings = new ConditionSettings(conditionText, PowerStatus.Undefined, ConditionType.Regular, "", new Socket(1, ""));

            var compiler = new ConditionCompiler();
            return compiler.Compile(new List<ConditionSettings>() { settings }).FirstOrDefault();
        }
    }
}
