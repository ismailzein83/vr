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

        private QueueExecutionFlowManager _manager;
        public ExecutionFlowController()
        {
            this._manager = new QueueExecutionFlowManager();
        }

        
        [HttpGet]
        [Route("GetExecutionFlows")]
        public IEnumerable<QueueExecutionFlowInfo> GetExecutionFlowsInfo(string filter = null)
        {
            QueueExecutionFlowFilter deserializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<QueueExecutionFlowFilter>(filter) : null;
            return _manager.GetExecutionFlowsInfo(deserializedFilter);

        }



        [HttpGet]
        [Route("GetExecutionFlow")]
        public QueueExecutionFlow GetExecutionFlow(Guid executionFlowId)
        {

            return _manager.GetExecutionFlow(executionFlowId);

        }


        [HttpPost]
        [Route("UpdateExecutionFlow")]
        public Vanrise.Entities.UpdateOperationOutput<QueueExecutionFlowDetail> UpdateExecutionFlow(QueueExecutionFlow executionFlowObject)
        {

            return _manager.UpdateExecutionFlow(executionFlowObject);

        }


        [HttpPost]
        [Route("AddExecutionFlow")]
        public Vanrise.Entities.InsertOperationOutput<QueueExecutionFlowDetail> AddExecutionFlow(QueueExecutionFlow executionFlowObject)
        {

            return _manager.AddExecutionFlow(executionFlowObject);

        }

        [HttpPost]
        [Route("GetFilteredExecutionFlows")]
        public object GetFilteredExecutionFlows(Vanrise.Entities.DataRetrievalInput<QueueExecutionFlowQuery> input)
        {

            return GetWebResponse(input, _manager.GetFilteredExecutionFlows(input));

        }

    }
}