using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Controller;
using AnAusAutomat.Controllers.Probe.Internals;
using AnAusAutomat.Toolbox.Logging;

namespace AnAusAutomat.Controllers.Probe
{
    public class ProbeController : IController
    {
        public IDevice Device { get; private set; }

        public ProbeController()
        {
            Device = new ProbeDevice("n/a");
        }

        public void Connect()
        {
            Logger.Debug("ProbeController: Connect()");
        }

        public void Disconnect()
        {
            Logger.Debug("ProbeController: Disconnect()");
        }

        public bool TurnOff(Socket socket)
        {
            Logger.Debug(string.Format("ProbeController: TurnOff({0})", socket));

            return true;
        }

        public bool TurnOn(Socket socket)
        {
            Logger.Debug(string.Format("ProbeController: TurnOn({0})", socket));

            return true;
        }
    }
}
