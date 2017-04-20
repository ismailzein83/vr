using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Common.Business.GenericLKUP;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "GenericLKUPDefinition")]
    [JSONWithTypeAttribute]
    public class GenericLKUPDefinitionController : BaseAPIController
    {
        GenericLKUPDefinitionManager _manager = new GenericLKUPDefinitionManager();

        [HttpGet]
        [Route("GetGenericLKUPDefintionTemplateConfigs")]
        public IEnumerable<GenericLKUPDefinitionConfig> GetGenericLKUPDefintionTemplateConfigs()
        {
            return _manager.GetGenericLKUPDefintionTemplateConfigs();
        }
    }
}