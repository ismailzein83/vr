using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "Agent")]
    public class AgentController : BaseAPIController
    {

        [HttpPost]
        [Route("GetFilteredAccounts")]
        public object GetFilteredAccounts(Vanrise.Entities.DataRetrievalInput<AgentQuery> input)
        {
            AgentManager agentManager = new AgentManager();
            return GetWebResponse(input, agentManager.GetFilteredAgents(input));
        }

        [HttpGet]
        [Route("GetAgentsInfo")]
        public IEnumerable<AgentInfo> GetAgentsInfo()
        {
            AgentManager agentManager = new AgentManager();
            return agentManager.GetAgentsInfo(null);
        }

        [HttpPost]
        [Route("AddAgent")]
        public Vanrise.Entities.InsertOperationOutput<AgentDetail> AddAgent(Agent agent)
        {
            AgentManager agentManager = new AgentManager();
            return agentManager.AddAgent(agent);
        }

        [HttpPost]
        [Route("UpdateAgent")]
        public Vanrise.Entities.UpdateOperationOutput<AgentDetail> UpdateAgent(Agent agent)
        {
            AgentManager agentManager = new AgentManager();
            return agentManager.UpdateAgent(agent);
        }
    }
}