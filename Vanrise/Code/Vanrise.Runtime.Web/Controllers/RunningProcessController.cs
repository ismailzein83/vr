using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Runtime.Entities;
using Vanrise.Web.Base;
using Vanrise.Runtime;
using Vanrise.Entities;


namespace Vanrise.Runtime.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RunningProcess")]
    [JSONWithTypeAttribute]
    public class RunningProcessController : BaseAPIController
    {


        [HttpPost]
        [Route("GetFilteredRunningProcesses")]
        public object GetFilteredRunningProcesses(Vanrise.Entities.DataRetrievalInput<RunningProcessQuery> input)
        {
            RunningProcessManager manager = new RunningProcessManager();
            return GetWebResponse(input, manager.GetFilteredRunningProcesses(input));
        }
    }
}