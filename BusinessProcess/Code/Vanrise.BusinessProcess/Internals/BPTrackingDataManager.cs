using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.SQLDataAccess;
using System.Configuration;
using System.Data;

namespace Vanrise.BusinessProcess
{
    internal class BPTrackingDataManager : BaseSQLDataManager, IBPTrackingDataManager
    {
        public BPTrackingDataManager()
            : base(ConfigurationManager.AppSettings["BusinessProcessTrackingDBConnStringKey"] ?? "LogDBConnString")
        {
        }

        #region Static

        static BPTrackingDataManager()
        {
            CreateSchemaTable();
        }

        static DataTable _trackingMessagesSchemaTable;
        static void CreateSchemaTable()
        {
            _trackingMessagesSchemaTable = new DataTable();
            _trackingMessagesSchemaTable.TableName = "bp.BPTracking";
            _trackingMessagesSchemaTable.Columns.Add("ProcessInstanceID", typeof(Guid));
            _trackingMessagesSchemaTable.Columns.Add("ParentProcessID", typeof(Guid));
            _trackingMessagesSchemaTable.Columns.Add("TrackingMessage", typeof(string));
            _trackingMessagesSchemaTable.Columns.Add("Severity", typeof(int));
            _trackingMessagesSchemaTable.Columns.Add("EventTime", typeof(DateTime));
        }


        #endregion

        #region IBPTrackingDataManager Members


        public void WriteTrackingMessagesToDB(List<BPTrackingMessage> lstTrackingMsgs)
        {
            DataTable dt = _trackingMessagesSchemaTable.Clone();            
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

        #endregion
    }
}
