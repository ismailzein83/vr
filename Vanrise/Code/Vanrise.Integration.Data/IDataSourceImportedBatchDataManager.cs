using System;
using System.Collections.Generic;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Data
{
    public interface IDataSourceImportedBatchDataManager : IDataManager
    {
        long InsertEntry(Guid dataSourceId, string batchDescription, decimal? batchSize, BatchState batchState, bool? isDuplicateSameSize, int recordCounts, MappingResult? result,
            string mapperMessage, string queueItemsIds, string logEntryTime, DateTime? batchStart, DateTime? batchEnd);

        Vanrise.Entities.BigResult<DataSourceImportedBatch> GetFilteredDataSourceImportedBatches(Vanrise.Entities.DataRetrievalInput<DataSourceImportedBatchQuery> input);

        List<DataSourceSummary> GetDataSourcesSummary(DateTime fromTime, List<Guid> dataSourcesIds);

        List<DataSourceImportedBatch> GetDataSourceImportedBatches(Guid DataSourceId, DateTime from);
    }
}
