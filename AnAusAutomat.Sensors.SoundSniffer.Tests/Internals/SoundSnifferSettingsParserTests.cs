using AnAusAutomat.Contracts.Sensor;
using AnAusAutomat.Sensors.SoundSniffer.Internals;
using System;
using System.Collections.Generic;
using Xunit;

namespace AnAusAutomat.Sensors.SoundSniffer.Tests.Internals
{
    public class SoundSnifferSettingsParserTests
    {
        [Fact]
        public void Parse_OffDelay()
        {
            var parser = new SoundSnifferSettingsParser();
            var result = parser.ParseSocketSettings(new List<SensorParameter>()
            {
                new SensorParameter("OffDelaySeconds", "60")
            });

            Assert.Equal(TimeSpan.FromSeconds(60), result.OffDelay);
        }

        [Fact]
        public void Parse_MinimumSignalDuration()
        {
            var parser = new SoundSnifferSettingsParser();
            var result = parser.ParseSocketSettings(new List<SensorParameter>()
            {
                new SensorParameter("MinimumSignalSeconds", "5")
            });

            Assert.Equal(TimeSpan.FromSeconds(5), result.MinimumSignalDuration);
        }

        [Fact]
        public void Parse_OffDelayNotDefined()
        {
            var defaultSettings = SoundSocketSnifferSettings.GetDefault();
            var parser = new SoundSnifferSettingsParser();
            var result = parser.ParseSocketSettings(new List<SensorParameter>());

            Assert.Equal(defaultSettings.OffDelay, result.OffDelay);
        }

        [Fact]
        public void Parse_MinimumSignalDurationNotDefined()
        {
            var defaultSettings = SoundSocketSnifferSettings.GetDefault();
            var parser = new SoundSnifferSettingsParser();
            var result = parser.ParseSocketSettings(new List<SensorParameter>());

            Assert.Equal(defaultSettings.MinimumSignalDuration, result.MinimumSignalDuration);
        }

        [Fact]
        public void Parse_OffDelayMultipleDefined()
        {
            var defaultSettings = SoundSocketSnifferSettings.GetDefault();
            var parser = new SoundSnifferSettingsParser();
            var result = parser.ParseSocketSettings(new List<SensorParameter>()
            {
                new SensorParameter("OffDelaySeconds", "60"),
                new SensorParameter("OffDelaySeconds", "90")
            });

            Assert.Equal(defaultSettings.OffDelay, result.OffDelay);
        }

        [Fact]
        public void Parse_MinimumSignalDurationMultipleDefined()
        {
            var defaultSettings = SoundSocketSnifferSettings.GetDefault();
            var parser = new SoundSnifferSettingsParser();
            var result = parser.ParseSocketSettings(new List<SensorParameter>()
            {
                new SensorParameter("MinimumSignalSeconds", "6"),
                new SensorParameter("MinimumSignalSeconds", "2")
            });

            Assert.Equal(defaultSettings.MinimumSignalDuration, result.MinimumSignalDuration);
        }

        [Fact]
        public void Parse_OffDelayInvalidValue()
        {
            var defaultSettings = SoundSocketSnifferSettings.GetDefault();
            var parser = new SoundSnifferSettingsParser();
            var result = parser.ParseSocketSettings(new List<SensorParameter>()
            {
                new SensorParameter("OffDelaySeconds", "-1")
            });

            Assert.Equal(defaultSettings.OffDelay, result.OffDelay);
        }

        [Fact]
        public void Parse_MinimumSignalDurationInvalidValue()
        {
            var defaultSettings = SoundSocketSnifferSettings.GetDefault();
            var parser = new SoundSnifferSettingsParser();
            var result = parser.ParseSocketSettings(new List<SensorParameter>()
            {
                new SensorParameter("MinimumSignalSeconds", "-1")
            });

            Assert.Equal(defaultSettings.MinimumSignalDuration, result.MinimumSignalDuration);
        }
    }
}
