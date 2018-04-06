using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.Logging.SQL
{
    public class SQLLogger : LogHandler
    {
        Timer _timer;
        ConcurrentQueue<LogEntry> _qLogEntries = new ConcurrentQueue<LogEntry>();
        string _machineName;
        string _applicationName;

        string _loggingConnectionStringKey;
        string _configurationConnectionStringKey;
        public string LoggingConnectionStringKey
        {
            set
            {
                _loggingConnectionStringKey = value;
            }
        }
        public string ConfigurationConnectionStringKey
        {
            set
            {
                _configurationConnectionStringKey = value;
            }
        }

        SQLDataManager _loggingDataManager;

        public SQLDataManager LoggingDataManager
        {
            get
            {
                if (_loggingDataManager == null)
                {
                    if (String.IsNullOrEmpty(_loggingConnectionStringKey))
                        throw new Exception("Connection String is not provided for the SQLLogger. ConnectionStringKey should be set in the parameters section");
                    _loggingDataManager = new SQLDataManager(_loggingConnectionStringKey);
                }
                return _loggingDataManager;
            }
        }

        LogAttributeDataManager _configurationDataManager;
        public LogAttributeDataManager ConfigurationDataManager
        {
            get
            {
                if (_configurationDataManager == null)
                {
                    if (String.IsNullOrEmpty(_configurationConnectionStringKey))
                        throw new Exception("Connection String is not provided for the SQLLogger. ConnectionStringKey should be set in the parameters section");
                    _configurationDataManager = new LogAttributeDataManager(_configurationConnectionStringKey);
                }
                return _configurationDataManager;
            }
        }
        public SQLLogger()
        {
            _machineName = Environment.MachineName;
            _applicationName = Process.GetCurrentProcess().ProcessName;
        }
       

        protected override void WriteEntry(string eventType, int? viewRequiredPermissionSetId, string exceptionDetail, LogEntryType entryType, string message, string callingModule, string callingType, string callingMethod)
        {
            try
            {
                this.ConfigurationDataManager.LoadLogAttributesIfNotLoaded();
                this.LoggingDataManager.AddEntry(this.ConfigurationDataManager.GetAttributeId, _machineName, _applicationName, callingModule, callingType, callingMethod, entryType,
                    eventType, viewRequiredPermissionSetId, message, exceptionDetail, DateTime.Now);
            }
            catch(Exception ex)
            {
                Common.HandleLoggerException.WriteException(ex);
            }
        }
    }
}
