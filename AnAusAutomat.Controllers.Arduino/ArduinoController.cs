using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Controller;
using AnAusAutomat.Controllers.Arduino.Internals;
using AnAusAutomat.Toolbox.Logging;
using ArduinoMajoro;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AnAusAutomat.Controllers.Arduino
{
    public class ArduinoController : IController
    {
        private IMajoro _majoro;
        private IEnumerable<Pin> _pins;

        public IDevice Device { get; private set; }

        public ArduinoController(IMajoro majoro, ControllerSettings settings)
        {
            _majoro = majoro;
            Device = settings;
            _pins = settings.Pins;
        }

        public void Connect()
        {
            _majoro.Connect();
            Thread.Sleep(100);
        }

        public void Disconnect()
        {
            _majoro.Disconnect();
        }

        public bool TurnOff(Socket socket)
        {
            bool socketIsDefined = _pins.Any(x => x.SocketID == socket.ID);

            if (socketIsDefined)
            {
                var pins = _pins.Where(x => x.SocketID == socket.ID);
                bool allSuccessful = pins.Select(x => switchPinOff(x)).Any(x => x);
                return allSuccessful;
            }

            return true;
        }

        public bool TurnOn(Socket socket)
        {
            bool socketIsDefined = _pins.Any(x => x.SocketID == socket.ID);

            if (socketIsDefined)
            {
                var pins = _pins.Where(x => x.SocketID == socket.ID);
                bool allSuccessful = pins.Select(x => switchPinOn(x)).Any(x => x);
                return allSuccessful;
            }

            return true;
        }

        private bool switchPinOn(Pin pin)
        {
            logSwitchPin(pin, pin.Logic == PinLogic.Positive);

            switch (pin.Logic)
            {
                case PinLogic.Positive:
                    return _majoro.WriteHigh(pin.Address);
                case PinLogic.Negative:
                    return _majoro.WriteLow(pin.Address);
            }

            return false;
        }

        private bool switchPinOff(Pin pin)
        {
            logSwitchPin(pin, pin.Logic == PinLogic.Negative);

            switch (pin.Logic)
            {
                case PinLogic.Positive:
                    return _majoro.WriteLow(pin.Address);
                case PinLogic.Negative:
                    return _majoro.WriteHigh(pin.Address);
            }

            return false;
        }

        private void logSwitchPin(Pin pin, bool writeHigh)
        {
            Logger.Debug(string.Format("{0} @ Arduino [ {1} ] => {2}", pin, Device, writeHigh ? "High" : "Low"));
        }
    }
}
