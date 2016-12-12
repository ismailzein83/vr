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
    [RoutePrefix(Constants.ROUTE_PREFIX + "UserActionAudit")]
    public class VRCommon_UserActionAuditController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredUserActionAudits")]
        public object GetFilteredUserActionAudits(Vanrise.Entities.DataRetrievalInput<UserActionAuditQuery> input)
        {
            UserActionAuditManager manager = new UserActionAuditManager();
            return GetWebResponse(input, manager.GetFilteredUserActionAudits(input));
        }
    }
}