using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.Web.Base;
using Vanrise.Entities;

namespace TOne.WhS.Deal.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "DealDefinition")]
    public class DealDefinitionController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredSwapDeals")]
        public object GetFilteredDeals(Vanrise.Entities.DataRetrievalInput<SwapDealQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredSwapDeals(input));
        }
        [HttpGet]
        [Route("GetDeal")]
        public DealDefinition GetDeal(int dealId)
        {
            return _manager.GetDeal(dealId);
        }

        [HttpPost]
        [Route("UpdateDeal")]
        public Vanrise.Entities.UpdateOperationOutput<DealDefinitionDetail> UpdateDeal(DealDefinition deal)
        {
            return _manager.UpdateDeal(deal);
        }

        DealManager _manager = new DealManager();
        [HttpPost]
        [Route("AddDeal")]
        public Vanrise.Entities.InsertOperationOutput<DealDefinitionDetail> AddDeal(DealDefinition deal)
        {
            return _manager.AddDeal(deal);
        }
    }
}