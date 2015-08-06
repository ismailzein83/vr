using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Data;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Business
{
    public class DataSourceLogger : Entities.IDataSourceLogger
    {
        int _dataSourceId = 0;

        public int DataSourceId
        {
            set { this._dataSourceId = value; }
        }

        #region Public Methods

        public long LogImportedBatchEntry(Entities.ImportedBatchEntry entry)
        {
            IDataSourceImportedBatchDataManager manager = IntegrationDataManagerFactory.GetDataManager<IDataSourceImportedBatchDataManager>();
            return manager.InsertEntry(this._dataSourceId, entry.BatchDescription, entry.BatchSize, entry.RecordsCount, entry.Result, entry.MapperMessage, entry.QueueItemsIds, DateTime.Now);
        }

        public void LogEntry(Vanrise.Common.LogEntryType logEntryType, string messageFormat, params object[] args)
        {
            this.PrivateWriteEntry(logEntryType, messageFormat, args);
        }

        public void LogEntry(Common.LogEntryType logEntryType, long importedBatchId, string messageFormat, params object[] args)
        {
            this.PrivateWriteEntry(logEntryType, importedBatchId, messageFormat, args);
        }

        public void WriteVerbose(string messageFormat, params object[] args)
        {
            this.PrivateWriteEntry(Vanrise.Common.LogEntryType.Verbose, messageFormat, args);
        }

        public void WriteInformation(string messageFormat, params object[] args)
        {
            this.PrivateWriteEntry(Vanrise.Common.LogEntryType.Information, messageFormat, args);
        }

        public void WriteWarning(string messageFormat, params object[] args)
        {
            this.PrivateWriteEntry(Vanrise.Common.LogEntryType.Warning, messageFormat, args);
        }

        public void WriteError(string messageFormat, params object[] args)
        {
            this.PrivateWriteEntry(Vanrise.Common.LogEntryType.Error, messageFormat, args);
        }

        public Vanrise.Entities.IDataRetrievalResult<DataSourceLog> GetFilteredDataSourceLogs(Vanrise.Entities.DataRetrievalInput<DataSourceLogQuery> input)
        {
            IDataSourceLogDataManager dataManager = IntegrationDataManagerFactory.GetDataManager<IDataSourceLogDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredDataSourceLogs(input));
        }

        #endregion

        #region Private Methods

        private void PrivateWriteEntry(Vanrise.Common.LogEntryType entryType, string messageFormat, params object[] args)
        {
            this.PrivateWriteEntry(entryType, null, messageFormat, args);
        }

        private void PrivateWriteEntry(Vanrise.Common.LogEntryType entryType, long? importedBatchId, string messageFormat, params object[] args)
        {
            IDataSourceLogDataManager manager = IntegrationDataManagerFactory.GetDataManager<IDataSourceLogDataManager>();
            manager.InsertEntry(entryType, String.Format(messageFormat, args), this._dataSourceId, importedBatchId, DateTime.Now);
        }

        #endregion 
    }
}
