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
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRConnection")]
    [JSONWithTypeAttribute]
    public class VRConnectionController : BaseAPIController
    {
        VRConnectionManager _manager = new VRConnectionManager();

        [HttpGet]
        [Route("GetVRConnectionInfos")]
        public IEnumerable<VRConnectionInfo> GetVRConnectionInfos(string filter = null)
        {
            VRConnectionFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<VRConnectionFilter>(filter) : null;
            return _manager.GetVRConnectionInfos(deserializedFilter);
        }
    }
}