using System;

namespace GUI.Internals.Dialogs
{
    internal class ComboBoxScheduleItem
    {
        internal ComboBoxScheduleItem(string text, TimeSpan countDown)
        {
            Text = text;
            CountDown = countDown;
        }

        internal string Text { get; private set; }

        internal TimeSpan CountDown { get; private set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
