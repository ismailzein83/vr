using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Data.RDB
{
    public class SchedulerTaskStateDataManager : ISchedulerTaskStateDataManager
    {
        static string TABLE_NAME = "runtime_ScheduleTaskState";
        static string TABLE_ALIAS = "STState";

        const string COL_TaskId = "TaskId";
        const string COL_Status = "Status";
        const string COL_LastRunTime = "LastRunTime";
        const string COL_NextRunTime = "NextRunTime";
        const string COL_LockedByProcessID = "LockedByProcessID";
        const string COL_ExecutionInfo = "ExecutionInfo";

        static SchedulerTaskStateDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_TaskId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Status, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_LastRunTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_NextRunTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LockedByProcessID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_ExecutionInfo, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "runtime",
                DBTableName = "ScheduleTaskState",
                Columns = columns,
                IdColumnName = COL_TaskId
            });
        }
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("Runtime", "RuntimeConnStringKey", "RuntimeDBConnString");
        }

        #region Public Methods
        public void DeleteTaskState(Guid taskId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);
            deleteQuery.Where().EqualsCondition(COL_TaskId).Value(taskId);

            queryContext.ExecuteNonQuery();
        }

        public List<SchedulerTaskState> GetAllScheduleTaskStates()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            return queryContext.GetItems(TaskStateMapper);
        }

        public SchedulerTaskState GetSchedulerTaskStateByTaskId(Guid taskId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            selectQuery.Where().EqualsCondition(COL_TaskId).Value(taskId);

            return queryContext.GetItem(TaskStateMapper);
        }

        public List<SchedulerTaskState> GetSchedulerTaskStateByTaskIds(List<Guid> taskIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            selectQuery.Where().ListCondition(COL_TaskId, RDBListConditionOperator.IN, taskIds);

            return queryContext.GetItems(TaskStateMapper);
        }

        public void InsertSchedulerTaskState(Guid taskId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.Column(COL_TaskId).Value(taskId);
            insertQuery.Column(COL_Status).Value((int)SchedulerTaskStatus.NotStarted);

            insertQuery.IfNotExists(TABLE_ALIAS).EqualsCondition(COL_TaskId).Value(taskId);

            queryContext.ExecuteNonQuery();
        }

        public void RunSchedulerTask(Guid taskId, bool allowRunIfEnabled)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_NextRunTime).DateNow();

            var whereContext = updateQuery.Where();
            whereContext.EqualsCondition(COL_TaskId).Value(taskId);
            whereContext.NullCondition().Column(COL_LockedByProcessID);
            whereContext.ListCondition(COL_Status, RDBListConditionOperator.IN, new List<int>() { (int)(SchedulerTaskStatus.NotStarted), (int)(SchedulerTaskStatus.Completed), (int)(SchedulerTaskStatus.Failed) });
            
            if (!allowRunIfEnabled)
                whereContext.NotNullCondition().Column(COL_NextRunTime);

            queryContext.ExecuteNonQuery();
        }

        public bool UpdateTaskState(SchedulerTaskState taskStateObject)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_Status).Value((int)taskStateObject.Status);

            if (taskStateObject.NextRunTime.HasValue)
                updateQuery.Column(COL_NextRunTime).Value(taskStateObject.NextRunTime.Value);
            else
                updateQuery.Column(COL_NextRunTime).Null();

            if (taskStateObject.LastRunTime.HasValue)
                updateQuery.Column(COL_LastRunTime).Value(taskStateObject.LastRunTime.Value);
            else
                updateQuery.Column(COL_LastRunTime).Null();

            if (taskStateObject.ExecutionInfo != null)
                updateQuery.Column(COL_ExecutionInfo).Value(Common.Serializer.Serialize(taskStateObject.ExecutionInfo));
            else
                updateQuery.Column(COL_ExecutionInfo).Null();

            updateQuery.Where().EqualsCondition(COL_TaskId).Value(taskStateObject.TaskId);

            return queryContext.ExecuteNonQuery() > 0;
        }
        #endregion
        #region Mappers
        SchedulerTaskState TaskStateMapper(IRDBDataReader reader)
        {
            var schedulerTaskState = new SchedulerTaskState
            {
                TaskId = reader.GetGuid(COL_TaskId),
                Status = (SchedulerTaskStatus)reader.GetInt(COL_Status),
                LastRunTime = reader.GetNullableDateTime(COL_LastRunTime),
                NextRunTime = reader.GetNullableDateTime(COL_NextRunTime),
            };
            string serializedSettings = reader.GetString(COL_ExecutionInfo);
            if (serializedSettings != null)
                schedulerTaskState.ExecutionInfo = Common.Serializer.Deserialize<Object>(serializedSettings);
            return schedulerTaskState;
        }
        #endregion
    }
}
