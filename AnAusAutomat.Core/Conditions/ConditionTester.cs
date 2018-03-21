using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Sensor;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AnAusAutomat.Core.Conditions
{
    public class ConditionTester
    {
        private IEnumerable<SocketStates> _states;
        private Dictionary<Condition, Type> _compiledConditions;

        public ConditionTester(IEnumerable<Condition> conditions)
        {
            _compiledConditions = conditions.Where(x => x.Type == ConditionType.Regular).ToDictionary(x => x, y => y.GetType()); // y => null | y.GetType() = placeholder
            _states = conditions.Select(x => x.Socket).Distinct().Select(x => new SocketStates(x)).ToList();
        }

        public void Compile()
        {
            for (int i = 0; i < _compiledConditions.Count; i++)
            {
                var key = _compiledConditions.ElementAt(i).Key;

                var compiler = new ConditionCompiler(key.Command);
                compiler.BuildSourceCode();
                var method = compiler.Compile();

                _compiledConditions[key] = method;
            }
        }

        public void UpdatePhysicalStatus(Socket socket, PowerStatus status)
        {
            var states = _states.First(x => x.Socket.Equals(socket));
            states.PhysicalStatus = status;
        }

        public void UpdateSensorStatus(Socket socket, string sensorName, PowerStatus status)
        {
            var states = _states.First(x => x.Socket.Equals(socket));
            states.SensorStates[sensorName] = status;
        }

        /// <summary>
        /// Returns the first true condition or null, if no condition is true.
        /// </summary>
        /// <returns></returns>
        public Condition CheckConditions(Socket socket, string affectedSensorName, string currentMode)
        {
            Condition result = null;

            var possibleTrueConditions = _compiledConditions
                .Where(x => x.Key.Socket.Equals(socket))
                .Where(x => conditionHasCurrentModeOrNoMode(x.Key, currentMode))
                .Where(x => x.Key.Command.Contains(affectedSensorName)).ToList();
            
            if (possibleTrueConditions.Count() > 0)
            {
                var states = _states.First(x => x.Socket.Equals(socket));

                var trueConditions = possibleTrueConditions.Where(x => executeConditionCheckMethod(x.Value, states)).ToList();

                if (trueConditions.Count() > 1)
                {
                    result = trueConditions.First().Key;
                    Log.Warning("More then one condition is true. Taking the first true condition result.");
                }
                else if (trueConditions.Count() == 1)
                {
                    result = trueConditions.First().Key;
                }
            }

            return result;
        }

        private bool executeConditionCheckMethod(Type type, SocketStates states)
        {
            bool conditionIsTrue = (bool)type.InvokeMember("Check", BindingFlags.InvokeMethod, null, null, new object[] { states });
            return conditionIsTrue;
        }

        private bool conditionHasCurrentModeOrNoMode(Condition condition, string currentMode)
        {
            return condition.Mode == currentMode || string.IsNullOrEmpty(condition.Mode);
        }
    }
}
