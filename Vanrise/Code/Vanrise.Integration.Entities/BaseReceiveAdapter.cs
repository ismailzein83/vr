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
        public abstract void ImportData(BaseAdapterArgument argument, Action<IImportedData> receiveData);

        //public abstract bool IsCredentialsAvailable(string connectionString);

        private IDataSourceLogger _logger;
        public void SetLogger(IDataSourceLogger logger)
        {
            _logger = logger;
        }

        protected void LogError(string error)
        {
            LogEntry(LogEntryType.Error, error);
        }

        protected void LogWarning(string warning)
        {
            LogEntry(LogEntryType.Warning, warning);
        }

        protected void LogInformation(string message)
        {
            LogEntry(LogEntryType.Information, message);
        }

        protected void LogVerbose(string message)
        {
            LogEntry(LogEntryType.Verbose, message);
        }

        void LogEntry(LogEntryType logEntryType, string message)
        {
            if (_logger != null)
                _logger.LogEntry(logEntryType, message);
        }
    }
}
