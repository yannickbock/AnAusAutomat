using AnAusAutomat.Contracts;
using AnAusAutomat.Core.Conditions;
using AnAusAutomat.Core.Configuration;
using System.Linq;
using Xunit;

namespace AnAusAutomat.Core.Tests.Configuration
{
    public class XmlAppConfigReaderTests
    {
        [Fact]
        public void Load_TestSensorsTopLevel()
        {
            var config = getConfig();

            Assert.Equal(3, config.Sensors.Count());
            Assert.Equal(1, config.Sensors.Count(a => a.SensorName == "UserInputDetector"));
            Assert.Equal(1, config.Sensors.Count(a => a.SensorName == "SoundDetector"));
            Assert.Equal(1, config.Sensors.Count(a => a.SensorName == "TrayIcon"));
        }

        [Fact]
        public void Load_TestSensorsGlobalParameters()
        {
            var config = getConfig();

            var userInputDetector = config.Sensors.First(a => a.SensorName == "UserInputDetector");
            var soundDetector = config.Sensors.First(a => a.SensorName == "SoundDetector");
            var trayIcon = config.Sensors.First(a => a.SensorName == "TrayIcon");

            Assert.Empty(userInputDetector.Parameters);
            Assert.Empty(soundDetector.Parameters);
            Assert.Empty(trayIcon.Parameters);
        }

        [Fact]
        public void Load_TestSensorsSockets()
        {
            var config = getConfig();

            var userInputDetector = config.Sensors.First(a => a.SensorName == "UserInputDetector");
            var soundDetector = config.Sensors.First(a => a.SensorName == "SoundDetector");
            var trayIcon = config.Sensors.First(a => a.SensorName == "TrayIcon");

            Assert.Equal(2, userInputDetector.Sockets.Count());
            Assert.Equal(3, soundDetector.Sockets.Count());
            Assert.Equal(4, trayIcon.Sockets.Count());

            Assert.Equal(1, userInputDetector.Sockets.Count(a => a.Name == "Monitor"));
            Assert.Equal(1, userInputDetector.Sockets.Count(a => a.Name == "Lampe"));

            Assert.Equal(1, soundDetector.Sockets.Count(a => a.Name == "Monitor"));
            Assert.Equal(1, soundDetector.Sockets.Count(a => a.Name == "Verstärker"));
            Assert.Equal(1, soundDetector.Sockets.Count(a => a.Name == "Externe HDDs"));

            Assert.Equal(1, trayIcon.Sockets.Count(a => a.Name == "Monitor"));
            Assert.Equal(1, trayIcon.Sockets.Count(a => a.Name == "Lampe"));
            Assert.Equal(1, trayIcon.Sockets.Count(a => a.Name == "Verstärker"));
            Assert.Equal(1, trayIcon.Sockets.Count(a => a.Name == "Externe HDDs"));
        }

        [Fact]
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

            Assert.Equal(1, userInputDetectorMonitor.Parameters.Count(a => a.Name == "OffDelaySeconds" && a.Value == "600"));
            Assert.Equal(1, userInputDetectorBulb.Parameters.Count(a => a.Name == "OffDelaySeconds" && a.Value == "570"));

            Assert.Equal(1, soundDetectorMonitor.Parameters.Count(a => a.Name == "OffDelaySeconds" && a.Value == "600"));
            Assert.Equal(1, soundDetectorMonitor.Parameters.Count(a => a.Name == "MinimumSignalSeconds" && a.Value == "5"));
            Assert.Equal(1, soundDetectorAmplifier.Parameters.Count(a => a.Name == "OffDelaySeconds" && a.Value == "900"));
            Assert.Equal(1, soundDetectorAmplifier.Parameters.Count(a => a.Name == "MinimumSignalSeconds" && a.Value == "5"));
            Assert.Equal(1, soundDetectorHDDs.Parameters.Count(a => a.Name == "OffDelaySeconds" && a.Value == "600"));
            Assert.Equal(1, soundDetectorHDDs.Parameters.Count(a => a.Name == "MinimumSignalSeconds" && a.Value == "5"));

            Assert.Empty(trayIconMonitor.Parameters);
            Assert.Empty(trayIconBulb.Parameters);
            Assert.Empty(trayIconAmplifier.Parameters);
            Assert.Empty(trayIconHDDs.Parameters);
        }

        [Fact]
        public void Load_TestConditionsStartupAndShutdownState()
        {
            var config = getConfig();

            var startup = config.Conditions.Where(a => a.Type == ConditionType.Startup);
            var shutdown = config.Conditions.Where(a => a.Type == ConditionType.Shutdown);

            Assert.Equal(4, startup.Count());
            Assert.Equal(4, shutdown.Count());

            Assert.Equal(PowerStatus.On, startup.First(x => x.Socket.Name == "Monitor").ResultingStatus);
            Assert.Equal(PowerStatus.On, startup.First(x => x.Socket.Name == "Lampe").ResultingStatus);
            Assert.Equal(PowerStatus.Off, startup.First(x => x.Socket.Name == "Verstärker").ResultingStatus);
            Assert.Equal(PowerStatus.Off, startup.First(x => x.Socket.Name == "Externe HDDs").ResultingStatus);

            Assert.Equal(PowerStatus.On, shutdown.First(x => x.Socket.Name == "Monitor").ResultingStatus);
            Assert.Equal(PowerStatus.Off, shutdown.First(x => x.Socket.Name == "Lampe").ResultingStatus);
            Assert.Equal(PowerStatus.Off, shutdown.First(x => x.Socket.Name == "Verstärker").ResultingStatus);
            Assert.Equal(PowerStatus.Off, shutdown.First(x => x.Socket.Name == "Externe HDDs").ResultingStatus);
        }

        [Fact]
        public void Load_TestConditionsRegular()
        {
            var config = getConfig();

            var regular = config.Conditions.Where(a => a.Type == ConditionType.Regular);

            // TODO
        }

        [Fact]
        public void Load_Modes()
        {
            var config = getConfig();

            Assert.Equal(3, config.Modes.Count());
            Assert.Contains(new ConditionMode("Arbeit", true), config.Modes);
            Assert.Contains(new ConditionMode("Musik", true), config.Modes);
            Assert.Contains(new ConditionMode("Video", false), config.Modes);
        }

        private AppConfig getConfig()
        {
            var reader = new XmlAppConfigReader("_TestData\\config_valid.xml");
            reader.Validate();
            return reader.Load();
        }
    }
}
