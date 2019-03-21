using AnAusAutomat.Contracts.Controller;
using AnAusAutomat.Controllers.Serial.Internals;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace AnAusAutomat.Controllers.Serial
{
    public class SerialControllerFactory : IControllerFactory
    {
        public IEnumerable<IController> Create()
        {
            var xmlConfigReader = new XmlConfigReader();

            bool isValid = xmlConfigReader.Validate();

            if (!isValid)
            {
                throw new ConfigurationErrorsException("Serial controller configuration is not valid.");
            }

            var devices = xmlConfigReader.Read();

            return devices.Select(x => new SerialController(x)).ToList();
        }
    }
}
