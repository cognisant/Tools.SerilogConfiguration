// <copyright file="EventStoreSerilogLogger.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.Logging.EventStore
{
    using System;
    using global::EventStore.ClientAPI;
    using Serilog.Core;

    /// <summary>
    /// Extension to support logging of EventStore events.
    /// </summary>
    public class EventStoreSerilogLogger : ILogger
    {
        private readonly Logger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStoreSerilogLogger"/> class.
        /// </summary>
        /// <param name="logger">Serilog Logger</param>
        public EventStoreSerilogLogger(Logger logger) => _logger = logger;

        /// <summary>
        /// Log Error
        /// </summary>
        /// <param name="format">Format String</param>
        /// <param name="args">Arguments to log</param>
        public void Error(string format, params object[] args) => _logger.Error(format, args);

        /// <summary>
        /// Log Error
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <param name="format">Format String</param>
        /// <param name="args">Arguments to log</param>
        public void Error(Exception ex, string format, params object[] args) => _logger.Error(ex, format, args);

        /// <summary>
        /// Log Info
        /// </summary>
        /// <param name="format">Format String</param>
        /// <param name="args">Arguments to log</param>
        public void Info(string format, params object[] args) => _logger.Information(format, args);

        /// <summary>
        /// Log Info
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <param name="format">Format String</param>
        /// <param name="args">Arguments to log</param>
        public void Info(Exception ex, string format, params object[] args) => _logger.Information(ex, format, args);

        /// <summary>
        /// Log Debugging info.
        /// </summary>
        /// <param name="format">Format String</param>
        /// <param name="args">Arguments to log</param>
        public void Debug(string format, params object[] args) => _logger.Debug(format, args);

        /// <summary>
        /// Log debugging info.
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <param name="format">Format String</param>
        /// <param name="args">Arguments to log</param>
        public void Debug(Exception ex, string format, params object[] args) => _logger.Debug(ex, format, args);
    }
}
