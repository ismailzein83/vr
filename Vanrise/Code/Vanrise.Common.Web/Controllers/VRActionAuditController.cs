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
    [RoutePrefix(Constants.ROUTE_PREFIX + "ActionAudit")]
    public class VRActionAuditController : BaseAPIController
    {
        VRActionAuditManager _manager = new VRActionAuditManager();

        [HttpPost]
        [Route("GetFilteredActionAudits")]
        public object GetFilteredActionAudits(Vanrise.Entities.DataRetrievalInput<VRActionAuditQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredActionAudits(input), "Action Audits");
        }
    }
}