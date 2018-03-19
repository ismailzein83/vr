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
    [RoutePrefix(Constants.ROUTE_PREFIX + "QualityConfiguration")]
    [JSONWithTypeAttribute]
    public class QualityConfigurationController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        [Route("GetQualityConfigurationInfo")]
        public IEnumerable<QualityConfigurationInfo> GetQualityConfigurationInfo(string filter = null)
        {
            QualityConfigurationInfoFilter qualityConfigurationInfoFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<QualityConfigurationInfoFilter>(filter) : null;
            return new QualityConfigurationManager().GetQualityConfigurationInfo(qualityConfigurationInfoFilter);
        }

        [HttpGet]
        [Route("ValidateRouteRuleQualityConfiguration")]
        public bool ValidateRouteRuleQualityConfiguration(string serializedRouteRuleQualityConfiguration)
        {
            RouteRuleQualityConfiguration routeRuleQualityConfiguration = Vanrise.Common.Serializer.Deserialize<RouteRuleQualityConfiguration>(serializedRouteRuleQualityConfiguration);
            return new QualityConfigurationManager().ValidateRouteRuleQualityConfiguration(routeRuleQualityConfiguration);
        }
    }
}