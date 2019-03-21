using AnAusAutomat.Toolbox.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace AnAusAutomat.Controllers.Serial.Internals
{
    public class XmlConfigReader
    {
        private XDocument _xDocument;

        public XmlConfigReader(string configFilePath)
        {
            _xDocument = XDocument.Load(configFilePath);
        }

        public IEnumerable<DeviceSettings> Read()
        {
            Logger.Information("Loading Serial controller settings ...");
            
            return readDeviceSettings();
        }

        private IEnumerable<DeviceSettings> readDeviceSettings()
        {
            var deviceNodes = _xDocument.Root.Element("devices").Elements("device");
            Logger.Debug(string.Format("Found settings for {0} device(s).", deviceNodes.Count()));

            return deviceNodes.Select(deviceNode =>
            {
                Logger.Information(string.Format("Reading settings for {0} ...", deviceNode.Attribute("name").Value));

                return new DeviceSettings(
                    name: deviceNode.Attribute("name").Value,
                    mapping: readMapping(deviceNode));
            }).ToList();
        }

        private Dictionary<int, int> readMapping(XElement deviceNode)
        {
            return deviceNode.Elements("mapping").ToDictionary(x => int.Parse(x.Attribute("socketId").Value), y => int.Parse(y.Value));
        }
    }
}
