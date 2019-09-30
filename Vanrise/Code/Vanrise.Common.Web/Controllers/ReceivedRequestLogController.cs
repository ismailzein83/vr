using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Vanrise.Common.Business;

using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "ReceivedRequestLog")]
    [JSONWithTypeAttribute]
    public class ReceivedRequestLogController : BaseAPIController
    {
        [HttpGet]
        [Route("GetReceivedRequestLogFilterModuleConfigs")]
        public IEnumerable<VRReceivedRequestLogModuleFilterConfig> GetReceivedRequestLogFilterModuleConfigs()
        {
            ReceivedRequestLogManager manager = new ReceivedRequestLogManager();
            return manager.GetReceivedRequestLogFilterModuleConfigs();
        }
    }
}