using AnAusAutomat.Contracts.Controller;

namespace AnAusAutomat.Controllers.Probe.Internals
{
    public class ProbeDevice : IDevice
    {
        public ProbeDevice(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
