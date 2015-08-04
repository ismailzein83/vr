using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Vanrise.Integration.Data.SQL
{
    public class DataSourceLogDataManager : BaseSQLDataManager, IDataSourceLogDataManager
    {
        public DataSourceLogDataManager()
            : base(GetConnectionStringName("BusinessProcessTrackingDBConnStringKey", "BusinessProcessTrackingDBConnString"))
        {

        }

        public void WriteEntry(Common.LogEntryType entryType, string message, int dataSourceId, int? importedBatchId, DateTime logTimeSpan)
        {
            ExecuteNonQuerySP("integration.sp_DataSourceLog_Insert", dataSourceId, entryType, message, importedBatchId, logTimeSpan);
        }
    }
}
