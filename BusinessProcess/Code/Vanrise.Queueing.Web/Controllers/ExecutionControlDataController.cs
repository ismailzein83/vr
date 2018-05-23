using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Vanrise.Queueing.Entities;
using Vanrise.Queueing.Web.ModelMappers;
using Vanrise.Queueing.Web.Models;

namespace Vanrise.Queueing.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "ExecutionControlData")]
    public class ExecutionControlDataController : Vanrise.Web.Base.BaseAPIController
    {
        ExecutionControlManager _manager = new ExecutionControlManager();

        [HttpGet]
        [Route("IsExecutionPaused")]
        public bool IsExecutionPaused()
        {
            return _manager.IsExecutionPaused();
        }

        [HttpGet]
        [Route("UpdateExecutionPaused")]
        public bool UpdateExecutionPaused(bool isPaused)
        {
            return _manager.UpdateExecutionPaused(isPaused);
        }

    }
}