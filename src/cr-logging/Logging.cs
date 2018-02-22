using System;
using System.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;

namespace CR.Logging
{
    public static class Logging
    {
        public static Logger SetupLogger()
        {
            var logger = new LoggerConfiguration().MinimumLevel.Verbose();

            #region Optional Fields

            var consoleLoggingEnabled = ParseConfigValue<bool>("CR.Logging.Console.Enabled", bool.TryParse, false);

            #region Json
            var jsonLoggingEnabled = ParseConfigValue<bool>("CR.Logging.Json.Enabled", bool.TryParse, false);

            var jsonFileRotateOnFileSizeLimit = ParseConfigValue<bool>("CR.Logging.Json.RotateOnFileSizeLimit", bool.TryParse, false);

            var jsonLogFile = GetConfigString("CR.Logging.Json.FilePath", "./logs/log.json");

            var jsonMinLogLevel = ParseConfigValue<LogEventLevel>("CR.Logging.Json.MinLogLevel", Enum.TryParse, LogEventLevel.Debug);

            var jsonFileRotationTime = ParseConfigValue<RollingInterval>("CR.Logging.Json.FileRotationTime", Enum.TryParse, RollingInterval.Day);

            var jsonFileSizeLimit = ParseConfigValue<long>("CR.Logging.Json.FileRotationSizeLimit", long.TryParse, 26214400);
            #endregion

            #region Text
            var textLoggingEnabled = ParseConfigValue<bool>("CR.Logging.Text.Enabled", bool.TryParse, false);

            var textFileRotateOnFileSizeLimit = ParseConfigValue<bool>("CR.Logging.Text.RotateOnFileSizeLimit", bool.TryParse, false);

            var textLogFile = GetConfigString("CR.Logging.Text.FilePath", "./logs/log.log");

            var textMinLogLevel = ParseConfigValue<LogEventLevel>("CR.Logging.Text.MinLogLevel", Enum.TryParse, LogEventLevel.Debug);

            var textFileRotationTime = ParseConfigValue<RollingInterval>("CR.Logging.Text.FileRotationTime", Enum.TryParse, RollingInterval.Day);

            var textFileSizeLimit = ParseConfigValue<long>("CR.Logging.Text.FileRotationSizeLimit", long.TryParse, 26214400);
            #endregion

            var consoleMinLogLevel = ParseConfigValue<LogEventLevel>("CR.Logging.Console.MinLogLevel", Enum.TryParse, LogEventLevel.Debug);
            #endregion

            if (jsonLoggingEnabled) logger.WriteToFile(new CrLogstashJsonFormatter(), jsonLogFile, jsonMinLogLevel, jsonFileRotationTime, jsonFileRotateOnFileSizeLimit, jsonFileSizeLimit);
            if (textLoggingEnabled) logger.WriteToFile(null, textLogFile, textMinLogLevel, textFileRotationTime, textFileRotateOnFileSizeLimit, textFileSizeLimit);
            if (consoleLoggingEnabled) logger.WriteTo.Console(consoleMinLogLevel);

            return logger.CreateLogger();
        }

        private static void WriteToFile(this LoggerConfiguration loggerConfig, ITextFormatter formatter, string filePath, LogEventLevel logLevel, RollingInterval rollingInterval, bool rollOnFileSizeLimit, long fileSizeLimitBytes)
        {
            if (rollOnFileSizeLimit && fileSizeLimitBytes <= 0) throw new ArgumentException("Cannot have rolling file and the size set to zero bytes.");
            if (formatter == null) loggerConfig.WriteTo.File(filePath, logLevel, rollingInterval: rollingInterval, rollOnFileSizeLimit: rollOnFileSizeLimit, fileSizeLimitBytes: fileSizeLimitBytes);
            else loggerConfig.WriteTo.File(formatter, filePath, logLevel, rollingInterval: rollingInterval, rollOnFileSizeLimit: rollOnFileSizeLimit, fileSizeLimitBytes: fileSizeLimitBytes);
        }

        private delegate bool TryParse<in T1, T2>(T1 valueToParse, out T2 obj2);

        private static T ParseConfigValue<T>(string configName, TryParse<string, T> tryParse, T? defaultValue = null) where T : struct
        {
            var valueString = ConfigurationManager.AppSettings[configName];
            if (defaultValue.HasValue)
            {
                if (string.IsNullOrWhiteSpace(valueString)) return defaultValue.Value;
            }
            else if (string.IsNullOrWhiteSpace(valueString)) throw new ArgumentNullException($"Please specify a {typeof(T).Name} value for {configName} in the App.config.", configName);
            return tryParse(valueString, out var value) ? value : throw new ArgumentException($"Invalid {typeof(T).Name} value specified for {configName}.", configName);
        }

        private static string GetConfigString(string configName, string defaultValue = null)
        {
            var value = ConfigurationManager.AppSettings[configName];
            return string.IsNullOrWhiteSpace(value) ? defaultValue ?? throw new ArgumentNullException($"Please specify a value for {configName} in the App.config.", configName) : value;
        }
    }
}
