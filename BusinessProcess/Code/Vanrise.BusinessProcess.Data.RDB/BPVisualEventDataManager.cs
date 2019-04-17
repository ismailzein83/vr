using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Data.RDB
{
    public class BPVisualEventDataManager : IBPVisualEventDataManager
    {
        public static string TABLE_NAME = "bp_BPVisualEvent";
        static string TABLE_ALIAS = "bpVisualEvent";
        public const string COL_ID = "ID";
        public const string COL_ProcessInstanceID = "ProcessInstanceID";
        public const string COL_ActivityID = "ActivityID";
        public const string COL_Title = "Title";
        public const string COL_EventTypeID = "EventTypeID";
        public const string COL_EventPayload = "EventPayload";
        public const string COL_CreatedTime = "CreatedTime";


        static BPVisualEventDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_ProcessInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_ActivityID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Title, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_EventTypeID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_EventPayload, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "bp",
                DBTableName = "BPVisualEvent",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime
            });
        }

        public BPVisualEventDataManager()
        {
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("BusinessProcessTracking", "BusinessProcessTrackingDBConnStringKey", "BusinessProcessTrackingDBConnString");

        }

        public void InsertVisualEvent(long processInstanceId, Guid activityId, string title, Guid eventTypeId, string eventPayload)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);

            insertQuery.Column(COL_ProcessInstanceID).Value(processInstanceId);
            insertQuery.Column(COL_ActivityID).Value(activityId);
            insertQuery.Column(COL_Title).Value(title);
            insertQuery.Column(COL_EventTypeID).Value(eventTypeId);
            insertQuery.Column(COL_EventPayload).Value(eventPayload);

            queryContext.ExecuteNonQuery();
        }

        public List<BPVisualEvent> GetAfterId(BPVisualEventDetailUpdateInput input)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var where = selectQuery.Where();
            where.EqualsCondition(COL_ProcessInstanceID).Value(input.BPInstanceID);

            if (input.GreaterThanID == 0)
            {
                selectQuery.Sort().ByColumn(COL_ID, RDBSortDirection.DESC);
            }
            else
            {
                where.GreaterThanCondition(COL_ID).Value(input.GreaterThanID);
                selectQuery.Sort().ByColumn(COL_ID, RDBSortDirection.ASC);
            }

            return queryContext.GetItems(BPVisualItemMapper);
        }

        BPVisualEvent BPVisualItemMapper(IRDBDataReader reader)
        {
            return new BPVisualEvent
            {
                BPVisualEventId = reader.GetLong(COL_ID),
                ProcessInstanceId = reader.GetLong(COL_ProcessInstanceID),
                ActivityId = reader.GetGuid(COL_ActivityID),
                Title = reader.GetString(COL_Title),
                EventTypeId = reader.GetGuid(COL_EventTypeID),
                EventPayload = Serializer.Deserialize<BPVisualEventPayload>(reader.GetString(COL_EventPayload)),
                CreatedTime = reader.GetDateTime(COL_CreatedTime)
            };
        }
    }
}
