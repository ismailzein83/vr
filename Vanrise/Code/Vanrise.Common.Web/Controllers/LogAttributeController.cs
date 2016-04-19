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
    [RoutePrefix(Constants.ROUTE_PREFIX + "LogAttribute")]
    public class VRCommon_LogAttributeController : BaseAPIController
    {

        [HttpGet]
        [Route("GetLogAttributesById")]
        public IEnumerable<LogAttribute> GetLogAttributesById(int attribute)
        {
            LogManager manager = new LogManager();
            return manager.GeLogAttributesById(attribute);
        }

    }
}