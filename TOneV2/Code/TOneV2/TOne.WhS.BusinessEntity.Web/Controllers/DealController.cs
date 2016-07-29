using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "Deal")]
    public class DealController : Vanrise.Web.Base.BaseAPIController
    {
        DealManager _manager = new DealManager();

        [HttpPost]
        [Route("GetFilteredDeals")]
        public object GetFilteredDeals(Vanrise.Entities.DataRetrievalInput<DealQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredDeals(input));
        }

        [HttpGet]
        [Route("GetDealsInfo")]
        public IEnumerable<DealInfo> GetDealsInfo()
        {
            return _manager.GetDealsInfo();
        }
        [HttpGet]
        [Route("GetDeal")]
        public Deal GetDeal(int dealId)
        {
            return _manager.GetDeal(dealId);
        }

        [HttpPost]
        [Route("AddDeal")]
        public Vanrise.Entities.InsertOperationOutput<DealDetail> AddDeal(Deal deal)
        {
            return _manager.AddDeal(deal);
        }

        [HttpPost]
        [Route("UpdateDeal")]
        public Vanrise.Entities.UpdateOperationOutput<DealDetail> UpdateDeal(Deal deal)
        {
            return _manager.UpdateDeal(deal);
        }
    }
}