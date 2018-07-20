using System;
using System.Web.Http;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Web.Base;

namespace Vanrise.BusinessProcess.Web.Controllers
{
    [Vanrise.Web.Base.JSONWithType]
    [RoutePrefix(Constants.ROUTE_PREFIX + "BPDefinitionArgumentState")]

    public class BPDefinitionArgumentStateController : BaseAPIController
    {
        BPDefintionArgumentStateManager _manager = new BPDefintionArgumentStateManager();

        [HttpGet]
        [Route("GetBPDefinitionArgumentState")]
        public BPDefinitionArgumentState GetBPDefinitionArgumentState(Guid bpDefinitionId) {
            return _manager.GetBPDefinitionArgumentState(bpDefinitionId);
        }
    }
}