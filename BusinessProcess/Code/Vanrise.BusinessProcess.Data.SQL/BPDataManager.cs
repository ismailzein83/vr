using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.Common;
using Vanrise.BusinessProcess.Entities;
using System.Data.SqlClient;

namespace Vanrise.BusinessProcess.Data.SQL
{
    internal class BPDataManager : BaseSQLDataManager, IBPDataManager
    {
        public BPDataManager()
            : base(GetConnectionStringName("BusinessProcessDBConnStringKey", "BusinessProcessDBConnString"))
        {
        }

        public BPDefinition GetDefinition(int ID)
        {
            return GetItemSP("bp.sp_BPDefinition_Get", BPDefinitionMapper, ID);
        }


        public Vanrise.Entities.BigResult<BPDefinition> GetFilteredDefinitions(Vanrise.Entities.DataRetrievalInput<BPDefinitionQuery> input)
        {
            return RetrieveData(input, (tempTableName) =>
            {
                ExecuteNonQuerySP("bp.sp_BPDefinition_CreateTempForFiltered", tempTableName, input.Query.Title);

            }, BPDefinitionMapper);
        }



        public List<BPDefinition> GetDefinitions()
        {
            return GetItemsSP("bp.sp_BPDefinition_GetAll", BPDefinitionMapper);
        }

        public T GetDefinitionObjectState<T>(int definitionId, string objectKey)
        {
            if (objectKey == null)
                objectKey = String.Empty;
            string objectVal = ExecuteScalarSP("bp.sp_BPDefinitionState_GetByKey", definitionId, objectKey) as string;
            return objectVal != null ? Serializer.Deserialize<T>(objectVal) : Activator.CreateInstance<T>();
        }

        public int InsertDefinitionObjectState(int definitionId, string objectKey, object objectValue)
        {
            if(objectKey == null)
                objectKey = String.Empty;
            return ExecuteNonQuerySP("bp.sp_BPDefinitionState_Insert", definitionId, objectKey, objectKey != null ? Serializer.Serialize(objectValue) : null);
        }

        public int UpdateDefinitionObjectState(int definitionId, string objectKey, object objectValue)
        {
            if (objectKey == null)
                objectKey = String.Empty;
            return ExecuteNonQuerySP("bp.sp_BPDefinitionState_Update", definitionId, objectKey, objectKey != null ? Serializer.Serialize(objectValue) : null);
        }

        public List<BPInstance> GetInstancesByCriteria(int definitionID, DateTime dateFrom, DateTime dateTo)
        {
            return GetItemsSP("bp.sp_BPInstance_GetByCriteria", BPInstanceMapper, definitionID, dateFrom, dateTo);
        }


        public List<BPInstance> GetRecentInstances(DateTime? StatusUpdatedAfter)
        {
            return GetItemsSP("bp.sp_BPInstance_GetRecent", BPInstanceMapper, StatusUpdatedAfter);
        }


        public Vanrise.Entities.BigResult<BPInstance> GetInstancesByCriteria(Vanrise.Entities.DataRetrievalInput<BPInstanceQuery> input)
        {
            return RetrieveData(input, (tempTableName) =>
            {
                ExecuteNonQuerySP("bp.sp_BPInstance_CreateTempForFiltered", tempTableName, input.Query.DefinitionsId == null ? null : string.Join(",", input.Query.DefinitionsId.Select(n => n.ToString()).ToArray()),
                    input.Query.InstanceStatus == null ? null : string.Join(",", input.Query.InstanceStatus.Select(n => ((int)n).ToString()).ToArray()), input.Query.DateFrom, input.Query.DateTo);

            }, BPInstanceMapper);
        }
       
        public BPInstance GetInstance(long instanceId)
        {
            return GetItemSP("[bp].[sp_BPInstance_GetByID]", BPInstanceMapper, instanceId);
        }

        public long InsertInstance(string processTitle, long? parentId, int definitionId, object inputArguments, BPInstanceStatus executionStatus)
        {
            object processInstanceId;
            if (ExecuteNonQuerySP("bp.sp_BPInstance_Insert", out processInstanceId, processTitle, parentId, definitionId, inputArguments != null ? Serializer.Serialize(inputArguments) : null, (int)executionStatus) > 0)
                return (long)processInstanceId;
            else
                return 0;
        }

        public int UpdateInstanceStatus(long processInstanceId, BPInstanceStatus status, string message, int retryCount)
        {
            return ExecuteNonQuerySP("bp.sp_BPInstance_UpdateStatus", processInstanceId, (int)status, message, ToDBNullIfDefault(retryCount));
        }

        public int UpdateWorkflowInstanceID(long processInstanceId, Guid workflowInstanceId)
        {
            return ExecuteNonQuerySP("bp.sp_BPInstance_UpdateWorkflowInstanceID", processInstanceId, workflowInstanceId);
        }

        public void LoadPendingProcesses(List<long> excludedProcessInstanceIds, IEnumerable<BPInstanceStatus> acceptableBPStatuses, Action<BPInstance> onInstanceLoaded)
        {
            ExecuteReaderSPCmd("bp.sp_BPInstance_GetPendings", (reader) =>
                {
                    while (reader.Read())
                    {
                        BPInstance instance = BPInstanceMapper(reader, true);
                        onInstanceLoaded(instance);
                    }
                },
                (cmd) =>
                {
                    var dtPrm = new SqlParameter("@ExcludeProcessInstanceIds", SqlDbType.Structured);
                    dtPrm.Value = BuildIDDataTable(excludedProcessInstanceIds);
                    cmd.Parameters.Add(dtPrm);
                    dtPrm = new SqlParameter("@BPStatuses", SqlDbType.Structured);
                    dtPrm.Value = BuildIDDataTable(acceptableBPStatuses.Select(itm => (int)itm));
                    cmd.Parameters.Add(dtPrm);
                });
        }

        public int InsertEvent(long processInstanceId, string bookmarkName, object eventData)
        {
            return ExecuteNonQuerySP("bp.sp_BPEvent_Insert", processInstanceId, bookmarkName, eventData != null ? Serializer.Serialize(eventData) : null);
        }

        public int DeleteEvent(long eventId)
        {
            return ExecuteNonQuerySP("bp.sp_BPEvent_Delete", eventId);
        }

        public void LoadPendingEvents(long lastRetrievedId, Action<BPEvent> onEventLoaded)
        {
            ExecuteReaderSP("bp.sp_BPEvent_GetPendings", (reader) =>
            {
                while (reader.Read())
                {
                    BPEvent instance = new BPEvent
                    {
                        BPEventID = (long)reader["ID"],
                        ProcessInstanceID = (long)reader["ProcessInstanceID"],
                        ProcessDefinitionID = (int)reader["DefinitionID"],
                        Bookmark = reader["Bookmark"] as string
                    };
                    string payload = reader["Payload"] as string;
                    if (!String.IsNullOrWhiteSpace(payload))
                        instance.Payload = Serializer.Deserialize(payload);
                    onEventLoaded(instance);
                }
            }, lastRetrievedId);
        }
        
        public bool TryLockProcessInstance(long processInstanceId, Guid workflowInstanceId, int currentRuntimeProcessId, IEnumerable<int> runningRuntimeProcessesIds, IEnumerable<BPInstanceStatus> acceptableStatuses)
        {
            int rslt = ExecuteNonQuerySPCmd("[bp].[sp_BPInstance_TryLockAndUpdateWorkflowInstanceID]",
                (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@ProcessInstanceID", processInstanceId));
                    cmd.Parameters.Add(new SqlParameter("@WorkflowInstanceID", workflowInstanceId));
                    cmd.Parameters.Add(new SqlParameter("@CurrentRuntimeProcessID", currentRuntimeProcessId));
                    var dtPrm = new SqlParameter("@RunningProcessIDs", SqlDbType.Structured);
                    dtPrm.Value = BuildIDDataTable(runningRuntimeProcessesIds);
                    cmd.Parameters.Add(dtPrm);
                    dtPrm = new SqlParameter("@BPStatuses", SqlDbType.Structured);
                    dtPrm.Value = BuildIDDataTable(acceptableStatuses.Select(itm => (int)itm));
                    cmd.Parameters.Add(dtPrm);
                });
            return rslt > 0;
        }

        public void UnlockProcessInstance(long processInstanceId, int currentRuntimeProcessId)
        {
            ExecuteNonQuerySP("[bp].[sp_BPInstance_UnLock]", processInstanceId, currentRuntimeProcessId);
        }

        public void UpdateProcessInstancesStatus(BPInstanceStatus fromStatus, BPInstanceStatus toStatus, IEnumerable<int> runningRuntimeProcessesIds)
        {
            ExecuteNonQuerySPCmd("[bp].[sp_BPInstance_UpdateStatuses]",
                 (cmd) =>
                 {
                     cmd.Parameters.Add(new SqlParameter("@FromStatus", (int)fromStatus));
                     cmd.Parameters.Add(new SqlParameter("@ToStatus", (int)toStatus));
                     var dtPrm = new SqlParameter("@RunningProcessIDs", SqlDbType.Structured);
                     dtPrm.Value = BuildIDDataTable(runningRuntimeProcessesIds);
                     cmd.Parameters.Add(dtPrm);
                 });
        }

        #region Mappers/Private Methods

        BPDefinition BPDefinitionMapper(IDataReader reader)
        {
            var bpDefinition = new BPDefinition
            {
                BPDefinitionID = (int)reader["ID"],
                Name = reader["Name"] as string,
                Title = reader["Title"] as string,
                WorkflowType = Type.GetType(reader["FQTN"] as string)
            };
            string config = reader["Config"] as string;
            if (!String.IsNullOrWhiteSpace(config))
                bpDefinition.Configuration = Serializer.Deserialize<BPConfiguration>(config);
            return bpDefinition;
        }
        private BPInstance BPInstanceMapper(IDataReader reader)
        {
            return BPInstanceMapper(reader, false);
        }
        private BPInstance BPInstanceMapper(IDataReader reader, bool withInputArguments)
        {
            BPInstance instance = new BPInstance
            {
                ProcessInstanceID = (long)reader["ID"],
                Title = reader["Title"] as string,
                ParentProcessID = GetReaderValue<long?>(reader, "ParentID"),
                DefinitionID = (int)reader["DefinitionID"],
                WorkflowInstanceID = GetReaderValue<Guid?>(reader, "WorkflowInstanceID"),
                Status = (BPInstanceStatus)reader["ExecutionStatus"],
                RetryCount = GetReaderValue<int>(reader, "RetryCount"),
                LastMessage = reader["LastMessage"] as string,
                CreatedTime =  (DateTime)reader["CreatedTime"],
                StatusUpdatedTime = GetReaderValue<DateTime?>(reader, "StatusUpdatedTime")
            };
            if (withInputArguments)
            {
                string inputArg = reader["InputArgument"] as string;
                if (!String.IsNullOrWhiteSpace(inputArg))
                    instance.InputArgument = Serializer.Deserialize(inputArg);
            }
            return instance;
        }

        DataTable BuildIDDataTable<T>(IEnumerable<T> ids)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(T));
            dt.BeginLoadData();
            if (ids != null)
            {
                foreach (var id in ids)
                {
                    dt.Rows.Add(id);
                }
            }
            dt.EndLoadData();
            return dt;
        }

        #endregion


    }
}
