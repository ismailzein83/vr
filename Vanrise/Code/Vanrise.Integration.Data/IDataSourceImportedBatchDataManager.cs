using System;
using System.Collections.Generic;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Data
{
    public interface IDataSourceImportedBatchDataManager : IDataManager
    {
        long InsertEntry(Guid dataSourceId, string batchDescription, decimal? batchSize, int recordCounts, MappingResult result, string mapperMessage, string queueItemsIds, string logEntryTime);

        Vanrise.Entities.BigResult<DataSourceImportedBatch> GetFilteredDataSourceImportedBatches(Vanrise.Entities.DataRetrievalInput<DataSourceImportedBatchQuery> input);

        List<DataSourceSummary> GetDataSourcesSummary(DateTime fromTime, List<Guid> dataSourcesIds);
    }
}
