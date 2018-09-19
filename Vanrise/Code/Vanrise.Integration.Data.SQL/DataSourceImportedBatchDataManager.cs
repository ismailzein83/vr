using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Vanrise.Data.SQL;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Data.SQL
{
    public class DataSourceImportedBatchDataManager : BaseSQLDataManager, IDataSourceImportedBatchDataManager
    {
        public DataSourceImportedBatchDataManager()
            : base(GetConnectionStringName("BusinessProcessTrackingDBConnStringKey", "BusinessProcessTrackingDBConnString"))
        {

        }

        public long InsertEntry(Guid dataSourceId, string batchDescription, decimal? batchSize, int recordCounts, Entities.MappingResult result, string mapperMessage, string queueItemsIds, string logEntryTime)
        {
            return (long)ExecuteScalarSP("integration.sp_DataSourceImportedBatch_Insert", dataSourceId, batchDescription, batchSize, recordCounts, result, mapperMessage, queueItemsIds, logEntryTime);
        }

        public Vanrise.Entities.BigResult<DataSourceImportedBatch> GetFilteredDataSourceImportedBatches(Vanrise.Entities.DataRetrievalInput<DataSourceImportedBatchQuery> input)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>();
            mapper.Add("MappingResultDescription", "MappingResult");

            DataTable dtMappingResults = BuildMappingResultsTable(input.Query.MappingResults);

            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySPCmd("integration.sp_DataSourceImportedBatch_CreateTempByFiltered", (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@TempTableName", tempTableName));
                    cmd.Parameters.Add(new SqlParameter("@DataSourceId", input.Query.DataSourceId));
                    cmd.Parameters.Add(new SqlParameter("@BatchName", input.Query.BatchName));

                    var dtParameter = new SqlParameter("@MappingResults", SqlDbType.Structured);
                    dtParameter.Value = dtMappingResults;
                    cmd.Parameters.Add(dtParameter);

                    cmd.Parameters.Add(new SqlParameter("@From", input.Query.From));
                    cmd.Parameters.Add(new SqlParameter("@To", input.Query.To));
                    cmd.Parameters.Add(new SqlParameter("@Top", input.Query.Top));
                });
            };

            return RetrieveData(input, createTempTableAction, DataSourceImportedBatchMapper, mapper);
        }

        public List<DataSourceSummary> GetDataSourcesSummary(DateTime fromTime, List<Guid> dataSourcesIds)
        {
            string serializedDataSourceIds = null;
            if (dataSourcesIds != null && dataSourcesIds.Count > 0)
                serializedDataSourceIds = string.Join<Guid>(",", dataSourcesIds);

            return GetItemsSP("integration.sp_DataSourceSummary_Get", DataSourceSummaryMapper, serializedDataSourceIds, fromTime);
        }

        Vanrise.Integration.Entities.DataSourceImportedBatch DataSourceImportedBatchMapper(IDataReader reader)
        {
            Vanrise.Integration.Entities.DataSourceImportedBatch dataSourceImportedBatch = new Vanrise.Integration.Entities.DataSourceImportedBatch
            {
                ID = (long)reader["ID"],
                BatchDescription = reader["BatchDescription"] as string,
                BatchSize = GetReaderValue<decimal>(reader, "BatchSize"),
                RecordsCount = (int)reader["RecordsCount"],
                MappingResult = (MappingResult)reader["MappingResult"],
                MapperMessage = reader["MapperMessage"] as string,
                QueueItemIds = reader["QueueItemIds"] as string,
                LogEntryTime = (DateTime)reader["LogEntryTime"]
            };

            return dataSourceImportedBatch;
        }

        private DataTable BuildMappingResultsTable(List<MappingResult> mappingResults)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MappingResult", typeof(int));
            dt.BeginLoadData();

            foreach (var mappingResult in mappingResults)
            {
                DataRow dr = dt.NewRow();
                dr["MappingResult"] = mappingResult;
                dt.Rows.Add(dr);
            }

            dt.EndLoadData();
            return dt;
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
                NbEmptyBatch = (int)reader["NbEmptyBatch"]
            };
        }
    }
}