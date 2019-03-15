using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;
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

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_Status).Value((int)BPTaskStatus.Cancelled);

            var whereContext = updateQuery.Where();
            whereContext.EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId);
            whereContext.NotEqualsCondition(COL_Status).Value((int)BPTaskStatus.Completed);

            queryContext.ExecuteNonQuery();
        }
        public List<Entities.BPTask> GetBeforeId(long lessThanID, int nbOfRows, int? processInstanceId, int? userId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, nbOfRows, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.LessThanCondition(COL_ID).Value(lessThanID);

            if (processInstanceId.HasValue)
                whereContext.EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId.Value);

            createUserIdCondition(userId, whereContext);

            var sortContext = selectQuery.Sort();
            sortContext.ByColumn(COL_ID, RDBSortDirection.DESC);

            return queryContext.GetItems(BPTaskMapper);
        }
        public Entities.BPTask GetTask(long taskId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, 1, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_ID).Value(taskId);

            return queryContext.GetItem(BPTaskMapper);
        }
        public List<Entities.BPTask> GetUpdated(ref object lastUpdateHandle, int nbOfRows, int? processInstanceId, int? userId)
        {
            List<Entities.BPTask> bpTasks = new List<Entities.BPTask>();

            DateTime? afterLastUpdateTime;
            long? afterId;
            SplitLastUpdateHandle(lastUpdateHandle, out afterLastUpdateTime, out afterId);

            DateTime? maxLastUpdateTime_local = null;
            long? id_local = null;

            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, nbOfRows, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();

            if (processInstanceId.HasValue)
                whereContext.EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId.Value);
            createUserIdCondition(userId, whereContext);

            if (lastUpdateHandle == null)
            {
                var sortContext = selectQuery.Sort();
                sortContext.ByColumn(COL_ID, RDBSortDirection.DESC);
            }
            else
            {
                var lastUpdatedTimeConditionContext = whereContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
                lastUpdatedTimeConditionContext.GreaterThanCondition(COL_LastUpdatedTime).Value(afterLastUpdateTime.Value);

                var lastUpdatedTimeSecondConditionContext = lastUpdatedTimeConditionContext.ChildConditionGroup(RDBConditionGroupOperator.AND);
                lastUpdatedTimeSecondConditionContext.EqualsCondition(COL_LastUpdatedTime).Value(afterLastUpdateTime.Value);

                lastUpdatedTimeSecondConditionContext.GreaterThanCondition(COL_ID).Value(afterId.Value);

                var sortContext = selectQuery.Sort();
                sortContext.ByColumn(COL_LastUpdatedTime, RDBSortDirection.ASC);
                sortContext.ByColumn(COL_ID, RDBSortDirection.ASC);
            }

            queryContext.ExecuteReader(reader =>
            {
                while (reader.Read())
                {
                    Entities.BPTask bpTask = BPTaskMapper(reader);

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
            if (input.ExecutionInformation != null)
                updateQuery.Column(COL_TaskExecutionInformation).Value(Serializer.Serialize(input.ExecutionInformation));
            else
                updateQuery.Column(COL_TaskExecutionInformation).Null();
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

        private void createUserIdCondition(int? userId, RDBConditionContext whereContext)
        {
            if (!userId.HasValue)
                return;

            var compareConditionContext = whereContext.CompareCondition(RDBCompareConditionOperator.Contains);

            var textConcat = compareConditionContext.Expression1().TextConcatenation();
            textConcat.Expression1().Value(",");
            var textConcat1 = textConcat.Expression2().TextConcatenation();
            textConcat1.Expression1().Column(COL_AssignedUsers);
            textConcat1.Expression2().Value(",");

            compareConditionContext.Expression2().Value($",{userId.Value},");
        }
        #endregion
        #region Mappers
        Entities.BPTask BPTaskMapper(IRDBDataReader reader)
        {
            var bpTask = new Entities.BPTask
            {
                BPTaskId = reader.GetLong(COL_ID),
                ProcessInstanceId = reader.GetLong(COL_ProcessInstanceID),
                TypeId = reader.GetGuid(COL_TypeID),
                Title = reader.GetString(COL_Title),
                ExecutedById = reader.GetNullableInt(COL_ExecutedBy),
                Status = (BPTaskStatus)reader.GetInt(COL_Status),
                CreatedTime = reader.GetDateTime(COL_CreatedTime),
                LastUpdatedTime = reader.GetDateTime(COL_LastUpdatedTime),
                AssignedUsersDescription = reader.GetString(COL_AssignedUsersDescription),
                Notes = reader.GetString(COL_Notes),
                Decision = reader.GetString(COL_Decision)
            };

            string taskData = reader.GetString(COL_TaskData);
            if (!String.IsNullOrWhiteSpace(taskData))
                bpTask.TaskData = Serializer.Deserialize<BPTaskData>(taskData);

            string taskExecutionInformation = reader.GetString(COL_TaskExecutionInformation);
            if (!String.IsNullOrWhiteSpace(taskExecutionInformation))
            {
                bpTask.TaskExecutionInformation = Serializer.Deserialize<BPTaskExecutionInformation>(taskExecutionInformation);
            }

            string assignedUsers = reader.GetString(COL_AssignedUsers);
            if (!String.IsNullOrWhiteSpace(assignedUsers))
                bpTask.AssignedUsers = assignedUsers.Split(',').Select(itm => int.Parse(itm)).ToList();

            return bpTask;
        }

        public bool UpdateTask(long taskId, BPTaskData taskData)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            if (taskData != null)
                updateQuery.Column(COL_TaskData).Value(Serializer.Serialize(taskData));
            else
                updateQuery.Column(COL_TaskData).Null();
            updateQuery.Column(COL_LastUpdatedTime).DateNow();
            updateQuery.Where().EqualsCondition(COL_ID).Value(taskId);
            return queryContext.ExecuteNonQuery() > 0;
        }

        #endregion
    }
}
