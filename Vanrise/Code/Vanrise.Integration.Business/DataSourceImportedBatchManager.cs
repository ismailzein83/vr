using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Data;

namespace Vanrise.Integration.Business
{
    public class DataSourceImportedBatchManager
    {
        public long WriteEntry(Entities.ImportedBatchEntry entry, int dataSourceId, DateTime logEntryTime)
        {
            IDataSourceImportedBatchDataManager manager = IntegrationDataManagerFactory.GetDataManager<IDataSourceImportedBatchDataManager>();
            return manager.InsertEntry(dataSourceId, entry.BatchDescription, entry.BatchSize, entry.RecordsCount, entry.Result, entry.MapperMessage, entry.QueueItemsIds, logEntryTime);
        }
    }
}
