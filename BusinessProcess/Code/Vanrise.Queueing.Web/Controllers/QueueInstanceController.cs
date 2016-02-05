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

      
        [HttpGet]
        [Route("GetQueueInstances")]
        public IEnumerable<QueueInstanceInfo> GetQueueInstances(string filter = null)
        {
            
            QueueInstanceFilter deserializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<QueueInstanceFilter>(filter) : null;
            return _manager.GetQueueInstances(deserializedFilter);
        }


    }
}