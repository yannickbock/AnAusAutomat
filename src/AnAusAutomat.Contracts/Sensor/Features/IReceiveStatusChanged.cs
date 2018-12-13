using AnAusAutomat.Contracts.Sensor.Events;

namespace AnAusAutomat.Contracts.Sensor.Features
{
    public interface IReceiveStatusChanged
    {
        /// <summary>
        /// Called if an other sensor has changed his status.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSensorStatusHasChanged(object sender, StatusChangedEventArgs e);

        /// <summary>
        /// Called if the status of the physical socket has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPhysicalStatusHasChanged(object sender, StatusChangedEventArgs e);
    }
}
