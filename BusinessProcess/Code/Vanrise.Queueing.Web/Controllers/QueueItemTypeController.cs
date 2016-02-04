using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Vanrise.Queueing.Entities;
using Vanrise.Queueing.Web.ModelMappers;
using Vanrise.Queueing.Web.Models;

namespace Vanrise.Queueing.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "QueueItemType")]
    public class QueueItemTypeController : Vanrise.Web.Base.BaseAPIController
    {

        private QueueItemTypeManager _manager;
        public QueueItemTypeController()
        {
            this._manager = new QueueItemTypeManager();
        }

        

        [HttpGet]
        [Route("GetItemTypes")]
        public IEnumerable<QueueItemTypeInfo> GetItemTypes(string filter = null)
        {
            QueueItemTypeFilter deserializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<QueueItemTypeFilter>(filter) : null;
            return _manager.GetItemTypes(deserializedFilter);
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