using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Vanrise.Integration.Data.SQL
{
    public class DataSourceImportedBatchDataManager : BaseSQLDataManager, IDataSourceImportedBatchDataManager 
    {
        public DataSourceImportedBatchDataManager()
            : base(GetConnectionStringName("BusinessProcessTrackingDBConnStringKey", "BusinessProcessTrackingDBConnString"))
        {

        }

        public long InsertEntry(int dataSourceId, string batchDescription, decimal? batchSize, int recordCounts, Entities.MappingResult result, string mapperMessage, string queueItemsIds)
        {
            return (long)ExecuteScalarSP("integration.sp_DataSourceImportedBatch_Insert",  dataSourceId, batchDescription, batchSize, recordCounts, result, mapperMessage, queueItemsIds);
        }
    }
}
