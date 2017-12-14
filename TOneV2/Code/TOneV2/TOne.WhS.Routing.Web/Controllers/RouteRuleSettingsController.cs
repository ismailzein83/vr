using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.Routing.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RouteRuleSettings")]
    [JSONWithTypeAttribute]
    public class RouteRuleSettingsController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        [Route("GetRouteOptionSettingsGroupTemplates")]
        public IEnumerable<RouteOptionSettingsGroupConfig> GetRouteOptionSettingsGroupTemplates()
        {
            RouteRuleSettingsManager manager = new RouteRuleSettingsManager();
            return manager.GetRouteOptionSettingsGroupTemplates();
        }

        [HttpGet]
        [Route("GetRouteOptionOrderSettingsTemplates")]
        public IEnumerable<RouteRuleOptionOrderSettingsConfig> GetRouteOptionOrderSettingsTemplates()
        {
            RouteRuleSettingsManager manager = new RouteRuleSettingsManager();
            return manager.GetRouteOptionOrderSettingsTemplates();
        }

        [HttpGet]
        [Route("GetRouteOptionFilterSettingsTemplates")]
        public IEnumerable<RouteRuleOptionFilterSettingsConfig> GetRouteOptionFilterSettingsTemplates()
        {
            RouteRuleSettingsManager manager = new RouteRuleSettingsManager();
            return manager.GetRouteOptionFilterSettingsTemplates();
        }

        [HttpGet]
        [Route("GetRouteOptionPercentageSettingsTemplates")]
        public IEnumerable<RouteRuleOptionPercentageSettingsConfig> GetRouteOptionPercentageSettingsTemplates()
        {
            RouteRuleSettingsManager manager = new RouteRuleSettingsManager();
            return manager.GetRouteOptionPercentageSettingsTemplates();
        }

        [HttpGet]
        [Route("GetRoutingOptimizerSettingsConfigs")]
        public IEnumerable<RoutingOptimizerSettingsConfig> GetRoutingOptimizerSettingsConfigs()
        {
            RouteRuleSettingsManager manager = new RouteRuleSettingsManager();
            return manager.GetRoutingOptimizerSettingsConfigs();
        }

        [HttpGet]
        [Route("GetCustomerRouteBuildNumberOfOptions")]
        public int GetCustomerRouteBuildNumberOfOptions()
        {
            ConfigManager configManager = new ConfigManager();
            return configManager.GetCustomerRouteBuildNumberOfOptions();
        }

        [HttpGet]
        [Route("GetQualityConfigurationFields")]
        public List<AnalyticMeasureInfo> GetQualityConfigurationFields()
        {
            ConfigManager configManager = new ConfigManager();
            return configManager.GetQualityConfigurationFields();
        }
        
        [HttpGet]
        [Route("GetQualityConfigurationInfo")]
        public IEnumerable<QualityConfigurationInfo> GetQualityConfigurationInfo (string filter = null) {  
            QualityConfigurationInfoFilter qualityConfigurationInfoFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<QualityConfigurationInfoFilter>(filter) : null;
            return new QualityConfigurationManager().GetQualityConfigurationInfo(qualityConfigurationInfoFilter);
        }
    }
}