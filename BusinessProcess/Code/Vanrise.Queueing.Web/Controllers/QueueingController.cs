using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing.Web.Controllers
{
    public class QueueingController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        public List<QueueItemType> GetQueueItemTypes()
        {
            return new QueueingManager().GetQueueItemTypes();
        }
    }
}