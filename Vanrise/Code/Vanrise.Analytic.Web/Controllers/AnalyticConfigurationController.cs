using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Analytic.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "AnalyticConfiguration")]
    public class AnalyticConfigurationController : Vanrise.Web.Base.BaseAPIController
    {
        AnalyticConfigurationManager _manager = new AnalyticConfigurationManager();

        [HttpGet]
        [Route("GetAnalyticReportSettingsTemplateConfigs")]
        public IEnumerable<HistorySearchSetting> GetAnalyticReportSettingsTemplateConfigs()
        {
            return _manager.GetAnalyticReportSettingsTemplateConfigs();
        }

        [HttpGet] 
        [Route("GetWidgetsTemplateConfigs")]
        public IEnumerable<WidgetDefinitionSetting> GetWidgetsTemplateConfigs()
        {
            return _manager.GetWidgetsTemplateConfigs();
        }

        [HttpGet]
        [Route("GetRealTimeReportSettingsTemplateConfigs")]
        public IEnumerable<RealTimeSearchSetting> GetRealTimeReportSettingsTemplateConfigs()
        {
            return _manager.GetRealTimeReportSettingsTemplateConfigs();
        }

        [HttpGet]
        [Route("GetRealTimeWidgetsTemplateConfigs")]
        public IEnumerable<RealTimeWidgetSetting> GetRealTimeWidgetsTemplateConfigs()
        {
            return _manager.GetRealTimeWidgetsTemplateConfigs();
        }

        [HttpGet]
        [Route("GetMeasureStyleRuleTemplateConfigs")]
        public IEnumerable<MeasureStyleRuleTemplate> GetMeasureStyleRuleTemplateConfigs()
        {
            return _manager.GetMeasureStyleRuleTemplateConfigs();
        }

        [HttpGet]
        [Route("GetAnalyticDataProviderConfigs")]
        public IEnumerable<AnalyticDataProviderConfig> GetAnalyticDataProviderConfigs()
        {
            return _manager.GetAnalyticDataProviderConfigs();
        }

        [HttpGet]
        [Route("GetVRRestAPIAnalyticQueryInterceptorConfigs")]
        public IEnumerable<VRRestAPIAnalyticQueryInterceptorConfig> GetVRRestAPIAnalyticQueryInterceptorConfigs()
        {
            return _manager.GetVRRestAPIAnalyticQueryInterceptorConfigs();
        }

        [HttpGet]
        [Route("GetDRSearchPageSubviewDefinitionSettingsConfigs")]
        public IEnumerable<DRSearchPageSubviewDefinitionSettingsConfig> GetDRSearchPageSubviewDefinitionSettingsConfigs()
        {
            return _manager.GetDRSearchPageSubviewDefinitionSettingsConfigs();
        }
    }
}