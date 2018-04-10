using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Controller;
using AnAusAutomat.Controllers.Arduino.Internals;
using ArduinoMajoro;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ArduinoDevice = ArduinoMajoro.Arduino;

namespace AnAusAutomat.Controllers.Arduino
{
    public class ArduinoController : IController
    {
        private Majoro _majoro;
        private IEnumerable<Pin> _pins;

        public string DeviceIdentifier { get; private set; }

        public ArduinoController(Device device)
        {
            DeviceIdentifier = device.Name;
            _pins = device.Pins;
        }

        public void Connect()
        {
            _majoro = findConnection();
            _majoro.Connect();

            Thread.Sleep(100);
        }

        public void Disconnect()
        {
            _majoro.Disconnect();
        }

        private Majoro findConnection()
        {
            int numberOfRetries = 10;
            ArduinoDevice arduino = null;

            Log.Information(string.Format("Searching for {0}", DeviceIdentifier));
            for (int i = 0; i < numberOfRetries && arduino == null; i++)
            {
                Log.Debug(string.Format("Try {0} / {1}", i + 1, numberOfRetries));
                arduino = Majoro.Hello(DeviceIdentifier);
            }

            if (arduino == null)
            {
                throwDeviceNotFoundException(DeviceIdentifier);
            }

            return new Majoro(arduino);
        }

        private void throwDeviceNotFoundException(string name)
        {
            string message = "Cannot establish connection to controller. " +
                "Maybe the device is not properly connected, the wire is to long or the wrong sketch is uploaded.";
            var exception = new DeviceNotFoundException(name, message);

            Log.Error(exception, message);
            throw exception;
        }

        public bool TurnOff(Socket socket)
        {
            var pins = _pins.Where(x => x.SocketID == socket.ID);
            bool allSuccessful = pins.Select(x => switchPinOff(x)).Any(x => x);

            return allSuccessful;
        }

        public bool TurnOn(Socket socket)
        {
            var pins = _pins.Where(x => x.SocketID == socket.ID);
            bool allSuccessful = pins.Select(x => switchPinOn(x)).Any(x => x);

            return allSuccessful;
        }

        private bool switchPinOn(Pin pin)
        {
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
            switch (pin.Logic)
            {
                case PinLogic.Positive:
                    return _majoro.WriteLow(pin.Address);
                case PinLogic.Negative:
                    return _majoro.WriteHigh(pin.Address);
            }

            return false;
        }
    }
}
