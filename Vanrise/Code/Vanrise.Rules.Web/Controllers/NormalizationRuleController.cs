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
    public class NormalizationRuleController:BaseAPIController
    {
        [HttpGet]
        [Route("GetNormalizeNumberActionSettingsTemplates")]
        public List<TemplateConfig> GetNormalizeNumberActionSettingsTemplates()
        {
            NormalizationRuleManager manager = new NormalizationRuleManager();
            return manager.GetNormalizeNumberActionSettingsTemplates();
        }
    }
}