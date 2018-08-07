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
            var modes = _stateStore.GetModes();
            var activeModes = modes.Where(x => x.IsActive).Select(x => x.Name);
            var physicalStates = _stateStore.GetPhysicalStates();
            var sensorStates = _stateStore.GetSensorStates(socket);

            var trueConditions = _conditions
                .Where(x => x.Socket.Equals(socket))
                .Where(x => activeModes.Contains(x.Mode) || string.IsNullOrEmpty(x.Mode))
                //.Where(x => x.Text.Contains(sensorName))
                .Where(x => x.IsTrue(physicalStates, sensorStates))
                .ToList();

            return trueConditions;
        }
    }
}
