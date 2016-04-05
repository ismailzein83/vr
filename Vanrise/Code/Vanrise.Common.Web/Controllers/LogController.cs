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
        [Route("GetSpecificLogAttribute")]
        public List<LogAttribute> GetSpecificLogAttribute(int attribute)
        {
            LoggingManager manager = new LoggingManager();
            return manager.GetSpecificLogAttribute(attribute);
        }

        [HttpPost]
        [Route("GetFilteredLoggers")]
        public object GetFilteredLoggers(Vanrise.Entities.DataRetrievalInput<LogEntryQuery> input)
        {
            LoggingManager manager = new LoggingManager();
            return GetWebResponse(input, manager.GetFilteredLoggers(input));
        }

    }
}