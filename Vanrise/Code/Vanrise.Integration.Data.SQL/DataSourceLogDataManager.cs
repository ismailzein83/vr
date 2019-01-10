using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Data.SQL
{
    public class DataSourceLogDataManager : BaseSQLDataManager, IDataSourceLogDataManager
    {
        private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();

        static DataSourceLogDataManager()
        {
            _columnMapper.Add("SeverityDescription", "Severity");
        }

        public DataSourceLogDataManager()
            : base(GetConnectionStringName("BusinessProcessTrackingDBConnStringKey", "BusinessProcessTrackingDBConnString"))
        {

        }

        public void InsertEntry(Vanrise.Entities.LogEntryType entryType, string message, Guid dataSourceId, long? importedBatchId, string logTimeSpan)
        {
            ExecuteNonQuerySP("integration.sp_DataSourceLog_Insert", dataSourceId, entryType, message, importedBatchId, logTimeSpan);
        }

        public List<DataSourceLog> GetFilteredDataSourceLogs(DataSourceLogQuery query)
        {
            string severities = null;
            if (query.Severities != null && query.Severities.Count > 0)
                severities = string.Join(",", query.Severities.MapRecords(x => (int)x));

            return GetItemsSP("[integration].[sp_DataSourceLog_GetFiltered]", DataSourceLogMapper,
                   query.DataSourceId,
                   severities,
                   query.From,
                   query.To,
                   query.Top
               );
        }


        Vanrise.Integration.Entities.DataSourceLog DataSourceLogMapper(IDataReader reader)
        {
            Vanrise.Integration.Entities.DataSourceLog dataSourceLog = new Vanrise.Integration.Entities.DataSourceLog
            {
                ID = (int)reader["ID"],
                DataSourceId = GetReaderValue<Guid>(reader,"DataSourceId"),
                Severity = (LogEntryType)reader["Severity"],
                Message = GetReaderValue<string>(reader, "Message"),
                LogEntryTime = (DateTime)reader["LogEntryTime"]
            };

            return dataSourceLog;
        }
                
    }
}
