using System;
using System.Collections.Generic;
using System.ComponentModel;
using Vanrise.Entities;

namespace Vanrise.Integration.Entities
{
    public abstract class DataMapper
    {
        private IDataSourceLogger _logger;

        public abstract MappingOutput MapData(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers);

        public void SetLogger(IDataSourceLogger logger)
        {
            _logger = logger;
        }

        protected void LogError(string messageFormat, params object[] args)
        {
            LogEntry(LogEntryType.Error, messageFormat, args);
        }

        protected void LogWarning(string messageFormat, params object[] args)
        {
            LogEntry(LogEntryType.Warning, messageFormat, args);
        }

        protected void LogInformation(string messageFormat, params object[] args)
        {
            LogEntry(LogEntryType.Information, messageFormat, args);
        }

        protected void LogVerbose(string messageFormat, params object[] args)
        {
            LogEntry(LogEntryType.Verbose, messageFormat, args);
        }

        private void LogEntry(LogEntryType logEntryType, string messageForamt, params object[] args)
        {
            if (_logger != null)
                _logger.LogEntry(logEntryType, messageForamt, args);
        }
    }

    public class MappingOutput
    {
        public MappingResult Result { get; set; }

        public string Message { get; set; }
    }

    public enum MappingResult
    {
        [Description("None")]
        None = 0,
        [Description("Valid")]
        Valid = 1,
        [Description("Invalid")]
        Invalid = 2,
        [Description("Empty")]
        Empty = 3,
        [Description("Partial Invalid")]
        PartialInvalid = 4
    }
}