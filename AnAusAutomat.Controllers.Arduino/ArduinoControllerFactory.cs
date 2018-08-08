using AnAusAutomat.Contracts.Controller;
using AnAusAutomat.Controllers.Arduino.Internals;
using AnAusAutomat.Toolbox.Xml;
using ArduinoMajoro;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace AnAusAutomat.Controllers.Arduino
{
    public class ArduinoControllerFactory : IControllerFactory
    {
        public IEnumerable<IController> Create()
        {
            string directoryPath = getCurrentDirectoryPath();
            string schemaFilePath = directoryPath + "\\config.xsd";
            string configFilePath = directoryPath + "\\config.xml";

            var xmlSchemaValidator = new XmlSchemaValidator(schemaFilePath, configFilePath);

            bool isValid = xmlSchemaValidator.Validate(out string message);
            if (!isValid)
            {
                throw new ConfigurationErrorsException("Arduino controller configuration is not valid.");
            }

            var xmlConfigReader = new XmlConfigReader(configFilePath);
            var config = xmlConfigReader.Read();

            return config.Select(x =>
            {
                var majoro = findConnection(x.DeviceName);
                return new ArduinoController(majoro, x);
            });
        }

        private string getCurrentDirectoryPath()
        {
            string currentAssemblyFilePath = new Uri(GetType().Assembly.CodeBase).LocalPath;
            return Path.GetDirectoryName(currentAssemblyFilePath);
        }

        private Majoro findConnection(string name)
        {
            int numberOfRetries = 10;
            string serialPort = null;

            Log.Information(string.Format("Searching for {0}", name));
            for (int i = 0; i < numberOfRetries && serialPort == null; i++)
            {
                Log.Debug(string.Format("Try {0} / {1}", i + 1, numberOfRetries));
                serialPort = Majoro.Hello(name)?.SerialPort;
            }

            if (serialPort == null)
            {
                throwDeviceNotFoundException(name);
            }

            return new Majoro(serialPort);
        }

        private void throwDeviceNotFoundException(string name)
        {
            string message = "Cannot establish connection to controller. Maybe the device is not properly connected, the wire is to long or the wrong sketch is uploaded.";
            var exception = new DeviceNotFoundException(name, message);

            Log.Error(exception, message);
            throw exception;
        }
    }
}
