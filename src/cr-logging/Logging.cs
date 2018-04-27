// <copyright file="Logging.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.Logging
{
    using System;
    using System.Configuration;
    using Microsoft.Extensions.Configuration;
    using Serilog;
    using Serilog.Core;
    using Serilog.Events;
    using Serilog.Formatting;

    /// <summary>
    /// Main Logging class. Holds the methods for setting up a SeriLog Logger.
    /// </summary>
    public static class Logging
    {
        private static IConfigurationRoot _appConfig;

        private delegate bool TryParse<in T1, T2>(T1 valueToParse, out T2 obj2);

        /// <summary>
        /// Function to setup the SeriLog Logger.
        /// </summary>
        /// <param name="appConfig">Optional IConfigurationRoot.</param>
        /// <returns>Configured Logger</returns>
        public static Logger SetupLogger(IConfigurationRoot appConfig = null)
        {
            _appConfig = appConfig;
            var logger = new LoggerConfiguration().MinimumLevel.Verbose();

            var jsonLoggingEnabled = ParseConfigValue<bool>("CR.Logging.Json.Enabled", bool.TryParse, false);
            if (jsonLoggingEnabled)
            {
                var jsonFileRotateOnFileSizeLimit = ParseConfigValue<bool>("CR.Logging.Json.RotateOnFileSizeLimit", bool.TryParse, false);
                var jsonLogFile = GetConfigString("CR.Logging.Json.FilePath", "./logs/log.json");
                var jsonMinLogLevel = ParseConfigValue<LogEventLevel>("CR.Logging.Json.MinLogLevel", Enum.TryParse, LogEventLevel.Debug);
                var jsonFileRotationTime = ParseConfigValue<RollingInterval>("CR.Logging.Json.FileRotationTime", Enum.TryParse, RollingInterval.Day);
                var jsonFileSizeLimit = ParseConfigValue<long>("CR.Logging.Json.FileRotationSizeLimit", long.TryParse, 26214400);
                logger.WriteToFile(new CrLogstashJsonFormatter(), jsonLogFile, jsonMinLogLevel, jsonFileRotationTime, jsonFileRotateOnFileSizeLimit, jsonFileSizeLimit);
            }

            var textLoggingEnabled = ParseConfigValue<bool>("CR.Logging.Text.Enabled", bool.TryParse, false);
            if (textLoggingEnabled)
            {
                var textFileRotateOnFileSizeLimit = ParseConfigValue<bool>("CR.Logging.Text.RotateOnFileSizeLimit", bool.TryParse, false);
                var textLogFile = GetConfigString("CR.Logging.Text.FilePath", "./logs/log.log");
                var textMinLogLevel = ParseConfigValue<LogEventLevel>("CR.Logging.Text.MinLogLevel", Enum.TryParse, LogEventLevel.Debug);
                var textFileRotationTime = ParseConfigValue<RollingInterval>("CR.Logging.Text.FileRotationTime", Enum.TryParse, RollingInterval.Day);
                var textFileSizeLimit = ParseConfigValue<long>("CR.Logging.Text.FileRotationSizeLimit", long.TryParse, 26214400);
                logger.WriteToFile(null, textLogFile, textMinLogLevel, textFileRotationTime, textFileRotateOnFileSizeLimit, textFileSizeLimit);
            }

            var consoleLoggingEnabled = ParseConfigValue<bool>("CR.Logging.Console.Enabled", bool.TryParse, false);
            if (consoleLoggingEnabled)
            {
                var consoleMinLogLevel = ParseConfigValue<LogEventLevel>("CR.Logging.Console.MinLogLevel", Enum.TryParse, LogEventLevel.Debug);
                logger.WriteTo.Console(consoleMinLogLevel);
            }

            return logger.CreateLogger();
        }

        private static void WriteToFile(this LoggerConfiguration loggerConfig, ITextFormatter formatter, string filePath, LogEventLevel logLevel, RollingInterval rollingInterval, bool rollOnFileSizeLimit, long fileSizeLimitBytes)
        {
            if (rollOnFileSizeLimit && fileSizeLimitBytes <= 0)
            {
                throw new ArgumentException("Cannot have rolling file and the size set to zero bytes.");
            }

            if (formatter == null)
            {
                loggerConfig.WriteTo.File(filePath, logLevel, rollingInterval: rollingInterval, rollOnFileSizeLimit: rollOnFileSizeLimit, fileSizeLimitBytes: fileSizeLimitBytes);
            }
            else
            {
                loggerConfig.WriteTo.File(formatter, filePath, logLevel, rollingInterval: rollingInterval, rollOnFileSizeLimit: rollOnFileSizeLimit, fileSizeLimitBytes: fileSizeLimitBytes);
            }
        }

        private static T ParseConfigValue<T>(string configName, TryParse<string, T> tryParse, T? defaultValue = null)
            where T : struct
        {
            var valueString = _appConfig == null ? ConfigurationManager.AppSettings[configName] : _appConfig[configName];
            if (defaultValue.HasValue)
            {
                if (string.IsNullOrWhiteSpace(valueString))
                {
                    return defaultValue.Value;
                }
            }
            else if (string.IsNullOrWhiteSpace(valueString))
            {
                throw new ArgumentNullException($"Please specify a {typeof(T).Name} value for {configName} in the App.config.", configName);
            }

            return tryParse(valueString, out var value) ? value : throw new ArgumentException($"Invalid {typeof(T).Name} value specified for {configName}.", configName);
        }

        private static string GetConfigString(string configName, string defaultValue = null)
        {
            var value = _appConfig == null ? ConfigurationManager.AppSettings[configName] : _appConfig[configName];
            return string.IsNullOrWhiteSpace(value) ? defaultValue ?? throw new ArgumentNullException($"Please specify a value for {configName} in the App.config.", configName) : value;
        }
    }
}
