using AnAusAutomat.Contracts;
using Serilog;
using System.Collections.Generic;
using System.Linq;

namespace AnAusAutomat.Core.Conditions
{
    public class ConditionFilter
    {
        private IStateStore _stateStore;
        private IEnumerable<Condition> _conditions;

        public ConditionFilter(IStateStore stateStore, IEnumerable<Condition> conditions)
        {
            _stateStore = stateStore;
            _conditions = conditions;
        }

        public IEnumerable<Condition> Filter(Socket socket, string sensorName)
        {
            string currentMode = _stateStore.GetCurrentMode();
            var physicalState = _stateStore.GetPhysicalState(socket);
            var sensorStates = _stateStore.GetSensorStates(socket);

            var trueConditions = _conditions
                .Where(x => x.Socket.Equals(socket))
                .Where(x => x.Mode.Equals(currentMode) || string.IsNullOrEmpty(x.Mode))
                .Where(x => x.Text.Contains(sensorName))
                .Where(x => x.IsTrue(physicalState, sensorStates))
                .ToList();

            return trueConditions;
        }
    }
}
