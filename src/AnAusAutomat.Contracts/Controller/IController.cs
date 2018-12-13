namespace AnAusAutomat.Contracts.Controller
{
    public interface IController
    {
        IDevice Device { get; }

        void Connect();

        void Disconnect();

        bool TurnOn(Socket socket);

        bool TurnOff(Socket socket);
    }
}
