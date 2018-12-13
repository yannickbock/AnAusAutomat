using System.IO;

namespace AnAusAutomat.Toolbox.Exceptions
{
    public class DeviceNotFoundException : IOException
    {
        public string Name { get; private set; }

        public DeviceNotFoundException(string name)
        {
            Name = name;
        }

        public DeviceNotFoundException(string name, string message) : base(message)
        {
            Name = name;
        }
    }
}
