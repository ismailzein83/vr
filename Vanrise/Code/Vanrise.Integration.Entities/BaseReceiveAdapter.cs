using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Integration.Entities
{
    public abstract class BaseReceiveAdapter
    {
        public abstract void ImportData(BaseAdapterArgument argument, Func<IImportedData, bool> receiveData);

        //public abstract bool IsCredentialsAvailable(string connectionString);

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
}
