using EventStore.ClientAPI;
using Serilog.Core;
using System;

namespace CR.Logging.EventStore
{
    public class EventStoreSerilogLogger : ILogger
    {
        private readonly Logger _logger;

        public EventStoreSerilogLogger(Logger logger) => _logger = logger;

        public void Error(string format, params object[] args) => _logger.Error(format, args);

        public void Error(Exception ex, string format, params object[] args) => _logger.Error(ex, format, args);

        public void Info(string format, params object[] args) => _logger.Information(format, args);

        public void Info(Exception ex, string format, params object[] args) => _logger.Information(ex, format, args);

        public void Debug(string format, params object[] args) => _logger.Debug(format, args);

        public void Debug(Exception ex, string format, params object[] args) => _logger.Debug(ex, format, args);
    }
}
