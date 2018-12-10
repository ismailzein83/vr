using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace Vanrise.BusinessProcess.Data.SQL
{
    public class BPInstanceDataManager : BaseSQLDataManager, IBPInstanceDataManager
    {
        #region Properties/Ctor

        private static Dictionary<string, string> s_mapper = new Dictionary<string, string>();

        public Dictionary<Guid, Type> InputArgumentTypeByDefinitionId { get; set; }

        static BPInstanceDataManager()
        {
            s_mapper.Add("ProcessInstanceID", "ID");
            s_mapper.Add("StatusDescription", "ExecutionStatus");
        }

        public BPInstanceDataManager()
            : base(GetConnectionStringName("BusinessProcessDBConnStringKey", "BusinessProcessDBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public BPInstance GetBPInstance(long bpInstanceId, bool getFromArchive)
        {
            string query = String.Format("SELECT {0} FROM bp.[BPInstance{1}] bp WITH(NOLOCK) WHERE ID = @ID", BPInstanceSELECTCOLUMNS, (getFromArchive ? "_Archived" : ""));
            return GetItemText(query, BPInstanceMapper, (cmd) => cmd.Parameters.Add(new SqlParameter("@ID", bpInstanceId)));
        }

        public List<BPInstance> GetFilteredBPInstances(BPInstanceQuery query, List<int> grantedPermissionSetIds, bool getFromArchive)
        {
            StringBuilder queryBuilder = new StringBuilder();

            queryBuilder.AppendFormat("SELECT TOP({0}) {1} FROM bp.[BPInstance{2}] bp WITH(NOLOCK)", query.Top, BPInstanceSELECTCOLUMNS, (getFromArchive ? "_Archived" : ""));

            List<string> filters = new List<string>();

            if (!string.IsNullOrEmpty(query.EntityId))
                filters.Add(" bp.EntityID = @EntityId ");

            string definitionIdsFilter = BuildFilterFromGuids("bp.DefinitionID", query.DefinitionsId);
            if (definitionIdsFilter != null)
                filters.Add(definitionIdsFilter);

            string grantedPermissionSetIdsFilter = BuildViewRequiredPermissionsFilter(grantedPermissionSetIds);
            if (grantedPermissionSetIdsFilter != null)
                filters.Add(grantedPermissionSetIdsFilter);

            string statusesFilter = BuildStatusesFilter(query.InstanceStatus);
            if (statusesFilter != null)
                filters.Add(statusesFilter);

            if (query.DateFrom.HasValue)
                filters.Add(" bp.CreatedTime >= @DateFrom ");

            if (query.DateTo.HasValue)
                filters.Add(" bp.CreatedTime <= @DateTo ");

            //if (query.TaskId.HasValue)
            //    filters.Add(string.Format(" [TaskId] = '{0}' ", query.TaskId.Value));

            if (filters.Count > 0)
                queryBuilder.AppendFormat(" WHERE {0} ", String.Join(" AND ", filters));

            queryBuilder.Append(" ORDER BY bp.[ID] DESC");

            queryBuilder.AppendLine();

            return GetItemsText(queryBuilder.ToString(), BPInstanceMapper, (cmd) =>
                {
                    if (!string.IsNullOrEmpty(query.EntityId))
                        cmd.Parameters.Add(new SqlParameter("@EntityId", query.EntityId));
                    if (query.DateFrom.HasValue)
                        cmd.Parameters.Add(new SqlParameter("@DateFrom", query.DateFrom.Value));
                    if (query.DateTo.HasValue)
                        cmd.Parameters.Add(new SqlParameter("@DateTo", query.DateTo.Value));
                });
        }

        public List<BPInstance> GetFirstPage(out object lastUpdateHandle, int nbOfRows, List<Guid> definitionsId, int parentId, List<string> entityIds, List<int> grantedPermissionSetIds, Guid? taskId)
        {
            StringBuilder queryBuilder = new StringBuilder();

            queryBuilder.Append(" SELECT TOP 1 StatusUpdatedTime as MaxGlobalStatusUpdatedTime, ID as GlobalId FROM [BP].[BPInstance] WITH(NOLOCK) order by StatusUpdatedTime, ID desc ");
            queryBuilder.AppendLine();

            queryBuilder.AppendFormat("SELECT TOP({0}) {1} INTO #TEMP FROM bp.[BPInstance] bp WITH(NOLOCK)", nbOfRows, BPInstanceSELECTCOLUMNS);
            List<string> filters = BuildFilters(definitionsId, parentId, entityIds, grantedPermissionSetIds);

            if (taskId.HasValue)
                filters.Add(string.Format(" [TaskId] = '{0}' ", taskId.Value));

            if (filters.Count > 0)
                queryBuilder.AppendFormat(" WHERE {0} ", String.Join(" AND ", filters));

            queryBuilder.Append(" ORDER BY bp.[ID] DESC");

            queryBuilder.AppendLine();

            queryBuilder.Append(" SELECT * FROM #TEMP ");

            DateTime? maxStatusUpdatedTime_local = null;
            long? id_local = null;

            List<BPInstance> bpInstances = new List<BPInstance>();

            ExecuteReaderText(queryBuilder.ToString(),
                (reader) =>
                {
                    if (reader.Read())
                    {
                        maxStatusUpdatedTime_local = GetReaderValue<DateTime>(reader, "MaxGlobalStatusUpdatedTime");
                        id_local = GetReaderValue<long>(reader, "GlobalId");
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
                },
                null);
            lastUpdateHandle = BuildLastUpdateHandle(maxStatusUpdatedTime_local, id_local);
            return bpInstances;
        }

        public List<Entities.BPInstance> GetFirstPageFromArchive(int nbOfRows, List<Guid> definitionsId, int parentId, List<string> entityIds, List<int> grantedPermissionSetIds, Guid? taskId)
        {
            StringBuilder queryBuilder = new StringBuilder();

            queryBuilder.AppendFormat("SELECT TOP({0}) {1} FROM bp.[BPInstance_Archived] bp WITH(NOLOCK)", nbOfRows, BPInstanceSELECTCOLUMNS);
            List<string> filters = BuildFilters(definitionsId, parentId, entityIds, grantedPermissionSetIds);

            if (taskId.HasValue)
                filters.Add(string.Format(" [TaskId] = '{0}' ", taskId.Value));

            if (filters.Count > 0)
                queryBuilder.AppendFormat(" WHERE {0} ", String.Join(" AND ", filters));

            queryBuilder.Append(" ORDER BY bp.[ID] DESC");

            queryBuilder.AppendLine();

            return GetItemsText(queryBuilder.ToString(), BPInstanceMapper, null);
        }

        public List<BPInstance> GetUpdated(ref object lastUpdateHandle, int nbOfRows, List<Guid> definitionsId, int parentId, List<string> entityIds, List<int> grantedPermissionSetIds, Guid? taskId)
        {
            StringBuilder queryBuilder = new StringBuilder();

            queryBuilder.AppendFormat("SELECT TOP({0}) {1} INTO #TEMP FROM bp.[BPInstance] bp WITH(NOLOCK)", nbOfRows, BPInstanceSELECTCOLUMNS);
            List<string> filters = BuildFilters(definitionsId, parentId, entityIds, grantedPermissionSetIds);
            filters.Add(" (bp.[StatusUpdatedTime] > @StatusUpdatedTime OR (bp.[StatusUpdatedTime] = @StatusUpdatedTime AND bp.ID > @BPInstanceId))");

            if (taskId.HasValue)
                filters.Add(string.Format(" [TaskId] = '{0}' ", taskId.Value));

            queryBuilder.AppendFormat(" WHERE {0} ", String.Join(" AND ", filters));

            queryBuilder.Append(" ORDER BY bp.[StatusUpdatedTime], bp.ID ");

            queryBuilder.AppendLine();

            queryBuilder.Append(" SELECT * FROM #TEMP ");

            object lastUpdateHandle_local = lastUpdateHandle;

            DateTime? maxStatusUpdatedTime_local = null;
            long? id_local = null;

            List<BPInstance> bpInstances = new List<BPInstance>();
            ExecuteReaderText(queryBuilder.ToString(),
                (reader) =>
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
                },
                (cmd) =>
                {
                    if (lastUpdateHandle_local != null)
                    {
                        DateTime? statusUpdateTime;
                        long? id;
                        SplitLastUpdateHandle(lastUpdateHandle_local, out statusUpdateTime, out id);

                        cmd.Parameters.Add(new SqlParameter("@StatusUpdatedTime", statusUpdateTime.Value));
                        cmd.Parameters.Add(new SqlParameter("@BPInstanceId", id.Value));
                    }
                });
            object handle = BuildLastUpdateHandle(maxStatusUpdatedTime_local, id_local);

            lastUpdateHandle = handle != null ? handle : lastUpdateHandle_local;
            return bpInstances;
        }

        public List<BPInstance> GetBeforeId(BPInstanceBeforeIdInput input, List<int> grantedPermissionSetIds, bool getFromArchive)
        {
            StringBuilder queryBuilder = new StringBuilder();

            queryBuilder.AppendFormat("SELECT TOP({0}) {1} FROM bp.[BPInstance{2}] bp WITH(NOLOCK)", input.NbOfRows, BPInstanceSELECTCOLUMNS, (getFromArchive ? "_Archived" : ""));
            List<string> filters = new List<string>();
            filters.Add(String.Concat("bp.ID < ", input.LessThanID.ToString()));

            string definitionIdsFilter = BuildFilterFromGuids("bp.DefinitionID", input.DefinitionsId);
            if (definitionIdsFilter != null)
                filters.Add(definitionIdsFilter);

            string grantedPermissionSetIdsFilter = BuildViewRequiredPermissionsFilter(grantedPermissionSetIds);
            if (grantedPermissionSetIdsFilter != null)
                filters.Add(grantedPermissionSetIdsFilter);

            string entityIdsFilter = BuildFilterFromStrings("bp.EntityId", input.EntityIds);
            if (entityIdsFilter != null)
                filters.Add(entityIdsFilter);

            if (input.ParentId != default(int))
                filters.Add(String.Concat(" bp.ParentID = ", input.ParentId.ToString()));

            if (input.TaskId.HasValue)
                filters.Add(string.Format(" [TaskId] = '{0}' ", input.TaskId.Value));

            if (filters.Count > 0)
                queryBuilder.AppendFormat(" WHERE {0} ", String.Join(" AND ", filters));

            queryBuilder.Append(" ORDER BY bp.[ID] DESC");

            queryBuilder.AppendLine();

            return GetItemsText(queryBuilder.ToString(), BPInstanceMapper, null);
        }

        public List<BPInstance> GetAfterId(long? processInstanceId, Guid bpDefinitionId, bool getFromArchive)
        {
            StringBuilder queryBuilder = new StringBuilder();

            queryBuilder.AppendFormat("SELECT {0} FROM bp.[BPInstance{1}] bp WITH(NOLOCK) WHERE bp.DefinitionID = @DefinitionId ", BPInstanceSELECTCOLUMNS, (getFromArchive ? "_Archived" : ""));
            if (processInstanceId.HasValue)
                queryBuilder.AppendFormat(" AND bp.[ID] > {0}", processInstanceId.Value);
            return GetItemsText(queryBuilder.ToString(), BPInstanceMapper, (cmd) => cmd.Parameters.Add(new SqlParameter("@DefinitionId", bpDefinitionId)));
        }

        public List<BPInstance> GetPendingInstances(Guid definitionId, IEnumerable<BPInstanceStatus> acceptableBPStatuses, BPInstanceAssignmentStatus assignmentStatus, int maxCounts, Guid serviceInstanceId)
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append("SELECT TOP(");
            queryBuilder.Append(maxCounts);
            queryBuilder.Append(") ");
            queryBuilder.Append(BPInstanceSELECTCOLUMNS);
            queryBuilder.Append(" FROM bp.[BPInstance] bp WITH(NOLOCK) WHERE [DefinitionID] = '");
            queryBuilder.Append(definitionId);
            queryBuilder.Append("' AND ServiceInstanceID = '");
            queryBuilder.Append(serviceInstanceId);
            queryBuilder.Append("' AND ISNULL(AssignmentStatus, 0) = ");
            queryBuilder.Append((int)assignmentStatus);
            queryBuilder.Append(" AND ");
            queryBuilder.Append(BuildStatusesFilter(acceptableBPStatuses));
            queryBuilder.Append(" ORDER BY bp.[ID]");
            return GetItemsText(queryBuilder.ToString(), BPInstanceMapper, null);
        }

        public List<BPInstance> GetPendingInstancesInfo(IEnumerable<BPInstanceStatus> statuses)
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append("SELECT ");
            queryBuilder.Append(BPInstanceSELECTCOLUMNS);
            queryBuilder.Append(" FROM bp.[BPInstance] bp WHERE ");
            queryBuilder.Append(BuildStatusesFilter(statuses));
            queryBuilder.Append(" ORDER BY bp.[ID]");
            return GetItemsText(queryBuilder.ToString(), BPInstanceMapper, null);
        }

        public void UpdateInstanceStatus(long processInstanceId, BPInstanceStatus status, BPInstanceAssignmentStatus assignmentStatus, string message, bool clearServiceInstanceId, Guid? workflowInstanceId)
        {
            ExecuteNonQuerySP("bp.sp_BPInstance_UpdateStatus", processInstanceId, (int)status, (int)assignmentStatus, message, clearServiceInstanceId, workflowInstanceId);
        }

        public void UpdateInstanceLastMessage(long processInstanceId, string message)
        {
            ExecuteNonQuerySP("[bp].[sp_BPInstance_UpdateLastMessage]", processInstanceId, message);
        }

        public bool HasRunningInstances(Guid definitionId, List<string> entityIds, IEnumerable<BPInstanceStatus> acceptableBPStatuses)
        {
            StringBuilder queryBuilder = new StringBuilder();
            List<string> filters = new List<string>();

            filters.Add(String.Concat("bp.DefinitionID = '", definitionId, "'"));

            string entityIdsFilter = BuildFilterFromStrings("bp.EntityId", entityIds);
            if (entityIdsFilter != null)
                filters.Add(entityIdsFilter);

            string statusesFilter = BuildStatusesFilter(acceptableBPStatuses);
            if (statusesFilter != null)
                filters.Add(statusesFilter);

            queryBuilder.AppendFormat(@"IF Exists ( SELECT top 1 null 	FROM bp.[BPInstance] bp WITH(NOLOCK)
	                                    WHERE	{0})
	                                    SELECT 1
	                                    ELSE SELECT 0", String.Join(" AND ", filters));

            bool hasRunningProcesses = Convert.ToBoolean(ExecuteScalarText(queryBuilder.ToString(), null));
            return hasRunningProcesses;
        }

        public long InsertInstance(BPInstanceToAdd bpInstanceToAdd)
        {
            bool serializeInputArgumentsWithoutType = false;
            if (InputArgumentTypeByDefinitionId.ContainsKey(bpInstanceToAdd.DefinitionID))
                serializeInputArgumentsWithoutType = true;

            string serializedInputArguments = null;
            if (bpInstanceToAdd.InputArgument != null)
                serializedInputArguments = Serializer.Serialize(bpInstanceToAdd.InputArgument, serializeInputArgumentsWithoutType);

            string serializedCompletionNotifier = null;
            if (bpInstanceToAdd.CompletionNotifier != null)
                serializedCompletionNotifier = Serializer.Serialize(bpInstanceToAdd.CompletionNotifier);

            object processInstanceId;
            if (ExecuteNonQuerySP("bp.sp_BPInstance_Insert", out processInstanceId, bpInstanceToAdd.Title, bpInstanceToAdd.ParentProcessID, bpInstanceToAdd.DefinitionID, serializedInputArguments,
                serializedCompletionNotifier, (int)bpInstanceToAdd.Status, bpInstanceToAdd.InitiatorUserId, bpInstanceToAdd.EntityId, bpInstanceToAdd.ViewRequiredPermissionSetId, bpInstanceToAdd.TaskId) > 0)
                return (long)processInstanceId;
            else
                return 0;
        }

        public void UpdateServiceInstancesAndAssignmentStatus(List<BPInstance> pendingInstancesToUpdate)
        {
            foreach (var pendingInstance in pendingInstancesToUpdate)
            {
                if (!pendingInstance.ServiceInstanceId.HasValue)
                    throw new NullReferenceException(String.Format("pendingInstance.ServiceInstanceId. ProcessInstanceId '{0}'", pendingInstance.ProcessInstanceID));
                ExecuteNonQuerySP("[bp].[sp_BPInstance_UpdateServiceInstanceIDAndAssignmentStatus]", pendingInstance.ProcessInstanceID, pendingInstance.ServiceInstanceId.Value, (int)pendingInstance.AssignmentStatus);
            }
        }

        public List<BPDefinitionSummary> GetBPDefinitionSummary(IEnumerable<BPInstanceStatus> executionStatus)
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append("SELECT bp.[DefinitionID],count(*) as RunningProcessNumber , max(bp.CreatedTime) as PendingInstanceTime FROM [BP].[BPInstance] bp WITH(NOLOCK) WHERE 1=1 AND ");
            queryBuilder.Append(BuildStatusesFilter(executionStatus));
            queryBuilder.Append(" GROUP BY [DefinitionID] ");
            return GetItemsText(queryBuilder.ToString(), BPDefinitionSummaryMapper, null);

        }

        public void UpdateInstanceAssignmentStatus(long processInstanceId, BPInstanceAssignmentStatus assignmentStatus)
        {
            ExecuteNonQuerySP("[bp].[sp_BPInstance_UpdateAssignmentStatus]", processInstanceId, assignmentStatus);
        }

        public void SetCancellationRequestUserId(long bpInstanceId, List<BPInstanceStatus> allowedStatuses, int cancelRequestByUserId)
        {
            string allowedStatusesString = String.Join(",", allowedStatuses.Select(status => (int)status));
            ExecuteNonQuerySP("bp.sp_BPInstance_SetCancellationRequestUserId", bpInstanceId, allowedStatusesString, cancelRequestByUserId);
        }

        public void ArchiveInstances(List<BPInstanceStatus> completedStatuses, DateTime completedBefore, int nbOfInstances)
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(@"SELECT top (@Top) *
into #InstancesToArchive
 from [bp].[BPInstance] bp WITH (NOLOCK) WHERE bp.[StatusUpdatedTime] < @CompletedBefore ");
            queryBuilder.AppendFormat(" AND {0} ", BuildStatusesFilter(completedStatuses));
            queryBuilder.Append(" ORDER BY bp.ID ");
            queryBuilder.AppendLine();

            queryBuilder.Append(@"IF EXISTS (SELECT TOP 1 ID FROM #InstancesToArchive)
                                    BEGIN
                                    INSERT INTO [bp].[BPInstance_Archived]
([ID]
      ,[Title]
      ,[ParentID]
      ,[DefinitionID]
      ,[ServiceInstanceID]
      ,[InitiatorUserId]
      ,[WorkflowInstanceID]
      ,[InputArgument]
      ,[CompletionNotifier]
      ,[ExecutionStatus]
      ,[AssignmentStatus]
      ,[LastMessage]
      ,[EntityId]
      ,[ViewRequiredPermissionSetId]
      ,[CancellationRequestUserId]
      ,[CreatedTime]
      ,[StatusUpdatedTime]
      ,[TaskId])
SELECT instancesToArchive.[ID]
      ,instancesToArchive.[Title]
      ,instancesToArchive.[ParentID]
      ,instancesToArchive.[DefinitionID]
      ,instancesToArchive.[ServiceInstanceID]
      ,instancesToArchive.[InitiatorUserId]
      ,instancesToArchive.[WorkflowInstanceID]
      ,instancesToArchive.[InputArgument]
      ,instancesToArchive.[CompletionNotifier]
      ,instancesToArchive.[ExecutionStatus]
      ,instancesToArchive.[AssignmentStatus]
      ,instancesToArchive.[LastMessage]
      ,instancesToArchive.[EntityId]
      ,instancesToArchive.[ViewRequiredPermissionSetId]
      ,instancesToArchive.[CancellationRequestUserId]
      ,instancesToArchive.[CreatedTime]
      ,instancesToArchive.[StatusUpdatedTime]
      ,instancesToArchive.[TaskId]
FROM #InstancesToArchive instancesToArchive
LEFT JOIN [bp].[BPInstance_Archived] a ON instancesToArchive.ID = a.ID
WHERE a.ID IS NULL 

DELETE bp FROM [bp].[BPInstance] bp
JOIN #InstancesToArchive instancesToArchive ON bp.ID = instancesToArchive.ID
 END 
");
            ExecuteNonQueryText(queryBuilder.ToString(), (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@Top", nbOfInstances));
                    cmd.Parameters.Add(new SqlParameter("@CompletedBefore", completedBefore));
                });
        }

        #endregion

        #region Private Methods
        private object BuildLastUpdateHandle(DateTime? statusUpdateTime, long? id)
        {
            if (statusUpdateTime.HasValue && id.HasValue)
                return string.Concat(statusUpdateTime.Value.ToString("yyyyMMddHHmmssfff"), "@", id.Value);

            return null;
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

        private string BuildStatusesFilter(IEnumerable<BPInstanceStatus> statuses)
        {
            if (statuses == null || statuses.Count() == 0)
                return null;
            else
                return String.Concat(" bp.ExecutionStatus in (", String.Join(",", statuses.Select(itm => (int)itm)), ") ");
        }

        private string BuildFilterFromGuids(string columnName, IEnumerable<Guid> guids)
        {
            if (guids == null || guids.Count() == 0)
                return null;
            else
                return string.Concat(columnName, " IN (", String.Join(",", guids.Select(itm => String.Concat("'", itm.ToString(), "'"))), ")");
        }

        private string BuildFilterFromStrings(string columnName, IEnumerable<string> items)
        {
            if (items == null || items.Count() == 0)
                return null;
            else
                return string.Concat(columnName, " IN (", String.Join(",", items.Select(itm => String.Concat("'", itm.ToString(), "'"))), ")");
        }

        private string BuildViewRequiredPermissionsFilter(List<int> grantedPermissionSetIds)
        {
            if (grantedPermissionSetIds == null || grantedPermissionSetIds.Count == 0)
                return "bp.ViewRequiredPermissionSetId IS NULL";
            else
                return string.Concat("(bp.ViewRequiredPermissionSetId IS NULL OR bp.ViewRequiredPermissionSetId IN (", String.Join(",", grantedPermissionSetIds), "))");
        }

        List<string> BuildFilters(List<Guid> definitionsId, int parentId, List<string> entityIds, List<int> grantedPermissionSetIds)
        {
            List<string> filters = new List<string>();

            string definitionIdsFilter = BuildFilterFromGuids("bp.DefinitionID", definitionsId);
            if (definitionIdsFilter != null)
                filters.Add(definitionIdsFilter);

            string grantedPermissionSetIdsFilter = BuildViewRequiredPermissionsFilter(grantedPermissionSetIds);
            if (grantedPermissionSetIdsFilter != null)
                filters.Add(grantedPermissionSetIdsFilter);

            string entityIdsFilter = BuildFilterFromStrings("bp.EntityId", entityIds);
            if (entityIdsFilter != null)
                filters.Add(entityIdsFilter);

            if (parentId != default(int))
                filters.Add(String.Concat(" bp.ParentID = ", parentId.ToString()));

            return filters;
        }

        #endregion

        #region Mappers

        const string BPInstanceSELECTCOLUMNS = @" bp.[ID]
	                                                ,[Title]
                                                    ,[ParentID]
                                                    ,[DefinitionID]
                                                    ,[WorkflowInstanceID]
                                                    ,[InputArgument]
	                                                , [CompletionNotifier]
                                                    ,[ExecutionStatus]
                                                    ,[AssignmentStatus]
                                                    ,[LastMessage]
	                                                ,EntityID
                                                    ,[ViewRequiredPermissionSetId]
                                                    ,[CreatedTime]
                                                    ,[StatusUpdatedTime]      
                                                    ,[InitiatorUserId]
	                                                ,[ServiceInstanceID]
                                                    ,[TaskId]
                                                    ,[CancellationRequestUserId]";

        private BPInstance BPInstanceMapper(IDataReader reader)
        {
            BPInstance instance = new BPInstance
            {
                ProcessInstanceID = (long)reader["ID"],
                Title = reader["Title"] as string,
                ParentProcessID = GetReaderValue<long?>(reader, "ParentID"),
                DefinitionID = GetReaderValue<Guid>(reader, "DefinitionID"),
                WorkflowInstanceID = GetReaderValue<Guid?>(reader, "WorkflowInstanceID"),
                Status = (BPInstanceStatus)reader["ExecutionStatus"],
                AssignmentStatus = (BPInstanceAssignmentStatus)GetReaderValue<int>(reader, "AssignmentStatus"),
                LastMessage = reader["LastMessage"] as string,
                CreatedTime = (DateTime)reader["CreatedTime"],
                StatusUpdatedTime = GetReaderValue<DateTime?>(reader, "StatusUpdatedTime"),
                InitiatorUserId = GetReaderValue<int>(reader, "InitiatorUserId"),
                EntityId = reader["EntityId"] as string,
                ViewRequiredPermissionSetId = GetReaderValue<int?>(reader, "ViewRequiredPermissionSetId"),
                ServiceInstanceId = GetReaderValue<Guid?>(reader, "ServiceInstanceID"),
                TaskId = GetReaderValue<Guid?>(reader, "TaskId"),
                CancellationRequestByUserId = GetReaderValue<int?>(reader, "CancellationRequestUserId")
            };

            string inputArg = reader["InputArgument"] as string;
            if (!String.IsNullOrWhiteSpace(inputArg))
            {
                Type inputArgumentType;
                if (InputArgumentTypeByDefinitionId.TryGetValue(instance.DefinitionID, out inputArgumentType))
                    instance.InputArgument = Serializer.Deserialize(inputArg, inputArgumentType).CastWithValidate<BaseProcessInputArgument>("bpInstance.InputArgument", instance.DefinitionID);
                else
                    instance.InputArgument = Serializer.Deserialize(inputArg).CastWithValidate<BaseProcessInputArgument>("bpInstance.InputArgument", instance.DefinitionID);
            }

            string completionNotifier = reader["CompletionNotifier"] as string;
            if (!string.IsNullOrWhiteSpace(completionNotifier))
                instance.CompletionNotifier = Serializer.Deserialize(completionNotifier) as ProcessCompletionNotifier;

            return instance;
        }

        private BPDefinitionSummary BPDefinitionSummaryMapper(IDataReader reader)
        {
            return new BPDefinitionSummary()
            {
                RunningProcessNumber = GetReaderValue<int>(reader, "RunningProcessNumber"),
                PendingInstanceTime = GetReaderValue<DateTime>(reader, "PendingInstanceTime"),
                BPDefinitionID = GetReaderValue<Guid>(reader, "DefinitionID")
            };
        }


        #endregion
    }
}
