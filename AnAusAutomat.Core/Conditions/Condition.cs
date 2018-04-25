using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Sensor;

namespace AnAusAutomat.Core.Conditions
{
    public class Condition
    {
        public Condition(string text, PowerStatus resultingStatus, ConditionType type, string mode, Socket socket)
        {
            Text = text;
            ResultingStatus = resultingStatus;
            Type = type;
            Mode = mode;
            Socket = socket;
        }

        public string Text { get; private set; }

        public PowerStatus ResultingStatus { get; private set; }

        public ConditionType Type { get; private set; }

        public string Mode { get; private set; }

        public Socket Socket { get; private set; }

        public override string ToString()
        {
            return string.Format("Condition [ {0} => {1} ]", Text, ResultingStatus);
        }
    }
}
