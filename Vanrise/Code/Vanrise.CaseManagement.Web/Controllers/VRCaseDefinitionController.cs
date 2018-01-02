using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.CaseManagement.Business;
using Vanrise.Web.Base;

namespace Vanrise.CaseManagement.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRCaseDefinition")]
    [JSONWithTypeAttribute]
    public class VRCaseDefinitionController : BaseAPIController
    {
        VRCaseDefinitionManager _manager = new VRCaseDefinitionManager();
        [HttpGet]
        [Route("GetVRCaseGridDefinition")]
        public VRCaseGridDefinition GetVRCaseGridDefinition(Guid vrCaseDefinitionId)
        {
            return _manager.GetVRCaseGridDefinition(vrCaseDefinitionId);
        }

    }
}