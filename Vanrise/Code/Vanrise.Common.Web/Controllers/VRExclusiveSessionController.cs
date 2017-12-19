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
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRExclusiveSession")]
    [JSONWithTypeAttribute]
    public class VRExclusiveSessionController : BaseAPIController
    {
        VRExclusiveSessionManager _manager = new VRExclusiveSessionManager();

        [HttpPost]
        [Route("GetFilteredVRExclusiveSessions")]
        public object GetFilteredVRExclusiveSessions(Vanrise.Entities.DataRetrievalInput<VRExclusiveSessionQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredVRExclusiveSessions(input));
        }

        [HttpGet]
        [Route("ForceReleaseSession")]
        public void ForceReleaseSession(int vrExclusiveSessionId)
        {
             _manager.ForceReleaseSession(vrExclusiveSessionId);
        }

        [HttpGet]
        [Route("ForceReleaseAllSessions")]
        public void ForceReleaseAllSessions()
        {
            _manager.ForceReleaseAllSessions();
        }
    }
}