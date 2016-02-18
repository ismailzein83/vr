using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Vanrise.Queueing.Entities;
using Vanrise.Queueing.Web.ModelMappers;
using Vanrise.Queueing.Web.Models;

namespace Vanrise.Queueing.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "QueueItemHeader")]
    public class QueueItemHeaderController : Vanrise.Web.Base.BaseAPIController
    {


        private QueueItemHeaderManager _manager;

        public QueueItemHeaderController() 
        {
            _manager = new QueueItemHeaderManager();
        }


        [HttpPost]
        [Route("GetFilteredQueueItemHeader")]
        public object GetFilteredQueueItemHeader(Vanrise.Entities.DataRetrievalInput<QueueItemHeaderQuery> input)
        {

            return GetWebResponse(input, _manager.GetFilteredQueueItemHeader(input));
        }


        [HttpGet]
        [Route("GetItemStatusSummary")]
        public List<QueueItemStatusSummary> GetItemStatusSummary()
        {
            return _manager.GetItemStatusSummary();
        }


        [HttpGet]
        [Route("GetExecutionFlowStatusSummary")]
        public IEnumerable<ExecutionFlowStatusSummary> GetExecutionFlowStatusSummary()
        {
            return _manager.GetExecutionFlowStatusSummary();
        }


    }
}