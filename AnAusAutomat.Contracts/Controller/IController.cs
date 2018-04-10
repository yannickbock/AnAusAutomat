namespace AnAusAutomat.Contracts.Controller
{
    public interface IController
    {
        string DeviceIdentifier { get; }

        void Connect();

        void Disconnect();

        bool TurnOn(Socket socket);

        bool TurnOff(Socket socket);
    }
}
