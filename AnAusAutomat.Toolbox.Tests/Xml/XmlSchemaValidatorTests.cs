using AnAusAutomat.Toolbox.Xml;
using Serilog;
using Serilog.Sinks.TestCorrelator;
using System.Linq;
using Xunit;

namespace AnAusAutomat.Toolbox.Tests.Xml
{
    public class XmlSchemaValidatorTests
    {
        [Fact]
        public void Validate_SchemaFileAndConfigFileValid()
        {
            setupLogger();

            using (TestCorrelator.CreateContext())
            {
                var validator = new XmlSchemaValidator(schemaFilePath: "_TestData\\config_valid.xsd", configFilePath: "_TestData\\config_valid.xml");

                bool isValid = validator.Validate();
                Assert.True(isValid);

                Assert.False(logMessageIsWritten(validator.SchemaFileNotFoundLogMessage));
                Assert.False(logMessageIsWritten(validator.ConfigFileNotFoundLogMessage));
                Assert.False(logMessageIsWritten(validator.SchemaNotValidLogMessage));
                Assert.False(logMessageIsWritten(validator.ConfigNotValidLogMessage));
            }
        }

        [Fact]
        public void Validate_SchemaFileNotFound()
        {
            setupLogger();

            using (TestCorrelator.CreateContext())
            {
                var validator = new XmlSchemaValidator(schemaFilePath: "", configFilePath: "_TestData\\config_valid.xml");

                bool isValid = validator.Validate();
                Assert.False(isValid);

                Assert.True(logMessageIsWritten(validator.SchemaFileNotFoundLogMessage)); // <--
                Assert.False(logMessageIsWritten(validator.ConfigFileNotFoundLogMessage));
                Assert.False(logMessageIsWritten(validator.SchemaNotValidLogMessage));
                Assert.False(logMessageIsWritten(validator.ConfigNotValidLogMessage));
            }
        }

        [Fact]
        public void Validate_ConfigFileNotFound()
        {
            setupLogger();

            using (TestCorrelator.CreateContext())
            {
                var validator = new XmlSchemaValidator(schemaFilePath: "_TestData\\config_valid.xsd", configFilePath: "");

                bool isValid = validator.Validate();
                Assert.False(isValid);

                Assert.False(logMessageIsWritten(validator.SchemaFileNotFoundLogMessage));
                Assert.True(logMessageIsWritten(validator.ConfigFileNotFoundLogMessage)); // <--
                Assert.False(logMessageIsWritten(validator.SchemaNotValidLogMessage));
                Assert.False(logMessageIsWritten(validator.ConfigNotValidLogMessage));
            }
        }

        [Fact]
        public void Validate_SchemaNotValid()
        {
            setupLogger();

            using (TestCorrelator.CreateContext())
            {
                var validator = new XmlSchemaValidator(schemaFilePath: "_TestData\\config_invalid.xsd", configFilePath: "_TestData\\config_valid.xml");

                bool isValid = validator.Validate();
                Assert.False(isValid);

                Assert.False(logMessageIsWritten(validator.SchemaFileNotFoundLogMessage));
                Assert.False(logMessageIsWritten(validator.ConfigFileNotFoundLogMessage));
                Assert.True(logMessageIsWritten(validator.SchemaNotValidLogMessage)); // <--
                Assert.False(logMessageIsWritten(validator.ConfigNotValidLogMessage));
            }
        }

        [Fact]
        public void Validate_ConfigNotValid()
        {
            setupLogger();

            using (TestCorrelator.CreateContext())
            {
                var validator = new XmlSchemaValidator(schemaFilePath: "_TestData\\config_valid.xsd", configFilePath: "_TestData\\config_invalid.xml");

                bool isValid = validator.Validate();
                Assert.False(isValid);

                Assert.False(logMessageIsWritten(validator.SchemaFileNotFoundLogMessage));
                Assert.False(logMessageIsWritten(validator.ConfigFileNotFoundLogMessage));
                Assert.False(logMessageIsWritten(validator.SchemaNotValidLogMessage));
                Assert.True(logMessageIsWritten(validator.ConfigNotValidLogMessage)); // <--
            }
        }

        private void setupLogger()
        {
            Log.Logger = new LoggerConfiguration().WriteTo.TestCorrelator().CreateLogger();
        }

        private bool logMessageIsWritten(string text)
        {
            int count = TestCorrelator.GetLogEventsFromCurrentContext().Count(x => x.MessageTemplate.Text == text);

            return count == 1;
        }
    }
}
