using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Data.RDB
{
    public class BPTrackingDataManager : IBPTrackingDataManager
    {
        static string TABLE_NAME = "bp_BPTracking";
        static string TABLE_ALIAS = "bpTracking";
        const string COL_ID = "ID";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_ParentProcessID = "ParentProcessID";
        const string COL_TrackingMessage = "TrackingMessage";
        const string COL_ExceptionDetail = "ExceptionDetail";
        const string COL_Severity = "Severity";
        const string COL_EventTime = "EventTime";

        static BPTrackingDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_ProcessInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_ParentProcessID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_TrackingMessage, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_ExceptionDetail, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_Severity, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_EventTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "bp",
                DBTableName = "BPTracking",
                Columns = columns,
                IdColumnName = COL_ID
            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("BusinessProcess", "BusinessProcessTrackingDBConnStringKey", "BusinessProcessTrackingDBConnString");

        }

        public List<BPTrackingMessage> GetBeforeId(BPTrackingBeforeIdInput input)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            IEnumerable<int> severitiesValues = input.Severities != null && input.Severities.Count > 0 ? input.Severities.Select(itm => (int)itm) : null;

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, input.NbOfRows, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var where = selectQuery.Where();
            where.EqualsCondition(COL_ProcessInstanceID).Value(input.BPInstanceID);
            where.LessThanCondition(COL_ID).Value(input.LessThanID);

            if (severitiesValues != null)
                where.ListCondition(COL_Severity, RDBListConditionOperator.IN, severitiesValues);

            selectQuery.Sort().ByColumn(COL_ID, RDBSortDirection.DESC);

            return queryContext.GetItems(BPTrackingMapper);
        }

        public List<BPTrackingMessage> GetBPInstanceTrackingMessages(long processInstanceId, List<LogEntryType> severities)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            IEnumerable<int> severitiesValues = severities != null && severities.Count > 0 ? severities.Select(itm => (int)itm) : null;

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var where = selectQuery.Where();
            if (severitiesValues != null)
                where.ListCondition(COL_Severity, RDBListConditionOperator.IN, severitiesValues);

            where.EqualsCondition(TABLE_ALIAS, COL_ProcessInstanceID).Value(processInstanceId);

            selectQuery.Sort().ByColumn(COL_ID, RDBSortDirection.DESC);

            return queryContext.GetItems(BPTrackingMapper);
        }

        public List<BPTrackingMessage> GetUpdated(BPTrackingUpdateInput input)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            IEnumerable<int> severitiesValues = input.Severities != null && input.Severities.Count > 0 ? input.Severities.Select(itm => (int)itm) : null;

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, input.NbOfRows, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var where = selectQuery.Where();
            where.EqualsCondition(COL_ProcessInstanceID).Value(input.BPInstanceID);

            if (severitiesValues != null)
                where.ListCondition(COL_Severity, RDBListConditionOperator.IN, severitiesValues);

            if (input.GreaterThanID == 0)
            {
                selectQuery.Sort().ByColumn(COL_ID, RDBSortDirection.DESC);
            }
            else
            {
                where.GreaterThanCondition(COL_ID).Value(input.GreaterThanID);
                selectQuery.Sort().ByColumn(COL_ID, RDBSortDirection.ASC);
            }

            return queryContext.GetItems(BPTrackingMapper);
        }

        public List<BPTrackingMessage> GetRecentBPInstanceTrackings(long bpInstanceId, int nbOfRecords, long? lessThanId, List<LogEntryType> severities)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            IEnumerable<int> severitiesValues = severities != null && severities.Count > 0 ? severities.Select(itm => (int)itm) : null;

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, nbOfRecords, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var where = selectQuery.Where();
            where.EqualsCondition(COL_ProcessInstanceID).Value(bpInstanceId);

            if (lessThanId.HasValue)
                where.LessThanCondition(COL_ID).Value(lessThanId.Value);

            if (severitiesValues != null)
                where.ListCondition(COL_Severity, RDBListConditionOperator.IN, severitiesValues);

            selectQuery.Sort().ByColumn(COL_ID, RDBSortDirection.DESC);

            return queryContext.GetItems(BPTrackingMapper);
        }

        public void Insert(BPTrackingMessage trackingMessage)
        {
            WriteTrackingMessagesToDB(new List<BPTrackingMessage> { trackingMessage });
        }

        public void WriteTrackingMessagesToDB(List<BPTrackingMessage> lstTrackingMsgs)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            if (lstTrackingMsgs != null)
            {
                foreach (var msg in lstTrackingMsgs)
                {
                    var insertQuery = queryContext.AddInsertQuery();
                    insertQuery.IntoTable(TABLE_NAME);

                    insertQuery.Column(COL_ProcessInstanceID).Value(msg.ProcessInstanceId);

                    if (msg.ParentProcessId.HasValue)
                        insertQuery.Column(COL_ParentProcessID).Value(msg.ParentProcessId.Value);

                    if (!string.IsNullOrEmpty(msg.TrackingMessage))
                        insertQuery.Column(COL_TrackingMessage).Value(msg.TrackingMessage);

                    if (!string.IsNullOrEmpty(msg.ExceptionDetail))
                        insertQuery.Column(COL_ExceptionDetail).Value(msg.ExceptionDetail);

                    insertQuery.Column(COL_Severity).Value((int)msg.Severity);
                    insertQuery.Column(COL_EventTime).Value(msg.EventTime);
                }
                queryContext.ExecuteNonQuery();
            }
        }

        BPTrackingMessage BPTrackingMapper(IRDBDataReader reader)
        {
            return new BPTrackingMessage
            {
                Id = reader.GetLong(COL_ID),
                ProcessInstanceId = reader.GetLong(COL_ProcessInstanceID),
                ParentProcessId = reader.GetNullableLong(COL_ProcessInstanceID),
                Severity = (LogEntryType)reader.GetInt(COL_Severity),
                TrackingMessage = reader.GetString(COL_TrackingMessage),
                ExceptionDetail = reader.GetString(COL_ExceptionDetail),
                EventTime = reader.GetDateTime(COL_EventTime)
            };
        }
    }
}