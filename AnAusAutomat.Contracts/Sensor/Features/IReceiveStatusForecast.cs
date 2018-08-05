using AnAusAutomat.Contracts.Sensor.Events;

namespace AnAusAutomat.Contracts.Sensor.Features
{
    public interface IReceiveStatusForecast
    {
        /// <summary>
        /// Is called if a sensor reports that his status probably will change in a specific time.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnStatusForecast(object sender, StatusForecastEventArgs e);
    }
}
