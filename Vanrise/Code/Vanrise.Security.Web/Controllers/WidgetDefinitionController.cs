using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Security.Web.Controllers
{
     [RoutePrefix(Constants.ROUTE_PREFIX + "WidgetDefinition")]
    public class WidgetDefinitionController:BaseAPIController
    {
        [HttpGet]
        [Route("GetWidgetsDefinition")]
        public IEnumerable<WidgetDefinition> GetWidgetsDefinition()
        {
            WidgetDefinitionManager manager = new WidgetDefinitionManager();
            return manager.GetWidgetsDefinition();
        }
    }
}