using System;
using System.Collections.Generic;
using Vanrise.Data.SQL;
using System.Data;
using System.Linq;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Data.SQL
{
    internal class BPTrackingDataManager : BaseSQLDataManager, IBPTrackingDataManager
    {
        public BPTrackingDataManager()
            : base(GetConnectionStringName("BusinessProcessTrackingDBConnStringKey", "BusinessProcessTrackingDBConnString"))
        {
        }

        #region Static

        static BPTrackingDataManager()
        {
            CreateSchemaTable();
        }

        static DataTable s_trackingMessagesSchemaTable;
        static void CreateSchemaTable()
        {
            s_trackingMessagesSchemaTable = new DataTable {TableName = "bp.BPTracking"};
            s_trackingMessagesSchemaTable.Columns.Add("ProcessInstanceID", typeof(long));
            s_trackingMessagesSchemaTable.Columns.Add("ParentProcessID", typeof(long));
            s_trackingMessagesSchemaTable.Columns.Add("TrackingMessage", typeof(string));
            s_trackingMessagesSchemaTable.Columns.Add("Severity", typeof(int));
            s_trackingMessagesSchemaTable.Columns.Add("EventTime", typeof(DateTime));
        }


        #endregion

        #region IBPTrackingDataManager Members

        public void Insert(BPTrackingMessage trackingMessage)
        {
            WriteTrackingMessagesToDB(new List<BPTrackingMessage> { trackingMessage });
        }

        public void WriteTrackingMessagesToDB(List<BPTrackingMessage> lstTrackingMsgs)
        {
            DataTable dt = s_trackingMessagesSchemaTable.Clone();            
            dt.BeginLoadData();
            foreach (var msg in lstTrackingMsgs)
            {
                DataRow row = dt.NewRow();
                int index = 0;
                row[index++] = msg.ProcessInstanceId;
                row[index++] = msg.ParentProcessId.HasValue ? (object)msg.ParentProcessId.Value : DBNull.Value;
                row[index++] = msg.TrackingMessage;
                row[index++] = (int)msg.Severity;
                row[index] = msg.EventTime;
                dt.Rows.Add(row);
            }
            dt.EndLoadData();
            WriteDataTableToDB(dt, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
        }

        public BigResult<BPTrackingMessage> GetFilteredTrackings(DataRetrievalInput<TrackingQuery> input)
        {
            int? top = input.DataRetrievalResultType == DataRetrievalResultType.Normal ? (int?) (input.ToRow ?? 0) - (input.FromRow ?? 0) : null;

            return new BigResult<BPTrackingMessage>()
            {
                Data = GetItemsSP("bp.sp_BPTrackings_GetByInstanceId", BPTrackingMapper, input.Query.ProcessInstanceId, input.Query.FromTrackingId,
                input.Query.Message,
                input.Query.Severities == null
                    ? null
                    : string.Join(",", input.Query.Severities.Select(n => ((int)n).ToString()).ToArray()),
                top)
            };
        }

        #endregion

        #region Private Methods

        BPTrackingMessage BPTrackingMapper(IDataReader reader)
        {
            var bpTrackingMessage = new BPTrackingMessage
            {
                Id = (long)reader["ID"],
                ProcessInstanceId = (long)reader["ProcessInstanceID"],
                ParentProcessId = GetReaderValue<long?>(reader, "ParentProcessId"),
                TrackingMessage = reader["TrackingMessage"] as string,
                Severity = (LogEntryType)((int)reader["Severity"]),
                EventTime = (DateTime)reader["EventTime"]
            };

            return bpTrackingMessage;
        }

        #endregion
    }
}
