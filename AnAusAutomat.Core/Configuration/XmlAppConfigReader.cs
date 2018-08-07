using AnAusAutomat.Contracts;
using AnAusAutomat.Contracts.Sensor;
using AnAusAutomat.Core.Conditions;
using AnAusAutomat.Toolbox.Xml;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace AnAusAutomat.Core.Configuration
{
    public class XmlAppConfigReader
    {
        private string _schemaFilePath;
        private string _configFilePath;
        private XmlSchemaValidator _xmlSchemaValidator;
        private XDocument _xDocument;

        private Dictionary<int, string> _socketIDsAndNames;

        private IEnumerable<XElement> _socketNodes;
        private IEnumerable<XElement> _globalSensorNodes;

        public XmlAppConfigReader()
        {
            string currentAssemblyFilePath = new Uri(typeof(XmlAppConfigReader).Assembly.CodeBase).LocalPath;
            string currentAssemblyDirectoryPath = Path.GetDirectoryName(currentAssemblyFilePath);

            _schemaFilePath = currentAssemblyDirectoryPath + "\\config.xsd";
            _configFilePath = currentAssemblyDirectoryPath + "\\config.xml";
            _xmlSchemaValidator = new XmlSchemaValidator(_schemaFilePath, _configFilePath);
        }

        public XmlAppConfigReader(string configFilePath)
        {
            string currentAssemblyFilePath = new Uri(typeof(XmlAppConfigReader).Assembly.CodeBase).LocalPath;
            string currentAssemblyDirectoryPath = Path.GetDirectoryName(currentAssemblyFilePath);

            _schemaFilePath = currentAssemblyDirectoryPath + "\\config.xsd";
            _configFilePath = configFilePath;
            _xmlSchemaValidator = new XmlSchemaValidator(_schemaFilePath, _configFilePath);
        }

        public bool Validate()
        {
            return _xmlSchemaValidator.Validate();
        }

        public AppConfig Load()
        {
            Log.Information(string.Format("Loading settings from {0} ...", _configFilePath));
            _xDocument = XDocument.Load(_configFilePath);

            readNodes();
            readSocketIDsAndNames();

            Log.Information("Loading sensor settings ...");
            var sensors = readSensorSettings();

            Log.Information("Loading conditions ...");
            var conditions = _socketNodes.Select(x => readConditions(x)).SelectMany(x => x);

            Log.Information("Loading modes ...");
            var modes = readModes();

            return new AppConfig(sensors, conditions, modes);
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

        private IEnumerable<ConditionSettings> readConditions(XElement socketNode)
        {
            var socket = new Socket(
                id: int.Parse(socketNode.Attribute("id").Value),
                name: socketNode.Attribute("name").Value);

            string startupAttributeValue = socketNode.Element("conditions").Attribute("startupState").Value.ToLower();
            var startupStatus = new ConditionSettings(
                text: "",
                resultingStatus: convertStringToPowerStatus(startupAttributeValue),
                type: ConditionType.Startup,
                mode: "",
                socket: socket);

            string shutdownAttributeValue = socketNode.Element("conditions").Attribute("shutdownState").Value.ToLower();
            var shutdownStatus = new ConditionSettings(
                text: "",
                resultingStatus: convertStringToPowerStatus(shutdownAttributeValue),
                type: ConditionType.Shutdown,
                mode: "",
                socket: socket);

            var regularConditions = socketNode.Element("conditions").Elements().Where(x => !string.IsNullOrEmpty(x.Value)).Select(x =>
            {
                return new ConditionSettings(
                    text: x.Value,
                    resultingStatus: convertStringToPowerStatus(x.Name.ToString()),
                    type: ConditionType.Regular,
                    mode: x.Attribute("mode") == null ? "" : x.Attribute("mode").Value,
                    socket: socket);
            }).ToList();

            var conditions = new List<ConditionSettings>();
            conditions.Add(startupStatus);
            conditions.Add(shutdownStatus);
            conditions.AddRange(regularConditions);
            return conditions;
        }

        private PowerStatus convertStringToPowerStatus(string value)
        {
            return value == "powerOn" || value == "on" ? PowerStatus.On : PowerStatus.Off;
        }

        private IEnumerable<ConditionMode> readModes()
        {
            Log.Debug("Reading modes ...");

            var modeNodes = _xDocument.Root.Element("modes").Elements("mode");
            return modeNodes.Select(x =>
            {
                string isActiveValueAsString = x.Attribute("active")?.Value ?? "false";

                return new ConditionMode(name: x.Value, isActive: bool.Parse(isActiveValueAsString));
            }).ToList();
        }

        private void readSocketIDsAndNames()
        {
            _socketIDsAndNames = _socketNodes.ToDictionary(x => int.Parse(x.Attribute("id").Value), y => y.Attribute("name").Value);
        }

        private void readNodes()
        {
            _socketNodes = _xDocument.Root.Element("sockets").Elements("socket");
            _globalSensorNodes = _xDocument.Root.Element("sensors").Elements("sensor");
        }

        private XElement getSocketNode(int socketID)
        {
            return _socketNodes.First(x => x.Attribute("id").Value == socketID.ToString());
        }
    }
}
