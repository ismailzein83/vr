using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.Integration.Entities
{    
    public abstract class DataMapper
    {
        public abstract MappingOutput MapData(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers);

        private IDataSourceLogger _logger;
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

    public enum MappingResult
    {
        Valid = 1,
        Invalid = 2,
        Empty = 3, 
        PartialInvalid = 4
    }
    public class MappingOutput
    {
        public MappingResult Result { get; set; }

        public string Message { get; set; }
    }
}
