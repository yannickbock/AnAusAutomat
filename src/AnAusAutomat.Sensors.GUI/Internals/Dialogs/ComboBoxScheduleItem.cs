using System;

namespace AnAusAutomat.Sensors.GUI.Internals.Dialogs
{
    public class ComboBoxScheduleItem
    {
        public ComboBoxScheduleItem(string text, TimeSpan countDown)
        {
            Text = text;
            CountDown = countDown;
        }

        public string Text { get; private set; }

        public TimeSpan CountDown { get; private set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
