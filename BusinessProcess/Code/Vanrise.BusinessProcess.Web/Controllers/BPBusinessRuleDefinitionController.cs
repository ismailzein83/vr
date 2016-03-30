using System.Collections.Generic;
using System.Web.Http;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Web.Base;

namespace Vanrise.BusinessProcess.Web.Controllers
{
    [Vanrise.Web.Base.JSONWithType]
    [RoutePrefix(Constants.ROUTE_PREFIX + "BPBusinessRuleDefinition")]
    public class BPBusinessRuleDefinitionController : BaseAPIController
    {
        [HttpGet]
        [Route("GetBusinessRuleDefintionsByBPDefinitionID")]
        public List<BPBusinessRuleDefinitionDetail> GetBusinessRuleDefintionsByBPDefinitionID(int bpDefinitionId)
        {
            BPBusinessRuleDefinitionManager manager = new BPBusinessRuleDefinitionManager();
            return manager.GetBusinessRuleDefintionsByBPDefinitionID(bpDefinitionId);
        }
    }
}