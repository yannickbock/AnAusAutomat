using Serilog;
using System;
using System.Configuration;
using System.IO;
using System.Xml.Linq;
using System.Xml.Schema;

namespace AnAusAutomat.Toolbox.Xml
{
    public class XmlSchemaValidator
    {
        private string _schemaFilePath;
        private string _configFilePath;

        public string SchemaNotValidLogMessage { get; set; }

        public string ConfigNotValidLogMessage { get; set; }

        public string SchemaFileNotFoundLogMessage { get; set; }

        public string ConfigFileNotFoundLogMessage { get; set; }

        public XmlSchemaValidator(string schemaFilePath, string configFilePath)
        {
            _schemaFilePath = schemaFilePath;
            _configFilePath = configFilePath;

            SchemaNotValidLogMessage = string.Format("Schema file {0} is corrupted.", _schemaFilePath);
            ConfigNotValidLogMessage = string.Format("Config file {0} is not valid.", _configFilePath);
            SchemaFileNotFoundLogMessage = string.Format("Schema file {0} does not exist.", _schemaFilePath);
            ConfigFileNotFoundLogMessage = string.Format("Config file {0} does not exist.", _configFilePath);
        }

        public bool Validate()
        {
            bool schemaFileExists = checkIfSchemaFileExists();
            bool configFileExists = checkIfConfigFileExists();

            if (schemaFileExists && configFileExists)
            {
                var xmlSchemaSet = loadXmlSchemaSet();
                var xDocument = loadXDocument();

                if (xmlSchemaSet != null && xDocument != null)
                {
                    bool isValid = validate(xmlSchemaSet, xDocument);

                    return isValid;
                }
            }

            return false;
        }

        private bool validate(XmlSchemaSet xmlSchemaSet, XDocument xDocument)
        {
            bool isValid = true;

            try
            {
                xDocument.Validate(xmlSchemaSet, (sender, e) =>
                {
                    var exception = new ConfigurationErrorsException(ConfigNotValidLogMessage, e.Exception, _configFilePath, 0);

                    Log.Error(ConfigNotValidLogMessage, exception);

                    isValid = false;
                });
            }
            catch (XmlSchemaValidationException e)
            {
                Log.Error(e.Message);
                isValid = false;
            }

            return isValid;
        }

        private bool checkIfSchemaFileExists()
        {
            bool fileExists = File.Exists(_schemaFilePath);

            if (!fileExists)
            {
                Log.Error(SchemaFileNotFoundLogMessage);
            }

            return fileExists;
        }

        private bool checkIfConfigFileExists()
        {
            bool fileExists = File.Exists(_configFilePath);

            if (!fileExists)
            {
                Log.Error(ConfigFileNotFoundLogMessage);
            }

            return fileExists;
        }

        private XmlSchemaSet loadXmlSchemaSet()
        {
            try
            {
                var schemas = new XmlSchemaSet();
                schemas.Add(null, _schemaFilePath);
                return schemas;
            }
            catch (XmlSchemaException e)
            {
                var exception = new ConfigurationErrorsException(SchemaNotValidLogMessage, e, _schemaFilePath, e.LineNumber);

                Log.Error(SchemaNotValidLogMessage, exception);
            }
            catch (ArgumentNullException)
            {
                // Should not be possible. checkIfSchemaFileExists() is called before.
            }

            return null;
        }

        private XDocument loadXDocument()
        {
            return XDocument.Load(_configFilePath);
        }
    }
}
