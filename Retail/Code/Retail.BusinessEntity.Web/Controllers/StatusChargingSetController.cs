using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.Entities.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "StatusChargingSet")]
    [JSONWithTypeAttribute]
    public class StatusChargingSetController : BaseAPIController
    {
        StatusChargingManager _manager = new StatusChargingManager();
        [HttpPost]
        [Route("GetFilteredStatusChargingSet")]
        public object GetFilteredStatusChargingSet(DataRetrievalInput<StatusChargingSetQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredStatusChargingSet(input));
        }
        [HttpPost]
        [Route("AddStatusChargingSet")]
        public InsertOperationOutput<StatusChargingSet> AddStatusChargingSet(StatusChargingSet statusChargingSetItem)
        {
            return _manager.AddStatusChargingSet(statusChargingSetItem);
        }
        [HttpGet]
        [Route("GetStatusChargingSet")]
        public StatusChargingSet GetStatusChargingSet(int statusChargingSetId)
        {
            return _manager.GetChargingSet(statusChargingSetId);
        }
        [HttpPost]
        [Route("UpdateStatusChargingSet")]
        public UpdateOperationOutput<StatusChargingSetDetail> UpdateStatusChargingSet(StatusChargingSet statusChargingSet)
        {
            return _manager.UpdateStatusChargingSet(statusChargingSet);
        }
        [HttpGet]
        [Route("GetStatusChargeInfos")]
        public List<EntityStatusChargeInfo> GetStatusChargeInfos(int entityTypeId)
        {
            return _manager.GetStatusChargeInfos(entityTypeId);
        }
    }
}