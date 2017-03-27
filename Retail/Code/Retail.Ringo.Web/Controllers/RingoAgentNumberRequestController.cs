using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Retail.Ringo.Business;
using Retail.Ringo.Entities;
using Vanrise.Web.Base;

namespace Retail.Ringo.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "RingoAgentNumberRequest")]
    public class RingoAgentNumberRequestController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredAgentNumberRequests")]
        public object GetFilteredProductFamilies(Vanrise.Entities.DataRetrievalInput<AgentNumberRequestQuery> input)
        {
            AgentRequestNumberManager manager = new AgentRequestNumberManager();
            return GetWebResponse(input, manager.GetFilteredAgentNumberRequests(input));
        }

        [HttpPost]
        [Route("AddAgentNumberRequest")]
        public object AddAgentNumberRequest(AgentNumberRequest agentNumberRequest)
        {
            AgentRequestNumberManager manager = new AgentRequestNumberManager();
            return manager.AddAgentNumberRequest(agentNumberRequest);
        }

        [HttpPost]
        [Route("UpdateAgentNumberRequest")]
        public object UpdateAgentNumberRequest(AgentNumberRequest agentNumberRequest)
        {
            AgentRequestNumberManager manager = new AgentRequestNumberManager();
            return manager.UpdateAgentNumberRequest(agentNumberRequest);
        }
    }
}