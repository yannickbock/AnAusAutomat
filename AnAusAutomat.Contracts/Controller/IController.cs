namespace AnAusAutomat.Contracts.Controller
{
    public interface IController
    {
        Device Device { get; set; }

        void Connect();

        void Disconnect();

        bool TurnOn(Socket socket);

        bool TurnOff(Socket socket);
    }
}
