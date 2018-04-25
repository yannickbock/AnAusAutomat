using Microsoft.CSharp;
using Serilog;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace AnAusAutomat.Core.Conditions
{
    public class ConditionCompiler
    {
        public IConditionChecker Compile(string conditionText)
        {
            string sourceCode = buildSourceCode(conditionText);

            Log.Debug(string.Format("Compile SourceCode for {0}", conditionText));
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters options = new CompilerParameters();
            options.GenerateExecutable = false;
            options.GenerateInMemory = true;
            options.ReferencedAssemblies.Add("System.Core.dll");
            options.ReferencedAssemblies.Add("Contracts.dll");
            options.ReferencedAssemblies.Add("Core.dll");
            options.ReferencedAssemblies.Add("Toolbox.dll");

            try
            {
                var results = provider.CompileAssemblyFromSource(options, sourceCode);

                if (results.Errors.Count == 0)
                {
                    return Activator.CreateInstance(results.CompiledAssembly.DefinedTypes.FirstOrDefault()) as IConditionChecker;
                }
                else
                {
                    Log.Error(string.Format("Can not compile condition: {0} ...", conditionText));
                    foreach (CompilerError error in results.Errors)
                    {
                        Log.Error(error.ErrorText);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, string.Format("Can not parse condition: {0}", conditionText));
            }

            return null;
        }

        private string buildSourceCode(string commandText)
        {
            Log.Debug(string.Format("Generate SourceCode for {0}", commandText));

            //Example: Socket.IsOn AND UserInputDetector.PowerOff AND SoundDetector.PowerOff AND TrayIcon.Undefined
            string condition = commandText.Trim().Replace("AND", "&&").Replace("OR", "||")
                .Replace("!Socket.IsOn", "physicalStatus == PowerStatus.Off")
                .Replace("!Socket.IsOff", "physicalStatus == PowerStatus.On")
                .Replace("Socket.IsOff", "physicalStatus == PowerStatus.Off")
                .Replace("Socket.IsOn", "physicalStatus == PowerStatus.On")
                .Replace(".PowerOn", " == PowerStatus.On")
                .Replace(".PowerOff", " == PowerStatus.Off")
                .Replace(".Undefined", " == PowerStatus.Undefined");

            var sensorNames = getSensorNames(commandText);
            foreach (string sensorName in sensorNames)
            {
                condition = condition.Replace(sensorName,
                    string.Format("sensorStates.GetValueOrDefault(\"{0}\", PowerStatus.Undefined)", sensorName));
            }

            string interfaceName = nameof(IConditionChecker);
            string sourceCode =
                "using AnAusAutomat.Contracts.Sensor;\n" +
                "using AnAusAutomat.Core.Conditions;\n" +
                "using AnAusAutomat.Toolbox.Extensions;\n" +
                "using System.Collections.Generic;\n" +
                "\n" +
                "public class Magic : " + interfaceName + "\n" +
                "{\n" +
                "  public bool IsTrue(PowerStatus physicalStatus, Dictionary<string, PowerStatus> sensorStates)\n" +
                "  {\n" +
                "    return " + condition.Replace("&&", "\n      &&") + ";\n" +
                "  }\n" +
                "}";

            return sourceCode;
        }

        private IEnumerable<string> getSensorNames(string commandText)
        {
            string temp = commandText
                .Replace("Socket.IsOn", "").Replace("Socket.IsOff", "")
                .Replace("AND", "").Replace("OR", "")
                .Replace("(", "").Replace(")", "")
                .Replace("&", "").Replace("|", "")
                .Replace(".", "").Replace("!", "");

            return temp.Split(new string[] { "PowerOn", "PowerOff", "Undefined" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Distinct().ToList();
        }
    }
}
