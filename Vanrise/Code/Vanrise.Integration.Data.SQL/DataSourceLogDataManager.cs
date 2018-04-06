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

        public Vanrise.Entities.BigResult<DataSourceLog> GetFilteredDataSourceLogs(Vanrise.Entities.DataRetrievalInput<DataSourceLogQuery> input)
        {
            DataTable dtSeverities = BuildSeveritiesTable(input.Query.Severities);

            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySPCmd("integration.sp_DataSourceLog_CreateTempByFiltered", (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@TempTableName", tempTableName));
                    cmd.Parameters.Add(new SqlParameter("@DataSourceId", input.Query.DataSourceId));

                    var dtParameter = new SqlParameter("@Severities", SqlDbType.Structured);
                    dtParameter.Value = dtSeverities;
                    cmd.Parameters.Add(dtParameter);
                    
                    cmd.Parameters.Add(new SqlParameter("@From", input.Query.From));
                    cmd.Parameters.Add(new SqlParameter("@To", input.Query.To));
                    cmd.Parameters.Add(new SqlParameter("@Top", input.Query.Top));

                });
            };

            return RetrieveData(input, createTempTableAction, DataSourceLogMapper, _columnMapper);
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

        private DataTable BuildSeveritiesTable(List<LogEntryType> severities)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Severity", typeof(int));
            dt.BeginLoadData();

            foreach (var severity in severities)
            {
                DataRow dr = dt.NewRow();
                dr["Severity"] = severity;
                dt.Rows.Add(dr);
            }

            dt.EndLoadData();
            return dt;
        }
    }
}
