using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Data.SQL;
using System.Configuration;
using System.Data;
using System.Runtime.Remoting.Messaging;
using Vanrise.BusinessProcess.Entities;

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
            s_trackingMessagesSchemaTable = new DataTable();
            s_trackingMessagesSchemaTable.TableName = "bp.BPTracking";
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
                row[index++] = msg.Message;
                row[index++] = (int)msg.Severity;
                row[index++] = msg.EventTime;
                dt.Rows.Add(row);
            }
            dt.EndLoadData();
            WriteDataTableToDB(dt, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
        }

        public TrackingResult GetTrackingsByInstanceId(Vanrise.Entities.DataRetrievalInput<TrackingQuery> input)
        {

            return RetrieveData(input, (tempTableName) =>
            {
                ExecuteNonQuerySP("bp.sp_BPTrackings_CreateTempForFiltered", tempTableName, input.Query.LastTrackingId, input.Query.ProcessInstanceId);

            }, BPTrackingMapper, null, new TrackingResult()) as TrackingResult;

            //return GetItemsSP("bp.sp_BPTrackings_GetByInstanceId", BPTrackingMapper, processInstanceID, lastTrackingId);
            //protected BigResult<T> RetrieveData<T>(Dictionary<string, string> fieldsToColumnsMapper = null, BigResult<T> rslt = null);
        }

        //public Vanrise.Entities.BigResult<BPInstance> GetInstancesByCriteria(Vanrise.Entities.DataRetrievalInput<BPInstanceQuery> input)
        //{
        //    
        //}
       

        #endregion

        #region Private Methods

        BPTrackingMessage BPTrackingMapper(IDataReader reader)
        {
            var bpTrackingMessage = new BPTrackingMessage
            {
                Id = (long)reader["ID"],
                ProcessInstanceId = (long)reader["ProcessInstanceID"],
                ParentProcessId = GetReaderValue<long?>(reader, "ParentProcessId"),
                Message = reader["TrackingMessage"] as string,
                Severity = (BPTrackingSeverity)((int)reader["Severity"]),
                EventTime = (DateTime)reader["EventTime"]
            };

            return bpTrackingMessage;
        }

        #endregion
    }
}
