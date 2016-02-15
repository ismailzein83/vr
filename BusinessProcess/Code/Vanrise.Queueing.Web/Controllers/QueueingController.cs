using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Vanrise.Queueing.Entities;
using Vanrise.Queueing.Web.ModelMappers;
using Vanrise.Queueing.Web.Models;

namespace Vanrise.Queueing.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Queueing")]
    public class QueueingController : Vanrise.Web.Base.BaseAPIController
    {
        private readonly QueueingManager _queueingManager;
        public QueueingController()
        {
            _queueingManager = new QueueingManager();
        }

        [HttpGet]
        [Route("GetQueueItemTypes")]
        public List<QueueItemType> GetQueueItemTypes()
        {
            return _queueingManager.GetQueueItemTypes();
        }

        [HttpGet]
        [Route("GetItemStatusList")]
        public List<EnumModel> GetItemStatusList()
        {
            var lst = new List<EnumModel>();
            foreach (var val in Enum.GetValues(typeof(QueueItemStatus)))
            {
                EnumModel item = new EnumModel
                {
                    Value = (int)val,
                    Description = ((QueueItemStatus)val).ToString()
                };
                lst.Add(item);
            }
            return lst;
        }

        [HttpPost]
        [Route("GetQueueInstances")]
        public IEnumerable<QueueInstanceModel> GetQueueInstances(IEnumerable<int> queueItemTypes)
        {
            QueueInstanceManager queueManager = new QueueInstanceManager();
            return QueueingMappers.MapQueueInstances(queueManager.GetQueueInstances(queueItemTypes));
        }

        [HttpPost]
        [Route("GetHeaders")]
        public IEnumerable<QueueItemHeaderModel> GetHeaders(GetHeadersInput param)
        {
            param.FromRow = param.FromRow - 1;
            IEnumerable<QueueItemHeaderModel> rows = QueueingMappers.MapQueueItemHeaders(_queueingManager.GetHeaders(param.QueueIds, param.Statuses, param.DateFrom ?? DateTime.Now.AddHours(-1), param.DateTo ?? DateTime.Now));
            return rows.Skip(param.FromRow).Take(param.ToRow - param.FromRow);
        }

        [HttpPost]
        [Route("GetQueueItemHeaders")]
        public object GetQueueItemHeaders(Vanrise.Entities.DataRetrievalInput<List<long>> input)
        {
            return GetWebResponse(input, _queueingManager.GetQueueItemsHeader(input));
        }
    }

    #region Argument Classes

    public class GetHeadersInput
    {
        public int FromRow { get; set; }
        public int ToRow { get; set; }
        public IEnumerable<int> QueueIds { get; set; }
        public IEnumerable<QueueItemStatus> Statuses { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
    

    #endregion
}