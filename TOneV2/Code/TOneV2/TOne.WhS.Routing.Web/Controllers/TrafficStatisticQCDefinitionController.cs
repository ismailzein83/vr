using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.Routing.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "TrafficStatisticQCDefinition")]
    [JSONWithTypeAttribute]
    public class TrafficStatisticQCDefinitionController : Vanrise.Web.Base.BaseAPIController
    {
        TrafficStatisticQCDefinitionManager manager = new TrafficStatisticQCDefinitionManager();

        [HttpGet]
        [Route("GetTrafficStatisticQCDefinitionData")]
        public TrafficStatisticQCDefinitionData GetTrafficStatisticQCDefinitionData(Guid qualityConfigurationDefinitionId)
        {
            return manager.GetTrafficStatisticQCDefinitionData(qualityConfigurationDefinitionId);
        }
    }
}