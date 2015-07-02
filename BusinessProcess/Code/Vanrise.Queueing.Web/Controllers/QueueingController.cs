using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Queueing.Entities;
using Vanrise.Queueing.Web.ModelMappers;
using Vanrise.Queueing.Web.Models;

namespace Vanrise.Queueing.Web.Controllers
{
    public class QueueingController : Vanrise.Web.Base.BaseAPIController
    {
        private readonly QueueingManager _queueingManager;
        public QueueingController()
        {
            _queueingManager = new QueueingManager();
        }

        [HttpGet]
        public List<QueueItemType> GetQueueItemTypes()
        {
            return _queueingManager.GetQueueItemTypes();
        }

        [HttpPost]
        public List<QueueInstanceModel> GetQueueInstances(IEnumerable<int> queueItemTypes)
        {
            return QueueingMappers.MapQueueInstances(_queueingManager.GetQueueInstances(queueItemTypes));
        }

    }
}