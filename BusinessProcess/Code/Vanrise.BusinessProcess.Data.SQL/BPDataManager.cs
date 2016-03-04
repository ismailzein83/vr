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


        public Dictionary<long, BPInstanceStatus> GetProcessesStatuses(List<long> Ids)
        {
            string stringIDs = (Ids != null && Ids.Count > 0) ? string.Join(",", Ids) : null;

            Dictionary<long, BPInstanceStatus> processesStatuses = new Dictionary<long, BPInstanceStatus>();

            ExecuteReaderSP("bp.sp_BPInstance_GetStatusesByIDs", (reader) =>
            {
                while (reader.Read())
                {
                    processesStatuses.Add((long)reader["ID"], (BPInstanceStatus)reader["ExecutionStatus"]);
                }

            }, stringIDs);

            return processesStatuses;
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
            if (objectKey == null)
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

        public int InsertEvent(long processInstanceId, string bookmarkName, object eventData)
        {
            return ExecuteNonQuerySP("bp.sp_BPEvent_Insert", processInstanceId, bookmarkName, eventData != null ? Serializer.Serialize(eventData) : null);
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
                CreatedTime = (DateTime)reader["CreatedTime"],
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
