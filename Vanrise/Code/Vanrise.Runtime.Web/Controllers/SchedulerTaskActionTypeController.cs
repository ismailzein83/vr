using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Runtime.Business;
using Vanrise.Runtime.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Runtime.Web.Controllers
{
    [Vanrise.Web.Base.JSONWithType]
    [RoutePrefix(Constants.ROUTE_PREFIX + "SchedulerTaskActionType")]
    public class SchedulerTaskActionTypeController : Vanrise.Web.Base.BaseAPIController
    {

        [HttpGet]
        [Route("GetSchedulerTaskActionTypes")]
        public List<SchedulerTaskActionType> GetSchedulerTaskActionTypes()
        {
            SchedulerTaskManager manager = new SchedulerTaskManager();
            return manager.GetSchedulerTaskActionTypes();
        }
    }
}