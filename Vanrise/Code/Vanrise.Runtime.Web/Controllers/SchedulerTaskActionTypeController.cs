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
        public IEnumerable<SchedulerTaskActionType> GetSchedulerTaskActionTypes(string filter = null)
        {
            SchedulerTaskActionTypeFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<SchedulerTaskActionTypeFilter>(filter) : null;
            SchedulerTaskActionTypeManager manager = new SchedulerTaskActionTypeManager();
            return manager.GetSchedulerTaskActionTypes(deserializedFilter);
        }
    }
}