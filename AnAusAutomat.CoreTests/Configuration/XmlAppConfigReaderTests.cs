using AnAusAutomat.Contracts.Controller;
using AnAusAutomat.Contracts.Sensor;
using AnAusAutomat.Core.Conditions;
using AnAusAutomat.Core.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace AnAusAutomat.Core.Tests
{
    [TestClass]
    public class XmlAppConfigReaderTests
    {
        [TestMethod]
        public void Load_TestDevicesTopLevel()
        {
            var config = getConfig();

            Assert.AreEqual(2, config.Devices.Count());

            Assert.AreEqual(1, config.Devices.Count(a => a.ID == 1 && a.Name == "Frodo" && a.Type == "Arduino"));
            Assert.AreEqual(1, config.Devices.Count(a => a.ID == 99 && a.Name == "Banane" && a.Type == "Probe"));
        }

        [TestMethod]
        public void Load_TestDevicesSockets()
        {
            var config = getConfig();

            var frodoDevice = config.Devices.First(a => a.Name == "Frodo");
            var bananeDevice = config.Devices.First(a => a.Name == "Banane");

            Assert.AreEqual(4, frodoDevice.Sockets.Count());
            Assert.AreEqual(1, frodoDevice.Sockets.Count(a => a.ID == 1 && a.Name == "Monitor"));
            Assert.AreEqual(1, frodoDevice.Sockets.Count(a => a.ID == 2 && a.Name == "Verstärker"));
            Assert.AreEqual(1, frodoDevice.Sockets.Count(a => a.ID == 3 && a.Name == "Lampe"));
            Assert.AreEqual(1, frodoDevice.Sockets.Count(a => a.ID == 4 && a.Name == "Externe HDDs"));
            Assert.AreEqual(1, bananeDevice.Sockets.Count(a => a.ID == 4 && a.Name == "Externe HDDs"));
        }

        [TestMethod]
        public void Load_TestDevicesSocketsPins()
        {
            var config = getConfig();

            var frodoDevice = config.Devices.First(a => a.Name == "Frodo");
            var bananeDevice = config.Devices.First(a => a.Name == "Banane");

            var monitorFrodo = frodoDevice.Sockets.First(a => a.Name == "Monitor");
            var amplifierFrodo = frodoDevice.Sockets.First(a => a.Name == "Verstärker");
            var bulbFrodo = frodoDevice.Sockets.First(a => a.Name == "Lampe");
            var hddsFrodo = frodoDevice.Sockets.First(a => a.Name == "Externe HDDs");
            var hddsBanane = bananeDevice.Sockets.First(a => a.Name == "Externe HDDs");

            Assert.AreEqual(2, monitorFrodo.Pins.Count());
            Assert.AreEqual(2, amplifierFrodo.Pins.Count());
            Assert.AreEqual(2, bulbFrodo.Pins.Count());
            Assert.AreEqual(2, hddsFrodo.Pins.Count());
            Assert.AreEqual(1, hddsBanane.Pins.Count());

            Assert.AreEqual(1, monitorFrodo.Pins.Count(a => a.Name == "Relay" && a.Address == 2 && a.Logic == PinLogic.Negative));
            Assert.AreEqual(1, monitorFrodo.Pins.Count(a => a.Name == "LED" && a.Address == 9 && a.Logic == PinLogic.Positive));
            Assert.AreEqual(1, amplifierFrodo.Pins.Count(a => a.Name == "Relay" && a.Address == 3 && a.Logic == PinLogic.Positive));
            Assert.AreEqual(1, amplifierFrodo.Pins.Count(a => a.Name == "LED" && a.Address == 10 && a.Logic == PinLogic.Positive));
            Assert.AreEqual(1, bulbFrodo.Pins.Count(a => a.Name == "Relay" && a.Address == 4 && a.Logic == PinLogic.Positive));
            Assert.AreEqual(1, bulbFrodo.Pins.Count(a => a.Name == "LED" && a.Address == 11 && a.Logic == PinLogic.Positive));
            Assert.AreEqual(1, hddsFrodo.Pins.Count(a => a.Name == "Relay" && a.Address == 5 && a.Logic == PinLogic.Positive));
            Assert.AreEqual(1, hddsFrodo.Pins.Count(a => a.Name == "LED" && a.Address == 12 && a.Logic == PinLogic.Positive));
            Assert.AreEqual(1, hddsBanane.Pins.Count(a => a.Name == "Pferd" && a.Address == 66 && a.Logic == PinLogic.Positive));
        }

        [TestMethod]
        public void Load_TestSensorsTopLevel()
        {
            var config = getConfig();

            Assert.AreEqual(3, config.Sensors.Count());
            Assert.AreEqual(1, config.Sensors.Count(a => a.SensorName == "UserInputDetector"));
            Assert.AreEqual(1, config.Sensors.Count(a => a.SensorName == "SoundDetector"));
            Assert.AreEqual(1, config.Sensors.Count(a => a.SensorName == "TrayIcon"));
        }

        [TestMethod]
        public void Load_TestSensorsGlobalParameters()
        {
            var config = getConfig();

            var userInputDetector = config.Sensors.First(a => a.SensorName == "UserInputDetector");
            var soundDetector = config.Sensors.First(a => a.SensorName == "SoundDetector");
            var trayIcon = config.Sensors.First(a => a.SensorName == "TrayIcon");

            Assert.AreEqual(0, userInputDetector.Parameters.Count());
            Assert.AreEqual(0, soundDetector.Parameters.Count());
            Assert.AreEqual(0, trayIcon.Parameters.Count());
        }

        [TestMethod]
        public void Load_TestSensorsSockets()
        {
            var config = getConfig();

            var userInputDetector = config.Sensors.First(a => a.SensorName == "UserInputDetector");
            var soundDetector = config.Sensors.First(a => a.SensorName == "SoundDetector");
            var trayIcon = config.Sensors.First(a => a.SensorName == "TrayIcon");

            Assert.AreEqual(2, userInputDetector.Sockets.Count());
            Assert.AreEqual(3, soundDetector.Sockets.Count());
            Assert.AreEqual(4, trayIcon.Sockets.Count());

            Assert.AreEqual(1, userInputDetector.Sockets.Count(a => a.Name == "Monitor"));
            Assert.AreEqual(1, userInputDetector.Sockets.Count(a => a.Name == "Lampe"));

            Assert.AreEqual(1, soundDetector.Sockets.Count(a => a.Name == "Monitor"));
            Assert.AreEqual(1, soundDetector.Sockets.Count(a => a.Name == "Verstärker"));
            Assert.AreEqual(1, soundDetector.Sockets.Count(a => a.Name == "Externe HDDs"));

            Assert.AreEqual(1, trayIcon.Sockets.Count(a => a.Name == "Monitor"));
            Assert.AreEqual(1, trayIcon.Sockets.Count(a => a.Name == "Lampe"));
            Assert.AreEqual(1, trayIcon.Sockets.Count(a => a.Name == "Verstärker"));
            Assert.AreEqual(1, trayIcon.Sockets.Count(a => a.Name == "Externe HDDs"));
        }

        [TestMethod]
        public void Load_TestSensorsSocketSpecificParameters()
        {
            var config = getConfig();

            var userInputDetector = config.Sensors.First(a => a.SensorName == "UserInputDetector");
            var soundDetector = config.Sensors.First(a => a.SensorName == "SoundDetector");
            var trayIcon = config.Sensors.First(a => a.SensorName == "TrayIcon");

            var userInputDetectorMonitor = userInputDetector.Sockets.First(a => a.Name == "Monitor");
            var userInputDetectorBulb = userInputDetector.Sockets.First(a => a.Name == "Lampe");

            var soundDetectorMonitor = soundDetector.Sockets.First(a => a.Name == "Monitor");
            var soundDetectorAmplifier = soundDetector.Sockets.First(a => a.Name == "Verstärker");
            var soundDetectorHDDs = soundDetector.Sockets.First(a => a.Name == "Externe HDDs");

            var trayIconMonitor = trayIcon.Sockets.First(a => a.Name == "Monitor");
            var trayIconBulb = trayIcon.Sockets.First(a => a.Name == "Lampe");
            var trayIconAmplifier = trayIcon.Sockets.First(a => a.Name == "Verstärker");
            var trayIconHDDs = trayIcon.Sockets.First(a => a.Name == "Externe HDDs");

            Assert.AreEqual(1, userInputDetectorMonitor.Parameters.Count(a => a.Name == "OffDelaySeconds" && a.Value == "600"));
            Assert.AreEqual(1, userInputDetectorBulb.Parameters.Count(a => a.Name == "OffDelaySeconds" && a.Value == "570"));

            Assert.AreEqual(1, soundDetectorMonitor.Parameters.Count(a => a.Name == "OffDelaySeconds" && a.Value == "600"));
            Assert.AreEqual(1, soundDetectorMonitor.Parameters.Count(a => a.Name == "MinimumSignalSeconds" && a.Value == "5"));
            Assert.AreEqual(1, soundDetectorAmplifier.Parameters.Count(a => a.Name == "OffDelaySeconds" && a.Value == "900"));
            Assert.AreEqual(1, soundDetectorAmplifier.Parameters.Count(a => a.Name == "MinimumSignalSeconds" && a.Value == "5"));
            Assert.AreEqual(1, soundDetectorHDDs.Parameters.Count(a => a.Name == "OffDelaySeconds" && a.Value == "600"));
            Assert.AreEqual(1, soundDetectorHDDs.Parameters.Count(a => a.Name == "MinimumSignalSeconds" && a.Value == "5"));

            Assert.AreEqual(0, trayIconMonitor.Parameters.Count());
            Assert.AreEqual(0, trayIconBulb.Parameters.Count());
            Assert.AreEqual(0, trayIconAmplifier.Parameters.Count());
            Assert.AreEqual(0, trayIconHDDs.Parameters.Count());
        }

        [TestMethod]
        public void Load_TestConditionsStartupAndShutdownState()
        {
            var config = getConfig();

            var startup = config.Conditions.Where(a => a.Type == ConditionType.Startup);
            var shutdown = config.Conditions.Where(a => a.Type == ConditionType.Shutdown);

            Assert.AreEqual(4, startup.Count());
            Assert.AreEqual(4, shutdown.Count());

            Assert.AreEqual(PowerStatus.On, startup.First(x => x.Socket.Name == "Monitor").ResultingStatus);
            Assert.AreEqual(PowerStatus.On, startup.First(x => x.Socket.Name == "Lampe").ResultingStatus);
            Assert.AreEqual(PowerStatus.Off, startup.First(x => x.Socket.Name == "Verstärker").ResultingStatus);
            Assert.AreEqual(PowerStatus.Off, startup.First(x => x.Socket.Name == "Externe HDDs").ResultingStatus);

            Assert.AreEqual(PowerStatus.On, shutdown.First(x => x.Socket.Name == "Monitor").ResultingStatus);
            Assert.AreEqual(PowerStatus.Off, shutdown.First(x => x.Socket.Name == "Lampe").ResultingStatus);
            Assert.AreEqual(PowerStatus.Off, shutdown.First(x => x.Socket.Name == "Verstärker").ResultingStatus);
            Assert.AreEqual(PowerStatus.Off, shutdown.First(x => x.Socket.Name == "Externe HDDs").ResultingStatus);
        }

        [TestMethod]
        public void Load_TestConditionsRegular()
        {
            var config = getConfig();

            var regular = config.Conditions.Where(a => a.Type == ConditionType.Regular);

            // TODO
        }

        private AppConfig getConfig()
        {
            var reader = new XmlAppConfigReader("valid.xml");
            reader.Validate();
            reader.Load();

            return reader;
        }
    }
}