using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace Vanrise.BusinessProcess.Data.SQL
{
    public class BPInstanceDataManager : BaseSQLDataManager, IBPInstanceDataManager
    {
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

        #region public methods
        public List<BPInstance> GetUpdated(ref byte[] maxTimeStamp, int nbOfRows, List<int> definitionsId, int parentId, string entityId)
        {
            string definitionsIdAsString = null;
            if (definitionsId != null && definitionsId.Count() > 0)
                definitionsIdAsString = string.Join<int>(",", definitionsId);

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
               maxTimeStamp, nbOfRows, definitionsIdAsString, ToDBNullIfDefault(parentId), entityId);
            maxTimeStamp = timestamp;
            return bpInstances;
        }

        public List<BPInstance> GetBeforeId(BPInstanceBeforeIdInput input)
        {
            string definitionsIdAsString = null;
            if (input.DefinitionsId != null && input.DefinitionsId.Count() > 0)
                definitionsIdAsString = string.Join<int>(",", input.DefinitionsId);

            return GetItemsSP("[BP].[sp_BPInstance_GetBeforeID]", BPInstanceMapper, input.LessThanID, input.NbOfRows, definitionsIdAsString, ToDBNullIfDefault(input.ParentId),input.EntityId);
        }

        public Vanrise.Entities.BigResult<BPInstanceDetail> GetFilteredBPInstances(Vanrise.Entities.DataRetrievalInput<BPInstanceQuery> input)
        {
            if (input.SortByColumnName != null)
                input.SortByColumnName = input.SortByColumnName.Replace("Entity.", "");

            return RetrieveData(input, (tempTableName) =>
            {
                ExecuteNonQuerySP("bp.sp_BPInstance_CreateTempForFiltered", tempTableName, input.Query.DefinitionsId == null ? null : string.Join(",", input.Query.DefinitionsId.Select(n => n.ToString()).ToArray()),
                    input.Query.InstanceStatus == null ? null : string.Join(",", input.Query.InstanceStatus.Select(n => ((int)n).ToString()).ToArray()),input.Query.EntityId, input.Query.DateFrom, input.Query.DateTo);

            }, BPInstanceDetailMapper, _mapper);
        }

        public List<BPInstance> GetPendingInstances(int definitionId, IEnumerable<BPInstanceStatus> acceptableBPStatuses, int maxCounts, Guid serviceInstanceId)
        {
            return GetItemsSP("[bp].[sp_BPInstance_GetPendingsByDefinitionId]", BPInstanceMapper, definitionId, String.Join(",", acceptableBPStatuses.Select(itm => (int)itm)), maxCounts, serviceInstanceId);
        }

        public List<BPPendingInstanceInfo> GetPendingInstancesInfo(IEnumerable<BPInstanceStatus> statuses, int nbOfInstancesToRetrieve)
        {
            return GetItemsSP("[bp].[sp_BPInstance_GetPendingsInfo]", BPPendingInstanceInfoMapper, String.Join(",", statuses.Select(itm => (int)itm)), nbOfInstancesToRetrieve);
        }

        public void UpdateInstanceStatus(long processInstanceId, BPInstanceStatus status, string message, Guid? workflowInstanceId)
        {
            ExecuteNonQuerySP("bp.sp_BPInstance_UpdateStatus", processInstanceId, (int)status, message, workflowInstanceId);
        }

        public BPInstance GetBPInstance(long bpInstanceId)
        {
            return GetItemSP("[bp].[sp_BPInstance_GetByID]", BPInstanceMapper, bpInstanceId);
        }

        public long InsertInstance(string processTitle, long? parentId, int definitionId, object inputArguments, BPInstanceStatus executionStatus, int initiatorUserId,string entityId)
        {
            object processInstanceId;
            if (ExecuteNonQuerySP("bp.sp_BPInstance_Insert", out processInstanceId, processTitle, parentId, definitionId, inputArguments != null ? Serializer.Serialize(inputArguments) : null, (int)executionStatus, initiatorUserId, entityId) > 0)
                return (long)processInstanceId;
            else
                return 0;
        }
        #endregion

        #region mapper
        private BPInstance BPInstanceMapper(IDataReader reader)
        {
            BPInstance instance = new BPInstance
            {
                ProcessInstanceID = (long)reader["ID"],
                Title = reader["Title"] as string,
                ParentProcessID = GetReaderValue<long?>(reader, "ParentID"),
                DefinitionID = (int)reader["DefinitionID"],
                WorkflowInstanceID = GetReaderValue<Guid?>(reader, "WorkflowInstanceID"),
                Status = (BPInstanceStatus)reader["ExecutionStatus"],
                LastMessage = reader["LastMessage"] as string,
                CreatedTime = (DateTime)reader["CreatedTime"],
                StatusUpdatedTime = GetReaderValue<DateTime?>(reader, "StatusUpdatedTime"),
                InitiatorUserId = GetReaderValue<int>(reader, "InitiatorUserId"),
                EntityId = reader["EntityId"] as string,
            };

            string inputArg = reader["InputArgument"] as string;
            if (!String.IsNullOrWhiteSpace(inputArg))
                instance.InputArgument = Serializer.Deserialize(inputArg) as BaseProcessInputArgument;

            return instance;
        }

        private BPPendingInstanceInfo BPPendingInstanceInfoMapper(IDataReader reader)
        {
            BPPendingInstanceInfo instance = new BPPendingInstanceInfo
            {
                BPDefinitionId = (int)reader["DefinitionID"],
                ProcessInstanceId = (long)reader["ID"],
                ParentProcessInstanceId = GetReaderValue<long?>(reader, "ParentID"),
                Status = (BPInstanceStatus)reader["ExecutionStatus"],
                ServiceInstanceId = GetReaderValue<Guid?>(reader, "ServiceInstanceID")
            };

            return instance;
        }

        private BPInstanceDetail BPInstanceDetailMapper(IDataReader reader)
        {
            return new BPInstanceDetail()
            {
                Entity = BPInstanceMapper(reader)
            };
        }
        #endregion
        
        public void SetServiceInstancesOfBPInstances(List<BPPendingInstanceInfo> pendingInstancesToUpdate)
        {
            foreach (var pendingInstance in pendingInstancesToUpdate)
            {
                if (!pendingInstance.ServiceInstanceId.HasValue)
                    throw new NullReferenceException(String.Format("pendingInstance.ServiceInstanceId. ProcessInstanceId '{0}'", pendingInstance.ProcessInstanceId));
                ExecuteNonQuerySP("[bp].[sp_BPInstance_UpdateServiceInstanceID]", pendingInstance.ProcessInstanceId, pendingInstance.ServiceInstanceId.Value);
            }
        }
    }
}
