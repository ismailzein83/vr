using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "DevProject")]
    [JSONWithTypeAttribute]
    public class VRDevProjectController : BaseAPIController
    {
        VRDevProjectManager _manager = new VRDevProjectManager();

        [HttpGet]
        [Route("GetVRDevProjectsInfo")]
        public IEnumerable<VRDevProjectInfo> GetVRDevProjectsInfo(string filter = null)
        {
            VRDevProjectInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<VRDevProjectInfoFilter>(filter) : null;
            return _manager.GetVRDevProjectsInfo(deserializedFilter);
        }
        [HttpGet]
        [Route("TryCompileDevProject")]
        public VRDevProjectCompilationOutput TryCompileDevProject(Guid devProjectId, VRDevProjectCompilationBuildOption buildOption)
        {
            return _manager.TryCompileDevProject(devProjectId, buildOption);
        }
    }
}