using AnAusAutomat.Controllers.Arduino.Internals;
using System.Linq;
using Xunit;

namespace AnAusAutomat.Controllers.Arduino.Tests.Internals
{
    public class XmlConfigReaderTests
    {
        [Fact]
        public void Read_NoDevice()
        {
            var xmlConfigReader = new XmlConfigReader("_TestData\\config_no_device.xml");
            var settings = xmlConfigReader.Read();

            Assert.Empty(settings);
        }

        [Fact]
        public void Read_OneDevice()
        {
            var xmlConfigReader = new XmlConfigReader("_TestData\\config_one_device.xml");
            var settings = xmlConfigReader.Read();

            var device = settings.FirstOrDefault();

            Assert.Single(settings);
            Assert.Equal("Sam", device.Name);

            Assert.Equal(4, device.Pins.Count());
            Assert.Contains(new Pin(1, 6, "Relay + LED", PinLogic.Negative), device.Pins);
            Assert.Contains(new Pin(2, 8, "Relay + LED", PinLogic.Positive), device.Pins);
            Assert.Contains(new Pin(3, 10, "Relay + LED", PinLogic.Positive), device.Pins);
            Assert.Contains(new Pin(4, 12, "Relay + LED", PinLogic.Positive), device.Pins);
        }
    }
}
