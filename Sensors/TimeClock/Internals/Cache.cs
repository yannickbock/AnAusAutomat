using AnAusAutomat.Contracts;

namespace TimeClock.Internals
{
    internal class Cache
    {
        internal Cache(Socket socket, Parameters parameters)
        {
            Socket = socket;
            Parameters = parameters;
        }

        internal Socket Socket { get; private set; }

        internal Parameters Parameters { get; private set; }
    }
}
