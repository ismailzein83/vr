using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;
using Vanrise.BEBridge.Entities;
using Vanrise.Common;

namespace Retail.BusinessEntity.Business
{
    public class AgentSynchronizer : TargetBESynchronizer
    {
        public override string Name
        {
            get
            {
                return "Agents";
            }
        }
        public override void InsertBEs(ITargetBESynchronizerInsertBEsContext context)
        {
            if (context.TargetBE == null)
                throw new NullReferenceException("context.TargetBE");
            AgentManager agentManager = new AgentManager();
            foreach (var targetAccount in context.TargetBE)
            {
                long agentId;
                SourceAgent agentData = targetAccount as SourceAgent;
                agentManager.TryAddAgent(agentData.Agent, out agentId);
            }
        }

        public override bool TryGetExistingBE(ITargetBESynchronizerTryGetExistingBEContext context)
        {
            AgentManager agentManager = new AgentManager();
            Agent agent = agentManager.GetAgentBySourceId(context.SourceBEId as string);
            if (agent != null)
            {
                context.TargetBE = new SourceAgent
                {
                    Agent = Serializer.Deserialize<Agent>(Serializer.Serialize(agent))
                };
                return true;
            }
            return false;
        }

        public override void UpdateBEs(ITargetBESynchronizerUpdateBEsContext context)
        {
            if (context.TargetBE == null)
                throw new NullReferenceException("context.TargetBE");
            AgentManager agentManager = new AgentManager();

            foreach (var target in context.TargetBE)
            {
                SourceAgent agentData = target as SourceAgent;
                Agent agent = new Agent
                {
                    Id = agentData.Agent.Id,
                    Type = agentData.Agent.Type,
                    Name = agentData.Agent.Name,
                    Settings = agentData.Agent.Settings,
                    SourceId = agentData.Agent.SourceId
                };
                agentManager.TryUpdateAgent(agent);
            }
        }
    }
}
