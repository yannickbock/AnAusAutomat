using AnAusAutomat.Contracts.Sensor.Events;

namespace AnAusAutomat.Contracts.Sensor.Features
{
    public interface IReceiveExit
    {
        void OnApplicationExit(object sender, ApplicationExitEventArgs e);
    }
}
