using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Vanrise.Queueing.Entities;
using Vanrise.Queueing.Web.ModelMappers;
using Vanrise.Queueing.Web.Models;

namespace Vanrise.Queueing.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "ExecutionFlowDefinition")]
    public class ExecutionFlowDefinitionController : Vanrise.Web.Base.BaseAPIController
    {

        private QueueExecutionFlowDefinitionManager _manager;
        public ExecutionFlowDefinitionController()
        {
            this._manager = new QueueExecutionFlowDefinitionManager();
        }

        [HttpGet]
        [Route("GetExecutionFlowDefinitions")]
        public IEnumerable<QueueExecutionFlowDefinitionInfo> GetExecutionFlowDefinitions(string filter = null)
        {

            QueueExecutionFlowDefinitionFilter deserializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<QueueExecutionFlowDefinitionFilter>(filter) : null;
            return _manager.GetExecutionFlowDefinitions(deserializedFilter);

        }


        [HttpPost]
        [Route("GetFilteredExecutionFlowDefinitions")]
        public object GetFilteredExecutionFlowDefinitions(Vanrise.Entities.DataRetrievalInput<QueueExecutionFlowDefinitionQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredExecutionFlowDefinitions(input));
        }

        [HttpGet]
        [Route("GetExecutionFlowDefinition")]
        public QueueExecutionFlowDefinition GetExecutionFlowDefinition(int executionFlowDefinitionId)
        {

            return _manager.GetExecutionFlowDefinition(executionFlowDefinitionId);

        }


        [HttpPost]
        [Route("UpdateExecutionFlowDefinition")]
        public Vanrise.Entities.UpdateOperationOutput<QueueExecutionFlowDefinitionDetail> UpdateExecutionFlowDefinition(QueueExecutionFlowDefinition executionFlowDefinitionObject)
        {

            return _manager.UpdateExecutionFlowDefinition(executionFlowDefinitionObject);

        }


        [HttpPost]
        [Route("AddExecutionFlowDefinition")]
        public Vanrise.Entities.InsertOperationOutput<QueueExecutionFlowDefinitionDetail> AddExecutionFlowDefinition(QueueExecutionFlowDefinition executionFlowDefinitionObject)
        {

            return _manager.AddExecutionFlowDefinition(executionFlowDefinitionObject);

        }

    }
}