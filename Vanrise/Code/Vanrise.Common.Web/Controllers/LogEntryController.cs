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
    [RoutePrefix(Constants.ROUTE_PREFIX + "LogEntry")]
    public class VRCommon_LogEntryController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredLogs")]
        public object GetFilteredLogs(Vanrise.Entities.DataRetrievalInput<LogEntryQuery> input)
        {
            LogManager manager = new LogManager();
            return GetWebResponse(input, manager.GetFilteredLogs(input), "Log Entries");
        }

    }
}