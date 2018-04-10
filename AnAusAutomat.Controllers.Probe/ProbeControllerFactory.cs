using AnAusAutomat.Contracts.Controller;
using System.Collections.Generic;

namespace AnAusAutomat.Controllers.Probe
{
    public class ProbeControllerFactory : IControllerFactory
    {
        public IEnumerable<IController> Create()
        {
            return new List<IController>() { new ProbeController() };
        }
    }
}
