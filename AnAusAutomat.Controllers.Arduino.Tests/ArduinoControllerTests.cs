using AnAusAutomat.Contracts;
using AnAusAutomat.Controllers.Arduino.Internals;
using ArduinoMajoro;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace AnAusAutomat.Controllers.Arduino.Tests
{
    public class ArduinoControllerTests
    {
        [Fact]
        public void Connect()
        {
            var majoro = new Mock<IMajoro>();
            var settings = getSettingsWithoutPins();

            var controller = new ArduinoController(majoro.Object, settings);
            controller.Connect();

            majoro.Verify(x => x.Connect(), Times.Once);
        }

        [Fact]
        public void Disconnect()
        {
            var majoro = new Mock<IMajoro>();
            var settings = getSettingsWithoutPins();

            var controller = new ArduinoController(majoro.Object, settings);
            controller.Disconnect();

            majoro.Verify(x => x.Disconnect(), Times.Once);
        }

        [Fact]
        public void DeviceIdentifier_IsSet()
        {
            var majoro = new Mock<IMajoro>();
            var settings = getSettingsWithPins();

            var controller = new ArduinoController(majoro.Object, settings);

            Assert.Equal(settings.Name, controller.Device.Name);
        }

        [Fact]
        public void TurnOff_NoPins()
        {
            var majoro = new Mock<IMajoro>();
            majoro.Verify(x => x.WriteLow(It.IsAny<int>()), Times.Never);
            majoro.Verify(x => x.WriteLow(It.IsAny<int>()), Times.Never);
            var settings = getSettingsWithoutPins();

            var controller = new ArduinoController(majoro.Object, settings);
            bool successful = controller.TurnOff(new Socket(-1, string.Empty));

            Assert.True(successful);
        }

        [Fact]
        public void TurnOff_SocketNotConfigured()
        {
            var majoro = new Mock<IMajoro>();
            majoro.Verify(x => x.WriteLow(It.IsAny<int>()), Times.Never);
            majoro.Verify(x => x.WriteLow(It.IsAny<int>()), Times.Never);
            var settings = getSettingsWithPins();

            var controller = new ArduinoController(majoro.Object, settings);
            bool successful = controller.TurnOff(new Socket(-1, string.Empty));

            Assert.True(successful);
        }

        [Fact]
        public void TurnOff()
        {
            var majoro = new Mock<IMajoro>();
            majoro.Setup(x => x.WriteHigh(8)).Returns(true);
            majoro.Setup(x => x.WriteLow(9)).Returns(true);
            var settings = getSettingsWithPins();

            var controller = new ArduinoController(majoro.Object, settings);
            bool successful = controller.TurnOff(new Socket(1, string.Empty));

            Assert.True(successful);
        }

        [Fact]
        public void TurnOn_NoPins()
        {
            var majoro = new Mock<IMajoro>();
            majoro.Verify(x => x.WriteLow(It.IsAny<int>()), Times.Never);
            majoro.Verify(x => x.WriteLow(It.IsAny<int>()), Times.Never);
            var settings = getSettingsWithoutPins();

            var controller = new ArduinoController(majoro.Object, settings);
            bool successful = controller.TurnOn(new Socket(-1, string.Empty));

            Assert.True(successful);
        }

        [Fact]
        public void TurnOn_SocketNotConfigured()
        {
            var majoro = new Mock<IMajoro>();
            majoro.Verify(x => x.WriteLow(It.IsAny<int>()), Times.Never);
            majoro.Verify(x => x.WriteLow(It.IsAny<int>()), Times.Never);
            var settings = getSettingsWithPins();

            var controller = new ArduinoController(majoro.Object, settings);
            bool successful = controller.TurnOff(new Socket(-1, string.Empty));

            Assert.True(successful);
        }

        [Fact]
        public void TurnOn()
        {
            var majoro = new Mock<IMajoro>();
            majoro.Setup(x => x.WriteLow(8)).Returns(true);
            majoro.Setup(x => x.WriteHigh(9)).Returns(true);
            var settings = getSettingsWithPins();

            var controller = new ArduinoController(majoro.Object, settings);
            bool successful = controller.TurnOn(new Socket(1, string.Empty));

            Assert.True(successful);
        }

        private DeviceSettings getSettingsWithoutPins()
        {
            return new DeviceSettings(
                name: "Sam",
                pins: new List<Pin>());
        }

        private DeviceSettings getSettingsWithPins()
        {
            return new DeviceSettings(
                name: "Sam",
                pins: new List<Pin>()
                {
                    new Pin(1, 8, "Relay", PinLogic.Negative),
                    new Pin(1, 9, "LED", PinLogic.Positive)
                });
        }
    }
}
