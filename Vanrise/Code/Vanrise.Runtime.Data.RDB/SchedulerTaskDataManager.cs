using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Data.RDB
{
    public class SchedulerTaskDataManager : ISchedulerTaskDataManager
    {
        static string TABLE_NAME = "runtime_ScheduleTask";
        static string TABLE_ALIAS = "STask";

        const string COL_Id = "Id";
        const string COL_Name = "Name";
        const string COL_IsEnabled = "IsEnabled";
        const string COL_TaskType = "TaskType";
        const string COL_TriggerTypeId = "TriggerTypeId";
        const string COL_ActionTypeId = "ActionTypeId";
        const string COL_TaskSettings = "TaskSettings";
        const string COL_OwnerId = "OwnerId";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static SchedulerTaskDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_Id, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
            columns.Add(COL_IsEnabled, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_TaskType, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_TriggerTypeId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_ActionTypeId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_TaskSettings, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar });
            columns.Add(COL_OwnerId, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "runtime",
                DBTableName = "ScheduleTask",
                Columns = columns,
                IdColumnName = COL_Id,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("RuntimeConfig", "RuntimeConfigDBConnStringKey", "RuntimeConfigDBConnString");
        }

        #region Public Methods

        public bool AddTask(SchedulerTask taskObject)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.Column(COL_Id).Value(taskObject.TaskId);
            insertQuery.Column(COL_Name).Value(taskObject.Name);
            insertQuery.Column(COL_IsEnabled).Value(taskObject.IsEnabled);
            insertQuery.Column(COL_TaskType).Value((int)taskObject.TaskType);
            insertQuery.Column(COL_TriggerTypeId).Value(taskObject.TriggerTypeId);
            insertQuery.Column(COL_ActionTypeId).Value(taskObject.ActionTypeId);
            insertQuery.Column(COL_TaskSettings).Value(Common.Serializer.Serialize(taskObject.TaskSettings));
            insertQuery.Column(COL_OwnerId).Value(taskObject.OwnerId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool AreSchedulerTasksUpdated(ref object lastReceivedDataInfo)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref lastReceivedDataInfo);
        }

        public bool DeleteTask(Guid taskId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);
            deleteQuery.Where().EqualsCondition(COL_Id).Value(taskId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool DisableTask(Guid taskId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_IsEnabled).Value(false);

            updateQuery.Where().EqualsCondition(COL_Id).Value(taskId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool EnableTask(Guid taskId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_IsEnabled).Value(true);

            updateQuery.Where().EqualsCondition(COL_Id).Value(taskId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public IEnumerable<SchedulerTask> GetSchedulerTasks()
        {
            var schedulerTaskTriggerTypeDataManager = new SchedulerTaskTriggerTypeDataManager();
            var schedulerTaskActionTypeDataManager = new SchedulerTaskActionTypeDataManager();

            var schedulerTaskTriggerTypeTableALias = "TR";
            var schedulerTaskActionTypeTableALias = "AC";

            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);

            var selectColumns = selectQuery.SelectColumns();
            selectColumns.AllTableColumns(TABLE_ALIAS);
            selectColumns.Column(schedulerTaskTriggerTypeTableALias, SchedulerTaskTriggerTypeDataManager.COL_TriggerTypeInfo, "TriggerTypeInfo");
            selectColumns.Column(schedulerTaskActionTypeTableALias, SchedulerTaskActionTypeDataManager.COL_ActionTypeInfo, "ActionTypeInfo");

            var joinContext = selectQuery.Join();
            schedulerTaskTriggerTypeDataManager.JoinScheduleTaskTriggerType(joinContext, schedulerTaskTriggerTypeTableALias, TABLE_ALIAS, COL_TriggerTypeId);
            schedulerTaskActionTypeDataManager.JoinScheduleTaskActionType(joinContext, schedulerTaskActionTypeTableALias, TABLE_ALIAS, COL_ActionTypeId);

            selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);

            return queryContext.GetItems(TaskMapper);
        }

        public bool UpdateTaskInfo(Guid taskId, string name, bool isEnabled, Guid triggerTypeId, Guid actionTypeId, SchedulerTaskSettings taskSettings)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_Name).Value(name);
            updateQuery.Column(COL_IsEnabled).Value(isEnabled);
            updateQuery.Column(COL_TriggerTypeId).Value(triggerTypeId);
            updateQuery.Column(COL_ActionTypeId).Value(actionTypeId);
            updateQuery.Column(COL_TaskSettings).Value(Common.Serializer.Serialize(taskSettings));

            updateQuery.Where().EqualsCondition(COL_Id).Value(taskId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        #endregion

        #region mapper
        SchedulerTask TaskMapper(IRDBDataReader reader)
        {
            SchedulerTask task = new SchedulerTask
            {
                TaskId = reader.GetGuid(COL_Id),
                Name = reader.GetString(COL_Name),
                IsEnabled = reader.GetBoolean(COL_IsEnabled),
                TriggerTypeId = reader.GetGuid(COL_TriggerTypeId),
                ActionTypeId = reader.GetGuid(COL_ActionTypeId),
                TriggerInfo = Common.Serializer.Deserialize<TriggerTypeInfo>(reader.GetString("TriggerTypeInfo")),
                ActionInfo = Common.Serializer.Deserialize<ActionTypeInfo>(reader.GetString("ActionTypeInfo")),
                TaskSettings = Common.Serializer.Deserialize<SchedulerTaskSettings>(reader.GetString(COL_TaskSettings)),
                OwnerId = reader.GetInt(COL_OwnerId)
            };
            return task;
        }
        #endregion
    }
}
