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
            : base(ConfigurationManager.AppSettings["BusinessProcessDBConnStringKey"] ?? "TransactionDBConnString")
        {
        }  

        public List<BPDefinition> GetDefinitions()
        {
            return GetItemsSP("bp.sp_BPDefinition_GetAll", BPDefinitionMapper);
        }

        public int InsertInstance(Guid processInstanceId, string processTitle, Guid? parentId, int definitionID, object inputArguments, BPInstanceStatus executionStatus)
        {
            return ExecuteNonQuerySP("bp.sp_BPInstance_Insert", processInstanceId, processTitle, parentId, definitionID, inputArguments != null ? Serializer.Serialize(inputArguments) : null, (int)executionStatus);
        }

        public int UpdateInstanceStatus(Guid processInstanceId, BPInstanceStatus status, string message, int retryCount)
        {
            return ExecuteNonQuerySP("bp.sp_BPInstance_UpdateStatus", processInstanceId, (int)status, message, ToDBNullIfDefault(retryCount));
        }

        public int UpdateWorkflowInstanceID(Guid processInstanceId, Guid workflowInstanceId)
        {
            return ExecuteNonQuerySP("bp.sp_BPInstance_UpdateWorkflowInstanceID", processInstanceId, workflowInstanceId);
        }

        public int UpdateLoadedFlag(Guid processInstanceId, bool loaded)
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

        public int InsertEvent(Guid processInstanceId, string bookmarkName, object eventData)
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
                        ProcessInstanceID = (Guid)reader["ProcessInstanceID"],
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
                WorkflowType = Type.GetType(reader["FQTN"] as string)
            };
            string config = reader["Config"] as string;
            if (!String.IsNullOrWhiteSpace(config))
                bpDefinition.Configuration = Serializer.Deserialize<BPConfiguration>(config);
            return bpDefinition;
        }

        private BPInstance BPInstanceMapper(IDataReader reader, bool withInputArguments = false)
        {
            BPInstance instance = new BPInstance
            {
                ProcessInstanceID = (Guid)reader["ID"],
                Title = reader["Title"] as string,
                ParentProcessID = GetReaderValue<Guid?>(reader, "ParentID"),
                DefinitionID = (int)reader["DefinitionID"],
                WorkflowInstanceID = GetReaderValue<Guid?>(reader, "WorkflowInstanceID"),
                Status = (BPInstanceStatus)reader["ExecutionStatus"],
                RetryCount = GetReaderValue<int>(reader, "RetryCount"),
                LastMessage = reader["LastMessage"] as string
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
