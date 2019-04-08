using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Data.RDB
{
    public class BPVisualEventDataManager : IBPVisualEventDataManager
    {
        public static string TABLE_NAME = "bp_BPVisualEvent";
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
    }
}
