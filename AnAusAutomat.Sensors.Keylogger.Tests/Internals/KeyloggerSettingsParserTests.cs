using AnAusAutomat.Contracts.Sensor;
using AnAusAutomat.Sensors.Keylogger.Internals;
using System;
using System.Collections.Generic;
using Xunit;

namespace AnAusAutomat.Sensors.Keylogger.Tests.Internals
{
    public class KeyloggerSettingsParserTests
    {
        [Fact]
        public void Parse_OffDelay()
        {
            var parser = new KeyloggerSettingsParser();
            var result = parser.Parse(new List<SensorParameter>()
            {
                new SensorParameter("OffDelaySeconds", "60")
            });

            Assert.Equal(TimeSpan.FromSeconds(60), result.OffDelay);
        }

        [Fact]
        public void Parse_OffDelayNotDefined()
        {
            var defaultSettings = KeyloggerSettings.GetDefault();
            var parser = new KeyloggerSettingsParser();
            var result = parser.Parse(new List<SensorParameter>());

            Assert.Equal(defaultSettings.OffDelay, result.OffDelay);
        }

        [Fact]
        public void Parse_OffDelayMultipleDefined()
        {
            var defaultSettings = KeyloggerSettings.GetDefault();
            var parser = new KeyloggerSettingsParser();
            var result = parser.Parse(new List<SensorParameter>()
            {
                new SensorParameter("OffDelaySeconds", "60"),
                new SensorParameter("OffDelaySeconds", "90")
            });

            Assert.Equal(defaultSettings.OffDelay, result.OffDelay);
        }

        [Fact]
        public void Parse_OffDelayInvalidValue()
        {
            var defaultSettings = KeyloggerSettings.GetDefault();
            var parser = new KeyloggerSettingsParser();
            var result = parser.Parse(new List<SensorParameter>()
            {
                new SensorParameter("OffDelaySeconds", "-1")
            });

            Assert.Equal(defaultSettings.OffDelay, result.OffDelay);
        }
    }
}
