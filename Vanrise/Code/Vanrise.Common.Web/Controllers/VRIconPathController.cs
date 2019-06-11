using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRIconPath")]
    public class VRCommon_VRIconPathController : BaseAPIController
    {
        [HttpGet]
        [Route("GetVRIconPathsInfo")]
        public IEnumerable<VRIconPath> GetVRIconPathsInfo([FromUri]List<VRIconVirtualPath> paths)
        {
            VRIconPathManger manager = new VRIconPathManger();
            return manager.GetVRIconPathsInfo(paths);
        }

    }
}