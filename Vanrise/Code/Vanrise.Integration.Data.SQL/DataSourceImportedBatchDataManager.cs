using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Vanrise.Data.SQL;
using Vanrise.Common;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Data.SQL
{
    public class DataSourceImportedBatchDataManager : BaseSQLDataManager, IDataSourceImportedBatchDataManager
    {
        public DataSourceImportedBatchDataManager()
            : base(GetConnectionStringName("BusinessProcessTrackingDBConnStringKey", "BusinessProcessTrackingDBConnString"))
        {

        }

        public long InsertEntry(Guid dataSourceId, string batchDescription, decimal? batchSize, BatchState batchState, bool? isDuplicateSameSize, int recordCounts, Entities.MappingResult? result,
            string mapperMessage, string queueItemsIds, string logEntryTime, DateTime? batchStart, DateTime? batchEnd)
        {
            return (long)ExecuteScalarSP("[integration].[sp_DataSourceImportedBatch_Insert]", dataSourceId, batchDescription, batchSize, batchState, isDuplicateSameSize, recordCounts, result, mapperMessage, queueItemsIds, logEntryTime, batchStart, batchEnd);
        }

      
        public List<DataSourceImportedBatch> GetFilteredDataSourceImportedBatches(DataSourceImportedBatchQuery query)
        {
            string mappingResults = null;
            if (query.MappingResults != null && query.MappingResults.Count > 0)
                mappingResults = string.Join(",", query.MappingResults.MapRecords(x => (int)x));

            return GetItemsSP("[integration].[sp_DataSourceImportedBatch_GetFiltered]", DataSourceImportedBatchMapper,
                   query.DataSourceId,
                   query.BatchName,
                   mappingResults,
                   query.From,
                   query.To,
                   query.Top
               );
        }
        public List<DataSourceImportedBatch> GetDataSourceImportedBatches(Guid DataSourceId, DateTime from)
        {
            return GetItemsSP("[integration].[sp_DataSourceImportedBatch_GetByDataSource]", DataSourceImportedBatchMapper, DataSourceId, from);
        }

        public List<DataSourceSummary> GetDataSourcesSummary(DateTime fromTime, List<Guid> dataSourcesIds)
        {
            string serializedDataSourceIds = null;
            if (dataSourcesIds != null && dataSourcesIds.Count > 0)
                serializedDataSourceIds = string.Join<Guid>(",", dataSourcesIds);

            return GetItemsSP("[integration].[sp_DataSourceSummary_Get]", DataSourceSummaryMapper, serializedDataSourceIds, fromTime);
        }

        private DataSourceImportedBatch DataSourceImportedBatchMapper(IDataReader reader)
        {
            var dataSourceImportedBatch = new Vanrise.Integration.Entities.DataSourceImportedBatch
            {
                ID = (long)reader["ID"],
                BatchDescription = reader["BatchDescription"] as string,
                BatchSize = GetReaderValue<decimal>(reader, "BatchSize"),
                RecordsCount = (int)reader["RecordsCount"],
                MappingResult = (MappingResult)reader["MappingResult"],
                MapperMessage = reader["MapperMessage"] as string,
                QueueItemIds = reader["QueueItemIds"] as string,
                LogEntryTime = (DateTime)reader["LogEntryTime"],
                BatchStart = GetReaderValue<DateTime?>(reader, "BatchStart"),
                BatchEnd = GetReaderValue<DateTime?>(reader, "BatchEnd")
            };

            int? batchStateAsInt = GetReaderValue<int?>(reader, "BatchState");
            dataSourceImportedBatch.BatchState = batchStateAsInt.HasValue ? (BatchState)batchStateAsInt.Value : BatchState.Normal;

            return dataSourceImportedBatch;
        }

        private DataSourceSummary DataSourceSummaryMapper(IDataReader reader)
        {
            return new DataSourceSummary()
            {
                DataSourceId = GetReaderValue<Guid>(reader, "DataSourceId"),
                LastImportedBatchTime = (DateTime)reader["LastImportedBatchTime"],
                NbImportedBatch = (int)reader["NbImportedBatch"],
                TotalRecordCount = (int)reader["TotalRecordCount"],
                MaxRecordCount = (int)reader["MaxRecordCount"],
                MinRecordCount = (int)reader["MinRecordCount"],
                MaxBatchSize = GetReaderValue<decimal?>(reader, "MaxBatchSize"),
                MinBatchSize = GetReaderValue<decimal?>(reader, "MinBatchSize"),
                NbInvalidBatch = (int)reader["NbInvalidBatch"],
                NbEmptyBatch = (int)reader["NbEmptyBatch"],
                MinBatchStart = GetReaderValue<DateTime?>(reader, "MinBatchStart"),
                MaxBatchEnd = GetReaderValue<DateTime?>(reader, "MaxBatchEnd")
            };
        }
    }
}