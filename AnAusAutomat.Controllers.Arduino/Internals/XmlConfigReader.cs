using AnAusAutomat.Toolbox.Xml;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace AnAusAutomat.Controllers.Arduino.Internals
{
    public class XmlConfigReader
    {
        private string _schemaFilePath;
        private string _configFilePath;
        private XmlSchemaValidator _xmlSchemaValidator;
        private XDocument _xDocument;

        public XmlConfigReader()
        {
            string currentAssemblyFilePath = new Uri(typeof(XmlConfigReader).Assembly.CodeBase).LocalPath;
            string currentAssemblyDirectoryPath = Path.GetDirectoryName(currentAssemblyFilePath);

            _schemaFilePath = currentAssemblyDirectoryPath + "\\config.xsd";
            _configFilePath = currentAssemblyDirectoryPath + "\\config.xml";

            _xmlSchemaValidator = new XmlSchemaValidator(_schemaFilePath, _configFilePath)
            {
                SchemaNotValidLogMessage = string.Format("Arduino controller schema file {0} is corrupted.", _schemaFilePath),
                ConfigNotValidLogMessage = string.Format("Arduino controller config file {0} is not valid.", _configFilePath),
                SchemaFileNotFoundLogMessage = string.Format("Arduino controller schema file {0} does not exist.", _schemaFilePath),
                ConfigFileNotFoundLogMessage = string.Format("Arduino controller config file {0} does not exist.", _configFilePath)
            };
        }

        public bool Validate()
        {
            return _xmlSchemaValidator.Validate();
        }

        public IEnumerable<Device> Read()
        {
            Log.Information("Loading arduino controller settings ...");
            _xDocument = XDocument.Load(_configFilePath);

            return readDevices();
        }

        private IEnumerable<Device> readDevices()
        {
            return _xDocument.Root.Element("devices").Elements("device").Select(deviceNode =>
            {
                return new Device(
                    name: deviceNode.Attribute("name").Value,
                    pins: readPins(deviceNode));
            }).ToList();
        }

        private IEnumerable<Pin> readPins(XElement deviceNode)
        {
            return deviceNode.Elements("pin").Select(pinNode =>
            {
                return new Pin(
                    socketID: int.Parse(pinNode.Attribute("socketId").Value),
                    address: int.Parse(pinNode.Value),
                    name: pinNode.Attribute("name").Value,
                    logic: convertStringToPinLogic(pinNode.Attribute("logic").Value));
            }).ToList();
        }

        private PinLogic convertStringToPinLogic(string value)
        {
            return value.ToLower() == "negative" ? PinLogic.Negative : PinLogic.Positive;
        }
    }
}
