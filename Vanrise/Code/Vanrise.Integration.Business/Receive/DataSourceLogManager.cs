using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Data;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Business
{
    public class DataSourceLogManager
    {
        public Vanrise.Entities.IDataRetrievalResult<DataSourceLog> GetFilteredDataSourceLogs(Vanrise.Entities.DataRetrievalInput<DataSourceLogQuery> input)
        {
            IDataSourceLogDataManager dataManager = IntegrationDataManagerFactory.GetDataManager<IDataSourceLogDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredDataSourceLogs(input));
        }

        public void WriteEntry(Vanrise.Entities.LogEntryType entryType, Guid dataSourceId, long? importedBatchId, string message, string logEntryTime)
        {
            IDataSourceLogDataManager manager = IntegrationDataManagerFactory.GetDataManager<IDataSourceLogDataManager>();
            manager.InsertEntry(entryType, message, dataSourceId, importedBatchId, logEntryTime);
        }
    }
}
