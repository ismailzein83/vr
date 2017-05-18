using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRRuleDefinition")]
    [JSONWithTypeAttribute]
    public class VRRuleDefinitionController : BaseAPIController
    {
        VRRuleDefinitionManager _manager = new VRRuleDefinitionManager();

        [HttpGet]
        [Route("GetVRRuleDefinitionExtendedSettingsConfigs")]
        public IEnumerable<VRRuleDefinitionExtendedSettingsConfig> GetVRRuleDefinitionExtendedSettingsConfigs()
        {
            return _manager.GetVRRuleDefinitionExtendedSettingsConfigs();
        }
    }
}