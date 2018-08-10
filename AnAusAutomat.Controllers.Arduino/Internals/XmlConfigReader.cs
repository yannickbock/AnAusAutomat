using AnAusAutomat.Toolbox.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace AnAusAutomat.Controllers.Arduino.Internals
{
    public class XmlConfigReader
    {
        private XDocument _xDocument;

        public XmlConfigReader(string configFilePath)
        {
            _xDocument = XDocument.Load(configFilePath);
        }

        public IEnumerable<ControllerSettings> Read()
        {
            Logger.Information("Loading arduino controller settings ...");

            return readDevices();
        }

        private IEnumerable<ControllerSettings> readDevices()
        {
            return _xDocument.Root.Element("devices").Elements("device").Select(deviceNode =>
            {
                return new ControllerSettings(
                    deviceName: deviceNode.Attribute("name").Value,
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
