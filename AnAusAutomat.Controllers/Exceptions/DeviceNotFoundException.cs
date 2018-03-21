using AnAusAutomat.Contracts.Controller;
using System.IO;

namespace AnAusAutomat.Controllers.Exceptions
{
    public class DeviceNotFoundException : IOException
    {
        public DeviceNotFoundException() : base()
        {
        }

        public DeviceNotFoundException(Device device) : base()
        {
            Device = device;
        }

        public DeviceNotFoundException(string message, Device device) : base(message)
        {
            Device = device;
        }

        public Device Device { get; private set; }
    }
}
