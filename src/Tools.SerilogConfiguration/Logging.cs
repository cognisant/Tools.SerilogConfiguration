﻿// <copyright file="Logging.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.Tools.SerilogConfiguration
{
    using System;
    using System.Configuration;
    using Microsoft.Extensions.Configuration;
    using Serilog;
    using Serilog.Core;
    using Serilog.Events;
    using Serilog.Formatting;

    /// <summary>
    /// The Logging helper class which can be used to initialize a pre-configured Serilog <see cref="Logger"/> instance.
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public static class Logging
    {
        private delegate bool TryParse<in T1, T2>(T1 valueToParse, out T2 obj2);

        /// <summary>
        /// Build a new Serilog <see cref="Logger"/> with the provided configuration.
        /// </summary>
        /// <param name="configuration">An optional <see cref="IConfiguration"/> to read the Serilog <see cref="Logger"/> configuration from; if this is null (default), the configuration will be read from the App.Config.</param>
        /// <returns>A Serilog <see cref="Logger"/> configured according to the passed in (or, if none is passed in, the app.config) configuration.</returns>
        /// <exception cref="ArgumentException">Thrown when a configuration variable has been set incorrectly (not missing, empty or whitespace).</exception>
        /// <exception cref="ArgumentNullException">Thrown when a configuration variableis missing, or is empty or whitespace, and no default is specified.</exception>
        // ReSharper disable once UnusedMember.Global
        public static Logger SetupLogger(IConfiguration configuration = null)
        {
            var logger = new LoggerConfiguration().MinimumLevel.Verbose();
            var jsonLoggingEnabled = ParseConfigValue<bool>(configuration, "CR.Logging.Json.Enabled", bool.TryParse, false);

            if (jsonLoggingEnabled)
            {
                var jsonFileRotateOnFileSizeLimit = ParseConfigValue<bool>(configuration, "CR.Logging.Json.RotateOnFileSizeLimit", bool.TryParse, false);
                var jsonLogFile = GetConfigString(configuration, "CR.Logging.Json.FilePath", "./logs/log.json");
                var jsonMinLogLevel = ParseConfigValue<LogEventLevel>(configuration, "CR.Logging.Json.MinLogLevel", Enum.TryParse, LogEventLevel.Debug);
                var jsonFileRotationTime = ParseConfigValue<RollingInterval>(configuration, "CR.Logging.Json.FileRotationTime", Enum.TryParse, RollingInterval.Day);
                var jsonFileSizeLimit = ParseConfigValue<long>(configuration, "CR.Logging.Json.FileRotationSizeLimit", long.TryParse, 26214400);
                logger.WriteToFile(new CrLogstashJsonFormatter(), jsonLogFile, jsonMinLogLevel, jsonFileRotationTime, jsonFileRotateOnFileSizeLimit, jsonFileSizeLimit);
            }

            var textLoggingEnabled = ParseConfigValue<bool>(configuration, "CR.Logging.Text.Enabled", bool.TryParse, false);
            if (textLoggingEnabled)
            {
                var textFileRotateOnFileSizeLimit = ParseConfigValue<bool>(configuration, "CR.Logging.Text.RotateOnFileSizeLimit", bool.TryParse, false);
                var textLogFile = GetConfigString(configuration, "CR.Logging.Text.FilePath", "./logs/log.log");
                var textMinLogLevel = ParseConfigValue<LogEventLevel>(configuration, "CR.Logging.Text.MinLogLevel", Enum.TryParse, LogEventLevel.Debug);
                var textFileRotationTime = ParseConfigValue<RollingInterval>(configuration, "CR.Logging.Text.FileRotationTime", Enum.TryParse, RollingInterval.Day);
                var textFileSizeLimit = ParseConfigValue<long>(configuration, "CR.Logging.Text.FileRotationSizeLimit", long.TryParse, 26214400);
                logger.WriteToFile(null, textLogFile, textMinLogLevel, textFileRotationTime, textFileRotateOnFileSizeLimit, textFileSizeLimit);
            }

            var consoleLoggingEnabled = ParseConfigValue<bool>(configuration, "CR.Logging.Console.Enabled", bool.TryParse, false); // ReSharper disable once InvertIf
            if (consoleLoggingEnabled)
            {
                var consoleMinLogLevel = ParseConfigValue<LogEventLevel>(configuration, "CR.Logging.Console.MinLogLevel", Enum.TryParse, LogEventLevel.Debug);
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

        private static T ParseConfigValue<T>(IConfiguration configuration, string configName, TryParse<string, T> tryParse, T? defaultValue = null)
            where T : struct
        {
            var valueString = configuration == null ? ConfigurationManager.AppSettings[configName] : configuration[configName];
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

        private static string GetConfigString(IConfiguration configuration, string configName, string defaultValue = null)
        {
            var value = configuration == null ? ConfigurationManager.AppSettings[configName] : configuration[configName];
            return string.IsNullOrWhiteSpace(value) ? defaultValue ?? throw new ArgumentNullException($"Please specify a value for {configName} in the App.config.", configName) : value;
        }
    }
}