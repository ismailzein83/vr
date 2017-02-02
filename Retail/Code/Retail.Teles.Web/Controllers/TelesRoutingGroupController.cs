using Retail.Teles.Business;
using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.Teles.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "TelesRoutingGroup")]
    public class TelesRoutingGroupController : BaseAPIController
    {
        TelesRoutingGroupManager _manager = new TelesRoutingGroupManager();
        [HttpGet]
        [Route("GetRoutingGroupConditionConfigs")]
        public IEnumerable<RoutingGroupConditionConfig> GetRoutingGroupConditionConfigs()
        {
            return _manager.GetRoutingGroupConditionConfigs();
        }
    }
}