using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using CP.Ringo.Business;
using CP.Ringo.Entities;
using Retail.Ringo.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace CP.Ringo.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "AgentNumbers")]
    [JSONWithTypeAttribute]
    public class AgentNumbersController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredAgentNumbers")]
        public object GetFilteredAgentNumbers(DataRetrievalInput<PortalAgentNumberRequestQuery> input)
        {
            AgentNumbersManager manager = new AgentNumbersManager();
            return GetWebResponse(input, manager.GetFilteredAgentNumbers(input));
        }
        [HttpPost]
        [Route("AddAgentNumbersRequest")]
        public object AddAgentNumbersRequest(AgentNumberRequest agentNumbersRequest)
        {
            AgentNumbersManager manager = new AgentNumbersManager();
            return manager.AddAgentNumbersRequest(agentNumbersRequest);
        }
    }
}