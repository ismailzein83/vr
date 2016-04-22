using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;
namespace Vanrise.Analytic.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "AnalyticConfiguration")]
    public class AnalyticConfigurationController : Vanrise.Web.Base.BaseAPIController
    {

        [HttpGet]
        [Route("GetAnalyticReportSettingsTemplateConfigs")]
        public IEnumerable<TemplateConfig> GetAnalyticReportSettingsTemplateConfigs()
        {
            AnalyticConfigurationManager manager = new AnalyticConfigurationManager();
            return manager.GetAnalyticReportSettingsTemplateConfigs();
        }
        [HttpGet]
        [Route("GetWidgetsTemplateConfigs")]
        public IEnumerable<TemplateConfig> GetWidgetsTemplateConfigs()
        {
            AnalyticConfigurationManager manager = new AnalyticConfigurationManager();
            return manager.GetWidgetsTemplateConfigs();
        }
    }
}