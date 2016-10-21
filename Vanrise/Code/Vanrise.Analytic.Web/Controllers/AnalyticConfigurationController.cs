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
        public IEnumerable<HistorySearchSetting> GetAnalyticReportSettingsTemplateConfigs()
        {
            AnalyticConfigurationManager manager = new AnalyticConfigurationManager();
            return manager.GetAnalyticReportSettingsTemplateConfigs();
        }
        [HttpGet] 
        [Route("GetWidgetsTemplateConfigs")]
        public IEnumerable<WidgetDefinitionSetting> GetWidgetsTemplateConfigs()
        {
            AnalyticConfigurationManager manager = new AnalyticConfigurationManager();
            return manager.GetWidgetsTemplateConfigs();
        }

        [HttpGet]
        [Route("GetRealTimeReportSettingsTemplateConfigs")]
        public IEnumerable<RealTimeSearchSetting> GetRealTimeReportSettingsTemplateConfigs()
        {
            AnalyticConfigurationManager manager = new AnalyticConfigurationManager();
            return manager.GetRealTimeReportSettingsTemplateConfigs();
        }
        [HttpGet]
        [Route("GetRealTimeWidgetsTemplateConfigs")]
        public IEnumerable<RealTimeWidgetSetting> GetRealTimeWidgetsTemplateConfigs()
        {
            AnalyticConfigurationManager manager = new AnalyticConfigurationManager();
            return manager.GetRealTimeWidgetsTemplateConfigs();
        }
        [HttpGet]
        [Route("GetMeasureStyleRuleTemplateConfigs")]
        public IEnumerable<MeasureStyleRuleTemplate> GetMeasureStyleRuleTemplateConfigs()
        {
            AnalyticConfigurationManager manager = new AnalyticConfigurationManager();
            return manager.GetMeasureStyleRuleTemplateConfigs();
        }

        [HttpGet]
        [Route("GetAnalyticDataProviderConfigs")]
        public IEnumerable<AnalyticDataProviderConfig> GetAnalyticDataProviderConfigs()
        {
            AnalyticConfigurationManager manager = new AnalyticConfigurationManager();
            return manager.GetAnalyticDataProviderConfigs();
        }
        
    }
}