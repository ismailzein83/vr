using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.Common;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Data.SQL
{
    internal class BPDataManager : BaseSQLDataManager, IBPDataManager
    {
        public BPDataManager()
            : base(GetConnectionStringName("BusinessProcessDBConnStringKey", "BusinessProcessDBConnString"))
        {
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

        public List<BPInstance> GetInstancesByCriteria(int definitionID, DateTime datefrom, DateTime dateto)
        {            
            return GetItemsSP("bp.sp_BPInstance_GetByCriteria", BPInstanceMapper, definitionID, datefrom , dateto);
        }
       
        public BPInstance GetInstance(long instanceId)
        {
            return GetItemSP("[bp].[sp_BPInstance_GetByID]", BPInstanceMapper, instanceId);
        }

        public long InsertInstance(string processTitle, long? parentId, int definitionID, object inputArguments, BPInstanceStatus executionStatus)
        {
            object processInstanceId;
            if (ExecuteNonQuerySP("bp.sp_BPInstance_Insert", out processInstanceId, processTitle, parentId, definitionID, inputArguments != null ? Serializer.Serialize(inputArguments) : null, (int)executionStatus) > 0)
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

        public int UpdateLoadedFlag(long processInstanceId, bool loaded)
        {
            return ExecuteNonQuerySP("bp.sp_BPInstance_UpdateLoadedFlag", processInstanceId, loaded);
        }

        public int ClearLoadedFlag()
        {
            return ExecuteNonQuerySP("bp.sp_BPInstance_ClearLoadedFlag");
        }

        public void LoadPendingProcesses(Action<BPInstance> onInstanceLoaded)
        {
            ExecuteReaderSP("bp.sp_BPInstance_GetPendings", (reader) =>
                {
                    while (reader.Read())
                    {
                        BPInstance instance = BPInstanceMapper(reader, true);
                        onInstanceLoaded(instance);
                    }
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

        public void LoadPendingEvents(Action<BPEvent> onEventLoaded)
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
            });
        }

        #region Mappers

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
                CreatedTime =  (DateTime)reader["CreatedTime"]
            };
            if (withInputArguments)
            {
                string inputArg = reader["InputArgument"] as string;
                if (!String.IsNullOrWhiteSpace(inputArg))
                    instance.InputArgument = Serializer.Deserialize(inputArg);
            }
            return instance;
        }

        #endregion

    }
}
