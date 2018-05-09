using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Sensor;
using Serilog;
using System.Collections.Generic;
using System.Linq;

namespace AnAusAutomat.Core.Conditions
{
    // TODO: rename. irgendwas mit status & condition service oder store.
    public class ConditionTester
    {
        //private IEnumerable<SocketStates> _states;
        private IStateStore _stateStore;
        private Dictionary<ConditionSettings, IConditionChecker> _compiledConditions;

        public ConditionTester(IStateStore stateStore, IEnumerable<ConditionSettings> conditions)
        {
            _stateStore = stateStore;
            _compiledConditions = conditions.Where(x => x.Type == ConditionType.Regular).ToDictionary(x => x, y => null as IConditionChecker); // y => null | y.GetType() = placeholder
            //_states = conditions.Select(x => x.Socket).Distinct().Select(x => new SocketStates(x)).ToList();
        }

        public void Compile()
        {
            for (int i = 0; i < _compiledConditions.Count; i++)
            {
                var key = _compiledConditions.ElementAt(i).Key;

                var compiler = new ConditionCompiler();
                var conditionChecker = compiler.Compile(key);

                _compiledConditions[key] = conditionChecker;
            }
        }

        //public void UpdatePhysicalStatus(Socket socket, PowerStatus status)
        //{
        //    var states = _states.First(x => x.Socket.Equals(socket));
        //    states.PhysicalStatus = status;
        //}

        //public void UpdateSensorStatus(Socket socket, string sensorName, PowerStatus status)
        //{
        //    var states = _states.First(x => x.Socket.Equals(socket));
        //    states.SensorStates[sensorName] = status;
        //}

        /// <summary>
        /// Returns the first true condition or null, if no condition is true.
        /// </summary>
        /// <returns></returns>
        public ConditionSettings CheckConditions(Socket socket, string affectedSensorName)
        {
            ConditionSettings result = null;

            string currentMode = _stateStore.GetCurrentMode();

            var possibleTrueConditions = _compiledConditions
                .Where(x => x.Key.Socket.Equals(socket))
                .Where(x => conditionHasCurrentModeOrNoMode(x.Key, currentMode))
                .Where(x => x.Key.Text.Contains(affectedSensorName)).ToList();
            
            if (possibleTrueConditions.Count() > 0)
            {
                var physicalState = _stateStore.GetPhysicalState(socket);
                var sensorStates = _stateStore.GetSensorStates(socket);

                var trueConditions = possibleTrueConditions
                    .Where(x => x.Value.IsTrue(physicalState, sensorStates)).ToList();

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

        private bool conditionHasCurrentModeOrNoMode(ConditionSettings condition, string currentMode)
        {
            return condition.Mode == currentMode || string.IsNullOrEmpty(condition.Mode);
        }
    }
}
