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
    [RoutePrefix(Constants.ROUTE_PREFIX + "WidgetDefinition")]
    public class WidgetDefinitionController:BaseAPIController
    {
        [HttpGet]
        [Route("GetWidgetDefinitions")]
        public IEnumerable<WidgetDefinition> GetWidgetDefinitions()
        {
            WidgetDefinitionManager manager = new WidgetDefinitionManager();
            return manager.GetWidgetDefinitions();
        }

    }
}