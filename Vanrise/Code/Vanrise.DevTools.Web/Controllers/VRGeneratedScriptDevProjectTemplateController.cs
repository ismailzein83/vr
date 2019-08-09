using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Web.Base;
using Vanrise.DevTools.Entities;
using Vanrise.DevTools.Business;
using Vanrise.Entities;
using System;

namespace Vanrise.DevTools.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "DevProjectTemplate")]
    [JSONWithTypeAttribute]
    public class DevProjectTemplateController : BaseAPIController
    {
        VRGeneratedScriptDevProjectTemplateManager devProjectTemplateManager = new VRGeneratedScriptDevProjectTemplateManager();

        [HttpGet]
        [Route("GetVRDevProjectsInfo")]
        public IEnumerable<VRDevProjectInfo> GetVRDevProjectsInfo(Guid connectionId)
        {
            return devProjectTemplateManager.GetVRDevProjectsInfo(connectionId);
        }
    }
}