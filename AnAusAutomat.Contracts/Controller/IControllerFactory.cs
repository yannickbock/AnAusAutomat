using System.Collections.Generic;

namespace AnAusAutomat.Contracts.Controller
{
    public interface IControllerFactory
    {
        IEnumerable<IController> Create();
    }
}
