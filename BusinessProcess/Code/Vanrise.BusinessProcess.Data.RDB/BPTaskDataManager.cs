using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;

namespace Vanrise.BusinessProcess.Data.RDB
{
    public class BPTaskDataManager : IBPTaskDataManager
    {
        static string TABLE_NAME = "bp_BPTask";
        static string TABLE_ALIAS = "BPTask";

        const string COL_ID = "ID";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_TypeID = "TypeID";
        const string COL_Title = "Title";
        const string COL_AssignedUsers = "AssignedUsers";
        const string COL_AssignedUsersDescription = "AssignedUsersDescription";
        const string COL_ExecutedBy = "ExecutedBy";
        const string COL_Status = "Status";
        const string COL_TaskData = "TaskData";
        const string COL_TaskExecutionInformation = "TaskExecutionInformation";
        const string COL_Notes = "Notes";
        const string COL_Decision = "Decision";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastUpdatedTime = "LastUpdatedTime";


        static BPTaskDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_ProcessInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_TypeID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Title, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_AssignedUsers, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar });
            columns.Add(COL_AssignedUsersDescription, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_ExecutedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Status, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_TaskData, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_TaskExecutionInformation, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar });
            columns.Add(COL_Notes, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_Decision, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastUpdatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "bp",
                DBTableName = "BPTask",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime
            });
        }
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("BusinessProcess", "BusinessProcessDBConnStringKey", "BusinessProcessDBConnString");
        }

        #region Public Methods
        public void CancelNotCompletedTasks(long processInstanceId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            // --Status: 50 = Completed, 60 = Canceled

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_Status).Value(60);

            var whereContext = updateQuery.Where();
            whereContext.EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId);
            whereContext.NotEqualsCondition(COL_Status).Value(50);

            queryContext.ExecuteNonQuery();
        }
        public List<BPTask> GetBeforeId(long lessThanID, int nbOfRows, int? processInstanceId, int? userId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, nbOfRows, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.LessThanCondition(COL_ID).Value(lessThanID);

            if (processInstanceId.HasValue)
                whereContext.EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId.Value);

            if (userId.HasValue)
                whereContext.ContainsCondition(COL_AssignedUsers, userId.Value.ToString());

            var sortContext = selectQuery.Sort();
            sortContext.ByColumn(COL_ID, RDBSortDirection.DESC);

            return queryContext.GetItems(BPTaskMapper);
        }
        public BPTask GetTask(long taskId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, 1, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_ID).Value(taskId);

            return queryContext.GetItem(BPTaskMapper);
        }
        public List<BPTask> GetUpdated(ref object lastUpdateHandle, int nbOfRows, int? processInstanceId, int? userId)
        {
            List<BPTask> bpTasks = new List<BPTask>();

            DateTime? afterLastUpdateTime;
            long? afterId;
            SplitLastUpdateHandle(lastUpdateHandle, out afterLastUpdateTime, out afterId);

            DateTime? maxLastUpdateTime_local = null;
            long? id_local = null;

            var queryContext = new RDBQueryContext(GetDataProvider());

            if (!afterLastUpdateTime.HasValue)
            {
                var selectQuery = queryContext.AddSelectQuery();
                selectQuery.From(TABLE_NAME, TABLE_ALIAS, nbOfRows, true);
                selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

                var whereContext = selectQuery.Where();

                if (processInstanceId.HasValue)
                    whereContext.EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId.Value);

                if (userId.HasValue)
                    whereContext.ContainsCondition(COL_AssignedUsers, userId.Value.ToString());

                var sortContext = selectQuery.Sort();
                sortContext.ByColumn(COL_ID, RDBSortDirection.DESC);
            }
            else
            {
                var selectQuery = queryContext.AddSelectQuery();
                selectQuery.From(TABLE_NAME, TABLE_ALIAS, nbOfRows, true);
                selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

                var whereContext = selectQuery.Where();

                if (processInstanceId.HasValue)
                    whereContext.EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId.Value);

                if (userId.HasValue)
                    whereContext.ContainsCondition(COL_AssignedUsers, userId.Value.ToString());

                var lastUpdatedTimeConditionContext = whereContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
                lastUpdatedTimeConditionContext.GreaterThanCondition(COL_LastUpdatedTime).Value(afterLastUpdateTime.Value);

                var lastUpdatedTimeSecondConditionContext = lastUpdatedTimeConditionContext.ChildConditionGroup(RDBConditionGroupOperator.AND);
                lastUpdatedTimeSecondConditionContext.EqualsCondition(COL_LastUpdatedTime).Value(afterLastUpdateTime.Value);
                if (afterId.HasValue)
                    lastUpdatedTimeSecondConditionContext.GreaterThanCondition(COL_ID).Value(afterId.Value);
                else
                    lastUpdatedTimeSecondConditionContext.FalseCondition();

                var sortContext = selectQuery.Sort();
                sortContext.ByColumn(COL_LastUpdatedTime, RDBSortDirection.ASC);
                sortContext.ByColumn(COL_ID, RDBSortDirection.ASC);
            }

            queryContext.ExecuteReader(reader =>
            {
                while (reader.Read())
                {
                    BPTask bpTask = BPTaskMapper(reader);

                    if (!maxLastUpdateTime_local.HasValue || maxLastUpdateTime_local.Value < bpTask.LastUpdatedTime)
                    {
                        maxLastUpdateTime_local = bpTask.LastUpdatedTime;
                        id_local = bpTask.BPTaskId;
                    }
                    else if (maxLastUpdateTime_local.Value == bpTask.LastUpdatedTime && bpTask.BPTaskId > id_local.Value)
                    {
                        id_local = bpTask.BPTaskId;
                    }

                    bpTasks.Add(bpTask);
                }
            });

            object lastUpdateHandle_local = BuildLastUpdateHandle(maxLastUpdateTime_local, id_local);

            if (lastUpdateHandle_local != null)
                lastUpdateHandle = lastUpdateHandle_local;

            return bpTasks;
        }
        public bool InsertTask(string title, long processInstanceId, Guid typeId, List<int> assignedUserIds, BPTaskStatus bpTaskStatus, BPTaskData taskData, string assignedUsersDescription, out long taskId)
        {

            string assignedUsers = null;
            if (assignedUserIds != null && assignedUserIds.Count > 0)
                assignedUsers = string.Join<int>(",", assignedUserIds);


            taskId = default(long);

            var queryContext = new RDBQueryContext(GetDataProvider());

            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.Column(COL_Title).Value(title);
            insertQuery.Column(COL_ProcessInstanceID).Value(processInstanceId);
            insertQuery.Column(COL_TypeID).Value(typeId);
            if (taskData != null)
                insertQuery.Column(COL_TaskData).Value(Serializer.Serialize(taskData));
            insertQuery.Column(COL_Status).Value((int)bpTaskStatus);
            insertQuery.Column(COL_AssignedUsers).Value(assignedUsers);
            insertQuery.Column(COL_AssignedUsersDescription).Value(assignedUsersDescription);
            insertQuery.Column(COL_LastUpdatedTime).DateNow();

            insertQuery.AddSelectGeneratedId();

            long result = queryContext.ExecuteScalar().LongValue;

            if (result > 0)
            {
                taskId = result;
                return true;
            }
            else
            {
                return false;
            }
        }
        public void UpdateTaskExecution(ExecuteBPTaskInput input, BPTaskStatus bpTaskStatus)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_ExecutedBy).Value(input.ExecutedBy);
            updateQuery.Column(COL_Status).Value((int)bpTaskStatus);
            updateQuery.Column(COL_TaskExecutionInformation).Value(input.ExecutionInformation != null ? Serializer.Serialize(input.ExecutionInformation) : null);
            updateQuery.Column(COL_Notes).Value(input.Notes);
            updateQuery.Column(COL_Decision).Value(input.Decision);
            updateQuery.Column(COL_LastUpdatedTime).DateNow();

            var whereContext = updateQuery.Where();
            whereContext.EqualsCondition(COL_ID).Value(input.TaskId);

            queryContext.ExecuteNonQuery();
        }
        #endregion

        #region Private Methods
        private object BuildLastUpdateHandle(DateTime? lastUpdateTime, long? id)
        {
            if (lastUpdateTime.HasValue && id.HasValue)
                return string.Concat(lastUpdateTime.Value.ToString("yyyyMMddHHmmssfff"), "@", id.Value);

            return null;
        }

        private void SplitLastUpdateHandle(object lastUpdateHandle, out DateTime? lastUpdateTime, out long? id)
        {
            lastUpdateTime = null;
            id = null;

            if (lastUpdateHandle != null)
            {
                string[] data = lastUpdateHandle.ToString().Split('@');
                lastUpdateTime = DateTime.ParseExact(data[0], "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
                id = long.Parse(data[1]);
            }
        }
        #endregion
        #region Mappers
        BPTask BPTaskMapper(IRDBDataReader reader)
        {
            var bpTask = new BPTask
            {
                BPTaskId = reader.GetLong("ID"),
                ProcessInstanceId = reader.GetLong("ProcessInstanceID"),
                TypeId = reader.GetGuid("TypeID"),
                Title = reader.GetString("Title"),
                ExecutedById = reader.GetNullableInt("ExecutedBy"),
                Status = (BPTaskStatus)reader.GetInt("Status"),
                CreatedTime = reader.GetDateTime("CreatedTime"),
                LastUpdatedTime = reader.GetDateTime("LastUpdatedTime"),
                AssignedUsersDescription = reader.GetString("AssignedUsersDescription"),
                Notes = reader.GetString("Notes"),
                Decision = reader.GetString("Decision")
            };

            string taskData = reader.GetString("TaskData");
            if (!String.IsNullOrWhiteSpace(taskData))
                bpTask.TaskData = Serializer.Deserialize<BPTaskData>(taskData);

            string taskExecutionInformation = reader.GetString("TaskExecutionInformation");
            if (!String.IsNullOrWhiteSpace(taskExecutionInformation))
            {
                bpTask.TaskExecutionInformation = Serializer.Deserialize<BPTaskExecutionInformation>(taskExecutionInformation);
            }

            string assignedUsers = reader.GetString("AssignedUsers");
            if (!String.IsNullOrWhiteSpace(assignedUsers))
                bpTask.AssignedUsers = assignedUsers.Split(',').Select(itm => int.Parse(itm)).ToList();

            return bpTask;
        }
        #endregion
    }
}
