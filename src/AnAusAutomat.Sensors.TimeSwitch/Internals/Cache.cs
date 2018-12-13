using AnAusAutomat.Contracts;

namespace AnAusAutomat.Sensors.TimeSwitch.Internals
{
    public class Cache
    {
        public Cache(Socket socket, Parameters parameters)
        {
            Socket = socket;
            Parameters = parameters;
        }

        public Socket Socket { get; private set; }

        public Parameters Parameters { get; private set; }
    }
}
