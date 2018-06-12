// <copyright file="EventStoreSerilogLogger.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.Tools.EventStoreSerilogLogger
{
    using System;
    using EventStore.ClientAPI;
    using Serilog.Core;

    /// <summary>
    /// A wrapper to enable passing Serilog Loggers configured via CR.Logging to an EventStore connection.
    /// </summary>
    public class EventStoreSerilogLogger : ILogger
    {
        private readonly Logger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStoreSerilogLogger"/> class, which can be used to pass a Serilog logger configured via CR.Logging to an EventStore connection.
        /// </summary>
        /// <param name="logger">Serilog Logger</param>
        // ReSharper disable once UnusedMember.Global
        public EventStoreSerilogLogger(Logger logger) => _logger = logger;

        /// <summary>
        /// Log an error to the Serilog Logger with the provided format string and arguments.
        /// </summary>
        /// <param name="format">Format String</param>
        /// <param name="args">Arguments to log</param>
        public void Error(string format, params object[] args) => _logger.Error(format, args);

        /// <summary>
        /// Log an error to the Serilog Logger with the provided exception, format string and arguments.
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <param name="format">Format String</param>
        /// <param name="args">Arguments to log</param>
        public void Error(Exception ex, string format, params object[] args) => _logger.Error(ex, format, args);

        /// <summary>
        /// Log information to the Serilog Logger with the provided format string and arguments.
        /// </summary>
        /// <param name="format">Format String</param>
        /// <param name="args">Arguments to log</param>
        public void Info(string format, params object[] args) => _logger.Information(format, args);

        /// <summary>
        /// Log information to the Serilog Logger with the provided exception, format string and arguments.
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <param name="format">Format String</param>
        /// <param name="args">Arguments to log</param>
        public void Info(Exception ex, string format, params object[] args) => _logger.Information(ex, format, args);

        /// <summary>
        /// Log debug information to the Serilog Logger with the provided format string and arguments.
        /// </summary>
        /// <param name="format">Format String</param>
        /// <param name="args">Arguments to log</param>
        public void Debug(string format, params object[] args) => _logger.Debug(format, args);

        /// <summary>
        /// Log debug information to the Serilog Logger with the provided exception, format string and arguments.
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <param name="format">Format String</param>
        /// <param name="args">Arguments to log</param>
        public void Debug(Exception ex, string format, params object[] args) => _logger.Debug(ex, format, args);
    }
}
