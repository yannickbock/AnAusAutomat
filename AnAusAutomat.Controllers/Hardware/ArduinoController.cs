using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Controller;
using AnAusAutomat.Controllers.Exceptions;
using ArduinoMajoro;
using Serilog;
using System.Linq;
using System.Threading;

namespace AnAusAutomat.Controllers.Hardware
{
    public class ArduinoController : IController
    {
        private Majoro _majoro;

        public Device Device { get; set; }

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
            Arduino arduino = null;

            Log.Information(string.Format("Searching for {0}", Device));
            for (int i = 0; i < numberOfRetries && arduino == null; i++)
            {
                Log.Debug(string.Format("Try {0} / {1}", i + 1, numberOfRetries));
                arduino = Majoro.Hello(Device.Name);
            }

            if (arduino == null)
            {
                throwDeviceNotFoundException();
            }

            return new Majoro(arduino);
        }

        private void throwDeviceNotFoundException()
        {
            string message = "Cannot establish connection to controller. Maybe the device is not properly connected or the wire is to long.";

            Log.Error(new DeviceNotFoundException(Device), message);

            throw new DeviceNotFoundException(message, Device);
        }

        public bool TurnOff(Socket socket)
        {
            var socketWithPins = Device.Sockets.FirstOrDefault(x => x.ID == socket.ID);
            bool anyNotSuccessful = socketWithPins.Pins.Select(x => switchPinOff(x)).Any(x => !x);

            // anyNotSuccessful = true => min one error
            // anyNotSuccessful = false => no error
            return !anyNotSuccessful;
        }

        public bool TurnOn(Socket socket)
        {
            var socketWithPins = Device.Sockets.FirstOrDefault(x => x.ID == socket.ID);
            bool anyNotSuccessful = socketWithPins.Pins.Select(x => switchPinOn(x)).Any(x => !x);

            return !anyNotSuccessful;
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
