using AnAusAutomat.Core;
using AnAusAutomat.Core.Configuration;
using Serilog;
using Serilog.Events;
using System;
using System.Windows.Forms;

namespace AnAusAutomat.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            setConsoleOptions();

            var commandLineOptions = parseCommandLineOptions(args);
            initializeLogger(commandLineOptions.MinimumLogLevel, commandLineOptions.LogFile);

            var appConfig = loadConfigurationOrExitApplicationOnError(commandLineOptions.ConfigurationFile);

            var app = AppFactory.Create(appConfig);
            app.Initialize();
            app.Start();

            Console.CancelKeyPress += (sender, e) => { app.Stop(); };

            Application.Run();
        }

        private static void setConsoleOptions()
        {
            Console.Title = "AnAusAutomat";
            Console.WindowWidth = (int)(Console.LargestWindowWidth * 0.9);
            Console.WindowHeight = (int)(Console.LargestWindowHeight * 0.6);
        }

        private static CommandLineOptions parseCommandLineOptions(string[] args)
        {
            var options = new CommandLineOptions();
            CommandLine.Parser.Default.ParseArguments(args, options);

            Console.Clear();

            return options;
        }

        private static AppConfig loadConfigurationOrExitApplicationOnError(string configFilePath)
        {
            var configuration = new XmlAppConfigReader(configFilePath);

            if (!configuration.Validate())
            {
                Log.Fatal("Invalid configuration. Exit application.");
                Application.Exit();

                return null;
            }

            return configuration.Load();
        }

        private static void initializeLogger(LogLevel minimumLogLevel, string logFile)
        {
            var logEventLevel = (LogEventLevel)Enum.Parse(typeof(LogEventLevel), minimumLogLevel.ToString(), true);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Is(logEventLevel)
                .WriteTo.ColoredConsole()
                .WriteTo.File(logFile)
                .CreateLogger();
        }
    }
}
