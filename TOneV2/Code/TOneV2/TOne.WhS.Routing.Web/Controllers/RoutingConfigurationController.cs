using System.Collections.Generic;
using System.Web.Http;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.Routing.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RoutingConfiguration")]
    public class RoutingConfigurationController : BaseAPIController
    {
        [HttpGet]
        [Route("GetRoutingExcludedDestinationsTemplateConfigs")]
        public IEnumerable<RoutingExcludedDestinationsConfig> GetRoutingExcludedDestinationsTemplateConfigs()
        {
            RoutingConfigurationManager manager = new RoutingConfigurationManager();
            return manager.GetRoutingExcludedDestinationsTemplateConfigs();
        }
    }
}