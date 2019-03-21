using AnAusAutomat.Controllers.Serial.Internals;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AnAusAutomat.Controllers.Serial.Tests.Internals
{
    public class XmlConfigReaderTests
    {
        [Fact]
        public void Read_NoDevices()
        {
            var xmlConfigReader = new XmlConfigReader("_TestData\\config_no_device.xml");
            var settings = xmlConfigReader.Read();

            Assert.Empty(settings);
        }

        [Fact]
        public void Read_DeviceWithoutMapping()
        {
            var xmlConfigReader = new XmlConfigReader("_TestData\\config_device_without_mapping.xml");
            var settings = xmlConfigReader.Read();

            Assert.Single(settings);
            Assert.Equal("Dings", settings.First().Name);
            Assert.Empty(settings.First().Mapping);
        }

        [Fact]
        public void Read_OneDevice()
        {
            var xmlConfigReader = new XmlConfigReader("_TestData\\config_one_device.xml");
            var settings = xmlConfigReader.Read();

            Assert.Single(settings);
            Assert.Equal("Dings", settings.First().Name);
            Assert.Equal(2, settings.First().Mapping.Count);
            Assert.Contains(new KeyValuePair<int, int>(1, 111), settings.First().Mapping);
            Assert.Contains(new KeyValuePair<int, int>(2, 222), settings.First().Mapping);
        }
    }
}
