using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Vanrise.Queueing.Entities;
using Vanrise.Queueing.Web.ModelMappers;
using Vanrise.Queueing.Web.Models;

namespace Vanrise.Queueing.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "QueueInstance")]
    public class QueueInstanceController : Vanrise.Web.Base.BaseAPIController
    {

        private QueueInstanceManager _manager;
        public QueueInstanceController()
        {
            this._manager = new QueueInstanceManager();
        }

        [HttpGet]
        [Route("GetStageNames")]
        public IEnumerable<QueueStageNameInfo> GetStageNames(string filter = null)
        {
            QueueStageFilter deserializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<QueueStageFilter>(filter) : null;
            return _manager.GetStageNames(deserializedFilter);
        }

        [HttpGet]
        [Route("GetItemTypes")]
        public IEnumerable<QueueItemTypeInfo> GetItemTypes(string filter = null)
        {
            QueueItemTypeManager manager = new QueueItemTypeManager();
            QueueItemTypeFilter deserializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<QueueItemTypeFilter>(filter) : null;
            return manager.GetItemTypes(deserializedFilter);
        }

        [HttpPost]
        [Route("GetFilteredQueueInstances")]
        public object GetFilteredQueueInstances(Vanrise.Entities.DataRetrievalInput<QueueInstanceQuery> input)
        {

            return GetWebResponse(input, _manager.GetFilteredQueueInstances(input));
        }


        [HttpGet]
        [Route("GetItemStatusSummary")]
        public List<QueueItemStatusSummary> GetItemStatusSummary()
        {
            QueueItemManager manager = new QueueItemManager();
            return manager.GetItemStatusSummary();
        }


    }
}