using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Data
{
    public interface IDataSourceImportedBatchDataManager : IDataManager
    {
        long InsertEntry(Guid dataSourceId, string batchDescription, decimal? batchSize, int recordCounts, MappingResult result, string mapperMessage, string queueItemsIds, string logEntryTime);

        Vanrise.Entities.BigResult<DataSourceImportedBatch> GetFilteredDataSourceImportedBatches(Vanrise.Entities.DataRetrievalInput<DataSourceImportedBatchQuery> input);
    }
}
