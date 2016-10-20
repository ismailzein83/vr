using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.Integration.Entities
{
    public abstract class BaseReceiveAdapter
    {        
        //public abstract bool IsCredentialsAvailable(string connectionString);
        public abstract void ImportData(IAdapterImportDataContext context);

        private IDataSourceLogger _logger;
        private IDataSourceManager _dataSourceManager;

        public void SetLogger(IDataSourceLogger logger)
        {
            _logger = logger;
        }

        public void SetDataSourceManager(IDataSourceManager manager)
        {
            _dataSourceManager = manager;
        }

        protected bool UpdateAdapterState(Guid dataSourceId, Vanrise.Integration.Entities.BaseAdapterState adapterState)
        {
            return _dataSourceManager.UpdateAdapterState(dataSourceId, adapterState);
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
