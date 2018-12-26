using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Notification.Entities;
using Vanrise.Common;
using System.Data;

namespace Vanrise.Notification.Data.RDB
{
    public class VRNotificationDataManager : IVRNotificationDataManager //timestamp column had to be used in this table
    {
        #region RDB
        static string TABLE_NAME = "VRNotification_VRNotification";
        static string TABLE_ALIAS = "vrNotification";

        const string COL_ID = "ID";
        const string COL_UserID = "UserID";
        const string COL_TypeID = "TypeID";
        const string COL_ParentType1 = "ParentType1";
        const string COL_ParentType2 = "ParentType2";
        const string COL_EventKey = "EventKey";
        const string COL_ExecuteBPInstanceID = "ExecuteBPInstanceID";
        const string COL_ClearBPInstanceID = "ClearBPInstanceID";
        const string COL_Status = "Status";
        const string COL_AlertLevelID = "AlertLevelID";
        const string COL_Description = "Description";
        const string COL_ErrorMessage = "ErrorMessage";
        const string COL_EventPayload = "EventPayload";
        const string COL_RollbackEventPayload = "RollbackEventPayload";
        const string COL_Data = "Data";
        const string COL_CreationTime = "CreationTime";
        const string COL_TimeStamp = "timestamp";
        static VRNotificationDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_UserID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_TypeID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_ParentType1, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
            columns.Add(COL_ParentType2, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
            columns.Add(COL_EventKey, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 900 });
            columns.Add(COL_ExecuteBPInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_ClearBPInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_Status, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_AlertLevelID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Description, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 900 });
            columns.Add(COL_ErrorMessage, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_EventPayload, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_RollbackEventPayload, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_Data, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreationTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_TimeStamp, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VRNotification",
                DBTableName = "VRNotification",
                Columns = columns,
                IdColumnName = COL_ID
            });
        }
        #endregion

        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_Notification_Transaction", "VRNotificationTransactionDBConnStringKey", "VRNotificationTransactionDBConnString");
        }
        #endregion
        #region Mappers
        public VRNotification VRNotificationMapper(IRDBDataReader reader)
        {
            var notification = new VRNotification
            {
                VRNotificationId = reader.GetLong(COL_ID),
                UserId = reader.GetInt(COL_UserID),
                ParentTypes = new VRNotificationParentTypes
                {
                    ParentType1 = reader.GetString(COL_ParentType1),
                    ParentType2 = reader.GetString(COL_ParentType2)
                },
                Status = (VRNotificationStatus)reader.GetInt(COL_Status),
                EventKey = reader.GetString(COL_EventKey),
                TypeId = reader.GetGuid(COL_TypeID),
                AlertLevelId = reader.GetGuid(COL_AlertLevelID),
                Description = reader.GetString(COL_Description),
                ExecuteBPInstanceID = reader.GetNullableLong("ExecuteBPInstanceID"),
                ClearBPInstanceID = reader.GetNullableLong("ClearBPInstanceID"),
                ErrorMessage = reader.GetString(COL_ErrorMessage),
                CreationTime = reader.GetDateTime(COL_CreationTime),
            };
            string serializedData = reader.GetStringWithEmptyHandling(COL_Data);
            if (!string.IsNullOrEmpty(serializedData))
                notification.Data = Serializer.Deserialize<VRNotificationData>(serializedData);
            string serializedEventPayload = reader.GetStringWithEmptyHandling("EventPayload");
            if (!string.IsNullOrEmpty(serializedEventPayload))
                notification.EventPayload = Serializer.Deserialize(serializedEventPayload) as IVRActionEventPayload;
            string serializedRollbackEventPayload = reader.GetStringWithEmptyHandling("RollbackEventPayload");
            if (!string.IsNullOrEmpty(serializedRollbackEventPayload))
                notification.RollbackEventPayload = Serializer.Deserialize(serializedRollbackEventPayload) as IVRActionRollbackEventPayload;
            return notification;
        }
        #endregion
        #region IVRNotificationDataManager
        public VRNotification GetVRNotificationById(long notificationId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery.Where().EqualsCondition(COL_ID).Value(notificationId);
            return queryContext.GetItem(VRNotificationMapper);
        }
        public bool Insert(VRNotification notification, out long notificationId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.AddSelectGeneratedId();
            insertQuery.Column(COL_UserID).Value(notification.UserId);
            insertQuery.Column(COL_TypeID).Value(notification.TypeId);
            insertQuery.Column(COL_ParentType1).Value(notification.ParentTypes.ParentType1);
            insertQuery.Column(COL_ParentType2).Value(notification.ParentTypes.ParentType2);
            insertQuery.Column(COL_EventKey).Value(notification.EventKey);
            insertQuery.Column(COL_Status).Value((int)notification.Status);
            insertQuery.Column(COL_AlertLevelID).Value(notification.AlertLevelId);
            insertQuery.Column(COL_Description).Value(notification.Description);
            insertQuery.Column(COL_ErrorMessage).Value(notification.ErrorMessage);
            if (notification.Data != null)
                insertQuery.Column(COL_Data).Value(Serializer.Serialize(notification.Data));
            else
                insertQuery.Column(COL_Data).Null();
            if (notification.EventPayload != null)
                insertQuery.Column(COL_EventPayload).Value(Serializer.Serialize(notification.EventPayload));
            else
                insertQuery.Column(COL_EventPayload).Null();
            var nullableId = queryContext.ExecuteScalar().NullableLongValue;
            if (nullableId.HasValue)
            {
                notificationId = nullableId.Value;
            }
            else
            {
                notificationId = -1;
            }
            return notificationId != -1;
        }
        public List<VRNotification> GetVRNotifications(Guid notificationTypeId, VRNotificationParentTypes parentTypes, string eventKey, List<VRNotificationStatus> statusesToLoad)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery.Where().EqualsCondition(COL_TypeID).Value(notificationTypeId);
            selectQuery.Where().EqualsCondition(COL_EventKey).Value(eventKey);
            if(parentTypes.ParentType1!=null)
                selectQuery.Where().EqualsCondition(COL_ParentType1).Value(parentTypes.ParentType1);
            if (parentTypes.ParentType2 != null)
                selectQuery.Where().EqualsCondition(COL_ParentType2).Value(parentTypes.ParentType2);
            if (statusesToLoad != null && statusesToLoad.Count > 0)
                selectQuery.Where().ListCondition(COL_Status, RDBListConditionOperator.IN, statusesToLoad.Select(itm => (int)itm));

            return queryContext.GetItems(VRNotificationMapper);
        }
        public void UpdateNotificationStatus(long notificationId, VRNotificationStatus vrNotificationStatus, IVRActionRollbackEventPayload rollbackEventPayload, long? executeBPInstanceId, long? clearBPInstanceId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_Status).Value((int)vrNotificationStatus);
            if (rollbackEventPayload != null)
                updateQuery.Column(COL_RollbackEventPayload).Value(Serializer.Serialize(rollbackEventPayload));
            else
                updateQuery.Column(COL_RollbackEventPayload).Null();
            if (executeBPInstanceId.HasValue)
                updateQuery.Column(COL_ExecuteBPInstanceID).Value(executeBPInstanceId.Value);
            else
                updateQuery.Column(COL_ExecuteBPInstanceID).Null();
            if (clearBPInstanceId.HasValue)
                updateQuery.Column(COL_ClearBPInstanceID).Value(clearBPInstanceId.Value);
            else
                updateQuery.Column(COL_ClearBPInstanceID).Null();
            updateQuery.Where().EqualsCondition(COL_ID).Value(notificationId);
            queryContext.ExecuteNonQuery();
        }
        public List<VRNotification> GetNotClearedNotifications(Guid notificationTypeId, VRNotificationParentTypes parentTypes, List<string> eventKeys, DateTime? notificationCreatedAfter)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumn(COL_EventKey, RDBDataType.NVarchar, false);

            if (eventKeys != null)
            {
                foreach (var eventKey in eventKeys)
                {
                    var insertQuery = queryContext.AddInsertQuery();
                    insertQuery.IntoTable(tempTableQuery);
                    insertQuery.Column(COL_EventKey).Value(eventKey);
                }
            }

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinTempCondition = selectQuery.Join().Join(tempTableQuery, "eventKeyTable").On();
            joinTempCondition.EqualsCondition(TABLE_ALIAS, COL_EventKey, "eventKeyTable", COL_EventKey);

            var whereQuery = selectQuery.Where();
            whereQuery.EqualsCondition(COL_TypeID).Value(notificationTypeId);
            if (parentTypes.ParentType1 != null)
                whereQuery.EqualsCondition(COL_ParentType1).Value(parentTypes.ParentType1);
            if (parentTypes.ParentType2 != null)
                whereQuery.EqualsCondition(COL_ParentType2).Value(parentTypes.ParentType2);
            if (notificationCreatedAfter.HasValue)
                whereQuery.GreaterThanCondition(COL_CreationTime).Value(notificationCreatedAfter.Value);
            return queryContext.GetItems<VRNotification>(VRNotificationMapper);
        }
        public void GetFirstPageVRNotifications(IVRNotificationFirstPageContext context)
        {
            bool isFinalRow = false;
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery1 = queryContext.AddSelectQuery();
            selectQuery1.From(TABLE_NAME, TABLE_ALIAS, context.NbOfRows, true);
            selectQuery1.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery1.Where().EqualsCondition(COL_TypeID).Value(context.NotificationTypeId);
            if (context.Query.Description != null)
                selectQuery1.Where().ContainsCondition(COL_Description, context.Query.Description);
            if (context.Query.StatusIds != null && context.Query.StatusIds.Count > 0)
                selectQuery1.Where().ListCondition(COL_Status, RDBListConditionOperator.IN, context.Query.StatusIds);
            if (context.Query.AlertLevelIds != null && context.Query.AlertLevelIds.Count > 0)
                selectQuery1.Where().ListCondition(COL_AlertLevelID, RDBListConditionOperator.IN, context.Query.AlertLevelIds);
            if (context.Query.From.HasValue)
                selectQuery1.Where().GreaterOrEqualCondition(COL_CreationTime).Value(context.Query.From.Value);
            if (context.Query.To.HasValue)
                selectQuery1.Where().LessThanCondition(COL_CreationTime).Value(context.Query.To.Value);

            selectQuery1.Sort().ByColumn(COL_ID, RDBSortDirection.DESC);

            var selectQuery2 = queryContext.AddSelectQuery();
            selectQuery2.From(TABLE_NAME, TABLE_ALIAS, null, true);

            var selectAggregate = selectQuery2.SelectAggregates();
            selectAggregate.Aggregate(RDBNonCountAggregateType.MAX, COL_TimeStamp, "MaxTimestamp");

            queryContext.ExecuteReader(reader =>
            {
                while (reader.Read() && !isFinalRow)
                    isFinalRow = context.onItemReady(VRNotificationMapper(reader));

                if (reader.NextResult())
                    while (reader.Read())
                        context.MaxTimeStamp = reader.GetBytes("MaxTimestamp");

            });
        }
        public List<VRNotification> GetUpdateVRNotifications(IVRNotificationUpdateContext context)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumnsFromTable(TABLE_NAME);

            var insertIntoTempTableQuery = queryContext.AddInsertQuery();
            insertIntoTempTableQuery.IntoTable(tempTableQuery);

            var selectQuery1 = insertIntoTempTableQuery.FromSelect();
            selectQuery1.From(TABLE_NAME, TABLE_ALIAS, context.NbOfRows, true);
            selectQuery1.SelectColumns().AllTableColumns(TABLE_ALIAS);

            selectQuery1.Where().EqualsCondition(COL_TypeID).Value(context.NotificationTypeId);

            if (context.Query.Description != null)
                selectQuery1.Where().ContainsCondition(COL_Description, context.Query.Description);
            if (context.Query.AlertLevelIds != null && context.Query.AlertLevelIds.Count > 0)
                selectQuery1.Where().ListCondition(COL_AlertLevelID, RDBListConditionOperator.IN, context.Query.AlertLevelIds);
            if (context.Query.From.HasValue)
                selectQuery1.Where().GreaterOrEqualCondition(COL_CreationTime).Value(context.Query.From.Value);
            if (context.Query.To.HasValue)
                selectQuery1.Where().LessThanCondition(COL_CreationTime).Value(context.Query.To.Value);
            selectQuery1.Where().GreaterThanCondition(COL_TimeStamp).Value(context.MaxTimeStamp);
            selectQuery1.Sort().ByColumn(COL_TimeStamp, RDBSortDirection.ASC);

            var selectQuery2 = queryContext.AddSelectQuery();
            selectQuery2.From(tempTableQuery, "tmp");
            selectQuery2.SelectColumns().AllTableColumns("tmp");
            var selectQuery3 = queryContext.AddSelectQuery();
            selectQuery3.From(tempTableQuery, "tmp");
            var selectAggregate = selectQuery3.SelectAggregates();
            selectAggregate.Count("RecordsCount");

            var selectQuery4 = queryContext.AddSelectQuery();
            selectQuery4.From(tempTableQuery, "tmp");
            var selectAggregate2 = selectQuery4.SelectAggregates();
            selectAggregate2.Aggregate(RDBNonCountAggregateType.MAX, "tmp", COL_TimeStamp, "MaxTimestamp");

            List<VRNotification> notifications = new List<VRNotification>();
            queryContext.ExecuteReader(reader => 
            {
                while (reader.Read())
                    notifications.Add(VRNotificationMapper(reader));
                if (reader.NextResult())
                    while (reader.Read())
                    {
                        if (reader.GetInt("RecordsCount")==0)
                        {
                            context.MaxTimeStamp = context.MaxTimeStamp;
                        }
                        else
                        {
                            context.MaxTimeStamp = reader.GetBytes("MaxTimestamp");
                        }
                    }
            });
            return notifications;
        }
        public void GetBeforeIdVRNotifications(IVRNotificationBeforeIdContext context)
        {
            bool isFinalRow = false;
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, context.NbOfRows, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            var whereQuery = selectQuery.Where();
            whereQuery.EqualsCondition(COL_TypeID).Value(context.NotificationTypeId);
            if (context.Query.Description != null)
                whereQuery.ContainsCondition(COL_Description, context.Query.Description);
            if (context.Query.StatusIds != null && context.Query.StatusIds.Count > 0)
                whereQuery.ListCondition(COL_Status, RDBListConditionOperator.IN, context.Query.StatusIds);
            if (context.Query.AlertLevelIds != null && context.Query.AlertLevelIds.Count > 0)
                whereQuery.ListCondition(COL_AlertLevelID, RDBListConditionOperator.IN, context.Query.AlertLevelIds);
            if (context.Query.From.HasValue)
                whereQuery.GreaterOrEqualCondition(COL_CreationTime).Value(context.Query.From.Value);
            if (context.Query.To.HasValue)
                whereQuery.LessThanCondition(COL_CreationTime).Value(context.Query.To.Value);
            whereQuery.LessThanCondition(COL_ID).Value(context.LessThanID);
            selectQuery.Sort().ByColumn(COL_ID, RDBSortDirection.DESC);

            queryContext.ExecuteReader(reader =>
            {
                while (reader.Read() && !isFinalRow)
                    isFinalRow = context.onItemReady(VRNotificationMapper(reader));
            });

        }
        #endregion
    }
}
