using System;
using System.Collections.Generic;
using System.Data;
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
                       grantedPermissionSetIdsAsString
                  );
        }

        public List<BPInstance> GetUpdated(ref byte[] maxTimeStamp, int nbOfRows, List<Guid> definitionsId, int parentId, List<string> entityIds, List<int> grantedPermissionSetIds)
        {
            string definitionsIdAsString = null;
            if (definitionsId != null && definitionsId.Count() > 0)
                definitionsIdAsString = string.Join<Guid>(",", definitionsId);
            string grantedPermissionSetIdsAsString = null;
            if (grantedPermissionSetIds != null)
                grantedPermissionSetIdsAsString = string.Join<int>(",", grantedPermissionSetIds);
            string entityIdsString = null;
            if (entityIds != null)
                entityIdsString = string.Join<string>(",", entityIds);
            List<BPInstance> bpInstances = new List<BPInstance>();
            byte[] timestamp = null;

            ExecuteReaderSP("[bp].[sp_BPInstance_GetUpdated]", (reader) =>
            {
                while (reader.Read())
                    bpInstances.Add(BPInstanceMapper(reader));
                if (reader.NextResult())
                    while (reader.Read())
                        timestamp = GetReaderValue<byte[]>(reader, "MaxTimestamp");
            },
               maxTimeStamp, nbOfRows, definitionsIdAsString, ToDBNullIfDefault(parentId), entityIdsString, grantedPermissionSetIdsAsString);
            maxTimeStamp = timestamp;
            return bpInstances;
        }

        public List<BPInstance> GetBeforeId(BPInstanceBeforeIdInput input, List<int> grantedPermissionSetIds)
        {
            string definitionsIdAsString = null;
            if (input.DefinitionsId != null && input.DefinitionsId.Count() > 0)
                definitionsIdAsString = string.Join<Guid>(",", input.DefinitionsId);

            string grantedPermissionSetIdsAsString = null;
            if (grantedPermissionSetIds != null)
                grantedPermissionSetIdsAsString = string.Join<int>(",", grantedPermissionSetIds);

            string entityIdsString = null;
            if (input.EntityIds != null)
                entityIdsString = string.Join<string>(",", input.EntityIds);

            return GetItemsSP("[BP].[sp_BPInstance_GetBeforeID]", BPInstanceMapper, input.LessThanID, input.NbOfRows, definitionsIdAsString, ToDBNullIfDefault(input.ParentId), entityIdsString, grantedPermissionSetIdsAsString);
        }

        public List<BPInstance> GetAfterId(long? processInstanceId, Guid bpDefinitionId)
        {
            return GetItemsSP("[BP].[sp_BPInstance_GetAfterID]", BPInstanceMapper, processInstanceId, bpDefinitionId);
        }

        public List<BPInstance> GetPendingInstances(Guid definitionId, IEnumerable<BPInstanceStatus> acceptableBPStatuses, int maxCounts, Guid serviceInstanceId)
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
            queryBuilder.Append("' AND ");
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

        public List<long> GetInstanceIdsHavingChildren(IEnumerable<BPInstanceStatus> statuses)
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append("SELECT bp.[ParentID] FROM bp.[BPInstance] bp WITH(NOLOCK) WHERE bp.[ParentID] IS NOT NULL AND ");
            queryBuilder.Append(BuildStatusesFilter(statuses));
            queryBuilder.Append(" GROUP by bp.[ParentID] ");
            return GetItemsText(queryBuilder.ToString(), (reader) => (long)reader["ParentID"], null);
        }

        public void UpdateInstanceStatus(long processInstanceId, BPInstanceStatus status, string message, Guid? workflowInstanceId)
        {
            ExecuteNonQuerySP("bp.sp_BPInstance_UpdateStatus", processInstanceId, (int)status, message, workflowInstanceId);
        }

        public void UpdateInstanceLastMessage(long processInstanceId, string message)
        {
            ExecuteNonQuerySP("[bp].[sp_BPInstance_UpdateLastMessage]", processInstanceId, message);
        }

        public bool HasRunningInstances(Guid definitionId, List<string> entityIds, IEnumerable<BPInstanceStatus> acceptableBPStatuses)
        {
            bool hasRunningProcesses = Convert.ToBoolean(ExecuteScalarSP("[bp].[sp_BPInstance_HasRunningInstance]", definitionId, String.Join(",", entityIds.Select(itm => (string)itm)), String.Join(",", acceptableBPStatuses.Select(itm => (int)itm))));
            return hasRunningProcesses;
        }

        public long InsertInstance(string processTitle, long? parentId, ProcessCompletionNotifier completionNotifier, Guid definitionId, object inputArguments, BPInstanceStatus executionStatus,
            int initiatorUserId, string entityId, int? viewInstanceRequiredPermissionSetId)
        {
            object processInstanceId;
            if (ExecuteNonQuerySP("bp.sp_BPInstance_Insert", out processInstanceId, processTitle, parentId, definitionId,
                inputArguments != null ? Serializer.Serialize(inputArguments) : null,
                completionNotifier != null ? Serializer.Serialize(completionNotifier) : null,
                (int)executionStatus, initiatorUserId, entityId, viewInstanceRequiredPermissionSetId) > 0)
                return (long)processInstanceId;
            else
                return 0;
        }

        public void SetServiceInstancesOfBPInstances(List<BPInstance> pendingInstancesToUpdate)
        {
            foreach (var pendingInstance in pendingInstancesToUpdate)
            {
                if (!pendingInstance.ServiceInstanceId.HasValue)
                    throw new NullReferenceException(String.Format("pendingInstance.ServiceInstanceId. ProcessInstanceId '{0}'", pendingInstance.ProcessInstanceID));
                ExecuteNonQuerySP("[bp].[sp_BPInstance_UpdateServiceInstanceID]", pendingInstance.ProcessInstanceID, pendingInstance.ServiceInstanceId.Value);
            }
        }


        public List<BPDefinitionSummary> GetBPDefinitionSummary(IEnumerable<BPInstanceStatus> executionStatus)
        {
           
            string excutionStatusIdsAsString = null;
            if (executionStatus != null && executionStatus.Count() > 0)
                excutionStatusIdsAsString = string.Join<int>(",", executionStatus.Select(itm=>(int)itm));

            return GetItemsSP("[bp].[sp_BPDefinitionSummary_GetUpdated]", BPDefinitionSummaryMapper,  excutionStatusIdsAsString);

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
      ,[LastMessage]
	   ,EntityID
      ,[ViewRequiredPermissionSetId]
      ,[CreatedTime]
      ,[StatusUpdatedTime]      
      ,[InitiatorUserId]
	  ,[ServiceInstanceID] ";

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
                LastMessage = reader["LastMessage"] as string,
                CreatedTime = (DateTime)reader["CreatedTime"],
                StatusUpdatedTime = GetReaderValue<DateTime?>(reader, "StatusUpdatedTime"),
                InitiatorUserId = GetReaderValue<int>(reader, "InitiatorUserId"),
                EntityId = reader["EntityId"] as string,
                ViewRequiredPermissionSetId = GetReaderValue<int?>(reader, "ViewRequiredPermissionSetId"),
                ServiceInstanceId = GetReaderValue<Guid?>(reader, "ServiceInstanceID")
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
            return new BPDefinitionSummary() {
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

        #endregion
    }
}
