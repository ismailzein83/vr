using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web.Controllers
{
   [RoutePrefix(Constants.ROUTE_PREFIX + "Switch")]
   [JSONWithTypeAttribute]
    public class SwitchController : BaseAPIController
    {
        [HttpGet]
        [Route("GetSwitchSettingsTemplateConfigs")]
       public IEnumerable<SwitchIntegrationConfig> GetSwitchSettingsTemplateConfigs()
        {
            SwitchManager manager = new SwitchManager();
            return manager.GetSwitchSettingsTemplateConfigs();
        }
       
    }
}