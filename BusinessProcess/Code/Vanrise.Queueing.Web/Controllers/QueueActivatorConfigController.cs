using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Vanrise.Queueing.Entities;
using Vanrise.Queueing.Web.ModelMappers;
using Vanrise.Queueing.Web.Models;

namespace Vanrise.Queueing.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "QueueActivatorConfig")]
    public class QueueActivatorConfigController : Vanrise.Web.Base.BaseAPIController
    {

        private QueueActivatorConfigManager _manager;
        public QueueActivatorConfigController()
        {
            this._manager = new QueueActivatorConfigManager();
        }


        [HttpGet]
        [Route("GetQueueActivatorsConfig")]
        public IEnumerable<QueueActivatorConfig> GetQueueActivatorsConfig()
        {

            return _manager.GetQueueActivatorsConfig();
        }


    }
}