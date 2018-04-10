﻿using CommandLine;
using CommandLine.Text;

namespace AnAusAutomat.ConsoleApplication.Models
{
    public class CommandLineOptions
    {
        [Option('c', "config", Required = false, DefaultValue = "config.xml", HelpText = "")]
        public string ConfigurationFile { get; set; }

        [Option('l', "minimumLogLevel", Required = false, DefaultValue = LogLevel.Information, HelpText = "")]
        public LogLevel MinimumLogLevel { get; set; }

        [Option('f', "logfile", Required = false, DefaultValue = "log.txt", HelpText = "")]
        public string LogFile { get; set; }

        [Option('h', "hidewindow", Required = false, DefaultValue = false, HelpText = "")]
        public bool HideConsoleWindow { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                Heading = new HeadingInfo("AnAusAutomat"),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };
            help.AddPreOptionsLine("Usage: AnAusAutomat -c config.xml -l Debug -...");
            help.AddOptions(this);
            return help;
        }
    }
}
