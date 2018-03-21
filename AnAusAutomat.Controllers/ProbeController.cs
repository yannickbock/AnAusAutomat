using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Controller;
using Serilog;
using System.Linq;

namespace AnAusAutomat.Controllers
{
    public class ProbeController : IController
    {
        public Device Device { get; set; }

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

            var socketWithPins = Device.Sockets.FirstOrDefault(x => x.ID == socket.ID);
            foreach (var pin in socketWithPins.Pins)
            {
                Log.Verbose(string.Format("ProbeController: TurnOff({0}) | ", pin));
            }

            return true;
        }

        public bool TurnOn(Socket socket)
        {
            Log.Debug(string.Format("ProbeController: TurnOn({0})", socket));

            var socketWithPins = Device.Sockets.FirstOrDefault(x => x.ID == socket.ID);
            foreach (var pin in socketWithPins.Pins)
            {
                Log.Verbose(string.Format("ProbeController: TurnOn({0}) | ", pin));
            }

            return true;
        }
    }
}
