using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Demo.Module.Entities;
using Demo.Module.Business;
using Vanrise.Entities;

namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "SettingConfigs")]
    [JSONWithTypeAttribute]
    public class Demo_Module_SettingConfigsController : BaseAPIController
    {
        [HttpGet]
        [Route("GetSettingTypeTemplateConfigs")]
        public IEnumerable<SettingConfig> GetSettingTypeTemplateConfigs()
        {
            SettingConfigsManager settingConfigsManager = new SettingConfigsManager();
            return settingConfigsManager.GetSettingTypeTemplateConfigs(); 
        }

        [HttpGet]
        [Route("GetDimensionsTypeTemplateConfigs")]
        public IEnumerable<DimensionsConfig> GetDimensionsTypeTemplateConfigs()
        {
            DimensionsConfigsManager dimConfigsManager = new DimensionsConfigsManager();
            return dimConfigsManager.GetDimensionsTypeTemplateConfigs();
        }
    }
}