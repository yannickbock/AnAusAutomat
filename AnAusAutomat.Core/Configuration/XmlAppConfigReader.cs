using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Controller;
using AnAusAutomat.Contracts.Sensor;
using AnAusAutomat.Core.Conditions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Schema;

namespace AnAusAutomat.Core.Configuration
{
    public class XmlAppConfigReader : AppConfig
    {
        private XDocument _xDocument;
        private string _filePath;
        private Dictionary<int, string> _socketIDsAndNames;

        private IEnumerable<XElement> _socketNodes;
        private IEnumerable<XElement> _controllerNodes;
        private IEnumerable<XElement> _globalSensorNodes;

        public XmlAppConfigReader(string filePath)
        {
            _filePath = filePath;

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("", filePath);
            }
            _xDocument = XDocument.Load(filePath);
        }

        public bool Validate()
        {
            bool isOK = true;
            var schemas = loadAndValidateSchema();

            Log.Debug(string.Format("Validating configuration file {0} ...", _filePath));
            try
            {
                _xDocument.Validate(schemas, (sender, e) =>
                {
                    string message = string.Format("The configuration file {0} is not valid.", _filePath);
                    var exception = new ConfigurationErrorsException(message, e.Exception, _filePath, 0);

                    Log.Error(message, exception);

                    // exception or not?
                    isOK = false;
                });
            }
            catch (XmlSchemaValidationException e)
            {
                Log.Error(e.Message);
                isOK = false;
            }

            return isOK;
        }

        private XmlSchemaSet loadAndValidateSchema()
        {
            string xsdFile = "config.xsd";
            var schemas = new XmlSchemaSet();

            Log.Debug(string.Format("Loading configuration schema file {0} ...", xsdFile));
            try
            {
                schemas.Add(null, xsdFile);
            }
            catch (XmlSchemaException e)
            {
                string message = string.Format("The configuration schema file {0} is corrupted.", xsdFile);
                var exception = new ConfigurationErrorsException(message, e, xsdFile, e.LineNumber);

                Log.Error(message, exception);
                throw exception;
            }
            catch (ArgumentNullException)
            {
                // Should not be possible. File name is hardcoded.
            }

            return schemas;
        }

        public void Load()
        {
            Log.Information(string.Format("Loading settings from {0} ...", _filePath));

            readNodes();
            readSocketIDsAndNames();

            Log.Information("Loading controller settings ...");
            Devices = readDevices();

            Log.Information("Loading sensor settings ...");
            Sensors = readSensorSettings();

            Log.Information("Loading conditions ...");
            Conditions = _socketNodes.Select(x => readConditions(x)).SelectMany(x => x);

            Log.Information("Loading modes ...");
            Modes = readModes();
            DefaultMode = readDefaultMode();
        }

        private IEnumerable<SensorSettings> readSensorSettings()
        {
            var sensorNames = readSensorNames();

            var listOfSensorSettings = new List<SensorSettings>();

            foreach (string sensorName in sensorNames)
            {
                var parameters = readGlobalSensorParameters(sensorName);

                var sensorSettings = new SensorSettings(sensorName, parameters, readSocketsWithSensorParameters(sensorName));

                listOfSensorSettings.Add(sensorSettings);
            }

            return listOfSensorSettings;
        }

        private IEnumerable<SocketWithSensorParameters> readSocketsWithSensorParameters(string sensorName)
        {
            return _socketIDsAndNames
                .Where(x => isSocketsRegistered(x.Key, sensorName))
                .Select(x => readSocketWithSensorParameters(x.Key, sensorName)).ToList();
        }

        private bool isSocketsRegistered(int socketID, string sensorName)
        {
            var socketNode = getSocketNode(socketID);
            return socketNode.Element("sensors").Elements("sensor").Count(x => x.Attribute("name").Value == sensorName) == 1;
        }

        private SocketWithSensorParameters readSocketWithSensorParameters(int socketID, string sensorName)
        {
            var socketNode = getSocketNode(socketID);
            var sensorNode = socketNode.Element("sensors").Elements("sensor").Where(x => x.Attribute("name").Value == sensorName);

            var sensorParameters = sensorNode.Elements("parameter").Select(x =>
                new SensorParameter(
                    name: x.Attribute("name").Value,
                    value: x.Value));

            return new SocketWithSensorParameters(
                id: socketID,
                name: _socketIDsAndNames[socketID],
                parameters: sensorParameters);
        }

        private IEnumerable<string> readSensorNames()
        {
            var sensorsWithGlobalParameters = _globalSensorNodes.Select(x => x.Attribute("name").Value).Distinct();

            var sensorsAtSocketLevel = _xDocument.Root.Element("sockets").Elements("socket").Elements("sensors").Elements("sensor")
                .Select(x => x.Attribute("name").Value).Distinct();

            var sensorNames = new List<string>();
            sensorNames.AddRange(sensorsWithGlobalParameters);
            sensorNames.AddRange(sensorsAtSocketLevel);

            return sensorNames.Distinct();
        }

        private IEnumerable<SensorParameter> readSocketSpecificSensorParameters(string sensorName, int socketID)
        {
            var socketNode = getSocketNode(socketID);
            var sensorNodes = socketNode.Element("sensors").Elements("sensor").Where(x => x.Attribute("name").Value == sensorName);

            return sensorNodes.Select(x => readSensorParameter(x));
        }

        private IEnumerable<SensorParameter> readGlobalSensorParameters(string sensorName)
        {
            var parameterNodes = _globalSensorNodes.Where(x => x.Attribute("name").Value == sensorName);

            var result = parameterNodes.Elements("parameter").Select(x => readSensorParameter(x));

            return result.ToList() ?? new List<SensorParameter>();
        }

        private SensorParameter readSensorParameter(XElement sensorNode)
        {
            return new SensorParameter(
                name: sensorNode.Attribute("name").Value,
                value: sensorNode.Value);
        }

        private IEnumerable<Device> readDevices()
        {
            var devices = _controllerNodes.Select(controllerNode =>
            {
                var socketIDs = readSocketIDsFromControllerNode(controllerNode);
                var sockets = socketIDs.Select(socketID =>
                {
                    return new SocketWithPins(
                        id: socketID,
                        name: _socketIDsAndNames[socketID],
                        pins: readPins(controllerNode, socketID));
                });

                return new Device(
                    id: int.Parse(controllerNode.Attribute("id").Value),
                    name: controllerNode.Attribute("name").Value,
                    type: controllerNode.Attribute("type").Value,
                    sockets: sockets);
            });

            return devices.ToList();
        }

        private IEnumerable<Pin> readPins(XElement controllerNode, int socketID)
        {
            var pinNodes = controllerNode.Elements("pin").Where(x => x.Attribute("socketID").Value == socketID.ToString());

            var pins = pinNodes.Select(pinNode =>
            {
                return new Pin(
                    address: int.Parse(pinNode.Value),
                    name: pinNode.Attribute("name").Value,
                    logic: convertStringToPinLogic(pinNode.Attribute("logic").Value));
            });

            return pins;
        }

        private PinLogic convertStringToPinLogic(string value)
        {
            return value.ToLower() == "negative" ? PinLogic.Negative : PinLogic.Positive;
        }

        private IEnumerable<int> readSocketIDsFromControllerNode(XElement controllerNode)
        {
            return controllerNode.Elements("pin").Attributes("socketID").Select(x => int.Parse(x.Value)).Distinct();
        }

        private IEnumerable<Condition> readConditions(XElement socketNode)
        {
            var socket = new Socket(
                id: int.Parse(socketNode.Attribute("id").Value),
                name: socketNode.Attribute("name").Value);

            string startupAttributeValue = socketNode.Element("controlConditions").Attribute("startupState").Value.ToLower();
            var startupStatus = new Condition(
                command: "",
                resultingStatus: convertStringToPowerStatus(startupAttributeValue),
                type: ConditionType.Startup,
                mode: "",
                socket: socket);

            string shutdownAttributeValue = socketNode.Element("controlConditions").Attribute("shutdownState").Value.ToLower();
            var shutdownStatus = new Condition(
                command: "",
                resultingStatus: convertStringToPowerStatus(shutdownAttributeValue),
                type: ConditionType.Shutdown,
                mode: "",
                socket: socket);

            var regularConditions = socketNode.Element("controlConditions").Elements().Where(x => !string.IsNullOrEmpty(x.Value)).Select(x =>
            {
                return new Condition(
                    command: x.Value,
                    resultingStatus: convertStringToPowerStatus(x.Name.ToString()),
                    type: ConditionType.Regular,
                    mode: x.Attribute("mode") == null ? "" : x.Attribute("mode").Value,
                    socket: socket);
            }).ToList();

            var conditions = new List<Condition>();
            conditions.Add(startupStatus);
            conditions.Add(shutdownStatus);
            conditions.AddRange(regularConditions);
            return conditions;
        }

        private PowerStatus convertStringToPowerStatus(string value)
        {
            return value == "powerOn" || value == "on" ? PowerStatus.On : PowerStatus.Off;
        }

        private IEnumerable<string> readModes()
        {
            Log.Debug("Reading modes ...");

            var conditionElements = _xDocument.Root.Descendants("controlConditions").Elements();
            var modes = conditionElements.Where(x => x.Attribute("mode") != null).Select(x => x.Attribute("mode").Value).Distinct();

            return modes.ToList();
        }

        private string readDefaultMode()
        {
            return _xDocument.Root.Element("sockets").Attribute("defaultMode").Value;
        }

        private void readSocketIDsAndNames()
        {
            _socketIDsAndNames = _socketNodes.ToDictionary(x => int.Parse(x.Attribute("id").Value), y => y.Attribute("name").Value);
        }

        private void readNodes()
        {
            _socketNodes = _xDocument.Root.Element("sockets").Elements("socket");
            _controllerNodes = _xDocument.Root.Element("controllers").Elements("controller");
            _globalSensorNodes = _xDocument.Root.Element("sensors").Elements("sensor");
        }

        private XElement getSocketNode(int socketID)
        {
            return _socketNodes.First(x => x.Attribute("id").Value == socketID.ToString());
        }
    }
}
