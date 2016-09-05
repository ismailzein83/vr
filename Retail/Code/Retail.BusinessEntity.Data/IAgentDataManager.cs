using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.Data
{
    public interface IAgentDataManager : IDataManager
    {
        IEnumerable<Agent> GetAgents();

        bool Insert(Agent agent, out long insertedId);

        bool Update(Agent agent, long? parentId);

        bool AreAgentsUpdated(ref object updateHandle);
    }
}
