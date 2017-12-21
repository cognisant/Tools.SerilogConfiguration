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
            var logger = new LoggerConfiguration();

            var jsonLoggingEnabled = ConfigurationManager.AppSettings["CR.Logging.Json.Enabled"] != null && Convert.ToBoolean(ConfigurationManager.AppSettings["CR.Logging.Json.Enabled"]);
            var jsonMinLogLevel = LogEventLevel.Debug;
            LogEventLevel.TryParse(ConfigurationManager.AppSettings["CR.Logging.Json.MinLogLevel"], out jsonMinLogLevel);
            var jsonLogFile = ConfigurationManager.AppSettings["CR.Logging.Json.FilePath"] == null ? "./logs/log.json" : ConfigurationManager.AppSettings["CR.Logging.Json.FilePath"];
            var jsonFileRotationTime = RollingInterval.Day;
            RollingInterval.TryParse(ConfigurationManager.AppSettings["CR.Logging.Json.FileRotationTime"], out jsonFileRotationTime);
            var jsonFileRotateOnFileSizeLimit = ConfigurationManager.AppSettings["CR.Logging.Json.RotateOnFileSizeLimit"] != null && Convert.ToBoolean(ConfigurationManager.AppSettings["CR.Logging.Json.RotateOnFileSizeLimit"]);
            var jsonFileSizeLimit = ConfigurationManager.AppSettings["CR.Logging.Json.FileRotationSizeLimit"] == null ? 26214400 : Convert.ToInt64(ConfigurationManager.AppSettings["CR.Logging.Json.FileRotationSizeLimit"]);
            var textLoggingEnabled = ConfigurationManager.AppSettings["CR.Logging.Text.Enabled"] != null && Convert.ToBoolean(ConfigurationManager.AppSettings["CR.Logging.Text.Enabled"]);
            var textMinLogLevel = LogEventLevel.Debug;
            LogEventLevel.TryParse(ConfigurationManager.AppSettings["CR.Logging.Text.MinLogLevel"], out textMinLogLevel);
            var textLogFile = ConfigurationManager.AppSettings["CR.Logging.Text.FilePath"] == null ? "./logs/log.log" : ConfigurationManager.AppSettings["CR.Logging.Text.FilePath"];
            var textFileRotationTime = RollingInterval.Day;
            RollingInterval.TryParse(ConfigurationManager.AppSettings["CR.Logging.Text.FileRotationTime"], out textFileRotationTime);
            var textFileRotarteOnFileSizeLimit = ConfigurationManager.AppSettings["CR.Logging.Text.RotateOnFileSizeLimit"] != null && Convert.ToBoolean(ConfigurationManager.AppSettings["CR.Logging.Text.RotateOnFileSizeLimit"]);
            var textFileSizeLimit = ConfigurationManager.AppSettings["CR.Logging.Text.FileRotationSizeLimit"] == null ? 26214400 : Convert.ToInt64(ConfigurationManager.AppSettings["CR.Logging.Text.FileRotationSizeLimit"]);
            var consoleLoggingEnabled =ConfigurationManager.AppSettings["CR.Logging.Console.Enabled"] != null && Convert.ToBoolean(ConfigurationManager.AppSettings["CR.Logging.Console.Enabled"]);
            var consoleMinLogLevel = LogEventLevel.Debug;
            LogEventLevel.TryParse(ConfigurationManager.AppSettings["CR.Logging.Console.MinLogLevel"], out consoleMinLogLevel);

            if (jsonLoggingEnabled)
                logger.WriteToFile(new CrLogstashJsonFormatter(), jsonLogFile,jsonMinLogLevel, jsonFileRotationTime, jsonFileRotateOnFileSizeLimit,jsonFileSizeLimit);

            if (textLoggingEnabled)
                logger.WriteToFile(null, textLogFile,textMinLogLevel,textFileRotationTime,textFileRotarteOnFileSizeLimit,textFileSizeLimit);

            if (consoleLoggingEnabled)
                logger.WriteTo.Console(consoleMinLogLevel);

            return logger.CreateLogger();
        }

        private static void WriteToFile(this LoggerConfiguration loggerConfig,ITextFormatter formatter, string filePath,LogEventLevel logLevel, RollingInterval rollingInterval,bool rollOnFileSizeLimit, long fileSizeLimitBytes)
        {
            if(rollOnFileSizeLimit && fileSizeLimitBytes <= 0)
                throw new ArgumentException("Cannot have rolling file and the size set to zero bytes.");

            if (formatter == null)
                loggerConfig.WriteTo.File(filePath, logLevel,
                    rollingInterval: rollingInterval, rollOnFileSizeLimit: rollOnFileSizeLimit,
                    fileSizeLimitBytes: fileSizeLimitBytes);
            else
                loggerConfig.WriteTo.File(formatter, filePath, logLevel,
                    rollingInterval: rollingInterval, rollOnFileSizeLimit: rollOnFileSizeLimit,
                    fileSizeLimitBytes: fileSizeLimitBytes);
        }
    }
}
