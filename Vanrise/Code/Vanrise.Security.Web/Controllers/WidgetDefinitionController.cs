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
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "WidgetDefinition")]
    public class WidgetDefinitionController : BaseAPIController
    {
        WidgetDefinitionManager _manager;
        public WidgetDefinitionController()
        {
            _manager = new WidgetDefinitionManager();
        }

        [HttpGet]
        [Route("GetWidgetsDefinition")]
        public IEnumerable<WidgetDefinition> GetWidgetsDefinition()
        {
            return _manager.GetWidgetsDefinition();
        }
    }
}