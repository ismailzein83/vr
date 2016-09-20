using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Rules.Normalization;
using Vanrise.Web.Base;

namespace Vanrise.Rules.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "NormalizationRule")]
    [JSONWithTypeAttribute]
    public class VR_Rules_NormalizationRuleController : BaseAPIController
    {
        [HttpGet]
        [Route("GetNormalizeNumberActionSettingsTemplates")]
        public IEnumerable<NormalizeNumberActionSettingsConfig> GetNormalizeNumberActionSettingsTemplates()
        {
            NormalizationRuleManager manager = new NormalizationRuleManager();
            return manager.GetNormalizeNumberActionSettingsTemplates();
        }
    }
}