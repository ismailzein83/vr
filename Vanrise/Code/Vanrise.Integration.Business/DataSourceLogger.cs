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
            DataSourceImportedBatchManager manager = new DataSourceImportedBatchManager();
            return manager.WriteEntry(entry, this._dataSourceId, DateTime.Now.ToString());
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

        #endregion

        #region Private Methods

        private void PrivateWriteEntry(Vanrise.Common.LogEntryType entryType, string messageFormat, params object[] args)
        {
            this.PrivateWriteEntry(entryType, null, messageFormat, args);
        }

        private void PrivateWriteEntry(Vanrise.Common.LogEntryType entryType, long? importedBatchId, string messageFormat, params object[] args)
        {
            DataSourceLogManager manager = new DataSourceLogManager();
            manager.WriteEntry(entryType, this._dataSourceId, importedBatchId, String.Format(messageFormat, args), DateTime.Now.ToString());
        }

        #endregion 
    }
}
