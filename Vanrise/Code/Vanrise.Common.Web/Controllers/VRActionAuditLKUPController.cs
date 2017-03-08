using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Vanrise.Common.Business;

using Vanrise.Entities;
namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRActionAuditLKUP")]
    [JSONWithTypeAttribute]
    public class VRActionAuditLKUPController : BaseAPIController
    {
        VRActionAuditLKUPManager _manager = new VRActionAuditLKUPManager();

        [HttpGet]
        [Route("GetVRActionAuditLKUPInfo")]
        public IEnumerable<VRActionAuditLKUPInfo> GetVRActionAuditLKUPInfo(string filter = null)
        {
            VRActionAuditLKUPInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<VRActionAuditLKUPInfoFilter>(filter) : null;
            return _manager.GetVRActionAuditLKUPInfo(deserializedFilter);
        }
    }
}