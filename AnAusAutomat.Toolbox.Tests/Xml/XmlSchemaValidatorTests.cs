using AnAusAutomat.Toolbox.Xml;
using Xunit;

namespace AnAusAutomat.Toolbox.Tests.Xml
{
    public class XmlSchemaValidatorTests
    {
        [Fact]
        public void Validate_SchemaFileAndConfigFileValid()
        {
            var validator = new XmlSchemaValidator(schemaFilePath: "_TestData\\config_valid.xsd", configFilePath: "_TestData\\config_valid.xml");

            bool isValid = validator.Validate(out string message);

            Assert.True(isValid);
            Assert.Empty(message);
        }

        [Fact]
        public void Validate_SchemaFileNotFound()
        {
            var validator = new XmlSchemaValidator(schemaFilePath: "ghost.xsd", configFilePath: "_TestData\\config_valid.xml");

            bool isValid = validator.Validate(out string message);

            Assert.False(isValid);
            Assert.Equal("Schema file ghost.xsd does not exist.", message);
        }

        [Fact]
        public void Validate_ConfigFileNotFound()
        {
            var validator = new XmlSchemaValidator(schemaFilePath: "_TestData\\config_valid.xsd", configFilePath: "ghost.xml");

            bool isValid = validator.Validate(out string message);

            Assert.False(isValid);
            Assert.Equal("Config file ghost.xml does not exist.", message);
        }

        [Fact]
        public void Validate_SchemaNotValid()
        {
            var validator = new XmlSchemaValidator(schemaFilePath: "_TestData\\config_invalid.xsd", configFilePath: "_TestData\\config_valid.xml");

            bool isValid = validator.Validate(out string message);

            Assert.False(isValid);
            Assert.Equal("Schema file _TestData\\config_invalid.xsd is corrupted.", message);
        }

        [Fact]
        public void Validate_ConfigNotValid()
        {
            var validator = new XmlSchemaValidator(schemaFilePath: "_TestData\\config_valid.xsd", configFilePath: "_TestData\\config_invalid.xml");

            bool isValid = validator.Validate(out string message);

            Assert.False(isValid);
            Assert.Equal("Config file _TestData\\config_invalid.xml is not valid.", message);
        }
    }
}
