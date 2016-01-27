﻿using System;
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
        [Route("GetFilteredExecutionFlowDefinitions")]
        public List<QueueExecutionFlowDefinition> GetFilteredExecutionFlowDefinitions()
        {
            QueueExecutionFlowDefinitionManager manager = new QueueExecutionFlowDefinitionManager();

            return manager.GetAll();

        }

        [HttpGet]
        [Route("GetExecutionFlow")]
        public QueueExecutionFlow GetExecutionFlow(int executionFlowId)
        {
            QueueExecutionFlowManager manager = new QueueExecutionFlowManager();
            return manager.GetExecutionFlow(executionFlowId);

        }


        [HttpPost]
        [Route("UpdateExecutionFlow")]
        public Vanrise.Entities.UpdateOperationOutput<QueueExecutionFlowDetail> UpdateExecutionFlow(QueueExecutionFlow executionFlowObject)
        {

            return _manager.UpdateExecutionFlow(executionFlowObject);

        }


        [HttpPost]
        [Route("GetFilteredExecutionFlows")]
        public object GetFilteredExecutionFlows(Vanrise.Entities.DataRetrievalInput<QueueExecutionFlowQuery> input)
        {

            return GetWebResponse(input, _manager.GetFilteredExecutionFlows(input));

        }

    }
}