using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
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

        public long InsertEntry(int dataSourceId, string batchDescription, decimal? batchSize, int recordCounts, Entities.MappingResult result, string mapperMessage, string queueItemsIds, DateTime logEntryTime)
        {
            return (long)ExecuteScalarSP("integration.sp_DataSourceImportedBatch_Insert",  dataSourceId, batchDescription, batchSize, recordCounts, result, mapperMessage, queueItemsIds, logEntryTime);
        }

        public Vanrise.Entities.BigResult<DataSourceImportedBatch> GetFilteredDataSourceImportedBatches(Vanrise.Entities.DataRetrievalInput<DataSourceImportedBatchQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("[integration].[sp_DataSourceImportedBatch_CreateTempForFiltered]", tempTableName, input.Query.DataSourceId, input.Query.BatchName, input.Query.MappingResult, input.Query.From, input.Query.To);
            };

            return RetrieveData(input, createTempTableAction, DataSourceImportedBatchMapper);
        }

        public List<Entities.DataSourceImportedBatchName> GetBatchNames()
        {
            return GetItemsSP("integration.sp_DataSourceImportedBatch_GetBatchNames", BatchNameMapper);
        }

        Vanrise.Integration.Entities.DataSourceImportedBatch DataSourceImportedBatchMapper(IDataReader reader)
        {
            Vanrise.Integration.Entities.DataSourceImportedBatch dataSourceImportedBatch = new Vanrise.Integration.Entities.DataSourceImportedBatch
            {
                ID = (long)reader["ID"],
                BatchName = reader["BatchDescription"] as string,
                BatchSize = GetReaderValue<decimal>(reader, "BatchSize"),
                RecordsCount = (int)reader["RecordsCount"],
                MappingResult = (MappingResultType)reader["MappingResult"],
                MapperMessage = reader["MapperMessage"] as string,
                LogEntryTime = (DateTime)reader["LogEntryTime"]
            };

            return dataSourceImportedBatch;
        }

        Vanrise.Integration.Entities.DataSourceImportedBatchName BatchNameMapper(IDataReader reader)
        {
            Vanrise.Integration.Entities.DataSourceImportedBatchName batchName = new Vanrise.Integration.Entities.DataSourceImportedBatchName
            {
                BatchName = reader["BatchDescription"] as string
            };

            return batchName;
        }
    }
}
