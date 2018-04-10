using AnAusAutomat.Contracts.Controller;
using AnAusAutomat.Controllers.Arduino.Internals;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace AnAusAutomat.Controllers.Arduino
{
    public class ArduinoControllerFactory : IControllerFactory
    {
        public IEnumerable<IController> Create()
        {
            var xmlConfigReader = new XmlConfigReader();

            bool isValid = xmlConfigReader.Validate();

            if (!isValid)
            {
                throw new ConfigurationErrorsException("Arduino controller configuration is not valid.");
            }

            var devices = xmlConfigReader.Read();

            return devices.Select(x => new ArduinoController(x)).ToList();
        }
    }
}
