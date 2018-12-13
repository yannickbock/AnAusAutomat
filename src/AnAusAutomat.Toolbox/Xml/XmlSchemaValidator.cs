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
        private string _schemaNotValidMessage;
        private string _configNotValidMessage;
        private string _schemaFileNotFoundMessage;
        private string _configFileNotFoundMessage;
        private string _message;

        public XmlSchemaValidator(string schemaFilePath, string configFilePath)
        {
            _schemaFilePath = schemaFilePath;
            _configFilePath = configFilePath;

            _schemaNotValidMessage = string.Format("Schema file {0} is corrupted.", _schemaFilePath);
            _configNotValidMessage = string.Format("Config file {0} is not valid.", _configFilePath);
            _schemaFileNotFoundMessage = string.Format("Schema file {0} does not exist.", _schemaFilePath);
            _configFileNotFoundMessage = string.Format("Config file {0} does not exist.", _configFilePath);
        }

        public bool Validate()
        {
            return Validate(out string message);
        }

        public bool Validate(out string message)
        {
            _message = string.Empty;

            bool schemaFileExists = checkIfSchemaFileExists();
            bool configFileExists = checkIfConfigFileExists();

            if (schemaFileExists && configFileExists)
            {
                var xmlSchemaSet = loadXmlSchemaSet();
                var xDocument = loadXDocument();

                if (xmlSchemaSet != null && xDocument != null)
                {
                    bool isValid = validate(xmlSchemaSet, xDocument);

                    message = _message.Trim();
                    return isValid;
                }
            }

            message = _message.Trim();
            return false;
        }

        private bool validate(XmlSchemaSet xmlSchemaSet, XDocument xDocument)
        {
            bool isValid = true;

            try
            {
                xDocument.Validate(xmlSchemaSet, (sender, e) =>
                {
                    var exception = new ConfigurationErrorsException(_configNotValidMessage, e.Exception, _configFilePath, 0);

                    _message += _configNotValidMessage + "\n";
                    Log.Error(_configNotValidMessage, exception);

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
                _message += _schemaFileNotFoundMessage + "\n";
                Log.Error(_schemaFileNotFoundMessage);
            }

            return fileExists;
        }

        private bool checkIfConfigFileExists()
        {
            bool fileExists = File.Exists(_configFilePath);

            if (!fileExists)
            {
                _message += _configFileNotFoundMessage + "\n";
                Log.Error(_configFileNotFoundMessage);
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
                var exception = new ConfigurationErrorsException(_schemaNotValidMessage, e, _schemaFilePath, e.LineNumber);

                _message += _schemaNotValidMessage + "\n";
                Log.Error(_schemaNotValidMessage, exception);
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
