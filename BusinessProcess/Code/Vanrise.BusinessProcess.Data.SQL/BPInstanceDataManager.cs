﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

        private static Dictionary<string, string> _mapper = new Dictionary<string, string>();

        static BPInstanceDataManager()
        {
            _mapper.Add("ProcessInstanceID", "ID");
            _mapper.Add("StatusDescription", "ExecutionStatus");
        }

        public BPInstanceDataManager()
            : base(GetConnectionStringName("BusinessProcessDBConnStringKey", "BusinessProcessDBConnString"))
        {

        }

        #endregion

        #region public methods

        public BPInstance GetBPInstance(long bpInstanceId)
        {
            return GetItemSP("[bp].[sp_BPInstance_GetByID]", BPInstanceMapper, bpInstanceId);
        }

        public List<BPInstance> GetAllBPInstances(BPInstanceQuery query, List<int> grantedPermissionSetIds)
        {
            string grantedPermissionSetIdsAsString = null;
            if (grantedPermissionSetIds != null)
                grantedPermissionSetIdsAsString = string.Join<int>(",", grantedPermissionSetIds);

            return GetItemsSP("[bp].[sp_BPInstance_GetFiltered]", BPInstanceMapper,
                       query.DefinitionsId == null ? null : string.Join(",", query.DefinitionsId.Select(n => n.ToString()).ToArray()),
                       query.InstanceStatus == null ? null : string.Join(",", query.InstanceStatus.Select(n => ((int)n).ToString()).ToArray()),
                       query.EntityId,
                       query.DateFrom,
                       query.DateTo,
                       grantedPermissionSetIdsAsString,
                       query.Top
                  );
        }

        public List<BPInstance> GetUpdated(ref byte[] maxTimeStamp, int nbOfRows, List<Guid> definitionsId, int parentId, List<string> entityIds, List<int> grantedPermissionSetIds)
        {
            StringBuilder queryBuilder = new StringBuilder();

            if (maxTimeStamp == null)//first time
            {
                queryBuilder.Append(" SELECT MAX(timestamp) MaxGlobalTimeStamp FROM [BP].[BPInstance] WITH(NOLOCK) ");
                queryBuilder.AppendLine();
            }

            queryBuilder.AppendFormat("SELECT TOP({0}) {1} INTO #TEMP FROM bp.[BPInstance] bp WITH(NOLOCK)", nbOfRows, BPInstanceSELECTCOLUMNS);
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

            if (maxTimeStamp != null)
                filters.Add(" bp.[timestamp] > @TimestampAfter ");

            if (filters.Count > 0)
            {
                queryBuilder.AppendFormat(" WHERE {0} ", String.Join(" AND ", filters));
            }

            if (maxTimeStamp == null)//first time
                queryBuilder.Append(" ORDER BY bp.[ID] DESC");
            else
                queryBuilder.Append(" ORDER BY bp.[timestamp] ");

            queryBuilder.AppendLine();

            queryBuilder.Append(" SELECT * FROM #TEMP ");

            queryBuilder.AppendLine();

            queryBuilder.Append(@" SELECT MAX([timestamp]) MaxTimestamp FROM #TEMP ");

            byte[] maxTimeStamp_local = maxTimeStamp;
            List<BPInstance> bpInstances = new List<BPInstance>();
            ExecuteReaderText(queryBuilder.ToString(),
                (reader) =>
                {
                    if (maxTimeStamp_local == null)
                    {
                        if (reader.Read())
                            maxTimeStamp_local = GetReaderValue<byte[]>(reader, "MaxGlobalTimeStamp");
                        reader.NextResult();
                    }
                    while (reader.Read())
                    {
                        bpInstances.Add(BPInstanceMapper(reader));
                    }
                    reader.NextResult();
                    if (reader.Read() && bpInstances.Count > 0)
                    {
                        maxTimeStamp_local = GetReaderValue<byte[]>(reader, "MaxTimestamp");
                    }
                },
                (cmd) =>
                {
                    if (maxTimeStamp_local != null)
                        cmd.Parameters.Add(new SqlParameter("@TimestampAfter", maxTimeStamp_local));
                });
            maxTimeStamp = maxTimeStamp_local;
            return bpInstances;
        }

        public List<BPInstance> GetBeforeId(BPInstanceBeforeIdInput input, List<int> grantedPermissionSetIds)
        {
            StringBuilder queryBuilder = new StringBuilder();

            queryBuilder.AppendFormat("SELECT TOP({0}) {1} FROM bp.[BPInstance] bp WITH(NOLOCK)", input.NbOfRows, BPInstanceSELECTCOLUMNS);
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

            if (filters.Count > 0)
            {
                queryBuilder.AppendFormat(" WHERE {0} ", String.Join(" AND ", filters));
            }

            queryBuilder.Append(" ORDER BY bp.[ID] DESC");

            queryBuilder.AppendLine();

            return GetItemsText(queryBuilder.ToString(),
                BPInstanceMapper,
                null);
        }

        public List<BPInstance> GetAfterId(long? processInstanceId, Guid bpDefinitionId)
        {
            return GetItemsSP("[BP].[sp_BPInstance_GetAfterID]", BPInstanceMapper, processInstanceId, bpDefinitionId);
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

        public long InsertInstance(string processTitle, long? parentId, ProcessCompletionNotifier completionNotifier, Guid definitionId, object inputArguments, BPInstanceStatus executionStatus,
            int initiatorUserId, string entityId, int? viewInstanceRequiredPermissionSetId, Guid? taskId)
        {
            object processInstanceId;
            if (ExecuteNonQuerySP("bp.sp_BPInstance_Insert", out processInstanceId, processTitle, parentId, definitionId,
                inputArguments != null ? Serializer.Serialize(inputArguments) : null,
                completionNotifier != null ? Serializer.Serialize(completionNotifier) : null,
                (int)executionStatus, initiatorUserId, entityId, viewInstanceRequiredPermissionSetId, taskId) > 0)
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

            string excutionStatusIdsAsString = null;
            if (executionStatus != null && executionStatus.Count() > 0)
                excutionStatusIdsAsString = string.Join<int>(",", executionStatus.Select(itm => (int)itm));

            return GetItemsSP("[bp].[sp_BPDefinitionSummary_GetUpdated]", BPDefinitionSummaryMapper, excutionStatusIdsAsString);

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

        #endregion

        #region mapper

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
      ,[CancellationRequestUserId]
	  ,[timestamp] ";

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
                instance.InputArgument = Serializer.Deserialize(inputArg) as BaseProcessInputArgument;

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
                LastProcessCreatedTime = GetReaderValue<DateTime>(reader, "LastProcessCreatedTime"),
                BPDefinitionID = GetReaderValue<Guid>(reader, "DefinitionID")
            };
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

        #endregion
    }
}
