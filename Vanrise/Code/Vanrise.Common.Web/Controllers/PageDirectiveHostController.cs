using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Logging.SQL;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "PageDirectiveHost")]
    public class VRCommon_PageDirectiveHostController : BaseAPIController
    {
        [HttpGet]
        [Route("GetPageDirectiveHostInfo")]
        public PageDirectiveHostInfo GetPageDirectiveHostInfo(Guid viewId)
        {
            PageDirectiveHostManager manager = new PageDirectiveHostManager();
            return manager.GetPageDirectiveHostInfo(viewId);
        }

    }
}