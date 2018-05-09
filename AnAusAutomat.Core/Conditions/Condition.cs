using AnAusAutomat.Contracts.Sensor;
using System.Collections.Generic;

namespace AnAusAutomat.Core.Conditions
{
    public class Condition : ConditionSettings, IConditionChecker
    {
        private IConditionChecker _conditionChecker;

        public Condition(ConditionSettings settings, IConditionChecker conditionChecker) : base(settings.Text, settings.ResultingStatus, settings.Type, settings.Mode, settings.Socket)
        {
            _conditionChecker = conditionChecker;
        }

        public bool IsTrue(PowerStatus physicalStatus, Dictionary<string, PowerStatus> sensorStates)
        {
            return _conditionChecker.IsTrue(physicalStatus, sensorStates);
        }
    }
}
