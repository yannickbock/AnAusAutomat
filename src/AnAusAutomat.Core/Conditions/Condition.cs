using AnAusAutomat.Contracts;
using System.Collections.Generic;

namespace AnAusAutomat.Core.Conditions
{
    public class Condition : ConditionSettings, IConditionExecutor
    {
        private IConditionExecutor _conditionChecker;

        public Condition(ConditionSettings settings, IConditionExecutor conditionChecker) : base(settings.Text, settings.ResultingStatus, settings.Type, settings.Mode, settings.Socket)
        {
            _conditionChecker = conditionChecker;
        }

        public bool IsTrue(Dictionary<Socket, PowerStatus> physicalStates, Dictionary<string, PowerStatus> sensorStates)
        {
            return _conditionChecker.IsTrue(physicalStates, sensorStates);
        }
    }
}
