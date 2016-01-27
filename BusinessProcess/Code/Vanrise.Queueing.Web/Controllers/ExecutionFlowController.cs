using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Vanrise.Queueing.Entities;
using Vanrise.Queueing.Web.ModelMappers;
using Vanrise.Queueing.Web.Models;

namespace Vanrise.Queueing.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "ExecutionFlow")]
    public class ExecutionFlowController : Vanrise.Web.Base.BaseAPIController
    {

         private  QueueExecutionFlowManager _manager;
        public ExecutionFlowController()
        {
            this._manager = new QueueExecutionFlowManager();
        }

        [HttpPost]
        [Route("GetFilteredExecutionFlowDefinitions")]
        public List<QueueExecutionFlowDefinition> GetFilteredExecutionFlowDefinitions()
        {

            return new List<QueueExecutionFlowDefinition>();

        }


        [HttpPost]
        [Route("GetFilteredExecutionFlows")]
        public object GetFilteredExecutionFlows()
        {
            return new object();
        }

    }
}