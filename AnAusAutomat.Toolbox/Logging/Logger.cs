using Serilog;
using System;

namespace AnAusAutomat.Toolbox.Logging
{
    public static class Logger
    {
        public static void Verbose(string messageTemplate)
        {
            Log.Verbose(messageTemplate);
        }

        public static void Verbose(Exception exception, string messageTemplate)
        {
            Log.Verbose(exception, messageTemplate);
        }

        public static void Debug(string messageTemplate)
        {
            Log.Debug(messageTemplate);
        }

        public static void Debug(Exception exception, string messageTemplate)
        {
            Log.Debug(exception, messageTemplate);
        }

        public static void Information(string messageTemplate)
        {
            Log.Information(messageTemplate);
        }

        public static void Information(Exception exception, string messageTemplate)
        {
            Log.Information(exception, messageTemplate);
        }

        public static void Warning(string messageTemplate)
        {
            Log.Warning(messageTemplate);
        }

        public static void Warning(Exception exception, string messageTemplate)
        {
            Log.Warning(exception, messageTemplate);
        }

        public static void Error(string messageTemplate)
        {
            Log.Error(messageTemplate);
        }

        public static void Error(Exception exception, string messageTemplate)
        {
            Log.Error(exception, messageTemplate);
        }

        public static void Fatal(string messageTemplate)
        {
            Log.Fatal(messageTemplate);
        }

        public static void Fatal(Exception exception, string messageTemplate)
        {
            Log.Fatal(exception, messageTemplate);
        }
    }
}
