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
    [RoutePrefix(Constants.ROUTE_PREFIX + "KPISettingsController")]
    public class KPISettingsController : BaseAPIController
    {
        [HttpGet]
        [Route("GetAnalytictableKPISettings")]
        public List<MeasureStyleRule> GetAnalytictableKPISettings(Guid analyticTableId)
        {
            ConfigManager configManager = new ConfigManager();
            return configManager.GetAnalytictableKPISettings(analyticTableId);
        } 
    }
}