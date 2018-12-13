using AnAusAutomat.Contracts;
using AnAusAutomat.Toolbox.Logging;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AnAusAutomat.Core.Conditions
{
    public class ConditionCompiler
    {
        public IEnumerable<Condition> Compile(IEnumerable<ConditionSettings> settings)
        {
            var conditions = settings.AsParallel()
                .Select(x => compile(x))
                .Where(x => x != null)
                .ToList();

            return conditions;
        }

        private Condition compile(ConditionSettings settings)
        {
            string sourceCode = buildSourceCode(settings.Text, settings.Socket);

            Logger.Debug(string.Format("Compile SourceCode for {0}", settings.Text));
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
                    var checker = Activator.CreateInstance(results.CompiledAssembly.DefinedTypes.FirstOrDefault()) as IConditionExecutor;

                    return new Condition(settings, checker);
                }
                else
                {
                    Logger.Error(string.Format("Can not compile condition: {0} ...", settings.Text));
                    foreach (CompilerError error in results.Errors)
                    {
                        Logger.Error(error.ErrorText);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, string.Format("Can not parse condition: {0}", settings.Text));
            }

            return null;
        }

        private string buildSourceCode(string commandText, Socket defaultSocket)
        {
            Logger.Debug(string.Format("Generate SourceCode for {0}", commandText));

            //Example: Socket.IsOn AND UserInputDetector.PowerOff AND SoundDetector.PowerOff AND TrayIcon.Undefined
            string condition = commandText.Trim().Replace("Socket.", string.Format("Socket({0}).", defaultSocket.ID));

            foreach (var x in Regex.Matches(condition, @"!Socket\(\d+\).IsOff").Cast<Match>())
            {
                condition = condition.Replace(x.Value, x.Value.Replace("!", "").Replace("IsOff", "IsOn"));
            }

            foreach (var x in Regex.Matches(condition, @"!Socket\(\d+\).IsOn").Cast<Match>())
            {
                condition = condition.Replace(x.Value, x.Value.Replace("!", "").Replace("IsOn", "IsOff"));
            }

            foreach (var x in Regex.Matches(condition, @"Socket\(\d+\)").Cast<Match>())
            {
                condition = condition.Replace(x.Value, x.Value.Replace("Socket(", "getPhysicalStateByID(physicalStates, "));
            }

            condition = condition
                .Replace("AND", "&&").Replace("OR", "||")
                .Replace(".IsOff", " == PowerStatus.Off")
                .Replace(".IsOn", " == PowerStatus.On")
                .Replace(".PowerOn", " == PowerStatus.On")
                .Replace(".PowerOff", " == PowerStatus.Off")
                .Replace(".Undefined", " == PowerStatus.Undefined");

            var sensorNames = getSensorNames(commandText);
            foreach (string sensorName in sensorNames)
            {
                condition = condition.Replace(sensorName,
                    string.Format("sensorStates.GetValueOrDefault(\"{0}\", PowerStatus.Undefined)", sensorName));
            }

            string interfaceName = nameof(IConditionExecutor);
            string sourceCode =
                "using AnAusAutomat.Contracts;\n" +
                "using AnAusAutomat.Core.Conditions;\n" +
                "using AnAusAutomat.Toolbox.Extensions;\n" +
                "using System.Collections.Generic;\n" +
                "using System.Linq;\n" +
                "\n" +
                "public class Magic : " + interfaceName + "\n" +
                "{\n" +
                "  public bool IsTrue(Dictionary<Socket, PowerStatus> physicalStates, Dictionary<string, PowerStatus> sensorStates)\n" +
                "  {\n" +
                "    return " + condition.Replace("&&", "\n      &&") + ";\n" +
                "  }\n" +
                "\n" +
                "  private PowerStatus getPhysicalStateByID(Dictionary <Socket, PowerStatus> physicalStates, int id)\n" +
                "  {\n" +
                "    return physicalStates.Any(x => x.Key.ID == id) ? physicalStates.FirstOrDefault(x => x.Key.ID == id).Value : PowerStatus.Undefined;\n" +
                "  }\n" +
                "}\n";

            return sourceCode;
        }

        private IEnumerable<string> getSensorNames(string commandText)
        {
            string temp = commandText
                .Replace("Socket.IsOn", "").Replace("Socket.IsOff", "")
                .Replace("0", "").Replace("1", "")
                .Replace("2", "").Replace("3", "")
                .Replace("4", "").Replace("5", "")
                .Replace("6", "").Replace("7", "")
                .Replace("8", "").Replace("9", "")
                .Replace("AND", "").Replace("OR", "")
                .Replace("(", "").Replace(")", "")
                .Replace("&", "").Replace("|", "")
                .Replace(".", "").Replace("!", "");

            return temp.Split(new string[] { "PowerOn", "PowerOff", "Undefined" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Distinct().ToList();
        }
    }
}
