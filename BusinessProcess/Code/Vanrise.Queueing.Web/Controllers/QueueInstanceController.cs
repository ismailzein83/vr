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
        public List<StageName> GetStageNames()
        {
            return _manager.GetStageNames();
        }

        [HttpGet]
        [Route("GetItemTypes")]
        public List<QueueItemType> GetItemTypes() 
        {
            QueueItemTypeManager manager = new QueueItemTypeManager();
            return manager.GetItemTypes();
        }


        public object GetFilteredQueueInstances(Vanrise.Entities.DataRetrievalInput<QueueInstanceQuery> input) {

            return GetWebResponse(input, _manager.GetFilteredQueueInstances(input));
        }
       

    }
}