using AnAusAutomat.ConsoleApplication.Adapters;
using AnAusAutomat.ConsoleApplication.Models;
using AnAusAutomat.Core;
using AnAusAutomat.Core.Configuration;
using Serilog;
using Serilog.Events;
using System;
using System.Threading;
using System.Windows.Forms;

namespace AnAusAutomat.ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var commandLineOptions = parseCommandLineOptions(args);
            setConsoleOptions(commandLineOptions.HideConsoleWindow);
            //initializeLogger(commandLineOptions.LogLevel, commandLineOptions.LogFile);
            initializeLogger(LogLevel.Verbose, commandLineOptions.LogFile);

            var configuration = loadConfigurationAndExitApplicationOnError(commandLineOptions.ConfigurationFile);

            var app = new App(configuration);
            app.Initialize();
            app.Start();

            Console.CancelKeyPress += (sender, e) => { app.Stop(); };

            Application.Run();
        }

        private static void setConsoleOptions(bool hideConsoleWindow)
        {
            Console.Title = "AnAusAutomat";

            if (hideConsoleWindow)
            {
                WinAPI.HideConsoleWindow();
            }
            else
            {
                Console.WindowWidth = (int)(Console.LargestWindowWidth * 0.9);
                Console.WindowHeight = (int)(Console.LargestWindowHeight * 0.6);
            }
        }

        private static CommandLineOptions parseCommandLineOptions(string[] args)
        {
            var options = new CommandLineOptions();
            CommandLine.Parser.Default.ParseArguments(args, options);

            Console.Clear();

            return options;
        }

        private static AppConfig loadConfigurationAndExitApplicationOnError(string configurationFile)
        {
            var configuration = new XmlAppConfigReader(configurationFile);

            if (!configuration.Validate())
            {
                Log.Fatal("Invalid configuration. Exit application.");
                Thread.Sleep(2000);
                Application.Exit();

                return null;
            }

            configuration.Load();

            return configuration;
        }

        private static void initializeLogger(LogLevel logLevel, string logFile)
        {
            var logEventLevel = (LogEventLevel)Enum.Parse(typeof(LogEventLevel), logLevel.ToString(), true);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Is(logEventLevel)
                .WriteTo.ColoredConsole()
                .WriteTo.File(logFile)
                .CreateLogger();
        }
    }
}
