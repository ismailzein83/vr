using System;
using System.Collections.Generic;
using Vanrise.Integration.Entities;
using Vanrise.Queueing.Entities;

namespace Vanrise.Integration.Data
{
    public interface IDataSourceImportedBatchDataManager : IDataManager
    {
        long InsertEntry(Guid dataSourceId, string batchDescription, decimal? batchSize, BatchState batchState, bool? isDuplicateSameSize, int recordCounts, Entities.MappingResult? result,
            string mapperMessage, string queueItemsIds, string logEntryTime, DateTime? batchStart, DateTime? batchEnd, ItemExecutionFlowStatus executionStatus);

        void UpdateExecutionStatus(Dictionary<long, ItemExecutionFlowStatus> executionStatusToUpdateById);

        List<DataSourceImportedBatch> GetFilteredDataSourceImportedBatches(DataSourceImportedBatchQuery query);

        List<DataSourceImportedBatch> GetDataSourceImportedBatchesAfterID(int limitResult, long? afterId, List<ItemExecutionFlowStatus> executionFlowsStatus);

        List<DataSourceSummary> GetDataSourcesSummary(DateTime fromTime, List<Guid> dataSourcesIds);

        List<DataSourceImportedBatch> GetDataSourceImportedBatches(Guid DataSourceId, DateTime from);
    }
}
