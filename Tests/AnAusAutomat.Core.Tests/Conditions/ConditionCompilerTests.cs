using AnAusAutomat.Core.Conditions;
using System.Linq;
using Xunit;

namespace AnAusAutomat.Core.Tests.Conditions
{
    public class ConditionCompilerTests
    {
        [Theory]
        [InlineData("Socket.IsOn AND UserInputDetector.PowerOff")]
        [InlineData("Socket.IsOn AND UserInputDetector.PowerOff AND (SoundDetector.PowerOff OR SoundDetector.Undefined)")]
        [InlineData("Socket.IsOff AND UserInputDetector.PowerOn AND GUI.Undefined")]
        [InlineData("!Socket.IsOn AND UserInputDetector.PowerOn AND GUI.Undefined")]
        [InlineData("Socket.IsOn AND SoundDetector.PowerOff AND GUI.Undefined")]
        public void BuildSourceCode_Compile_Test(string conditionCommandText)
        {
            var compiler = new ConditionCompiler(conditionCommandText);
            string sourceCode = compiler.BuildSourceCode();
            var type = compiler.Compile();

            var foundMethods = type.GetMethods().Where(x => x.Name == "Check").ToList();

            Assert.NotEmpty(sourceCode);
            Assert.Single(foundMethods);
            Assert.Equal(typeof(bool), foundMethods.FirstOrDefault().ReturnParameter.ParameterType);
            Assert.Single(foundMethods.FirstOrDefault().GetParameters());
        }
    }
}
