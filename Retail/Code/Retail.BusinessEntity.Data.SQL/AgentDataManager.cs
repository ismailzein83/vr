using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Retail.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace Retail.BusinessEntity.Data.SQL
{
    public class AgentDataManager : BaseSQLDataManager, IAgentDataManager
    {

        public IEnumerable<Agent> GetAgents()
        {
            return GetItemsSP("Retail.sp_Agent_GetAll", AgentMapper);
        }

        public bool Insert(Agent agent, out long insertedId)
        {
            string serializedSettings = agent.Settings != null ? Serializer.Serialize(agent.Settings) : null;
            object agentId;
            int affectedRecords = ExecuteNonQuerySP("Retail.sp_Agent_Insert", out agentId, agent.Name, agent.Type, serializedSettings, agent.SourceId);

            if (affectedRecords > 0)
            {
                insertedId = (int)agentId;
                return true;
            }
            insertedId = -1;
            return false;
        }

        public bool Update(Agent agent)
        {
            string serializedSettings = agent.Settings != null ? Serializer.Serialize(agent.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("Retail.sp_Agent_Update", agent.Id, agent.Name, agent.Type, serializedSettings, agent.SourceId);
            return (affectedRecords > 0);
        }

        public bool AreAgentsUpdated(ref object updateHandle)
        {
            return IsDataUpdated("Retail.Agent", ref updateHandle);
        }

        #region Mapper
        private Agent AgentMapper(IDataReader reader)
        {
            return new Agent
            {
                Id = (long)reader["ID"],
                Name = reader["Name"] as string,
                Type = reader["TypeID"] as string,
                Settings = Serializer.Deserialize<AgentSetting>(reader["Settings"] as string),
                SourceId = reader["SourceID"] as string
            };
        }

        #endregion

    }
}
