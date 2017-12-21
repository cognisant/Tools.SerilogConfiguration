﻿using System;
using System.Collections.Generic;
using System.IO;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Json;

namespace CR.Logging
{
    public class CrLogstashJsonFormatter : ITextFormatter
    {
        private static readonly JsonValueFormatter ValueFormatter = new JsonValueFormatter();

        public void Format(LogEvent logEvent, TextWriter output)
        {
            FormatContent(logEvent, output);
            output.WriteLine();
        }

        private static void FormatContent(LogEvent logEvent, TextWriter output)
        {
            if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
            if (output == null) throw new ArgumentNullException(nameof(output));

            output.Write('{');

            WritePropertyAndValue(output, "timestamp", logEvent.Timestamp.ToString("o"));
            WritePropertyAndValue(output, "level", logEvent.Level.ToString());
            WritePropertyAndValue(output, "message", logEvent.MessageTemplate.Render(logEvent.Properties));

            if (logEvent.Exception != null)
            {
                WritePropertyAndValue(output, "exception", logEvent.Exception.ToString());
            }

            WriteProperties(logEvent.Properties, output);

            output.Write('}');
        }

        private static void WritePropertyAndValue(TextWriter output, string propertyKey, string propertyValue)
        {
            JsonValueFormatter.WriteQuotedJsonString(propertyKey, output);
            output.Write(":");
            JsonValueFormatter.WriteQuotedJsonString(propertyValue, output);
            output.Write(",");
        }

        private static void WriteProperties(IReadOnlyDictionary<string, LogEventPropertyValue> properties, TextWriter output)
        {
            var precedingDelimiter = "";
            foreach (var property in properties)
            {
                output.Write(precedingDelimiter);
                precedingDelimiter = ",";

                var camelCasePropertyKey = property.Key[0].ToString().ToLower() + property.Key.Substring(1);
                JsonValueFormatter.WriteQuotedJsonString(camelCasePropertyKey, output);
                output.Write(':');
                ValueFormatter.Format(property.Value, output);
            }
        }
    }
}