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
    [RoutePrefix(Constants.ROUTE_PREFIX + "MasterLog")]
    public class VRCommon_MasterLogController : BaseAPIController
    {
        [HttpGet]
        [Route("GetMasterLogDirectives")]
        public List<LogViewInfo> GetMasterLogDirectives(Guid viewId)
        {
            MasterLogManager manager = new MasterLogManager();
            return manager.GetMasterLogDirectives(viewId);
        }

    }
}