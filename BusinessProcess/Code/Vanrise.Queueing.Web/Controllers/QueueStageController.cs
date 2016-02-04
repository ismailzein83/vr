using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Vanrise.Queueing.Entities;
using Vanrise.Queueing.Web.ModelMappers;
using Vanrise.Queueing.Web.Models;

namespace Vanrise.Queueing.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "QueueStage")]
    public class QueueStageController : Vanrise.Web.Base.BaseAPIController
    {
        private readonly QueueStageManager _manager;
        public QueueStageController()
        {
            _manager = new QueueStageManager();
        }

        [HttpGet]
        [Route("GetStageNames")]
        public IEnumerable<QueueStageNameInfo> GetStageNames(string filter = null)
        {
            QueueStageFilter deserializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<QueueStageFilter>(filter) : null;
            return _manager.GetStageNames(deserializedFilter);
        }


    }
        
}