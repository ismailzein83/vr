using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Reprocess.Business;
using Vanrise.Reprocess.Entities;
using Vanrise.Web.Base;
using Vanrise.Entities;

namespace Vanrise.Reprocess.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "ReprocessFilterDefinition")]
    [JSONWithTypeAttribute]
    public class ReprocessFilterDefinitionController : BaseAPIController
    {
        ReprocessFilterDefinitionManager _manager = new ReprocessFilterDefinitionManager();

        [HttpGet]
        [Route("GetReprocessFilterDefinitionConfigs")]
        public IEnumerable<ReprocessFilterDefinitionConfig> GetReprocessFilterDefinitionConfigs()
        {
            return _manager.GetReprocessFilterDefinitionConfigs();
        }
    }
}