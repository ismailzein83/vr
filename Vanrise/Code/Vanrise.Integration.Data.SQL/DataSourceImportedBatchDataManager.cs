using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Vanrise.Data.SQL;
using Vanrise.Integration.Entities;
using Vanrise.Queueing.Entities;
using Vanrise.Common;

namespace Vanrise.Integration.Data.SQL
{
    public class DataSourceImportedBatchDataManager : BaseSQLDataManager, IDataSourceImportedBatchDataManager
    {
        #region Ctor/Properties
        public DataSourceImportedBatchDataManager()
            : base(GetConnectionStringName("BusinessProcessTrackingDBConnStringKey", "BusinessProcessTrackingDBConnString"))
        {

        }

        #endregion

        #region Public Methods
        public long InsertEntry(Guid dataSourceId, string batchDescription, decimal? batchSize, BatchState batchState, bool? isDuplicateSameSize, int recordCounts, Entities.MappingResult? result,
            string mapperMessage, string queueItemsIds, string logEntryTime, DateTime? batchStart, DateTime? batchEnd, ItemExecutionFlowStatus executionStatus)
        {
            return (long)ExecuteScalarSP("[integration].[sp_DataSourceImportedBatch_Insert]", dataSourceId, batchDescription, batchSize, batchState, isDuplicateSameSize, recordCounts, result, mapperMessage, queueItemsIds, logEntryTime, batchStart, batchEnd, executionStatus);
        }

        public void UpdateExecutionStatus(Dictionary<long, ItemExecutionFlowStatus> executionStatusToUpdateById)
        {
            DataTable dt = BuildDataSourceImportedBatchTable(executionStatusToUpdateById);

            ExecuteNonQuerySPCmd("[integration].[sp_DataSourceImportedBatch_UpdateStatus]", (cmd) =>
            {
                var batchStatusToUpdate = new SqlParameter("@BatchStatusToUpdate", SqlDbType.Structured);
                batchStatusToUpdate.TypeName = "[integration].[DataSourceImportedBatchExecutionStatusType]";
                batchStatusToUpdate.Value = dt;
                cmd.Parameters.Add(batchStatusToUpdate);
            });
        }

        public List<DataSourceImportedBatch> GetFilteredDataSourceImportedBatches(DataSourceImportedBatchQuery query)
        {
            string executionStatusString = (query.ExecutionFlowsStatus != null) ? string.Join(",", query.ExecutionFlowsStatus.Select(n => (int)n)) : null;

            string mappingResults = null;
            if (query.MappingResults != null && query.MappingResults.Count > 0)
                mappingResults = string.Join(",", query.MappingResults.MapRecords(x => (int)x));

            return GetItemsSP("[integration].[sp_DataSourceImportedBatch_GetFiltered]", DataSourceImportedBatchMapper,
                   query.DataSourceId,
                   query.BatchName,
                   mappingResults,
                   query.From,
                   query.To,
                   query.Top,
                   executionStatusString
               );
        }

        public List<DataSourceImportedBatch> GetDataSourceImportedBatchesAfterID(int limitResult, long? afterId, List<ItemExecutionFlowStatus> executionFlowsStatus)
        {
            string executionStatusString = (executionFlowsStatus != null) ? string.Join(",", executionFlowsStatus.Select(n => (int)n)) : null;
            return GetItemsSP("[integration].[sp_DataSourceImportedBatch_GetAfterID]", DataSourceImportedBatchMapper, limitResult, afterId, executionStatusString);
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

        #endregion

        #region Private Methods

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
                BatchEnd = GetReaderValue<DateTime?>(reader, "BatchEnd"),
                ExecutionStatus = reader["ExecutionStatus"] != DBNull.Value ? (ItemExecutionFlowStatus)reader["ExecutionStatus"] : ItemExecutionFlowStatus.New
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

        private DataTable BuildDataSourceImportedBatchTable(Dictionary<long, ItemExecutionFlowStatus> executionStatusToUpdateById)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(long));
            dt.Columns.Add("ExecutionStatus", typeof(int));
            dt.BeginLoadData();

            foreach (var item in executionStatusToUpdateById)
            {
                DataRow dr = dt.NewRow();
                dr["ID"] = item.Key;
                dr["ExecutionStatus"] = item.Value;
                dt.Rows.Add(dr);
            }

            dt.EndLoadData();
            return dt;
        }

        #endregion

        //#region Queries 

        //const string query_UpdateDataSourceImportedBatch = @"UPDATE importedBatches set importedBatches.ExecutionStatus = batches.ExecutionStatus
        //                                                    FROM [integration].[DataSourceImportedBatch] importedBatches
        //                                                    JOIN @Batches batches on batches.ID = importedBatches.ID";
        //#endregion
    }
}