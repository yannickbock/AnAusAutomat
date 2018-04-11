using AnAusAutomat.Contracts.Controller;
using AnAusAutomat.Controllers.Proprietary.Internals;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace AnAusAutomat.Controllers.Proprietary
{
    public class ProprietaryControllerFactory : IControllerFactory
    {
        public IEnumerable<IController> Create()
        {
            var xmlConfigReader = new XmlConfigReader();

            bool isValid = xmlConfigReader.Validate();

            if (!isValid)
            {
                throw new ConfigurationErrorsException("Proprietary controller configuration is not valid.");
            }

            var devices = xmlConfigReader.Read();

            return devices.Select(x => new ProprietaryController(x)).ToList();
        }
    }
}
