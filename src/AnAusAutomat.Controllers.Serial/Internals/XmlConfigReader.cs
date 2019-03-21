using AnAusAutomat.Toolbox.Logging;
using AnAusAutomat.Toolbox.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace AnAusAutomat.Controllers.Serial.Internals
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

            _xmlSchemaValidator = new XmlSchemaValidator(_schemaFilePath, _configFilePath);
        }

        public bool Validate()
        {
            return _xmlSchemaValidator.Validate(out string message);
        }

        public IEnumerable<DeviceSettings> Read()
        {
            Logger.Information("Loading proprietary controller settings ...");
            _xDocument = XDocument.Load(_configFilePath);

            return readDeviceSettings();
        }

        private IEnumerable<DeviceSettings> readDeviceSettings()
        {
            return _xDocument.Root.Element("devices").Elements("device").Select(deviceNode =>
            {
                return new DeviceSettings(
                    name: deviceNode.Attribute("name").Value,
                    mapping: readPins(deviceNode));
            }).ToList();
        }

        private Dictionary<int, int> readPins(XElement deviceNode)
        {
            return deviceNode.Elements("mapping").ToDictionary(x => int.Parse(x.Attribute("socketId").Value), y => int.Parse(y.Value));
        }
    }
}
