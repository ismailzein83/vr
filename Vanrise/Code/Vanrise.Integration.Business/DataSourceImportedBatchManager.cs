using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Data;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Business
{
    public class DataSourceImportedBatchManager
    {
        public long WriteEntry(Entities.ImportedBatchEntry entry, int dataSourceId, DateTime logEntryTime)
        {
            IDataSourceImportedBatchDataManager manager = IntegrationDataManagerFactory.GetDataManager<IDataSourceImportedBatchDataManager>();
            return manager.InsertEntry(dataSourceId, entry.BatchDescription, entry.BatchSize, entry.RecordsCount, entry.Result, entry.MapperMessage, entry.QueueItemsIds, logEntryTime);
        }

        public Vanrise.Entities.IDataRetrievalResult<DataSourceImportedBatch> GetFilteredDataSourceImportedBatches(Vanrise.Entities.DataRetrievalInput<DataSourceImportedBatchQuery> input)
        {
            IDataSourceImportedBatchDataManager dataManager = IntegrationDataManagerFactory.GetDataManager<IDataSourceImportedBatchDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredDataSourceImportedBatches(input));
        }

        public List<Vanrise.Integration.Entities.DataSourceImportedBatchName> GetBatchNames()
        {
            IDataSourceImportedBatchDataManager dataManager = IntegrationDataManagerFactory.GetDataManager<IDataSourceImportedBatchDataManager>();
            return dataManager.GetBatchNames();
        }
    }
}
