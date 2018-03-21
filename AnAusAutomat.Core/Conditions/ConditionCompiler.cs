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
        private string _commandText;
        private string _sourceCode;

        public ConditionCompiler(string commandText)
        {
            _commandText = commandText;
        }

        public string BuildSourceCode()
        {
            Log.Debug(string.Format("Generate SourceCode for {0}", _commandText));

            //Example: Socket.IsOn AND UserInputDetector.PowerOff AND SoundDetector.PowerOff AND TrayIcon.Undefined
            string condition = _commandText.Trim().Replace("AND", "&&").Replace("OR", "||")
                .Replace("!Socket.IsOn", "states.PhysicalStatus == PowerStatus.Off")
                .Replace("!Socket.IsOff", "states.PhysicalStatus == PowerStatus.On")
                .Replace("Socket.IsOff", "states.PhysicalStatus == PowerStatus.Off")
                .Replace("Socket.IsOn", "states.PhysicalStatus == PowerStatus.On")
                .Replace(".PowerOn", " == PowerStatus.On")
                .Replace(".PowerOff", " == PowerStatus.Off")
                .Replace(".Undefined", " == PowerStatus.Undefined");

            var sensorNames = getSensorNames();
            foreach (string sensorName in sensorNames)
            {
                //condition = condition.Replace(sensorName, string.Format("states.SensorStates[\"{0}\"]", sensorName));
                condition = condition.Replace(sensorName,
                    string.Format("(states.SensorStates.ContainsKey(\"{0}\") ? states.SensorStates[\"{0}\"] : PowerStatus.Undefined)", sensorName));
            }

            string sourceCode =
                "using AnAusAutomat.Contracts.Sensor;\n" +
                "using AnAusAutomat.Core;\n" +
                "\n" +
                "public class Magic\n" +
                "{\n" +
                "  public static bool Check(SocketStates states)\n" +
                "  {\n" +
                "    return " + condition.Replace("&&", "\n      &&") + ";\n" +
                "  }\n" +
                "\n" +
                "}";

            _sourceCode = sourceCode;
            return sourceCode;
        }

        public Type Compile()
        {
            Log.Debug(string.Format("Compile SourceCode for {0}", _commandText));
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters options = new CompilerParameters();
            options.GenerateExecutable = false;
            options.GenerateInMemory = true;
            options.ReferencedAssemblies.Add("System.Core.dll");
            options.ReferencedAssemblies.Add("Contracts.dll");
            options.ReferencedAssemblies.Add("Core.dll");

            try
            {
                CompilerResults results = provider.CompileAssemblyFromSource(options, _sourceCode);
                if (results.Errors.Count > 0)
                {
                    Log.Error(string.Format("Can not compile condition: {0} ...", _commandText));
                    foreach (CompilerError error in results.Errors)
                    {
                        Log.Error(error.ErrorText);
                    }
                }

                return results.Errors.Count == 0 ? results.CompiledAssembly.DefinedTypes.FirstOrDefault() : null;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Format("Can not parse condition: {0}", _commandText));
                return null;
            }
        }

        private IEnumerable<string> getSensorNames()
        {
            string temp = _commandText
                .Replace("Socket.IsOn", "").Replace("Socket.IsOff", "")
                .Replace("AND", "").Replace("OR", "")
                .Replace("(", "").Replace(")", "")
                .Replace("&", "").Replace("|", "")
                .Replace(".", "").Replace("!", "");

            return temp.Split(new string[] { "PowerOn", "PowerOff", "Undefined" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Distinct().ToList();
        }
    }
}
