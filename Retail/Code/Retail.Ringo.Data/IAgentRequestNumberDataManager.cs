using System.Collections.Generic;
using Retail.Ringo.Entities;

namespace Retail.Ringo.Data
{
    public interface IAgentRequestNumberDataManager : IDataManager
    {
        bool AreAgentNumberRequestsUpdated(ref object updateHandle);
        bool Update(AgentNumberRequest agentNumberRequest);
        bool Insert(AgentNumberRequest agentNumberRequest, out int insertedId);
        IEnumerable<AgentNumberRequest> GetAgentNumberRequestByAgentId(long agentId);
        IEnumerable<AgentNumberRequest> GetAgentNumberRequests();
    }
}
