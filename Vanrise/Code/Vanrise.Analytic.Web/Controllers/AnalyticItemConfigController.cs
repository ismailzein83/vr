using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Analytic.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "AnalyticItemConfig")]
    public class AnalyticItemConfigController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        [Route("GetDimensionsInfo")]
        public IEnumerable<AnalyticDimensionConfigInfo> GetDimensionsInfo(string filter)
        {
            AnalyticItemConfigManager manager = new AnalyticItemConfigManager();
            AnalyticDimensionConfigInfoFilter serializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<AnalyticDimensionConfigInfoFilter>(filter) : null;
            return manager.GetDimensionsInfo(serializedFilter);
        }

        [HttpGet]
        [Route("GetMeasuresInfo")]
        public IEnumerable<AnalyticMeasureConfigInfo> GetMeasuresInfo(string filter)
        {
            AnalyticItemConfigManager manager = new AnalyticItemConfigManager();
            AnalyticMeasureConfigInfoFilter serializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<AnalyticMeasureConfigInfoFilter>(filter) : null;
            return manager.GetMeasuresInfo(serializedFilter);
        }

    }
}