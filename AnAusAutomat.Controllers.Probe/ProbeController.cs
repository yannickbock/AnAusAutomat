using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Controller;
using Serilog;

namespace AnAusAutomat.Controllers.Probe
{
    public class ProbeController : IController
    {
        public string DeviceIdentifier { get; private set; }

        public ProbeController()
        {
            DeviceIdentifier = "n/a";
        }

        public void Connect()
        {
            Log.Debug("ProbeController: Connect()");
        }

        public void Disconnect()
        {
            Log.Debug("ProbeController: Disconnect()");
        }

        public bool TurnOff(Socket socket)
        {
            Log.Debug(string.Format("ProbeController: TurnOff({0})", socket));

            return true;
        }

        public bool TurnOn(Socket socket)
        {
            Log.Debug(string.Format("ProbeController: TurnOn({0})", socket));

            return true;
        }
    }
}
