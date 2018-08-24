using AnAusAutomat.Contracts.Sensor.Events;

namespace AnAusAutomat.Contracts.Sensor.Features
{
    public interface IReceiveModeChanged
    {
        /// <summary>
        /// Is called if the mode has changed by an other sensor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnModeHasChanged(object sender, ModeChangedEventArgs e);
    }
}
