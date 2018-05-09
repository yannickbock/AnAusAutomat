using AnAusAutomat.Contracts;
using Serilog;
using System.Collections.Generic;
using System.Linq;

namespace AnAusAutomat.Core.Conditions
{
    public class ConditionTester
    {
        private IStateStore _stateStore;
        private IEnumerable<Condition> _conditions;

        public ConditionTester(IStateStore stateStore, IEnumerable<Condition> conditions)
        {
            _stateStore = stateStore;
            _conditions = conditions;
        }

        /// <summary>
        /// Returns the first true condition or null, if no condition is true.
        /// </summary>
        /// <returns></returns>
        public ConditionSettings CheckConditions(Socket socket, string affectedSensorName)
        {
            ConditionSettings result = null;

            string currentMode = _stateStore.GetCurrentMode();

            var possibleTrueConditions = _conditions
                .Where(x => x.Socket.Equals(socket))
                .Where(x => x.Mode.Equals(currentMode) || string.IsNullOrEmpty(x.Mode))
                .Where(x => x.Text.Contains(affectedSensorName))
                .ToList();

            if (possibleTrueConditions.Count() > 0)
            {
                var physicalState = _stateStore.GetPhysicalState(socket);
                var sensorStates = _stateStore.GetSensorStates(socket);

                var trueConditions = possibleTrueConditions
                    .Where(x => x.IsTrue(physicalState, sensorStates))
                    .ToList();

                if (trueConditions.Count() > 1)
                {
                    result = trueConditions.First();
                    Log.Warning("More then one condition is true. Taking the first true condition result.");
                }
                else if (trueConditions.Count() == 1)
                {
                    result = trueConditions.First();
                }
            }

            return result;
        }
    }
}
