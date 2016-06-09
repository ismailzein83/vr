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
    [RoutePrefix(Constants.ROUTE_PREFIX + "AnalyticItemAction")]
    public class AnalyticItemActionController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        [Route("GetAnalyticItemActionsTemplateConfigs")]
        public IEnumerable<AnalyticItemActionTemplate> GetAnalyticItemActionsTemplateConfigs()
        {
            AnalyticConfigurationManager manager = new AnalyticConfigurationManager();
            return manager.GetAnalyticItemActionsTemplateConfigs();
        }
    }
}