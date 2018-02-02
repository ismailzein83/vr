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
        private static Dictionary<string, string> _mapper = new Dictionary<string, string>();
        public BPTrackingDataManager()
            : base(GetConnectionStringName("BusinessProcessTrackingDBConnStringKey", "BusinessProcessTrackingDBConnString"))
        {
        }

        #region Static

        static BPTrackingDataManager()
        {
            CreateSchemaTable();
            _mapper.Add("SeverityDescription", "Severity");
        }

        static DataTable s_trackingMessagesSchemaTable;
        static void CreateSchemaTable()
        {
            s_trackingMessagesSchemaTable = new DataTable { TableName = "bp.BPTracking" };
            s_trackingMessagesSchemaTable.Columns.Add("ProcessInstanceID", typeof(long));
            s_trackingMessagesSchemaTable.Columns.Add("ParentProcessID", typeof(long));
            s_trackingMessagesSchemaTable.Columns.Add("TrackingMessage", typeof(string));
            s_trackingMessagesSchemaTable.Columns.Add("ExceptionDetail", typeof(string));
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
                row["ProcessInstanceID"] = msg.ProcessInstanceId;
                row["ParentProcessID"] = msg.ParentProcessId.HasValue ? (object)msg.ParentProcessId.Value : DBNull.Value;
                row["TrackingMessage"] = msg.TrackingMessage;
                row["ExceptionDetail"] = msg.ExceptionDetail;
                row["Severity"] = (int)msg.Severity;
                row["EventTime"] = msg.EventTime;
                dt.Rows.Add(row);
            }
            dt.EndLoadData();
            WriteDataTableToDB(dt, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
        }

        public BigResult<BPTrackingMessage> GetFilteredTrackings(DataRetrievalInput<TrackingQuery> input)
        {
            int? top = input.DataRetrievalResultType == DataRetrievalResultType.Normal ? (int?)(input.ToRow ?? 0) - (input.FromRow ?? 0) + 1 : null;

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

        public BigResult<BPTrackingMessageDetail> GetFilteredBPInstanceTracking(DataRetrievalInput<BPTrackingQuery> input)
        {
            if (input.SortByColumnName != null)
                input.SortByColumnName = input.SortByColumnName.Replace("Entity.", "");

            return RetrieveData(input, (tempTableName) =>
            {
                ExecuteNonQuerySP("bp.sp_BPTrackings_CreateTempForFiltered", tempTableName, input.Query.ProcessInstanceId, input.Query.Message,
                    input.Query.Severities == null ? null : string.Join(",", input.Query.Severities.Select(n => (int)n).ToArray()));

            }, BPTrackingDetailMapper, _mapper);
        }
        public List<BPTrackingMessage> GetTrackingsFrom(TrackingQuery input)
        {
            return GetItemsSP("bp.sp_BPTrackings_GetFromInstanceId", BPTrackingMapper, input.ProcessInstanceId, input.FromTrackingId);
        }
        public List<BPTrackingMessage> GetBPInstanceTrackingMessages(long processInstanceId, List<LogEntryType> severities)
        {
            string severitiesString = (severities == null) ? string.Empty : string.Join(",", severities.Select(n => ((int)n).ToString()).ToArray());
            return GetItemsSP("bp.sp_BPTrackings_GetBPInstanceTrackingMessages", BPTrackingMapper, processInstanceId, severitiesString);
        }

        public List<BPTrackingMessage> GetUpdated(BPTrackingUpdateInput input)
        {
            string severityIdsAsString = null;
            if (input.Severities != null && input.Severities.Count() > 0)
                severityIdsAsString = string.Join<int>(",", input.Severities.Select(n => ((int)n)));

            List<BPTrackingMessage> bpTrackingMessages = new List<BPTrackingMessage>();

            ExecuteReaderSP("[bp].[sp_BPTrackings_GetUpdated]", (reader) =>
            {
                while (reader.Read())
                    bpTrackingMessages.Add(BPTrackingMapper(reader));
            },
               input.NbOfRows, ToDBNullIfDefault(input.GreaterThanID), input.BPInstanceID, severityIdsAsString);

            return bpTrackingMessages;
        }

        public List<BPTrackingMessage> GetBeforeId(BPTrackingBeforeIdInput input)
        {
            string severityIdsAsString = null;
            if (input.Severities != null && input.Severities.Count() > 0)
                severityIdsAsString = string.Join<int>(",", input.Severities.Select(n => ((int)n)));

            return GetItemsSP("[bp].[sp_BPTrackings_GetBeforeID]", BPTrackingMapper, input.LessThanID, input.NbOfRows, input.BPInstanceID, severityIdsAsString);
        }



        public List<BPTrackingMessage> GetRecentBPInstanceTrackings(long bpInstanceId, int nbOfRecords, long? lessThanId, List<LogEntryType> severities)
        {
            string severityIdsAsString = null;
            if (severities != null && severities.Count() > 0)
                severityIdsAsString = string.Join<int>(",", severities.Select(n => ((int)n)));
            return GetItemsSP("[bp].[sp_BPTrackings_GetBPInstanceRecent]", BPTrackingMapper, bpInstanceId, nbOfRecords, lessThanId, severityIdsAsString);
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
                EventTime = (DateTime)reader["EventTime"],
                ExceptionDetail = reader["ExceptionDetail"] as string
            };

            return bpTrackingMessage;
        }

        BPTrackingMessageDetail BPTrackingDetailMapper(IDataReader reader)
        {
            return new BPTrackingMessageDetail() { Entity = BPTrackingMapper(reader) };
        }

        #endregion

    }
}
