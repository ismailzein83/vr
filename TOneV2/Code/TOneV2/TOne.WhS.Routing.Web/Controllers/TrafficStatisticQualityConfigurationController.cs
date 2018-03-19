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
    [RoutePrefix(Constants.ROUTE_PREFIX + "TrafficStatisticQualityConfiguration")]
    [JSONWithTypeAttribute]
    public class TrafficStatisticQualityConfigurationController : Vanrise.Web.Base.BaseAPIController
    {
        TrafficStatisticQualityConfigurationManager manager = new TrafficStatisticQualityConfigurationManager();

        [HttpGet]
        [Route("GetTrafficStatisticQualityConfigurationMeasures")]
        public List<AnalyticMeasureInfo> GetTrafficStatisticQualityConfigurationMeasures(Guid qualityConfigurationDefinitionId)
        {
            return manager.GetTrafficStatisticQualityConfigurationMeasures(qualityConfigurationDefinitionId);
        }
    }
}