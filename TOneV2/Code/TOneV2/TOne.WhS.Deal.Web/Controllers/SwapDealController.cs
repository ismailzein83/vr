using System.Collections.Generic;
using System.Web.Http;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.Web.Base;
using TOne.WhS.Deal.Entities.Settings;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Deal.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "SwapDeal")]
    public class SwapDealController : Vanrise.Web.Base.BaseAPIController
    {
        SwapDealManager _manager = new SwapDealManager();

        [HttpPost]
        [Route("GetFilteredSwapDeals")]
        public object GetFilteredDeals(Vanrise.Entities.DataRetrievalInput<SwapDealQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredSwapDeals(input), "Swap Deals");
        }

        [HttpGet]
        [Route("GetSwapDealHistoryDetailbyHistoryId")]
        public DealDefinition GetSwapDealHistoryDetailbyHistoryId(int swapDealHistoryId)
        {
            return _manager.GetSwapDealHistoryDetailbyHistoryId(swapDealHistoryId);
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
        [HttpPost]
        [Route("AddDeal")]
        public Vanrise.Entities.InsertOperationOutput<DealDefinitionDetail> AddDeal(DealDefinition deal)
        {
            return _manager.AddDeal(deal);
        }

        [HttpGet]
        [Route("GetSwapDealSettingData")]
        public SwapDealSettingData GetSwapDealSettingData()
        {
            return _manager.GetSwapDealSettingData();
        }
        
        [HttpGet]
        [Route("GetSwapDealSettingsDetail")]
        public SwapDealSettingsDetail GetSwapDealSettingsDetail(int dealId)
        {
            return _manager.GetSwapDealSettingsDetail(dealId);
        }

        [HttpGet]
        [Route("RecurDeal")]
        public InsertDealOperationOutput<RecurredDealItem> RecurDeal(int dealId, int recurringNumber, RecurringType recurringType)
        {
            return _manager.RecurDeal(dealId, recurringNumber, recurringType);
        }
    }
}