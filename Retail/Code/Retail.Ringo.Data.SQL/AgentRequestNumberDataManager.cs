using System;
using System.Collections.Generic;
using System.Data;
using Retail.Ringo.Entities;
using Vanrise.Data.SQL;

namespace Retail.Ringo.Data.SQL
{
    public class AgentRequestNumberDataManager : BaseSQLDataManager, IAgentRequestNumberDataManager
    {
        #region Constructor
        public AgentRequestNumberDataManager()
            : base(GetConnectionStringName("RingoDBConnStringKey", "RingoDBConnString"))
        {

        }

        #endregion
        public IEnumerable<AgentNumberRequest> GetAgentNumberRequests()
        {
            return GetItemsSP("Ringo.sp_AgentNumberRequest_GetAll", AgentNumberRequestMapper);
        }
        public IEnumerable<AgentNumberRequest> GetAgentNumberRequestByAgentId(long agentId)
        {
            return GetItemsSP("Ringo.sp_AgentNumberRequest_GetByAgentId", AgentNumberRequestMapper, agentId);
        }
        public bool Insert(AgentNumberRequest agentNumberRequest, out int insertedId)
        {
            object agentNumberRequestId;
            string serializedSettings = agentNumberRequest.Settings != null ? Vanrise.Common.Serializer.Serialize(agentNumberRequest.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("[Ringo].[sp_AgentNumberRequest_Insert]", out agentNumberRequestId, agentNumberRequest.AgentId, serializedSettings, agentNumberRequest.Status);
            insertedId = (int)agentNumberRequestId;
            return (affectedRecords > 0);
        }
        public bool Update(AgentNumberRequest agentNumberRequest)
        {
            string serializedSettings = agentNumberRequest.Settings != null ? Vanrise.Common.Serializer.Serialize(agentNumberRequest.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("[Ringo].[sp_AgentNumberRequest_Update]", agentNumberRequest.Id, agentNumberRequest.Status, serializedSettings);
            return (affectedRecords > 0);
        }
        public bool AreAgentNumberRequestsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[Ringo].[AgentNumberRequest]", ref updateHandle);
        }

        #region Mapper
        AgentNumberRequest AgentNumberRequestMapper(IDataReader reader)
        {
            string settings = reader["Settings"] as string;
            return new AgentNumberRequest
            {
                Id = (int)reader["ID"],
                AgentId = (long)reader["AgentID"],
                Status = (Status)GetReaderValue<byte>(reader, "Status"),
                Settings = string.IsNullOrEmpty(settings) ? null : Vanrise.Common.Serializer.Deserialize<AgentNumberSetting>(settings),
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime")
            };
        }

        #endregion
    }
}
