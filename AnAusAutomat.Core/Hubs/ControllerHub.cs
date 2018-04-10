using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Controller;
using Serilog;
using System.Collections.Generic;

namespace AnAusAutomat.Core.Hubs
{
    // chain of responsibility pattern?
    public class ControllerHub
    {
        private IEnumerable<IController> _controllers;

        public ControllerHub(IEnumerable<IController> controllers)
        {
            _controllers = controllers;
        }

        public void Connect()
        {
            foreach (var controller in _controllers)
            {
                controller.Connect();
            }
        }

        public void Disconnect()
        {
            foreach (var controller in _controllers)
            {
                controller.Disconnect();
            }
        }

        public void TurnOn(Socket socket)
        {
            Log.Information(string.Format("Turn on {0}", socket));

            foreach (var controller in _controllers)
            {
                controller.TurnOn(socket);
            }
        }

        public void TurnOff(Socket socket)
        {
            Log.Information(string.Format("Turn off {0}", socket));

            foreach (var controller in _controllers)
            {
                controller.TurnOff(socket);
            }
        }
    }
}
