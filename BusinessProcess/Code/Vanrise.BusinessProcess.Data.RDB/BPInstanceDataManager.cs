using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Data.RDB
{
    public class BPInstanceDataManager : IBPInstanceDataManager
    {
        #region Constructor

        static string BPINSTANCE_NAME = "bp_BPInstance";
        static string BPINSTANCE_ALIAS = "BPInstance";
        static string BPINSTANCEARCHIVE_Name = "bp_BPInstance_Archived";
        static string BPINSTANCEARCHIVE_ALIAS = "BPInstanceArc";

        const string COL_ID = "ID";
        const string COL_Title = "Title";
        const string COL_ParentID = "ParentID";
        const string COL_DefinitionID = "DefinitionID";
        const string COL_ServiceInstanceID = "ServiceInstanceID";
        const string COL_InitiatorUserId = "InitiatorUserId";
        const string COL_WorkflowInstanceID = "WorkflowInstanceID";
        const string COL_InputArgument = "InputArgument";
        const string COL_CompletionNotifier = "CompletionNotifier";
        const string COL_ExecutionStatus = "ExecutionStatus";
        const string COL_AssignmentStatus = "AssignmentStatus";
        const string COL_LastMessage = "LastMessage";
        const string COL_EntityId = "EntityId";
        const string COL_ViewRequiredPermissionSetId = "ViewRequiredPermissionSetId";
        const string COL_CancellationRequestUserId = "CancellationRequestUserId";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_StatusUpdatedTime = "StatusUpdatedTime";
        const string COL_TaskId = "TaskId";

        const string PendingInstanceTimeColumnName = "PendingInstanceTime";
        const string RunningProcessNumberColumnName = "RunningProcessNumber";
        const string MaxGlobalStatusUpdatedTime = "MaxGlobalStatusUpdatedTime";
        const string GlobalId = "GlobalId";
        static BPInstanceDataManager()
        {
            #region BPInstance Table Registration

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(BPINSTANCE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "bp",
                DBTableName = "BPInstance",
                Columns = GetRDBTableColumnDefinitionDictionary(),
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime
            });
            #endregion

            #region BPInstance Archive Table Registration

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(BPINSTANCEARCHIVE_Name, new RDBTableDefinition
            {
                DBSchemaName = "bp",
                DBTableName = "BPInstance_Archived",
                Columns = GetRDBTableColumnDefinitionDictionary(),
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime
            });
            #endregion

        }

        #endregion

        #region Public Methods

        public Dictionary<Guid, Type> InputArgumentTypeByDefinitionId { get; set; }

        public void ArchiveInstances(List<BPInstanceStatus> completedStatuses, DateTime completedBefore, int nbOfInstances)
        {
            string tempTableAlias = "tempTable";
            var queryContext = new RDBQueryContext(GetDataProvider());
            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumnsFromTable(BPINSTANCE_NAME);

            var insertToTempTableQuery = queryContext.AddInsertQuery();
            insertToTempTableQuery.IntoTable(tempTableQuery);

            var fromSelectQuery = insertToTempTableQuery.FromSelect();
            fromSelectQuery.From(BPINSTANCE_NAME, BPINSTANCE_ALIAS, nbOfInstances, true);
            fromSelectQuery.SelectColumns().AllTableColumns(BPINSTANCE_ALIAS);

            var fromSelectWhereContext = fromSelectQuery.Where();
            fromSelectWhereContext.LessThanCondition(COL_StatusUpdatedTime).Value(completedBefore);

            BuildExecutionStatusFilter(completedStatuses, fromSelectWhereContext);

            var fromSelectQuerySortContext = fromSelectQuery.Sort();
            fromSelectQuerySortContext.ByColumn(COL_ID, RDBSortDirection.ASC);

            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(BPINSTANCEARCHIVE_Name);

            var insertToArchiveFromSelectQuery = insertQuery.FromSelect();
            insertToArchiveFromSelectQuery.From(tempTableQuery, tempTableAlias, null, false);
            insertToArchiveFromSelectQuery.SelectColumns().AllTableColumns(tempTableAlias);

            var joinContext = insertToArchiveFromSelectQuery.Join();

            var joinStatement = joinContext.Join(BPINSTANCEARCHIVE_Name, BPINSTANCEARCHIVE_ALIAS);
            joinStatement.JoinType(RDBJoinType.Left);
            var joinCondition = joinStatement.On();
            joinCondition.EqualsCondition(BPINSTANCEARCHIVE_ALIAS, COL_ID, tempTableAlias, COL_ID);

            var insertToArchiveFromSelectWhereCondition = insertToArchiveFromSelectQuery.Where();
            insertToArchiveFromSelectWhereCondition.NullCondition(BPINSTANCEARCHIVE_ALIAS, COL_ID);

            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(BPINSTANCE_NAME);
            var deleteJoinStatement = deleteQuery.Join(BPINSTANCE_ALIAS);
            deleteJoinStatement.JoinOnEqualOtherTableColumn(RDBJoinType.Inner, tempTableQuery, tempTableAlias, COL_ID, BPINSTANCE_ALIAS, COL_ID);

            queryContext.ExecuteNonQuery();
        }

        public List<BPInstance> GetAfterId(long? processInstanceId, Guid bpDefinitionId, bool getFromArchive)
        {
            string tableName, tableAlias;
            GetTableNameAndAlias(getFromArchive, out tableName, out tableAlias);

            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(tableName, tableAlias, null, true);
            selectQuery.SelectColumns().AllTableColumns(tableAlias);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_DefinitionID).Value(bpDefinitionId);

            if (processInstanceId.HasValue)
                whereContext.GreaterThanCondition(COL_ID).Value(processInstanceId.Value);

            return queryContext.GetItems(BPInstanceMapper);
        }

        public List<BPInstance> GetBeforeId(BPInstanceBeforeIdInput input, List<int> grantedPermissionSetIds, bool getFromArchive)
        {
            string tableName, tableAlias;
            GetTableNameAndAlias(getFromArchive, out tableName, out tableAlias);

            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(tableName, tableAlias, input.NbOfRows, true);
            selectQuery.SelectColumns().AllTableColumns(tableAlias);

            var whereContext = selectQuery.Where();

            whereContext.LessThanCondition(COL_ID).Value(input.LessThanID);
            BuildFilters(input.DefinitionsId, input.ParentId, input.EntityIds, grantedPermissionSetIds, whereContext);
            BuildTaskIdFilter(input.TaskId, whereContext);

            var sortContext = selectQuery.Sort();
            sortContext.ByColumn(COL_ID, RDBSortDirection.DESC);

            return queryContext.GetItems(BPInstanceMapper);
        }

        public List<BPDefinitionSummary> GetBPDefinitionSummary(IEnumerable<BPInstanceStatus> executionStatus)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(BPINSTANCE_NAME, BPINSTANCE_ALIAS, null, true);

            var groupByContext = selectQuery.GroupBy();
            groupByContext.Select().Column(COL_DefinitionID);
            groupByContext.SelectAggregates().Aggregate(RDBNonCountAggregateType.MAX, BPINSTANCE_ALIAS, COL_CreatedTime, PendingInstanceTimeColumnName);
            groupByContext.SelectAggregates().Count(RunningProcessNumberColumnName);

            var whereContext = selectQuery.Where();
            BuildExecutionStatusFilter(executionStatus, whereContext);

            return queryContext.GetItems<BPDefinitionSummary>(BPDefinitionSummaryMapper);
        }

        public BPInstance GetBPInstance(long bpInstanceId, bool getFromArchive)
        {
            string tableName, tableAlias;
            GetTableNameAndAlias(getFromArchive, out tableName, out tableAlias);

            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(tableName, "BPInstance", 1, true);
            selectQuery.SelectColumns().AllTableColumns("BPInstance");

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_ID).Value(bpInstanceId);

            return queryContext.GetItem(BPInstanceMapper);
        }

        public List<BPInstance> GetFilteredBPInstances(BPInstanceQuery query, List<int> grantedPermissionSetIds, bool getFromArchive)
        {
            string tableName, tableAlias;
            GetTableNameAndAlias(getFromArchive, out tableName, out tableAlias);
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQueryContext = queryContext.AddSelectQuery();
            selectQueryContext.From(tableName, tableAlias, query.Top, true);
            selectQueryContext.SelectColumns().AllTableColumns(tableAlias);

            var whereContext = selectQueryContext.Where();
            if (!string.IsNullOrEmpty(query.EntityId))
                whereContext.EqualsCondition(COL_EntityId).Value(query.EntityId);

            BuildDefinitionsIdFilter(query.DefinitionsId, whereContext);
            BuildGrantedPermissionsSetIdsFilter(grantedPermissionSetIds, whereContext);

            if (query.InstanceStatus != null && query.InstanceStatus.Count > 0)
                whereContext.ListCondition(COL_ExecutionStatus, RDBListConditionOperator.IN, query.InstanceStatus.Select(itm => (int)itm));

            if (query.DateFrom.HasValue)
                whereContext.GreaterOrEqualCondition(COL_CreatedTime).Value(query.DateFrom.Value);

            if (query.DateTo.HasValue)
                whereContext.LessOrEqualCondition(COL_CreatedTime).Value(query.DateTo.Value);

            if (query.TaskId.HasValue)
                whereContext.EqualsCondition(COL_TaskId).Value(query.TaskId.Value);

            selectQueryContext.Sort().ByColumn(COL_ID, RDBSortDirection.DESC);

            return queryContext.GetItems(BPInstanceMapper);
        }

        public List<BPInstance> GetFirstPage(out object lastUpdateHandle, int nbOfRows, List<Guid> definitionsId, int parentId, List<string> entityIds, List<int> grantedPermissionSetIds, Guid? taskId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            //First Select Query
            var selectStatusUpdatedTimeQuery = queryContext.AddSelectQuery();

            selectStatusUpdatedTimeQuery.From(BPINSTANCE_NAME, BPINSTANCE_ALIAS, 1, true);
            var columns = selectStatusUpdatedTimeQuery.SelectColumns();
            columns.Column(COL_StatusUpdatedTime, MaxGlobalStatusUpdatedTime);
            columns.Column(COL_ID, GlobalId);

            var sortContext = selectStatusUpdatedTimeQuery.Sort();
            sortContext.ByColumn(COL_StatusUpdatedTime, RDBSortDirection.DESC);
            sortContext.ByColumn(COL_ID, RDBSortDirection.DESC);

            //Second  Query
            var SelectQuery = queryContext.AddSelectQuery();
            SelectQuery.From(BPINSTANCE_NAME, BPINSTANCE_ALIAS, nbOfRows, true);
            SelectQuery.SelectColumns().AllTableColumns(BPINSTANCE_ALIAS);

            var whereContext = SelectQuery.Where();
            BuildFilters(definitionsId, parentId, entityIds, grantedPermissionSetIds, whereContext);
            BuildTaskIdFilter(taskId, whereContext);

            SelectQuery.Sort().ByColumn(COL_ID, RDBSortDirection.DESC);

            DateTime? maxStatusUpdatedTime_local = null;
            long? id_local = null;

            List<BPInstance> bpInstances = new List<BPInstance>();

            queryContext.ExecuteReader((reader) =>
            {
                if (reader.Read())
                {
                    maxStatusUpdatedTime_local = reader.GetDateTime(MaxGlobalStatusUpdatedTime);
                    id_local = reader.GetLong(GlobalId);
                }
                reader.NextResult();

                while (reader.Read())
                {
                    BPInstance bpInstance = BPInstanceMapper(reader);
                    if (!maxStatusUpdatedTime_local.HasValue || maxStatusUpdatedTime_local.Value < bpInstance.StatusUpdatedTime)
                    {
                        maxStatusUpdatedTime_local = bpInstance.StatusUpdatedTime;
                        id_local = bpInstance.ProcessInstanceID;
                    }
                    else if (maxStatusUpdatedTime_local.Value == bpInstance.StatusUpdatedTime && bpInstance.ProcessInstanceID > id_local.Value)
                    {
                        id_local = bpInstance.ProcessInstanceID;
                    }
                    bpInstances.Add(bpInstance);
                }
            });
            lastUpdateHandle = BuildLastUpdateHandle(maxStatusUpdatedTime_local, id_local);
            return bpInstances;
        }

        public List<BPInstance> GetFirstPageFromArchive(int nbOfRows, List<Guid> definitionsId, int parentId, List<string> entityIds, List<int> grantedPermissionSetIds, Guid? taskId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(BPINSTANCEARCHIVE_Name, BPINSTANCEARCHIVE_ALIAS, nbOfRows, true);
            selectQuery.SelectColumns().AllTableColumns(BPINSTANCEARCHIVE_ALIAS);

            var whereContext = selectQuery.Where();
            BuildFilters(definitionsId, parentId, entityIds, grantedPermissionSetIds, whereContext);
            BuildTaskIdFilter(taskId, whereContext);

            selectQuery.Sort().ByColumn(COL_ID, RDBSortDirection.DESC);

            return queryContext.GetItems(BPInstanceMapper);
        }

        public List<BPInstance> GetPendingInstances(Guid definitionId, IEnumerable<BPInstanceStatus> acceptableBPStatuses, BPInstanceAssignmentStatus assignmentStatus, int maxCounts, Guid serviceInstanceId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(BPINSTANCE_NAME, BPINSTANCE_ALIAS, maxCounts, true);
            selectQuery.SelectColumns().AllTableColumns(BPINSTANCE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_DefinitionID).Value(definitionId);
            whereContext.EqualsCondition(COL_ServiceInstanceID).Value(serviceInstanceId);

            if (assignmentStatus == BPInstanceAssignmentStatus.Free)
            {
                whereContext.ConditionIfColumnNotNull(COL_AssignmentStatus).EqualsCondition(COL_AssignmentStatus).Value((int)assignmentStatus);
            }
            else
            {
                whereContext.NotNullCondition(COL_AssignmentStatus);
                whereContext.EqualsCondition(COL_AssignmentStatus).Value((int)assignmentStatus);
            }

            BuildExecutionStatusFilter(acceptableBPStatuses, whereContext);

            selectQuery.Sort().ByColumn(COL_ID, RDBSortDirection.ASC);

            return queryContext.GetItems(BPInstanceMapper);
        }

        public List<BPInstance> GetPendingInstancesInfo(IEnumerable<BPInstanceStatus> statuses)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(BPINSTANCE_NAME, BPINSTANCE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(BPINSTANCE_ALIAS);

            BuildExecutionStatusFilter(statuses, selectQuery.Where());
            selectQuery.Sort().ByColumn(COL_ID, RDBSortDirection.ASC);
            return queryContext.GetItems(BPInstanceMapper);
        }

        public List<BPInstance> GetUpdated(ref object lastUpdateHandle, int nbOfRows, List<Guid> definitionsId, int parentId, List<string> entityIds, List<int> grantedPermissionSetIds, Guid? taskId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(BPINSTANCE_NAME, BPINSTANCE_ALIAS, nbOfRows, true);
            selectQuery.SelectColumns().AllTableColumns(BPINSTANCE_ALIAS);

            var whereContext = selectQuery.Where();
            BuildDefinitionsIdFilter(definitionsId, whereContext);
            BuildParentIdFilter(parentId, whereContext);
            BuildEntityIdsFilter(entityIds, whereContext);
            BuildGrantedPermissionsSetIdsFilter(grantedPermissionSetIds, whereContext);

            object lastUpdateHandle_local = lastUpdateHandle;

            if (lastUpdateHandle_local != null)
            {
                long? id;
                DateTime? statusUpdateTime = null;
                SplitLastUpdateHandle(lastUpdateHandle_local, out statusUpdateTime, out id);

                var childConditionGroup = whereContext.ChildConditionGroup(RDBConditionGroupOperator.OR);

                childConditionGroup.GreaterThanCondition(COL_StatusUpdatedTime).Value(statusUpdateTime.Value);

                var newchildConditionGroup = childConditionGroup.ChildConditionGroup(RDBConditionGroupOperator.AND);
                newchildConditionGroup.EqualsCondition(COL_StatusUpdatedTime).Value(statusUpdateTime.Value);
                newchildConditionGroup.GreaterThanCondition(COL_ID).Value(id.Value);
            }

            BuildTaskIdFilter(taskId, whereContext);

            var sort = selectQuery.Sort();
            sort.ByColumn(COL_StatusUpdatedTime, RDBSortDirection.ASC);
            sort.ByColumn(COL_ID, RDBSortDirection.ASC);

            DateTime? maxStatusUpdatedTime_local = null;
            long? id_local = null;
            List<BPInstance> bpInstances = new List<BPInstance>();

            queryContext.ExecuteReader((reader) =>
            {
                while (reader.Read())
                {
                    BPInstance bpInstance = BPInstanceMapper(reader);

                    if (!maxStatusUpdatedTime_local.HasValue || maxStatusUpdatedTime_local.Value < bpInstance.StatusUpdatedTime)
                    {
                        maxStatusUpdatedTime_local = bpInstance.StatusUpdatedTime;
                        id_local = bpInstance.ProcessInstanceID;
                    }
                    else if (maxStatusUpdatedTime_local.Value == bpInstance.StatusUpdatedTime && bpInstance.ProcessInstanceID > id_local.Value)
                    {
                        id_local = bpInstance.ProcessInstanceID;
                    }

                    bpInstances.Add(bpInstance);
                }
            });
            object handle = BuildLastUpdateHandle(maxStatusUpdatedTime_local, id_local);

            lastUpdateHandle = handle != null ? handle : lastUpdateHandle_local;
            return bpInstances;
        }

        public bool HasRunningInstances(Guid definitionId, List<string> entityIds, IEnumerable<BPInstanceStatus> statuses)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(BPINSTANCE_NAME, BPINSTANCE_ALIAS, 1, true);
            selectQuery.SelectColumns().Expression(BPINSTANCE_ALIAS).Value(1);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_DefinitionID).Value(definitionId);
            BuildEntityIdsFilter(entityIds, whereContext);

            if (statuses != null && statuses.Count() > 0)
                BuildExecutionStatusFilter(statuses, whereContext);

            return queryContext.ExecuteScalar().NullableIntValue != null ? true : false;
        }

        public long InsertInstance(BPInstanceToAdd bpInstanceToAdd)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(BPINSTANCE_NAME);
            insertQuery.AddSelectGeneratedId();

            bool serializeInputArgumentsWithoutType = false;

            if (InputArgumentTypeByDefinitionId.ContainsKey(bpInstanceToAdd.DefinitionID))
                serializeInputArgumentsWithoutType = true;

            string serializedInputArguments = null;

            if (bpInstanceToAdd.InputArgument != null)
                serializedInputArguments = Serializer.Serialize(bpInstanceToAdd.InputArgument, serializeInputArgumentsWithoutType);

            string serializedCompletionNotifier = null;

            if (bpInstanceToAdd.CompletionNotifier != null)
                serializedCompletionNotifier = Serializer.Serialize(bpInstanceToAdd.CompletionNotifier);

            insertQuery.Column(COL_Title).Value(bpInstanceToAdd.Title);

            if (bpInstanceToAdd.ParentProcessID.HasValue)
                insertQuery.Column(COL_ParentID).Value(bpInstanceToAdd.ParentProcessID.Value);

            insertQuery.Column(COL_DefinitionID).Value(bpInstanceToAdd.DefinitionID);
            insertQuery.Column(COL_InputArgument).Value(serializedInputArguments);
            insertQuery.Column(COL_CompletionNotifier).Value(serializedCompletionNotifier);
            insertQuery.Column(COL_ExecutionStatus).Value((int)bpInstanceToAdd.Status);
            insertQuery.Column(COL_StatusUpdatedTime).DateNow();
            insertQuery.Column(COL_InitiatorUserId).Value(bpInstanceToAdd.InitiatorUserId);
            insertQuery.Column(COL_EntityId).Value(bpInstanceToAdd.EntityId);

            if (bpInstanceToAdd.ViewRequiredPermissionSetId.HasValue)
                insertQuery.Column(COL_ViewRequiredPermissionSetId).Value(bpInstanceToAdd.ViewRequiredPermissionSetId.Value);

            if (bpInstanceToAdd.TaskId.HasValue)
                insertQuery.Column(COL_TaskId).Value(bpInstanceToAdd.TaskId.Value);

            return queryContext.ExecuteScalar().LongValue;
        }

        public void SetCancellationRequestUserId(long bpInstanceId, List<BPInstanceStatus> allowedStatuses, int cancelRequestByUserId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(BPINSTANCE_NAME);
            updateQuery.Column(COL_CancellationRequestUserId).Value(cancelRequestByUserId);

            var whereContext = updateQuery.Where();
            whereContext.EqualsCondition(COL_ID).Value(bpInstanceId);
            whereContext.NullCondition(COL_CancellationRequestUserId);
            whereContext.ListCondition(COL_ExecutionStatus, RDBListConditionOperator.IN, allowedStatuses.Select(itm => (int)itm));

            queryContext.ExecuteNonQuery();
        }

        public void UpdateInstanceAssignmentStatus(long processInstanceId, BPInstanceAssignmentStatus assignmentStatus)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(BPINSTANCE_NAME);
            updateQuery.Column(COL_AssignmentStatus).Value((int)assignmentStatus);

            var whereContext = updateQuery.Where();
            whereContext.EqualsCondition(COL_ID).Value(processInstanceId);

            queryContext.ExecuteNonQuery();
        }

        public void UpdateInstanceLastMessage(long processInstanceId, string message)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(BPINSTANCE_NAME);
            updateQuery.Column(COL_LastMessage).Value(message);

            var whereContext = updateQuery.Where();
            whereContext.EqualsCondition(COL_ID).Value(processInstanceId);

            queryContext.ExecuteNonQuery();
        }

        public void UpdateInstanceStatus(long processInstanceId, BPInstanceStatus status, BPInstanceAssignmentStatus assignmentStatus, string message, bool clearServiceInstanceId, Guid? workflowInstanceId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(BPINSTANCE_NAME);
            updateQuery.Column(COL_ExecutionStatus).Value((int)status);
            updateQuery.Column(COL_AssignmentStatus).Value((int)assignmentStatus);
            updateQuery.Column(COL_StatusUpdatedTime).DateNow();

            if (!string.IsNullOrWhiteSpace(message))
                updateQuery.Column(COL_LastMessage).Value(message);

            if (clearServiceInstanceId)
                updateQuery.Column(COL_ServiceInstanceID).Null();

            if (workflowInstanceId.HasValue)
                updateQuery.Column(COL_WorkflowInstanceID).Value(workflowInstanceId.Value);

            var whereContext = updateQuery.Where();
            whereContext.EqualsCondition(COL_ID).Value(processInstanceId);

            queryContext.ExecuteNonQuery();
        }

        public void UpdateServiceInstancesAndAssignmentStatus(List<BPInstance> pendingInstancesToUpdate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            foreach (var pendingInstance in pendingInstancesToUpdate)
            {
                if (!pendingInstance.ServiceInstanceId.HasValue)
                    throw new NullReferenceException(String.Format("pendingInstance.ServiceInstanceId. ProcessInstanceId '{0}'", pendingInstance.ProcessInstanceID));
                
                var updateQuery = queryContext.AddUpdateQuery();
                updateQuery.FromTable(BPINSTANCE_NAME);
                updateQuery.Column(COL_ServiceInstanceID).Value(pendingInstance.ServiceInstanceId.Value);
                updateQuery.Column(COL_AssignmentStatus).Value((int)pendingInstance.AssignmentStatus);

                var whereContext = updateQuery.Where();
                whereContext.EqualsCondition(COL_ID).Value(pendingInstance.ProcessInstanceID);
            }

            queryContext.ExecuteNonQuery();
        }

        #endregion

        #region Private Methods

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("BusinessProcess", "BusinessProcessDBConnStringKey", "BusinessProcessDBConnString");
        }
        private void SplitLastUpdateHandle(object lastUpdateHandle, out DateTime? statusUpdateTime, out long? id)
        {
            statusUpdateTime = null;
            id = null;

            if (lastUpdateHandle != null)
            {
                string[] data = lastUpdateHandle.ToString().Split('@');
                statusUpdateTime = DateTime.ParseExact(data[0], "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
                id = long.Parse(data[1]);
            }
        }
        private object BuildLastUpdateHandle(DateTime? statusUpdateTime, long? id)
        {
            if (statusUpdateTime.HasValue && id.HasValue)
                return string.Concat(statusUpdateTime.Value.ToString("yyyyMMddHHmmssfff"), "@", id.Value);

            return null;
        }
        private void GetTableNameAndAlias(bool fromArchive, out string tableName, out string tableAlias)
        {
            tableName = fromArchive ? BPINSTANCEARCHIVE_Name : BPINSTANCE_NAME;
            tableAlias = fromArchive ? BPINSTANCEARCHIVE_ALIAS : BPINSTANCE_ALIAS;
        }
        private void BuildGrantedPermissionsSetIdsFilter(List<int> grantedPermissionSetIds, RDBConditionContext whereContext)
        {
            if (grantedPermissionSetIds == null || grantedPermissionSetIds.Count == 0)
                whereContext.NullCondition(COL_ViewRequiredPermissionSetId);
            else
                whereContext.ConditionIfColumnNotNull(COL_ViewRequiredPermissionSetId, RDBConditionGroupOperator.OR).ListCondition(COL_ViewRequiredPermissionSetId, RDBListConditionOperator.IN, grantedPermissionSetIds);

        }
        private void BuildParentIdFilter(int parentId, RDBConditionContext whereContext)
        {
            if (parentId != default(int))
                whereContext.EqualsCondition(COL_ParentID).Value(parentId);
        }
        private void BuildEntityIdsFilter(BPInstanceBeforeIdInput input, RDBConditionContext whereContext)
        {
            if (input.EntityIds != null && input.EntityIds.Count > 0)
                whereContext.ListCondition(COL_EntityId, RDBListConditionOperator.IN, input.EntityIds);
        }
        private void BuildEntityIdsFilter(List<string> entityIds, RDBConditionContext whereContext)
        {
            if (entityIds != null && entityIds.Count > 0)
                whereContext.ListCondition(COL_EntityId, RDBListConditionOperator.IN, entityIds);
        }
        private void BuildDefinitionsIdFilter(List<Guid> definitionsId, RDBConditionContext whereContext)
        {
            if (definitionsId != null && definitionsId.Count > 0)
                whereContext.ListCondition(COL_DefinitionID, RDBListConditionOperator.IN, definitionsId);
        }
        private void BuildTaskIdFilter(Guid? taskId, RDBConditionContext whereContext)
        {
            if (taskId.HasValue)
                whereContext.EqualsCondition(COL_TaskId).Value(taskId.Value);
        }
        private void BuildExecutionStatusFilter(IEnumerable<BPInstanceStatus> statuses, RDBConditionContext whereContext)
        {
            if (statuses != null && statuses.Count() > 0)
                whereContext.ListCondition(COL_ExecutionStatus, RDBListConditionOperator.IN, statuses.Select(itm => (int)itm));
        }
        private void BuildFilters(List<Guid> definitionsId, int parentId, List<string> entityIds, List<int> grantedPermissionSetIds, RDBConditionContext whereContext)
        {
            BuildGrantedPermissionsSetIdsFilter(grantedPermissionSetIds, whereContext);

            BuildDefinitionsIdFilter(definitionsId, whereContext);

            BuildEntityIdsFilter(entityIds, whereContext);

            BuildParentIdFilter(parentId, whereContext);

        }
        private static Dictionary<string, RDBTableColumnDefinition> GetRDBTableColumnDefinitionDictionary()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_Title, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 1000 });
            columns.Add(COL_ParentID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_DefinitionID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_ServiceInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_InitiatorUserId, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_WorkflowInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_InputArgument, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CompletionNotifier, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_ExecutionStatus, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_AssignmentStatus, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_LastMessage, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_EntityId, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
            columns.Add(COL_ViewRequiredPermissionSetId, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CancellationRequestUserId, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_StatusUpdatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_TaskId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            return columns;
        }

        #endregion

        #region Mappers

        private BPDefinitionSummary BPDefinitionSummaryMapper(IRDBDataReader reader)
        {
            return new BPDefinitionSummary()
            {
                RunningProcessNumber = reader.GetInt("RunningProcessNumber"),
                PendingInstanceTime = reader.GetDateTime("PendingInstanceTime"),
                BPDefinitionID = reader.GetGuid(COL_DefinitionID)
            };
        }
        private BPInstance BPInstanceMapper(IRDBDataReader reader)
        {
            BPInstance instance = new BPInstance
            {
                ProcessInstanceID = reader.GetLong(COL_ID),
                Title = reader.GetString(COL_Title),
                ParentProcessID = reader.GetNullableLong(COL_ParentID),
                DefinitionID = reader.GetGuid(COL_DefinitionID),
                WorkflowInstanceID = reader.GetNullableGuid(COL_WorkflowInstanceID),
                Status = (BPInstanceStatus)reader.GetInt(COL_ExecutionStatus),
                AssignmentStatus = (BPInstanceAssignmentStatus)reader.GetInt(COL_AssignmentStatus),
                LastMessage = reader.GetString(COL_LastMessage),
                CreatedTime = reader.GetDateTime(COL_CreatedTime),
                StatusUpdatedTime = reader.GetNullableDateTime(COL_StatusUpdatedTime),
                InitiatorUserId = reader.GetInt(COL_InitiatorUserId),
                EntityId = reader.GetString(COL_EntityId),
                ViewRequiredPermissionSetId = reader.GetNullableInt(COL_ViewRequiredPermissionSetId),
                ServiceInstanceId = reader.GetNullableGuid(COL_ServiceInstanceID),
                TaskId = reader.GetNullableGuid(COL_TaskId),
                CancellationRequestByUserId = reader.GetNullableInt(COL_CancellationRequestUserId)
            };

            string inputArg = reader.GetString(COL_InputArgument);
            if (!String.IsNullOrWhiteSpace(inputArg))
            {
                Type inputArgumentType;
                if (InputArgumentTypeByDefinitionId.TryGetValue(instance.DefinitionID, out inputArgumentType))
                    instance.InputArgument = Serializer.Deserialize(inputArg, inputArgumentType).CastWithValidate<BaseProcessInputArgument>("bpInstance.InputArgument", instance.DefinitionID);
                else
                    instance.InputArgument = Serializer.Deserialize(inputArg).CastWithValidate<BaseProcessInputArgument>("bpInstance.InputArgument", instance.DefinitionID);
            }

            string completionNotifier = reader.GetString(COL_CompletionNotifier);
            if (!string.IsNullOrWhiteSpace(completionNotifier))
                instance.CompletionNotifier = Serializer.Deserialize(completionNotifier) as ProcessCompletionNotifier;

            return instance;
        }

        #endregion
    }
}