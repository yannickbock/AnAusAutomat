using AnAusAutomat.Contracts.Controller;
using AnAusAutomat.Controllers.Serial.Internals;
using AnAusAutomat.Toolbox.Xml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace AnAusAutomat.Controllers.Serial
{
    public class SerialControllerFactory : IControllerFactory
    {
        public IEnumerable<IController> Create()
        {
            string currentAssemblyFilePath = new Uri(typeof(XmlConfigReader).Assembly.CodeBase).LocalPath;
            string currentAssemblyDirectoryPath = Path.GetDirectoryName(currentAssemblyFilePath);
            string schemaFilePath = currentAssemblyDirectoryPath + "\\config.xsd";
            string configFilePath = currentAssemblyDirectoryPath + "\\config.xml";

            var validator = new XmlSchemaValidator(schemaFilePath, configFilePath);
            bool isValid = validator.Validate();

            if (!isValid)
            {
                throw new ConfigurationErrorsException("Serial controller configuration is not valid.");
            }
            else
            {
                var xmlConfigReader = new XmlConfigReader(configFilePath);
                var devices = xmlConfigReader.Read();

                return devices.Select(x => new SerialController(x)).ToList();
            }
        }
    }
}
